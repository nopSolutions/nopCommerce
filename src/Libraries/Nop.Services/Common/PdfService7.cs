// RTL Support provided by Credo inc (www.credo.co.il  ||   info@credo.co.il)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Html;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using iText.Kernel.Font;
using iText.IO.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Borders;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Action;
using iText.IO.Image;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Events;
using Nop.Services.Tax;
using System.Text.RegularExpressions;

namespace Nop.Services.Common
{
    /// <summary>
    /// PDF service
    /// </summary>
    public partial class PdfService7 : IPdfService
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
        private readonly ISettingService _settingContext;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;

        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly MeasureSettings _measureSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly TaxSettings _taxSettings;
        private readonly AddressSettings _addressSettings;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ITaxService _taxService;

        #endregion

        #region Ctor

        public PdfService7(ILocalizationService localizationService,
            ILanguageService languageService,
            IWorkContext workContext,
            IOrderService orderService,
            IPaymentService paymentService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            ICurrencyService currencyService,
            IMeasureService measureService,
            IPictureService pictureService,
            IProductService productService,
            IProductAttributeParser productAttributeParser,
            IStoreService storeService,
            IStoreContext storeContext,
            ISettingService settingContext,
            IAddressAttributeFormatter addressAttributeFormatter,
            CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            MeasureSettings measureSettings,
            PdfSettings pdfSettings,
            TaxSettings taxSettings,
            AddressSettings addressSettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            ITaxService taxService)
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
            this._settingContext = settingContext;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._currencySettings = currencySettings;
            this._catalogSettings = catalogSettings;
            this._measureSettings = measureSettings;
            this._pdfSettings = pdfSettings;
            this._taxSettings = taxSettings;
            this._addressSettings = addressSettings;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._taxService = taxService;
        }

        #endregion

        #region Utilities

        #region fonts
        /// <summary>
        /// Get font
        /// </summary>
        /// <returns>Font</returns>
        protected virtual PdfFont GetFont()
        {
            //nopCommerce supports unicode characters
            //nopCommerce uses Free Serif font by default (~/App_Data/Pdf/FreeSerif.ttf file)
            //It was downloaded from http://savannah.gnu.org/projects/freefont
            return GetFont(_pdfSettings.FontFileName);
        }
        /// <summary>
        /// Get font
        /// </summary>
        /// <param name="fontFileName">Font file name</param>
        /// <returns>Font</returns>
        protected virtual PdfFont GetFont(string fontFileName)
        {
            //if (fontFileName == null)
            //    throw new ArgumentNullException("fontFileName");

            //string fontPath = Path.Combine(CommonHelper.MapPath("~/App_Data/Pdf/"), fontFileName);
            //var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var font = PdfFontFactory.CreateFont(FontConstants.HELVETICA, PdfEncodings.CP1252, true);
            return font;
        }
        /// <summary>
        /// Get font bold
        /// </summary>
        /// <returns>Font</returns>
        protected virtual PdfFont GetFontBold()
        {
            var font = PdfFontFactory.CreateFont(FontConstants.HELVETICA_BOLD, PdfEncodings.CP1252, true);
            return font;
        }
        /// <summary>
        /// Get font italic
        /// </summary>
        /// <returns>Font</returns>
        protected virtual PdfFont GetFontItalic()
        {
            var font = PdfFontFactory.CreateFont(FontConstants.HELVETICA_OBLIQUE, PdfEncodings.CP1252, true);
            return font;
        }
        /// <summary>
        /// Get font bold italic
        /// </summary>
        /// <returns>Font</returns>
        protected virtual PdfFont GetFontBoldItalic()
        {
            var font = PdfFontFactory.CreateFont(FontConstants.HELVETICA_BOLDOBLIQUE, PdfEncodings.CP1252, true);
            return font;
        }
        #endregion

        #region generic
        protected virtual float millimetersToPoints(float value)
        {
            return (value / 25.4f) * 72f;
        }

        protected virtual string TakeCountLines(string text, int count)
        {
            string lines = "";
            int i = 0;
            Match match = Regex.Match(text, "^.*$", RegexOptions.Multiline);

            while (match.Success && i < count)
            {
                lines += match + "\n";
                match = match.NextMatch();
                i++;
            }

            return lines;
        }
        #endregion
        #region pagehandler
        private class PageNumSetter : IEventHandler
        {
            PdfFormXObject placeholder;
            float side = 20;
            float space = 3.5f;
            float descent = 3;
            float x; float y;
            string txtPage = "Page {0}";
            int pageOffset = 0;

            //public string txtPage { get; set; } = "Page {0}";
            //public int pageOffset { get; set; } = 0;

            public PageNumSetter(float posX, float posY, string TxtPage, int PageOffset)
            {
                placeholder = new PdfFormXObject(new Rectangle(0, 0, side, side));
                x = posX;
                y = posY;
                txtPage = TxtPage;
                pageOffset = PageOffset;

            }

            public void HandleEvent(Event e)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)e;
                PdfDocument pdf = docEvent.GetDocument();
                PdfPage page = docEvent.GetPage();
                int pageNumber = pdf.GetPageNumber(page) - pageOffset;
                Rectangle pageSize = page.GetPageSize();
                PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdf);
                Canvas canvas = new Canvas(pdfCanvas, pdf, pageSize);
                Paragraph p = new Paragraph()
                    .Add(string.Format(txtPage, pageNumber)).Add(" / ").SetFontSize(8);
                canvas.ShowTextAligned(p, x, y, TextAlignment.RIGHT);
                pdfCanvas.AddXObject(placeholder, x + space, y - descent);
                pdfCanvas.Release();


            }
            public void writeTotPageNum(PdfDocument pdf)
            {
                Canvas canvas = new Canvas(placeholder, pdf);
                Paragraph p = new Paragraph()
                    .Add((pdf.GetNumberOfPages() - pageOffset).ToString()).SetFontSize(8);
                canvas.ShowTextAligned(p, 0, descent, TextAlignment.LEFT);
            }


        }
        #endregion
        #endregion

        #region Methods

        /// <summary>
        /// Print an order to PDF
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        /// <param name="vendorId">Vendor identifier to limit products; 0 to to print all products. If specified, then totals won't be printed</param>
        /// <returns>A path of generated file</returns>
        public virtual string PrintOrderToPdf(Order order, int languageId = 0, int vendorId = 0)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            string fileName = string.Format("order_{0}_{1}.pdf", order.OrderGuid, CommonHelper.GenerateRandomDigitCode(4));
            string filePath = System.IO.Path.Combine(CommonHelper.MapPath("~/content/files/ExportImport"), fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var orders = new List<Order>();
                orders.Add(order);
                PrintOrdersToPdf(fileStream, orders, languageId, vendorId);
            }
            return filePath;
        }

        /// <summary>
        /// Print orders to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="orders">Orders</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        /// <param name="vendorId">Vendor identifier to limit products; 0 to to print all products. If specified, then totals won't be printed</param>
        public virtual void PrintOrdersToPdf(Stream stream, IList<Order> orders, int languageId = 0, int vendorId = 0)
        {
            #region doc settings
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (orders == null)
                throw new ArgumentNullException("orders");

            var pageSize = PageSize.A4; //595 x 842

            if (_pdfSettings.LetterPageSizeEnabled)
            {
                pageSize = PageSize.LETTER;
            }

            var pdfWriter = new PdfWriter(stream);
            var pdfDoc = new PdfDocument(pdfWriter);
            var doc = new Document(pdfDoc, pageSize);//, false);

            //store footer properties
            var footerY = doc.GetLeftMargin();
            var footerX = doc.GetBottomMargin();
            var footerWidht = pageSize.GetWidth() - doc.GetLeftMargin() - doc.GetRightMargin();

            //set margin for footer
            var bottomMatgin = 100f;
            doc.SetBottomMargin(bottomMatgin);

            //generic vars
            var cellPdf = new Cell();
            var cellPdf2 = new Cell();
            var paraPdf = new Paragraph();

            pdfDoc.GetCatalog().SetPageLayout(PdfName.SinglePage);
            //info
            PdfDocumentInfo info = pdfDoc.GetDocumentInfo();
            info.SetTitle("Rechnung");
            info.SetAuthor("Förderverein The FoodCoop");
            info.SetSubject("Invoice");
            //info.SetCreator("The FoodCoop");

            //styles
            Style styleTitle = new Style().SetFont(GetFontBold()).SetFontSize(12).SetFontColor(Color.BLACK).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);
            Style styleNormal = new Style().SetFont(GetFont()).SetFontSize(8).SetFontColor(Color.BLACK).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);
            Style styleAttrib = new Style().SetFont(GetFontItalic()).SetFontSize(8).SetFontColor(Color.BLACK).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);
            Style style5 = new Style().SetFont(GetFont()).SetFontSize(5).SetFontColor(Color.BLACK).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);
            Style style5b = new Style().SetFont(GetFontBold()).SetFontSize(5).SetFontColor(Color.BLACK).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);
            Style style6 = new Style().SetFont(GetFont()).SetFontSize(6).SetFontColor(Color.BLACK).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);
            Style style6b = new Style().SetFont(GetFontBold()).SetFontSize(6).SetFontColor(Color.BLACK).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);
            Style style8 = new Style().SetFont(GetFont()).SetFontSize(8).SetFontColor(Color.BLACK).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);
            Style style8b = new Style().SetFont(GetFontBold()).SetFontSize(8).SetFontColor(Color.BLACK).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);
            Style style9b = new Style().SetFont(GetFontBold()).SetFontSize(9).SetFontColor(Color.BLACK).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);
            Style styleCell = new Style().SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetTextAlignment(TextAlignment.LEFT);

            #endregion


            int ordCount = orders.Count;
            int ordNum = 0;
            int pagesSofar = 0;

            foreach (var order in orders)
            {
                bool includingTax = order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;

                //by default _pdfSettings contains settings for the current active store
                //and we need PdfSettings for the store which was used to place an order
                //so let's load it based on a store of the current order
                var pdfSettingsByStore = _settingContext.LoadSetting<PdfSettings>(order.StoreId);


                var lang = _languageService.GetLanguageById(languageId == 0 ? order.CustomerLanguageId : languageId);
                if (lang == null || !lang.Published)
                    lang = _workContext.WorkingLanguage;

                var currency = _currencyService.GetCurrencyByCode(order.CustomerCurrencyCode)
                 ?? new Currency
                 {
                     CurrencyCode = order.CustomerCurrencyCode
                 };
                bool showCurrency = false;

                var pageEvent = new PageNumSetter(pageSize.GetWidth() / 2, footerX - 20, _localizationService.GetResource("PDFInvoice.Page", lang.Id) + " {0}", pagesSofar);
                pdfDoc.AddEventHandler(PdfDocumentEvent.INSERT_PAGE, pageEvent);

                //there is no need for a new page as last element is a footer
                //if (ordNum > 0)
                //    doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                //main layout is based on a single table
                int col = 7;
                int headerCol = 4; int logoCol = col - headerCol;
                int shippAddrCol = 4; int ivAddrCol = col - shippAddrCol;
                //main layout table. Will have only 1 column.
                var tabPage = new Table(col).SetBorder(Border.NO_BORDER).SetWidthPercent(100f);
                tabPage.SetProperty(Property.BORDER, null);
                tabPage.SetProperty(Property.TEXT_ALIGNMENT, TextAlignment.LEFT);

                #region Header&Logo

                var logoPicture = _pictureService.GetPictureById(pdfSettingsByStore.LogoPictureId);
                var logoExists = logoPicture != null;

                //store info
                var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
                var anchor = new Text(store.Url.Trim(new[] { '/' })).SetAction(PdfAction.CreateURI(store.Url));

                paraPdf = new Paragraph(new Text(store.CompanyName).AddStyle(styleTitle)).AddStyle(styleNormal).SetMultipliedLeading(1.2f).Add("\n");


                //We use seetings for address and bank
                if (!string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn1))
                {
                    paraPdf.Add(new Text(pdfSettingsByStore.InvoiceFooterTextColumn1).AddStyle(style5));
                    paraPdf.Add(new Text("\n").AddStyle(style6));
                }
                if (!string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn2))
                {
                    paraPdf.Add(new Text(pdfSettingsByStore.InvoiceFooterTextColumn2).AddStyle(style5));
                    paraPdf.Add(new Text("\n").AddStyle(style6));
                }

                paraPdf.Add(anchor.AddStyle(style5)).Add("\n");
                tabPage.AddHeaderCell(new Cell(1, logoExists ? headerCol : col).Add(paraPdf).AddStyle(styleCell));

                //logo
                if (logoExists)
                {
                    var logoFilePath = _pictureService.GetThumbLocalPath(logoPicture, 0, false);
                    var logo = new Image(ImageDataFactory.Create(logoFilePath)).SetAutoScale(true);
                    tabPage.AddHeaderCell(new Cell(1, logoCol).Add(logo).AddStyle(styleCell));
                }

                //empty Line
                tabPage.AddHeaderCell(new Cell(1, col).Add("\n").AddStyle(styleCell));

                #endregion

                #region Addresses

                #region shippingaddr

                //shipping info
                if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
                {
                    if (!order.PickUpInStore)
                    {
                        if (order.ShippingAddress == null)
                            throw new NopException(string.Format("Shipping is required, but address is not available. Order ID = {0}", order.Id));

                        paraPdf = new Paragraph(new Text(_localizationService.GetResource("PDFInvoice.ShippingInformation", lang.Id)).AddStyle(style8b)).AddStyle(styleNormal).SetMultipliedLeading(1.2f).SetFirstLineIndent(-5f).SetMarginLeft(5f);
                        paraPdf.Add("\n");
                        if (!String.IsNullOrEmpty(order.ShippingAddress.Company))
                            paraPdf.Add(new Text(order.ShippingAddress.Company)).Add("\n");
                        if (!String.IsNullOrEmpty(order.ShippingAddress.FirstName))
                            paraPdf.Add(new Text(order.ShippingAddress.FirstName)).Add(" ");
                        if (!String.IsNullOrEmpty(order.ShippingAddress.LastName))
                            paraPdf.Add(new Text(order.ShippingAddress.LastName)).Add("\n");
                        if (_addressSettings.PhoneEnabled && !String.IsNullOrEmpty(order.ShippingAddress.PhoneNumber))
                            paraPdf.Add(new Text(String.Format(_localizationService.GetResource("PDFInvoice.Phone", lang.Id), order.ShippingAddress.PhoneNumber))).Add("\n");
                        if (_addressSettings.FaxEnabled && !String.IsNullOrEmpty(order.ShippingAddress.FaxNumber))
                            paraPdf.Add(new Text(String.Format(_localizationService.GetResource("PDFInvoice.FaxNumber", lang.Id), order.ShippingAddress.FaxNumber))).Add("\n");
                        if (_addressSettings.StreetAddressEnabled && !String.IsNullOrEmpty(order.ShippingAddress.Address1))
                            paraPdf.Add(new Text(order.ShippingAddress.Address1)).Add("\n");
                        if (_addressSettings.StreetAddress2Enabled && !String.IsNullOrEmpty(order.ShippingAddress.Address2))
                            paraPdf.Add(new Text(order.ShippingAddress.Address2)).Add("\n");

                        if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled || _addressSettings.ZipPostalCodeEnabled)
                            paraPdf.Add(new Text(String.Format("{2} {0}, {1}", order.ShippingAddress.City, order.ShippingAddress.StateProvince != null ? order.ShippingAddress.StateProvince.GetLocalized(x => x.Name, lang.Id) : "", order.ShippingAddress.ZipPostalCode))).Add("\n");
                        if (_addressSettings.CountryEnabled && order.ShippingAddress.Country != null)
                            paraPdf.Add(new Text(String.Format("{0}", order.ShippingAddress.Country != null ? order.ShippingAddress.Country.GetLocalized(x => x.Name, lang.Id) : ""))).Add("\n");

                        //custom attributes
                        var customShippingAddressAttributes = _addressAttributeFormatter.FormatAttributes(order.ShippingAddress.CustomAttributes);
                        if (!String.IsNullOrEmpty(customShippingAddressAttributes))
                        {
                            //TODO: we should add padding to each line (in case if we have sevaral custom address attributes)
                            paraPdf.Add(new Text(HtmlHelper.ConvertHtmlToPlainText(customShippingAddressAttributes, true, true))).Add("\n");
                        }
                    }
                    else
                        if (order.PickupAddress != null)
                    {
                        paraPdf = new Paragraph(new Text(_localizationService.GetResource("PDFInvoice.Pickup", lang.Id)).AddStyle(style9b)).AddStyle(styleNormal).SetMultipliedLeading(1f).SetFirstLineIndent(5f);
                        paraPdf.Add("\n");
                        if (!string.IsNullOrEmpty(order.PickupAddress.Address1))
                            paraPdf.Add(new Text(String.Format(_localizationService.GetResource("PDFInvoice.Address", lang.Id), order.PickupAddress.Address1))).Add("\n");
                        if (!string.IsNullOrEmpty(order.PickupAddress.City))
                            paraPdf.Add(order.PickupAddress.City).Add("\n");
                        if (order.PickupAddress.Country != null)
                            paraPdf.Add(order.PickupAddress.Country.GetLocalized(x => x.Name, lang.Id)).Add("\n");
                        if (!string.IsNullOrEmpty(order.PickupAddress.ZipPostalCode))
                            paraPdf.Add(order.PickupAddress.ZipPostalCode).Add("\n");

                    }

                    tabPage.AddHeaderCell(new Cell(1, shippAddrCol).Add(paraPdf).AddStyle(styleCell));
                }
                else
                {
                    paraPdf.Add("\n");
                    tabPage.AddHeaderCell(new Cell(1, shippAddrCol).Add(paraPdf).AddStyle(styleCell));
                }

                #endregion

                #region billing
                //billing info
                paraPdf = new Paragraph(new Text(_localizationService.GetResource("PDFInvoice.BillingInformation", lang.Id)).AddStyle(style8b)).AddStyle(styleNormal).SetMultipliedLeading(1.2f).SetFirstLineIndent(-5f).SetMarginLeft(5f);
                paraPdf.Add("\n");
                //paraPdf = new Paragraph().AddStyle(styleNormal).SetMultipliedLeading(1.2f);
                if (_addressSettings.CompanyEnabled && !String.IsNullOrEmpty(order.BillingAddress.Company))
                    paraPdf.Add(new Text(order.BillingAddress.Company)).Add("\n");
                if (_addressSettings.CompanyEnabled && !String.IsNullOrEmpty(order.BillingAddress.FirstName))
                    paraPdf.Add(new Text(order.BillingAddress.FirstName)).Add(" ");
                if (_addressSettings.CompanyEnabled && !String.IsNullOrEmpty(order.BillingAddress.LastName))
                    paraPdf.Add(new Text(order.BillingAddress.LastName)).Add("\n");
                if (_addressSettings.PhoneEnabled && !String.IsNullOrEmpty(order.BillingAddress.PhoneNumber))
                    paraPdf.Add(new Text(String.Format(_localizationService.GetResource("PDFInvoice.Phone", lang.Id), order.BillingAddress.PhoneNumber))).Add("\n");
                if (_addressSettings.FaxEnabled && !String.IsNullOrEmpty(order.BillingAddress.FaxNumber))
                    paraPdf.Add(new Text(String.Format(_localizationService.GetResource("PDFInvoice.FaxNumber", lang.Id), order.BillingAddress.FaxNumber))).Add("\n");
                if (_addressSettings.StreetAddressEnabled && !String.IsNullOrEmpty(order.BillingAddress.Address1))
                    paraPdf.Add(new Text(order.BillingAddress.Address1)).Add("\n");
                if (_addressSettings.StreetAddress2Enabled && !String.IsNullOrEmpty(order.BillingAddress.Address2))
                    paraPdf.Add(new Text(order.BillingAddress.Address2)).Add("\n");

                if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled || _addressSettings.ZipPostalCodeEnabled)
                    paraPdf.Add(new Text(String.Format("{2} {0}, {1}", order.BillingAddress.City, order.BillingAddress.StateProvince != null ? order.BillingAddress.StateProvince.GetLocalized(x => x.Name, lang.Id) : "", order.BillingAddress.ZipPostalCode))).Add("\n");
                if (_addressSettings.CountryEnabled && order.BillingAddress.Country != null)
                    paraPdf.Add(new Text(String.Format("{0}", order.BillingAddress.Country != null ? order.BillingAddress.Country.GetLocalized(x => x.Name, lang.Id) : ""))).Add("\n");

                //VAT number
                //if (!String.IsNullOrEmpty(order.VatNumber))
                //    paraPdf.Add(new Text(String.Format(_localizationService.GetResource("PDFInvoice.VATNumber", lang.Id), order.VatNumber))).Add("\n");

                //custom attributes
                var customBillingAddressAttributes = _addressAttributeFormatter.FormatAttributes(order.BillingAddress.CustomAttributes);
                if (!String.IsNullOrEmpty(customBillingAddressAttributes))
                {
                    //TODO: we should add padding to each line (in case if we have sevaral custom address attributes)
                    paraPdf.Add(new Text(HtmlHelper.ConvertHtmlToPlainText(customBillingAddressAttributes, true, true))).Add("\n");
                }

                //vendors payment details from payment provider
                if (vendorId == 0)
                {
                    //custom values
                    var customValues = order.DeserializeCustomValues();
                    if (customValues != null)
                    {
                        foreach (var item in customValues)
                        {
                            if (!String.IsNullOrEmpty(item.Value.ToString()))
                              paraPdf.Add(new Text(item.Key + ": " + item.Value)).Add("\n");
                        }
                    }
                }

                //add
                tabPage.AddHeaderCell(new Cell(1, ivAddrCol).Add(paraPdf).AddStyle(styleCell));
                #endregion

                #endregion

                #region invoice header
                //empty Line
                tabPage.AddHeaderCell(new Cell(1, col).Add("\n").AddStyle(styleCell));
                //Invoice
                paraPdf = new Paragraph(new Text(_localizationService.GetResource(order.InvoiceId != null? "PDFInvoice.Invoice" :"PDFInvoice.Order", lang.Id)).AddStyle(styleTitle));
                if (order.InvoiceId != null)
                    paraPdf.Add(new Text(" " + order.InvoiceId).AddStyle(styleTitle));
                tabPage.AddHeaderCell(new Cell(1, col - 3).Add(paraPdf).AddStyle(styleCell));
                paraPdf = new Paragraph();
                if (order.InvoiceDateUtc.HasValue)
                {
                    paraPdf.Add(new Text(_localizationService.GetResource("PDFInvoice.InvoiceDate", lang.Id)).AddStyle(style9b)).SetTextAlignment(TextAlignment.RIGHT);
                    tabPage.AddHeaderCell(new Cell(1, 2).Add(paraPdf).AddStyle(styleCell));
                    paraPdf = new Paragraph(new Text(_dateTimeHelper.ConvertToUserTime((System.DateTime)(order.InvoiceDateUtc), DateTimeKind.Utc).ToString("d", new CultureInfo(lang.LanguageCulture)) ?? "").AddStyle(style9b)).SetTextAlignment(TextAlignment.RIGHT);
                    tabPage.AddHeaderCell(new Cell(1, 1).Add(paraPdf).AddStyle(styleCell));
                }
                else
                    tabPage.AddHeaderCell(new Cell(1, 3).Add(paraPdf).AddStyle(styleCell));

                #region invoice titles
                //compose titles

                //payment method
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
                string paymentMethodStr = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, lang.Id) : order.PaymentMethodSystemName;

                //titles
                string[,] tit = { { "PDFInvoice.Order#", order.Id.ToString() ?? ""}
                                , { "PDFInvoice.OrderDate", _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToString("d", new CultureInfo(lang.LanguageCulture)) ?? ""}
                                , { "Order.ShippingMethod", order.ShippingMethod ?? ""}
                                , { "Order.PaymentMethod",  paymentMethodStr ?? ""}
                                , { "Order.Payment.Status", order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext) ?? ""}
                                , { "Order.VatNumber", order.VatNumber ?? ""}
                                , { "PDFInvoice.Currency", order.CustomerCurrencyCode}
                                };

                for (var i = 0; i <= tit.GetUpperBound(0); i++)
                {
                    cellPdf = new Cell();
                    paraPdf = new Paragraph(new Text(String.Format(_localizationService.GetResource(tit[i, 0], lang.Id))).AddStyle(style8));
                    cellPdf.Add(paraPdf);
                    tabPage.AddHeaderCell(cellPdf.SetBorder(null)
                        .SetVerticalAlignment(VerticalAlignment.TOP)
                        .SetBackgroundColor(Color.LIGHT_GRAY)
                        .SetPadding(0)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetBorderTop(new SolidBorder(1))
                    );
                }

                for (var i = 0; i <= tit.GetUpperBound(0); i++)
                {
                    cellPdf = new Cell();
                    paraPdf = new Paragraph(new Text(tit[i, 1]).AddStyle(style9b));
                    cellPdf.Add(paraPdf);
                    tabPage.AddHeaderCell(cellPdf.SetBorder(null)
                        .SetVerticalAlignment(VerticalAlignment.BOTTOM)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetBorderBottom(new SolidBorder(0.5f))
                    );
                }

                #endregion

                #endregion

                #region Products

                //products
                var orderItems = order.OrderItems;

                var hasSku = _catalogSettings.ShowSkuOnProductDetailsPage;

                tabPage.AddHeaderCell(new Cell(1, hasSku ? 2 : 3).Add(_localizationService.GetResource("PDFInvoice.ProductName", lang.Id)).AddStyle(styleCell).AddStyle(style9b).SetBackgroundColor(Color.LIGHT_GRAY));
                if (_catalogSettings.ShowSkuOnProductDetailsPage)
                {
                    tabPage.AddHeaderCell(new Cell(1, 1).Add(_localizationService.GetResource("PDFInvoice.SKU", lang.Id)).AddStyle(styleCell).AddStyle(style9b).SetBackgroundColor(Color.LIGHT_GRAY));
                }
                tabPage.AddHeaderCell(new Cell(1, 1).Add(_localizationService.GetResource("PDFInvoice.VatRate", lang.Id)).AddStyle(styleCell).AddStyle(style9b).SetBackgroundColor(Color.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));
                tabPage.AddHeaderCell(new Cell(1, 1).Add(_priceFormatter.FormatTaxString(_localizationService.GetResource("PDFInvoice.ProductPrice", lang.Id), lang, includingTax)).AddStyle(styleCell).AddStyle(style9b).SetBackgroundColor(Color.LIGHT_GRAY).SetTextAlignment(TextAlignment.RIGHT));
                tabPage.AddHeaderCell(new Cell(1, 1).Add(_localizationService.GetResource("PDFInvoice.ProductQuantity", lang.Id)).AddStyle(styleCell).AddStyle(style9b).SetBackgroundColor(Color.LIGHT_GRAY).SetTextAlignment(TextAlignment.RIGHT));
                tabPage.AddHeaderCell(new Cell(1, 1).Add(_priceFormatter.FormatTaxString(_localizationService.GetResource("PDFInvoice.ProductTotal", lang.Id), lang, includingTax)).AddStyle(styleCell).AddStyle(style9b).SetBackgroundColor(Color.LIGHT_GRAY).SetTextAlignment(TextAlignment.RIGHT));
                int ic = 1; //init product numerator

                foreach (var orderItem in orderItems)
                {
                    var p = orderItem.Product;

                    //a vendor should have access only to his products
                    if (vendorId > 0 && p.VendorId != vendorId)
                        continue;

                    //product name
                    string name;
                    name = ic.ToString() + ") " + p.GetLocalized(x => x.Name, lang.Id);
                    paraPdf = new Paragraph(new Text(name).AddStyle(style9b)).AddStyle(styleNormal).SetMultipliedLeading(1.5f).Add("\n");

                    //attributes
                    if (!String.IsNullOrEmpty(orderItem.AttributeDescription))
                    {
                        paraPdf.Add(HtmlHelper.ConvertHtmlToPlainText(orderItem.AttributeDescription, true, true)).AddStyle(styleNormal).Add("\n");
                    }
                    //rental info
                    if (orderItem.Product != null && orderItem.Product.IsRental)
                    {
                        var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                        var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                        var rentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                            rentalStartDate, rentalEndDate);

                        paraPdf.Add(rentalInfo).AddStyle(styleNormal).Add("\n");
                    }

                    tabPage.AddCell(new Cell(1, hasSku ? 2 : 3).Add(paraPdf).AddStyle(styleCell).SetKeepTogether(true));

                    //SKU
                    if (_catalogSettings.ShowSkuOnProductDetailsPage)
                    {
                        var sku = p.FormatSku(orderItem.AttributesXml, _productAttributeParser);
                        paraPdf = new Paragraph(sku ?? String.Empty).AddStyle(styleNormal);
                        tabPage.AddCell(new Cell().Add(paraPdf).AddStyle(styleCell));
                    }
                    //vatrate
                    bool hasAttribVat = !String.IsNullOrEmpty(orderItem.AttributesXml) && orderItem.AttributesXml.Contains("<TaxRate Rate=");
                    paraPdf = new Paragraph(hasAttribVat ? "(*)" : _priceFormatter.FormatTaxRate(orderItem.VatRate)).AddStyle(styleNormal).SetTextAlignment(TextAlignment.CENTER);
                    tabPage.AddCell(new Cell().Add(paraPdf).AddStyle(styleCell));

                    //price
                    string unitPrice;
                    var unitPriceInCustomerCurrency = _currencyService.ConvertCurrency(includingTax ? orderItem.UnitPriceInclTax : orderItem.UnitPriceExclTax, order.CurrencyRate);
                    unitPrice = _priceFormatter.FormatPrice(unitPriceInCustomerCurrency, showCurrency, currency, lang, true, false);

                    paraPdf = new Paragraph(unitPrice).AddStyle(styleNormal).SetTextAlignment(TextAlignment.RIGHT);
                    tabPage.AddCell(new Cell().Add(paraPdf).AddStyle(styleCell));

                    //qty
                    paraPdf = new Paragraph(orderItem.Quantity.ToString()).AddStyle(styleNormal).SetTextAlignment(TextAlignment.RIGHT);
                    tabPage.AddCell(new Cell().Add(paraPdf).AddStyle(styleCell));

                    //total
                    string subTotal;
                    var priceInCustomerCurrency = _currencyService.ConvertCurrency(includingTax ? orderItem.PriceInclTax : orderItem.PriceExclTax, order.CurrencyRate);
                    subTotal = _priceFormatter.FormatPrice(priceInCustomerCurrency, showCurrency, currency, lang, true, false);

                    paraPdf = new Paragraph(subTotal).AddStyle(styleNormal).SetTextAlignment(TextAlignment.RIGHT);
                    tabPage.AddCell(new Cell().Add(paraPdf).AddStyle(styleCell));

                    //increase product numerator
                    ++ic;

                }

                #endregion

                #region checkout attributes
                //checkout attributes
                var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(order.CheckoutAttributesXml);
                if (attributeValues != null && attributeValues.Any())
                {
                    string name;
                    name = _localizationService.GetResource("PDFInvoice.CheckOutAttrib", lang.Id);
                    paraPdf = new Paragraph(new Text(name).AddStyle(styleTitle)).AddStyle(styleNormal).SetMultipliedLeading(1.5f);
                    tabPage.AddCell(new Cell(1, col).Add(paraPdf).AddStyle(styleCell));

                    foreach (var attributeValue in attributeValues)
                    {
                        decimal taxRate = 0;
                        var customer = order.Customer;
                        decimal itemAmount = _taxService.GetCheckoutAttributePrice(attributeValue, includingTax, customer, out taxRate);

                        //a vendor should have access only to his products
                        if (vendorId > 0)
                            continue;

                        //product name
                        paraPdf = new Paragraph(HtmlHelper.ConvertHtmlToPlainText(attributeValue.Name, true, true)).AddStyle(styleNormal).Add("\n");
                        tabPage.AddCell(new Cell(1, hasSku ? 2 : 3).Add(paraPdf).AddStyle(styleCell));

                        //SKU
                        if (_catalogSettings.ShowSkuOnProductDetailsPage)
                        {
                            paraPdf = new Paragraph("").AddStyle(styleNormal);
                            tabPage.AddCell(new Cell().Add(paraPdf).AddStyle(styleCell));
                        }
                        //vatrate
                        paraPdf = new Paragraph(_priceFormatter.FormatTaxRate(taxRate)).AddStyle(styleNormal).SetTextAlignment(TextAlignment.CENTER);
                        tabPage.AddCell(new Cell().Add(paraPdf).AddStyle(styleCell));

                        //price
                        string unitPrice;
                        var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(itemAmount, order.CurrencyRate);
                        unitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, showCurrency, currency, lang, true, false);

                        paraPdf = new Paragraph(unitPrice).AddStyle(styleNormal).SetTextAlignment(TextAlignment.RIGHT);
                        tabPage.AddCell(new Cell().Add(paraPdf).AddStyle(styleCell));

                        //qty
                        paraPdf = new Paragraph("1").AddStyle(styleNormal).SetTextAlignment(TextAlignment.RIGHT);
                        tabPage.AddCell(new Cell().Add(paraPdf).AddStyle(styleCell));

                        //total
                        string subTotal = unitPrice;
                        paraPdf = new Paragraph(subTotal).AddStyle(styleNormal).SetTextAlignment(TextAlignment.RIGHT);
                        tabPage.AddCell(new Cell().Add(paraPdf).AddStyle(styleCell));

                    }
                }
                #endregion

                //footer products, checkout table
                paraPdf = new Paragraph(_localizationService.GetResource("PDFInvoice.Continue", lang.Id)).AddStyle(styleNormal).SetTextAlignment(TextAlignment.RIGHT);
                tabPage.AddFooterCell(new Cell(1, col).Add(paraPdf).AddStyle(styleCell)).SetSkipLastFooter(true);

                tabPage.AddCell(new Cell(1, col).Add(" ").AddStyle(styleCell).SetBorderTop(new SolidBorder(0.1f)));

                #region Order notes first. not used atm
                //var notesCell = 4;
                //int totNotes = 0;
                //if (pdfSettingsByStore.RenderOrderNotes)
                //{
                //    var orderNotes = order.OrderNotes
                //        .Where(on => on.DisplayToCustomer)
                //        .OrderByDescending(on => on.CreatedOnUtc)
                //        .ToList();

                //    if (orderNotes.Any())
                //    {
                //        totNotes = orderNotes.Count();
                //        int notesCol = 4;
                //        var tabNotes = new Table(notesCol).SetBorder(Border.NO_BORDER).SetWidthPercent(100f);
                //        paraPdf = new Paragraph(new Text(_localizationService.GetResource("PDFInvoice.OrderNotes", lang.Id)).AddStyle(styleTitle)).AddStyle(styleNormal).SetMultipliedLeading(1.5f);
                //        tabNotes.AddHeaderCell(new Cell(1, notesCol).Add(paraPdf).AddStyle(styleCell));

                //        //created on
                //        tabNotes.AddHeaderCell(new Cell(1, 1).Add(_localizationService.GetResource("PDFInvoice.OrderNotes.CreatedOn", lang.Id)).AddStyle(styleCell).AddStyle(style9b).SetTextAlignment(TextAlignment.LEFT));

                //        //note
                //        tabNotes.AddHeaderCell(new Cell(1, notesCol - 1).Add(_localizationService.GetResource("PDFInvoice.OrderNotes.Note", lang.Id)).AddStyle(styleCell).AddStyle(style9b).SetTextAlignment(TextAlignment.LEFT));
                //        var orderNote = orderNotes.FirstOrDefault();

                //        paraPdf = new Paragraph(_dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc).ToString("g", new CultureInfo(lang.LanguageCulture))).AddStyle(styleNormal).SetTextAlignment(TextAlignment.LEFT);
                //        tabNotes.AddCell(new Cell(1, 1).Add(paraPdf).AddStyle(styleCell).SetKeepTogether(true));
                //        var strNotes = TakeCountLines(HtmlHelper.ConvertHtmlToPlainText(orderNote.FormatOrderNoteText(), true, true), 20);

                //        paraPdf = new Paragraph(strNotes).AddStyle(styleNormal).SetTextAlignment(TextAlignment.LEFT);
                //        tabNotes.AddCell(new Cell(1, notesCol - 1).Add(paraPdf).AddStyle(styleCell).SetKeepTogether(true));

                //            //should we display a link to downloadable files here?
                //            //I think, no. Onyway, PDFs are printable documents and links (files) are useful here

                //        tabPage.AddCell(new Cell(1, notesCell).Add(tabNotes).AddStyle(styleCell).SetPadding(0));
                //    }
                //}

                #endregion

                #region Order totals
                var taxRates = order.TaxRatesDictionary;

                var sumCell = col; // - notesCell;
                int subcol = col; // totNotes > 0 ? sumCell : col;
                var tabTot = new Table(subcol).SetBorder(Border.NO_BORDER).SetWidthPercent(100f);

                //vendors cannot see totals
                if (vendorId == 0)
                {
                    var lstSummary = new List<Tuple<string, decimal, bool, bool, bool, bool>> //desc, amount, show when zero,  doLocalize, borderTop, borderBottom
                    {
                        Tuple.Create("PDFInvoice.Sub-Total",  includingTax ? order.OrderSubtotalInclTax : order.OrderSubtotalExclTax, true, true, false, false) //order subtotal
                        ,Tuple.Create("PDFInvoice.Discount",  includingTax ? -order.OrderSubTotalDiscountInclTax : -order.OrderSubTotalDiscountExclTax, false, true, false, false) //discount (applied to order subtotal)
                        ,Tuple.Create("PDFInvoice.Shipping",  includingTax ? order.OrderShippingInclTax : order.OrderShippingExclTax, false,  true, order.ShippingStatus != ShippingStatus.ShippingNotRequired, false) //shipping
                        ,Tuple.Create("PDFInvoice.PaymentMethodAdditionalFee",  includingTax ? order.PaymentMethodAdditionalFeeInclTax : order.PaymentMethodAdditionalFeeExclTax, false, true, false, false) //payment fee
                        ,Tuple.Create("PDFInvoice.InvoiceDiscount",  -order.OrderDiscount, false, true, false, false) //discount (applied to order total)
                    };

                    if (!includingTax)
                    {
                        lstSummary.Add(new Tuple<string, decimal, bool, bool, bool, bool>("PDFInvoice.OrderAmount", order.OrderAmount, true, true, false, false)); //tax base
                        lstSummary.Add(new Tuple<string, decimal, bool, bool, bool, bool>("PDFInvoice.Tax", order.OrderTax, true, true, false, false)); //tax amount
                    }
                    //order total incl.
                    lstSummary.Add(new Tuple<string, decimal, bool, bool, bool, bool>("PDFInvoice.OrderAmountIncl", order.OrderAmountIncl, true, true, false, false));

                    //gift cards
                    foreach (var gcuh in order.GiftCardUsageHistory)
                    {
                        lstSummary.Add(new Tuple<string, decimal, bool, bool, bool, bool>(
                            string.Format(_localizationService.GetResource("PDFInvoice.GiftCardInfo", lang.Id), gcuh.GiftCard.GiftCardCouponCode), -gcuh.UsedValue, false, false, false, false
                            )
                        );
                    }

                    //reward points
                    if (order.RedeemedRewardPointsEntry != null)
                    {
                        lstSummary.Add(new Tuple<string, decimal, bool, bool, bool, bool>(
                           string.Format(_localizationService.GetResource("PDFInvoice.RewardPoints", lang.Id), -order.RedeemedRewardPointsEntry.Points), -order.RedeemedRewardPointsEntry.UsedAmount, false, false, false, false
                           )
                       );
                    }

                    //order total to pay
                    lstSummary.Add(new Tuple<string, decimal, bool, bool, bool, bool>("PDFInvoice.AmountToPay", order.OrderTotal, true, true, true, false));

                    foreach (var tupSummary in lstSummary)
                    {
                        var desc = tupSummary.Item1;
                        var amount = tupSummary.Item2;
                        var showZero = tupSummary.Item3;
                        var doLocalize = tupSummary.Item4;
                        var borderTop = tupSummary.Item5;
                        var borderBottom = tupSummary.Item6;

                        var amountInCustomerCurrency = _currencyService.ConvertCurrency(amount, order.CurrencyRate);
                        var amountInCustomerCurrencyStr = _priceFormatter.FormatPrice(amountInCustomerCurrency, showCurrency, currency, lang, includingTax, false);

                        if (amountInCustomerCurrency != decimal.Zero || showZero)
                        {
                            paraPdf = new Paragraph(doLocalize ? _localizationService.GetResource(desc, lang.Id) : desc).AddStyle(style9b).SetTextAlignment(TextAlignment.RIGHT);
                            cellPdf = new Cell(1, subcol - 1).Add(paraPdf).AddStyle(styleCell);
                            paraPdf = new Paragraph(amountInCustomerCurrencyStr).AddStyle(style9b).SetTextAlignment(TextAlignment.RIGHT);
                            cellPdf2 = new Cell(1, 1).Add(paraPdf).AddStyle(styleCell);
                            if (borderTop)
                            {
                                cellPdf.SetBorderTop(new SolidBorder(0.5f)); cellPdf2.SetBorderTop(new SolidBorder(0.5f));
                            }
                            if (borderBottom)
                            {
                                cellPdf.SetBorderBottom(new SolidBorder(0.5f)); cellPdf2.SetBorderBottom(new SolidBorder(0.5f));
                            }
                            tabTot.AddCell(cellPdf);
                            tabTot.AddCell(cellPdf2);
                        }
                    }


                }
                else
                    tabTot.AddCell("");

                tabPage.AddCell(new Cell(1, col) //totNotes > 0 ? sumCell : col)
                    .Add(tabTot).AddStyle(styleCell).SetPadding(0).SetKeepTogether(true));
                #endregion

                #region Order notes

                if (pdfSettingsByStore.RenderOrderNotes ) //&& totNotes > 1)
                {
                    var orderNotes = order.OrderNotes
                        .Where(on => on.DisplayToCustomer)
                        .OrderByDescending(on => on.CreatedOnUtc)
                        .ToList();


                    if (orderNotes.Any())
                    {
                        paraPdf = new Paragraph(new Text(_localizationService.GetResource("PDFInvoice.OrderNotes", lang.Id)).AddStyle(styleTitle)).AddStyle(styleNormal).SetMultipliedLeading(1.5f);
                        tabPage.AddCell(new Cell(1, col).Add(paraPdf).AddStyle(styleCell));

                        //created on
                        tabPage.AddCell(new Cell(1, 1).Add(_localizationService.GetResource("PDFInvoice.OrderNotes.CreatedOn", lang.Id)).AddStyle(styleCell).AddStyle(style9b).SetTextAlignment(TextAlignment.LEFT));

                        //note
                        tabPage.AddCell(new Cell(1, col - 1).Add(_localizationService.GetResource("PDFInvoice.OrderNotes.Note", lang.Id)).AddStyle(styleCell).AddStyle(style9b).SetTextAlignment(TextAlignment.LEFT));

                        foreach (var orderNote in orderNotes)//.Skip(1))
                        {
                            paraPdf = new Paragraph(_dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc).ToString("g", new CultureInfo(lang.LanguageCulture))).AddStyle(styleNormal).SetTextAlignment(TextAlignment.LEFT);
                            tabPage.AddCell(new Cell(1, 1).Add(paraPdf).AddStyle(styleCell));
                            var strNotes = TakeCountLines(HtmlHelper.ConvertHtmlToPlainText(orderNote.FormatOrderNoteText(), true, true), 20);

                            paraPdf = new Paragraph(strNotes).AddStyle(styleNormal).SetTextAlignment(TextAlignment.LEFT);
                            tabPage.AddCell(new Cell(1, col - 1).Add(paraPdf).AddStyle(styleCell));
                        }

                    }
                }

                #endregion

                #region tax summary
                var footerTable = new Table(col).SetBorder(Border.NO_BORDER).SetWidthPercent(100f);
                var displayTaxRates = _taxSettings.DisplayTaxRates;
                if (displayTaxRates)
                {
                    var taxTable = new Table(new float[] { 20, 20, 20, 20, 20 }).SetBorder(Border.NO_BORDER).SetWidthPercent(100f).AddStyle(styleNormal).SetHorizontalAlignment(HorizontalAlignment.LEFT);

                    //header
                    taxTable.AddCell(new Cell().Add(_localizationService.GetResource("PDFInvoice.VatRate", lang.Id)).AddStyle(styleCell).AddStyle(style8).SetBackgroundColor(Color.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));
                    taxTable.AddCell(new Cell().Add(_localizationService.GetResource(includingTax ? "PDFInvoice.OrderAmountIncl" : "PDFInvoice.OrderAmount", lang.Id)).AddStyle(styleCell).AddStyle(style8).SetBackgroundColor(Color.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));
                    taxTable.AddCell(new Cell().Add(_localizationService.GetResource(includingTax ? "PDFInvoice.DiscountAmountIncl" : "PDFInvoice.DiscountAmount", lang.Id)).AddStyle(styleCell).AddStyle(style8).SetBackgroundColor(Color.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));
                    taxTable.AddCell(new Cell().Add(_localizationService.GetResource("PDFInvoice.BaseAmount", lang.Id)).AddStyle(styleCell).AddStyle(style8).SetBackgroundColor(Color.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));
                    taxTable.AddCell(new Cell().Add(_localizationService.GetResource("PDFInvoice.VatAmount", lang.Id)).AddStyle(styleCell).AddStyle(style8).SetBackgroundColor(Color.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));


                    foreach (var item in taxRates)
                    {
                        string taxRate = String.Format(_priceFormatter.FormatTaxRate(item.Key));
                        string Amount = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(item.Value.Amount, order.CurrencyRate), showCurrency, order.CustomerCurrencyCode, false, lang);
                        string DiscountAmount = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(item.Value.DiscountAmount, order.CurrencyRate), showCurrency, order.CustomerCurrencyCode, false, lang);
                        string BaseAmount = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(item.Value.BaseAmount, order.CurrencyRate), showCurrency, order.CustomerCurrencyCode, false, lang);
                        string VatAmount = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(item.Value.VatAmount, order.CurrencyRate), showCurrency, order.CustomerCurrencyCode, false, lang);
                        //string AmountIncludingVAT = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(item.Value.AmountIncludingVAT, order.CurrencyRate), true, order.CustomerCurrencyCode, false, lang);

                        taxTable.AddCell(new Cell().Add(taxRate).AddStyle(styleCell).AddStyle(style8b).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(Color.WHITE));
                        taxTable.AddCell(new Cell().Add(Amount).AddStyle(styleCell).AddStyle(style8b).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(Color.WHITE));
                        taxTable.AddCell(new Cell().Add(DiscountAmount).AddStyle(styleCell).AddStyle(style8b).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(Color.WHITE));
                        taxTable.AddCell(new Cell().Add(BaseAmount).AddStyle(styleCell).AddStyle(style8b).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(Color.WHITE));
                        taxTable.AddCell(new Cell().Add(VatAmount).AddStyle(styleCell).AddStyle(style8b).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(Color.WHITE));
                    }

                    footerTable.AddCell(new Cell(1, 5).Add(taxTable).AddStyle(styleCell).SetPadding(0).SetBorderTop(new SolidBorder(0.1f)).SetBorderBottom(new SolidBorder(0.1f)));

                    var taxAmountTable = new Table(2).SetBorder(Border.NO_BORDER).SetWidthPercent(100f).AddStyle(styleNormal).SetHorizontalAlignment(HorizontalAlignment.LEFT);

                    //base amount head
                    paraPdf = new Paragraph(_localizationService.GetResource("PDFInvoice.BaseAmountTotal", lang.Id)).AddStyle(style8).SetTextAlignment(TextAlignment.CENTER);
                    taxAmountTable.AddCell(new Cell(1, 1).Add(paraPdf).AddStyle(styleCell).SetBackgroundColor(Color.LIGHT_GRAY));

                    //total pay header
                    paraPdf = new Paragraph(_localizationService.GetResource("PDFInvoice.AmountToPay", lang.Id)).AddStyle(style8).SetTextAlignment(TextAlignment.CENTER);
                    taxAmountTable.AddCell(new Cell(2, 1).Add(paraPdf).AddStyle(styleCell).SetBackgroundColor(Color.LIGHT_GRAY).SetVerticalAlignment(VerticalAlignment.TOP));

                    //base amount
                    var amountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderAmount, order.CurrencyRate);
                    string amountInCustomerCurrencyStr = _priceFormatter.FormatPrice(amountInCustomerCurrency, showCurrency, currency, lang, false, false);
                    paraPdf = new Paragraph(amountInCustomerCurrencyStr).AddStyle(style9b).SetTextAlignment(TextAlignment.CENTER);
                    taxAmountTable.AddCell(new Cell(1, 1).Add(paraPdf).AddStyle(styleCell).SetBackgroundColor(Color.WHITE));

                    //vat amount head
                    paraPdf = new Paragraph(_localizationService.GetResource("PDFInvoice.VatAmount", lang.Id)).AddStyle(style8).SetTextAlignment(TextAlignment.CENTER);
                    taxAmountTable.AddCell(new Cell(1, 1).Add(paraPdf).AddStyle(styleCell).SetBackgroundColor(Color.LIGHT_GRAY));

                    //total pay amount
                    amountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                    amountInCustomerCurrencyStr = _priceFormatter.FormatPrice(amountInCustomerCurrency, showCurrency, currency, lang, false, false);
                    paraPdf = new Paragraph(amountInCustomerCurrencyStr).AddStyle(style9b).SetTextAlignment(TextAlignment.CENTER);
                    taxAmountTable.AddCell(new Cell(2, 1).Add(paraPdf).AddStyle(styleCell).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetBackgroundColor(Color.WHITE));

                    //vat amount
                    amountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    amountInCustomerCurrencyStr = _priceFormatter.FormatPrice(amountInCustomerCurrency, showCurrency, currency, lang, false, false);
                    paraPdf = new Paragraph(amountInCustomerCurrencyStr).AddStyle(style9b).SetTextAlignment(TextAlignment.CENTER);
                    taxAmountTable.AddCell(new Cell(1, 1).Add(paraPdf).AddStyle(styleCell).SetBackgroundColor(Color.WHITE));

                    footerTable.AddCell(new Cell(1, 2).Add(taxAmountTable).AddStyle(styleCell).SetPadding(0).SetBorderTop(new SolidBorder(0.1f)).SetBorderBottom(new SolidBorder(0.1f)).SetKeepTogether(true));

                    //place footer in tabPage cell and position it using setfixedpos
                    tabPage.AddCell(new Cell(1, col).Add(footerTable).AddStyle(styleCell).SetPadding(0).SetKeepTogether(true).SetFixedPosition(footerX, footerY, footerWidht));

                }
                #endregion


                doc.Add(tabPage);

                //tmp margin marker
                //var pdfcan = new PdfCanvas(pdfDoc.GetLastPage());
                //pdfcan.MoveTo(0, bottomMatgin).LineTo(100, bottomMatgin).Stroke();


                //doc.Relayout();

                //finalize page
                pdfDoc.RemoveEventHandler(PdfDocumentEvent.INSERT_PAGE, pageEvent);
                pageEvent.writeTotPageNum(pdfDoc);
                pageEvent = null;
                pagesSofar = pdfDoc.GetNumberOfPages();


                ordNum++;

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
            var _pdfService = new PdfService(_localizationService, _languageService, _workContext, _orderService, _paymentService, _dateTimeHelper, _priceFormatter, _currencyService, _measureService, _pictureService, _productService, _productAttributeParser, _storeService, _storeContext, _settingContext, _addressAttributeFormatter, _catalogSettings, _currencySettings, _measureSettings, _pdfSettings, _taxSettings, _addressSettings);
            _pdfService.PrintPackagingSlipsToPdf(stream, shipments, languageId);

        }

        /// <summary>
        /// Print products to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="products">Products</param>
        public virtual void PrintProductsToPdf(Stream stream, IList<Product> products)
        {
            var _pdfService = new PdfService(_localizationService, _languageService, _workContext, _orderService, _paymentService, _dateTimeHelper, _priceFormatter, _currencyService, _measureService, _pictureService, _productService, _productAttributeParser, _storeService, _storeContext, _settingContext, _addressAttributeFormatter, _catalogSettings, _currencySettings, _measureSettings, _pdfSettings, _taxSettings, _addressSettings);
            _pdfService.PrintProductsToPdf(stream, products);

        }
        #endregion
    }


}