//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): mb, haydie. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Utils.Html;

namespace NopSolutions.NopCommerce.BusinessLogic.Utils
{
    /// <summary>
    /// Represents a PDF helper
    /// </summary>
    public partial class PDFHelper
    {
        #region Methods
        /// <summary>
        /// Print product collection to PDF
        /// </summary>
        /// <param name="productCollection"></param>
        /// <param name="filePath"></param>
        public static void PrintProductsToPdf(List<Product> productCollection, string filePath)
        {
            if(String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            Document doc = new Document();
            Section section = doc.AddSection();

            int productNumber = 1;
            int prodCount = productCollection.Count;

            foreach(var product in productCollection)
            {
                Paragraph p1 = section.AddParagraph(String.Format("{0}. {1}", productNumber, product.LocalizedName));
                p1.Format.Font.Bold = true;
                p1.Format.Font.Color = Colors.Black;

                section.AddParagraph();

                section.AddParagraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(product.LocalizedFullDescription)));

                section.AddParagraph();

                var pictures = PictureManager.GetPicturesByProductId(product.ProductId);
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
                            row.Cells[cellNum].AddImage(PictureManager.GetPictureLocalPath(pic, 200, true));
                        }

                        if(i != 0 && i % 2 == 0)
                        {
                            row = table.AddRow();
                        }
                    }

                    section.AddParagraph();
                }

                int pvNum = 1;

                foreach(var productVariant in product.ProductVariants)
                {
                    string pvName = String.IsNullOrEmpty(productVariant.LocalizedName) ? LocalizationManager.GetLocaleResourceString("PDFProductCatalog.UnnamedProductVariant") : productVariant.LocalizedName;
                    section.AddParagraph(String.Format("{0}.{1}. {2}", productNumber, pvNum, pvName));

                    section.AddParagraph();

                    if (!String.IsNullOrEmpty(productVariant.LocalizedDescription))
                    {
                        section.AddParagraph(HtmlHelper.StripTags(HtmlHelper.ConvertHtmlToPlainText(productVariant.LocalizedDescription)));
                        section.AddParagraph();
                    }

                    var pic = productVariant.Picture;
                    if (pic != null && pic.LoadPictureBinary() != null && pic.LoadPictureBinary().Length > 0)
                    {
                        section.AddImage(PictureManager.GetPictureLocalPath(pic, 200, true));
                    }

                    section.AddParagraph(String.Format("{0}: {1} {2}", LocalizationManager.GetLocaleResourceString("PDFProductCatalog.Price"), productVariant.Price, CurrencyManager.PrimaryStoreCurrency.CurrencyCode));
                    section.AddParagraph(String.Format("{0}: {1}", LocalizationManager.GetLocaleResourceString("PDFProductCatalog.SKU"), productVariant.SKU));

                    if(productVariant.Weight > Decimal.Zero)
                    {
                        section.AddParagraph(String.Format("{0}: {1} {2}", LocalizationManager.GetLocaleResourceString("PDFProductCatalog.Weight"), productVariant.Weight, MeasureManager.BaseWeightIn.Name));
                    }

                    if(productVariant.ManageInventory == (int)ManageInventoryMethodEnum.ManageStock)
                    {
                        section.AddParagraph(String.Format("{0}: {1}", LocalizationManager.GetLocaleResourceString("PDFProductCatalog.StockQuantity"), productVariant.StockQuantity));
                    }

                    section.AddParagraph();

                    pvNum++;
                }

                productNumber++;

                if(productNumber <= prodCount)
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
        /// Print an order to PDF
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="filePath">File path</param>
        public static void PrintOrderToPdf(Order order, int languageId, string filePath)
        {
            if(order == null)
                throw new ArgumentNullException("order");

            if(String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            Language lang = LanguageManager.GetLanguageById(languageId);

            if (lang == null)
                throw new NopException("Language could not be loaded");

            Document doc = new Document();

            Section sec = doc.AddSection();

            Table table = sec.AddTable();
            table.Borders.Visible = false;

            bool logoExists = File.Exists(PDFHelper.LogoFilePath);

            table.AddColumn(Unit.FromCentimeter(10));
            if(logoExists)
            {
                table.AddColumn(Unit.FromCentimeter(10));
            }

            Row ordRow = table.AddRow();

            int rownum = logoExists ? 1 : 0;
            Paragraph p1 = ordRow[rownum].AddParagraph(String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Order#", languageId), order.OrderId));
            p1.Format.Font.Bold = true;
            p1.Format.Font.Color = Colors.Black;
            ordRow[rownum].AddParagraph(SettingManager.StoreUrl.Trim(new char[] { '/' })).AddHyperlink(SettingManager.StoreUrl, HyperlinkType.Url);
            ordRow[rownum].AddParagraph(String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.OrderDate", languageId), DateTimeHelper.ConvertToUserTime(order.CreatedOn, DateTimeKind.Utc).ToString("D")));

            if(File.Exists(PDFHelper.LogoFilePath))
            {
                ordRow[0].AddImage(PDFHelper.LogoFilePath);
            }


            var addressTable = sec.AddTable();

            if(order.ShippingStatus != ShippingStatusEnum.ShippingNotRequired)
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
            Paragraph p2 = row.Cells[0].AddParagraph(LocalizationManager.GetLocaleResourceString("PDFInvoice.BillingInformation", languageId));
            p2.Format.Font.Bold = true;
            p2.Format.Font.Color = Colors.Black;


            if(!String.IsNullOrEmpty(order.BillingCompany))
            {
                row.Cells[0].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Company", languageId), order.BillingCompany));
            }
            row.Cells[0].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Name", languageId), order.BillingFullName));
            row.Cells[0].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Phone", languageId), order.BillingPhoneNumber));
            if(!String.IsNullOrEmpty(order.BillingFaxNumber))
                row.Cells[0].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Fax", languageId), order.BillingFaxNumber));
            row.Cells[0].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Address", languageId), order.BillingAddress1));
            if(!String.IsNullOrEmpty(order.BillingAddress2))
                row.Cells[0].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Address2", languageId), order.BillingAddress2));
            row.Cells[0].AddParagraph("   " + String.Format("{0}, {1}", order.BillingCountry, order.BillingStateProvince));
            row.Cells[0].AddParagraph("   " + String.Format("{0}, {1}", order.BillingCity, order.BillingZipPostalCode));
            //VAT number
            if (!String.IsNullOrEmpty(order.VatNumber))
            {
                row.Cells[0].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.VATNumber", languageId), order.VatNumber));
            }
            row.Cells[0].AddParagraph();

            //shipping info
            if(order.ShippingStatus != ShippingStatusEnum.ShippingNotRequired)
            {
                row.Cells[1].AddParagraph();
                Paragraph p3 = row.Cells[1].AddParagraph(LocalizationManager.GetLocaleResourceString("PDFInvoice.ShippingInformation", languageId));
                p3.Format.Font.Bold = true;
                p3.Format.Font.Color = Colors.Black;

                if(!String.IsNullOrEmpty(order.ShippingCompany))
                    row.Cells[1].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Company", languageId), order.ShippingCompany));
                row.Cells[1].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Name", languageId), order.ShippingFullName));
                row.Cells[1].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Phone", languageId), order.ShippingPhoneNumber));
                if(!String.IsNullOrEmpty(order.ShippingFaxNumber))
                    row.Cells[1].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Fax", languageId), order.ShippingFaxNumber));
                row.Cells[1].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Address", languageId), order.ShippingAddress1));
                if(!String.IsNullOrEmpty(order.ShippingAddress2))
                    row.Cells[1].AddParagraph("   " + String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Address2", languageId), order.ShippingAddress2));
                row.Cells[1].AddParagraph("   " + String.Format("{0}, {1}", order.ShippingCountry, order.ShippingStateProvince));
                row.Cells[1].AddParagraph("   " + String.Format("{0}, {1}", order.ShippingCity, order.ShippingZipPostalCode));
                row.Cells[1].AddParagraph();
            }

            sec.AddParagraph();

            
            //products
            Paragraph p4 = sec.AddParagraph(LocalizationManager.GetLocaleResourceString("PDFInvoice.Product(s)", languageId));
            p4.Format.Font.Bold = true;
            p4.Format.Font.Color = Colors.Black;

            sec.AddParagraph();

            var productCollection = order.OrderProductVariants;
            var tbl = sec.AddTable();

            tbl.Borders.Visible = true;
            tbl.Borders.Width = 1;

            tbl.AddColumn(Unit.FromCentimeter(8));
            tbl.AddColumn(Unit.FromCentimeter(4));
            tbl.AddColumn(Unit.FromCentimeter(2));
            tbl.AddColumn(Unit.FromCentimeter(4));

            Row header = tbl.AddRow();

            header.Cells[0].AddParagraph(LocalizationManager.GetLocaleResourceString("PDFInvoice.ProductName", languageId));
            header.Cells[0].Format.Alignment = ParagraphAlignment.Center;

            header.Cells[1].AddParagraph(LocalizationManager.GetLocaleResourceString("PDFInvoice.ProductPrice", languageId));
            header.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            header.Cells[2].AddParagraph(LocalizationManager.GetLocaleResourceString("PDFInvoice.ProductQuantity", languageId));
            header.Cells[2].Format.Alignment = ParagraphAlignment.Center;

            header.Cells[3].AddParagraph(LocalizationManager.GetLocaleResourceString("PDFInvoice.ProductTotal", languageId));
            header.Cells[3].Format.Alignment = ParagraphAlignment.Center;

            for(int i = 0; i < productCollection.Count; i++)
            {
                var orderProductVariant = productCollection[i];
                int rowNum = i + 1;
                Row prodRow = tbl.AddRow();

                string name = String.Format("Not available. Id={0}", orderProductVariant.ProductVariantId);
                var pv = ProductManager.GetProductVariantById(orderProductVariant.ProductVariantId);
                if(pv != null)
                {
                    name = pv.GetLocalizedFullProductName(languageId);
                }

                prodRow.Cells[0].AddParagraph(name);
                Paragraph p5 = prodRow.Cells[0].AddParagraph(HtmlHelper.ConvertHtmlToPlainText(orderProductVariant.AttributeDescription, true));
                p5.Format.Font.Italic = true;
                prodRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                string unitPrice = string.Empty;
                switch(order.CustomerTaxDisplayType)
                {
                    case TaxDisplayTypeEnum.ExcludingTax:
                        unitPrice = PriceHelper.FormatPrice(orderProductVariant.UnitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);
                        break;
                    case TaxDisplayTypeEnum.IncludingTax:
                        unitPrice = PriceHelper.FormatPrice(orderProductVariant.UnitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
                        break;
                }

                prodRow.Cells[1].AddParagraph(unitPrice);

                prodRow.Cells[2].AddParagraph(orderProductVariant.Quantity.ToString());

                string subTotal = string.Empty;
                switch(order.CustomerTaxDisplayType)
                {
                    case TaxDisplayTypeEnum.ExcludingTax:
                        subTotal = PriceHelper.FormatPrice(orderProductVariant.PriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);
                        break;
                    case TaxDisplayTypeEnum.IncludingTax:
                        subTotal = PriceHelper.FormatPrice(orderProductVariant.PriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
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
            switch(order.CustomerTaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    {
                        string orderSubtotalExclTaxStr = PriceHelper.FormatPrice(order.OrderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);
                        p6 = sec.AddParagraph(String.Format("{0} {1}", LocalizationManager.GetLocaleResourceString("PDFInvoice.Sub-Total", languageId), orderSubtotalExclTaxStr));
                    }
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    {
                        string orderSubtotalInclTaxStr = PriceHelper.FormatPrice(order.OrderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
                        p6 = sec.AddParagraph(String.Format("{0} {1}", LocalizationManager.GetLocaleResourceString("PDFInvoice.Sub-Total", languageId), orderSubtotalInclTaxStr));
                    }
                    break;
            }
            if(p6 != null)
            {
                p6.Format.Alignment = ParagraphAlignment.Right;
            }

            //shipping
            if(order.ShippingStatus != ShippingStatusEnum.ShippingNotRequired)
            {
                Paragraph p9 = null;
                switch(order.CustomerTaxDisplayType)
                {
                    case TaxDisplayTypeEnum.ExcludingTax:
                        {
                            string orderShippingExclTaxStr = PriceHelper.FormatShippingPrice(order.OrderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);
                            p9 = sec.AddParagraph(String.Format("{0} {1}", LocalizationManager.GetLocaleResourceString("PDFInvoice.Shipping", languageId), orderShippingExclTaxStr));
                        }
                        break;
                    case TaxDisplayTypeEnum.IncludingTax:
                        {
                            string orderShippingInclTaxStr = PriceHelper.FormatShippingPrice(order.OrderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
                            p9 = sec.AddParagraph(String.Format("{0} {1}", LocalizationManager.GetLocaleResourceString("PDFInvoice.Shipping", languageId), orderShippingInclTaxStr));
                        }
                        break;
                }

                if(p9 != null)
                {
                    p9.Format.Alignment = ParagraphAlignment.Right;
                }
            }

            //payment fee
            if(order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
            {
                Paragraph p10 = null;
                switch(order.CustomerTaxDisplayType)
                {
                    case TaxDisplayTypeEnum.ExcludingTax:
                        {
                            string paymentMethodAdditionalFeeExclTaxStr = PriceHelper.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, false);
                            p10 = sec.AddParagraph(String.Format("{0} {1}", LocalizationManager.GetLocaleResourceString("PDFInvoice.PaymentMethodAdditionalFee", languageId), paymentMethodAdditionalFeeExclTaxStr));
                        }
                        break;
                    case TaxDisplayTypeEnum.IncludingTax:
                        {
                            string paymentMethodAdditionalFeeInclTaxStr = PriceHelper.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, lang, true);
                            p10 = sec.AddParagraph(String.Format("{0} {1}", LocalizationManager.GetLocaleResourceString("PDFInvoice.PaymentMethodAdditionalFee", languageId), paymentMethodAdditionalFeeInclTaxStr));
                        }
                        break;
                }
                if(p10 != null)
                {
                    p10.Format.Alignment = ParagraphAlignment.Right;
                }
            }

            //tax
            string taxStr = string.Empty;
            SortedDictionary<decimal, decimal> taxRates = new SortedDictionary<decimal, decimal>();
            bool displayTax = true;
            bool displayTaxRates = true;
            if(TaxManager.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayTypeEnum.IncludingTax)
            {
                displayTax = false;
            }
            else
            {
                if(order.OrderTax == 0 && TaxManager.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    taxRates = order.TaxRatesDictionaryInCustomerCurrency;

                    displayTaxRates = TaxManager.DisplayTaxRates && taxRates.Count > 0;
                    displayTax = !displayTaxRates;

                    taxStr = PriceHelper.FormatPrice(order.OrderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                }
            }
            if(displayTax)
            {
                var p11 = sec.AddParagraph(String.Format("{0} {1}", LocalizationManager.GetLocaleResourceString("PDFInvoice.Tax", languageId), taxStr));
                p11.Format.Alignment = ParagraphAlignment.Right;
            }
            if (displayTaxRates)
            {
                foreach (var item in taxRates)
                {
                    string taxRate = String.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.Totals.TaxRate"), TaxManager.FormatTaxRate(item.Key));
                    string taxValue = PriceHelper.FormatPrice(item.Value, true, false);

                    var p13 = sec.AddParagraph(String.Format("{0} {1}", taxRate, taxValue));
                    p13.Format.Alignment = ParagraphAlignment.Right;
                }
            }

            //discount
            if (order.OrderDiscountInCustomerCurrency > decimal.Zero)
            {
                string orderDiscountInCustomerCurrencyStr = PriceHelper.FormatPrice(-order.OrderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                Paragraph p7 = sec.AddParagraph(String.Format("{0} {1}", LocalizationManager.GetLocaleResourceString("PDFInvoice.Discount", languageId), orderDiscountInCustomerCurrencyStr));
                p7.Format.Alignment = ParagraphAlignment.Right;
            }

            //gift cards
            var gcuhC = OrderManager.GetAllGiftCardUsageHistoryEntries(null, null, order.OrderId);
            foreach (var giftCardUsageHistory in gcuhC)
            {
                string gcTitle = string.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.GiftCardInfo", languageId), giftCardUsageHistory.GiftCard.GiftCardCouponCode);
                string gcAmountStr = PriceHelper.FormatPrice(-giftCardUsageHistory.UsedValueInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                Paragraph p8 = sec.AddParagraph(String.Format("{0} {1}", gcTitle, gcAmountStr));
                p8.Format.Alignment = ParagraphAlignment.Right;
            }

            //reward points
            if (order.RedeemedRewardPoints != null)
            {
                string rpTitle = string.Format(LocalizationManager.GetLocaleResourceString("PDFInvoice.RewardPoints", languageId), -order.RedeemedRewardPoints.Points);
                string rpAmount = PriceHelper.FormatPrice(-order.RedeemedRewardPoints.UsedAmountInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                
                var p11 = sec.AddParagraph(String.Format("{0} {1}", rpTitle, rpAmount));
                p11.Format.Alignment = ParagraphAlignment.Right;
            }
           
            //order total
            string orderTotalStr = PriceHelper.FormatPrice(order.OrderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false);
            var p12 = sec.AddParagraph(String.Format("{0} {1}", LocalizationManager.GetLocaleResourceString("PDFInvoice.OrderTotal", languageId), orderTotalStr));
            p12.Format.Font.Bold = true;
            p12.Format.Font.Color = Colors.Black;
            p12.Format.Alignment = ParagraphAlignment.Right;


            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = doc;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(filePath);
        }

        /// <summary>
        /// Print packaging slips to PDF
        /// </summary>
        /// <param name="orderCollection">Order collection</param>
        /// <param name="filePath">File path</param>
        public static void PrintPackagingSlipsToPdf(List<Order> orderCollection, 
            string filePath)
        {
            if(String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            Document doc = new Document();
            Section section = doc.AddSection();

            int ordCount = orderCollection.Count;
            int ordNum = 0;

            foreach(var order in orderCollection)
            {
                Paragraph p1 = section.AddParagraph(String.Format("{0} #{1}", LocalizationManager.GetLocaleResourceString("PdfPackagingSlip.Order"), order.OrderId));
                p1.Format.Font.Bold = true;
                p1.Format.Font.Color = Colors.Black;
                p1.Format.Font.Underline = Underline.None;

                section.AddParagraph();

                section.AddParagraph(order.ShippingFullName);
                section.AddParagraph(order.ShippingAddress1);
                section.AddParagraph(String.Format("{0}, {1}", order.ShippingCity, order.ShippingZipPostalCode));

                section.AddParagraph();

                Table productTable = section.AddTable();
                productTable.Borders.Visible = true;
                productTable.AddColumn(Unit.FromCentimeter(4));
                productTable.AddColumn(Unit.FromCentimeter(10));
                productTable.AddColumn(Unit.FromCentimeter(4));

                Row header = productTable.AddRow();
                header.Shading.Color = Colors.LightGray;

                header.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                header.Cells[0].AddParagraph(LocalizationManager.GetLocaleResourceString("PdfPackagingSlip.QTY"));
                
                header.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                header.Cells[1].AddParagraph(LocalizationManager.GetLocaleResourceString("PdfPackagingSlip.ProductName"));

                header.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                header.Cells[2].AddParagraph(LocalizationManager.GetLocaleResourceString("PdfPackagingSlip.SKU"));

                var opvc = order.OrderProductVariants;
                foreach(var orderProductVariant in opvc)
                {
                    Row row = productTable.AddRow();

                    row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                    row.Cells[0].AddParagraph(orderProductVariant.Quantity.ToString());

                    string name = String.Format("Not available. ID={0}", orderProductVariant.ProductVariantId);
                    var pv = ProductManager.GetProductVariantById(orderProductVariant.ProductVariantId);
                    if(pv != null)
                    {
                        name = pv.LocalizedFullProductName;
                    }
                    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[1].AddParagraph(name);
                    Paragraph p2 = row.Cells[1].AddParagraph(HtmlHelper.ConvertHtmlToPlainText(orderProductVariant.AttributeDescription, true));
                    p2.Format.Font.Italic = true;

                    row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                    row.Cells[2].AddParagraph(orderProductVariant.ProductVariant.SKU);
                }

                ordNum++;

                if(ordNum < ordCount)
                {
                    section.AddPageBreak();
                }
            }
            
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = doc;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(filePath);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets a file path to PDF logo
        /// </summary>
        public static string LogoFilePath
        {
            get
            {
                return HttpContext.Current.Request.PhysicalApplicationPath + "images/pdflogo.img";
            }
        }
        #endregion
    }
}
