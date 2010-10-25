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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Products.Attributes
{
    /// <summary>
    /// Checkout attribute manager
    /// </summary>
    public partial class CheckoutAttributeManager : ICheckoutAttributeManager
    {
        #region Constants
        private const string CHECKOUTATTRIBUTES_ALL_KEY = "Nop.checkoutattribute.all-{0}";
        private const string CHECKOUTATTRIBUTES_BY_ID_KEY = "Nop.checkoutattribute.id-{0}";
        private const string CHECKOUTATTRIBUTEVALUES_ALL_KEY = "Nop.checkoutattributevalue.all-{0}";
        private const string CHECKOUTATTRIBUTEVALUES_BY_ID_KEY = "Nop.checkoutattributevalue.id-{0}";
        private const string CHECKOUTATTRIBUTES_PATTERN_KEY = "Nop.checkoutattribute.";
        private const string CHECKOUTATTRIBUTEVALUES_PATTERN_KEY = "Nop.checkoutattributevalue.";
        #endregion

        #region Methods

        #region Checkout attributes

        /// <summary>
        /// Deletes a checkout attribute
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        public void DeleteCheckoutAttribute(int checkoutAttributeId)
        {
            var checkoutAttribute = GetCheckoutAttributeById(checkoutAttributeId);
            if (checkoutAttribute == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(checkoutAttribute))
                context.CheckoutAttributes.Attach(checkoutAttribute);
            context.DeleteObject(checkoutAttribute);
            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all checkout attributes
        /// </summary>
        /// <param name="dontLoadShippableProductRequired">Value indicating whether to do not load attributes for checkout attibutes which require shippable products</param>
        /// <returns>Checkout attribute collection</returns>
        public List<CheckoutAttribute> GetAllCheckoutAttributes(bool dontLoadShippableProductRequired)
        {
            string key = string.Format(CHECKOUTATTRIBUTES_ALL_KEY, dontLoadShippableProductRequired);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<CheckoutAttribute>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ca in context.CheckoutAttributes
                        orderby ca.DisplayOrder
                        where !dontLoadShippableProductRequired || !ca.ShippableProductRequired
                        select ca;
            var checkoutAttributes = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, checkoutAttributes);
            }
            return checkoutAttributes;
        }

        /// <summary>
        /// Gets a checkout attribute 
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute</returns>
        public CheckoutAttribute GetCheckoutAttributeById(int checkoutAttributeId)
        {
            if (checkoutAttributeId == 0)
                return null;

            string key = string.Format(CHECKOUTATTRIBUTES_BY_ID_KEY, checkoutAttributeId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (CheckoutAttribute)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ca in context.CheckoutAttributes
                        where ca.CheckoutAttributeId == checkoutAttributeId
                        select ca;
            var checkoutAttribute = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, checkoutAttribute);
            }
            return checkoutAttribute;
        }

        /// <summary>
        /// Inserts a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        public void InsertCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException("checkoutAttribute");

            checkoutAttribute.Name = CommonHelper.EnsureNotNull(checkoutAttribute.Name);
            checkoutAttribute.Name = CommonHelper.EnsureMaximumLength(checkoutAttribute.Name, 100);
            checkoutAttribute.TextPrompt = CommonHelper.EnsureNotNull(checkoutAttribute.TextPrompt);
            checkoutAttribute.TextPrompt = CommonHelper.EnsureMaximumLength(checkoutAttribute.TextPrompt, 300);

            var context = ObjectContextHelper.CurrentObjectContext;

            context.CheckoutAttributes.AddObject(checkoutAttribute);
            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <param name="displayOrder">Display order</param>
        public void UpdateCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException("checkoutAttribute");

            checkoutAttribute.Name = CommonHelper.EnsureNotNull(checkoutAttribute.Name);
            checkoutAttribute.Name = CommonHelper.EnsureMaximumLength(checkoutAttribute.Name, 100);
            checkoutAttribute.TextPrompt = CommonHelper.EnsureNotNull(checkoutAttribute.TextPrompt);
            checkoutAttribute.TextPrompt = CommonHelper.EnsureMaximumLength(checkoutAttribute.TextPrompt, 300);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(checkoutAttribute))
                context.CheckoutAttributes.Attach(checkoutAttribute);

            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets localized checkout attribute by id
        /// </summary>
        /// <param name="checkoutAttributeLocalizedId">Localized checkout attribute identifier</param>
        /// <returns>Checkout attribute content</returns>
        public CheckoutAttributeLocalized GetCheckoutAttributeLocalizedById(int checkoutAttributeLocalizedId)
        {
            if (checkoutAttributeLocalizedId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cal in context.CheckoutAttributeLocalized
                        where cal.CheckoutAttributeLocalizedId == checkoutAttributeLocalizedId
                        select cal;
            var checkoutAttributeLocalized = query.SingleOrDefault();
            return checkoutAttributeLocalized;
        }

        /// <summary>
        /// Gets localized checkout attribute by category id
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute content</returns>
        public List<CheckoutAttributeLocalized> GetCheckoutAttributeLocalizedByCheckoutAttributeId(int checkoutAttributeId)
        {
            if (checkoutAttributeId == 0)
                return new List<CheckoutAttributeLocalized>();

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cal in context.CheckoutAttributeLocalized
                        where cal.CheckoutAttributeId == checkoutAttributeId
                        select cal;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets localized checkout attribute by checkout attribute id and language id
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Checkout attribute content</returns>
        public CheckoutAttributeLocalized GetCheckoutAttributeLocalizedByCheckoutAttributeIdAndLanguageId(int checkoutAttributeId, int languageId)
        {
            if (checkoutAttributeId == 0 || languageId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cal in context.CheckoutAttributeLocalized
                        orderby cal.CheckoutAttributeLocalizedId
                        where cal.CheckoutAttributeId == checkoutAttributeId &&
                        cal.LanguageId == languageId
                        select cal;
            var checkoutAttributeLocalized = query.FirstOrDefault();
            return checkoutAttributeLocalized;
        }

        /// <summary>
        /// Inserts a localized checkout attribute
        /// </summary>
        /// <param name="checkoutAttributeLocalized">Checkout attribute content</param>
        public void InsertCheckoutAttributeLocalized(CheckoutAttributeLocalized checkoutAttributeLocalized)
        {
            if (checkoutAttributeLocalized == null)
                throw new ArgumentNullException("checkoutAttributeLocalized");

            checkoutAttributeLocalized.Name = CommonHelper.EnsureNotNull(checkoutAttributeLocalized.Name);
            checkoutAttributeLocalized.Name = CommonHelper.EnsureMaximumLength(checkoutAttributeLocalized.Name, 100);
            checkoutAttributeLocalized.TextPrompt = CommonHelper.EnsureNotNull(checkoutAttributeLocalized.TextPrompt);
            checkoutAttributeLocalized.TextPrompt = CommonHelper.EnsureMaximumLength(checkoutAttributeLocalized.TextPrompt, 300);

            var context = ObjectContextHelper.CurrentObjectContext;

            context.CheckoutAttributeLocalized.AddObject(checkoutAttributeLocalized);
            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Update a localized checkout attribute
        /// </summary>
        /// <param name="checkoutAttributeLocalized">Checkout attribute content</param>
        public void UpdateCheckoutAttributeLocalized(CheckoutAttributeLocalized checkoutAttributeLocalized)
        {
            if (checkoutAttributeLocalized == null)
                throw new ArgumentNullException("checkoutAttributeLocalized");

            checkoutAttributeLocalized.Name = CommonHelper.EnsureNotNull(checkoutAttributeLocalized.Name);
            checkoutAttributeLocalized.Name = CommonHelper.EnsureMaximumLength(checkoutAttributeLocalized.Name, 100);
            checkoutAttributeLocalized.TextPrompt = CommonHelper.EnsureNotNull(checkoutAttributeLocalized.TextPrompt);
            checkoutAttributeLocalized.TextPrompt = CommonHelper.EnsureMaximumLength(checkoutAttributeLocalized.TextPrompt, 300);

            bool allFieldsAreEmpty = string.IsNullOrEmpty(checkoutAttributeLocalized.Name) &&
                string.IsNullOrEmpty(checkoutAttributeLocalized.TextPrompt);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(checkoutAttributeLocalized))
                context.CheckoutAttributeLocalized.Attach(checkoutAttributeLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                context.DeleteObject(checkoutAttributeLocalized);
                context.SaveChanges();
            }
            else
            {
                context.SaveChanges();
            }

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }
        
        #endregion

        #region Checkout variant attribute values

        /// <summary>
        /// Deletes a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        public void DeleteCheckoutAttributeValue(int checkoutAttributeValueId)
        {
            var checkoutAttributeValue = GetCheckoutAttributeValueById(checkoutAttributeValueId);
            if (checkoutAttributeValue == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(checkoutAttributeValue))
                context.CheckoutAttributeValues.Attach(checkoutAttributeValue);
            context.DeleteObject(checkoutAttributeValue);
            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets checkout attribute values by checkout attribute identifier
        /// </summary>
        /// <param name="checkoutAttributeId">The checkout attribute identifier</param>
        /// <returns>Checkout attribute value collection</returns>
        public List<CheckoutAttributeValue> GetCheckoutAttributeValues(int checkoutAttributeId)
        {
            string key = string.Format(CHECKOUTATTRIBUTEVALUES_ALL_KEY, checkoutAttributeId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<CheckoutAttributeValue>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cav in context.CheckoutAttributeValues
                        orderby cav.DisplayOrder
                        where cav.CheckoutAttributeId == checkoutAttributeId
                        select cav;
            var checkoutAttributeValues = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, checkoutAttributeValues);
            }
            return checkoutAttributeValues;
        }
        
        /// <summary>
        /// Gets a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <returns>Checkout attribute value</returns>
        public CheckoutAttributeValue GetCheckoutAttributeValueById(int checkoutAttributeValueId)
        {
            if (checkoutAttributeValueId == 0)
                return null;

            string key = string.Format(CHECKOUTATTRIBUTEVALUES_BY_ID_KEY, checkoutAttributeValueId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (CheckoutAttributeValue)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cav in context.CheckoutAttributeValues
                        where cav.CheckoutAttributeValueId == checkoutAttributeValueId
                        select cav;
            var checkoutAttributeValue = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, checkoutAttributeValue);
            }
            return checkoutAttributeValue;
        }

        /// <summary>
        /// Inserts a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public void InsertCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException("checkoutAttributeValue");

            checkoutAttributeValue.Name = CommonHelper.EnsureNotNull(checkoutAttributeValue.Name);
            checkoutAttributeValue.Name = CommonHelper.EnsureMaximumLength(checkoutAttributeValue.Name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            context.CheckoutAttributeValues.AddObject(checkoutAttributeValue);
            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public void UpdateCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException("checkoutAttributeValue");

            checkoutAttributeValue.Name = CommonHelper.EnsureNotNull(checkoutAttributeValue.Name);
            checkoutAttributeValue.Name = CommonHelper.EnsureMaximumLength(checkoutAttributeValue.Name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(checkoutAttributeValue))
                context.CheckoutAttributeValues.Attach(checkoutAttributeValue);

            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets localized checkout attribute value by id
        /// </summary>
        /// <param name="checkoutAttributeValueLocalizedId">Localized checkout attribute value identifier</param>
        /// <returns>Localized checkout attribute value</returns>
        public CheckoutAttributeValueLocalized GetCheckoutAttributeValueLocalizedById(int checkoutAttributeValueLocalizedId)
        {
            if (checkoutAttributeValueLocalizedId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cavl in context.CheckoutAttributeValueLocalized
                        where cavl.CheckoutAttributeValueLocalizedId == checkoutAttributeValueLocalizedId
                        select cavl;
            var checkoutAttributeValueLocalized = query.SingleOrDefault();
            return checkoutAttributeValueLocalized;
        }

        /// <summary>
        /// Gets localized checkout attribute value by checkout attribute value id
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <returns>Localized checkout attribute value</returns>
        public List<CheckoutAttributeValueLocalized> GetCheckoutAttributeValueLocalizedByCheckoutAttributeValueId(int checkoutAttributeValueId)
        {
            if (checkoutAttributeValueId == 0)
                return new List<CheckoutAttributeValueLocalized>();

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cavl in context.CheckoutAttributeValueLocalized
                        where cavl.CheckoutAttributeValueId == checkoutAttributeValueId
                        select cavl;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets localized checkout attribute value by checkout attribute value id and language id
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized checkout attribute value</returns>
        public CheckoutAttributeValueLocalized GetCheckoutAttributeValueLocalizedByCheckoutAttributeValueIdAndLanguageId(int checkoutAttributeValueId, int languageId)
        {
            if (checkoutAttributeValueId == 0 || languageId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cavl in context.CheckoutAttributeValueLocalized
                        orderby cavl.CheckoutAttributeValueLocalizedId
                        where cavl.CheckoutAttributeValueId == checkoutAttributeValueId &&
                        cavl.LanguageId == languageId
                        select cavl;
            var checkoutAttributeValueLocalized = query.FirstOrDefault();
            return checkoutAttributeValueLocalized;
        }

        /// <summary>
        /// Inserts a localized checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueLocalized">Localized checkout attribute value</param>
        public void InsertCheckoutAttributeValueLocalized(CheckoutAttributeValueLocalized checkoutAttributeValueLocalized)
        {
            if (checkoutAttributeValueLocalized == null)
                throw new ArgumentNullException("checkoutAttributeValueLocalized");

            checkoutAttributeValueLocalized.Name = CommonHelper.EnsureNotNull(checkoutAttributeValueLocalized.Name);
            checkoutAttributeValueLocalized.Name = CommonHelper.EnsureMaximumLength(checkoutAttributeValueLocalized.Name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;
            
            context.CheckoutAttributeValueLocalized.AddObject(checkoutAttributeValueLocalized);
            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Update a localized checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueLocalized">Localized checkout attribute value</param>
        public void UpdateCheckoutAttributeValueLocalized(CheckoutAttributeValueLocalized checkoutAttributeValueLocalized)
        {
            if (checkoutAttributeValueLocalized == null)
                throw new ArgumentNullException("checkoutAttributeValueLocalized");

            checkoutAttributeValueLocalized.Name = CommonHelper.EnsureNotNull(checkoutAttributeValueLocalized.Name);
            checkoutAttributeValueLocalized.Name = CommonHelper.EnsureMaximumLength(checkoutAttributeValueLocalized.Name, 100);

            bool allFieldsAreEmpty = string.IsNullOrEmpty(checkoutAttributeValueLocalized.Name);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(checkoutAttributeValueLocalized))
                context.CheckoutAttributeValueLocalized.Attach(checkoutAttributeValueLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                context.DeleteObject(checkoutAttributeValueLocalized);
                context.SaveChanges();
            }
            else
            {
                context.SaveChanges();
            }

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }
        
        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.CheckoutAttributeManager.CacheEnabled");
            }
        }

        #endregion
    }
}
