using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Extensions
{
    public static partial class MigrationExtension
    {
        /// <summary>
        /// Get language data
        /// </summary>
        /// <returns>Default language identifier</returns>
        public static (int? defaultLanguageId, IList<Language> allLanguages) GetLanguageData(this IMigration _, ILanguageService languageService=null)
        {
            languageService ??= EngineContext.Current.Resolve<ILanguageService>();

            var languages = languageService.GetAllLanguages(true);
            var languageId = languages
                .FirstOrDefault(lang => lang.UniqueSeoCode == new CultureInfo(NopCommonDefaults.DefaultLanguageCulture).TwoLetterISOLanguageName)?.Id;

            return (languageId, languages);
        }
    }
}
