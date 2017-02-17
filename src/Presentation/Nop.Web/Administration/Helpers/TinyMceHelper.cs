using System.IO;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Admin.Helpers
{
    /// <summary>
    /// TinyMCE helper
    /// </summary>
    public static class TinyMceHelper
    {
        /// <summary>
        /// Get tinyMCE language name for curent language 
        /// </summary>
        /// <returns>tinyMCE language name</returns>
        public static string GetTinyMceLanguage()
        {
            //nopCommerce supports TinyMCE's localization for 10 languages:
            //Chinese, Spanish, Arabic, Portuguese, Russian, German, French, Italian, Dutch and English out-of-the-box.
            //Additional languages can be downloaded from the website TinyMCE(https://www.tinymce.com/download/language-packages/)

            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var languageCulture = workContext.WorkingLanguage.LanguageCulture;

            var langFile = string.Format("{0}.js", languageCulture);
            var path = CommonHelper.MapPath("~/Administration/Content/tinymce/langs/");
            var fileExists = File.Exists(string.Format("{0}{1}", path, langFile));

            if (!fileExists)
            {
                languageCulture = languageCulture.Replace('-', '_');
                langFile = string.Format("{0}.js", languageCulture);
                fileExists = File.Exists(string.Format("{0}{1}", path, langFile));
            }

            if (!fileExists)
            {
                languageCulture = languageCulture.Replace('-', '_');
                langFile = string.Format("{0}.js", languageCulture);
                fileExists = File.Exists(string.Format("{0}{1}", path, langFile));
            }

            if (!fileExists)
            {
                languageCulture = languageCulture.Split('_', '-')[0];
                langFile = string.Format("{0}.js", languageCulture);
                fileExists = File.Exists(string.Format("{0}{1}", path, langFile));
            }

            return fileExists ? languageCulture : string.Empty;
        }
    }
}