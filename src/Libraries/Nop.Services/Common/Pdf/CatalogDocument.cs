using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents the catalog document
/// </summary>
public partial class CatalogDocument : PdfDocument<CatalogItem>
{
    #region Utilities

    protected virtual PdfPCell BuildProductImages(IList<CellData> rowData)
    {
        var table = PdfDocumentHelper.BuildPdfGrid(2, DocumentRunDirection);

        table.HorizontalAlignment = Element.ALIGN_CENTER;

        var picPaths = (HashSet<string>)rowData.GetValueOf<CatalogItem>(x => x.PicturePaths) ?? new();

        if (!picPaths.Any())
            return PdfDocumentHelper.BuildPdfPCell(table, DocumentRunDirection, horizontalAlign: Element.ALIGN_CENTER);

        foreach (var path in picPaths)
        {
            var image = PdfImageHelper.GetITextSharpImageFromByteArray(GetImageAsync(path).Result ?? Array.Empty<byte>());
            image.ScaleToFit(ImageTargetSize, ImageTargetSize);

            var imageCell = new PdfPCell(image, fit: false)
            {
                Border = 0,
                Padding = 10,
                VerticalAlignment = Element.ALIGN_CENTER,
                HorizontalAlignment = Element.ALIGN_CENTER
            };

            if (picPaths.Count % 2 != 0 && picPaths.Last() == path)
                imageCell.Colspan = 2;

            table.AddCell(imageCell);
        }

        return PdfDocumentHelper.BuildPdfPCell(table, DocumentRunDirection, horizontalAlign: Element.ALIGN_CENTER);
    }

    protected virtual PdfPCell BuildProductProperties(IList<CellData> rowData)
    {
        var table = PdfDocumentHelper.BuildPdfGrid(1, DocumentRunDirection);

        var price = rowData.GetSafeStringValueOf<CatalogItem>(p => p.Price);
        if (!string.IsNullOrEmpty(price))
            table.AddCell(BuildTextCell<CatalogItem>(p => p.Price, price));

        var sku = rowData.GetSafeStringValueOf<CatalogItem>(p => p.Sku);
        if (!string.IsNullOrEmpty(sku))
            table.AddCell(BuildTextCell<CatalogItem>(p => p.Sku, sku));

        var weight = rowData.GetSafeStringValueOf<CatalogItem>(p => p.Weight);
        if (!string.IsNullOrEmpty(weight))
            table.AddCell(BuildTextCell<CatalogItem>(p => p.Weight, weight));

        var stock = rowData.GetSafeStringValueOf<CatalogItem>(p => p.Stock);
        if (!string.IsNullOrEmpty(stock))
            table.AddCell(BuildTextCell<CatalogItem>(p => p.Stock, stock));

        return PdfDocumentHelper.BuildPdfPCell(table, DocumentRunDirection);
    }

    protected virtual PdfPCell ConfigureCatalogItemTemplate(InlineFieldData data)
    {
        var font16Bold = PdfDocumentHelper.GetFont(Font, Font.Size * 1.6f, DocumentFontStyle.Bold);

        var table = new PdfGrid(numColumns: 1)
        {
            WidthPercentage = 100,
            RunDirection = DocumentRunDirection,
            HorizontalAlignment = Element.ALIGN_LEFT
        };
        var rowData = data.Attributes.RowData.TableRowData;
        var name = rowData.GetSafeStringValueOf<CatalogItem>(p => p.Name);
        table.AddCell(new PdfPCell(new Phrase(name, font16Bold))
        {
            RunDirection = DocumentRunDirection,
            HorizontalAlignment = Element.ALIGN_LEFT,
            VerticalAlignment = Element.ALIGN_TOP,
            PaddingBottom = 14,
            Border = 0
        });

        var description = rowData.GetSafeStringValueOf<CatalogItem>(p => p.Description);

        var cell = new PdfPCell(new Paragraph(description, Font))
        {
            RunDirection = DocumentRunDirection,
            HorizontalAlignment = Element.ALIGN_LEFT,
            VerticalAlignment = Element.ALIGN_TOP,
            Border = 0,
            PaddingBottom = 10
        };
        cell.SetLeading(0f, 1.3f);
        table.AddCell(cell);

        table.AddCell(BuildProductProperties(rowData));
        table.AddCell(BuildProductImages(rowData));

        var tableContainer = new PdfGrid(numColumns: 1)
        {
            WidthPercentage = 100,
            RunDirection = DocumentRunDirection,
            HorizontalAlignment = Element.ALIGN_LEFT
        };

        tableContainer.AddCell(new PdfPCell(table)
        {
            VerticalAlignment = Element.ALIGN_TOP,
            HorizontalAlignment = Element.ALIGN_CENTER,
            Border = 0,
        });

        return new PdfPCell(tableContainer)
        {
            RunDirection = DocumentRunDirection,
            Border = 0,
            MinimumHeight = 25,
            VerticalAlignment = Element.ALIGN_TOP
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate the catalog
    /// </summary>
    /// <param name="pdfStreamOutput">Stream for PDF output</param>
    public override void Generate(Stream pdfStreamOutput)
    {
        Document
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                table.NumberOfDataRowsPerPage(1);
                table.ShowHeaderRow(false);
            })
            .MainTableDataSource(dataSource =>
            {
                dataSource.StronglyTypedList(Products);
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    ConfigureProductColumn(column, p => p.Name);
                    column.ColumnItemsTemplate(itemsTemplate =>
                    {
                        itemsTemplate.InlineField(inlineField => inlineField.RenderCell(ConfigureCatalogItemTemplate));
                    });
                });

            })
            .Generate(builder => builder.AsPdfStream(pdfStreamOutput, closeStream: false));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a function to get an image binary data by the path
    /// </summary>
    public required Func<string, Task<byte[]>> GetImageAsync { get; init; }

    #endregion
}