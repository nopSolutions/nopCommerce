using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Web.Areas.Admin.Infrastructure.Cache;

namespace Nop.Web.Areas.Admin.Helpers;

/// <summary>
/// Summernote helper
/// </summary>
public partial class SummernoteHelper : ISummernoteHelper
{
    protected readonly INopFileProvider _nopFileProvider;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IWorkContext _workContext;

    public SummernoteHelper(
        INopFileProvider nopFileProvider, 
        IStaticCacheManager staticCacheManager, 
        IWorkContext workContext)
    {
        _nopFileProvider = nopFileProvider;
        _staticCacheManager = staticCacheManager;
        _workContext = workContext;
    }

    /// <summary>
    /// Get Summernote language name for current language 
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the Summernote language name
    /// </returns>
    public async Task<string> GetRichEditorLanguageAsync()
    {
        var languageCulture = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.SummernoteLanguageKey,
            languageCulture);

        return await _staticCacheManager.GetAsync(cacheKey, () =>
        {
            var langFile = $"summernote-{languageCulture}.min.js";
            var directoryPath = _nopFileProvider.GetAbsolutePath(@"lib_npm\summernote\lang");
            var fileExists = _nopFileProvider.FileExists($"{directoryPath}\\{langFile}");

            return fileExists ? languageCulture : string.Empty;
        });
    }
}
