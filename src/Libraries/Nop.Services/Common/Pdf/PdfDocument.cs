using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Nop.Services.Localization;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents base document class
/// </summary>
/// <typeparam name="T">Type of the data source</typeparam>
public abstract class PdfDocument<T> : IDocument where T : DocumentSource
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected T Source { get; init; }

    #endregion

    #region Ctor

    protected PdfDocument(T source, ILocalizationService localizationService)
    {
        ArgumentNullException.ThrowIfNull(localizationService);
        ArgumentNullException.ThrowIfNull(source);

        _localizationService = localizationService;
        Source = source;
    }

    #endregion

    #region Methods

    public TextDescriptor ComposeLabel<TModel>(TextDescriptor text, Expression<Func<TModel, object>> propertyExpression)
    {
        var expression = (MemberExpression)propertyExpression.Body;
        var propertyInfo = (PropertyInfo)expression.Member;

        var label = propertyInfo
            .GetCustomAttributes<DisplayNameAttribute>(true)
            .FirstOrDefault() is DisplayNameAttribute attr ?
            _localizationService.GetResourceAsync(attr.DisplayName, Source.Language?.Id ?? 0).Result : string.Empty;

        if (!string.IsNullOrEmpty(label))
            text.Span($"{label} ");

        return text;
    }

    public TextDescriptor ComposeField<TModel, TOut>(
        TextDescriptor text, TModel source,
        Expression<Func<TModel, TOut>> propertyExpression,
        Func<TOut, string> format = null, string delimiter = " ")
    {
        var expression = (MemberExpression)propertyExpression.Body;
        var propertyInfo = (PropertyInfo)expression.Member;

        var label = propertyInfo
            .GetCustomAttributes<DisplayNameAttribute>(true)
            .FirstOrDefault() is DisplayNameAttribute attr ?
            _localizationService.GetResourceAsync(attr.DisplayName, Source.Language?.Id ?? 0).Result : string.Empty;

        if (source is null)
            return text;

        var value = propertyInfo.GetValue(source);

        var formattedValue = format == null ? value?.ToString() : value switch
        {
            TOut f => format(f),
            var f => f?.ToString()
        };

        if (string.IsNullOrEmpty(formattedValue))
            return text;

        if (!string.IsNullOrEmpty(label))
        {
            text.Span(label);
            var delimiterSpan = text.Span(delimiter);

            if (Source.IsRightToLeft)
            {
                delimiterSpan.DirectionFromRightToLeft();
            }
        }

        text.Span(formattedValue);

        return text;
    }

    public abstract void Compose(IDocumentContainer container);

    public DocumentMetadata GetMetadata()
    {
        return DocumentMetadata.Default;
    }

    #endregion

    public TextStyle DefaultStyle => TextStyle.Default.FontFamily(Source.FontFamily);
}