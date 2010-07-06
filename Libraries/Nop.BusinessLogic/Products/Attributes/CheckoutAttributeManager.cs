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

namespace NopSolutions.NopCommerce.BusinessLogic.Products.Attributes
{
    /// <summary>
    /// Checkout attribute manager
    /// </summary>
    public partial class CheckoutAttributeManager
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
        public static void DeleteCheckoutAttribute(int checkoutAttributeId)
        {
            var checkoutAttribute = GetCheckoutAttributeById(checkoutAttributeId);
            if (checkoutAttribute == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(checkoutAttribute))
                context.CheckoutAttributes.Attach(checkoutAttribute);
            context.DeleteObject(checkoutAttribute);
            context.SaveChanges();

            if (CheckoutAttributeManager.CacheEnabled)
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
        public static List<CheckoutAttribute> GetAllCheckoutAttributes(bool dontLoadShippableProductRequired)
        {
            string key = string.Format(CHECKOUTATTRIBUTES_ALL_KEY, dontLoadShippableProductRequired);
            object obj2 = NopRequestCache.Get(key);
            if (CheckoutAttributeManager.CacheEnabled && (obj2 != null))
            {
                return (List<CheckoutAttribute>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ca in context.CheckoutAttributes
                        orderby ca.DisplayOrder
                        where !dontLoadShippableProductRequired || !ca.ShippableProductRequired
                        select ca;
            var checkoutAttributes = query.ToList();

            if (CheckoutAttributeManager.CacheEnabled)
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
        public static CheckoutAttribute GetCheckoutAttributeById(int checkoutAttributeId)
        {
            if (checkoutAttributeId == 0)
                return null;

            string key = string.Format(CHECKOUTATTRIBUTES_BY_ID_KEY, checkoutAttributeId);
            object obj2 = NopRequestCache.Get(key);
            if (CheckoutAttributeManager.CacheEnabled && (obj2 != null))
            {
                return (CheckoutAttribute)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ca in context.CheckoutAttributes
                        where ca.CheckoutAttributeId == checkoutAttributeId
                        select ca;
            var checkoutAttribute = query.SingleOrDefault();

            if (CheckoutAttributeManager.CacheEnabled)
            {
                NopRequestCache.Add(key, checkoutAttribute);
            }
            return checkoutAttribute;
        }

        /// <summary>
        /// Inserts a checkout attribute
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="textPrompt">Text prompt</param>
        /// <param name="isRequired">Value indicating whether the entity is required</param>
        /// <param name="shippableProductRequired">Value indicating whether shippable products are required in order to display this attribute</param>
        /// <param name="isTaxExempt">Value indicating whether the attribute is marked as tax exempt</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="attributeControlTypeId">Attribute control type identifier</param>
        /// <param name="displayOrder">Display order</param>
        /// <returns>Checkout attribute</returns>
        public static CheckoutAttribute InsertCheckoutAttribute(string name,
            string textPrompt, bool isRequired, bool shippableProductRequired,
            bool isTaxExempt, int taxCategoryId, int attributeControlTypeId,
            int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            textPrompt = CommonHelper.EnsureMaximumLength(textPrompt, 300);

            var context = ObjectContextHelper.CurrentObjectContext;

            var checkoutAttribute = context.CheckoutAttributes.CreateObject();
            checkoutAttribute.Name = name;
            checkoutAttribute.TextPrompt = textPrompt;
            checkoutAttribute.IsRequired = isRequired;
            checkoutAttribute.ShippableProductRequired = shippableProductRequired;
            checkoutAttribute.IsTaxExempt = isTaxExempt;
            checkoutAttribute.TaxCategoryId = taxCategoryId;
            checkoutAttribute.AttributeControlTypeId = attributeControlTypeId;
            checkoutAttribute.DisplayOrder = displayOrder;

            context.CheckoutAttributes.AddObject(checkoutAttribute);
            context.SaveChanges();

            if (CheckoutAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }
            return checkoutAttribute;
        }

        /// <summary>
        /// Updates the checkout attribute
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <param name="name">Name</param>
        /// <param name="textPrompt">Text prompt</param>
        /// <param name="isRequired">Value indicating whether the entity is required</param>
        /// <param name="shippableProductRequired">Value indicating whether shippable products are required in order to display this attribute</param>
        /// <param name="isTaxExempt">Value indicating whether the attribute is marked as tax exempt</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="attributeControlTypeId">Attribute control type identifier</param>
        /// <param name="displayOrder">Display order</param>
        /// <returns>Checkout attribute</returns>
        public static CheckoutAttribute UpdateCheckoutAttribute(int checkoutAttributeId,
            string name, string textPrompt, bool isRequired, bool shippableProductRequired,
            bool isTaxExempt, int taxCategoryId, int attributeControlTypeId,
            int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            textPrompt = CommonHelper.EnsureMaximumLength(textPrompt, 300);

            var checkoutAttribute = GetCheckoutAttributeById(checkoutAttributeId);
            if (checkoutAttribute == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(checkoutAttribute))
                context.CheckoutAttributes.Attach(checkoutAttribute);

            checkoutAttribute.Name = name;
            checkoutAttribute.TextPrompt = textPrompt;
            checkoutAttribute.IsRequired = isRequired;
            checkoutAttribute.ShippableProductRequired = shippableProductRequired;
            checkoutAttribute.IsTaxExempt = isTaxExempt;
            checkoutAttribute.TaxCategoryId = taxCategoryId;
            checkoutAttribute.AttributeControlTypeId = attributeControlTypeId;
            checkoutAttribute.DisplayOrder = displayOrder;
            context.SaveChanges();

            if (CheckoutAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }

            return checkoutAttribute;
        }

        /// <summary>
        /// Gets localized checkout attribute by id
        /// </summary>
        /// <param name="checkoutAttributeLocalizedId">Localized checkout attribute identifier</param>
        /// <returns>Checkout attribute content</returns>
        public static CheckoutAttributeLocalized GetCheckoutAttributeLocalizedById(int checkoutAttributeLocalizedId)
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
        public static List<CheckoutAttributeLocalized> GetCheckoutAttributeLocalizedByCheckoutAttributeId(int checkoutAttributeId)
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
        public static CheckoutAttributeLocalized GetCheckoutAttributeLocalizedByCheckoutAttributeIdAndLanguageId(int checkoutAttributeId, int languageId)
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
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <param name="textPrompt">Text prompt</param>
        /// <returns>Checkout attribute content</returns>
        public static CheckoutAttributeLocalized InsertCheckoutAttributeLocalized(int checkoutAttributeId,
            int languageId, string name, string textPrompt)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            textPrompt = CommonHelper.EnsureMaximumLength(textPrompt, 300);

            var context = ObjectContextHelper.CurrentObjectContext;

            var checkoutAttributeLocalized = context.CheckoutAttributeLocalized.CreateObject();
            checkoutAttributeLocalized.CheckoutAttributeId = checkoutAttributeId;
            checkoutAttributeLocalized.LanguageId = languageId;
            checkoutAttributeLocalized.Name = name;
            checkoutAttributeLocalized.TextPrompt = textPrompt;

            context.CheckoutAttributeLocalized.AddObject(checkoutAttributeLocalized);
            context.SaveChanges();

            if (CheckoutAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }

            return checkoutAttributeLocalized;
        }

        /// <summary>
        /// Update a localized checkout attribute
        /// </summary>
        /// <param name="checkoutAttributeLocalizedId">Localized checkout attribute identifier</param>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <param name="textPrompt">Text prompt</param>
        /// <returns>Checkout attribute content</returns>
        public static CheckoutAttributeLocalized UpdateCheckoutAttributeLocalized(int checkoutAttributeLocalizedId,
            int checkoutAttributeId, int languageId, string name, string textPrompt)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            textPrompt = CommonHelper.EnsureMaximumLength(textPrompt, 300);

            var checkoutAttributeLocalized = GetCheckoutAttributeLocalizedById(checkoutAttributeLocalizedId);
            if (checkoutAttributeLocalized == null)
                return null;

            bool allFieldsAreEmpty = string.IsNullOrEmpty(name) &&
                string.IsNullOrEmpty(textPrompt);

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
                checkoutAttributeLocalized.CheckoutAttributeId = checkoutAttributeId;
                checkoutAttributeLocalized.LanguageId = languageId;
                checkoutAttributeLocalized.Name = name;
                checkoutAttributeLocalized.TextPrompt = textPrompt;
                context.SaveChanges();
            }

            if (CheckoutAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }

            return checkoutAttributeLocalized;
        }
        
        #endregion

        #region Checkout variant attribute values

        /// <summary>
        /// Deletes a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        public static void DeleteCheckoutAttributeValue(int checkoutAttributeValueId)
        {
            var checkoutAttributeValue = GetCheckoutAttributeValueById(checkoutAttributeValueId);
            if (checkoutAttributeValue == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(checkoutAttributeValue))
                context.CheckoutAttributeValues.Attach(checkoutAttributeValue);
            context.DeleteObject(checkoutAttributeValue);
            context.SaveChanges();

            if (CheckoutAttributeManager.CacheEnabled)
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
        public static List<CheckoutAttributeValue> GetCheckoutAttributeValues(int checkoutAttributeId)
        {
            string key = string.Format(CHECKOUTATTRIBUTEVALUES_ALL_KEY, checkoutAttributeId);
            object obj2 = NopRequestCache.Get(key);
            if (CheckoutAttributeManager.CacheEnabled && (obj2 != null))
            {
                return (List<CheckoutAttributeValue>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cav in context.CheckoutAttributeValues
                        orderby cav.DisplayOrder
                        where cav.CheckoutAttributeId == checkoutAttributeId
                        select cav;
            var checkoutAttributeValues = query.ToList();

            if (CheckoutAttributeManager.CacheEnabled)
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
        public static CheckoutAttributeValue GetCheckoutAttributeValueById(int checkoutAttributeValueId)
        {
            if (checkoutAttributeValueId == 0)
                return null;

            string key = string.Format(CHECKOUTATTRIBUTEVALUES_BY_ID_KEY, checkoutAttributeValueId);
            object obj2 = NopRequestCache.Get(key);
            if (CheckoutAttributeManager.CacheEnabled && (obj2 != null))
            {
                return (CheckoutAttributeValue)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cav in context.CheckoutAttributeValues
                        where cav.CheckoutAttributeValueId == checkoutAttributeValueId
                        select cav;
            var checkoutAttributeValue = query.SingleOrDefault();

            if (CheckoutAttributeManager.CacheEnabled)
            {
                NopRequestCache.Add(key, checkoutAttributeValue);
            }
            return checkoutAttributeValue;
        }

        /// <summary>
        /// Inserts a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeId">The checkout attribute identifier</param>
        /// <param name="name">The checkout attribute name</param>
        /// <param name="priceAdjustment">The price adjustment</param>
        /// <param name="weightAdjustment">The weight adjustment</param>
        /// <param name="isPreSelected">The value indicating whether the value is pre-selected</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Checkout attribute value</returns>
        public static CheckoutAttributeValue InsertCheckoutAttributeValue(int checkoutAttributeId,
            string name, decimal priceAdjustment, decimal weightAdjustment,
            bool isPreSelected, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var checkoutAttributeValue = context.CheckoutAttributeValues.CreateObject();
            checkoutAttributeValue.CheckoutAttributeId = checkoutAttributeId;
            checkoutAttributeValue.Name = name;
            checkoutAttributeValue.PriceAdjustment = priceAdjustment;
            checkoutAttributeValue.WeightAdjustment = weightAdjustment;
            checkoutAttributeValue.IsPreSelected = isPreSelected;
            checkoutAttributeValue.DisplayOrder = displayOrder;

            context.CheckoutAttributeValues.AddObject(checkoutAttributeValue);
            context.SaveChanges();

            if (CheckoutAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }

            return checkoutAttributeValue;
        }

        /// <summary>
        /// Updates the checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueId">The checkout attribute value identifier</param>
        /// <param name="checkoutAttributeId">The checkout attribute identifier</param>
        /// <param name="name">The checkout attribute name</param>
        /// <param name="priceAdjustment">The price adjustment</param>
        /// <param name="weightAdjustment">The weight adjustment</param>
        /// <param name="isPreSelected">The value indicating whether the value is pre-selected</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Checkout attribute value</returns>
        public static CheckoutAttributeValue UpdateCheckoutAttributeValue(int checkoutAttributeValueId,
            int checkoutAttributeId, string name, decimal priceAdjustment, decimal weightAdjustment,
            bool isPreSelected, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);

            var checkoutAttributeValue = GetCheckoutAttributeValueById(checkoutAttributeValueId);
            if (checkoutAttributeValue == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(checkoutAttributeValue))
                context.CheckoutAttributeValues.Attach(checkoutAttributeValue);

            checkoutAttributeValue.CheckoutAttributeId = checkoutAttributeId;
            checkoutAttributeValue.Name = name;
            checkoutAttributeValue.PriceAdjustment = priceAdjustment;
            checkoutAttributeValue.WeightAdjustment = weightAdjustment;
            checkoutAttributeValue.IsPreSelected = isPreSelected;
            checkoutAttributeValue.DisplayOrder = displayOrder;
            context.SaveChanges();

            if (CheckoutAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }

            return checkoutAttributeValue;
        }

        /// <summary>
        /// Gets localized checkout attribute value by id
        /// </summary>
        /// <param name="checkoutAttributeValueLocalizedId">Localized checkout attribute value identifier</param>
        /// <returns>Localized checkout attribute value</returns>
        public static CheckoutAttributeValueLocalized GetCheckoutAttributeValueLocalizedById(int checkoutAttributeValueLocalizedId)
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
        public static List<CheckoutAttributeValueLocalized> GetCheckoutAttributeValueLocalizedByCheckoutAttributeValueId(int checkoutAttributeValueId)
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
        public static CheckoutAttributeValueLocalized GetCheckoutAttributeValueLocalizedByCheckoutAttributeValueIdAndLanguageId(int checkoutAttributeValueId, int languageId)
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
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <returns>Localized checkout attribute value</returns>
        public static CheckoutAttributeValueLocalized InsertCheckoutAttributeValueLocalized(int checkoutAttributeValueId,
            int languageId, string name)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var checkoutAttributeValueLocalized = context.CheckoutAttributeValueLocalized.CreateObject();
            checkoutAttributeValueLocalized.CheckoutAttributeValueId = checkoutAttributeValueId;
            checkoutAttributeValueLocalized.LanguageId = languageId;
            checkoutAttributeValueLocalized.Name = name;

            context.CheckoutAttributeValueLocalized.AddObject(checkoutAttributeValueLocalized);
            context.SaveChanges();

            if (CheckoutAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }

            return checkoutAttributeValueLocalized;
        }

        /// <summary>
        /// Update a localized checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueLocalizedId">Localized checkout attribute value identifier</param>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <returns>Localized checkout attribute value</returns>
        public static CheckoutAttributeValueLocalized UpdateCheckoutAttributeValueLocalized(int checkoutAttributeValueLocalizedId,
            int checkoutAttributeValueId, int languageId, string name)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);

            var checkoutAttributeValueLocalized = GetCheckoutAttributeValueLocalizedById(checkoutAttributeValueLocalizedId);
            if (checkoutAttributeValueLocalized == null)
                return null;

            bool allFieldsAreEmpty = string.IsNullOrEmpty(name);

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
                checkoutAttributeValueLocalized.CheckoutAttributeValueId = checkoutAttributeValueId;
                checkoutAttributeValueLocalized.LanguageId = languageId;
                checkoutAttributeValueLocalized.Name = name;
                context.SaveChanges();
            }

            if (CheckoutAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CHECKOUTATTRIBUTEVALUES_PATTERN_KEY);
            }

            return checkoutAttributeValueLocalized;
        }
        
        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.CheckoutAttributeManager.CacheEnabled");
            }
        }
        #endregion
    }
}
