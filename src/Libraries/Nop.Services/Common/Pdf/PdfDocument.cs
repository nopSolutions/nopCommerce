using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using Language = Nop.Core.Domain.Localization.Language;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents base document class
/// </summary>
public abstract class PdfDocument<TItem>
{
    #region Utilities

    /// <summary>
    /// Build a cell with a hyperlink 
    /// </summary>
    /// <param name="labelSelector">Property selector to get resource key annotation</param>
    /// <param name="url">URL</param>
    /// <returns>A cell for PDF table</returns>
    protected virtual PdfPCell BuildHyperLinkCell<TLabel>(Expression<Func<TLabel, string>> labelSelector, string url)
    {
        ArgumentNullException.ThrowIfNull(labelSelector);
        ArgumentNullException.ThrowIfNullOrEmpty(url);

        var content = new Phrase();
        var label = LabelField(labelSelector, Font, Language);

        if (label.IsEmpty())
            label.Append(url);

        content.Add(new Anchor(label) { Reference = url });

        var cell = new PdfPCell(content)
        {
            HorizontalAlignment = Element.ALIGN_LEFT,
            RunDirection = DocumentRunDirection,
            Border = 0,
            Padding = 3
        };

        cell.SetLeading(0f, PdfDocumentHelper.RELATIVE_LEADING);

        return cell;
    }

    /// <summary>
    /// Build a cell with the given property
    /// </summary>
    /// <param name="labelSelector">Property selector to get resource key annotation</param>
    /// <param name="value">Value to format</param>
    /// <param name="horizontalAlign">Horizontal alignment</param>
    /// <returns>A cell for PDF table</returns>
    protected virtual PdfPCell BuildPdfPCell<TLabel>(Expression<Func<TLabel, string>> labelSelector, string value, int horizontalAlign = Element.ALIGN_LEFT)
    {
        var label = LabelField(labelSelector, Font, Language, value);

        var cell = new PdfPCell(new Phrase() { label })
        {
            RunDirection = DocumentRunDirection,
            HorizontalAlignment = horizontalAlign,
            VerticalAlignment = Element.ALIGN_CENTER,
            Border = 0,
            Padding = 3
        };

        cell.SetLeading(0f, PdfDocumentHelper.RELATIVE_LEADING);

        return cell;
    }

    /// <summary>
    /// Build a cell with the given text
    /// </summary>
    /// <param name="text">Text</param>
    /// <param name="collSpan">The number of columns occupied by a cell</param>
    /// <param name="horizontalAlign">Horizontal alignment</param>
    /// <param name="verticalAlignment">Vertical alignment</param>
    /// <returns>A cell for PDF table</returns>
    protected virtual PdfPCell BuildPdfPCell(string text, int collSpan = 1, int horizontalAlign = Element.ALIGN_LEFT, int verticalAlignment = Element.ALIGN_CENTER)
    {
        var cell = new PdfPCell(new Phrase(text, Font))
        {
            HorizontalAlignment = horizontalAlign,
            VerticalAlignment = verticalAlignment,
            Colspan = collSpan,
            RunDirection = DocumentRunDirection,
            Border = 0,
            Padding = 3
        };

        cell.SetLeading(0f, PdfDocumentHelper.RELATIVE_LEADING);

        return cell;
    }

    /// <summary>
    /// Build a cell for the given selector and text
    /// </summary>
    /// <param name="labelSelector">Property selector to get resource key annotation</param>
    /// <param name="text">Text</param>
    /// <returns>A cell for PDF table</returns>
    protected virtual PdfPCell BuildTextCell<TLabel>(Expression<Func<TLabel, string>> labelSelector, string text)
    {
        ArgumentNullException.ThrowIfNull(labelSelector);
        ArgumentNullException.ThrowIfNullOrEmpty(text);

        var label = LabelField(labelSelector, Font, Language);

        var content = new Phrase() { label, new Chunk(":", Font), new Chunk(" ", Font), new Chunk(text, Font) };
        var cell = new PdfPCell(content)
        {
            HorizontalAlignment = Element.ALIGN_LEFT,
            RunDirection = DocumentRunDirection,
            Border = 0,
            Padding = 3
        };

        cell.SetLeading(0f, PdfDocumentHelper.RELATIVE_LEADING);

        return cell;
    }

    /// <summary>
    /// Build a table for address item
    /// </summary>
    /// <param name="labelSelector">Property selector to get resource key annotation</param>
    /// <param name="address">Address item</param>
    /// <returns>PDF table</returns>
    protected virtual PdfGrid BuildAddressTable<TLabel>(Expression<Func<TLabel, AddressItem>> labelSelector, AddressItem address)
    {
        ArgumentNullException.ThrowIfNull(address);

        var addressTable = PdfDocumentHelper.BuildPdfGrid(numColumns: 1, DocumentRunDirection);

        var fontBold = PdfDocumentHelper.GetFont(Font, Font.Size, DocumentFontStyle.Bold);
        var label = LabelField(labelSelector, fontBold, Language);

        var captionCell = new PdfPCell()
        {
            HorizontalAlignment = Element.ALIGN_LEFT,
            RunDirection = DocumentRunDirection,
            Border = 0
        };

        captionCell.AddElement(new Paragraph(label) { Alignment = Element.ALIGN_LEFT });
        captionCell.AddElement(new LineSeparator(2f, 100f, BaseColor.LightGray, Element.ALIGN_LEFT, -4));

        addressTable.AddCell(captionCell);

        if (!string.IsNullOrEmpty(address?.Company))
            addressTable.AddCell(BuildTextCell<AddressItem>(address => address.Company, address.Company));

        if (!string.IsNullOrEmpty(address?.Name))
            addressTable.AddCell(BuildTextCell<AddressItem>(address => address.Name, address?.Name));

        if (!string.IsNullOrEmpty(address?.Phone))
            addressTable.AddCell(BuildTextCell<AddressItem>(address => address.Phone, address?.Phone));

        if (!string.IsNullOrEmpty(address?.AddressLine))
            addressTable.AddCell(BuildTextCell<AddressItem>(address => address.AddressLine, address?.AddressLine));

        if (!string.IsNullOrEmpty(address?.VATNumber))
            addressTable.AddCell(BuildTextCell<AddressItem>(address => address.VATNumber, address?.VATNumber));

        if (!string.IsNullOrEmpty(address?.PaymentMethod))
            addressTable.AddCell(BuildTextCell<AddressItem>(address => address.PaymentMethod, address?.PaymentMethod));

        if (!string.IsNullOrEmpty(address?.ShippingMethod))
            addressTable.AddCell(BuildTextCell<AddressItem>(address => address.ShippingMethod, address?.ShippingMethod));

        if (address?.CustomValues.Any() == true)
        {
            foreach (var (key, value) in address.CustomValues)
            {
                addressTable.AddCell(new PdfPCell(new Phrase { new Chunk(key), new Chunk(":"), new Chunk(value) }));
            }
        }

        return addressTable;
    }

    /// <summary>
    /// Specify default behavior for maintable column
    /// </summary>
    /// <param name="column">Column builder</param>
    /// <param name="propertyExpression">Property selector for cells in the column</param>
    /// <param name="width">The column's width according to the PdfRptPageSetup.MainTableColumnsWidthsType value</param>
    /// <param name="printProductAttributes">Indicates that product attribute descriptions should be printed if they exist</param>
    protected virtual void ConfigureProductColumn(ColumnAttributesBuilder column, Expression<Func<TItem, object>> propertyExpression, int width = 1, bool printProductAttributes = false)
    {
        column.PropertyName(propertyExpression);
        column.CellsHorizontalAlignment(HorizontalAlignment.Left);
        column.IsVisible(true);
        column.Width(width);
        column.HeaderCell(LabelField(propertyExpression, Font, Language).Content, horizontalAlignment: HorizontalAlignment.Left);

        column.ColumnItemsTemplate(itemsTemplate =>
        {
            itemsTemplate.InlineField(inlineField =>
            {
                inlineField.RenderCell(cellData =>
                {
                    var table = new PdfGrid(numColumns: 1)
                    {
                        WidthPercentage = 100,
                        RunDirection = DocumentRunDirection,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        SpacingAfter = 5,
                        SpacingBefore = 5
                    };

                    var data = cellData.Attributes.RowData.TableRowData;
                    var text = data.GetSafeStringValueOf(propertyExpression);

                    table.AddCell(BuildPdfPCell(text, verticalAlignment: Element.ALIGN_TOP));

                    if (printProductAttributes)
                    {
                        var productAttributes = (List<string>)data.GetValueOf((ProductItem x) => x.ProductAttributes);
                        var font8Italic = PdfDocumentHelper.GetFont(Font, Font.Size * 0.8f, DocumentFontStyle.Italic);

                        foreach (var pa in productAttributes)
                        {
                            table.AddCell(new PdfPCell(new Phrase(pa, font8Italic))
                            {
                                RunDirection = DocumentRunDirection,
                                HorizontalAlignment = Element.ALIGN_LEFT,
                                Border = 0
                            });
                        }
                    }

                    return new PdfPCell(table)
                    {
                        RunDirection = DocumentRunDirection,
                        BorderWidthBottom = 2,
                        BorderColorBottom = BaseColor.LightGray,
                        MinimumHeight = 25,
                        VerticalAlignment = Element.ALIGN_CENTER
                    };
                });
            });
        });
    }

    /// <summary>
    /// Get default document builder
    /// </summary>
    /// <returns>PDF document builder</returns>
    protected virtual PdfReport DefaultDocument()
    {
        return new PdfReport()
            .DocumentPreferences(doc =>
            {
                doc.RunDirection(Language.Rtl ? PdfRunDirection.RightToLeft : PdfRunDirection.LeftToRight);
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PageSize);
            })
            .MainTableEvents(events =>
            {
                events.CellCreated(args =>
                {
                    if (args.CellType == CellType.HeaderCell && !string.IsNullOrWhiteSpace(args.Cell.RowData.Value?.ToString()))
                    {
                        args.Cell.BasicProperties.BackgroundColor = BaseColor.LightGray;
                        args.Cell.BasicProperties.CellPadding = 5;
                    }
                });
            });
    }

    /// <summary>
    /// Get a label for the given property
    /// </summary>
    /// <param name="propertyExpression">Property selector to get resource key annotation</param>
    /// <param name="font">Font</param>
    /// <param name="language">Language</param>
    /// <param name="args">Array of objects to format the resource string</param>
    /// <returns>A chunk with localized annotation if present, otherwise an empty chunk</returns>
    protected virtual Chunk LabelField<TLabel, TOut>(Expression<Func<TLabel, TOut>> propertyExpression, Font font, Language language, params string[] args)
    {
        var expression = (MemberExpression)propertyExpression.Body;
        var propertyInfo = (PropertyInfo)expression.Member;

        var label = propertyInfo.GetCustomAttributes<DisplayNameAttribute>(true).FirstOrDefault() is DisplayNameAttribute attr
            ? GetResourceAsync(attr.DisplayName, language?.Id ?? 0).Result
            : string.Empty;

        if (!string.IsNullOrEmpty(label) && args.Any())
            label = string.Format(label, args);

        return new Chunk(label, font);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate document
    /// </summary>
    /// <param name="pdfStreamOutput">Stream for PDF output</param>
    public abstract void Generate(Stream pdfStreamOutput);

    #endregion

    #region Properties

    /// <summary>
    /// PDF document builder 
    /// </summary>
    protected PdfReport Document => DefaultDocument();

    /// <summary>
    /// Gets or sets a collection of items
    /// </summary>
    public List<TItem> Products { get; init; }

    /// <summary>
    /// Gets or sets the language context
    /// </summary>
    public required Language Language { get; init; }

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public required PdfPageSize PageSize { get; init; }

    /// <summary>
    /// Gets or sets the font name. Loaded from the ~/App_Data/Pdf directory during application start.
    /// </summary>
    public required Font Font { get; init; }

    /// <summary>
    /// Gets or sets the size required to scale images before rendering
    /// </summary>
    public required int ImageTargetSize { get; init; }

    /// <summary>
    /// Gets document run direction
    /// </summary>
    public int DocumentRunDirection => Language?.Rtl == true ? PdfWriter.RUN_DIRECTION_RTL : PdfWriter.RUN_DIRECTION_LTR;

    /// <summary>
    /// Gets or sets a function to get a resource string by the specified key and language identifier
    /// </summary>
    public required Func<string, int, Task<string>> GetResourceAsync { get; init; }

    #endregion
}