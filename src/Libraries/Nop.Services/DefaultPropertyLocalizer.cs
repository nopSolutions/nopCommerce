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

using System;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Localization;

namespace Nop.Services
{
    public partial class DefaultPropertyLocalizer<From, To> : IPropertyLocalizer<From, To>
        where From : BaseEntity
        where To : LocalizedBaseEntity
    {
        protected ILocalizedEntityService _leService;
        protected BaseEntity _entity;

        public DefaultPropertyLocalizer(ILocalizedEntityService leService, From entity)
        {
            this._leService = leService;
            this._entity = entity;
        }

        public To GetLocalizedEntity(bool setDefaultValueIfNotFound, int languageId)
        {
            if (_entity == null)
                return null;

            //load localized properties
            var leAttributes = typeof(From).GetCustomAttributes(typeof(LocalizableEntityAttribute), false);
            if (leAttributes == null || leAttributes.Length == 0)
                return null;
            string localeKeyGroup = ((LocalizableEntityAttribute)leAttributes[0]).LocaleKeyGroup;
            if (String.IsNullOrEmpty(localeKeyGroup))
                return null;

            if (languageId == 0)
                return null;

            //load appropriate properties
            var propsFrom = from p in typeof(From).GetProperties()
                            where p.CanRead && p.CanWrite
                            && p.GetCustomAttributes(typeof(LocalizablePropertyAttribute), false).Any()
                            select p;

            var propsTo = from p in typeof(To).GetProperties()
                          where p.CanRead && p.CanWrite
                          select p;

            //create localized entity
            var localizedEntity = Activator.CreateInstance<To>();

            //UNDONE validate whether we have more than one active language
            var localizedProperties = _leService.GetLocalizedProperties(_entity.Id, languageId, localeKeyGroup);
            foreach (var pFrom in propsFrom)
            {
                var lpAttribute = pFrom.GetCustomAttributes(typeof(LocalizablePropertyAttribute), false).SingleOrDefault() as LocalizablePropertyAttribute;
                if (lpAttribute != null)
                {
                    foreach (var pTo in propsTo)
                    {
                        if (pFrom.Name.Equals(pTo.Name))
                        {
                            //UNDONE get localized property from storage
                            string localizedValue = "";
                            if (localizedProperties.ContainsKey(lpAttribute.LocaleKey))
                                localizedValue = localizedProperties[lpAttribute.LocaleKey].LocaleValue;

                            if (String.IsNullOrEmpty(localizedValue))
                            {
                                if (setDefaultValueIfNotFound)
                                {
                                    //set default value
                                    localizedValue = (string)pFrom.GetValue(_entity, null);
                                }
                            }
                            pTo.SetValue(localizedEntity, localizedValue, null);
                        }
                    }
                }
            }
            localizedEntity.LanguageId = languageId;
            return localizedEntity;
        }

        public void Localize()
        {
            if (_entity == null)
                return;

            //TODO set current language identifier here
            int languageId = 1;

            //get localized entity
            var localizedEntity = GetLocalizedEntity(true, languageId);

            //save it
            var props = from p in typeof(From).GetProperties()
                        where p.CanRead && p.CanWrite
                        && p.GetCustomAttributes(typeof(LocalizedEntityResultAttribute), false).Any()
                        select p;
            var resultProperty = props.FirstOrDefault();
            if (resultProperty != null)
            {
                resultProperty.SetValue(_entity, localizedEntity, null);
            }
        }
    }
}