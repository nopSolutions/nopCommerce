using System.Globalization;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Attributes;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Message token provider
    /// </summary>
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        protected readonly CatalogSettings _catalogSettings;
        protected readonly CurrencySettings _currencySettings;
        protected readonly IActionContextAccessor _actionContextAccessor;
        protected readonly IAddressService _addressService;
        protected readonly IAttributeFormatter<AddressAttribute, AddressAttributeValue> _addressAttributeFormatter;
        protected readonly IAttributeFormatter<CustomerAttribute, CustomerAttributeValue> _customerAttributeFormatter;
        protected readonly IAttributeFormatter<VendorAttribute, VendorAttributeValue> _vendorAttributeFormatter;
        protected readonly IBlogService _blogService;
        protected readonly ICountryService _countryService;
        protected readonly ICurrencyService _currencyService;
        protected readonly ICustomerService _customerService;
        protected readonly IDateTimeHelper _dateTimeHelper;
        protected readonly IEventPublisher _eventPublisher;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IGiftCardService _giftCardService;
        protected readonly IHtmlFormatter _htmlFormatter;
        protected readonly ILanguageService _languageService;
        protected readonly ILocalizationService _localizationService;
        protected readonly INewsService _newsService;
        protected readonly IOrderService _orderService;
        protected readonly IPaymentPluginManager _paymentPluginManager;
        protected readonly IPaymentService _paymentService;
        protected readonly IPriceFormatter _priceFormatter;
        protected readonly IProductService _productService;
        protected readonly IRewardPointService _rewardPointService;
        protected readonly IShipmentService _shipmentService;
        protected readonly IStateProvinceService _stateProvinceService;
        protected readonly IStoreContext _storeContext;
        protected readonly IStoreService _storeService;
        protected readonly IUrlHelperFactory _urlHelperFactory;
        protected readonly IUrlRecordService _urlRecordService;
        protected readonly IWorkContext _workContext;
        protected readonly MessageTemplatesSettings _templatesSettings;
        protected readonly PaymentSettings _paymentSettings;
        protected readonly StoreInformationSettings _storeInformationSettings;
        protected readonly TaxSettings _taxSettings;

        protected Dictionary<string, IEnumerable<string>> _allowedTokens;

        #endregion

        #region Ctor

        public MessageTokenProvider(CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            IAddressService addressService,
            IAttributeFormatter<AddressAttribute, AddressAttributeValue> addressAttributeFormatter,
            IAttributeFormatter<CustomerAttribute, CustomerAttributeValue> customerAttributeFormatter,
            IAttributeFormatter<VendorAttribute, VendorAttributeValue> vendorAttributeFormatter,
            IBlogService blogService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHtmlFormatter htmlFormatter,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INewsService newsService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IRewardPointService rewardPointService,
            IShipmentService shipmentService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MessageTemplatesSettings templatesSettings,
            PaymentSettings paymentSettings,
            StoreInformationSettings storeInformationSettings,
            TaxSettings taxSettings)
        {
            _catalogSettings = catalogSettings;
            _currencySettings = currencySettings;
            _actionContextAccessor = actionContextAccessor;
            _addressService = addressService;
            _addressAttributeFormatter = addressAttributeFormatter;
            _customerAttributeFormatter = customerAttributeFormatter;
            _vendorAttributeFormatter = vendorAttributeFormatter;
            _blogService = blogService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _htmlFormatter = htmlFormatter;
            _languageService = languageService;
            _localizationService = localizationService;
            _newsService = newsService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _rewardPointService = rewardPointService;
            _shipmentService = shipmentService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _storeService = storeService;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _templatesSettings = templatesSettings;
            _paymentSettings = paymentSettings;
            _storeInformationSettings = storeInformationSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Allowed tokens

        /// <summary>
        /// Get all available tokens by token groups
        /// </summary>
        protected Dictionary<string, IEnumerable<string>> AllowedTokens
        {
            get
            {
                if (_allowedTokens != null)
                    return _allowedTokens;

                _allowedTokens = new Dictionary<string, IEnumerable<string>>();

                //store tokens
                _allowedTokens.Add(TokenGroupNames.StoreTokens, new[]
                {
                    "%Store.Name%",
                    "%Store.URL%",
                    "%Store.Email%",
                    "%Store.CompanyName%",
                    "%Store.CompanyAddress%",
                    "%Store.CompanyPhoneNumber%",
                    "%Store.CompanyVat%",
                    "%Facebook.URL%",
                    "%Twitter.URL%",
                    "%YouTube.URL%",
                    "%Instagram.URL%"
                });

                //customer tokens
                _allowedTokens.Add(TokenGroupNames.CustomerTokens, new[]
                {
                    "%Customer.Email%",
                    "%Customer.Username%",
                    "%Customer.FullName%",
                    "%Customer.FirstName%",
                    "%Customer.LastName%",
                    "%Customer.VatNumber%",
                    "%Customer.VatNumberStatus%",
                    "%Customer.CustomAttributes%",
                    "%Customer.PasswordRecoveryURL%",
                    "%Customer.AccountActivationURL%",
                    "%Customer.EmailRevalidationURL%",
                    "%Wishlist.URLForCustomer%"
                });

                //order tokens
                _allowedTokens.Add(TokenGroupNames.OrderTokens, new[]
                {
                    "%Order.OrderNumber%",
                    "%Order.CustomerFullName%",
                    "%Order.CustomerEmail%",
                    "%Order.BillingFirstName%",
                    "%Order.BillingLastName%",
                    "%Order.BillingPhoneNumber%",
                    "%Order.BillingEmail%",
                    "%Order.BillingFaxNumber%",
                    "%Order.BillingCompany%",
                    "%Order.BillingAddress1%",
                    "%Order.BillingAddress2%",
                    "%Order.BillingCity%",
                    "%Order.BillingCounty%",
                    "%Order.BillingStateProvince%",
                    "%Order.BillingZipPostalCode%",
                    "%Order.BillingCountry%",
                    "%Order.BillingCustomAttributes%",
                    "%Order.BillingAddressLine%",
                    "%Order.Shippable%",
                    "%Order.ShippingMethod%",
                    "%Order.ShippingFirstName%",
                    "%Order.ShippingLastName%",
                    "%Order.ShippingPhoneNumber%",
                    "%Order.ShippingEmail%",
                    "%Order.ShippingFaxNumber%",
                    "%Order.ShippingCompany%",
                    "%Order.ShippingAddress1%",
                    "%Order.ShippingAddress2%",
                    "%Order.ShippingCity%",
                    "%Order.ShippingCounty%",
                    "%Order.ShippingStateProvince%",
                    "%Order.ShippingZipPostalCode%",
                    "%Order.ShippingCountry%",
                    "%Order.ShippingCustomAttributes%",
                    "%Order.ShippingAddressLine%",
                    "%Order.PaymentMethod%",
                    "%Order.VatNumber%",
                    "%Order.CustomValues%",
                    "%Order.Product(s)%",
                    "%Order.CreatedOn%",
                    "%Order.OrderURLForCustomer%",
                    "%Order.PickupInStore%",
                    "%Order.OrderId%",
                    "%Order.IsCompletelyShipped%",
                    "%Order.IsCompletelyReadyForPickup%",
                    "%Order.IsCompletelyDelivered%"
                });

                //shipment tokens
                _allowedTokens.Add(TokenGroupNames.ShipmentTokens, new[]
                {
                    "%Shipment.ShipmentNumber%",
                    "%Shipment.TrackingNumber%",
                    "%Shipment.TrackingNumberURL%",
                    "%Shipment.Product(s)%",
                    "%Shipment.URLForCustomer%"
                });

                //refunded order tokens
                _allowedTokens.Add(TokenGroupNames.RefundedOrderTokens, new[]
                {
                    "%Order.AmountRefunded%"
                });

                //order note tokens
                _allowedTokens.Add(TokenGroupNames.OrderNoteTokens, new[]
                {
                    "%Order.NewNoteText%",
                    "%Order.OrderNoteAttachmentUrl%"
                });

                //recurring payment tokens
                _allowedTokens.Add(TokenGroupNames.RecurringPaymentTokens, new[]
                {
                    "%RecurringPayment.ID%",
                    "%RecurringPayment.CancelAfterFailedPayment%",
                    "%RecurringPayment.RecurringPaymentType%"
                });

                //newsletter subscription tokens
                _allowedTokens.Add(TokenGroupNames.SubscriptionTokens, new[]
                {
                    "%NewsLetterSubscription.Email%",
                    "%NewsLetterSubscription.ActivationUrl%",
                    "%NewsLetterSubscription.DeactivationUrl%"
                });

                //product tokens
                _allowedTokens.Add(TokenGroupNames.ProductTokens, new[]
                {
                    "%Product.ID%",
                    "%Product.Name%",
                    "%Product.ShortDescription%",
                    "%Product.ProductURLForCustomer%",
                    "%Product.SKU%",
                    "%Product.StockQuantity%"
                });

                //return request tokens
                _allowedTokens.Add(TokenGroupNames.ReturnRequestTokens, new[]
                {
                    "%ReturnRequest.CustomNumber%",
                    "%ReturnRequest.OrderId%",
                    "%ReturnRequest.Product.Quantity%",
                    "%ReturnRequest.Product.Name%",
                    "%ReturnRequest.Reason%",
                    "%ReturnRequest.RequestedAction%",
                    "%ReturnRequest.CustomerComment%",
                    "%ReturnRequest.StaffNotes%",
                    "%ReturnRequest.Status%"
                });

                //forum tokens
                _allowedTokens.Add(TokenGroupNames.ForumTokens, new[]
                {
                    "%Forums.ForumURL%",
                    "%Forums.ForumName%"
                });

                //forum topic tokens
                _allowedTokens.Add(TokenGroupNames.ForumTopicTokens, new[]
                {
                    "%Forums.TopicURL%",
                    "%Forums.TopicName%"
                });

                //forum post tokens
                _allowedTokens.Add(TokenGroupNames.ForumPostTokens, new[]
                {
                    "%Forums.PostAuthor%",
                    "%Forums.PostBody%"
                });

                //private message tokens
                _allowedTokens.Add(TokenGroupNames.PrivateMessageTokens, new[]
                {
                    "%PrivateMessage.Subject%",
                    "%PrivateMessage.Text%"
                });

                //vendor tokens
                _allowedTokens.Add(TokenGroupNames.VendorTokens, new[]
                {
                    "%Vendor.Name%",
                    "%Vendor.Email%",
                    "%Vendor.VendorAttributes%"
                });

                //gift card tokens
                _allowedTokens.Add(TokenGroupNames.GiftCardTokens, new[]
                {
                    "%GiftCard.SenderName%",
                    "%GiftCard.SenderEmail%",
                    "%GiftCard.RecipientName%",
                    "%GiftCard.RecipientEmail%",
                    "%GiftCard.Amount%",
                    "%GiftCard.CouponCode%",
                    "%GiftCard.Message%"
                });

                //product review tokens
                _allowedTokens.Add(TokenGroupNames.ProductReviewTokens, new[]
                {
                    "%ProductReview.ProductName%",
                    "%ProductReview.Title%",
                    "%ProductReview.IsApproved%",
                    "%ProductReview.ReviewText%",
                    "%ProductReview.ReplyText%"
                });

                //attribute combination tokens
                _allowedTokens.Add(TokenGroupNames.AttributeCombinationTokens, new[]
                {
                    "%AttributeCombination.Formatted%",
                    "%AttributeCombination.SKU%",
                    "%AttributeCombination.StockQuantity%"
                });

                //blog comment tokens
                _allowedTokens.Add(TokenGroupNames.BlogCommentTokens, new[]
                {
                    "%BlogComment.BlogPostTitle%"
                });

                //news comment tokens
                _allowedTokens.Add(TokenGroupNames.NewsCommentTokens, new[]
                {
                    "%NewsComment.NewsTitle%"
                });

                //product back in stock tokens
                _allowedTokens.Add(TokenGroupNames.ProductBackInStockTokens, new[]
                {
                    "%BackInStockSubscription.ProductName%",
                    "%BackInStockSubscription.ProductUrl%"
                });

                //email a friend tokens
                _allowedTokens.Add(TokenGroupNames.EmailAFriendTokens, new[]
                {
                    "%EmailAFriend.PersonalMessage%",
                    "%EmailAFriend.Email%"
                });

                //wishlist to friend tokens
                _allowedTokens.Add(TokenGroupNames.WishlistToFriendTokens, new[]
                {
                    "%Wishlist.PersonalMessage%",
                    "%Wishlist.Email%"
                });

                //VAT validation tokens
                _allowedTokens.Add(TokenGroupNames.VatValidation, new[]
                {
                    "%VatValidationResult.Name%",
                    "%VatValidationResult.Address%"
                });

                //contact us tokens
                _allowedTokens.Add(TokenGroupNames.ContactUs, new[]
                {
                    "%ContactUs.SenderEmail%",
                    "%ContactUs.SenderName%",
                    "%ContactUs.Body%"
                });

                //contact vendor tokens
                _allowedTokens.Add(TokenGroupNames.ContactVendor, new[]
                {
                    "%ContactUs.SenderEmail%",
                    "%ContactUs.SenderName%",
                    "%ContactUs.Body%"
                });

                return _allowedTokens;
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Convert a collection to a HTML table
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="vendorId">Vendor identifier (used to limit products by vendor</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the hTML table of products
        /// </returns>
        protected virtual async Task<string> ProductListToHtmlTableAsync(Order order, int languageId, int vendorId)
        {
            var language = await _languageService.GetLanguageByIdAsync(languageId);

            var sb = new StringBuilder();
            sb.AppendLine("<table border=\"0\" style=\"width:100%;\">");

            sb.AppendLine($"<tr style=\"background-color:{_templatesSettings.Color1};text-align:center;\">");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Name", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Price", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Quantity", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Total", languageId)}</th>");
            sb.AppendLine("</tr>");

            var table = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendorId);
            for (var i = 0; i <= table.Count - 1; i++)
            {
                var orderItem = table[i];

                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                if (product == null)
                    continue;

                sb.AppendLine($"<tr style=\"background-color: {_templatesSettings.Color2};text-align: center;\">");
                //product name
                var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId);

                sb.AppendLine("<td style=\"padding: 0.6em 0.4em;text-align: left;\">" + WebUtility.HtmlEncode(productName));

                //add download link
                if (await _orderService.IsDownloadAllowedAsync(orderItem))
                {
                    var downloadUrl = await RouteUrlAsync(order.StoreId, "GetDownload", new { orderItemId = orderItem.OrderItemGuid });
                    var downloadLink = $"<a class=\"link\" href=\"{downloadUrl}\">{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Download", languageId)}</a>";
                    sb.AppendLine("<br />");
                    sb.AppendLine(downloadLink);
                }
                //add download link
                if (await _orderService.IsLicenseDownloadAllowedAsync(orderItem))
                {
                    var licenseUrl = await RouteUrlAsync(order.StoreId, "GetLicense", new { orderItemId = orderItem.OrderItemGuid });
                    var licenseLink = $"<a class=\"link\" href=\"{licenseUrl}\">{await _localizationService.GetResourceAsync("Messages.Order.Product(s).License", languageId)}</a>";
                    sb.AppendLine("<br />");
                    sb.AppendLine(licenseLink);
                }
                //attributes
                if (!string.IsNullOrEmpty(orderItem.AttributeDescription))
                {
                    sb.AppendLine("<br />");
                    sb.AppendLine(orderItem.AttributeDescription);
                }
                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : string.Empty;
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : string.Empty;
                    var rentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                    sb.AppendLine("<br />");
                    sb.AppendLine(rentalInfo);
                }
                //SKU
                if (_catalogSettings.ShowSkuOnProductDetailsPage)
                {
                    var sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml);
                    if (!string.IsNullOrEmpty(sku))
                    {
                        sb.AppendLine("<br />");
                        sb.AppendLine(string.Format(await _localizationService.GetResourceAsync("Messages.Order.Product(s).SKU", languageId), WebUtility.HtmlEncode(sku)));
                    }
                }

                sb.AppendLine("</td>");

                string unitPriceStr;
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                    unitPriceStr = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                    unitPriceStr = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                }

                sb.AppendLine($"<td style=\"padding: 0.6em 0.4em;text-align: right;\">{unitPriceStr}</td>");

                sb.AppendLine($"<td style=\"padding: 0.6em 0.4em;text-align: center;\">{orderItem.Quantity}</td>");

                string priceStr;
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                    priceStr = await _priceFormatter.FormatPriceAsync(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                    priceStr = await _priceFormatter.FormatPriceAsync(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                }

                sb.AppendLine($"<td style=\"padding: 0.6em 0.4em;text-align: right;\">{priceStr}</td>");

                sb.AppendLine("</tr>");
            }

            if (vendorId == 0)
            {
                //we render checkout attributes and totals only for store owners (hide for vendors)

                if (!string.IsNullOrEmpty(order.CheckoutAttributeDescription))
                {
                    sb.AppendLine("<tr><td style=\"text-align:right;\" colspan=\"1\">&nbsp;</td><td colspan=\"3\" style=\"text-align:right\">");
                    sb.AppendLine(order.CheckoutAttributeDescription);
                    sb.AppendLine("</td></tr>");
                }

                //totals
                await WriteTotalsAsync(order, language, sb);
            }

            sb.AppendLine("</table>");
            var result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Write order totals
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="language">Language</param>
        /// <param name="sb">StringBuilder</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task WriteTotalsAsync(Order order, Language language, StringBuilder sb)
        {
            //subtotal
            string cusSubTotal;
            var displaySubTotalDiscount = false;
            var cusSubTotalDiscount = string.Empty;
            var languageId = language.Id;
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax

                //subtotal
                var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                cusSubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                {
                    cusSubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                    displaySubTotalDiscount = true;
                }
            }
            else
            {
                //excluding tax

                //subtotal
                var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                cusSubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                {
                    cusSubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                    displaySubTotalDiscount = true;
                }
            }

            //shipping, payment method fee
            string cusShipTotal;
            string cusPaymentMethodAdditionalFee;
            var taxRates = new SortedDictionary<decimal, decimal>();
            var cusTaxTotal = string.Empty;
            var cusDiscount = string.Empty;
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax

                //shipping
                var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                cusShipTotal = await _priceFormatter.FormatShippingPriceAsync(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                //payment method additional fee
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                cusPaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax

                //shipping
                var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                cusShipTotal = await _priceFormatter.FormatShippingPriceAsync(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                //payment method additional fee
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                cusPaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }

            //shipping
            var displayShipping = order.ShippingStatus != ShippingStatus.ShippingNotRequired;

            //payment method fee
            var displayPaymentMethodFee = order.PaymentMethodAdditionalFeeExclTax > decimal.Zero;

            //tax
            bool displayTax;
            bool displayTaxRates;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    taxRates = new SortedDictionary<decimal, decimal>();
                    foreach (var tr in _orderService.ParseTaxRates(order, order.TaxRates))
                        taxRates.Add(tr.Key, _currencyService.ConvertCurrency(tr.Value, order.CurrencyRate));

                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    var taxStr = await _priceFormatter.FormatPriceAsync(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                        false, languageId);
                    cusTaxTotal = taxStr;
                }
            }

            //discount
            var displayDiscount = false;
            if (order.OrderDiscount > decimal.Zero)
            {
                var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
                cusDiscount = await _priceFormatter.FormatPriceAsync(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);
                displayDiscount = true;
            }

            //total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            var cusTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

            //subtotal
            sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.SubTotal", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusSubTotal}</strong></td></tr>");

            //discount (applied to order subtotal)
            if (displaySubTotalDiscount)
            {
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.SubTotalDiscount", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusSubTotalDiscount}</strong></td></tr>");
            }

            //shipping
            if (displayShipping)
            {
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.Shipping", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusShipTotal}</strong></td></tr>");
            }

            //payment method fee
            if (displayPaymentMethodFee)
            {
                var paymentMethodFeeTitle = await _localizationService.GetResourceAsync("Messages.Order.PaymentMethodAdditionalFee", languageId);
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{paymentMethodFeeTitle}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusPaymentMethodAdditionalFee}</strong></td></tr>");
            }

            //tax
            if (displayTax)
            {
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.Tax", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusTaxTotal}</strong></td></tr>");
            }

            if (displayTaxRates)
            {
                foreach (var item in taxRates)
                {
                    var taxRate = string.Format(await _localizationService.GetResourceAsync("Messages.Order.TaxRateLine"),
                        _priceFormatter.FormatTaxRate(item.Key));
                    var taxValue = await _priceFormatter.FormatPriceAsync(item.Value, true, order.CustomerCurrencyCode, false, languageId);
                    sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{taxRate}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{taxValue}</strong></td></tr>");
                }
            }

            //discount
            if (displayDiscount)
            {
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.TotalDiscount", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusDiscount}</strong></td></tr>");
            }

            //gift cards
            foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
            {
                var giftCardText = string.Format(await _localizationService.GetResourceAsync("Messages.Order.GiftCardInfo", languageId),
                    WebUtility.HtmlEncode((await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId))?.GiftCardCouponCode));
                var giftCardAmount = await _priceFormatter.FormatPriceAsync(-_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate), true, order.CustomerCurrencyCode,
                    false, languageId);
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{giftCardText}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{giftCardAmount}</strong></td></tr>");
            }

            //reward points
            if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
            {
                var rpTitle = string.Format(await _localizationService.GetResourceAsync("Messages.Order.RewardPoints", languageId),
                    -redeemedRewardPointsEntry.Points);
                var rpAmount = await _priceFormatter.FormatPriceAsync(-_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate), true,
                    order.CustomerCurrencyCode, false, languageId);
                sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{rpTitle}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{rpAmount}</strong></td></tr>");
            }

            //total
            sb.AppendLine($"<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.OrderTotal", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusTotal}</strong></td></tr>");
        }

        /// <summary>
        /// Convert a collection to a HTML table
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the hTML table of products
        /// </returns>
        protected virtual async Task<string> ProductListToHtmlTableAsync(Shipment shipment, int languageId)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table border=\"0\" style=\"width:100%;\">");

            sb.AppendLine($"<tr style=\"background-color:{_templatesSettings.Color1};text-align:center;\">");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Name", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Quantity", languageId)}</th>");
            sb.AppendLine("</tr>");

            var table = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);
            for (var i = 0; i <= table.Count - 1; i++)
            {
                var si = table[i];
                var orderItem = await _orderService.GetOrderItemByIdAsync(si.OrderItemId);

                if (orderItem == null)
                    continue;

                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                if (product == null)
                    continue;

                sb.AppendLine($"<tr style=\"background-color: {_templatesSettings.Color2};text-align: center;\">");
                //product name
                var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId);

                sb.AppendLine("<td style=\"padding: 0.6em 0.4em;text-align: left;\">" + WebUtility.HtmlEncode(productName));

                //attributes
                if (!string.IsNullOrEmpty(orderItem.AttributeDescription))
                {
                    sb.AppendLine("<br />");
                    sb.AppendLine(orderItem.AttributeDescription);
                }

                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : string.Empty;
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : string.Empty;
                    var rentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                    sb.AppendLine("<br />");
                    sb.AppendLine(rentalInfo);
                }

                //SKU
                if (_catalogSettings.ShowSkuOnProductDetailsPage)
                {
                    var sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml);
                    if (!string.IsNullOrEmpty(sku))
                    {
                        sb.AppendLine("<br />");
                        sb.AppendLine(string.Format(await _localizationService.GetResourceAsync("Messages.Order.Product(s).SKU", languageId), WebUtility.HtmlEncode(sku)));
                    }
                }

                sb.AppendLine("</td>");

                sb.AppendLine($"<td style=\"padding: 0.6em 0.4em;text-align: center;\">{si.Quantity}</td>");

                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            var result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Generates an absolute URL for the specified store, routeName and route values
        /// </summary>
        /// <param name="storeId">Store identifier; Pass 0 to load URL of the current store</param>
        /// <param name="routeName">The name of the route that is used to generate URL</param>
        /// <param name="routeValues">An object that contains route values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the generated URL
        /// </returns>
        protected virtual async Task<string> RouteUrlAsync(int storeId = 0, string routeName = null, object routeValues = null)
        {
            //try to get a store by the passed identifier
            var store = await _storeService.GetStoreByIdAsync(storeId) ?? await _storeContext.GetCurrentStoreAsync()
                ?? throw new Exception("No store could be loaded");

            //ensure that the store URL is specified
            if (string.IsNullOrEmpty(store.Url))
                throw new Exception("URL cannot be null");

            //generate the relative URL
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var url = urlHelper.RouteUrl(routeName, routeValues);

            //compose the result
            return new Uri(new Uri(store.Url), url).AbsoluteUri;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add store tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="store">Store</param>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddStoreTokensAsync(IList<Token> tokens, Store store, EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            tokens.Add(new Token("Store.Name", await _localizationService.GetLocalizedAsync(store, x => x.Name)));
            tokens.Add(new Token("Store.URL", store.Url, true));
            tokens.Add(new Token("Store.Email", emailAccount.Email));
            tokens.Add(new Token("Store.CompanyName", store.CompanyName));
            tokens.Add(new Token("Store.CompanyAddress", store.CompanyAddress));
            tokens.Add(new Token("Store.CompanyPhoneNumber", store.CompanyPhoneNumber));
            tokens.Add(new Token("Store.CompanyVat", store.CompanyVat));

            tokens.Add(new Token("Facebook.URL", _storeInformationSettings.FacebookLink));
            tokens.Add(new Token("Twitter.URL", _storeInformationSettings.TwitterLink));
            tokens.Add(new Token("YouTube.URL", _storeInformationSettings.YoutubeLink));
            tokens.Add(new Token("Instagram.URL", _storeInformationSettings.InstagramLink));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(store, tokens);
        }

        /// <summary>
        /// Add order tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="order"></param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddOrderTokensAsync(IList<Token> tokens, Order order, int languageId, int vendorId = 0)
        {
            //lambda expression for choosing correct order address
            async Task<Address> orderAddress(Order o) => await _addressService.GetAddressByIdAsync((o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) ?? 0);

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
            var (billingAddressLine, _) = await _addressService.FormatAddressAsync(billingAddress, languageId);
            var (shippingAddressLine, _) = await _addressService.FormatAddressAsync(await orderAddress(order), languageId);

            tokens.Add(new Token("Order.OrderId", order.Id));
            tokens.Add(new Token("Order.OrderNumber", order.CustomOrderNumber));

            tokens.Add(new Token("Order.CustomerFullName", $"{billingAddress.FirstName} {billingAddress.LastName}"));
            tokens.Add(new Token("Order.CustomerEmail", billingAddress.Email));

            tokens.Add(new Token("Order.BillingFirstName", billingAddress.FirstName));
            tokens.Add(new Token("Order.BillingLastName", billingAddress.LastName));
            tokens.Add(new Token("Order.BillingPhoneNumber", billingAddress.PhoneNumber));
            tokens.Add(new Token("Order.BillingEmail", billingAddress.Email));
            tokens.Add(new Token("Order.BillingFaxNumber", billingAddress.FaxNumber));
            tokens.Add(new Token("Order.BillingCompany", billingAddress.Company));
            tokens.Add(new Token("Order.BillingAddress1", billingAddress.Address1));
            tokens.Add(new Token("Order.BillingAddress2", billingAddress.Address2));
            tokens.Add(new Token("Order.BillingCity", billingAddress.City));
            tokens.Add(new Token("Order.BillingCounty", billingAddress.County));
            tokens.Add(new Token("Order.BillingStateProvince", await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress) is StateProvince billingStateProvince ? await _localizationService.GetLocalizedAsync(billingStateProvince, x => x.Name) : string.Empty));
            tokens.Add(new Token("Order.BillingZipPostalCode", billingAddress.ZipPostalCode));
            tokens.Add(new Token("Order.BillingCountry", await _countryService.GetCountryByAddressAsync(billingAddress) is Country billingCountry ? await _localizationService.GetLocalizedAsync(billingCountry, x => x.Name) : string.Empty));
            tokens.Add(new Token("Order.BillingCustomAttributes", await _addressAttributeFormatter.FormatAttributesAsync(billingAddress.CustomAttributes), true));
            tokens.Add(new Token("Order.BillingAddressLine", billingAddressLine));
            tokens.Add(new Token("Order.Shippable", !string.IsNullOrEmpty(order.ShippingMethod)));
            tokens.Add(new Token("Order.ShippingMethod", order.ShippingMethod));
            tokens.Add(new Token("Order.PickupInStore", order.PickupInStore));
            tokens.Add(new Token("Order.ShippingFirstName", (await orderAddress(order))?.FirstName ?? string.Empty));
            tokens.Add(new Token("Order.ShippingLastName", (await orderAddress(order))?.LastName ?? string.Empty));
            tokens.Add(new Token("Order.ShippingPhoneNumber", (await orderAddress(order))?.PhoneNumber ?? string.Empty));
            tokens.Add(new Token("Order.ShippingEmail", (await orderAddress(order))?.Email ?? string.Empty));
            tokens.Add(new Token("Order.ShippingFaxNumber", (await orderAddress(order))?.FaxNumber ?? string.Empty));
            tokens.Add(new Token("Order.ShippingCompany", (await orderAddress(order))?.Company ?? string.Empty));
            tokens.Add(new Token("Order.ShippingAddress1", (await orderAddress(order))?.Address1 ?? string.Empty));
            tokens.Add(new Token("Order.ShippingAddress2", (await orderAddress(order))?.Address2 ?? string.Empty));
            tokens.Add(new Token("Order.ShippingCity", (await orderAddress(order))?.City ?? string.Empty));
            tokens.Add(new Token("Order.ShippingCounty", (await orderAddress(order))?.County ?? string.Empty));
            tokens.Add(new Token("Order.ShippingStateProvince", await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(order)) is StateProvince shippingStateProvince ? await _localizationService.GetLocalizedAsync(shippingStateProvince, x => x.Name) : string.Empty));
            tokens.Add(new Token("Order.ShippingZipPostalCode", (await orderAddress(order))?.ZipPostalCode ?? string.Empty));
            tokens.Add(new Token("Order.ShippingCountry", await _countryService.GetCountryByAddressAsync(await orderAddress(order)) is Country orderCountry ? await _localizationService.GetLocalizedAsync(orderCountry, x => x.Name) : string.Empty));
            tokens.Add(new Token("Order.ShippingCustomAttributes", await _addressAttributeFormatter.FormatAttributesAsync((await orderAddress(order))?.CustomAttributes ?? string.Empty), true));
            tokens.Add(new Token("Order.ShippingAddressLine", shippingAddressLine));
            tokens.Add(new Token("Order.IsCompletelyShipped", !order.PickupInStore && order.ShippingStatus == ShippingStatus.Shipped));
            tokens.Add(new Token("Order.IsCompletelyReadyForPickup", order.PickupInStore && !await _orderService.HasItemsToAddToShipmentAsync(order) && !await _orderService.HasItemsToReadyForPickupAsync(order)));
            tokens.Add(new Token("Order.IsCompletelyDelivered", order.ShippingStatus == ShippingStatus.Delivered));

            var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName);
            var paymentMethodName = paymentMethod != null ? await _localizationService.GetLocalizedFriendlyNameAsync(paymentMethod, (await _workContext.GetWorkingLanguageAsync()).Id) : order.PaymentMethodSystemName;
            tokens.Add(new Token("Order.PaymentMethod", paymentMethodName));
            tokens.Add(new Token("Order.VatNumber", order.VatNumber));
            var sbCustomValues = new StringBuilder();
            var customValues = _paymentService.DeserializeCustomValues(order);
            if (customValues != null)
            {
                foreach (var item in customValues)
                {
                    sbCustomValues.AppendFormat("{0}: {1}", WebUtility.HtmlEncode(item.Key), WebUtility.HtmlEncode(item.Value != null ? item.Value.ToString() : string.Empty));
                    sbCustomValues.Append("<br />");
                }
            }

            tokens.Add(new Token("Order.CustomValues", sbCustomValues.ToString(), true));

            tokens.Add(new Token("Order.Product(s)", await ProductListToHtmlTableAsync(order, languageId, vendorId), true));

            var language = await _languageService.GetLanguageByIdAsync(languageId);
            if (language != null && !string.IsNullOrEmpty(language.LanguageCulture))
            {
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                var createdOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, TimeZoneInfo.Utc, await _dateTimeHelper.GetCustomerTimeZoneAsync(customer));
                tokens.Add(new Token("Order.CreatedOn", createdOn.ToString("D", new CultureInfo(language.LanguageCulture))));
            }
            else
            {
                tokens.Add(new Token("Order.CreatedOn", order.CreatedOnUtc.ToString("D")));
            }

            var orderUrl = await RouteUrlAsync(order.StoreId, "OrderDetails", new { orderId = order.Id });
            tokens.Add(new Token("Order.OrderURLForCustomer", orderUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(order, tokens);
        }

        /// <summary>
        /// Add refunded order tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="order">Order</param>
        /// <param name="refundedAmount">Refunded amount of order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddOrderRefundedTokensAsync(IList<Token> tokens, Order order, decimal refundedAmount)
        {
            //should we convert it to customer currency?
            //most probably, no. It can cause some rounding or legal issues
            //furthermore, exchange rate could be changed
            //so let's display it the primary store currency

            var primaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            var refundedAmountStr = await _priceFormatter.FormatPriceAsync(refundedAmount, true, primaryStoreCurrencyCode, false, (await _workContext.GetWorkingLanguageAsync()).Id);

            tokens.Add(new Token("Order.AmountRefunded", refundedAmountStr));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(order, tokens);
        }

        /// <summary>
        /// Add shipment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="shipment">Shipment item</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddShipmentTokensAsync(IList<Token> tokens, Shipment shipment, int languageId)
        {
            tokens.Add(new Token("Shipment.ShipmentNumber", shipment.Id));
            tokens.Add(new Token("Shipment.TrackingNumber", shipment.TrackingNumber));
            var trackingNumberUrl = string.Empty;
            if (!string.IsNullOrEmpty(shipment.TrackingNumber))
            {
                var shipmentTracker = await _shipmentService.GetShipmentTrackerAsync(shipment);
                if (shipmentTracker != null)
                    trackingNumberUrl = await shipmentTracker.GetUrlAsync(shipment.TrackingNumber, shipment);
            }

            tokens.Add(new Token("Shipment.TrackingNumberURL", trackingNumberUrl, true));
            tokens.Add(new Token("Shipment.Product(s)", await ProductListToHtmlTableAsync(shipment, languageId), true));

            var shipmentUrl = await RouteUrlAsync((await _orderService.GetOrderByIdAsync(shipment.OrderId)).StoreId, "ShipmentDetails", new { shipmentId = shipment.Id });
            tokens.Add(new Token("Shipment.URLForCustomer", shipmentUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(shipment, tokens);
        }

        /// <summary>
        /// Add order note tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="orderNote">Order note</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddOrderNoteTokensAsync(IList<Token> tokens, OrderNote orderNote)
        {
            var order = await _orderService.GetOrderByIdAsync(orderNote.OrderId);

            tokens.Add(new Token("Order.NewNoteText", _orderService.FormatOrderNoteText(orderNote), true));
            var orderNoteAttachmentUrl = await RouteUrlAsync(order.StoreId, "GetOrderNoteFile", new { ordernoteid = orderNote.Id });
            tokens.Add(new Token("Order.OrderNoteAttachmentUrl", orderNoteAttachmentUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(orderNote, tokens);
        }

        /// <summary>
        /// Add recurring payment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddRecurringPaymentTokensAsync(IList<Token> tokens, RecurringPayment recurringPayment)
        {
            tokens.Add(new Token("RecurringPayment.ID", recurringPayment.Id));
            tokens.Add(new Token("RecurringPayment.CancelAfterFailedPayment",
                recurringPayment.LastPaymentFailed && _paymentSettings.CancelRecurringPaymentsAfterFailedPayment));
            if (await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId) is Order order)
                tokens.Add(new Token("RecurringPayment.RecurringPaymentType", (await _paymentService.GetRecurringPaymentTypeAsync(order.PaymentMethodSystemName)).ToString()));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(recurringPayment, tokens);
        }

        /// <summary>
        /// Add return request tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddReturnRequestTokensAsync(IList<Token> tokens, ReturnRequest returnRequest, OrderItem orderItem, int languageId)
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            tokens.Add(new Token("ReturnRequest.CustomNumber", returnRequest.CustomNumber));
            tokens.Add(new Token("ReturnRequest.OrderId", orderItem.OrderId));
            tokens.Add(new Token("ReturnRequest.Product.Quantity", returnRequest.Quantity));
            tokens.Add(new Token("ReturnRequest.Product.Name", await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId)));
            tokens.Add(new Token("ReturnRequest.Reason", returnRequest.ReasonForReturn));
            tokens.Add(new Token("ReturnRequest.RequestedAction", returnRequest.RequestedAction));
            tokens.Add(new Token("ReturnRequest.CustomerComment", _htmlFormatter.FormatText(returnRequest.CustomerComments, false, true, false, false, false, false), true));
            tokens.Add(new Token("ReturnRequest.StaffNotes", _htmlFormatter.FormatText(returnRequest.StaffNotes, false, true, false, false, false, false), true));
            tokens.Add(new Token("ReturnRequest.Status", await _localizationService.GetLocalizedEnumAsync(returnRequest.ReturnRequestStatus, languageId)));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(returnRequest, tokens);
        }

        /// <summary>
        /// Add gift card tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="giftCard">Gift card</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddGiftCardTokensAsync(IList<Token> tokens, GiftCard giftCard, int languageId)
        {
            tokens.Add(new Token("GiftCard.SenderName", giftCard.SenderName));
            tokens.Add(new Token("GiftCard.SenderEmail", giftCard.SenderEmail));
            tokens.Add(new Token("GiftCard.RecipientName", giftCard.RecipientName));
            tokens.Add(new Token("GiftCard.RecipientEmail", giftCard.RecipientEmail));

            var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
            tokens.Add(new Token("GiftCard.Amount", await _priceFormatter.FormatPriceAsync(giftCard.Amount, true, primaryStoreCurrency.CurrencyCode, false, languageId)));
            tokens.Add(new Token("GiftCard.CouponCode", giftCard.GiftCardCouponCode));

            var giftCardMessage = !string.IsNullOrWhiteSpace(giftCard.Message) ?
                _htmlFormatter.FormatText(giftCard.Message, false, true, false, false, false, false) : string.Empty;

            tokens.Add(new Token("GiftCard.Message", giftCardMessage, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(giftCard, tokens);
        }

        /// <summary>
        /// Add customer tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddCustomerTokensAsync(IList<Token> tokens, int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentOutOfRangeException(nameof(customerId));

            var customer = await _customerService.GetCustomerByIdAsync(customerId);

            await AddCustomerTokensAsync(tokens, customer);
        }

        /// <summary>
        /// Add customer tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddCustomerTokensAsync(IList<Token> tokens, Customer customer)
        {
            tokens.Add(new Token("Customer.Email", customer.Email));
            tokens.Add(new Token("Customer.Username", customer.Username));
            tokens.Add(new Token("Customer.FullName", await _customerService.GetCustomerFullNameAsync(customer)));
            tokens.Add(new Token("Customer.FirstName", customer.FirstName));
            tokens.Add(new Token("Customer.LastName", customer.LastName));
            tokens.Add(new Token("Customer.VatNumber", customer.VatNumber));
            tokens.Add(new Token("Customer.VatNumberStatus", ((VatNumberStatus)customer.VatNumberStatusId).ToString()));

            var customAttributesXml = customer.CustomCustomerAttributesXML;
            tokens.Add(new Token("Customer.CustomAttributes", await _customerAttributeFormatter.FormatAttributesAsync(customAttributesXml), true));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            var passwordRecoveryUrl = await RouteUrlAsync(routeName: "PasswordRecoveryConfirm", routeValues: new { token = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute), guid = customer.CustomerGuid });
            var accountActivationUrl = await RouteUrlAsync(routeName: "AccountActivation", routeValues: new { token = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.AccountActivationTokenAttribute), guid = customer.CustomerGuid });
            var emailRevalidationUrl = await RouteUrlAsync(routeName: "EmailRevalidation", routeValues: new { token = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute), guid = customer.CustomerGuid });
            var wishlistUrl = await RouteUrlAsync(routeName: "Wishlist", routeValues: new { customerGuid = customer.CustomerGuid });
            tokens.Add(new Token("Customer.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("Customer.AccountActivationURL", accountActivationUrl, true));
            tokens.Add(new Token("Customer.EmailRevalidationURL", emailRevalidationUrl, true));
            tokens.Add(new Token("Wishlist.URLForCustomer", wishlistUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(customer, tokens);
        }

        /// <summary>
        /// Add vendor tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="vendor">Vendor</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddVendorTokensAsync(IList<Token> tokens, Vendor vendor)
        {
            tokens.Add(new Token("Vendor.Name", vendor.Name));
            tokens.Add(new Token("Vendor.Email", vendor.Email));

            var vendorAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(vendor, NopVendorDefaults.VendorAttributes);
            tokens.Add(new Token("Vendor.VendorAttributes", await _vendorAttributeFormatter.FormatAttributesAsync(vendorAttributesXml), true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(vendor, tokens);
        }

        /// <summary>
        /// Add newsletter subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">Newsletter subscription</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddNewsLetterSubscriptionTokensAsync(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            tokens.Add(new Token("NewsLetterSubscription.Email", subscription.Email));

            var activationUrl = await RouteUrlAsync(routeName: "NewsletterActivation", routeValues: new { token = subscription.NewsLetterSubscriptionGuid, active = "true" });
            tokens.Add(new Token("NewsLetterSubscription.ActivationUrl", activationUrl, true));

            var deactivationUrl = await RouteUrlAsync(routeName: "NewsletterActivation", routeValues: new { token = subscription.NewsLetterSubscriptionGuid, active = "false" });
            tokens.Add(new Token("NewsLetterSubscription.DeactivationUrl", deactivationUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(subscription, tokens);
        }

        /// <summary>
        /// Add product review tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="productReview">Product review</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddProductReviewTokensAsync(IList<Token> tokens, ProductReview productReview)
        {
            tokens.Add(new Token("ProductReview.ProductName", (await _productService.GetProductByIdAsync(productReview.ProductId))?.Name));
            tokens.Add(new Token("ProductReview.Title", productReview.Title));
            tokens.Add(new Token("ProductReview.IsApproved", productReview.IsApproved));
            tokens.Add(new Token("ProductReview.ReviewText", productReview.ReviewText));
            tokens.Add(new Token("ProductReview.ReplyText", productReview.ReplyText));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(productReview, tokens);
        }

        /// <summary>
        /// Add blog comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="blogComment">Blog post comment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddBlogCommentTokensAsync(IList<Token> tokens, BlogComment blogComment)
        {
            var blogPost = await _blogService.GetBlogPostByIdAsync(blogComment.BlogPostId);

            tokens.Add(new Token("BlogComment.BlogPostTitle", blogPost.Title));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(blogComment, tokens);
        }

        /// <summary>
        /// Add news comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="newsComment">News comment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddNewsCommentTokensAsync(IList<Token> tokens, NewsComment newsComment)
        {
            var newsItem = await _newsService.GetNewsByIdAsync(newsComment.NewsItemId);

            tokens.Add(new Token("NewsComment.NewsTitle", newsItem.Title));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(newsComment, tokens);
        }

        /// <summary>
        /// Add product tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="product">Product</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddProductTokensAsync(IList<Token> tokens, Product product, int languageId)
        {
            tokens.Add(new Token("Product.ID", product.Id));
            tokens.Add(new Token("Product.Name", await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId)));
            tokens.Add(new Token("Product.ShortDescription", await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription, languageId), true));
            tokens.Add(new Token("Product.SKU", product.Sku));
            tokens.Add(new Token("Product.StockQuantity", await _productService.GetTotalStockQuantityAsync(product)));

            var seName = await _urlRecordService.GetSeNameAsync(product);
            var productUrl = await RouteUrlAsync(routeName: "ProductDetails", routeValues: new { SeName = seName });
            tokens.Add(new Token("Product.ProductURLForCustomer", productUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(product, tokens);
        }

        /// <summary>
        /// Add product attribute combination tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="combination">Product attribute combination</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddAttributeCombinationTokensAsync(IList<Token> tokens, ProductAttributeCombination combination, int languageId)
        {
            //attributes
            //we cannot inject IProductAttributeFormatter into constructor because it'll cause circular references.
            //that's why we resolve it here this way
            var productAttributeFormatter = EngineContext.Current.Resolve<IProductAttributeFormatter>();

            var product = await _productService.GetProductByIdAsync(combination.ProductId);
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            var attributes = await productAttributeFormatter.FormatAttributesAsync(product,
                combination.AttributesXml,
                currentCustomer,
                currentStore,
                renderPrices: false);

            tokens.Add(new Token("AttributeCombination.Formatted", attributes, true));
            tokens.Add(new Token("AttributeCombination.SKU", await _productService.FormatSkuAsync(await _productService.GetProductByIdAsync(combination.ProductId), combination.AttributesXml)));
            tokens.Add(new Token("AttributeCombination.StockQuantity", combination.StockQuantity));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(combination, tokens);
        }

        /// <summary>
        /// Add forum topic tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="appendedPostIdentifierAnchor">Forum post identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddForumTopicTokensAsync(IList<Token> tokens, ForumTopic forumTopic,
            int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null)
        {
            //attributes
            //we cannot inject IForumService into constructor because it'll cause circular references.
            //that's why we resolve it here this way
            var forumService = EngineContext.Current.Resolve<IForumService>();

            string topicUrl;
            if (friendlyForumTopicPageIndex.HasValue && friendlyForumTopicPageIndex.Value > 1)
                topicUrl = await RouteUrlAsync(routeName: "TopicSlugPaged", routeValues: new { id = forumTopic.Id, slug = await forumService.GetTopicSeNameAsync(forumTopic), pageNumber = friendlyForumTopicPageIndex.Value });
            else
                topicUrl = await RouteUrlAsync(routeName: "TopicSlug", routeValues: new { id = forumTopic.Id, slug = await forumService.GetTopicSeNameAsync(forumTopic) });
            if (appendedPostIdentifierAnchor.HasValue && appendedPostIdentifierAnchor.Value > 0)
                topicUrl = $"{topicUrl}#{appendedPostIdentifierAnchor.Value}";
            tokens.Add(new Token("Forums.TopicURL", topicUrl, true));
            tokens.Add(new Token("Forums.TopicName", forumTopic.Subject));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(forumTopic, tokens);
        }

        /// <summary>
        /// Add forum post tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumPost">Forum post</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddForumPostTokensAsync(IList<Token> tokens, ForumPost forumPost)
        {
            //attributes
            //we cannot inject IForumService into constructor because it'll cause circular references.
            //that's why we resolve it here this way
            var forumService = EngineContext.Current.Resolve<IForumService>();

            var customer = await _customerService.GetCustomerByIdAsync(forumPost.CustomerId);

            tokens.Add(new Token("Forums.PostAuthor", await _customerService.FormatUsernameAsync(customer)));
            tokens.Add(new Token("Forums.PostBody", forumService.FormatPostText(forumPost), true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(forumPost, tokens);
        }

        /// <summary>
        /// Add forum tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forum">Forum</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddForumTokensAsync(IList<Token> tokens, Forum forum)
        {
            //attributes
            //we cannot inject IForumService into constructor because it'll cause circular references.
            //that's why we resolve it here this way
            var forumService = EngineContext.Current.Resolve<IForumService>();

            var forumUrl = await RouteUrlAsync(routeName: "ForumSlug", routeValues: new { id = forum.Id, slug = await forumService.GetForumSeNameAsync(forum) });
            tokens.Add(new Token("Forums.ForumURL", forumUrl, true));
            tokens.Add(new Token("Forums.ForumName", forum.Name));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(forum, tokens);
        }

        /// <summary>
        /// Add private message tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="privateMessage">Private message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddPrivateMessageTokensAsync(IList<Token> tokens, PrivateMessage privateMessage)
        {
            //attributes
            //we cannot inject IForumService into constructor because it'll cause circular references.
            //that's why we resolve it here this way
            var forumService = EngineContext.Current.Resolve<IForumService>();

            tokens.Add(new Token("PrivateMessage.Subject", privateMessage.Subject));
            tokens.Add(new Token("PrivateMessage.Text", forumService.FormatPrivateMessageText(privateMessage), true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(privateMessage, tokens);
        }

        /// <summary>
        /// Add tokens of BackInStock subscription
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">BackInStock subscription</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddBackInStockTokensAsync(IList<Token> tokens, BackInStockSubscription subscription)
        {
            var product = await _productService.GetProductByIdAsync(subscription.ProductId);

            tokens.Add(new Token("BackInStockSubscription.ProductName", product.Name));
            var productUrl = await RouteUrlAsync(subscription.StoreId, "Product", new { SeName = await _urlRecordService.GetSeNameAsync(product) });
            tokens.Add(new Token("BackInStockSubscription.ProductUrl", productUrl, true));

            //event notification
            await _eventPublisher.EntityTokensAddedAsync(subscription, tokens);
        }

        /// <summary>
        /// Get collection of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of allowed (supported) message tokens for campaigns
        /// </returns>
        public virtual async Task<IEnumerable<string>> GetListOfCampaignAllowedTokensAsync()
        {
            var additionalTokens = new CampaignAdditionalTokensAddedEvent();
            await _eventPublisher.PublishAsync(additionalTokens);

            var allowedTokens = (await GetListOfAllowedTokensAsync(new[] { TokenGroupNames.StoreTokens, TokenGroupNames.SubscriptionTokens })).ToList();
            allowedTokens.AddRange(additionalTokens.AdditionalTokens);

            return allowedTokens.Distinct();
        }

        /// <summary>
        /// Get collection of allowed (supported) message tokens
        /// </summary>
        /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of allowed message tokens
        /// </returns>
        public virtual async Task<IEnumerable<string>> GetListOfAllowedTokensAsync(IEnumerable<string> tokenGroups = null)
        {
            var additionalTokens = new AdditionalTokensAddedEvent
            {
                TokenGroups = tokenGroups
            };
            await _eventPublisher.PublishAsync(additionalTokens);

            var allowedTokens = AllowedTokens.Where(x => tokenGroups == null || tokenGroups.Contains(x.Key))
                .SelectMany(x => x.Value).ToList();

            allowedTokens.AddRange(additionalTokens.AdditionalTokens);

            return allowedTokens.Distinct();
        }

        /// <summary>
        /// Get token groups of message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Collection of token group names</returns>
        public virtual IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate)
        {
            //groups depend on which tokens are added at the appropriate methods in IWorkflowMessageService
            return messageTemplate.Name switch
            {
                MessageTemplateSystemNames.CustomerRegisteredStoreOwnerNotification or
                MessageTemplateSystemNames.CustomerWelcomeMessage or
                MessageTemplateSystemNames.CustomerEmailValidationMessage or
                MessageTemplateSystemNames.CustomerEmailRevalidationMessage or
                MessageTemplateSystemNames.CustomerPasswordRecoveryMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens },

                MessageTemplateSystemNames.OrderPlacedVendorNotification or
                MessageTemplateSystemNames.OrderPlacedStoreOwnerNotification or
                MessageTemplateSystemNames.OrderPlacedAffiliateNotification or
                MessageTemplateSystemNames.OrderPaidStoreOwnerNotification or
                MessageTemplateSystemNames.OrderPaidCustomerNotification or
                MessageTemplateSystemNames.OrderPaidVendorNotification or
                MessageTemplateSystemNames.OrderPaidAffiliateNotification or
                MessageTemplateSystemNames.OrderPlacedCustomerNotification or
                MessageTemplateSystemNames.OrderProcessingCustomerNotification or
                MessageTemplateSystemNames.OrderCompletedCustomerNotification or
                MessageTemplateSystemNames.OrderCancelledCustomerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderTokens, TokenGroupNames.CustomerTokens },

                MessageTemplateSystemNames.ShipmentSentCustomerNotification or
                MessageTemplateSystemNames.ShipmentReadyForPickupCustomerNotification or
                MessageTemplateSystemNames.ShipmentDeliveredCustomerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ShipmentTokens, TokenGroupNames.OrderTokens, TokenGroupNames.CustomerTokens },

                MessageTemplateSystemNames.OrderRefundedStoreOwnerNotification or
                MessageTemplateSystemNames.OrderRefundedCustomerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderTokens, TokenGroupNames.RefundedOrderTokens, TokenGroupNames.CustomerTokens },

                MessageTemplateSystemNames.NewOrderNoteAddedCustomerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderNoteTokens, TokenGroupNames.OrderTokens, TokenGroupNames.CustomerTokens },

                MessageTemplateSystemNames.RecurringPaymentCancelledStoreOwnerNotification or
                MessageTemplateSystemNames.RecurringPaymentCancelledCustomerNotification or
                MessageTemplateSystemNames.RecurringPaymentFailedCustomerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.RecurringPaymentTokens },

                MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage or
                MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.SubscriptionTokens },

                MessageTemplateSystemNames.EmailAFriendMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.ProductTokens, TokenGroupNames.EmailAFriendTokens },
                MessageTemplateSystemNames.WishlistToFriendMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.WishlistToFriendTokens },

                MessageTemplateSystemNames.NewReturnRequestStoreOwnerNotification or
                MessageTemplateSystemNames.NewReturnRequestCustomerNotification or
                MessageTemplateSystemNames.ReturnRequestStatusChangedCustomerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.ReturnRequestTokens },

                MessageTemplateSystemNames.NewForumTopicMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ForumTopicTokens, TokenGroupNames.ForumTokens, TokenGroupNames.CustomerTokens },
                MessageTemplateSystemNames.NewForumPostMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ForumPostTokens, TokenGroupNames.ForumTopicTokens, TokenGroupNames.ForumTokens, TokenGroupNames.CustomerTokens },
                MessageTemplateSystemNames.PrivateMessageNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.PrivateMessageTokens, TokenGroupNames.CustomerTokens },
                MessageTemplateSystemNames.NewVendorAccountApplyStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.VendorTokens },
                MessageTemplateSystemNames.VendorInformationChangeStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.VendorTokens },
                MessageTemplateSystemNames.GiftCardNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.GiftCardTokens },

                MessageTemplateSystemNames.ProductReviewStoreOwnerNotification or
                MessageTemplateSystemNames.ProductReviewReplyCustomerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ProductReviewTokens, TokenGroupNames.CustomerTokens },

                MessageTemplateSystemNames.QuantityBelowStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ProductTokens },
                MessageTemplateSystemNames.QuantityBelowAttributeCombinationStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ProductTokens, TokenGroupNames.AttributeCombinationTokens },
                MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.VatValidation },
                MessageTemplateSystemNames.BlogCommentStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.BlogCommentTokens, TokenGroupNames.CustomerTokens },
                MessageTemplateSystemNames.NewsCommentStoreOwnerNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.NewsCommentTokens, TokenGroupNames.CustomerTokens },
                MessageTemplateSystemNames.BackInStockNotification => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.ProductBackInStockTokens },
                MessageTemplateSystemNames.ContactUsMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ContactUs },
                MessageTemplateSystemNames.ContactVendorMessage => new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ContactVendor },
                _ => Array.Empty<string>(),
            };
        }

        #endregion
    }
}