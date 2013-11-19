using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Html;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;

namespace Nop.Services.Common
{
    /// <summary>
    /// PDF service
    /// </summary>
    public partial class PdfService : IPdfService
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IMeasureService _measureService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;

        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly MeasureSettings _measureSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly TaxSettings _taxSettings;
        private readonly AddressSettings _addressSettings;

        #endregion

        #region Ctor

        public PdfService(ILocalizationService localizationService, 
            ILanguageService languageService,
            IWorkContext workContext,
            IOrderService orderService,
            IPaymentService paymentService,
            IDateTimeHelper dateTimeHelper, IPriceFormatter priceFormatter,
            ICurrencyService currencyService, IMeasureService measureService,
            IPictureService pictureService, IProductService productService, 
            IProductAttributeParser productAttributeParser, IStoreService storeService,
            IStoreContext storeContext, IWebHelper webHelper, 
            CatalogSettings catalogSettings, CurrencySettings currencySettings,
            MeasureSettings measureSettings, PdfSettings pdfSettings, TaxSettings taxSettings,
            AddressSettings addressSettings)
        {
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._workContext = workContext;
            this._orderService = orderService;
            this._paymentService = paymentService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._measureService = measureService;
            this._pictureService = pictureService;
            this._productService = productService;
            this._productAttributeParser = productAttributeParser;
            this._storeService = storeService;
            this._storeContext = storeContext;
            this._webHelper = webHelper;
            this._currencySettings = currencySettings;
            this._catalogSettings = catalogSettings;
            this._measureSettings = measureSettings;
            this._pdfSettings = pdfSettings;
            this._taxSettings = taxSettings;
            this._addressSettings = addressSettings;
        }

        #endregion

        #region Utilities

        protected virtual Font GetFont()
        {
            //nopCommerce supports unicode characters
            //nopCommerce uses Free Serif font by default (~/App_Data/Pdf/FreeSerif.ttf file)
            //It was downloaded from http://savannah.gnu.org/projects/freefont
            string fontPath = Path.Combine(_webHelper.MapPath("~/App_Data/Pdf/"), _pdfSettings.FontFileName);
            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var font = new Font(baseFont, 10, Font.NORMAL);
            return font;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Print an order to PDF
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        /// <returns>A path of generates file</returns>
        public virtual string PrintOrderToPdf(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            string fileName = string.Format("order_{0}_{1}.pdf", order.OrderGuid, CommonHelper.GenerateRandomDigitCode(4));
            string filePath = Path.Combine(_webHelper.MapPath("~/content/files/ExportImport"), fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var orders = new List<Order>();
                orders.Add(order);
                PrintOrdersToPdf(fileStream, orders, languageId);
            }
            return filePath;
        }

        /// <summary>
        /// Print orders to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="orders">Orders</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        public virtual void PrintOrdersToPdf(Stream stream, IList<Order> orders, int languageId = 0)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (orders == null)
                throw new ArgumentNullException("orders");

            var pageSize = PageSize.A4;

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.LETTER;
            }


            var doc = new Document(pageSize);
            var pdfWriter = PdfWriter.GetInstance(doc, stream);
            doc.Open();

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.BLACK;
            var font = GetFont();
            var attributesFont = GetFont();
            attributesFont.SetStyle(Font.ITALIC);

            int ordCount = orders.Count;
            int ordNum = 0;

            foreach (var order in orders)
            {
                var lang = _languageService.GetLanguageById(languageId == 0 ? order.CustomerLanguageId : languageId);
                if (lang == null || !lang.Published)
                    lang = _workContext.WorkingLanguage;

                #region Header

                //logo
                var logoPicture = _pictureService.GetPictureById(_pdfSettings.LogoPictureId);
                var logoExists = logoPicture != null;

                //header
                var headerTable = new PdfPTable(logoExists ? 2 : 1);
                headerTable.WidthPercentage = 100f;
                if (logoExists)
                    headerTable.SetWidths(new[] { 50, 50 });

                //logo
                if (logoExists)
                {
                    var logoFilePath = _pictureService.GetThumbLocalPath(logoPicture, 0, false);
                    var cellLogo = new PdfPCell(Image.GetInstance(logoFilePath));
                    cellLogo.Border = Rectangle.NO_BORDER;
                    headerTable.AddCell(cellLogo);
                }
                //store info
                var cell = new PdfPCell();
                cell.Border = Rectangle.NO_BORDER;
                cell.AddElement(new Paragraph(String.Format(_localizationService.GetResource("PDFInvoice.Order#", lang.Id), order.Id), titleFont));
                var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
                var anchor = new Anchor(store.Url.Trim(new char[] { '/' }), font);
                anchor.Reference = store.Url;
                cell.AddElement(new Paragraph(anchor));
                cell.AddElement(new Paragraph(String.Format(_localizationService.GetResource("PDFInvoice.OrderDate", lang.Id), _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToString("D", new CultureInfo(lang.LanguageCulture))), font));
                headerTable.AddCell(cell);
                doc.Add(headerTable);

                #endregion

                #region Addresses

                var addressTable = new PdfPTable(2);
                addressTable.WidthPercentage = 100f;
                addressTable.SetWidths(new[] { 50, 50 });

                //billing info
                cell = new PdfPCell();
                cell.Border = Rectangle.NO_BORDER;
                cell.AddElement(new Paragraph(_localizationService.GetResource("PDFInvoice.BillingInformation", lang.Id), titleFont));

                if (_addressSettings.CompanyEnabled && !String.IsNullOrEmpty(order.BillingAddress.Company))
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Company", lang.Id), order.BillingAddress.Company), font));

                cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Name", lang.Id), order.BillingAddress.FirstName + " " + order.BillingAddress.LastName), font));
                if (_addressSettings.PhoneEnabled)
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Phone", lang.Id), order.BillingAddress.PhoneNumber), font));
                if (_addressSettings.FaxEnabled && !String.IsNullOrEmpty(order.BillingAddress.FaxNumber))
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Fax", lang.Id), order.BillingAddress.FaxNumber), font));
                if (_addressSettings.StreetAddressEnabled)
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address", lang.Id), order.BillingAddress.Address1), font));
                if (_addressSettings.StreetAddress2Enabled && !String.IsNullOrEmpty(order.BillingAddress.Address2))
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address2", lang.Id), order.BillingAddress.Address2), font));
                if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled || _addressSettings.ZipPostalCodeEnabled)
                    cell.AddElement(new Paragraph("   " + String.Format("{0}, {1} {2}", order.BillingAddress.City, order.BillingAddress.StateProvince != null ? order.BillingAddress.StateProvince.GetLocalized(x => x.Name, lang.Id) : "", order.BillingAddress.ZipPostalCode), font));
                if (_addressSettings.CountryEnabled && order.BillingAddress.Country != null) 
                    cell.AddElement(new Paragraph("   " + String.Format("{0}", order.BillingAddress.Country != null ? order.BillingAddress.Country.GetLocalized(x => x.Name, lang.Id) : ""), font));

                //VAT number
                if (!String.IsNullOrEmpty(order.VatNumber))
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.VATNumber", lang.Id), order.VatNumber), font));

                //payment method
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
                string paymentMethodStr = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, lang.Id) : order.PaymentMethodSystemName;
                if (!String.IsNullOrEmpty(paymentMethodStr))
                {
                    cell.AddElement(new Paragraph(" "));
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.PaymentMethod", lang.Id), paymentMethodStr), font));
                    cell.AddElement(new Paragraph());
                }

                //purchase order number (we have to find a better to inject this information because it's related to a certain plugin)
                if (paymentMethod != null && paymentMethod.PluginDescriptor.SystemName.Equals("Payments.PurchaseOrder", StringComparison.InvariantCultureIgnoreCase))
                {
                    cell.AddElement(new Paragraph(" "));
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.PurchaseOrderNumber", lang.Id), order.PurchaseOrderNumber), font));
                    cell.AddElement(new Paragraph());
                }

                addressTable.AddCell(cell);

                //shipping info
                if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
                {
                    if (order.ShippingAddress == null)
                        throw new NopException(string.Format("Shipping is required, but address is not available. Order ID = {0}", order.Id));
                    cell = new PdfPCell();
                    cell.Border = Rectangle.NO_BORDER;

                    cell.AddElement(new Paragraph(_localizationService.GetResource("PDFInvoice.ShippingInformation", lang.Id), titleFont));
                    if (!String.IsNullOrEmpty(order.ShippingAddress.Company))
                        cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Company", lang.Id), order.ShippingAddress.Company), font));
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Name", lang.Id), order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName), font));
                    if (_addressSettings.PhoneEnabled) 
                        cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Phone", lang.Id), order.ShippingAddress.PhoneNumber), font));
                    if (_addressSettings.FaxEnabled && !String.IsNullOrEmpty(order.ShippingAddress.FaxNumber))
                        cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Fax", lang.Id), order.ShippingAddress.FaxNumber), font));
                    if (_addressSettings.StreetAddressEnabled) 
                        cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address", lang.Id), order.ShippingAddress.Address1), font));
                    if (_addressSettings.StreetAddress2Enabled && !String.IsNullOrEmpty(order.ShippingAddress.Address2))
                        cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address2", lang.Id), order.ShippingAddress.Address2), font));
                    if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled || _addressSettings.ZipPostalCodeEnabled)
                        cell.AddElement(new Paragraph("   " + String.Format("{0}, {1} {2}", order.ShippingAddress.City, order.ShippingAddress.StateProvince != null ? order.ShippingAddress.StateProvince.GetLocalized(x => x.Name, lang.Id) : "", order.ShippingAddress.ZipPostalCode), font));
                    if (_addressSettings.CountryEnabled && order.ShippingAddress.Country != null)
                        cell.AddElement(new Paragraph("   " + String.Format("{0}", order.ShippingAddress.Country != null ? order.ShippingAddress.Country.GetLocalized(x => x.Name, lang.Id) : ""), font));
                    cell.AddElement(new Paragraph(" "));
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.ShippingMethod", lang.Id), order.ShippingMethod), font));
                    cell.AddElement(new Paragraph());

                    addressTable.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Phrase(" "));
                    cell.Border = Rectangle.NO_BORDER;
                    addressTable.AddCell(cell);
                }

                doc.Add(addressTable);
                doc.Add(new Paragraph(" "));

                #endregion

                #region Products
                //products
                doc.Add(new Paragraph(_localizationService.GetResource("PDFInvoice.Product(s)", lang.Id), titleFont));
                doc.Add(new Paragraph(" "));


                var orderItems = _orderService.GetAllOrderItems(order.Id, null, null, null, null, null, null);

                var productsTable = new PdfPTable(_catalogSettings.ShowProductSku ? 5 : 4);
                productsTable.WidthPercentage = 100f;
                productsTable.SetWidths(_catalogSettings.ShowProductSku ? new[] { 40, 15, 15, 15, 15 } : new[] { 40, 20, 20, 20 });

                //product name
                cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.ProductName", lang.Id), font));
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cell);

                //SKU
                if (_catalogSettings.ShowProductSku)
                {
                    cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.SKU", lang.Id), font));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    productsTable.AddCell(cell);
                }

                //price
                cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.ProductPrice", lang.Id), font));
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cell);

                //qty
                cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.ProductQuantity", lang.Id), font));
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cell);

                //total
                cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.ProductTotal", lang.Id), font));
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cell);

                for (int i = 0; i < orderItems.Count; i++)
                {
                    var orderItem = orderItems[i];
                    var p = orderItem.Product;

                    //product name
                    string name = p.GetLocalized(x => x.Name, lang.Id);
                    cell = new PdfPCell();
                    cell.AddElement(new Paragraph(name, font));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    var attributesParagraph = new Paragraph(HtmlHelper.ConvertHtmlToPlainText(orderItem.AttributeDescription, true, true), attributesFont);
                    cell.AddElement(attributesParagraph);
                    productsTable.AddCell(cell);

                    //SKU
                    if (_catalogSettings.ShowProductSku)
                    {
                        var sku = p.FormatSku(orderItem.AttributesXml, _productAttributeParser);
                        cell = new PdfPCell(new Phrase(sku ?? String.Empty, font));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        productsTable.AddCell(cell);
                    }

                    //price
                    string unitPrice = string.Empty;
                    switch (order.CustomerTaxDisplayType)
                    {
                        case TaxDisplayType.ExcludingTax:
                            {
                                var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                                unitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);
                            }
                            break;
                        case TaxDisplayType.IncludingTax:
                            {
                                var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                                unitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
                            }
                            break;
                    }
                    cell = new PdfPCell(new Phrase(unitPrice, font));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    productsTable.AddCell(cell);

                    //qty
                    cell = new PdfPCell(new Phrase(orderItem.Quantity.ToString(), font));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    productsTable.AddCell(cell);

                    //total
                    string subTotal = string.Empty;
                    switch (order.CustomerTaxDisplayType)
                    {
                        case TaxDisplayType.ExcludingTax:
                            {
                                var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                                subTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);
                            }
                            break;
                        case TaxDisplayType.IncludingTax:
                            {
                                var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                                subTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
                            }
                            break;
                    }
                    cell = new PdfPCell(new Phrase(subTotal, font));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    productsTable.AddCell(cell);
                }
                doc.Add(productsTable);

                #endregion

                #region Checkout attributes

                if (!String.IsNullOrEmpty(order.CheckoutAttributeDescription))
                {
                    doc.Add(new Paragraph(" "));
                    string attributes = HtmlHelper.ConvertHtmlToPlainText(order.CheckoutAttributeDescription, true, true);
                    var pCheckoutAttributes = new Paragraph(attributes, font);
                    pCheckoutAttributes.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(pCheckoutAttributes);
                    doc.Add(new Paragraph(" "));
                }

                #endregion

                #region Totals

                //subtotal
                doc.Add(new Paragraph(" "));
                switch (order.CustomerTaxDisplayType)
                {
                    case TaxDisplayType.ExcludingTax:
                        {
                            var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                            string orderSubtotalExclTaxStr = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);

                            var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Sub-Total", lang.Id), orderSubtotalExclTaxStr), font);
                            p.Alignment = Element.ALIGN_RIGHT;
                            doc.Add(p);
                        }
                        break;
                    case TaxDisplayType.IncludingTax:
                        {
                            var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                            string orderSubtotalInclTaxStr = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);

                            var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Sub-Total", lang.Id), orderSubtotalInclTaxStr), font);
                            p.Alignment = Element.ALIGN_RIGHT;
                            doc.Add(p);
                        }
                        break;
                }
                //discount (applied to order subtotal)
                if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
                {
                    switch (order.CustomerTaxDisplayType)
                    {
                        case TaxDisplayType.ExcludingTax:
                            {
                                var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                                string orderSubTotalDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);

                                var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Discount", lang.Id), orderSubTotalDiscountInCustomerCurrencyStr), font);
                                p.Alignment = Element.ALIGN_RIGHT;
                                doc.Add(p);
                            }
                            break;
                        case TaxDisplayType.IncludingTax:
                            {
                                var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                                string orderSubTotalDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);

                                var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Discount", lang.Id), orderSubTotalDiscountInCustomerCurrencyStr), font);
                                p.Alignment = Element.ALIGN_RIGHT;
                                doc.Add(p);
                            }
                            break;
                    }
                }

                //shipping
                if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
                {
                    switch (order.CustomerTaxDisplayType)
                    {
                        case TaxDisplayType.ExcludingTax:
                            {
                                var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                                string orderShippingExclTaxStr = _priceFormatter.FormatShippingPrice(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);

                                var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Shipping", lang.Id), orderShippingExclTaxStr), font);
                                p.Alignment = Element.ALIGN_RIGHT;
                                doc.Add(p);
                            }
                            break;
                        case TaxDisplayType.IncludingTax:
                            {
                                var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                                string orderShippingInclTaxStr = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);

                                var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Shipping", lang.Id), orderShippingInclTaxStr), font);
                                p.Alignment = Element.ALIGN_RIGHT;
                                doc.Add(p);
                            }
                            break;
                    }
                }

                //payment fee
                if (order.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
                {
                    switch (order.CustomerTaxDisplayType)
                    {
                        case TaxDisplayType.ExcludingTax:
                            {
                                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                                string paymentMethodAdditionalFeeExclTaxStr = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);

                                var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.PaymentMethodAdditionalFee", lang.Id), paymentMethodAdditionalFeeExclTaxStr), font);
                                p.Alignment = Element.ALIGN_RIGHT;
                                doc.Add(p);
                            }
                            break;
                        case TaxDisplayType.IncludingTax:
                            {
                                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                                string paymentMethodAdditionalFeeInclTaxStr = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);

                                var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.PaymentMethodAdditionalFee", lang.Id), paymentMethodAdditionalFeeInclTaxStr), font);
                                p.Alignment = Element.ALIGN_RIGHT;
                                doc.Add(p);
                            }
                            break;
                    }
                }

                //tax
                string taxStr = string.Empty;
                var taxRates = new SortedDictionary<decimal, decimal>();
                bool displayTax = true;
                bool displayTaxRates = true;
                if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
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
                        taxRates = order.TaxRatesDictionary;

                        displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Count > 0;
                        displayTax = !displayTaxRates;

                        var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                        taxStr = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false, lang);
                    }
                }
                if (displayTax)
                {
                    var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Tax", lang.Id), taxStr), font);
                    p.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(p);
                }
                if (displayTaxRates)
                {
                    foreach (var item in taxRates)
                    {
                        string taxRate = String.Format(_localizationService.GetResource("PDFInvoice.TaxRate", lang.Id), _priceFormatter.FormatTaxRate(item.Key));
                        string taxValue = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(item.Value, order.CurrencyRate), true, order.CustomerCurrencyCode, false, lang);

                        var p = new Paragraph(String.Format("{0} {1}", taxRate, taxValue), font);
                        p.Alignment = Element.ALIGN_RIGHT;
                        doc.Add(p);
                    }
                }

                //discount (applied to order total)
                if (order.OrderDiscount > decimal.Zero)
                {
                    var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
                    string orderDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, lang);

                    var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Discount", lang.Id), orderDiscountInCustomerCurrencyStr), font);
                    p.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(p);
                }

                //gift cards
                foreach (var gcuh in order.GiftCardUsageHistory)
                {
                    string gcTitle = string.Format(_localizationService.GetResource("PDFInvoice.GiftCardInfo", lang.Id), gcuh.GiftCard.GiftCardCouponCode);
                    string gcAmountStr = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, lang);

                    var p = new Paragraph(String.Format("{0} {1}", gcTitle, gcAmountStr), font);
                    p.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(p);
                }

                //reward points
                if (order.RedeemedRewardPointsEntry != null)
                {
                    string rpTitle = string.Format(_localizationService.GetResource("PDFInvoice.RewardPoints", lang.Id), -order.RedeemedRewardPointsEntry.Points);
                    string rpAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(order.RedeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, lang);

                    var p = new Paragraph(String.Format("{0} {1}", rpTitle, rpAmount), font);
                    p.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(p);
                }

                //order total
                var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                string orderTotalStr = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, lang);


                var pTotal = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.OrderTotal", lang.Id), orderTotalStr), titleFont);
                pTotal.Alignment = Element.ALIGN_RIGHT;
                doc.Add(pTotal);

                #endregion

                #region Order notes

                if (_pdfSettings.RenderOrderNotes)
                {
                    var orderNotes = order.OrderNotes
                        .Where(on => on.DisplayToCustomer)
                        .OrderByDescending(on => on.CreatedOnUtc)
                        .ToList();
                    if (orderNotes.Count > 0)
                    {
                        doc.Add(new Paragraph(_localizationService.GetResource("PDFInvoice.OrderNotes", lang.Id), titleFont));

                        doc.Add(new Paragraph(" "));

                        var notesTable = new PdfPTable(2);
                        notesTable.WidthPercentage = 100f;
                        notesTable.SetWidths(new[] { 30, 70 });

                        //created on
                        cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.OrderNotes.CreatedOn", lang.Id), font));
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        notesTable.AddCell(cell);

                        //note
                        cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.OrderNotes.Note", lang.Id), font));
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        notesTable.AddCell(cell);

                        foreach (var orderNote in orderNotes)
                        {
                            cell = new PdfPCell();
                            cell.AddElement(new Paragraph(_dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc).ToString(), font));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            notesTable.AddCell(cell);

                            cell = new PdfPCell();
                            cell.AddElement(new Paragraph(HtmlHelper.ConvertHtmlToPlainText(orderNote.FormatOrderNoteText(), true, true), font));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            notesTable.AddCell(cell);
                        }
                        doc.Add(notesTable);
                    }
                }

                #endregion

                #region Footer

                if (!String.IsNullOrEmpty(_pdfSettings.InvoiceFooterTextColumn1) || !String.IsNullOrEmpty(_pdfSettings.InvoiceFooterTextColumn2))
                {
                    var column1Lines = String.IsNullOrEmpty(_pdfSettings.InvoiceFooterTextColumn1) ?
                        new List<string>() :
                        _pdfSettings.InvoiceFooterTextColumn1
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    var column2Lines = String.IsNullOrEmpty(_pdfSettings.InvoiceFooterTextColumn2) ?
                        new List<string>() :
                        _pdfSettings.InvoiceFooterTextColumn2
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    if (column1Lines.Count > 0 || column2Lines.Count > 0)
                    {
                        var totalLines = Math.Max(column1Lines.Count, column2Lines.Count);
                        const float margin = 43;

                        //if you have really a lot of lines in the footer, then replace 9 with 10 or 11
                        int footerHeight = totalLines * 9;
                        var directContent = pdfWriter.DirectContent;
                        directContent.MoveTo(pageSize.GetLeft(margin), pageSize.GetBottom(margin) + footerHeight);
                        directContent.LineTo(pageSize.GetRight(margin), pageSize.GetBottom(margin) + footerHeight);
                        directContent.Stroke();


                        var footerTable = new PdfPTable(2);
                        footerTable.WidthPercentage = 100f;
                            footerTable.SetTotalWidth(new float[] { 250, 250 });

                        //column 1
                        if (column1Lines.Count > 0)
                        {
                            var column1 = new PdfPCell();
                            column1.Border = Rectangle.NO_BORDER;
                            column1.HorizontalAlignment = Element.ALIGN_LEFT;
                            foreach (var footerLine in column1Lines)
                            {
                                column1.AddElement(new Phrase(footerLine, font));
                            }
                            footerTable.AddCell(column1);
                        }
                        else
                        {
                            var column = new PdfPCell(new Phrase(" "));
                            column.Border = Rectangle.NO_BORDER;
                            footerTable.AddCell(column);
                        }

                        //column 2
                        if (column2Lines.Count > 0)
                        {
                            var column2 = new PdfPCell();
                            column2.Border = Rectangle.NO_BORDER;
                            column2.HorizontalAlignment = Element.ALIGN_LEFT;
                            foreach (var footerLine in column2Lines)
                            {
                                column2.AddElement(new Phrase(footerLine, font));
                            }
                            footerTable.AddCell(column2);
                        }
                        else
                        {
                            var column = new PdfPCell(new Phrase(" "));
                            column.Border = Rectangle.NO_BORDER;
                            footerTable.AddCell(column);
                        }

                        footerTable.WriteSelectedRows(0, totalLines, pageSize.GetLeft(margin), pageSize.GetBottom(margin) + footerHeight, directContent);
                    }
                }

                #endregion

                ordNum++;
                if (ordNum < ordCount)
                {
                    doc.NewPage();
                }
            }
            doc.Close();
        }
        
        /// <summary>
        /// Print packaging slips to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="shipments">Shipments</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        public virtual void PrintPackagingSlipsToPdf(Stream stream, IList<Shipment> shipments, int languageId = 0)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (shipments == null)
                throw new ArgumentNullException("shipments");

            var lang = _languageService.GetLanguageById(languageId);
            if (lang == null)
                throw new ArgumentException(string.Format("Cannot load language. ID={0}", languageId));

            var pageSize = PageSize.A4;

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.LETTER;
            }

            var doc = new Document(pageSize);
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.BLACK;
            var font = GetFont();
            var attributesFont = GetFont();
            attributesFont.SetStyle(Font.ITALIC);
            
            int shipmentCount = shipments.Count;
            int shipmentNum = 0;

            foreach (var shipment in shipments)
            {
                var order = shipment.Order;

                if (languageId == 0)
                {
                    lang = _languageService.GetLanguageById(order.CustomerLanguageId);
                    if (lang == null || !lang.Published)
                        lang = _workContext.WorkingLanguage;
                }


                if (order.ShippingAddress != null)
                {
                    doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Shipment", lang.Id), shipment.Id), titleFont));
                    doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Order", lang.Id), order.Id), titleFont));

                    if (_addressSettings.CompanyEnabled && !String.IsNullOrEmpty(order.ShippingAddress.Company))
                        doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Company", lang.Id), order.ShippingAddress.Company), font));

                    doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Name", lang.Id), order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName), font));
                    if (_addressSettings.PhoneEnabled)
                        doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Phone", lang.Id), order.ShippingAddress.PhoneNumber), font));
                    if (_addressSettings.StreetAddressEnabled)
                        doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Address", lang.Id), order.ShippingAddress.Address1), font));

                    if (_addressSettings.StreetAddress2Enabled && !String.IsNullOrEmpty(order.ShippingAddress.Address2))
                        doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Address2", lang.Id), order.ShippingAddress.Address2), font));

                    if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled || _addressSettings.ZipPostalCodeEnabled)
                        doc.Add(new Paragraph(String.Format("{0}, {1} {2}", order.ShippingAddress.City, order.ShippingAddress.StateProvince != null ? order.ShippingAddress.StateProvince.GetLocalized(x => x.Name, lang.Id) : "", order.ShippingAddress.ZipPostalCode), font));

                    if (_addressSettings.CountryEnabled && order.ShippingAddress.Country != null)
                        doc.Add(new Paragraph(String.Format("{0}", order.ShippingAddress.Country != null ? order.ShippingAddress.Country.GetLocalized(x => x.Name, lang.Id) : ""), font));
               
                    doc.Add(new Paragraph(" "));

                    doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.ShippingMethod", lang.Id), order.ShippingMethod), font));
                    doc.Add(new Paragraph(" "));

                    var productsTable = new PdfPTable(3);
                    productsTable.WidthPercentage = 100f;
                    productsTable.SetWidths(new[] { 60, 20, 20 });

                    //product name
                    var cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFPackagingSlip.ProductName", lang.Id), font));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    productsTable.AddCell(cell);

                    //SKU
                    cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFPackagingSlip.SKU", lang.Id), font));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    productsTable.AddCell(cell);

                    //qty
                    cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFPackagingSlip.QTY", lang.Id), font));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    productsTable.AddCell(cell);

                    foreach (var si in shipment.ShipmentItems)
                    {
                        //product name
                        var orderItem = _orderService.GetOrderItemById(si.OrderItemId);
                        if (orderItem == null)
                            continue;

                        var p = orderItem.Product;
                        string name = p.GetLocalized(x => x.Name, lang.Id);
                        cell = new PdfPCell();
                        cell.AddElement(new Paragraph(name, font));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        var attributesParagraph = new Paragraph(HtmlHelper.ConvertHtmlToPlainText(orderItem.AttributeDescription, true, true), attributesFont);
                        cell.AddElement(attributesParagraph);
                        productsTable.AddCell(cell);

                        //SKU
                        var sku = p.FormatSku(orderItem.AttributesXml, _productAttributeParser);
                        cell = new PdfPCell(new Phrase(sku ?? String.Empty, font));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        productsTable.AddCell(cell);

                        //qty
                        cell = new PdfPCell(new Phrase(si.Quantity.ToString(), font));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        productsTable.AddCell(cell);
                    }
                    doc.Add(productsTable);
                }

                shipmentNum++;
                if (shipmentNum < shipmentCount)
                {
                    doc.NewPage();
                }
            }


            doc.Close();
        }

        /// <summary>
        /// Print product collection to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="products">Products</param>
        public virtual void PrintProductsToPdf(Stream stream, IList<Product> products)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (products == null)
                throw new ArgumentNullException("products");

            var lang = _workContext.WorkingLanguage;

            var pageSize = PageSize.A4;

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.LETTER;
            }

            var doc = new Document(pageSize);
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.BLACK;
            var font = GetFont();

            int productNumber = 1;
            int prodCount = products.Count;

            foreach (var product in products)
            {
                string productName = product.GetLocalized(x => x.Name, lang.Id);
                string productFullDescription = product.GetLocalized(x => x.FullDescription, lang.Id);

                doc.Add(new Paragraph(String.Format("{0}. {1}", productNumber, productName), titleFont));
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(productFullDescription)), font));
                doc.Add(new Paragraph(" "));

                if (product.ProductType == ProductType.SimpleProduct)
                {
                    //simple product
                    //render its properties such as price, weight, etc
                    doc.Add(new Paragraph(String.Format("{0}: {1} {2}", _localizationService.GetResource("PDFProductCatalog.Price", lang.Id), product.Price.ToString("0.00"), _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode), font));
                    doc.Add(new Paragraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFProductCatalog.SKU", lang.Id), product.Sku), font));

                    if (product.IsShipEnabled && product.Weight > Decimal.Zero)
                        doc.Add(new Paragraph(String.Format("{0}: {1} {2}", _localizationService.GetResource("PDFProductCatalog.Weight", lang.Id), product.Weight.ToString("0.00"), _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name), font));

                    if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                        doc.Add(new Paragraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFProductCatalog.StockQuantity", lang.Id), product.StockQuantity), font));

                    doc.Add(new Paragraph(" "));
                }
                var pictures = _pictureService.GetPicturesByProductId(product.Id);
                if (pictures.Count > 0)
                {
                    var table = new PdfPTable(2);
                    table.WidthPercentage = 100f;

                    for (int i = 0; i < pictures.Count; i++)
                    {
                        var pic = pictures[i];
                        if (pic != null)
                        {
                            var picBinary = _pictureService.LoadPictureBinary(pic);
                            if (picBinary != null && picBinary.Length > 0)
                            {
                                var pictureLocalPath = _pictureService.GetThumbLocalPath(pic, 200, false);
                                var cell = new PdfPCell(Image.GetInstance(pictureLocalPath));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.Border = Rectangle.NO_BORDER;
                                table.AddCell(cell);
                            }
                        }
                    }

                    if (pictures.Count % 2 > 0)
                    {
                        var cell = new PdfPCell(new Phrase(" "));
                        cell.Border = Rectangle.NO_BORDER;
                        table.AddCell(cell);
                    }

                    doc.Add(table);
                    doc.Add(new Paragraph(" "));
                }


                if (product.ProductType == ProductType.GroupedProduct)
                {
                    //grouped product. render its associated products
                    int pvNum = 1;
                    foreach (var associatedProduct in _productService.SearchProducts(parentGroupedProductId: product.Id,
                        showHidden: true))
                    {
                        doc.Add(new Paragraph(String.Format("{0}-{1}. {2}", productNumber, pvNum, associatedProduct.GetLocalized(x => x.Name, lang.Id)), font));
                        doc.Add(new Paragraph(" "));

                        //uncomment to render associated product description
                        //string apDescription = associatedProduct.GetLocalized(x => x.ShortDescription, lang.Id);
                        //if (!String.IsNullOrEmpty(apDescription))
                        //{
                        //    doc.Add(new Paragraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(apDescription)), font));
                        //    doc.Add(new Paragraph(" "));
                        //}

                        //uncomment to render associated product picture
                        //var apPicture = _pictureService.GetPicturesByProductId(associatedProduct.Id).FirstOrDefault();
                        //if (apPicture != null)
                        //{
                        //    var picBinary = _pictureService.LoadPictureBinary(apPicture);
                        //    if (picBinary != null && picBinary.Length > 0)
                        //    {
                        //        var pictureLocalPath = _pictureService.GetThumbLocalPath(apPicture, 200, false);
                        //        doc.Add(Image.GetInstance(pictureLocalPath));
                        //    }
                        //}

                        doc.Add(new Paragraph(String.Format("{0}: {1} {2}", _localizationService.GetResource("PDFProductCatalog.Price", lang.Id), associatedProduct.Price.ToString("0.00"), _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode), font));
                        doc.Add(new Paragraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFProductCatalog.SKU", lang.Id), associatedProduct.Sku), font));

                        if (associatedProduct.IsShipEnabled && associatedProduct.Weight > Decimal.Zero)
                            doc.Add(new Paragraph(String.Format("{0}: {1} {2}", _localizationService.GetResource("PDFProductCatalog.Weight", lang.Id), associatedProduct.Weight.ToString("0.00"), _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name), font));

                        if (associatedProduct.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                            doc.Add(new Paragraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFProductCatalog.StockQuantity", lang.Id), associatedProduct.StockQuantity), font));

                        doc.Add(new Paragraph(" "));

                        pvNum++;
                    }
                }


                productNumber++;

                if (productNumber <= prodCount)
                {
                    doc.NewPage();
                }
            }

            doc.Close();
        }

        #endregion
    }
}