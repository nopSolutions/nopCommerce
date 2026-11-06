using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Moq;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Security;
using Nop.Data;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Framework;

// Uses the test fixture's isolated database and real services for customers,
// generic attributes, languages, orders, and queued email notifications.
// No storefront HTTP checkout or outbound SMTP calls are performed.
[TestFixture]
[NonParallelizable]
public class ImpersonationOrderEmailIntegrationTests : BaseNopTest
{
    [Test]
    public async Task EnglishAdministratorImpersonatingFrenchCustomer_PreservesFrenchPreferenceInOrderAndEmail()
    {
        var customers = GetService<ICustomerService>();
        var languages = GetService<ILanguageService>();
        var attributes = GetService<IGenericAttributeService>();
        var addresses = GetService<IAddressService>();
        var orders = GetService<IOrderService>();
        var messages = GetService<IWorkflowMessageService>();
        var templates = GetService<IMessageTemplateService>();
        var localized = GetService<ILocalizedEntityService>();
        var queuedEmails = GetService<IQueuedEmailService>();
        var stores = GetService<IStoreService>();

        var store = await stores.GetStoreByIdAsync(1);
        var administrator = await customers.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
        Assert.Multiple(() =>
        {
            Assert.That(store, Is.Not.Null);
            Assert.That(administrator, Is.Not.Null);
        });

        var french = new Language
        {
            Name = "French (impersonation regression test)",
            LanguageCulture = "fr-FR",
            UniqueSeoCode = "fr-regression",
            FlagImageFileName = "fr.png",
            Published = true,
            LimitedToStores = false
        };
        Customer customer = null;
        Address address = null;
        Order order = null;
        var queuedIds = new List<int>();
        MessageTemplate template = null;
        var templateLocalized = false;
        var impersonationSet = false;

        try
        {
            await languages.InsertLanguageAsync(french);
            customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = $"impersonation-{Guid.NewGuid():N}@example.invalid",
                Username = $"impersonation-{Guid.NewGuid():N}",
                Active = true,
                LanguageId = french.Id,
                RegisteredInStoreId = store.Id,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };
            await customers.InsertCustomerAsync(customer);

            address = new Address
            {
                FirstName = "French",
                LastName = "Customer",
                Email = customer.Email,
                City = "Paris",
                Address1 = "Test address",
                ZipPostalCode = "75001",
                CreatedOnUtc = DateTime.UtcNow
            };
            await addresses.InsertAddressAsync(address);
            customer.BillingAddressId = address.Id;
            await customers.UpdateCustomerAsync(customer);

            await attributes.SaveAttributeAsync<int?>(administrator, NopCustomerDefaults.ImpersonatedCustomerIdAttribute, customer.Id);
            impersonationSet = true;

            var authentication = new Mock<IAuthenticationService>();
            authentication.Setup(service => service.GetAuthenticatedCustomerAsync()).ReturnsAsync(administrator);
            var storeContext = new Mock<IStoreContext>();
            storeContext.Setup(service => service.GetCurrentStoreAsync()).ReturnsAsync(store);
            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<IRequestCultureFeature>(
                new RequestCultureFeature(new RequestCulture("en-US"), new CookieRequestCultureProvider()));
            var accessor = new Mock<IHttpContextAccessor>();
            accessor.SetupGet(a => a.HttpContext).Returns(httpContext);

            WebWorkContext CreateContext() => new(
                new CookieSettings(),
                new CurrencySettings(),
                authentication.Object,
                Mock.Of<ICurrencyService>(),
                customers,
                Mock.Of<IEventPublisher>(),
                attributes,
                accessor.Object,
                languages,
                storeContext.Object,
                Mock.Of<IUserAgentHelper>(),
                Mock.Of<IVendorService>(),
                Mock.Of<IWebHelper>(),
                new LocalizationSettings { AutomaticallyDetectLanguage = true },
                new TaxSettings());

            var impersonatedContext = CreateContext();
            var activeCustomer = await impersonatedContext.GetCurrentCustomerAsync();
            Assert.That(activeCustomer.Id, Is.EqualTo(customer.Id));
            Assert.That(impersonatedContext.OriginalCustomerIfImpersonated?.Id, Is.EqualTo(administrator.Id));

            var sessionLanguage = await impersonatedContext.GetWorkingLanguageAsync();
            Assert.That(sessionLanguage.LanguageCulture, Is.EqualTo("en-US"),
                "The administrator's session culture may be English.");
            var storedCustomer = await customers.GetCustomerByIdAsync(customer.Id);
            Assert.That(storedCustomer.LanguageId, Is.EqualTo(french.Id),
                "Neither language auto-detection nor impersonation may change the customer's preference.");

            // Create a persisted order representing the impersonated customer's purchase.
            // The production checkout path reads the same Customer.LanguageId and
            // sends its confirmation with Order.CustomerLanguageId.
            order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                CustomOrderNumber = $"IMP-TEST-{Guid.NewGuid():N}",
                StoreId = store.Id,
                CustomerId = customer.Id,
                BillingAddressId = address.Id,
                CustomerLanguageId = storedCustomer.LanguageId.Value,
                CustomerCurrencyCode = "USD",
                CurrencyRate = 1m,
                CreatedOnUtc = DateTime.UtcNow
            };
            await orders.InsertOrderAsync(order);
            var persistedOrder = await orders.GetOrderByIdAsync(order.Id);
            Assert.That(persistedOrder.CustomerLanguageId, Is.EqualTo(french.Id));

            template = (await templates.GetMessageTemplatesByNameAsync(
                MessageTemplateSystemNames.ORDER_PLACED_CUSTOMER_NOTIFICATION, store.Id)).First();
            Assert.That(template.IsActive, Is.True, "The test needs an active order confirmation template.");
            await localized.SaveLocalizedValueAsync(template, item => item.Subject,
                "Confirmation de commande — test", french.Id);
            await localized.SaveLocalizedValueAsync(template, item => item.Body,
                "Votre commande a été enregistrée.", french.Id);
            templateLocalized = true;

            queuedIds.AddRange(await messages.SendOrderPlacedCustomerNotificationAsync(
                persistedOrder, persistedOrder.CustomerLanguageId));
            Assert.That(queuedIds, Is.Not.Empty, "A French confirmation must be queued.");
            foreach (var id in queuedIds)
            {
                var email = await queuedEmails.GetQueuedEmailByIdAsync(id);
                Assert.Multiple(() =>
                {
                    Assert.That(email.Subject, Does.Contain("Confirmation de commande"));
                    Assert.That(email.Body, Does.Contain("Votre commande"));
                    Assert.That(email.To, Is.EqualTo(customer.Email));
                });
            }

            await attributes.SaveAttributeAsync<int?>(administrator,
                NopCustomerDefaults.ImpersonatedCustomerIdAttribute, null);
            impersonationSet = false;

            // A fresh HTTP request should resolve the administrator again after logout.
            var afterLogout = CreateContext();
            var authenticatedCustomer = await afterLogout.GetCurrentCustomerAsync();
            Assert.That(authenticatedCustomer.Id, Is.EqualTo(administrator.Id));
            Assert.That(afterLogout.OriginalCustomerIfImpersonated, Is.Null);
            storedCustomer = await customers.GetCustomerByIdAsync(customer.Id);
            Assert.That(storedCustomer.LanguageId, Is.EqualTo(french.Id));
        }
        finally
        {
            if (impersonationSet)
                await attributes.SaveAttributeAsync<int?>(administrator,
                    NopCustomerDefaults.ImpersonatedCustomerIdAttribute, null);
            foreach (var id in queuedIds)
                if (await queuedEmails.GetQueuedEmailByIdAsync(id) is { } email)
                    await queuedEmails.DeleteQueuedEmailAsync(email);
            if (order is { Id: > 0 })
            {
                //Orders use soft deletion. Release references to test-owned entities.
                order.BillingAddressId = 1;
                order.CustomerLanguageId = 1;
                await orders.UpdateOrderAsync(order);
                await orders.DeleteOrderAsync(order);
            }
            if (templateLocalized)
            {
                await localized.SaveLocalizedValueAsync(template, item => item.Subject, null, french.Id);
                await localized.SaveLocalizedValueAsync(template, item => item.Body, null, french.Id);
            }
            if (customer != null)
            {
                //Customer deletion is soft; clear its billing address FK first.
                customer.BillingAddressId = null;
                customer.LanguageId = null;
                customer.Active = false;
                await customers.UpdateCustomerAsync(customer);
                await customers.DeleteCustomerAsync(customer);
            }
            if (address != null)
                await addresses.DeleteAddressAsync(address);
            if (french.Id != 0)
                await languages.DeleteLanguageAsync(french);
        }
    }
}
