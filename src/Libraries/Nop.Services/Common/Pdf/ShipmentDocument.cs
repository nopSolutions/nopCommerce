using System.ComponentModel;
using PdfRpt.Core.Contracts;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents the shipment document
/// </summary>
public partial class ShipmentDocument : PdfDocument<ProductItem>
{
    #region Utilities

    private PdfGrid CreateDefaultHeader()
    {
        var headerTable = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);

        headerTable.AddCell(BuildPdfPCell<ShipmentDocument>(source => OrderNumberText, OrderNumberText));
        headerTable.AddCell(BuildPdfPCell<ShipmentDocument>(source => ShipmentNumberText, ShipmentNumberText));

        return headerTable;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate shipment
    /// </summary>
    /// <param name="pdfStreamOutput">Stream for PDF output</param>
    public override void Generate(Stream pdfStreamOutput)
    {
        Document
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
            })
            .PagesHeader(header =>
            {
                header.InlineHeader(inlineHeader =>
                {
                    inlineHeader.AddPageHeader(data => CreateDefaultHeader());
                });
            })
            .MainTableDataSource(dataSource =>
            {
                dataSource.StronglyTypedList(Products);
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Name, width: 10));
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Sku, width: 3));
                columns.AddColumn(column => ConfigureProductColumn(column, p => p.Quantity, width: 3));

            })
            .MainTableEvents(events => events.MainTableCreated(events =>
            {
                events.PdfDoc.Add(BuildAddressTable<ShipmentDocument>(p => p.Address, Address));
            }))
            .Generate(builder => builder.AsPdfStream(pdfStreamOutput, closeStream: false));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the shipping address
    /// </summary>
    [DisplayName("Pdf.ShippingInformation")]
    public required AddressItem Address { get; init; }

    /// <summary>
    /// Gets or sets the order number
    /// </summary>
    [DisplayName("Pdf.Order")]
    public required string OrderNumberText { get; init; }

    /// <summary>
    /// Gets or sets the shipment number
    /// </summary>
    [DisplayName("Pdf.Shipment")]
    public required string ShipmentNumberText { get; init; }

    #endregion
}