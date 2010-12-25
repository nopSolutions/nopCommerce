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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Localization;

namespace Nop.Services
{
    public partial class DefaultPropertyLocalizer<From, To> : IPropertyLocalizer<From, To>
        where From : BaseEntity
        where To : LocalizedBaseEntity<From>
    {
        private readonly ILocalizedEntityService _leService;
        private readonly BaseEntity _entity;

        public DefaultPropertyLocalizer(ILocalizedEntityService leService, From entity)
        {
            this._leService = leService;
            this._entity = entity;
        }

        protected To GetLocalizedEntity(bool setDefaultValueIfNotFound, int languageId)
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
            localizedEntity.EntityId = _entity.Id;
            return localizedEntity;
        }

        public void Localize()
        {
            if (_entity == null)
                return;

            //load a property with localized entity
            var resultProperty = (from p in typeof(From).GetProperties()
                        where p.CanRead && p.CanWrite
                        && p.GetCustomAttributes(typeof(LocalizedEntityResultAttribute), false).Any()
                        select p).FirstOrDefault();
            if (resultProperty == null)
                return;

            //set it to null
            resultProperty.SetValue(_entity, null, null);
            
            //TODO set current language identifier here
            int languageId = 1;

            //get localized entity
            var localizedEntity = GetLocalizedEntity(true, languageId);

            //save new localized entity
            resultProperty.SetValue(_entity, localizedEntity, null);
        }
        
        public List<LocalizedProperty> GetLozalizedProperties(LocalizedBaseEntity<From> localizedEntity)
        {
            if (localizedEntity == null)
                throw new ArgumentNullException("localizedEntity");

            if (localizedEntity.LanguageId == 0)
                throw new ArgumentException("Language ID should not be 0");

            var result = new List<LocalizedProperty>();

            //load localized properties
            var leAttributes = typeof(From).GetCustomAttributes(typeof(LocalizableEntityAttribute), false);
            if (leAttributes == null || leAttributes.Length == 0)
                return result;
            string localeKeyGroup = ((LocalizableEntityAttribute)leAttributes[0]).LocaleKeyGroup;
            if (String.IsNullOrEmpty(localeKeyGroup))
                return result;

            //load appropriate properties
            var propsFrom = from p in typeof(From).GetProperties()
                            where p.CanRead && p.CanWrite
                            && p.GetCustomAttributes(typeof(LocalizablePropertyAttribute), false).Any()
                            select p;

            var propsTo = from p in typeof(To).GetProperties()
                          where p.CanRead && p.CanWrite
                          select p;
            
            //UNDONE validate whether we have more than one active language
            var existingLocalizedProperties = _leService.GetLocalizedProperties(localizedEntity.EntityId, localizedEntity.LanguageId, localeKeyGroup);
            foreach (var pFrom in propsFrom)
            {
                var lpAttribute = pFrom.GetCustomAttributes(typeof(LocalizablePropertyAttribute), false).SingleOrDefault() as LocalizablePropertyAttribute;
                if (lpAttribute != null)
                {
                    foreach (var pTo in propsTo)
                    {
                        if (pFrom.Name.Equals(pTo.Name))
                        {
                            if (existingLocalizedProperties.ContainsKey(lpAttribute.LocaleKey))
                            {
                                result.Add(existingLocalizedProperties[lpAttribute.LocaleKey]);
                            }
                            else
                            {
                                result.Add(new LocalizedProperty()
                                {
                                    EntityId = localizedEntity.EntityId,
                                    LanguageId = localizedEntity.LanguageId,
                                    LocaleKeyGroup = localeKeyGroup,
                                    LocaleKey = lpAttribute.LocaleKey,
                                    LocaleValue = (string)pTo.GetValue(localizedEntity, null)
                                });
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}