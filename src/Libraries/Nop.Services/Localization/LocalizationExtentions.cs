//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using Nop.Core.Domain.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System;
using System.Reflection;
using Nop.Core;

namespace Nop.Services.Localization
{
    public static class LocalizationExtentions
    {
        public static string GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector)
            where T : BaseEntity, ILocalizedEntity
        {
            return GetLocalized(entity, keySelector, true);
        }

        public static string GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector,
            bool returnDefaultValue) where T : BaseEntity, ILocalizedEntity
        {
            //TODO: set language id (IWorkingContext)
            int languageId = 1;
            return GetLocalized(entity, keySelector, returnDefaultValue, languageId);
        }

        public static string GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector,
            bool returnDefaultValue, 
            int languageId) where T : BaseEntity, ILocalizedEntity
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

            //TODO use DependencyResolver to load service
            //var leService = new LocalizedEntityService();
            //var props = leService.GetLocalizedProperties(entity.Id, localeKeyGroup);
            //var prop = props.FirstOrDefault(lp => lp.LanguageId == languageId &&
            //    lp.LocaleKeyGroup == localeKeyGroup &&
            //    lp.LocaleKey == localeKey);
            //if (prop != null)
            //    result = prop.LocaleValue;

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
