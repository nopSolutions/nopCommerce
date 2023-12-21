using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web.Areas.Admin.Helpers;

/// <summary>
/// TinyMCE helper
/// </summary>
public partial class TinyMceHelper : ITinyMceHelper
{
    protected readonly INopFileProvider _nopFileProvider;
    protected readonly IWebHostEnvironment _webHostEnvironment;
    protected readonly IWorkContext _workContext;

    public TinyMceHelper(INopFileProvider nopFileProvider, IWebHostEnvironment webHostEnvironment, IWorkContext workContext)
    {
        _nopFileProvider = nopFileProvider;
        _webHostEnvironment = webHostEnvironment;
        _workContext = workContext;
    }

    /// <summary>
    /// Get tinyMCE language name for current language 
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the inyMCE language name
    /// </returns>
    public async Task<string> GetTinyMceLanguageAsync()
    {
        //nopCommerce supports TinyMCE's localization for 10 languages:
        //Chinese, Spanish, Arabic, Portuguese, Russian, German, French, Italian, Dutch and English out-of-the-box.
        //Additional languages can be downloaded from the website TinyMCE(https://www.tinymce.com/download/language-packages/)

        var languageCulture = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;

        var langFile = $"{languageCulture}.js";
        var directoryPath = _nopFileProvider.Combine(_webHostEnvironment.WebRootPath, @"lib_npm\tinymce\langs");
        var fileExists = _nopFileProvider.FileExists($"{directoryPath}\\{langFile}");

        if (!fileExists)
        {
            languageCulture = languageCulture.Replace('-', '_');
            langFile = $"{languageCulture}.js";
            fileExists = _nopFileProvider.FileExists($"{directoryPath}\\{langFile}");
        }

        if (!fileExists)
        {
            languageCulture = languageCulture.Split('_', '-')[0];
            langFile = $"{languageCulture}.js";
            fileExists = _nopFileProvider.FileExists($"{directoryPath}\\{langFile}");
        }

        return fileExists ? languageCulture : string.Empty;
    }
}