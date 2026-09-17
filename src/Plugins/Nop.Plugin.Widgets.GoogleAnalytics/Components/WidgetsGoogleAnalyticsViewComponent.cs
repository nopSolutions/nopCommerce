using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Http;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.UI;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Components;

public class WidgetsGoogleAnalyticsViewComponent : NopViewComponent
{
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly GoogleAnalyticsSettings _googleAnalyticsSettings;
    protected readonly ICategoryService _categoryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly ILogger _logger;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly INopHtmlHelper _nopHtmlHelper;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IProductService _productService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly ITaxService _taxService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public WidgetsGoogleAnalyticsViewComponent(
        CurrencySettings currencySettings,
        GoogleAnalyticsSettings googleAnalyticsSettings,
        ICategoryService categoryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        ILogger logger,
        IManufacturerService manufacturerService,
        INopHtmlHelper nopHtmlHelper,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPriceCalculationService priceCalculationService,
        IProductService productService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        ITaxService taxService,
        IWorkContext workContext)
    {
        _currencySettings = currencySettings;
        _googleAnalyticsSettings = googleAnalyticsSettings;
        _categoryService = categoryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _logger = logger;
        _manufacturerService = manufacturerService;
        _nopHtmlHelper = nopHtmlHelper;
        _orderTotalCalculationService = orderTotalCalculationService;
        _priceCalculationService = priceCalculationService;
        _productService = productService;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _taxService = taxService;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get primary currency
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains primary store currency</returns>
    /// </returns>
    protected async Task<Currency> GetPrimaryCurrencyAsync()
    {
        var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);

        return currency;
    }

    /// <summary>
    /// Get script for Google Analytics tracking code
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains script for Google Analytics tracking code
    /// </returns>
    protected async Task<string> GetScriptAsync()
    {
        try
        {
            var analyticsTrackingScript = _googleAnalyticsSettings.TrackingScript + "\n";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", _googleAnalyticsSettings.GoogleId);
            //remove {ECOMMERCE} (used in previous versions of the plugin)
            analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", "");
            //remove {CustomerID} (used in previous versions of the plugin)
            analyticsTrackingScript = analyticsTrackingScript.Replace("{CustomerID}", "");

            //whether to include customer identifier
            var customerIdCode = string.Empty;
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (_googleAnalyticsSettings.IncludeCustomerId && !await _customerService.IsGuestAsync(customer))
                customerIdCode = $"gtag('set', {{'user_id': '{customer.Id}'}});{Environment.NewLine}";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{CUSTOMER_TRACKING}", customerIdCode);
            analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE_TRACKING}", "");

            return analyticsTrackingScript;
        }
        catch (Exception ex)
        {
            await _logger.InsertLogAsync(LogLevel.Error, "Error creating scripts for Google eCommerce tracking", ex.ToString());
        }

        return "";
    }

    /// <summary>
    /// Gets the view item event script for Google Analytics
    /// </summary>
    /// <param name="additionalData">The additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view item event script for Google Analytics
    /// </returns>
    protected async Task<string> GetViewItemEventScriptAsync(object additionalData)
    {
        if (additionalData == null || additionalData is not ProductDetailsModel model)
            return string.Empty;

        var stringBuilder = new StringBuilder("<script>");
        stringBuilder.AppendLine("dataLayer.push({");
        stringBuilder.AppendLine("'event': 'view_item',");

        var currency = await GetPrimaryCurrencyAsync();
        stringBuilder.AppendLine("'ecommerce': {");
        stringBuilder.AppendLine($"'currency': '{currency.CurrencyCode}',");
        stringBuilder.AppendLine("'items': [");

        if (model.ProductType == ProductType.SimpleProduct)
        {
            WriteProductDetails(stringBuilder, ConvertToGoogleProduct(model));
        }
        else
        {
            var needComma = false;

            foreach (var associatedProduct in model.AssociatedProducts)
            {
                if (needComma)
                    stringBuilder.AppendLine(",");

                needComma = WriteProductDetails(stringBuilder, ConvertToGoogleProduct(associatedProduct));
            }
        }
        stringBuilder.AppendLine("]");
        stringBuilder.AppendLine("}");
        stringBuilder.AppendLine("});");
        stringBuilder.AppendLine("</script>");

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Gets the view cart event script for Google Analytics
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view cart event script for Google Analytics
    /// </returns>
    private async Task<string> GetViewCartEventScriptAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        if (!cart.Any())
            return string.Empty;

        var stringBuilder = new StringBuilder("<script>");
        stringBuilder.AppendLine("dataLayer.push({");
        stringBuilder.AppendLine("'event': 'view_cart',");
        await AddShoppingCartDataAsync(stringBuilder, cart);
        stringBuilder.AppendLine("});");
        stringBuilder.AppendLine("</script>");

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Gets the view item list event script for Google Analytics
    /// </summary>
    /// <param name="widgetZone">The widget zone</param>
    /// <param name="additionalData">The additional data</param>
    /// <param name="controller">The controller name</param>
    /// <param name="action">The action name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view item list event script for Google Analytics
    /// </returns>
    private async Task<string> GetViewItemListEventScriptAsync(string widgetZone, object additionalData, string controller, string action)
    {
        //create event script for Google Analytics if item list is not empty
        if (widgetZone.Equals(PublicWidgetZones.Footer))
        {
            if (!HttpContext.Items.TryGetValue("view_item_list", out var models) || models is not List<ProductOverviewModel> listOfModel || !listOfModel.Any())
                return string.Empty;

            var stringBuilder = new StringBuilder("<script>");
            stringBuilder.AppendLine("dataLayer.push({");
            stringBuilder.AppendLine("'event': 'view_item_list',");
            await AddProductDetailsDataAsync(stringBuilder, listOfModel, controller, action);
            stringBuilder.AppendLine("});");
            stringBuilder.AppendLine("</script>");

            return stringBuilder.ToString();
        }

        //store item(s) in session for later use
        if (widgetZone.Equals(PublicWidgetZones.ProductBoxAddinfoAfter))
        {
            if (additionalData == null || additionalData is not ProductOverviewModel model)
                return string.Empty;

            List<ProductOverviewModel> modelsToSave;

            if (HttpContext.Items.TryGetValue("view_item_list", out var models)
                && models is List<ProductOverviewModel> listOfModel)
            {
                modelsToSave = listOfModel;
            }
            else
            {
                modelsToSave = new List<ProductOverviewModel>();
            }

            modelsToSave.Add(model);

            HttpContext.Items["view_item_list"] = modelsToSave;
        }

        return string.Empty;
    }

    /// <summary>
    /// Get Google Analytics events script for the specified widget zone
    /// </summary>
    /// <param name="widgetZone">Widget zone</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains script for Google Analytics events
    /// </returns>
    private async Task<string> GetEventAsync(string widgetZone, object additionalData)
    {
        if (!_googleAnalyticsSettings.EnableEcommerce)
            return string.Empty;

        var script = string.Empty;

        try
        {
            //view_item
            if (widgetZone.Equals(PublicWidgetZones.ProductDetailsTop))
                script = await GetViewItemEventScriptAsync(additionalData);

            RouteData.Values.TryGetValue("controller", out var controllerObj);
            RouteData.Values.TryGetValue("action", out var actionObj);

            var controller = controllerObj?.ToString() ?? string.Empty;
            var action = actionObj?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action))
                return string.Empty;

            //view_item_list
            if (widgetZone.Equals(PublicWidgetZones.Footer) || widgetZone.Equals(PublicWidgetZones.ProductBoxAddinfoAfter))
                script = await GetViewItemListEventScriptAsync(widgetZone, additionalData, controller, action);

            var routeName = _nopHtmlHelper.GetRouteName();

            if (routeName.Equals(NopRouteNames.General.CART, StringComparison.InvariantCultureIgnoreCase) ||
                routeName.Equals(NopRouteNames.Standard.LOGIN_CHECKOUT_AS_GUEST,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                script = await GetViewCartEventScriptAsync();
            }
        }
        catch
        {
            //ignore
        }

        return script;
    }

    /// <summary>
    /// Adds the shopping cart data to the string builder
    /// </summary>
    /// <param name="stringBuilder">String builder</param>
    /// <param name="cart">Shopping cart</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task AddShoppingCartDataAsync(StringBuilder stringBuilder, IList<ShoppingCartItem> cart)
    {
        var (shoppingCartTotalBase, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart);

        shoppingCartTotalBase ??= (await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, true))
            .subTotalWithDiscount;

        var shoppingCartTotal = await _priceCalculationService.RoundPriceAsync(shoppingCartTotalBase.Value, await GetPrimaryCurrencyAsync());
        var currency = await GetPrimaryCurrencyAsync();
        stringBuilder.AppendLine("'ecommerce': {");
        stringBuilder.AppendLine($"'currency': '{currency.CurrencyCode}',");
        
        if (shoppingCartTotal > 0)
        {
            stringBuilder.AppendLine(
                $"'value': {shoppingCartTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)},");
        }

        stringBuilder.AppendLine("'items': [");

        var needComma = false;

        foreach (var cartItem in cart)
        {
            if (needComma)
                stringBuilder.AppendLine(",");

            needComma = WriteProductDetails(stringBuilder, await ConvertToGoogleProductAsync(cartItem));
        }

        stringBuilder.AppendLine("]");
        stringBuilder.AppendLine("}");
    }

    /// <summary>
    /// Adds the product details data to the string builder
    /// </summary>
    /// <param name="stringBuilder">String builder</param>
    /// <param name="models">Product overview models</param>
    /// <param name="controller">The controller name</param>
    /// <param name="action">The action name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task AddProductDetailsDataAsync(StringBuilder stringBuilder, IList<ProductOverviewModel> models, string controller, string action)
    {
        var list = action;

        if (controller.Equals("Catalog", StringComparison.InvariantCultureIgnoreCase))
        {
            if (action.Contains("ProductsByTag", StringComparison.InvariantCultureIgnoreCase))
                list = "Products Tag";
        }
        else if (controller.Equals("Product", StringComparison.InvariantCultureIgnoreCase))
        {
            if (action.Contains("ProductDetails", StringComparison.InvariantCultureIgnoreCase))
                list = "Product Details";

            if (action.Contains("RecentlyViewedProducts", StringComparison.InvariantCultureIgnoreCase))
                list = "Recently Viewed Products";
        }
        else
        {
            list = controller;
        }

        list += " page";

        var currency = await GetPrimaryCurrencyAsync();
        stringBuilder.AppendLine("'ecommerce': {");
        stringBuilder.AppendLine($"'currency': '{currency.CurrencyCode}',");
        stringBuilder.AppendLine("'items': [");

        foreach (var model in models)
        {
            if (model.ProductType == ProductType.SimpleProduct)
            {
                WriteProductDetails(stringBuilder, await ConvertToGoogleProductAsync(model.Id, null, list));
            }
            else
            {
                var needComma = false;

                foreach (var associatedProduct in await _productService.GetAssociatedProductsAsync(model.Id))
                {
                    if (needComma)
                        stringBuilder.AppendLine(",");

                    needComma = WriteProductDetails(stringBuilder, await ConvertToGoogleProductAsync(associatedProduct.Id, null, list));
                }
            }
        }
        stringBuilder.AppendLine("]");
        stringBuilder.AppendLine("}");
    }

    /// <summary>
    /// Convert a product details to Google Analytics format
    /// </summary>
    /// <param name="model">Product details model</param>
    /// <returns>Product details in Google Analytics format</returns>
    private GoogleProduct ConvertToGoogleProduct(ProductDetailsModel model)
    {
        var sku = model.Sku;

        if (string.IsNullOrEmpty(sku))
            sku = model.Id.ToString();

        return new GoogleProduct
        {
            Id = sku,
            ProductId = model.Id,
            Name = model.Name,
            Brand = model.ProductManufacturers.LastOrDefault()?.Name,
            Category = model.Breadcrumb.CategoryBreadcrumb.LastOrDefault()?.Name,
            Price = model.ProductPrice.PriceValue ?? 0,
        };
    }

    /// <summary>
    /// Convert a product details to Google Analytics format
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="attributesXml">Product attributes in XML format</param>
    /// <param name="list">List name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product details in Google Analytics format
    /// </returns>
    protected virtual async Task<GoogleProduct> ConvertToGoogleProductAsync(int productId, string attributesXml = null, string list = "")
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(productId);

            if (product == null)
                return null;

            var sku = await _productService.FormatSkuAsync(product, attributesXml);

            if (string.IsNullOrEmpty(sku))
                sku = product.Id.ToString();

            decimal? price = null;

            if (product.ProductType == ProductType.SimpleProduct && !product.CustomerEntersPrice && !product.CallForPrice)
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var store = await _storeContext.GetCurrentStoreAsync();

                var minPossiblePrice = (await _priceCalculationService.GetFinalPriceAsync(product, customer, store)).finalPrice;
                price = (await _taxService.GetProductPriceAsync(product, minPossiblePrice)).price;
            }

            var productManufacturers = await _manufacturerService.GetProductManufacturersByProductIdAsync(productId);
            var id = productManufacturers.OrderBy(p => p.DisplayOrder).Select(pc => pc.ManufacturerId).FirstOrDefault();
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);

            return new GoogleProduct
            {
                Id = sku,
                Name = product.Name,
                List = list,
                Price = price ?? 0,
                ProductId = productId,
                Brand = manufacturer?.Name ?? string.Empty,
                Category = await _categoryService.GetCategoryNameForProductAsync(productId)
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Convert a product details to Google Analytics format
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product details in Google Analytics format
    /// </returns>
    protected virtual async Task<GoogleProduct> ConvertToGoogleProductAsync(ShoppingCartItem shoppingCartItem)
    {
        try
        {
            var productId = shoppingCartItem.ProductId;
            var product = await _productService.GetProductByIdAsync(productId);

            if (product == null)
                return null;

            var sku = await _productService.FormatSkuAsync(product, shoppingCartItem.AttributesXml);

            if (string.IsNullOrEmpty(sku))
                sku = product.Id.ToString();
            
            var sciUnitPrice = await _shoppingCartService.GetUnitPriceAsync(shoppingCartItem, true);
            
            var price = sciUnitPrice.unitPrice;

            var productManufacturers = await _manufacturerService.GetProductManufacturersByProductIdAsync(productId);
            var id = productManufacturers.OrderBy(p => p.DisplayOrder).Select(pc => pc.ManufacturerId).FirstOrDefault();
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);

            return new GoogleProduct
            {
                Id = sku,
                Name = product.Name,
                List = string.Empty,
                Price = price,
                ProductId = productId,
                Brand = manufacturer?.Name ?? string.Empty,
                Category = await _categoryService.GetCategoryNameForProductAsync(productId)
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Adds the product details data to the string builder
    /// </summary>
    /// <param name="stringBuilder">String builder</param>
    /// <param name="product">Product</param>
    /// <returns>Returns the flag indicating whether the data was written</returns>
    private bool WriteProductDetails(StringBuilder stringBuilder, GoogleProduct product)
    {
        if (product == null)
            return false;

        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine(string.Format("  'item_name': '{0}',", product.Name));
        stringBuilder.AppendLine(string.Format("  'item_id': '{0}',", product.Id));
        stringBuilder.AppendLine(string.Format("  'price': {0},", product.Price.ToString("0.00", CultureInfo.InvariantCulture)));

        if (!string.IsNullOrWhiteSpace(product.Brand))
            stringBuilder.AppendLine(string.Format("  'item_brand': '{0}',", product.Brand));

        if (!string.IsNullOrWhiteSpace(product.Category))
            stringBuilder.AppendLine(string.Format("  'item_category': '{0}',", product.Category));

        if (!string.IsNullOrWhiteSpace(product.List))
            stringBuilder.AppendLine(string.Format("  'item_list_name': '{0}',", product.List));

        if (product.Position.HasValue)
            stringBuilder.AppendLine(string.Format("  'index': {0},", product.Position));

        if (product.Quantity > 0)
            stringBuilder.AppendLine(string.Format("  'quantity': {0},", product.Quantity));

        if (product.CartItemId > 0)
            stringBuilder.AppendLine(string.Format("  'cartItemId': {0},", product.CartItemId));

        stringBuilder.AppendLine(string.Format("  'productId': {0}", product.ProductId));
        stringBuilder.Append("}");

        return true;
    }

    #endregion

    #region Methods

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        if (widgetZone.Equals(PublicWidgetZones.HeadHtmlTag))
        {
            var script = await GetScriptAsync();

            return new HtmlContentViewComponentResult(new HtmlString(script));
        }

        var eventScript = await GetEventAsync(widgetZone, additionalData);

        return new HtmlContentViewComponentResult(new HtmlString(eventScript));
    }

    #endregion

    #region Nestead class

    /// <summary>
    /// Represents a Google product model
    /// </summary>
    public partial record GoogleProduct
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the Brand
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Gets or sets the Category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the Position
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// Gets or sets the Variant
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// Gets or sets theQuantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the product Id
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart item Id
        /// </summary>
        public int CartItemId { get; set; }

        /// <summary>
        /// Gets or sets the List
        /// </summary>
        public string List { get; set; }
    }

    #endregion
}