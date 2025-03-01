using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.Core.Contracts;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents PDF document helper
/// </summary>
public static partial class PdfDocumentHelper
{
    #region Constants

    public const float RELATIVE_LEADING = 1.3f;

    #endregion

    #region Methods

    /// <summary>
    /// Build a cell with the given table
    /// </summary>
    /// <param name="table">PDF table</param>
    /// <param name="runDirection">Preferential run direction</param>
    /// <param name="collSpan">The number of columns occupied by a cell</param>
    /// <param name="horizontalAlign">Horizontal alignment</param>
    /// <returns>A cell for PDF table</returns>
    public static PdfPCell BuildPdfPCell(PdfGrid table, int runDirection, int collSpan = 1, int horizontalAlign = Element.ALIGN_CENTER)
    {
        var cell = new PdfPCell(table)
        {
            RunDirection = runDirection,
            Colspan = collSpan,
            HorizontalAlignment = horizontalAlign,
            Border = 0
        };

        cell.SetLeading(0f, RELATIVE_LEADING);

        return cell;
    }

    /// <summary>
    /// Build default PDF table
    /// </summary>
    /// <param name="numColumns">The number of columns</param>
    /// <param name="runDirection">Preferential run direction</param>
    /// <returns>A PDF table</returns>
    public static PdfGrid BuildPdfGrid(int numColumns, int runDirection)
    {
        return new PdfGrid(numColumns: numColumns)
        {
            WidthPercentage = 100,
            RunDirection = runDirection,
            HorizontalAlignment = Element.ALIGN_LEFT,
            SpacingAfter = 10
        };
    }

    /// <summary>
    /// Get a font
    /// </summary>
    /// <param name="fontName">The name of the font</param>
    /// <param name="size">The size of this font</param>
    /// <param name="style">The style of this font</param>
    /// <returns>A font object</returns>
    public static Font GetFont(string fontName, float size, DocumentFontStyle style = DocumentFontStyle.Normal)
    {
        return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, true, size, (int)style, BaseColor.Black);
    }

    /// <summary>
    /// Get a font
    /// </summary>
    /// <param name="font">Font to calculate BaseFont</param>
    /// <param name="size">The size of this font</param>
    /// <param name="style">The style of this font</param>
    /// <returns>A font object</returns>
    public static Font GetFont(Font font, float size, DocumentFontStyle style = DocumentFontStyle.Normal)
    {
        return new Font(font.GetCalculatedBaseFont(false), size, (int)style, font.Color);
    }

    #endregion
}
