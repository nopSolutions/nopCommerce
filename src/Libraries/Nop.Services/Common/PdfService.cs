using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
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
using System.Globalization;

namespace Nop.Services.Common
{
    /// <summary>
    /// PDF service
    /// </summary>
    public partial class PdfService : IPdfService
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IMeasureService _measureService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IWebHelper _webHelper;

        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly MeasureSettings _measureSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly TaxSettings _taxSettings;
        private readonly StoreInformationSettings _storeInformationSettings;

        #endregion

        #region Ctor

        public PdfService(ILocalizationService localizationService, IOrderService orderService,
            IPaymentService paymentService,
            IDateTimeHelper dateTimeHelper, IPriceFormatter priceFormatter,
            ICurrencyService currencyService, IMeasureService measureService,
            IPictureService pictureService, IProductService productService, 
            IWebHelper webHelper, 
            CatalogSettings catalogSettings, CurrencySettings currencySettings,
            MeasureSettings measureSettings, PdfSettings pdfSettings, TaxSettings taxSettings,
            StoreInformationSettings storeInformationSettings)
        {
            this._localizationService = localizationService;
            this._orderService = orderService;
            this._paymentService = paymentService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._measureService = measureService;
            this._pictureService = pictureService;
            this._productService = productService;
            this._webHelper = webHelper;
            this._currencySettings = currencySettings;
            this._catalogSettings = catalogSettings;
            this._measureSettings = measureSettings;
            this._pdfSettings = pdfSettings;
            this._taxSettings = taxSettings;
            this._storeInformationSettings = storeInformationSettings;
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
        /// <param name="orders">Orders</param>
        /// <param name="lang">Language</param>
        /// <param name="filePath">File path</param>
        public virtual void PrintOrdersToPdf(IList<Order> orders, Language lang, string filePath)
        {
            if (orders == null)
                throw new ArgumentNullException("orders");

            if (lang == null)
                throw new ArgumentNullException("lang");

            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            var pageSize = PageSize.A4;

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.LETTER;
            }


            var doc = new Document(pageSize);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
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
                    var logoFilePath = _pictureService.GetPictureLocalPath(logoPicture, 0, false);
                    var cellLogo = new PdfPCell(Image.GetInstance(logoFilePath));
                    cellLogo.Border = Rectangle.NO_BORDER;
                    headerTable.AddCell(cellLogo);
                }
                //store info
                var cell = new PdfPCell();
                cell.Border = Rectangle.NO_BORDER;
                cell.AddElement(new Paragraph(String.Format(_localizationService.GetResource("PDFInvoice.Order#", lang.Id), order.Id), titleFont));
                var anchor = new Anchor(_storeInformationSettings.StoreUrl.Trim(new char[] { '/' }), font);
                anchor.Reference = _storeInformationSettings.StoreUrl;
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

                if (!String.IsNullOrEmpty(order.BillingAddress.Company))
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Company", lang.Id), order.BillingAddress.Company), font));

                cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Name", lang.Id), order.BillingAddress.FirstName + " " + order.BillingAddress.LastName), font));
                cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Phone", lang.Id), order.BillingAddress.PhoneNumber), font));
                if (!String.IsNullOrEmpty(order.BillingAddress.FaxNumber))
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Fax", lang.Id), order.BillingAddress.FaxNumber), font));
                cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address", lang.Id), order.BillingAddress.Address1), font));
                if (!String.IsNullOrEmpty(order.BillingAddress.Address2))
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address2", lang.Id), order.BillingAddress.Address2), font));

                cell.AddElement(new Paragraph("   " + String.Format("{0}, {1} {2}", order.BillingAddress.City, order.BillingAddress.StateProvince != null ? order.BillingAddress.StateProvince.GetLocalized(x => x.Name, lang.Id) : "", order.BillingAddress.ZipPostalCode), font));
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
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Phone", lang.Id), order.ShippingAddress.PhoneNumber), font));
                    if (!String.IsNullOrEmpty(order.ShippingAddress.FaxNumber))
                        cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Fax", lang.Id), order.ShippingAddress.FaxNumber), font));
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address", lang.Id), order.ShippingAddress.Address1), font));
                    if (!String.IsNullOrEmpty(order.ShippingAddress.Address2))
                        cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address2", lang.Id), order.ShippingAddress.Address2), font));
                    cell.AddElement(new Paragraph("   " + String.Format("{0}, {1} {2}", order.ShippingAddress.City, order.ShippingAddress.StateProvince != null ? order.ShippingAddress.StateProvince.GetLocalized(x => x.Name, lang.Id) : "", order.ShippingAddress.ZipPostalCode), font));
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


                var orderProductVariants = _orderService.GetAllOrderProductVariants(order.Id, null, null, null, null, null, null);

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

                for (int i = 0; i < orderProductVariants.Count; i++)
                {
                    var orderProductVariant = orderProductVariants[i];
                    var pv = orderProductVariant.ProductVariant;

                    //product name
                    string name = "";
                    if (!String.IsNullOrEmpty(pv.GetLocalized(x => x.Name, lang.Id)))
                        name = string.Format("{0} ({1})", pv.Product.GetLocalized(x => x.Name, lang.Id), pv.GetLocalized(x => x.Name, lang.Id));
                    else
                        name = pv.Product.GetLocalized(x => x.Name, lang.Id);
                    cell = new PdfPCell();
                    cell.AddElement(new Paragraph(name, font));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    var attributesParagraph = new Paragraph(HtmlHelper.ConvertHtmlToPlainText(orderProductVariant.AttributeDescription, true, true), attributesFont);
                    cell.AddElement(attributesParagraph);
                    productsTable.AddCell(cell);

                    //SKU
                    if (_catalogSettings.ShowProductSku)
                    {
                        cell = new PdfPCell(new Phrase(pv.Sku ?? String.Empty, font));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        productsTable.AddCell(cell);
                    }

                    //price
                    string unitPrice = string.Empty;
                    switch (order.CustomerTaxDisplayType)
                    {
                        case TaxDisplayType.ExcludingTax:
                            {
                                var opvUnitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderProductVariant.UnitPriceExclTax, order.CurrencyRate);
                                unitPrice = _priceFormatter.FormatPrice(opvUnitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);
                            }
                            break;
                        case TaxDisplayType.IncludingTax:
                            {
                                var opvUnitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderProductVariant.UnitPriceInclTax, order.CurrencyRate);
                                unitPrice = _priceFormatter.FormatPrice(opvUnitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
                            }
                            break;
                    }
                    cell = new PdfPCell(new Phrase(unitPrice, font));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    productsTable.AddCell(cell);

                    //qty
                    cell = new PdfPCell(new Phrase(orderProductVariant.Quantity.ToString(), font));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    productsTable.AddCell(cell);

                    //total
                    string subTotal = string.Empty;
                    switch (order.CustomerTaxDisplayType)
                    {
                        case TaxDisplayType.ExcludingTax:
                            {
                                var opvPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderProductVariant.PriceExclTax, order.CurrencyRate);
                                subTotal = _priceFormatter.FormatPrice(opvPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);
                            }
                            break;
                        case TaxDisplayType.IncludingTax:
                            {
                                var opvPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderProductVariant.PriceInclTax, order.CurrencyRate);
                                subTotal = _priceFormatter.FormatPrice(opvPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
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
        /// <param name="shipments">Shipments</param>
        /// <param name="lang">Language</param>
        /// <param name="filePath">File path</param>
        public virtual void PrintPackagingSlipsToPdf(IList<Shipment> shipments, Language lang, string filePath)
        {
            if (shipments == null)
                throw new ArgumentNullException("shipments");

            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            var pageSize = PageSize.A4;

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.LETTER;
            }

            var doc = new Document(pageSize);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
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
                if (order.ShippingAddress != null)
                {
                    doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Shipment", lang.Id), shipment.Id), titleFont));
                    doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Order", lang.Id), order.Id), titleFont));

                    if (!String.IsNullOrEmpty(order.ShippingAddress.Company))
                        doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Company", lang.Id), order.ShippingAddress.Company), font));

                    doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Name", lang.Id), order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName), font));
                    doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Phone", lang.Id), order.ShippingAddress.PhoneNumber), font));
                    doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Address", lang.Id), order.ShippingAddress.Address1), font));

                    if (!String.IsNullOrEmpty(order.ShippingAddress.Address2))
                        doc.Add(new Paragraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Address2", lang.Id), order.ShippingAddress.Address2), font));

                    doc.Add(new Paragraph(String.Format("{0}, {1} {2}", order.ShippingAddress.City, order.ShippingAddress.StateProvince != null ? order.ShippingAddress.StateProvince.GetLocalized(x => x.Name, lang.Id) : "", order.ShippingAddress.ZipPostalCode), font));
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

                    foreach (var sopv in shipment.ShipmentOrderProductVariants)
                    {
                        //product name
                        var opv = _orderService.GetOrderProductVariantById(sopv.OrderProductVariantId);
                        if (opv == null)
                            continue;
                        
                        var pv = opv.ProductVariant;
                        string name = "";
                        if (!String.IsNullOrEmpty(pv.GetLocalized(x => x.Name, lang.Id)))
                            name = string.Format("{0} ({1})", pv.Product.GetLocalized(x => x.Name, lang.Id), pv.GetLocalized(x => x.Name, lang.Id));
                        else
                            name = pv.Product.GetLocalized(x => x.Name, lang.Id);
                        cell = new PdfPCell();
                        cell.AddElement(new Paragraph(name, font));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        var attributesParagraph = new Paragraph(HtmlHelper.ConvertHtmlToPlainText(opv.AttributeDescription, true, true), attributesFont);
                        cell.AddElement(attributesParagraph);
                        productsTable.AddCell(cell);

                        //SKU
                        cell = new PdfPCell(new Phrase(pv.Sku ?? String.Empty, font));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        productsTable.AddCell(cell);

                        //qty
                        cell = new PdfPCell(new Phrase(sopv.Quantity.ToString(), font));
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
        /// <param name="products">Products</param>
        /// <param name="lang">Language</param>
        /// <param name="filePath">File path</param>
        public virtual void PrintProductsToPdf(IList<Product> products, Language lang, string filePath)
        {
            if (products == null)
                throw new ArgumentNullException("products");

            if (lang == null)
                throw new ArgumentNullException("lang");

            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");


            var pageSize = PageSize.A4;

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.LETTER;
            }

            var doc = new Document(pageSize);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
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
                                var pictureLocalPath = _pictureService.GetPictureLocalPath(pic, 200, false);
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

                int pvNum = 1;

                foreach (var productVariant in _productService.GetProductVariantsByProductId(product.Id, true))
                {
                    string pvName = String.IsNullOrEmpty(productVariant.GetLocalized(x => x.Name, lang.Id)) ? _localizationService.GetResource("PDFProductCatalog.UnnamedProductVariant", lang.Id) : productVariant.GetLocalized(x => x.Name, lang.Id);

                    doc.Add(new Paragraph(String.Format("{0}.{1}. {2}", productNumber, pvNum, pvName), font));
                    doc.Add(new Paragraph(" "));

                    string productVariantDescription = productVariant.GetLocalized(x => x.Description, lang.Id);
                    if (!String.IsNullOrEmpty(productVariantDescription))
                    {
                        doc.Add(new Paragraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(productVariantDescription)), font));
                        doc.Add(new Paragraph(" "));
                    }

                    var pic = _pictureService.GetPictureById(productVariant.PictureId);
                    if (pic != null)
                    {
                        var picBinary = _pictureService.LoadPictureBinary(pic);
                        if (picBinary != null && picBinary.Length > 0)
                        {
                            var pictureLocalPath = _pictureService.GetPictureLocalPath(pic, 200, false);
                            doc.Add(Image.GetInstance(pictureLocalPath));
                        }
                    }

                    doc.Add(new Paragraph(String.Format("{0}: {1} {2}", _localizationService.GetResource("PDFProductCatalog.Price", lang.Id), productVariant.Price.ToString("0.00"), _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode), font));
                    doc.Add(new Paragraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFProductCatalog.SKU", lang.Id), productVariant.Sku), font));

                    if (productVariant.IsShipEnabled && productVariant.Weight > Decimal.Zero)
                        doc.Add(new Paragraph(String.Format("{0}: {1} {2}", _localizationService.GetResource("PDFProductCatalog.Weight", lang.Id), productVariant.Weight.ToString("0.00"), _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name), font));

                    if (productVariant.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                        doc.Add(new Paragraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFProductCatalog.StockQuantity", lang.Id), productVariant.StockQuantity), font));

                    doc.Add(new Paragraph(" "));

                    pvNum++;
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