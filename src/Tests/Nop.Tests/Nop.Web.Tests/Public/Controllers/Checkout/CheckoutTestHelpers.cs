using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Orders;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Controllers.Checkout;

/// <summary>
/// Arrange and restore helpers for checkout controller tests.
/// </summary>
/// <remarks>
/// <see cref="CaptureStateAsync"/> snapshots what checkout tests change in the shared store
/// (the settings changed through <see cref="SetSettingsAsync{TSettings}"/>, the current customer's cart items
/// and the current customer's generic attributes, plus any sample product changed through
/// <see cref="AddSampleProductToCartAsync"/> or <see cref="UpdateProductAsync"/>, customer role mappings added through
/// <see cref="MakeCurrentCustomerGuestAsync"/> and log records written during the test);
/// <see cref="RestoreStateAsync"/> puts them back,
/// so the outcome of a test does not depend on test order.
/// </remarks>
public class CheckoutTestHelpers
{
    #region Fields

    private readonly IServiceProvider _serviceProvider;
    private readonly List<Func<Task>> _settingsRestorers = new();
    private readonly HashSet<Type> _changedSettingsTypes = new();
    private readonly List<Func<Task>> _productRestorers = new();
    private readonly List<Func<Task>> _customerRestorers = new();
    private int _lastLogIdSnapshot;
    private List<ShoppingCartItem> _cartSnapshot = new();
    private List<GenericAttribute> _genericAttributesSnapshot = new();

    #endregion

    #region Ctor

    public CheckoutTestHelpers(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    #endregion

    #region Utilities

    private T GetService<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    private static ShoppingCartItem Copy(ShoppingCartItem item)
    {
        return new ShoppingCartItem
        {
            Id = item.Id,
            StoreId = item.StoreId,
            ShoppingCartTypeId = item.ShoppingCartTypeId,
            CustomerId = item.CustomerId,
            ProductId = item.ProductId,
            CustomWishlistId = item.CustomWishlistId,
            AttributesXml = item.AttributesXml,
            CustomerEnteredPrice = item.CustomerEnteredPrice,
            Quantity = item.Quantity,
            RentalStartDateUtc = item.RentalStartDateUtc,
            RentalEndDateUtc = item.RentalEndDateUtc,
            CreatedOnUtc = item.CreatedOnUtc,
            UpdatedOnUtc = item.UpdatedOnUtc
        };
    }

    private static GenericAttribute Copy(GenericAttribute attribute)
    {
        return new GenericAttribute
        {
            Id = attribute.Id,
            EntityId = attribute.EntityId,
            KeyGroup = attribute.KeyGroup,
            Key = attribute.Key,
            Value = attribute.Value,
            StoreId = attribute.StoreId,
            CreatedOrUpdatedDateUTC = attribute.CreatedOrUpdatedDateUTC
        };
    }

    private async Task<List<ShoppingCartItem>> LoadCartItemsAsync(int customerId)
    {
        //read the table directly, so the snapshot never comes from a cache
        return await GetService<IRepository<ShoppingCartItem>>().Table
            .Where(sci => sci.CustomerId == customerId)
            .ToListAsync();
    }

    private async Task<List<GenericAttribute>> LoadGenericAttributesAsync(int customerId)
    {
        var keyGroup = nameof(Customer);

        return await GetService<IRepository<GenericAttribute>>().Table
            .Where(ga => ga.EntityId == customerId && ga.KeyGroup == keyGroup)
            .ToListAsync();
    }

    private async Task RestoreCartAsync(Customer customer)
    {
        var shoppingCartService = GetService<IShoppingCartService>();
        var cartRepository = GetService<IRepository<ShoppingCartItem>>();
        var snapshotIds = _cartSnapshot.Select(sci => sci.Id).ToHashSet();

        var current = await LoadCartItemsAsync(customer.Id);

        //remove items the test added
        foreach (var item in current.Where(sci => !snapshotIds.Contains(sci.Id)))
            await shoppingCartService.DeleteShoppingCartItemAsync(item, resetCheckoutData: false);

        //put back items the test removed or changed
        var currentById = current.ToDictionary(sci => sci.Id);
        foreach (var original in _cartSnapshot)
        {
            if (!currentById.TryGetValue(original.Id, out var item))
            {
                var restored = Copy(original);
                restored.Id = 0;
                await cartRepository.InsertAsync(restored);
                continue;
            }

            if (item.Quantity == original.Quantity && item.AttributesXml == original.AttributesXml &&
                item.CustomerEnteredPrice == original.CustomerEnteredPrice)
                continue;

            item.Quantity = original.Quantity;
            item.AttributesXml = original.AttributesXml;
            item.CustomerEnteredPrice = original.CustomerEnteredPrice;
            await cartRepository.UpdateAsync(item);
        }
    }

    private async Task RestoreLogAsync()
    {
        var logRepository = GetService<IRepository<Log>>();
        var lastLogId = _lastLogIdSnapshot;

        var added = await logRepository.Table.Where(log => log.Id > lastLogId).ToListAsync();
        if (added.Any())
            await logRepository.DeleteAsync(added);
    }

    private async Task RestoreGenericAttributesAsync(Customer customer)
    {
        var genericAttributeService = GetService<IGenericAttributeService>();
        var snapshotById = _genericAttributesSnapshot.ToDictionary(ga => ga.Id);

        var current = await LoadGenericAttributesAsync(customer.Id);
        var currentIds = current.Select(ga => ga.Id).ToHashSet();

        foreach (var attribute in current)
        {
            if (!snapshotById.TryGetValue(attribute.Id, out var original))
            {
                //added by the test
                await genericAttributeService.DeleteAttributeAsync(attribute);
                continue;
            }

            if (attribute.Value == original.Value)
                continue;

            attribute.Value = original.Value;
            await genericAttributeService.UpdateAttributeAsync(attribute);
        }

        //deleted by the test
        foreach (var original in _genericAttributesSnapshot.Where(ga => !currentIds.Contains(ga.Id)))
        {
            var restored = Copy(original);
            restored.Id = 0;
            await genericAttributeService.InsertAttributeAsync(restored);
        }
    }

    #endregion

    #region State

    /// <summary>
    /// Snapshot the current customer's cart and generic attributes; call in [SetUp]
    /// </summary>
    public async Task CaptureStateAsync()
    {
        var customer = await GetCurrentCustomerAsync();

        _cartSnapshot = (await LoadCartItemsAsync(customer.Id)).Select(Copy).ToList();
        _genericAttributesSnapshot = (await LoadGenericAttributesAsync(customer.Id)).Select(Copy).ToList();
        _settingsRestorers.Clear();
        _changedSettingsTypes.Clear();
        _productRestorers.Clear();
        _customerRestorers.Clear();
        _lastLogIdSnapshot = await GetService<IRepository<Log>>().Table.MaxAsync(log => (int?)log.Id) ?? 0;
    }

    /// <summary>
    /// Restore changed settings, the current customer's cart and generic attributes; call in [TearDown]
    /// </summary>
    public async Task RestoreStateAsync()
    {
        foreach (var restore in _settingsRestorers)
            await restore();

        _settingsRestorers.Clear();
        _changedSettingsTypes.Clear();

        foreach (var restore in _productRestorers)
            await restore();

        _productRestorers.Clear();

        foreach (var restore in _customerRestorers)
            await restore();

        _customerRestorers.Clear();

        var customer = await GetCurrentCustomerAsync();

        //the cart goes first: deleting cart items may reset checkout generic attributes
        await RestoreCartAsync(customer);
        await RestoreGenericAttributesAsync(customer);
        await RestoreLogAsync();
    }

    #endregion

    #region Context

    public async Task<Customer> GetCurrentCustomerAsync()
    {
        return await GetService<IWorkContext>().GetCurrentCustomerAsync();
    }

    public async Task<int> GetCurrentStoreIdAsync()
    {
        return (await GetService<IStoreContext>().GetCurrentStoreAsync()).Id;
    }

    /// <summary>
    /// Put the current customer into the "Guests" role, so CustomerService.IsGuestAsync returns true for it;
    /// the role mapping is removed in [TearDown]
    /// </summary>
    public async Task MakeCurrentCustomerGuestAsync()
    {
        var customerService = GetService<ICustomerService>();
        var customer = await GetCurrentCustomerAsync();

        if (await customerService.IsGuestAsync(customer))
            return;

        var guestRole = await customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName);
        await customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = guestRole.Id });
        _customerRestorers.Add(() => customerService.RemoveCustomerRoleMappingAsync(customer, guestRole));
    }

    /// <summary>
    /// Get warning log records written for the given customer since <see cref="CaptureStateAsync"/>
    /// (every record written during the test is deleted in [TearDown])
    /// </summary>
    public async Task<IList<Log>> GetWarningLogsSinceCaptureAsync(int customerId)
    {
        var lastLogId = _lastLogIdSnapshot;
        var warning = (int)LogLevel.Warning;

        return await GetService<IRepository<Log>>().Table
            .Where(log => log.Id > lastLogId && log.LogLevelId == warning && log.CustomerId == customerId)
            .OrderBy(log => log.Id)
            .ToListAsync();
    }

    #endregion

    #region Settings

    /// <summary>
    /// Change settings of the given type and save them; the original values are restored in [TearDown]
    /// </summary>
    public async Task SetSettingsAsync<TSettings>(Action<TSettings> change) where TSettings : ISettings, new()
    {
        var settingService = GetService<ISettingService>();

        if (_changedSettingsTypes.Add(typeof(TSettings)))
        {
            var original = await settingService.LoadSettingAsync<TSettings>();
            _settingsRestorers.Add(() => settingService.SaveSettingAsync(original));
        }

        var settings = await settingService.LoadSettingAsync<TSettings>();
        change(settings);
        await settingService.SaveSettingAsync(settings);
    }

    public Task SetOrderSettingsAsync(Action<OrderSettings> change) => SetSettingsAsync(change);

    public Task SetShippingSettingsAsync(Action<ShippingSettings> change) => SetSettingsAsync(change);

    public Task SetPaymentSettingsAsync(Action<PaymentSettings> change) => SetSettingsAsync(change);

    public Task SetCaptchaSettingsAsync(Action<CaptchaSettings> change) => SetSettingsAsync(change);

    public Task SetCustomerSettingsAsync(Action<CustomerSettings> change) => SetSettingsAsync(change);

    #endregion

    #region Cart

    /// <summary>
    /// Remove every shopping cart item of the current customer in the current store; restored in [TearDown]
    /// </summary>
    public async Task ClearCartAsync()
    {
        var shoppingCartService = GetService<IShoppingCartService>();
        var customer = await GetCurrentCustomerAsync();
        var storeId = await GetCurrentStoreIdAsync();

        var cart = await shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, storeId);
        foreach (var item in cart.ToList())
            await shoppingCartService.DeleteShoppingCartItemAsync(item, resetCheckoutData: false);
    }

    /// <summary>
    /// Add one sample product (the first simple, published, ship-enabled product without attributes that is not
    /// a gift card, download, rental or recurring product and can be added without warnings)
    /// to the current customer's cart; removed in [TearDown]
    /// </summary>
    /// <param name="requiresShipping">
    /// Whether the product requires shipping. The sample data has no plain product that is not ship enabled,
    /// so for false the chosen product's IsShipEnabled is switched off; it is switched back in [TearDown]
    /// </param>
    /// <returns>The product added to the cart</returns>
    public async Task<Product> AddSampleProductToCartAsync(bool requiresShipping = true)
    {
        var productRepository = GetService<IRepository<Product>>();
        var mappingRepository = GetService<IRepository<ProductAttributeMapping>>();
        var shoppingCartService = GetService<IShoppingCartService>();
        var customer = await GetCurrentCustomerAsync();
        var storeId = await GetCurrentStoreIdAsync();

        var productsWithAttributes = mappingRepository.Table.Select(pam => pam.ProductId);

        var candidates = await productRepository.Table
            .Where(p => p.Published && !p.Deleted &&
                        p.ProductTypeId == (int)ProductType.SimpleProduct &&
                        p.IsShipEnabled &&
                        !p.IsGiftCard && !p.IsDownload && !p.IsRental && !p.IsRecurring &&
                        !p.CustomerEntersPrice &&
                        !productsWithAttributes.Contains(p.Id))
            .OrderBy(p => p.Id)
            .ToListAsync();

        foreach (var product in candidates)
        {
            var warnings = await shoppingCartService.AddToCartAsync(customer, product, ShoppingCartType.ShoppingCart,
                storeId, quantity: Math.Max(product.OrderMinimumQuantity, 1));

            if (warnings.Any())
                continue;

            if (!requiresShipping)
            {
                product.IsShipEnabled = false;
                await productRepository.UpdateAsync(product);

                var productId = product.Id;
                _productRestorers.Add(async () =>
                {
                    var changed = await productRepository.GetByIdAsync(productId);
                    changed.IsShipEnabled = true;
                    await productRepository.UpdateAsync(changed);
                });
            }

            return product;
        }

        Assert.Fail("No sample product can be added to the cart without warnings");
        return null;
    }

    /// <summary>
    /// Change a product; Price, IsDownload and IsShipEnabled are the properties restored in [TearDown]
    /// </summary>
    public async Task UpdateProductAsync(Product product, Action<Product> change)
    {
        var productRepository = GetService<IRepository<Product>>();
        var productId = product.Id;

        var original = await productRepository.GetByIdAsync(productId);
        var (price, isDownload, isShipEnabled) = (original.Price, original.IsDownload, original.IsShipEnabled);

        change(original);
        await productRepository.UpdateAsync(original);

        //insert at the start, so the restorers run in reverse order and the oldest values win
        _productRestorers.Insert(0, async () =>
        {
            var changed = await productRepository.GetByIdAsync(productId);
            changed.Price = price;
            changed.IsDownload = isDownload;
            changed.IsShipEnabled = isShipEnabled;
            await productRepository.UpdateAsync(changed);
        });
    }

    #endregion

    #region Checkout generic attributes

    public async Task SetSelectedShippingOptionAsync(ShippingOption shippingOption)
    {
        await GetService<IGenericAttributeService>().SaveAttributeAsync(await GetCurrentCustomerAsync(),
            NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, await GetCurrentStoreIdAsync());
    }

    public async Task SetOfferedShippingOptionsAsync(List<ShippingOption> shippingOptions)
    {
        await GetService<IGenericAttributeService>().SaveAttributeAsync(await GetCurrentCustomerAsync(),
            NopCustomerDefaults.OfferedShippingOptionsAttribute, shippingOptions, await GetCurrentStoreIdAsync());
    }

    public async Task SetSelectedPickupPointAsync(PickupPoint pickupPoint)
    {
        await GetService<IGenericAttributeService>().SaveAttributeAsync(await GetCurrentCustomerAsync(),
            NopCustomerDefaults.SelectedPickupPointAttribute, pickupPoint, await GetCurrentStoreIdAsync());
    }

    public async Task SetSelectedPaymentMethodAsync(string paymentMethodSystemName)
    {
        await GetService<IGenericAttributeService>().SaveAttributeAsync(await GetCurrentCustomerAsync(),
            NopCustomerDefaults.SelectedPaymentMethodAttribute, paymentMethodSystemName, await GetCurrentStoreIdAsync());
    }

    /// <summary>
    /// Clear the selected/offered shipping options, the selected pickup point, the desired delivery date,
    /// the selected payment method and the stored process payment request
    /// </summary>
    public async Task ClearCheckoutAttributesAsync()
    {
        var genericAttributeService = GetService<IGenericAttributeService>();
        var customer = await GetCurrentCustomerAsync();
        var storeId = await GetCurrentStoreIdAsync();

        await genericAttributeService.SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, storeId);
        await genericAttributeService.SaveAttributeAsync<List<ShippingOption>>(customer, NopCustomerDefaults.OfferedShippingOptionsAttribute, null, storeId);
        await genericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, storeId);
        await genericAttributeService.SaveAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, null, storeId);
        await genericAttributeService.SaveAttributeAsync<DateTime?>(customer, NopCustomerDefaults.DesiredDeliveryDate, null, storeId);
        await genericAttributeService.SaveAttributeAsync<string>(customer, NopCustomerDefaults.ProcessPaymentRequestAttribute, null, storeId);
    }

    /// <summary>
    /// Read a generic attribute of the current customer in the current store through IGenericAttributeService
    /// </summary>
    public async Task<TPropType> GetCustomerAttributeAsync<TPropType>(string key)
    {
        return await GetService<IGenericAttributeService>().GetAttributeAsync<TPropType>(await GetCurrentCustomerAsync(),
            key, await GetCurrentStoreIdAsync());
    }

    public Task<ShippingOption> GetSelectedShippingOptionAsync() =>
        GetCustomerAttributeAsync<ShippingOption>(NopCustomerDefaults.SelectedShippingOptionAttribute);

    public Task<PickupPoint> GetSelectedPickupPointAsync() =>
        GetCustomerAttributeAsync<PickupPoint>(NopCustomerDefaults.SelectedPickupPointAttribute);

    public Task<DateTime?> GetDesiredDeliveryDateAsync() =>
        GetCustomerAttributeAsync<DateTime?>(NopCustomerDefaults.DesiredDeliveryDate);

    #endregion
}

/// <summary>
/// Readers for checkout action results
/// </summary>
public static class CheckoutResults
{
    /// <summary>
    /// Assert the result is a RedirectToRouteResult and return its route name
    /// </summary>
    public static string RouteName(IActionResult result)
    {
        return result.Should().BeOfType<RedirectToRouteResult>().Subject.RouteName;
    }

    /// <summary>
    /// Assert the result is a ChallengeResult
    /// </summary>
    public static ChallengeResult Challenge(IActionResult result)
    {
        return result.Should().BeOfType<ChallengeResult>().Subject;
    }

    /// <summary>
    /// Assert the result is a ViewResult and return its view name (null for the default view) and model type
    /// </summary>
    public static (string ViewName, Type ModelType) View(IActionResult result)
    {
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;

        return (viewResult.ViewName, viewResult.Model?.GetType());
    }

    /// <summary>
    /// Assert the result is a JsonResult and return the keys and values of its (anonymous) value object
    /// </summary>
    public static IDictionary<string, object> Json(IActionResult result)
    {
        var value = result.Should().BeOfType<JsonResult>().Subject.Value;
        value.Should().NotBeNull();

        return value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(value));
    }

    /// <summary>
    /// Assert the result is the OPC error JSON { error = 1, message } and return the message
    /// </summary>
    public static string JsonError(IActionResult result)
    {
        var json = Json(result);
        json.Keys.Should().BeEquivalentTo("error", "message");
        json["error"].Should().Be(1);

        return json["message"].Should().BeOfType<string>().Subject;
    }

    /// <summary>
    /// Return the name and html of the "update_section" value of a JSON result
    /// </summary>
    public static (string Name, string Html) UpdateSection(IDictionary<string, object> json)
    {
        json.Should().ContainKey("update_section");
        var section = json["update_section"];
        section.Should().NotBeNull();

        var type = section.GetType();

        return ((string)type.GetProperty("name")?.GetValue(section), (string)type.GetProperty("html")?.GetValue(section));
    }
}
