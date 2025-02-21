using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using PdfRpt.Core.Contracts;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Extensions
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
    /// <param name="language">Language</param>
    /// <param name="collSpan">The number of columns occupied by a cell</param>
    /// <param name="horizontalAlign">Horizontal alignment</param>
    /// <returns>A cell for PDF table</returns>
    public static PdfPCell BuildPdfPCell(PdfGrid table, Language language, int collSpan = 1, int horizontalAlign = Element.ALIGN_CENTER)
    {
        var cell = new PdfPCell(table)
        {
            RunDirection = language.GetPdfRunDirection(),
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
    /// <param name="language">Language</param>
    /// <returns>A PDF table</returns>
    public static PdfGrid BuildPdfGrid(int numColumns, Language language)
    {
        return new PdfGrid(numColumns: numColumns)
        {
            WidthPercentage = 100,
            RunDirection = language.GetPdfRunDirection(),
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

    /// <summary>
    /// Get a label for the given property
    /// </summary>
    /// <param name="propertyExpression">Property selector to get resource key annotation</param>
    /// <param name="font">Font</param>
    /// <param name="languageId">Language</param>
    /// <param name="args">Array of objects to format the resource string</param>
    /// <returns>A chunk with localized annotation if present, otherwise an empty chunk</returns>
    public static Chunk LabelField<TLabel, TOut>(Expression<Func<TLabel, TOut>> propertyExpression, Font font, Language language, params string[] args)
    {
        var expression = (MemberExpression)propertyExpression.Body;
        var propertyInfo = (PropertyInfo)expression.Member;
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        var label = propertyInfo
            .GetCustomAttributes<DisplayNameAttribute>(true)
            .FirstOrDefault() is DisplayNameAttribute attr ?
            localizationService.GetResourceAsync(attr.DisplayName, language?.Id ?? 0).Result : string.Empty;

        if (!string.IsNullOrEmpty(label) && args.Any())
            label = string.Format(label, args);

        return new Chunk(label, font);
    }

    #endregion
}
