using System.Net;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;

namespace Nop.Services.Localization;

/// <summary>
/// Represents extensions for localized URLs
/// </summary>
public static class LocalizedUrlExtensions
{
    private static readonly char[] _separator = ['/', '?'];

    /// <summary>
    /// Get a value indicating whether URL is localized (contains SEO code)
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="pathBase">Application path base</param>
    /// <param name="isRawPath">A value indicating whether passed URL is raw URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if passed URL contains SEO code; otherwise false. Language whose SEO code is in the URL if URL is localized
    /// </returns>
    public static async Task<(bool IsLocalized, Language Language)> IsLocalizedUrlAsync(this string url, PathString pathBase, bool isRawPath)
    {
        if (string.IsNullOrEmpty(url))
            return (false, null);

        //remove application path from raw URL
        if (isRawPath)
            url = url.RemoveApplicationPathFromRawUrl(pathBase);

        //get first segment of passed URL
        var firstSegment = url.Split(_separator, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrEmpty(firstSegment))
            return (false, null);

        //suppose that the first segment is the language code and try to get language
        var languageService = EngineContext.Current.Resolve<ILanguageService>();
        var language = (await languageService.GetAllLanguagesAsync())
            .FirstOrDefault(urlLanguage => urlLanguage.UniqueSeoCode.Equals(firstSegment, StringComparison.InvariantCultureIgnoreCase));

        //if language exists and published passed URL is localized
        return (language?.Published ?? false, language);
    }

    /// <summary>
    /// Remove application path from raw URL
    /// </summary>
    /// <param name="rawUrl">Raw URL</param>
    /// <param name="pathBase">Application path base</param>
    /// <returns>Result</returns>
    public static string RemoveApplicationPathFromRawUrl(this string rawUrl, PathString pathBase)
    {
        new PathString(rawUrl).StartsWithSegments(pathBase, out var result);
        return WebUtility.UrlDecode(result);
    }

    /// <summary>
    /// Remove language SEO code from URL
    /// </summary>
    /// <param name="url">Raw URL</param>
    /// <param name="pathBase">Application path base</param>
    /// <param name="isRawPath">A value indicating whether passed URL is raw URL</param>
    /// <returns>URL without language SEO code</returns>
    public static string RemoveLanguageSeoCodeFromUrl(this string url, PathString pathBase, bool isRawPath)
    {
        if (string.IsNullOrEmpty(url))
            return url;

        //remove application path from raw URL
        if (isRawPath)
            url = url.RemoveApplicationPathFromRawUrl(pathBase);

        //get result URL
        url = url.TrimStart('/');
        var result = url.Contains('/') ? url[(url.IndexOf('/'))..] : string.Empty;

        //and add back application path to raw URL
        if (isRawPath)
            result = pathBase + result;

        return result;
    }

    /// <summary>
    /// Add language SEO code to URL
    /// </summary>
    /// <param name="url">Raw URL</param>
    /// <param name="pathBase">Application path base</param>
    /// <param name="isRawPath">A value indicating whether passed URL is raw URL</param>
    /// <param name="language">Language</param>
    /// <returns>Result</returns>
    public static string AddLanguageSeoCodeToUrl(this string url, PathString pathBase, bool isRawPath, Language language)
    {
        ArgumentNullException.ThrowIfNull(language);

        //null validation is not required
        //if (string.IsNullOrEmpty(url))
        //    return url;

        //remove application path from raw URL
        if (isRawPath && !string.IsNullOrEmpty(url))
            url = url.RemoveApplicationPathFromRawUrl(pathBase);

        //add language code
        url = $"/{language.UniqueSeoCode}{url}";

        //and add back application path to raw URL
        if (isRawPath)
            url = pathBase + url;

        return url;
    }
}