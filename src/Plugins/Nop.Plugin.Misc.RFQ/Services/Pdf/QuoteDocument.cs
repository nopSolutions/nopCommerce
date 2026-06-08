using iTextSharp.text;
using iTextSharp.text.pdf;
using Nop.Services.Common.Pdf;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace Nop.Plugin.Misc.RFQ.Services.Pdf;

/// <summary>
/// Represents the quote document
/// </summary>
public class QuoteDocument : PdfDocument<PdfQuoteItem>
{
    #region Utilities

    private PdfGrid CreateQuoteHeader()
    {
        var headerTable = PdfDocumentHelper.BuildPdfGrid(numColumns: 2, DocumentRunDirection);

        var info = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);
        info.SpacingAfter = 15;

        //quote info
        info.AddCell(BuildPdfPCell<QuoteDocument>(source => QuoteInfo.QuoteNumber, QuoteInfo.QuoteNumber));
        info.AddCell(BuildTextCell<QuoteDocument>(source => QuoteInfo.QuoteDate, QuoteInfo.QuoteDate));
        info.AddCell(BuildTextCell<QuoteDocument>(source => QuoteInfo.CustomerInfo, QuoteInfo.CustomerInfo));

        if (!string.IsNullOrEmpty(QuoteInfo.ExpirationDate))
            info.AddCell(BuildTextCell<QuoteDocument>(source => QuoteInfo.ExpirationDate, QuoteInfo.ExpirationDate));

        info.AddCell(BuildTextCell<QuoteDocument>(source => QuoteInfo.Status, QuoteInfo.Status));

        if (!string.IsNullOrEmpty(QuoteInfo.OrderNumber))
            info.AddCell(BuildTextCell<QuoteDocument>(source => QuoteInfo.OrderNumber, QuoteInfo.OrderNumber));

        info.AddCell(BuildHyperLinkCell<QuoteDocument>(source => QuoteUrl, QuoteUrl));
        headerTable.AddCell(PdfDocumentHelper.BuildPdfPCell(info, DocumentRunDirection, horizontalAlign: Element.ALIGN_LEFT));

        if (LogoData is not null)
        {
            var logo = LogoData.GetITextSharpImageFromByteArray();
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
            headerTable.AddCell(new PdfPCell([])
            {
                Border = 0,
                Padding = 0
            });
        }

        return headerTable;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate the pdf document
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
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Name, width: 10, printProductAttributes: true));
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Price, width: 3));
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Quantity, width: 2));
            })
            .MainTableEvents(events =>
            {
                events.MainTableCreated(events =>
                {
                    //add to body, since adding hyperlinks to document header is not allowed
                    events.PdfDoc.Add(CreateQuoteHeader());
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
    /// Gets or sets quote location
    /// </summary>
    public string QuoteUrl { get; init; }

    /// <summary>
    /// Gets or sets the quote info
    /// </summary>
    public required QuoteInfo QuoteInfo { get; init; }

    #endregion
}