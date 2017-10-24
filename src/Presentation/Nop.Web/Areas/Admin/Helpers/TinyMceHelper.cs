using System.IO;
using Microsoft.AspNetCore.Hosting;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web.Areas.Admin.Helpers
{
    /// <summary>
    /// TinyMCE helper
    /// </summary>
    public static class TinyMceHelper
    {
        /// <summary>
        /// Get tinyMCE language name for current language 
        /// </summary>
        /// <returns>tinyMCE language name</returns>
        public static string GetTinyMceLanguage()
        {
            //nopCommerce supports TinyMCE's localization for 10 languages:
            //Chinese, Spanish, Arabic, Portuguese, Russian, German, French, Italian, Dutch and English out-of-the-box.
            //Additional languages can be downloaded from the website TinyMCE(https://www.tinymce.com/download/language-packages/)

            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var hostingEnvironment = EngineContext.Current.Resolve<IHostingEnvironment>();

            var languageCulture = workContext.WorkingLanguage.LanguageCulture;

            var langFile = $"{languageCulture}.js";
            var directoryPath = Path.Combine(hostingEnvironment.WebRootPath, @"lib\tinymce\langs");
            var fileExists = File.Exists($"{directoryPath}\\{langFile}");

            if (!fileExists)
            {
                languageCulture = languageCulture.Replace('-', '_');
                langFile = $"{languageCulture}.js";
                fileExists = File.Exists($"{directoryPath}\\{langFile}");
            }

            if (!fileExists)
            {
                languageCulture = languageCulture.Split('_', '-')[0];
                langFile = $"{languageCulture}.js";
                fileExists = File.Exists($"{directoryPath}\\{langFile}");
            }

            return fileExists ? languageCulture : string.Empty;
        }
    }
}