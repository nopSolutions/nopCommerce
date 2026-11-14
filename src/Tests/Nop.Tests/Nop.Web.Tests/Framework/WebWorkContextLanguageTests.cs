using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Security;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Framework;

[TestFixture]
public class WebWorkContextLanguageTests
{
    private static (
        Mock<WebWorkContext> WorkContext,
        Mock<ICustomerService> CustomerService,
        Mock<IEventPublisher> EventPublisher,
        Mock<IStoreContext> StoreContext,
        DefaultHttpContext HttpContext)
        CreateWorkContext(Customer currentCustomer, Customer originalCustomer = null)
    {
        var customerService = new Mock<ICustomerService>();
        var eventPublisher = new Mock<IEventPublisher>();
        var storeContext = new Mock<IStoreContext>();
        var httpContext = new DefaultHttpContext();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

        var workContext = new Mock<WebWorkContext>(
            new CookieSettings(),
            new CurrencySettings(),
            Mock.Of<IAuthenticationService>(),
            Mock.Of<ICurrencyService>(),
            customerService.Object,
            eventPublisher.Object,
            Mock.Of<IGenericAttributeService>(),
            httpContextAccessor.Object,
            Mock.Of<ILanguageService>(),
            storeContext.Object,
            Mock.Of<IUserAgentHelper>(),
            Mock.Of<IVendorService>(),
            Mock.Of<IWebHelper>(),
            new LocalizationSettings(),
            new TaxSettings())
        {
            CallBase = true
        };

        workContext.Setup(context => context.GetCurrentCustomerAsync()).ReturnsAsync(currentCustomer);
        workContext.SetupGet(context => context.OriginalCustomerIfImpersonated).Returns(originalCustomer);
        return (workContext, customerService, eventPublisher, storeContext, httpContext);
    }

    [Test]
    public async Task SetWorkingLanguageAsync_WhenImpersonating_DoesNotOverwriteCustomerLanguage()
    {
        var admin = new Customer { Id = 100, LanguageId = 1 };
        var customer = new Customer { Id = 101, LanguageId = 2 };
        var english = new Language { Id = 1, LanguageCulture = "en-US" };
        var (context, customerService, eventPublisher, _, httpContext) = CreateWorkContext(customer, admin);

        await context.Object.SetWorkingLanguageAsync(english);

        Assert.That(customer.LanguageId, Is.EqualTo(2));
        customerService.Verify(service => service.UpdateCustomerAsync(It.IsAny<Customer>()), Times.Never);
        eventPublisher.Verify(publisher => publisher.PublishAsync(It.IsAny<CustomerChangeWorkingLanguageEvent>()), Times.Never);
        Assert.That(httpContext.Response.Headers.SetCookie.ToString(), Is.Not.Empty,
            "A temporary session language cookie should still be written.");
    }

    [Test]
    public async Task SetWorkingLanguageAsync_ForOrdinaryCustomer_PersistsLanguageAndPublishesEvent()
    {
        var customer = new Customer { Id = 101, LanguageId = 2 };
        var english = new Language { Id = 1, LanguageCulture = "en-US" };
        var (context, customerService, eventPublisher, _, httpContext) = CreateWorkContext(customer);

        await context.Object.SetWorkingLanguageAsync(english);

        Assert.That(customer.LanguageId, Is.EqualTo(1));
        customerService.Verify(service => service.UpdateCustomerAsync(customer), Times.Once);
        eventPublisher.Verify(publisher => publisher.PublishAsync(It.IsAny<CustomerChangeWorkingLanguageEvent>()), Times.Once);
        Assert.That(httpContext.Response.Headers.SetCookie.ToString(), Is.Not.Empty);
    }

    [Test]
    public async Task GetWorkingLanguageAsync_WhenImpersonating_UsesRequestCultureWithoutChangingCustomerPreference()
    {
        var admin = new Customer { Id = 100, LanguageId = 1 };
        var customer = new Customer { Id = 101, LanguageId = 2 };
        var english = new Language { Id = 1, LanguageCulture = "en-US" };
        var (context, customerService, eventPublisher, storeContext, _) = CreateWorkContext(customer, admin);
        storeContext.Setup(service => service.GetCurrentStoreAsync()).ReturnsAsync(new Store { Id = 1 });
        context.Protected()
            .Setup<Task<Language>>("GetLanguageFromRequestAsync", ItExpr.IsAny<int>())
            .ReturnsAsync(english);

        var workingLanguage = await context.Object.GetWorkingLanguageAsync();

        Assert.That(workingLanguage.Id, Is.EqualTo(1));
        Assert.That(customer.LanguageId, Is.EqualTo(2));
        customerService.Verify(service => service.UpdateCustomerAsync(It.IsAny<Customer>()), Times.Never);
        eventPublisher.Verify(publisher => publisher.PublishAsync(It.IsAny<CustomerChangeWorkingLanguageEvent>()), Times.Never);
    }
}
