
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;

namespace Nop.Services.Localization
{
    public static class LocalizationExtentions
    {
        public static string GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector)
            where T : BaseEntity, ILocalizedEntity
        {
            //int languageId = ServiceLocator.Current.GetInstance<IWorkContext>().WorkingLanguage.Id;
            int languageId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
            return GetLocalized(entity, keySelector, languageId);
        }

        public static string GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector,
            int languageId,
            bool returnDefaultValue = true) where T : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            string result = string.Empty;

            //load localized value
            string localeKeyGroup = typeof(T).Name;
            string localeKey = propInfo.Name;


            //var leService = ServiceLocator.Current.GetInstance<ILocalizedEntityService>();
            var leService = EngineContext.Current.Resolve<ILocalizedEntityService>();
            var props = leService.GetLocalizedProperties(entity.Id, localeKeyGroup);
            var prop = props.FirstOrDefault(lp => lp.LanguageId == languageId &&
                lp.LocaleKeyGroup == localeKeyGroup &&
                lp.LocaleKey == localeKey);
            if (prop != null)
                result = prop.LocaleValue;

            //set default value if required
            if (String.IsNullOrEmpty(result) && returnDefaultValue)
            {
                var localizer = keySelector.Compile();
                result = localizer(entity);
            }

            return result;
        }
    }
}
