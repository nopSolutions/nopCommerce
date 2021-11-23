using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class ShoppingCartController : BasePublicController
    {
        #region Fields

        protected CaptchaSettings CaptchaSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected ICheckoutAttributeParser CheckoutAttributeParser { get; }
        protected ICheckoutAttributeService CheckoutAttributeService { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDiscountService DiscountService { get; }
        protected IDownloadService DownloadService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IGiftCardService GiftCardService { get; }
        protected ILocalizationService LocalizationService { get; }
        private readonly INopHtmlHelper _nopHtmlHelper;
        protected INopFileProvider FileProvider { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IPictureService PictureService { get; }
        protected IPriceFormatter PriceFormatter { get; }
        protected IProductAttributeParser ProductAttributeParser { get; }
        protected IProductAttributeService ProductAttributeService { get; }
        protected IProductService ProductService { get; }
        protected IShippingService ShippingService { get; }
        protected IShoppingCartModelFactory ShoppingCartModelFactory { get; }
        protected IShoppingCartService ShoppingCartService { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IStoreContext StoreContext { get; }
        protected ITaxService TaxService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected MediaSettings MediaSettings { get; }
        protected OrderSettings OrderSettings { get; }
        protected ShoppingCartSettings ShoppingCartSettings { get; }
        protected ShippingSettings ShippingSettings { get; }

        #endregion

        #region Ctor

        public ShoppingCartController(CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            INopHtmlHelper nopHtmlHelper,
            INopFileProvider fileProvider,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings,
            ShippingSettings shippingSettings)
        {
            CaptchaSettings = captchaSettings;
            CustomerSettings = customerSettings;
            CheckoutAttributeParser = checkoutAttributeParser;
            CheckoutAttributeService = checkoutAttributeService;
            CurrencyService = currencyService;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            DiscountService = discountService;
            DownloadService = downloadService;
            GenericAttributeService = genericAttributeService;
            GiftCardService = giftCardService;
            LocalizationService = localizationService;
            _nopHtmlHelper = nopHtmlHelper;
            FileProvider = fileProvider;
            NotificationService = notificationService;
            PermissionService = permissionService;
            PictureService = pictureService;
            PriceFormatter = priceFormatter;
            ProductAttributeParser = productAttributeParser;
            ProductAttributeService = productAttributeService;
            ProductService = productService;
            ShippingService = shippingService;
            ShoppingCartModelFactory = shoppingCartModelFactory;
            ShoppingCartService = shoppingCartService;
            StaticCacheManager = staticCacheManager;
            StoreContext = storeContext;
            TaxService = taxService;
            UrlRecordService = urlRecordService;
            WebHelper = webHelper;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
            MediaSettings = mediaSettings;
            OrderSettings = orderSettings;
            ShoppingCartSettings = shoppingCartSettings;
            ShippingSettings = shippingSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task ParseAndSaveCheckoutAttributesAsync(IList<ShoppingCartItem> cart, IFormCollection form)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var excludeShippableAttributes = !await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            var store = await StoreContext.GetCurrentStoreAsync();
            var checkoutAttributes = await CheckoutAttributeService.GetAllCheckoutAttributesAsync(store.Id, excludeShippableAttributes);
            foreach (var attribute in checkoutAttributes)
            {
                var controlId = $"checkout_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = CheckoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = CheckoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = await CheckoutAttributeService.GetCheckoutAttributeValuesAsync(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = CheckoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = CheckoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }

                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(date));
                            }
                            catch
                            {
                                // ignored
                            }

                            if (selectedDate.HasValue)
                                attributesXml = CheckoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                        }

                        break;
                    case AttributeControlType.FileUpload:
                        {
                            _ = Guid.TryParse(form[controlId], out var downloadGuid);
                            var download = await DownloadService.GetDownloadByGuidAsync(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = CheckoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                           attribute, download.DownloadGuid.ToString());
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            //validate conditional attributes (if specified)
            foreach (var attribute in checkoutAttributes)
            {
                var conditionMet = await CheckoutAttributeParser.IsConditionMetAsync(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                    attributesXml = CheckoutAttributeParser.RemoveCheckoutAttribute(attributesXml, attribute);
            }

            //save checkout attributes
            await GenericAttributeService.SaveAttributeAsync(await WorkContext.GetCurrentCustomerAsync(), NopCustomerDefaults.CheckoutAttributes, attributesXml, store.Id);
        }

        protected virtual async Task SaveItemAsync(ShoppingCartItem updatecartitem, List<string> addToCartWarnings, Product product,
           ShoppingCartType cartType, string attributes, decimal customerEnteredPriceConverted, DateTime? rentalStartDate,
           DateTime? rentalEndDate, int quantity)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            if (updatecartitem == null)
            {
                //add to the cart
                addToCartWarnings.AddRange(await ShoppingCartService.AddToCartAsync(customer,
                    product, cartType, store.Id,
                    attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity, true));
            }
            else
            {
                var cart = await ShoppingCartService.GetShoppingCartAsync(customer, updatecartitem.ShoppingCartType, store.Id);

                var otherCartItemWithSameParameters = await ShoppingCartService.FindShoppingCartItemInTheCartAsync(
                    cart, updatecartitem.ShoppingCartType, product, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate);
                if (otherCartItemWithSameParameters != null &&
                    otherCartItemWithSameParameters.Id == updatecartitem.Id)
                {
                    //ensure it's some other shopping cart item
                    otherCartItemWithSameParameters = null;
                }
                //update existing item
                addToCartWarnings.AddRange(await ShoppingCartService.UpdateShoppingCartItemAsync(customer,
                    updatecartitem.Id, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity + (otherCartItemWithSameParameters?.Quantity ?? 0), true));
                if (otherCartItemWithSameParameters != null && !addToCartWarnings.Any())
                {
                    //delete the same shopping cart item (the other one)
                    await ShoppingCartService.DeleteShoppingCartItemAsync(otherCartItemWithSameParameters);
                }
            }
        }

        protected virtual async Task<IActionResult> GetProductToCartDetailsAsync(List<string> addToCartWarnings, ShoppingCartType cartType,
            Product product)
        {
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart/wishlist
                //let's display warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //added to the cart/wishlist
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        await CustomerActivityService.InsertActivityAsync("PublicStore.AddToWishlist",
                            string.Format(await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.AddToWishlist"), product.Name), product);

                        if (ShoppingCartSettings.DisplayWishlistAfterAddingProduct)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id);

                        var updateTopWishlistSectionHtml = string.Format(
                            await LocalizationService.GetResourceAsync("Wishlist.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));

                        return Json(new
                        {
                            success = true,
                            message = string.Format(
                                await LocalizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheWishlist.Link"),
                                Url.RouteUrl("Wishlist")),
                            updatetopwishlistsectionhtml = updateTopWishlistSectionHtml
                        });
                    }

                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        await CustomerActivityService.InsertActivityAsync("PublicStore.AddToShoppingCart",
                            string.Format(await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.AddToShoppingCart"), product.Name), product);

                        if (ShoppingCartSettings.DisplayCartAfterAddingProduct)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                        var updateTopCartSectionHtml = string.Format(
                            await LocalizationService.GetResourceAsync("ShoppingCart.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));

                        var updateFlyoutCartSectionHtml = ShoppingCartSettings.MiniShoppingCartEnabled
                            ? await RenderViewComponentToStringAsync("FlyoutShoppingCart")
                            : string.Empty;

                        return Json(new
                        {
                            success = true,
                            message = string.Format(await LocalizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheCart.Link"),
                                Url.RouteUrl("ShoppingCart")),
                            updatetopcartsectionhtml = updateTopCartSectionHtml,
                            updateflyoutcartsectionhtml = updateFlyoutCartSectionHtml
                        });
                    }
            }
        }

        #endregion

        #region Shopping cart

        [HttpPost]
        public virtual async Task<IActionResult> SelectShippingOption([FromQuery] string name, [FromQuery] EstimateShippingModel model)
        {
            if (model == null)
                model = new EstimateShippingModel();

            var errors = new List<string>();
            if (string.IsNullOrEmpty(model.ZipPostalCode))
                errors.Add(await LocalizationService.GetResourceAsync("Shipping.EstimateShipping.ZipPostalCode.Required"));

            if (model.CountryId == null || model.CountryId == 0)
                errors.Add(await LocalizationService.GetResourceAsync("Shipping.EstimateShipping.Country.Required"));

            if (errors.Count > 0)
                return Json(new
                {
                    success = false,
                    errors = errors
                });

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, model.Form);

            var shippingOptions = new List<ShippingOption>();
            ShippingOption selectedShippingOption = null;

            if (!string.IsNullOrWhiteSpace(name))
            {
                //find shipping options
                //performance optimization. try cache first
                shippingOptions = await GenericAttributeService.GetAttributeAsync<List<ShippingOption>>(customer,
                    NopCustomerDefaults.OfferedShippingOptionsAttribute, store.Id);

                if (shippingOptions == null || !shippingOptions.Any())
                {
                    var address = new Address
                    {
                        CountryId = model.CountryId,
                        StateProvinceId = model.StateProvinceId,
                        ZipPostalCode = model.ZipPostalCode,
                    };

                    //not found? let's load them using shipping service
                    var getShippingOptionResponse = await ShippingService.GetShippingOptionsAsync(cart, address,
                        customer, storeId: store.Id);

                    if (getShippingOptionResponse.Success)
                        shippingOptions = getShippingOptionResponse.ShippingOptions.ToList();
                    else
                        foreach (var error in getShippingOptionResponse.Errors)
                            errors.Add(error);
                }
            }

            selectedShippingOption = shippingOptions.Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (selectedShippingOption == null)
                errors.Add(await LocalizationService.GetResourceAsync("Shipping.EstimateShippingPopUp.ShippingOption.IsNotFound"));

            if (errors.Count > 0)
                return Json(new
                {
                    success = false,
                    errors = errors
                });

            //reset pickup point
            await GenericAttributeService.SaveAttributeAsync<PickupPoint>(customer,
                NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);

            //cache shipping option
            await GenericAttributeService.SaveAttributeAsync(customer,
                NopCustomerDefaults.SelectedShippingOptionAttribute, selectedShippingOption, store.Id);

            var orderTotalsSectionHtml = await RenderViewComponentToStringAsync("OrderTotals", new { isEditable = true });

            return Json(new
            {
                success = true,
                ordertotalssectionhtml = orderTotalsSectionHtml
            });
        }

        //add product to cart using AJAX
        //currently we use this method on catalog pages (category/manufacturer/etc)
        [HttpPost]
        public virtual async Task<IActionResult> AddProductToCart_Catalog(int productId, int shoppingCartTypeId,
            int quantity, bool forceredirection = false)
        {
            var cartType = (ShoppingCartType)shoppingCartTypeId;

            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null)
                //no product found
                return Json(new
                {
                    success = false,
                    message = "No product found with the specified ID"
                });

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = await UrlRecordService.GetSeNameAsync(product) })
                });
            }

            //products with "minimum order quantity" more than a specified qty
            if (product.OrderMinimumQuantity > quantity)
            {
                //we cannot add to the cart such products from category pages
                //it can confuse customers. That's why we redirect customers to the product details page
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = await UrlRecordService.GetSeNameAsync(product) })
                });
            }

            if (product.CustomerEntersPrice)
            {
                //cannot be added to the cart (requires a customer to enter price)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = await UrlRecordService.GetSeNameAsync(product) })
                });
            }

            if (product.IsRental)
            {
                //rental products require start/end dates to be entered
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = await UrlRecordService.GetSeNameAsync(product) })
                });
            }

            var allowedQuantities = ProductService.ParseAllowedQuantities(product);
            if (allowedQuantities.Length > 0)
            {
                //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = await UrlRecordService.GetSeNameAsync(product) })
                });
            }

            //allow a product to be added to the cart when all attributes are with "read-only checkboxes" type
            var productAttributes = await ProductAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            if (productAttributes.Any(pam => pam.AttributeControlType != AttributeControlType.ReadonlyCheckboxes))
            {
                //product has some attributes. let a customer see them
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = await UrlRecordService.GetSeNameAsync(product) })
                });
            }

            //creating XML for "read-only checkboxes" attributes
            var attXml = await productAttributes.AggregateAwaitAsync(string.Empty, async (attributesXml, attribute) =>
            {
                var attributeValues = await ProductAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                foreach (var selectedAttributeId in attributeValues
                    .Where(v => v.IsPreSelected)
                    .Select(v => v.Id)
                    .ToList())
                {
                    attributesXml = ProductAttributeParser.AddProductAttribute(attributesXml,
                        attribute, selectedAttributeId.ToString());
                }

                return attributesXml;
            });

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, cartType, store.Id);
            var shoppingCartItem = await ShoppingCartService.FindShoppingCartItemInTheCartAsync(cart, cartType, product);
            //if we already have the same product in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
            var addToCartWarnings = await ShoppingCartService
                .GetShoppingCartItemWarningsAsync(customer, cartType,
                product, store.Id, string.Empty,
                decimal.Zero, null, null, quantityToValidate, false, shoppingCartItem?.Id ?? 0, true, false, false, false);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //let's display standard warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //now let's try adding product to the cart (now including product attribute validation, etc)
            addToCartWarnings = await ShoppingCartService.AddToCartAsync(customer: customer,
                product: product,
                shoppingCartType: cartType,
                storeId: store.Id,
                attributesXml: attXml,
                quantity: quantity);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the product details page
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = await UrlRecordService.GetSeNameAsync(product) })
                });
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        await CustomerActivityService.InsertActivityAsync("PublicStore.AddToWishlist",
                            string.Format(await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.AddToWishlist"), product.Name), product);

                        if (ShoppingCartSettings.DisplayWishlistAfterAddingProduct || forceredirection)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id);

                        var updatetopwishlistsectionhtml = string.Format(await LocalizationService.GetResourceAsync("Wishlist.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));
                        return Json(new
                        {
                            success = true,
                            message = string.Format(await LocalizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
                            updatetopwishlistsectionhtml
                        });
                    }

                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        await CustomerActivityService.InsertActivityAsync("PublicStore.AddToShoppingCart",
                            string.Format(await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.AddToShoppingCart"), product.Name), product);

                        if (ShoppingCartSettings.DisplayCartAfterAddingProduct || forceredirection)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                        var updatetopcartsectionhtml = string.Format(await LocalizationService.GetResourceAsync("ShoppingCart.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));

                        var updateflyoutcartsectionhtml = ShoppingCartSettings.MiniShoppingCartEnabled
                            ? await RenderViewComponentToStringAsync("FlyoutShoppingCart")
                            : string.Empty;

                        return Json(new
                        {
                            success = true,
                            message = string.Format(await LocalizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
                            updatetopcartsectionhtml,
                            updateflyoutcartsectionhtml
                        });
                    }
            }
        }

        //add product to cart using AJAX
        //currently we use this method on the product details pages
        [HttpPost]
        public virtual async Task<IActionResult> AddProductToCart_Details(int productId, int shoppingCartTypeId, IFormCollection form)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("Homepage")
                });
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return Json(new
                {
                    success = false,
                    message = "Only simple products could be added to the cart"
                });
            }

            //update existing shopping cart item
            var updatecartitemid = 0;
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"addtocart_{productId}.UpdatedShoppingCartItemId", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(form[formKey], out updatecartitemid);
                    break;
                }

            ShoppingCartItem updatecartitem = null;
            if (ShoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var store = await StoreContext.GetCurrentStoreAsync();
                //search with the same cart type as specified
                var cart = await ShoppingCartService.GetShoppingCartAsync(await WorkContext.GetCurrentCustomerAsync(), (ShoppingCartType)shoppingCartTypeId, store.Id);

                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found? let's ignore it. in this case we'll add a new item
                //if (updatecartitem == null)
                //{
                //    return Json(new
                //    {
                //        success = false,
                //        message = "No shopping cart item found to update"
                //    });
                //}
                //is it this product?
                if (updatecartitem != null && product.Id != updatecartitem.ProductId)
                {
                    return Json(new
                    {
                        success = false,
                        message = "This product does not match a passed shopping cart item identifier"
                    });
                }
            }

            var addToCartWarnings = new List<string>();

            //customer entered price
            var customerEnteredPriceConverted = await ProductAttributeParser.ParseCustomerEnteredPriceAsync(product, form);

            //entered quantity
            var quantity = ProductAttributeParser.ParseEnteredQuantity(product, form);

            //product and gift card attributes
            var attributes = await ProductAttributeParser.ParseProductAttributesAsync(product, form, addToCartWarnings);

            //rental attributes
            ProductAttributeParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);

            var cartType = updatecartitem == null ? (ShoppingCartType)shoppingCartTypeId :
                //if the item to update is found, then we ignore the specified "shoppingCartTypeId" parameter
                updatecartitem.ShoppingCartType;

            await SaveItemAsync(updatecartitem, addToCartWarnings, product, cartType, attributes, customerEnteredPriceConverted, rentalStartDate, rentalEndDate, quantity);

            //return result
            return await GetProductToCartDetailsAsync(addToCartWarnings, cartType, product);
        }

        //handle product attribute selection event. this way we return new price, overridden gtin/sku/mpn
        //currently we use this method on the product details pages
        [HttpPost]
        public virtual async Task<IActionResult> ProductDetails_AttributeChange(int productId, bool validateAttributeConditions,
            bool loadPicture, IFormCollection form)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null)
                return new NullJsonResult();

            var errors = new List<string>();
            var attributeXml = await ProductAttributeParser.ParseProductAttributesAsync(product, form, errors);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ProductAttributeParser.ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            //sku, mpn, gtin
            var sku = await ProductService.FormatSkuAsync(product, attributeXml);
            var mpn = await ProductService.FormatMpnAsync(product, attributeXml);
            var gtin = await ProductService.FormatGtinAsync(product, attributeXml);

            // calculating weight adjustment
            var attributeValues = await ProductAttributeParser.ParseProductAttributeValuesAsync(attributeXml);
            var totalWeight = product.BasepriceAmount;

            foreach (var attributeValue in attributeValues)
            {
                switch (attributeValue.AttributeValueType)
                {
                    case AttributeValueType.Simple:
                        //simple attribute
                        totalWeight += attributeValue.WeightAdjustment;
                        break;
                    case AttributeValueType.AssociatedToProduct:
                        //bundled product
                        var associatedProduct = await ProductService.GetProductByIdAsync(attributeValue.AssociatedProductId);
                        if (associatedProduct != null)
                            totalWeight += associatedProduct.BasepriceAmount * attributeValue.Quantity;
                        break;
                }
            }

            //price
            var price = string.Empty;
            //base price
            var basepricepangv = string.Empty;
            if (await PermissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices) && !product.CustomerEntersPrice)
            {
                //we do not calculate price of "customer enters price" option is enabled
                var (finalPrice, _, _) = await ShoppingCartService.GetUnitPriceAsync(product,
                    await WorkContext.GetCurrentCustomerAsync(),
                    ShoppingCartType.ShoppingCart,
                    1, attributeXml, 0,
                    rentalStartDate, rentalEndDate, true);
                var (finalPriceWithDiscountBase, _) = await TaxService.GetProductPriceAsync(product, finalPrice);
                var finalPriceWithDiscount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, await WorkContext.GetWorkingCurrencyAsync());
                price = await PriceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                basepricepangv = await PriceFormatter.FormatBasePriceAsync(product, finalPriceWithDiscountBase, totalWeight);
            }

            //stock
            var stockAvailability = await ProductService.FormatStockMessageAsync(product, attributeXml);

            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                var attributes = await ProductAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                foreach (var attribute in attributes)
                {
                    var conditionMet = await ProductAttributeParser.IsConditionMetAsync(attribute, attributeXml);
                    if (conditionMet.HasValue)
                    {
                        if (conditionMet.Value)
                            enabledAttributeMappingIds.Add(attribute.Id);
                        else
                            disabledAttributeMappingIds.Add(attribute.Id);
                    }
                }
            }

            //picture. used when we want to override a default product picture when some attribute is selected
            var pictureFullSizeUrl = string.Empty;
            var pictureDefaultSizeUrl = string.Empty;
            if (loadPicture)
            {
                //first, try to get product attribute combination picture
                var pictureId = (await ProductAttributeParser.FindProductAttributeCombinationAsync(product, attributeXml))?.PictureId ?? 0;

                //then, let's see whether we have attribute values with pictures
                if (pictureId == 0)
                {
                    pictureId = (await ProductAttributeParser.ParseProductAttributeValuesAsync(attributeXml))
                        .FirstOrDefault(attributeValue => attributeValue.PictureId > 0)?.PictureId ?? 0;
                }

                if (pictureId > 0)
                {
                    var productAttributePictureCacheKey = StaticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductAttributePictureModelKey,
                        pictureId, WebHelper.IsCurrentConnectionSecured(), await StoreContext.GetCurrentStoreAsync());
                    var pictureModel = await StaticCacheManager.GetAsync(productAttributePictureCacheKey, async () =>
                    {
                        var picture = await PictureService.GetPictureByIdAsync(pictureId);
                        string fullSizeImageUrl, imageUrl;

                        (fullSizeImageUrl, picture) = await PictureService.GetPictureUrlAsync(picture);
                        (imageUrl, picture) = await PictureService.GetPictureUrlAsync(picture, MediaSettings.ProductDetailsPictureSize);

                        return picture == null ? new PictureModel() : new PictureModel
                        {
                            FullSizeImageUrl = fullSizeImageUrl,
                            ImageUrl = imageUrl
                        };
                    });
                    pictureFullSizeUrl = pictureModel.FullSizeImageUrl;
                    pictureDefaultSizeUrl = pictureModel.ImageUrl;
                }
            }

            var isFreeShipping = product.IsFreeShipping;
            if (isFreeShipping && !string.IsNullOrEmpty(attributeXml))
            {
                isFreeShipping = await (await ProductAttributeParser.ParseProductAttributeValuesAsync(attributeXml))
                    .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    .SelectAwait(async attributeValue => await ProductService.GetProductByIdAsync(attributeValue.AssociatedProductId))
                    .AllAsync(associatedProduct => associatedProduct == null || !associatedProduct.IsShipEnabled || associatedProduct.IsFreeShipping);
            }

            return Json(new
            {
                productId,
                gtin,
                mpn,
                sku,
                price,
                basepricepangv,
                stockAvailability,
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
                pictureFullSizeUrl,
                pictureDefaultSizeUrl,
                isFreeShipping,
                message = errors.Any() ? errors.ToArray() : null
            });
        }

        [HttpPost]
        public virtual async Task<IActionResult> CheckoutAttributeChange(IFormCollection form, bool isEditable)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            //save selected attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);
            var attributeXml = await GenericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.CheckoutAttributes, store.Id);

            //conditions
            var enabledAttributeIds = new List<int>();
            var disabledAttributeIds = new List<int>();
            var excludeShippableAttributes = !await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            var attributes = await CheckoutAttributeService.GetAllCheckoutAttributesAsync(store.Id, excludeShippableAttributes);
            foreach (var attribute in attributes)
            {
                var conditionMet = await CheckoutAttributeParser.IsConditionMetAsync(attribute, attributeXml);
                if (conditionMet.HasValue)
                {
                    if (conditionMet.Value)
                        enabledAttributeIds.Add(attribute.Id);
                    else
                        disabledAttributeIds.Add(attribute.Id);
                }
            }

            //update blocks
            var ordetotalssectionhtml = await RenderViewComponentToStringAsync("OrderTotals", new { isEditable });
            var selectedcheckoutattributesssectionhtml = await RenderViewComponentToStringAsync("SelectedCheckoutAttributes");

            return Json(new
            {
                ordetotalssectionhtml,
                selectedcheckoutattributesssectionhtml,
                enabledattributeids = enabledAttributeIds.ToArray(),
                disabledattributeids = disabledAttributeIds.ToArray()
            });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> UploadFileProductAttribute(int attributeId)
        {
            var attribute = await ProductAttributeService.GetProductAttributeMappingByIdAsync(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty
                });
            }

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }

            var fileBinary = await DownloadService.GetDownloadBitsAsync(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = FileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = FileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(await LocalizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty
                    });
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = string.Empty,
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = FileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            await DownloadService.InsertDownloadAsync(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = await LocalizationService.GetResourceAsync("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid
            });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> UploadFileCheckoutAttribute(int attributeId)
        {
            var attribute = await CheckoutAttributeService.GetCheckoutAttributeByIdAsync(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty
                });
            }

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }

            var fileBinary = await DownloadService.GetDownloadBitsAsync(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = FileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = FileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(await LocalizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty
                    });
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = string.Empty,
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = FileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            await DownloadService.InsertDownloadAsync(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = await LocalizationService.GetResourceAsync("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid
            });
        }

        public virtual async Task<IActionResult> Cart()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("Homepage");

            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(await WorkContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
            var model = new ShoppingCartModel();
            model = await ShoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("updatecart")]
        public virtual async Task<IActionResult> UpdateCart(IFormCollection form)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("Homepage");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            //get identifiers of items to remove
            var itemIdsToRemove = form["removefromcart"]
                .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(idString => int.TryParse(idString, out var id) ? id : 0)
                .Distinct().ToList();

            var products = (await ProductService.GetProductsByIdsAsync(cart.Select(item => item.ProductId).Distinct().ToArray()))
                .ToDictionary(item => item.Id, item => item);

            //get order items with changed quantity
            var itemsWithNewQuantity = cart.Select(item => new
            {
                //try to get a new quantity for the item, set 0 for items to remove
                NewQuantity = itemIdsToRemove.Contains(item.Id) ? 0 : int.TryParse(form[$"itemquantity{item.Id}"], out var quantity) ? quantity : item.Quantity,
                Item = item,
                Product = products.ContainsKey(item.ProductId) ? products[item.ProductId] : null
            }).Where(item => item.NewQuantity != item.Item.Quantity);

            //order cart items
            //first should be items with a reduced quantity and that require other products; or items with an increased quantity and are required for other products
            var orderedCart = await itemsWithNewQuantity
                .OrderByDescendingAwait(async cartItem =>
                    (cartItem.NewQuantity < cartItem.Item.Quantity &&
                     (cartItem.Product?.RequireOtherProducts ?? false)) ||
                    (cartItem.NewQuantity > cartItem.Item.Quantity && cartItem.Product != null && (await ShoppingCartService
                         .GetProductsRequiringProductAsync(cart, cartItem.Product)).Any()))
                .ToListAsync();

            //try to update cart items with new quantities and get warnings
            var warnings = await orderedCart.SelectAwait(async cartItem => new
            {
                ItemId = cartItem.Item.Id,
                Warnings = await ShoppingCartService.UpdateShoppingCartItemAsync(customer,
                    cartItem.Item.Id, cartItem.Item.AttributesXml, cartItem.Item.CustomerEnteredPrice,
                    cartItem.Item.RentalStartDateUtc, cartItem.Item.RentalEndDateUtc, cartItem.NewQuantity, true)
            }).ToListAsync();

            //updated cart
            cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            //prepare model
            var model = new ShoppingCartModel();
            model = await ShoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);

            //update current warnings
            foreach (var warningItem in warnings.Where(warningItem => warningItem.Warnings.Any()))
            {
                //find shopping cart item model to display appropriate warnings
                var itemModel = model.Items.FirstOrDefault(item => item.Id == warningItem.ItemId);
                if (itemModel != null)
                    itemModel.Warnings = warningItem.Warnings.Concat(itemModel.Warnings).Distinct().ToList();
            }

            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("continueshopping")]
        public virtual async Task<IActionResult> ContinueShopping()
        {
            var store = await StoreContext.GetCurrentStoreAsync();
            var returnUrl = await GenericAttributeService.GetAttributeAsync<string>(await WorkContext.GetCurrentCustomerAsync(), NopCustomerDefaults.LastContinueShoppingPageAttribute, store.Id);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToRoute("Homepage");
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("checkout")]
        public virtual async Task<IActionResult> StartCheckout(IFormCollection form)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            //validate attributes
            var checkoutAttributes = await GenericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.CheckoutAttributes, store.Id);
            var checkoutAttributeWarnings = await ShoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributes, true);
            if (checkoutAttributeWarnings.Any())
            {
                //something wrong, redisplay the page with warnings
                var model = new ShoppingCartModel();
                model = await ShoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart, validateCheckoutAttributes: true);
                return View(model);
            }

            var anonymousPermissed = OrderSettings.AnonymousCheckoutAllowed
                                     && CustomerSettings.UserRegistrationType == UserRegistrationType.Disabled;

            if (anonymousPermissed || !await CustomerService.IsGuestAsync(customer))
                return RedirectToRoute("Checkout");

            var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();
            var downloadableProductsRequireRegistration =
                CustomerSettings.RequireRegistrationForDownloadableProducts && await ProductService.HasAnyDownloadableProductAsync(cartProductIds);

            if (!OrderSettings.AnonymousCheckoutAllowed || downloadableProductsRequireRegistration)
            {
                //verify user identity (it may be facebook login page, or google, or local)
                return Challenge();
            }

            return RedirectToRoute("LoginCheckoutAsGuest", new { returnUrl = Url.RouteUrl("ShoppingCart") });
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applydiscountcouponcode")]
        public virtual async Task<IActionResult> ApplyDiscountCoupon(string discountcouponcode, IFormCollection form)
        {
            //trim
            if (discountcouponcode != null)
                discountcouponcode = discountcouponcode.Trim();

            //cart
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            var model = new ShoppingCartModel();
            if (!string.IsNullOrWhiteSpace(discountcouponcode))
            {
                //we find even hidden records here. this way we can display a user-friendly message if it's expired
                var discounts = (await DiscountService.GetAllDiscountsAsync(couponCode: discountcouponcode, showHidden: true))
                    .Where(d => d.RequiresCouponCode)
                    .ToList();
                if (discounts.Any())
                {
                    var userErrors = new List<string>();
                    var anyValidDiscount = await discounts.AnyAwaitAsync(async discount =>
                    {
                        var validationResult = await DiscountService.ValidateDiscountAsync(discount, customer, new[] { discountcouponcode });
                        userErrors.AddRange(validationResult.Errors);

                        return validationResult.IsValid;
                    });

                    if (anyValidDiscount)
                    {
                        //valid
                        await CustomerService.ApplyDiscountCouponCodeAsync(customer, discountcouponcode);
                        model.DiscountBox.Messages.Add(await LocalizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.Applied"));
                        model.DiscountBox.IsApplied = true;
                    }
                    else
                    {
                        if (userErrors.Any())
                            //some user errors
                            model.DiscountBox.Messages = userErrors;
                        else
                            //general error text
                            model.DiscountBox.Messages.Add(await LocalizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                    }
                }
                else
                    //discount cannot be found
                    model.DiscountBox.Messages.Add(await LocalizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.CannotBeFound"));
            }
            else
                //empty coupon code
                model.DiscountBox.Messages.Add(await LocalizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.Empty"));

            model = await ShoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);

            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applygiftcardcouponcode")]
        public virtual async Task<IActionResult> ApplyGiftCard(string giftcardcouponcode, IFormCollection form)
        {
            //trim
            if (giftcardcouponcode != null)
                giftcardcouponcode = giftcardcouponcode.Trim();

            //cart
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            var model = new ShoppingCartModel();
            if (!await ShoppingCartService.ShoppingCartIsRecurringAsync(cart))
            {
                if (!string.IsNullOrWhiteSpace(giftcardcouponcode))
                {
                    var giftCard = (await GiftCardService.GetAllGiftCardsAsync(giftCardCouponCode: giftcardcouponcode)).FirstOrDefault();
                    var isGiftCardValid = giftCard != null && await GiftCardService.IsGiftCardValidAsync(giftCard);
                    if (isGiftCardValid)
                    {
                        await CustomerService.ApplyGiftCardCouponCodeAsync(customer, giftcardcouponcode);
                        model.GiftCardBox.Message = await LocalizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.Applied");
                        model.GiftCardBox.IsApplied = true;
                    }
                    else
                    {
                        model.GiftCardBox.Message = await LocalizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                        model.GiftCardBox.IsApplied = false;
                    }
                }
                else
                {
                    model.GiftCardBox.Message = await LocalizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                    model.GiftCardBox.IsApplied = false;
                }
            }
            else
            {
                model.GiftCardBox.Message = await LocalizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts");
                model.GiftCardBox.IsApplied = false;
            }

            model = await ShoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetEstimateShipping(EstimateShippingModel model)
        {
            if (model == null)
                model = new EstimateShippingModel();

            var errors = new List<string>();

            if (!ShippingSettings.EstimateShippingCityNameEnabled && string.IsNullOrEmpty(model.ZipPostalCode))
                errors.Add(await LocalizationService.GetResourceAsync("Shipping.EstimateShipping.ZipPostalCode.Required"));

            if (ShippingSettings.EstimateShippingCityNameEnabled && string.IsNullOrEmpty(model.City))
                errors.Add(await LocalizationService.GetResourceAsync("Shipping.EstimateShipping.City.Required"));

            if (model.CountryId == null || model.CountryId == 0)
                errors.Add(await LocalizationService.GetResourceAsync("Shipping.EstimateShipping.Country.Required"));

            if (errors.Count > 0)
                return Json(new
                {
                    Success = false,
                    Errors = errors
                });

            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(await WorkContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, model.Form);

            var result = await ShoppingCartModelFactory.PrepareEstimateShippingResultModelAsync(cart, model, true);

            return Json(result);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removediscount-")]
        public virtual async Task<IActionResult> RemoveDiscountCoupon(IFormCollection form)
        {
            var model = new ShoppingCartModel();

            //get discount identifier
            var discountId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("removediscount-", StringComparison.InvariantCultureIgnoreCase))
                    discountId = Convert.ToInt32(formValue["removediscount-".Length..]);
            var discount = await DiscountService.GetDiscountByIdAsync(discountId);
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (discount != null)
                await CustomerService.RemoveDiscountCouponCodeAsync(customer, discount.CouponCode);

            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            model = await ShoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removegiftcard-")]
        public virtual async Task<IActionResult> RemoveGiftCardCode(IFormCollection form)
        {
            var model = new ShoppingCartModel();

            //get gift card identifier
            var giftCardId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("removegiftcard-", StringComparison.InvariantCultureIgnoreCase))
                    giftCardId = Convert.ToInt32(formValue["removegiftcard-".Length..]);
            var gc = await GiftCardService.GetGiftCardByIdAsync(giftCardId);
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (gc != null)
                await CustomerService.RemoveGiftCardCouponCodeAsync(customer, gc.GiftCardCouponCode);

            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            model = await ShoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
            return View(model);
        }

        #endregion

        #region Wishlist

        public virtual async Task<IActionResult> Wishlist(Guid? customerGuid)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("Homepage");

            var customer = customerGuid.HasValue ?
                await CustomerService.GetCustomerByGuidAsync(customerGuid.Value)
                : await WorkContext.GetCurrentCustomerAsync();
            if (customer == null)
                return RedirectToRoute("Homepage");

            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id);

            var model = new WishlistModel();
            model = await ShoppingCartModelFactory.PrepareWishlistModelAsync(model, cart, !customerGuid.HasValue);
            return View(model);
        }

        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired("updatecart")]
        public virtual async Task<IActionResult> UpdateWishlist(IFormCollection form)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("Homepage");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id);

            var allIdsToRemove = form.ContainsKey("removefromcart")
                ? form["removefromcart"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                var remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    await ShoppingCartService.DeleteShoppingCartItemAsync(sci);
                else
                {
                    foreach (var formKey in form.Keys)
                        if (formKey.Equals($"itemquantity{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (int.TryParse(form[formKey], out var newQuantity))
                            {
                                var currSciWarnings = await ShoppingCartService.UpdateShoppingCartItemAsync(customer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                    sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                    newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }

                            break;
                        }
                }
            }

            //updated wishlist
            cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id);
            var model = new WishlistModel();
            model = await ShoppingCartModelFactory.PrepareWishlistModelAsync(model, cart);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!sciModel.Warnings.Contains(w))
                            sciModel.Warnings.Add(w);
            }

            return View(model);
        }

        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired("addtocartbutton")]
        public virtual async Task<IActionResult> AddItemsToCartFromWishlist(Guid? customerGuid, IFormCollection form)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("Homepage");

            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("Homepage");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var pageCustomer = customerGuid.HasValue
                ? await CustomerService.GetCustomerByGuidAsync(customerGuid.Value)
                : customer;
            if (pageCustomer == null)
                return RedirectToRoute("Homepage");

            var store = await StoreContext.GetCurrentStoreAsync();
            var pageCart = await ShoppingCartService.GetShoppingCartAsync(pageCustomer, ShoppingCartType.Wishlist, store.Id);

            var allWarnings = new List<string>();
            var countOfAddedItems = 0;
            var allIdsToAdd = form.ContainsKey("addtocart")
                ? form["addtocart"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                : new List<int>();
            foreach (var sci in pageCart)
            {
                if (allIdsToAdd.Contains(sci.Id))
                {
                    var product = await ProductService.GetProductByIdAsync(sci.ProductId);

                    var warnings = await ShoppingCartService.AddToCartAsync(customer,
                        product, ShoppingCartType.ShoppingCart,
                        store.Id,
                        sci.AttributesXml, sci.CustomerEnteredPrice,
                        sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, true);
                    if (!warnings.Any())
                        countOfAddedItems++;
                    if (ShoppingCartSettings.MoveItemsFromWishlistToCart && //settings enabled
                        !customerGuid.HasValue && //own wishlist
                        !warnings.Any()) //no warnings ( already in the cart)
                    {
                        //let's remove the item from wishlist
                        await ShoppingCartService.DeleteShoppingCartItemAsync(sci);
                    }

                    allWarnings.AddRange(warnings);
                }
            }

            if (countOfAddedItems > 0)
            {
                //redirect to the shopping cart page

                if (allWarnings.Any())
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Wishlist.AddToCart.Error"));
                }

                return RedirectToRoute("ShoppingCart");
            }
            else
            {
                NotificationService.WarningNotification(await LocalizationService.GetResourceAsync("Wishlist.AddToCart.NoAddedItems"));
            }
            //no items added. redisplay the wishlist page

            if (allWarnings.Any())
            {
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Wishlist.AddToCart.Error"));
            }

            var cart = await ShoppingCartService.GetShoppingCartAsync(pageCustomer, ShoppingCartType.Wishlist, store.Id);

            var model = new WishlistModel();
            model = await ShoppingCartModelFactory.PrepareWishlistModelAsync(model, cart, !customerGuid.HasValue);
            return View(model);
        }

        public virtual async Task<IActionResult> EmailWishlist()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) || !ShoppingCartSettings.EmailWishlistEnabled)
                return RedirectToRoute("Homepage");

            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(await WorkContext.GetCurrentCustomerAsync(), ShoppingCartType.Wishlist, store.Id);

            if (!cart.Any())
                return RedirectToRoute("Homepage");

            var model = new WishlistEmailAFriendModel();
            model = await ShoppingCartModelFactory.PrepareWishlistEmailAFriendModelAsync(model, false);
            return View(model);
        }

        [HttpPost, ActionName("EmailWishlist")]
        [FormValueRequired("send-email")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> EmailWishlistSend(WishlistEmailAFriendModel model, bool captchaValid)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) || !ShoppingCartSettings.EmailWishlistEnabled)
                return RedirectToRoute("Homepage");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id);

            if (!cart.Any())
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnEmailWishlistToFriendPage && !captchaValid)
            {
                ModelState.AddModelError(string.Empty, await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            //check whether the current customer is guest and ia allowed to email wishlist
            if (await CustomerService.IsGuestAsync(customer) && !ShoppingCartSettings.AllowAnonymousUsersToEmailWishlist)
            {
                ModelState.AddModelError(string.Empty, await LocalizationService.GetResourceAsync("Wishlist.EmailAFriend.OnlyRegisteredUsers"));
            }

            if (ModelState.IsValid)
            {
                //email
                await WorkflowMessageService.SendWishlistEmailAFriendMessageAsync(customer,
                        (await WorkContext.GetWorkingLanguageAsync()).Id, model.YourEmailAddress,
                        model.FriendEmail, _nopHtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model.SuccessfullySent = true;
                model.Result = await LocalizationService.GetResourceAsync("Wishlist.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = await ShoppingCartModelFactory.PrepareWishlistEmailAFriendModelAsync(model, true);

            return View(model);
        }

        #endregion
    }
}