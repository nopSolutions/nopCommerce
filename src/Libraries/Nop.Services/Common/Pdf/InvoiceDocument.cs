using System.ComponentModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents the invoice document
/// </summary>
public partial class InvoiceDocument : PdfDocument<ProductItem>
{
    #region Utilities

    protected virtual PdfGrid CreateAdressesInfo()
    {
        var hasShipping = !string.IsNullOrEmpty(ShippingAddress.ShippingMethod);
        var addressesTable = PdfDocumentHelper.BuildPdfGrid(numColumns: hasShipping ? 2 : 1, DocumentRunDirection);

        var billingInfo = PdfDocumentHelper.BuildPdfPCell(BuildAddressTable<InvoiceDocument>(source => BillingAddress, BillingAddress), DocumentRunDirection);
        addressesTable.AddCell(billingInfo);

        if (hasShipping)
        {
            var shippingInfo = PdfDocumentHelper.BuildPdfPCell(BuildAddressTable<InvoiceDocument>(source => ShippingAddress, ShippingAddress), DocumentRunDirection);
            addressesTable.AddCell(shippingInfo);
        }

        return addressesTable;
    }

    protected virtual PdfGrid CreateInvoiceHeader()
    {
        var headerTable = PdfDocumentHelper.BuildPdfGrid(numColumns: 2, DocumentRunDirection);

        var info = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);
        info.SpacingAfter = 15;

        info.AddCell(BuildPdfPCell<InvoiceDocument>(source => OrderNumberText, OrderNumberText));
        info.AddCell(BuildHyperLinkCell<InvoiceDocument>(source => StoreUrl, StoreUrl));
        info.AddCell(BuildTextCell<InvoiceDocument>(source => OrderDateUser, OrderDateUser));

        headerTable.AddCell(PdfDocumentHelper.BuildPdfPCell(info, DocumentRunDirection, horizontalAlign: Element.ALIGN_LEFT));

        if (LogoData is not null)
        {
            var logo = PdfImageHelper.GetITextSharpImageFromByteArray(LogoData);
            headerTable.AddCell(new PdfPCell(logo, fit: true)
            {
                Border = 0,
                FixedHeight = 65,
                RunDirection = DocumentRunDirection,
                VerticalAlignment = Element.ALIGN_CENTER,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
        }
        else
        {
            headerTable.AddCell(new PdfPCell(new Phrase())
            {
                Border = 0,
                Padding = 0
            });
        }

        headerTable.AddCell(PdfDocumentHelper.BuildPdfPCell(CreateAdressesInfo(), DocumentRunDirection, collSpan: 2));

        return headerTable;
    }

    protected virtual PdfGrid CreateFooter(FooterData footerData)
    {
        var footerTable = PdfDocumentHelper.BuildPdfGrid(numColumns: 2, DocumentRunDirection);

        if (!FooterTextColumn1.Any() && !FooterTextColumn2.Any())
            return footerTable;

        var footer1Table = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);
        foreach (var line in FooterTextColumn1)
            footer1Table.AddCell(BuildPdfPCell(line));

        var footer2Table = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);
        foreach (var line in FooterTextColumn2)
            footer2Table.AddCell(BuildPdfPCell(line));

        footerTable.AddCell(PdfDocumentHelper.BuildPdfPCell(footer1Table, DocumentRunDirection));
        footerTable.AddCell(PdfDocumentHelper.BuildPdfPCell(footer2Table, DocumentRunDirection));

        footerTable.AddCell(BuildPdfPCell($"- {footerData.CurrentPageNumber} -", collSpan: 2, horizontalAlign: Element.ALIGN_CENTER));

        return footerTable;
    }

    protected virtual PdfGrid CreateSummary()
    {
        var summaryData = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);

        if (!string.IsNullOrEmpty(Totals.SubTotal))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.SubTotal, Totals.SubTotal));
        if (!string.IsNullOrEmpty(Totals.Discount))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.Discount, Totals.Discount));
        if (!string.IsNullOrEmpty(Totals.Shipping))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.Shipping, Totals.Shipping));
        if (!string.IsNullOrEmpty(Totals.PaymentMethodAdditionalFee))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.PaymentMethodAdditionalFee, Totals.PaymentMethodAdditionalFee));
        if (!string.IsNullOrEmpty(Totals.Tax))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.Tax, Totals.Tax));

        foreach (var rate in Totals.TaxRates)
            summaryData.AddCell(BuildPdfPCell(rate));

        foreach (var card in Totals.GiftCards)
            summaryData.AddCell(BuildPdfPCell(card));

        if (!string.IsNullOrEmpty(Totals.RewardPoints))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.RewardPoints, Totals.RewardPoints));
        if (!string.IsNullOrEmpty(Totals.OrderTotal))
            summaryData.AddCell(BuildTextCell<InvoiceTotals>(totals => totals.OrderTotal, Totals.OrderTotal));

        return summaryData;
    }

    protected virtual PdfGrid CreateCheckoutAttributes()
    {
        var attributesData = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);

        attributesData.AddCell(BuildPdfPCell(CheckoutAttributes));

        return attributesData;
    }

    protected virtual PdfGrid CreateOrderNotes()
    {
        var notesTable = PdfDocumentHelper.BuildPdfGrid(numColumns: 2, DocumentRunDirection);

        if (OrderNotes?.Any() != true)
            return notesTable;

        notesTable.SetWidths([2, 5]);

        var fontBold = PdfDocumentHelper.GetFont(Font, Font.Size, DocumentFontStyle.Bold);
        var label = LabelField<InvoiceDocument, List<(string, string)>>(invoice => invoice.OrderNotes, fontBold, Language);

        notesTable.AddCell(
            new PdfPCell(new Phrase(label))
            {
                Border = 0,
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_LEFT,
                PaddingBottom = 5,
                RunDirection = DocumentRunDirection,
            });

        foreach (var (date, note) in OrderNotes)
        {
            notesTable.AddCell(BuildPdfPCell(Language.Rtl ? date.FixWeakCharacters() : date));
            notesTable.AddCell(BuildPdfPCell(note));
        }

        return notesTable;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate the invoice
    /// </summary>
    /// <param name="pdfStreamOutput">Stream for PDF output</param>
    public override void Generate(Stream pdfStreamOutput)
    {
        Document
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
            })
            .MainTableDataSource(dataSource =>
            {
                dataSource.StronglyTypedList(Products);
            })
            .PagesFooter(footer =>
            {
                footer.InlineFooter(inlineFooter =>
                {
                    inlineFooter.FooterProperties(new FooterBasicProperties
                    {
                        PdfFont = footer.PdfFont,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        RunDirection = Language.Rtl ? PdfRunDirection.RightToLeft : PdfRunDirection.LeftToRight
                    });
                    inlineFooter.AddPageFooter(data => CreateFooter(data));
                });
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Name, width: 10, printProductAttributes: true));
                if (ShowSkuInProductList)
                    columns.AddColumn(column => ConfigureProductColumn(column, p => p.Sku, width: 3));
                if (ShowVendorInProductList)
                    columns.AddColumn(column => ConfigureProductColumn(column, p => p.VendorName, width: 3));
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Price, width: 3));
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Quantity, width: 2));
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Total, width: 3));
            })
            .MainTableEvents(events =>
            {
                events.MainTableCreated(events =>
                {
                    //add to body, since adding hyperlinks to document header is not allowed
                    events.PdfDoc.Add(CreateInvoiceHeader());
                });
                events.MainTableAdded(events =>
                {
                    var summaryTable = PdfDocumentHelper.BuildPdfGrid(numColumns: 3, DocumentRunDirection);
                    summaryTable.AddCell(PdfDocumentHelper.BuildPdfPCell(CreateCheckoutAttributes(), DocumentRunDirection, 3, horizontalAlign: Element.ALIGN_LEFT));

                    summaryTable.AddCell(new PdfPCell() { Colspan = 2, Border = 0 });
                    summaryTable.AddCell(PdfDocumentHelper.BuildPdfPCell(CreateSummary(), DocumentRunDirection));

                    events.PdfDoc.Add(summaryTable);
                    events.PdfDoc.Add(CreateOrderNotes());
                });
            })
            .Generate(builder => builder.AsPdfStream(pdfStreamOutput, closeStream: false));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the logo binary
    /// </summary>
    public byte[] LogoData { get; set; }

    /// <summary>
    /// Gets or sets the date and time of order creation
    /// </summary>
    [DisplayName("Pdf.OrderDate")]
    public required string OrderDateUser { get; init; }

    /// <summary>
    /// Gets or sets the order number
    /// </summary>
    [DisplayName("Pdf.Order")]
    public required string OrderNumberText { get; init; }

    /// <summary>
    /// Gets or sets store location
    /// </summary>
    public string StoreUrl { get; init; }

    /// <summary>
    /// Gets or sets the billing address
    /// </summary>
    [DisplayName("Pdf.BillingInformation")]
    public required AddressItem BillingAddress { get; init; }

    /// <summary>
    /// Gets or sets the shipping address
    /// </summary>
    [DisplayName("Pdf.ShippingInformation")]
    public AddressItem ShippingAddress { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether to display product SKU in the invoice document
    /// </summary>
    public bool ShowSkuInProductList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display vendor name in the invoice document
    /// </summary>
    public bool ShowVendorInProductList { get; set; }

    /// <summary>
    /// Gets or sets the checkout attribute description
    /// </summary>
    public string CheckoutAttributes { get; set; }

    /// <summary>
    /// Gets or sets order totals
    /// </summary>
    public InvoiceTotals Totals { get; set; } = new();

    /// <summary>
    /// Gets or sets order notes
    /// </summary>
    [DisplayName("Pdf.OrderNotes")]
    public List<(string, string)> OrderNotes { get; set; }

    /// <summary>
    /// Gets or sets the text that will appear at the bottom of invoice (column 1)
    /// </summary>
    public List<string> FooterTextColumn1 { get; set; } = new();

    /// <summary>
    /// Gets or sets the text that will appear at the bottom of invoice (column 2)
    /// </summary>
    public List<string> FooterTextColumn2 { get; set; } = new();

    #endregion
}