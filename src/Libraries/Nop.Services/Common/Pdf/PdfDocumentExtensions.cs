using iTextSharp.text;
using iTextSharp.text.pdf;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Extensions
/// </summary>
public static class PdfDocumentExtensions
{
    /// <summary>
    /// Get document run direction
    /// </summary>
    /// <param name="language"></param>
    /// <returns>Run direction</returns>
    public static int GetPdfRunDirection(this Language language)
    {
        return language?.Rtl == true ? PdfWriter.RUN_DIRECTION_RTL : PdfWriter.RUN_DIRECTION_LTR;
    }

    /// <summary>
    /// Resolve font for PDF document
    /// </summary>
    /// <param name="language">Language</param>
    /// <param name="settings">PDF settings</param>
    /// <returns>A font object</returns>
    public static Font ResolvePdfFont(this Language language, PdfSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var fontName = language?.Rtl == true
            ? !string.IsNullOrEmpty(settings.RtlFontName) ? settings.RtlFontName : NopCommonDefaults.PdfRtlFontName
            : !string.IsNullOrEmpty(settings.LtrFontName) ? settings.LtrFontName : NopCommonDefaults.PdfLtrFontName;

        var fontSize = settings.BaseFontSize >= 0 ? settings.BaseFontSize : 10;

        return PdfDocumentHelper.GetFont(fontName, fontSize);
    }
}