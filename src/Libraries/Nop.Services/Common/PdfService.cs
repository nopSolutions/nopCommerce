using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
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
            IPictureService pictureService, CurrencySettings currencySettings, 
            MeasureSettings measureSettings, PdfSettings pdfSettings, TaxSettings taxSettings, 
            StoreInformationSettings storeInformationSettings)
        {
            this._localizationService = localizationService;
            this._orderService = orderService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._measureService = measureService;
            this._pictureService = pictureService;
            this._currencySettings = currencySettings;
            this._measureSettings = measureSettings;
            this._pdfSettings = pdfSettings;
            this._taxSettings = taxSettings;
            this._storeInformationSettings = storeInformationSettings;
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

            Document doc = new Document();

            Section sec = doc.AddSection();

            Table table = sec.AddTable();
            table.Borders.Visible = false;

            //little hack here because MigraDoc doesn't support loading image from stream (only from file system)
            //that's why we save an image to a file system
            var logoPicture = _pictureService.GetPictureById(_pdfSettings.LogoPictureId);
            var logoExists = logoPicture != null;
            string logoFilePath = "";
            if (logoExists)
                logoFilePath = _pictureService.GetPictureLocalPath(logoPicture, 0, false);
            
            table.AddColumn(Unit.FromCentimeter(10));
            if (logoExists)
            {
                table.AddColumn(Unit.FromCentimeter(10));
            }

            Row ordRow = table.AddRow();

            int rownum = logoExists ? 1 : 0;
            Paragraph p1 = ordRow[rownum].AddParagraph(String.Format(_localizationService.GetResource("PDFInvoice.Order#", lang.Id), order.Id));
            p1.Format.Font.Bold = true;
            p1.Format.Font.Color = Colors.Black;
            ordRow[rownum].AddParagraph(_storeInformationSettings.StoreUrl.Trim(new char[] { '/' })).AddHyperlink(_storeInformationSettings.StoreUrl, HyperlinkType.Url);
            ordRow[rownum].AddParagraph(String.Format(_localizationService.GetResource("PDFInvoice.OrderDate", lang.Id), _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToString("D")));

            if (logoExists)
                ordRow[0].AddImage(logoFilePath);
            
            var addressTable = sec.AddTable();

            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                addressTable.AddColumn(Unit.FromCentimeter(9));
                addressTable.AddColumn(Unit.FromCentimeter(9));
            }
            else
            {
                addressTable.AddColumn(Unit.FromCentimeter(18));
            }
            addressTable.Borders.Visible = false;
            Row row = addressTable.AddRow();

            //billing info
            row.Cells[0].AddParagraph();
            Paragraph p2 = row.Cells[0].AddParagraph(_localizationService.GetResource("PDFInvoice.BillingInformation", lang.Id));
            p2.Format.Font.Bold = true;
            p2.Format.Font.Color = Colors.Black;


            if (!String.IsNullOrEmpty(order.BillingAddress.Company))
            {
                row.Cells[0].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Company", lang.Id), order.BillingAddress.Company));
            }
            row.Cells[0].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Name", lang.Id), order.BillingAddress.FirstName + " " + order.BillingAddress.LastName));
            row.Cells[0].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Phone", lang.Id), order.BillingAddress.PhoneNumber));
            if (!String.IsNullOrEmpty(order.BillingAddress.FaxNumber))
                row.Cells[0].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Fax", lang.Id), order.BillingAddress.FaxNumber));
            row.Cells[0].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address", lang.Id), order.BillingAddress.Address1));
            if (!String.IsNullOrEmpty(order.BillingAddress.Address2))
                row.Cells[0].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address2", lang.Id), order.BillingAddress.Address2));
            
            row.Cells[0].AddParagraph("   " + String.Format("{0}, {1}", order.BillingAddress.Country != null ? order.BillingAddress.Country.Name : "", order.BillingAddress.StateProvince != null ?  order.BillingAddress.StateProvince.Name : ""));
            row.Cells[0].AddParagraph("   " + String.Format("{0}, {1}", order.BillingAddress.City, order.BillingAddress.ZipPostalCode));
            //VAT number
            if (!String.IsNullOrEmpty(order.VatNumber))
            {
                row.Cells[0].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.VATNumber", lang.Id), order.VatNumber));
            }
            row.Cells[0].AddParagraph();

            //shipping info
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                row.Cells[1].AddParagraph();
                Paragraph p3 = row.Cells[1].AddParagraph(_localizationService.GetResource("PDFInvoice.ShippingInformation", lang.Id));
                p3.Format.Font.Bold = true;
                p3.Format.Font.Color = Colors.Black;

                if (!String.IsNullOrEmpty(order.ShippingAddress.Company))
                    row.Cells[1].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Company", lang.Id), order.ShippingAddress.Company));
                row.Cells[1].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Name", lang.Id), order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName));
                row.Cells[1].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Phone", lang.Id), order.ShippingAddress.PhoneNumber));
                if (!String.IsNullOrEmpty(order.ShippingAddress.FaxNumber))
                    row.Cells[1].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Fax", lang.Id), order.ShippingAddress.FaxNumber));
                row.Cells[1].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address", lang.Id), order.ShippingAddress.Address1));
                if (!String.IsNullOrEmpty(order.ShippingAddress.Address2))
                    row.Cells[1].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.Address2", lang.Id), order.ShippingAddress.Address2));
                row.Cells[1].AddParagraph("   " + String.Format("{0}, {1}", order.ShippingAddress.Country != null ? order.ShippingAddress.Country.Name : "", order.ShippingAddress.StateProvince != null ? order.ShippingAddress.StateProvince.Name : ""));
                row.Cells[1].AddParagraph("   " + String.Format("{0}, {1}", order.ShippingAddress.City, order.ShippingAddress.ZipPostalCode));
                row.Cells[1].AddParagraph();
                row.Cells[1].AddParagraph("   " + String.Format(_localizationService.GetResource("PDFInvoice.ShippingMethod", lang.Id), order.ShippingMethod));
                row.Cells[1].AddParagraph();
            }

            sec.AddParagraph();


            //products
            Paragraph p4 = sec.AddParagraph(_localizationService.GetResource("PDFInvoice.Product(s)", lang.Id));
            p4.Format.Font.Bold = true;
            p4.Format.Font.Color = Colors.Black;

            sec.AddParagraph();

            var orderProductVariants = _orderService.GetAllOrderProductVariants(order.Id, null, null, null, null, null, null);
            var tbl = sec.AddTable();

            tbl.Borders.Visible = true;
            tbl.Borders.Width = 1;

            tbl.AddColumn(Unit.FromCentimeter(8));
            tbl.AddColumn(Unit.FromCentimeter(4));
            tbl.AddColumn(Unit.FromCentimeter(2));
            tbl.AddColumn(Unit.FromCentimeter(4));

            Row header = tbl.AddRow();

            header.Cells[0].AddParagraph(_localizationService.GetResource("PDFInvoice.ProductName", lang.Id));
            header.Cells[0].Format.Alignment = ParagraphAlignment.Center;

            header.Cells[1].AddParagraph(_localizationService.GetResource("PDFInvoice.ProductPrice", lang.Id));
            header.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            header.Cells[2].AddParagraph(_localizationService.GetResource("PDFInvoice.ProductQuantity", lang.Id));
            header.Cells[2].Format.Alignment = ParagraphAlignment.Center;

            header.Cells[3].AddParagraph(_localizationService.GetResource("PDFInvoice.ProductTotal", lang.Id));
            header.Cells[3].Format.Alignment = ParagraphAlignment.Center;

            for (int i = 0; i < orderProductVariants.Count; i++)
            {
                var orderProductVariant = orderProductVariants[i];
                var pv = orderProductVariant.ProductVariant;
                Row prodRow = tbl.AddRow();

                string name = "";
                if (!String.IsNullOrEmpty(pv.GetLocalized(x => x.Name)))
                    name = string.Format("{0} ({1})", pv.Product.GetLocalized(x => x.Name), pv.GetLocalized(x => x.Name));
                else
                    name = pv.Product.GetLocalized(x => x.Name);

                prodRow.Cells[0].AddParagraph(name);
                Paragraph p5 = prodRow.Cells[0].AddParagraph(HtmlHelper.ConvertHtmlToPlainText(orderProductVariant.AttributeDescription, true));
                p5.Format.Font.Italic = true;
                prodRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;

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

                prodRow.Cells[1].AddParagraph(unitPrice);

                prodRow.Cells[2].AddParagraph(orderProductVariant.Quantity.ToString());

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
                prodRow.Cells[3].AddParagraph(subTotal);
            }

            //checkout attributes
            if (!String.IsNullOrEmpty(order.CheckoutAttributeDescription))
            {
                sec.AddParagraph();
                Paragraph pCheckoutAttributes = null;
                string attributes = HtmlHelper.ConvertHtmlToPlainText(order.CheckoutAttributeDescription, true);
                pCheckoutAttributes = sec.AddParagraph(attributes);
                if (pCheckoutAttributes != null)
                {
                    pCheckoutAttributes.Format.Alignment = ParagraphAlignment.Right;
                }
            }

            //subtotal
            sec.AddParagraph();
            Paragraph p6 = null;
            switch (order.CustomerTaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    {
                        var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                        string orderSubtotalExclTaxStr = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);
                        p6 = sec.AddParagraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Sub-Total", lang.Id), orderSubtotalExclTaxStr));
                    }
                    break;
                case TaxDisplayType.IncludingTax:
                    {
                        var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                        string orderSubtotalInclTaxStr = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
                        p6 = sec.AddParagraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Sub-Total", lang.Id), orderSubtotalInclTaxStr));
                    }
                    break;
            }
            if (p6 != null)
            {
                p6.Format.Alignment = ParagraphAlignment.Right;
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
                            Paragraph p6a = sec.AddParagraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Discount", lang.Id), orderSubTotalDiscountInCustomerCurrencyStr));
                            p6a.Format.Alignment = ParagraphAlignment.Right;
                        }
                        break;
                    case TaxDisplayType.IncludingTax:
                        {
                            var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                            string orderSubTotalDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
                            Paragraph p6a = sec.AddParagraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Discount", lang.Id), orderSubTotalDiscountInCustomerCurrencyStr));
                            p6a.Format.Alignment = ParagraphAlignment.Right;
                        }
                        break;
                }
            }

            //shipping
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                Paragraph p9 = null;
                switch (order.CustomerTaxDisplayType)
                {
                    case TaxDisplayType.ExcludingTax:
                        {
                            var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                            string orderShippingExclTaxStr = _priceFormatter.FormatShippingPrice(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);
                            p9 = sec.AddParagraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Shipping", lang.Id), orderShippingExclTaxStr));
                        }
                        break;
                    case TaxDisplayType.IncludingTax:
                        {
                            var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                            string orderShippingInclTaxStr = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
                            p9 = sec.AddParagraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Shipping", lang.Id), orderShippingInclTaxStr));
                        }
                        break;
                }

                if (p9 != null)
                {
                    p9.Format.Alignment = ParagraphAlignment.Right;
                }
            }

            //payment fee
            if (order.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
            {
                Paragraph p10 = null;
                switch (order.CustomerTaxDisplayType)
                {
                    case TaxDisplayType.ExcludingTax:
                        {
                            var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                            string paymentMethodAdditionalFeeExclTaxStr = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);
                            p10 = sec.AddParagraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.PaymentMethodAdditionalFee", lang.Id), paymentMethodAdditionalFeeExclTaxStr));
                        }
                        break;
                    case TaxDisplayType.IncludingTax:
                        {
                            var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                            string paymentMethodAdditionalFeeInclTaxStr = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
                            p10 = sec.AddParagraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.PaymentMethodAdditionalFee", lang.Id), paymentMethodAdditionalFeeInclTaxStr));
                        }
                        break;
                }
                if (p10 != null)
                {
                    p10.Format.Alignment = ParagraphAlignment.Right;
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
                    taxStr = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                }
            }
            if (displayTax)
            {
                var p11 = sec.AddParagraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Tax", lang.Id), taxStr));
                p11.Format.Alignment = ParagraphAlignment.Right;
            }
            if (displayTaxRates)
            {
                foreach (var item in taxRates)
                {
                    string taxRate = String.Format(_localizationService.GetResource("PDFInvoice.Totals.TaxRate"), _priceFormatter.FormatTaxRate(item.Key));
                    string taxValue = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(item.Value, order.CurrencyRate), true, false);

                    var p13 = sec.AddParagraph(String.Format("{0} {1}", taxRate, taxValue));
                    p13.Format.Alignment = ParagraphAlignment.Right;
                }
            }

            //discount (applied to order total)
            if (order.OrderDiscount > decimal.Zero)
            {
                var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
                string orderDiscountInCustomerCurrencyStr = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                Paragraph p7 = sec.AddParagraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.Discount", lang.Id), orderDiscountInCustomerCurrencyStr));
                p7.Format.Alignment = ParagraphAlignment.Right;
            }

            //gift cards
            foreach (var gcuh in order.GiftCardUsageHistory)
            {
                string gcTitle = string.Format(_localizationService.GetResource("PDFInvoice.GiftCardInfo", lang.Id), gcuh.GiftCard.GiftCardCouponCode);
                string gcAmountStr = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false);
                Paragraph p8 = sec.AddParagraph(String.Format("{0} {1}", gcTitle, gcAmountStr));
                p8.Format.Alignment = ParagraphAlignment.Right;
            }

            //reward points
            if (order.RedeemedRewardPointsEntry != null)
            {
                string rpTitle = string.Format(_localizationService.GetResource("PDFInvoice.RewardPoints", lang.Id), -order.RedeemedRewardPointsEntry.Points);
                string rpAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(order.RedeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false);

                var p11 = sec.AddParagraph(String.Format("{0} {1}", rpTitle, rpAmount));
                p11.Format.Alignment = ParagraphAlignment.Right;
            }

            //order total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            string orderTotalStr = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false);
            var p12 = sec.AddParagraph(String.Format("{0} {1}", _localizationService.GetResource("PDFInvoice.OrderTotal", lang.Id), orderTotalStr));
            p12.Format.Font.Bold = true;
            p12.Format.Font.Color = Colors.Black;
            p12.Format.Alignment = ParagraphAlignment.Right;

            //order notes
            if (_pdfSettings.RenderOrderNotes)
            {
                var orderNotes = order.OrderNotes
                    .Where(on => on.DisplayToCustomer)
                    .OrderByDescending(on => on.CreatedOnUtc)
                    .ToList();
                if (orderNotes.Count > 0)
                {
                    Paragraph p14 = sec.AddParagraph(_localizationService.GetResource("PDFInvoice.OrderNotes", lang.Id));
                    p14.Format.Font.Bold = true;
                    p14.Format.Font.Color = Colors.Black;

                    sec.AddParagraph();

                    var tbl1 = sec.AddTable();

                    tbl1.Borders.Visible = true;
                    tbl1.Borders.Width = 1;

                    tbl1.AddColumn(Unit.FromCentimeter(6));
                    tbl1.AddColumn(Unit.FromCentimeter(12));

                    Row header1 = tbl1.AddRow();

                    header1.Cells[0].AddParagraph(_localizationService.GetResource("PDFInvoice.OrderNotes.CreatedOn", lang.Id));
                    header1.Cells[0].Format.Alignment = ParagraphAlignment.Center;

                    header1.Cells[1].AddParagraph(_localizationService.GetResource("PDFInvoice.OrderNotes.Note", lang.Id));
                    header1.Cells[1].Format.Alignment = ParagraphAlignment.Center;

                    foreach (var orderNote in orderNotes)
                    {
                        Row noteRow = tbl1.AddRow();

                        noteRow.Cells[0].AddParagraph(_dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc).ToString());
                        noteRow.Cells[1].AddParagraph(HtmlHelper.ConvertHtmlToPlainText(HtmlHelper.FormatText(orderNote.Note, false, true, false, false, false, false), true));
                    }
                }
            }

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = doc;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(filePath);
        }
        
        /// <summary>
        /// Print packaging slips to PDF
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <param name="filePath">File path</param>
        public virtual void PrintPackagingSlipsToPdf(IList<Order> orders, string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }
            
            Document doc = new Document();
            Section section = doc.AddSection();

            int ordCount = orders.Count;
            int ordNum = 0;

            foreach (var order in orders)
            {
                if (order.ShippingAddress != null)
                {
                    Paragraph p1 = section.AddParagraph(String.Format("{0} #{1}", _localizationService.GetResource("PDFPackagingSlip.Order"), order.Id));
                    p1.Format.Font.Bold = true;
                    p1.Format.Font.Color = Colors.Black;
                    p1.Format.Font.Underline = Underline.None;
                    section.AddParagraph();

                    if (!String.IsNullOrEmpty(order.ShippingAddress.Company))
                    {
                        section.AddParagraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Company"), order.ShippingAddress.Company));
                    }
                    section.AddParagraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Name"), order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName));
                    section.AddParagraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Phone"), order.ShippingAddress.PhoneNumber));
                    section.AddParagraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Address"), order.ShippingAddress.Address1));
                    if (!String.IsNullOrEmpty(order.ShippingAddress.Address2))
                        section.AddParagraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.Address2"), order.ShippingAddress.Address2));
                    section.AddParagraph(String.Format("{0}, {1}", order.ShippingAddress.Country != null ? order.ShippingAddress.Country.Name : "", order.ShippingAddress.StateProvince != null ? order.ShippingAddress.StateProvince.Name : ""));
                    section.AddParagraph(String.Format("{0}, {1}", order.ShippingAddress.City, order.ShippingAddress.ZipPostalCode));

                    section.AddParagraph();

                    section.AddParagraph(String.Format(_localizationService.GetResource("PDFPackagingSlip.ShippingMethod"), order.ShippingMethod));
                    section.AddParagraph();

                    Table productTable = section.AddTable();
                    productTable.Borders.Visible = true;
                    productTable.AddColumn(Unit.FromCentimeter(4));
                    productTable.AddColumn(Unit.FromCentimeter(10));
                    productTable.AddColumn(Unit.FromCentimeter(4));

                    Row header = productTable.AddRow();
                    header.Shading.Color = Colors.LightGray;

                    header.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                    header.Cells[0].AddParagraph(_localizationService.GetResource("PDFPackagingSlip.QTY"));

                    header.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                    header.Cells[1].AddParagraph(_localizationService.GetResource("PDFPackagingSlip.ProductName"));

                    header.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                    header.Cells[2].AddParagraph(_localizationService.GetResource("PDFPackagingSlip.SKU"));

                    var opvc = order.OrderProductVariants;
                    foreach (var orderProductVariant in opvc)
                    {
                        Row row = productTable.AddRow();

                        row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                        row.Cells[0].AddParagraph(orderProductVariant.Quantity.ToString());

                        var pv = orderProductVariant.ProductVariant;
                        string name = "";
                        if (!String.IsNullOrEmpty(pv.GetLocalized(x => x.Name)))
                            name = string.Format("{0} ({1})", pv.Product.GetLocalized(x => x.Name), pv.GetLocalized(x => x.Name));
                        else
                            name = pv.Product.GetLocalized(x => x.Name);

                        row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                        row.Cells[1].AddParagraph(name);
                        Paragraph p2 = row.Cells[1].AddParagraph(HtmlHelper.ConvertHtmlToPlainText(orderProductVariant.AttributeDescription, true));
                        p2.Format.Font.Italic = true;

                        row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                        row.Cells[2].AddParagraph(pv.Sku);
                    }
                }

                ordNum++;
                if (ordNum < ordCount)
                {
                    section.AddPageBreak();
                }
            }

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = doc;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(filePath);
        }
        
        /// <summary>
        /// Print product collection to PDF
        /// </summary>
        /// <param name="products">Products</param>
        /// <param name="lang">Language</param>
        /// <param name="filePath">File path</param>
        public virtual void PrintProductsToPdf(IList<Product> products, Language lang, string filePath)
        {
            if (lang == null)
                throw new ArgumentNullException("lang");

            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            Document doc = new Document();
            Section section = doc.AddSection();

            int productNumber = 1;
            int prodCount = products.Count;

            foreach (var product in products)
            {
                string productName = product.GetLocalized(x => x.Name, lang.Id);
                string productFullDescription = product.GetLocalized(x => x.FullDescription, lang.Id);
                Paragraph p1 = section.AddParagraph(String.Format("{0}. {1}", productNumber, productName));
                p1.Format.Font.Bold = true;
                p1.Format.Font.Color = Colors.Black;

                section.AddParagraph();

                section.AddParagraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(productFullDescription)));

                section.AddParagraph();

                var pictures = _pictureService.GetPicturesByProductId(product.Id);
                if (pictures.Count > 0)
                {
                    Table table = section.AddTable();
                    table.Borders.Visible = false;

                    table.AddColumn(Unit.FromCentimeter(10));
                    table.AddColumn(Unit.FromCentimeter(10));

                    Row row = table.AddRow();
                    for (int i = 0; i < pictures.Count; i++)
                    {
                        int cellNum = i % 2;
                        var pic = pictures[i];

                        if (pic != null && pic.LoadPictureBinary() != null && pic.LoadPictureBinary().Length > 0)
                        {
                            row.Cells[cellNum].AddImage(_pictureService.GetPictureLocalPath(pic, 200, true));
                        }

                        if (i != 0 && i % 2 == 0)
                        {
                            row = table.AddRow();
                        }
                    }

                    section.AddParagraph();
                }

                int pvNum = 1;

                foreach (var productVariant in product.ProductVariants)
                {
                    string pvName = String.IsNullOrEmpty(productVariant.GetLocalized(x => x.Name, lang.Id)) ? _localizationService.GetResource("PDFProductCatalog.UnnamedProductVariant") : productVariant.GetLocalized(x => x.Name, lang.Id);
                    section.AddParagraph(String.Format("{0}.{1}. {2}", productNumber, pvNum, pvName));

                    section.AddParagraph();

                    string productVariantDescription = productVariant.GetLocalized(x => x.Description, lang.Id);
                    if (!String.IsNullOrEmpty(productVariantDescription))
                    {
                        section.AddParagraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(productVariantDescription)));
                        section.AddParagraph();
                    }

                    var pic = _pictureService.GetPictureById(productVariant.PictureId);
                    if (pic != null && pic.LoadPictureBinary() != null && pic.LoadPictureBinary().Length > 0)
                    {
                        section.AddImage(_pictureService.GetPictureLocalPath(pic, 200, true));
                    }

                    section.AddParagraph(String.Format("{0}: {1} {2}", _localizationService.GetResource("PDFProductCatalog.Price"), productVariant.Price.ToString("0.00"), _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode));
                    section.AddParagraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFProductCatalog.SKU"), productVariant.Sku));

                    if (productVariant.Weight > Decimal.Zero)
                    {
                        section.AddParagraph(String.Format("{0}: {1} {2}", _localizationService.GetResource("PDFProductCatalog.Weight"), productVariant.Weight.ToString("0.00"), _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name));
                    }

                    if (productVariant.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                    {
                        section.AddParagraph(String.Format("{0}: {1}", _localizationService.GetResource("PDFProductCatalog.StockQuantity"), productVariant.StockQuantity));
                    }

                    section.AddParagraph();

                    pvNum++;
                }

                productNumber++;

                if (productNumber <= prodCount)
                {
                    section.AddPageBreak();
                }
            }

            var renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = doc;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(filePath);
        }

        #endregion
    }
}