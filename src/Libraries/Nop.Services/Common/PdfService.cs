using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
using Nop.Core;

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
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IMeasureService _measureService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;

        private readonly CurrencySettings _currencySettings;
        private readonly MeasureSettings _measureSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly TaxSettings _taxSettings;
        private readonly StoreInformationSettings _storeInformationSettings;

        #endregion

        #region Ctor

        public PdfService(ILocalizationService localizationService, IOrderService orderService,
            IDateTimeHelper dateTimeHelper, IPriceFormatter priceFormatter,
            ICurrencyService currencyService, IMeasureService measureService,
            IPictureService pictureService, IProductService productService, CurrencySettings currencySettings,
            MeasureSettings measureSettings, PdfSettings pdfSettings, TaxSettings taxSettings,
            StoreInformationSettings storeInformationSettings)
        {
            _localizationService = localizationService;
            _orderService = orderService;
            _dateTimeHelper = dateTimeHelper;
            _priceFormatter = priceFormatter;
            _currencyService = currencyService;
            _measureService = measureService;
            _pictureService = pictureService;
            _productService = productService;
            _currencySettings = currencySettings;
            _measureSettings = measureSettings;
            _pdfSettings = pdfSettings;
            _taxSettings = taxSettings;
            _storeInformationSettings = storeInformationSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Print an order to PDF
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="lang">Language</param>
        /// <param name="filePath">File path</param>
        public virtual void PrintOrderToPdf(Order order, Language lang, string filePath)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (lang == null)
                throw new ArgumentNullException("lang");

            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            var doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();

            var titleFont = new Font();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.BLACK;

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
            var anchor = new Anchor(_storeInformationSettings.StoreUrl.Trim(new char[] { '/' }));
            anchor.Reference = _storeInformationSettings.StoreUrl;
            cell.AddElement(new Paragraph(anchor));
            cell.AddElement(new Paragraph(String.Format(_localizationService.GetResource("PDFInvoice.OrderDate", lang.Id), _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToString("D"))));
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
                cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Company", lang.Id), order.BillingAddress.Company)));

            cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Name", lang.Id), order.BillingAddress.FirstName + " " + order.BillingAddress.LastName)));
            cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Phone", lang.Id), order.BillingAddress.PhoneNumber)));
            if (!String.IsNullOrEmpty(order.BillingAddress.FaxNumber))
                cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Fax", lang.Id), order.BillingAddress.FaxNumber)));
            cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address", lang.Id), order.BillingAddress.Address1)));
            if (!String.IsNullOrEmpty(order.BillingAddress.Address2))
                cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address2", lang.Id), order.BillingAddress.Address2)));

            cell.AddElement(new Paragraph("   " + String.Format("{0}, {1}", order.BillingAddress.Country != null ? order.BillingAddress.Country.Name : "", order.BillingAddress.StateProvince != null ? order.BillingAddress.StateProvince.Name : "")));
            cell.AddElement(new Paragraph("   " + String.Format("{0}, {1}", order.BillingAddress.City, order.BillingAddress.ZipPostalCode)));
            
            //VAT number
            if (!String.IsNullOrEmpty(order.VatNumber))
                cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.VATNumber", lang.Id), order.VatNumber)));
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
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Company", lang.Id), order.ShippingAddress.Company)));
                cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Name", lang.Id), order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName)));
                cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Phone", lang.Id), order.ShippingAddress.PhoneNumber)));
                if (!String.IsNullOrEmpty(order.ShippingAddress.FaxNumber))
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Fax", lang.Id), order.ShippingAddress.FaxNumber)));
                cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address", lang.Id), order.ShippingAddress.Address1)));
                if (!String.IsNullOrEmpty(order.ShippingAddress.Address2))
                    cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address2", lang.Id), order.ShippingAddress.Address2)));
                cell.AddElement(new Paragraph("   " + String.Format("{0}, {1}", order.ShippingAddress.Country != null ? order.ShippingAddress.Country.Name : "", order.ShippingAddress.StateProvince != null ? order.ShippingAddress.StateProvince.Name : "")));
                cell.AddElement(new Paragraph("   " + String.Format("{0}, {1}", order.ShippingAddress.City, order.ShippingAddress.ZipPostalCode)));
                cell.AddElement(new Paragraph(" "));
                cell.AddElement(new Paragraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.ShippingMethod", lang.Id), order.ShippingMethod)));
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

            var productsTable = new PdfPTable(4);
            productsTable.WidthPercentage = 100f;
            productsTable.SetWidths(new[] { 40, 20, 20, 20 });

            //product name
            cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.ProductName", lang.Id)));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cell);

            //price
            cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.ProductPrice", lang.Id)));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cell);

            //qty
            cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.ProductQuantity", lang.Id)));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cell);

            //total
            cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.ProductTotal", lang.Id)));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cell);
            
            for (int i = 0; i < orderProductVariants.Count; i++)
            {
                var orderProductVariant = orderProductVariants[i];
                var pv = orderProductVariant.ProductVariant;

                //product name
                string name = "";
                if (!String.IsNullOrEmpty(pv.GetLocalized(x => x.Name)))
                    name = string.Format("{0} ({1})", pv.Product.GetLocalized(x => x.Name), pv.GetLocalized(x => x.Name));
                else
                    name = pv.Product.GetLocalized(x => x.Name);
                cell = new PdfPCell();
                cell.AddElement(new Paragraph(name));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                var attributesFont = new Font();
                attributesFont.SetStyle(Font.ITALIC);
                var attributesParagraph = new Paragraph(HtmlHelper.ConvertHtmlToPlainText(orderProductVariant.AttributeDescription, true),
                    attributesFont);
                cell.AddElement(attributesParagraph);
                productsTable.AddCell(cell);
                
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
                cell = new PdfPCell(new Phrase(unitPrice));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cell);

                //qty
                cell = new PdfPCell(new Phrase(orderProductVariant.Quantity.ToString()));
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
                cell = new PdfPCell(new Phrase(subTotal));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cell);
            }
            doc.Add(productsTable);

            #endregion

            #region Checkout attributes

            if (!String.IsNullOrEmpty(order.CheckoutAttributeDescription))
            {
                doc.Add(new Paragraph(" "));
                string attributes = HtmlHelper.ConvertHtmlToPlainText(order.CheckoutAttributeDescription, true);
                var pCheckoutAttributes = new Paragraph(attributes);
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

                        var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Sub-Total", lang.Id), orderSubtotalExclTaxStr));
                        p.Alignment = Element.ALIGN_RIGHT;
                        doc.Add(p);
                    }
                    break;
                case TaxDisplayType.IncludingTax:
                    {
                        var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                        string orderSubtotalInclTaxStr = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);

                        var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Sub-Total", lang.Id), orderSubtotalInclTaxStr));
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

                            var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Discount", lang.Id), orderSubTotalDiscountInCustomerCurrencyStr));
                            p.Alignment = Element.ALIGN_RIGHT;
                            doc.Add(p);
                        }
                        break;
                    case TaxDisplayType.IncludingTax:
                        {
                            var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                            string orderSubTotalDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);

                            var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Discount", lang.Id), orderSubTotalDiscountInCustomerCurrencyStr));
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
                            
                            var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Shipping", lang.Id), orderShippingExclTaxStr));
                            p.Alignment = Element.ALIGN_RIGHT;
                            doc.Add(p);
                        }
                        break;
                    case TaxDisplayType.IncludingTax:
                        {
                            var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                            string orderShippingInclTaxStr = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);

                            var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Shipping", lang.Id), orderShippingInclTaxStr));
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

                            var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.PaymentMethodAdditionalFee", lang.Id), paymentMethodAdditionalFeeExclTaxStr));
                            p.Alignment = Element.ALIGN_RIGHT;
                            doc.Add(p);
                        }
                        break;
                    case TaxDisplayType.IncludingTax:
                        {
                            var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                            string paymentMethodAdditionalFeeInclTaxStr = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);

                            var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.PaymentMethodAdditionalFee", lang.Id), paymentMethodAdditionalFeeInclTaxStr));
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
                    //TODO pass languageId to _priceFormatter.FormatPrice
                    taxStr = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                }
            }
            if (displayTax)
            {
                var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Tax", lang.Id), taxStr));
                p.Alignment = Element.ALIGN_RIGHT;
                doc.Add(p);
            }
            if (displayTaxRates)
            {
                foreach (var item in taxRates)
                {
                    string taxRate = String.Format(_localizationService.GetResource("PDFInvoice.Totals.TaxRate"), _priceFormatter.FormatTaxRate(item.Key));
                    //TODO pass languageId to _priceFormatter.FormatPrice
                    string taxValue = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(item.Value, order.CurrencyRate), true, false);
                    
                    var p = new Paragraph(String.Format("{0} {1}", taxRate, taxValue));
                    p.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(p);
                }
            }

            //discount (applied to order total)
            if (order.OrderDiscount > decimal.Zero)
            {
                var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
                string orderDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                
                var p = new Paragraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Discount", lang.Id), orderDiscountInCustomerCurrencyStr));
                p.Alignment = Element.ALIGN_RIGHT;
                doc.Add(p);
            }

            //gift cards
            foreach (var gcuh in order.GiftCardUsageHistory)
            {
                string gcTitle = string.Format(_localizationService.GetResource("PDFInvoice.GiftCardInfo", lang.Id), gcuh.GiftCard.GiftCardCouponCode);
                string gcAmountStr = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false);
                
                var p = new Paragraph(String.Format("{0} {1}", gcTitle, gcAmountStr));
                p.Alignment = Element.ALIGN_RIGHT;
                doc.Add(p);
            }

            //reward points
            if (order.RedeemedRewardPointsEntry != null)
            {
                string rpTitle = string.Format(_localizationService.GetResource("PDFInvoice.RewardPoints", lang.Id), -order.RedeemedRewardPointsEntry.Points);
                string rpAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(order.RedeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false);

                var p = new Paragraph(String.Format("{0} {1}", rpTitle, rpAmount));
                p.Alignment = Element.ALIGN_RIGHT;
                doc.Add(p);
            }

            //order total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            //TODO pass languageId to _priceFormatter.FormatPrice
            string orderTotalStr = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false);


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
                    cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.OrderNotes.CreatedOn", lang.Id)));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    notesTable.AddCell(cell);

                    //note
                    cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFInvoice.OrderNotes.Note", lang.Id)));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    notesTable.AddCell(cell);

                    foreach (var orderNote in orderNotes)
                    {
                        cell = new PdfPCell();
                        cell.AddElement(new Paragraph(_dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc).ToString()));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        notesTable.AddCell(cell);

                        cell = new PdfPCell();
                        cell.AddElement(new Paragraph(HtmlHelper.ConvertHtmlToPlainText(HtmlHelper.FormatText(orderNote.Note, false, true, false, false, false, false), true)));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        notesTable.AddCell(cell);
                    }
                    doc.Add(notesTable);
                }
            }

            #endregion

            doc.Close();
        }

        /// <summary>
        /// Print packaging slips to PDF
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <param name="filePath">File path</param>
        public virtual void PrintPackagingSlipsToPdf(IList<Order> orders, string filePath)
        {
            if (orders == null)
                throw new ArgumentNullException("orders");

            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            var doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();
            
            int ordCount = orders.Count;
            int ordNum = 0;

            foreach (var order in orders)
            {
                if (order.ShippingAddress != null)
                {
                    var titleFont = new Font();
                    titleFont.SetStyle(Font.BOLD);
                    titleFont.Color = BaseColor.BLACK;
                    doc.Add(new Paragraph(String.Format("{0}# {1}", _localizationService.GetResource("PDFPackagingSlip.Order"), order.Id), titleFont));

                    if (!String.IsNullOrEmpty(order.ShippingAddress.Company))
                        doc.Add(new Paragraph((String.Format(_localizationService.GetResource("PDFPackagingSlip.Company"), order.ShippingAddress.Company))));

                    doc.Add(new Paragraph((String.Format(_localizationService.GetResource("PDFPackagingSlip.Name"), order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName))));
                    doc.Add(new Paragraph((String.Format(_localizationService.GetResource("PDFPackagingSlip.Phone"), order.ShippingAddress.PhoneNumber))));
                    doc.Add(new Paragraph((String.Format(_localizationService.GetResource("PDFPackagingSlip.Address"), order.ShippingAddress.Address1))));

                    if (!String.IsNullOrEmpty(order.ShippingAddress.Address2))
                        doc.Add(new Paragraph((String.Format(_localizationService.GetResource("PDFPackagingSlip.Address2"), order.ShippingAddress.Address2))));

                    doc.Add(new Paragraph((String.Format("{0}, {1}", order.ShippingAddress.Country != null ? order.ShippingAddress.Country.Name : "", order.ShippingAddress.StateProvince != null ? order.ShippingAddress.StateProvince.Name : ""))));
                    doc.Add(new Paragraph((String.Format("{0}, {1}", order.ShippingAddress.City, order.ShippingAddress.ZipPostalCode))));

                    doc.Add(new Paragraph(" "));

                    doc.Add(new Paragraph((String.Format(_localizationService.GetResource("PDFPackagingSlip.ShippingMethod"), order.ShippingMethod))));
                    doc.Add(new Paragraph(" "));

                    var productsTable = new PdfPTable(3);
                    productsTable.WidthPercentage = 100f;
                    productsTable.SetWidths(new[] { 60, 20, 20 });

                    //product name
                    var cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFPackagingSlip.ProductName")));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    productsTable.AddCell(cell);

                    //qty
                    cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFPackagingSlip.QTY")));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    productsTable.AddCell(cell);

                    //SKU
                    cell = new PdfPCell(new Phrase(_localizationService.GetResource("PDFPackagingSlip.SKU")));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    productsTable.AddCell(cell);

                    foreach (var orderProductVariant in order.OrderProductVariants)
                    {
                        //product name
                        var pv = orderProductVariant.ProductVariant;
                        string name = "";
                        if (!String.IsNullOrEmpty(pv.GetLocalized(x => x.Name)))
                            name = string.Format("{0} ({1})", pv.Product.GetLocalized(x => x.Name), pv.GetLocalized(x => x.Name));
                        else
                            name = pv.Product.GetLocalized(x => x.Name);
                        cell = new PdfPCell();
                        cell.AddElement(new Paragraph(name));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        var attributesFont = new Font();
                        attributesFont.SetStyle(Font.ITALIC);
                        var attributesParagraph = new Paragraph(HtmlHelper.ConvertHtmlToPlainText(orderProductVariant.AttributeDescription, true),
                            attributesFont);
                        cell.AddElement(attributesParagraph);
                        productsTable.AddCell(cell);


                        //qty
                        cell = new PdfPCell(new Phrase(orderProductVariant.Quantity.ToString()));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        productsTable.AddCell(cell);

                        //SKU
                        cell = new PdfPCell(new Phrase(pv.Sku ?? String.Empty));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        productsTable.AddCell(cell);
                    }
                    doc.Add(productsTable);
                }

                ordNum++;
                if (ordNum < ordCount)
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


            var doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();

            int productNumber = 1;
            int prodCount = products.Count;

            foreach (var product in products)
            {
                string productName = product.GetLocalized(x => x.Name, lang.Id);
                string productFullDescription = product.GetLocalized(x => x.FullDescription, lang.Id);

                var titleFont = new Font();
                titleFont.SetStyle(Font.BOLD);
                titleFont.Color = BaseColor.BLACK;
                doc.Add(new Paragraph(String.Format("{0}. {1}", productNumber, productName), titleFont));
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(productFullDescription))));
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
                    string pvName = String.IsNullOrEmpty(productVariant.GetLocalized(x => x.Name, lang.Id)) ? _localizationService.GetResource("PDFProductCatalog.UnnamedProductVariant") : productVariant.GetLocalized(x => x.Name, lang.Id);

                    doc.Add(new Paragraph(String.Format("{0}.{1}. {2}", productNumber, pvNum, pvName)));
                    doc.Add(new Paragraph(" "));

                    string productVariantDescription = productVariant.GetLocalized(x => x.Description, lang.Id);
                    if (!String.IsNullOrEmpty(productVariantDescription))
                    {
                        doc.Add(new Paragraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(productVariantDescription))));
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

                    doc.Add(new Paragraph(String.Format("{0}: {1} {2}", _localizationService.GetResource("PDFProductCatalog.Price"), productVariant.Price.ToString("0.00"), _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode)));
                    doc.Add(new Paragraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFProductCatalog.SKU"), productVariant.Sku)));

                    if (productVariant.IsShipEnabled && productVariant.Weight > Decimal.Zero)
                        doc.Add(new Paragraph(String.Format("{0}: {1} {2}", _localizationService.GetResource("PDFProductCatalog.Weight"), productVariant.Weight.ToString("0.00"), _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name)));

                    if (productVariant.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                        doc.Add(new Paragraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFProductCatalog.StockQuantity"), productVariant.StockQuantity)));

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