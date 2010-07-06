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

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Products.Specs
{
    /// <summary>
    /// Specification attribute manager
    /// </summary>
    public partial class SpecificationAttributeManager
    {
        #region Constants
        private const string SPECIFICATIONATTRIBUTE_BY_ID_KEY = "Nop.specificationattributes.id-{0}";
        private const string SPECIFICATIONATTRIBUTEOPTION_BY_ID_KEY = "Nop.specificationattributeoptions.id-{0}";
        private const string PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY = "Nop.productspecificationattribute.allbyproductid-{0}-{1}-{2}";
        private const string SPECIFICATIONATTRIBUTE_PATTERN_KEY = "Nop.specificationattributes.";
        private const string SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY = "Nop.specificationattributeoptions.";
        private const string PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY = "Nop.productspecificationattribute.";
        #endregion

        #region Methods

        #region Specification attribute
        
        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute</returns>
        public static SpecificationAttribute GetSpecificationAttributeById(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return null;

            string key = string.Format(SPECIFICATIONATTRIBUTE_BY_ID_KEY, specificationAttributeId);
            object obj2 = NopRequestCache.Get(key);
            if (SpecificationAttributeManager.CacheEnabled && (obj2 != null))
            {
                return (SpecificationAttribute)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sa in context.SpecificationAttributes
                        where sa.SpecificationAttributeId == specificationAttributeId
                        select sa;
            var specificationAttribute = query.SingleOrDefault();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.Add(key, specificationAttribute);
            }
            return specificationAttribute;
        }

        /// <summary>
        /// Gets specification attribute collection
        /// </summary>
        /// <returns>Specification attribute collection</returns>
        public static List<SpecificationAttribute> GetSpecificationAttributes()
        {
            int languageId = 0;
            if (NopContext.Current != null)
                languageId = NopContext.Current.WorkingLanguage.LanguageId;
            return GetSpecificationAttributes(languageId);
        }

        /// <summary>
        /// Gets specification attribute collection
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Specification attribute collection</returns>
        public static List<SpecificationAttribute> GetSpecificationAttributes(int languageId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sa in context.SpecificationAttributes
                        orderby sa.DisplayOrder
                        select sa;
            var specificationAttributes = query.ToList();
            return specificationAttributes;
        }

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        public static void DeleteSpecificationAttribute(int specificationAttributeId)
        {
            var specificationAttribute = GetSpecificationAttributeById(specificationAttributeId);
            if (specificationAttribute == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(specificationAttribute))
                context.SpecificationAttributes.Attach(specificationAttribute);
            context.DeleteObject(specificationAttribute);
            context.SaveChanges();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="displayOrder">Display order</param>
        /// <returns>Specification attribute</returns>
        public static SpecificationAttribute InsertSpecificationAttribute(string name, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var specificationAttribute = context.SpecificationAttributes.CreateObject();
            specificationAttribute.Name = name;
            specificationAttribute.DisplayOrder = displayOrder;

            context.SpecificationAttributes.AddObject(specificationAttribute);
            context.SaveChanges();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }

            return specificationAttribute;
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <param name="name">The name</param>
        /// <param name="displayOrder">Display order</param>
        /// <returns>Specification attribute</returns>
        public static SpecificationAttribute UpdateSpecificationAttribute(int specificationAttributeId, string name, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);

            var specificationAttribute = GetSpecificationAttributeById(specificationAttributeId);
            if (specificationAttribute == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(specificationAttribute))
                context.SpecificationAttributes.Attach(specificationAttribute);

            specificationAttribute.Name = name;
            specificationAttribute.DisplayOrder = displayOrder;
            context.SaveChanges();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }

            return specificationAttribute;
        }

        /// <summary>
        /// Gets localized specification attribute by id
        /// </summary>
        /// <param name="specificationAttributeLocalizedId">Localized specification identifier</param>
        /// <returns>Specification attribute content</returns>
        public static SpecificationAttributeLocalized GetSpecificationAttributeLocalizedById(int specificationAttributeLocalizedId)
        {
            if (specificationAttributeLocalizedId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sal in context.SpecificationAttributeLocalized
                        where sal.SpecificationAttributeLocalizedId == specificationAttributeLocalizedId
                        select sal;
            var specificationAttributeLocalized = query.SingleOrDefault();
            return specificationAttributeLocalized;
        }

        /// <summary>
        /// Gets localized specification attribute by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">Specification attribute identifier</param>
        /// <returns>Secification attribute content</returns>
        public static List<SpecificationAttributeLocalized> GetSpecificationAttributeLocalizedBySpecificationAttributeId(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return new List<SpecificationAttributeLocalized>();

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sal in context.SpecificationAttributeLocalized
                        where sal.SpecificationAttributeId == specificationAttributeId
                        select sal;
            var content = query.ToList();
            return content;
        }
        
        /// <summary>
        /// Gets localized specification attribute by specification attribute id and language id
        /// </summary>
        /// <param name="specificationAttributeId">Specification attribute identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Specification attribute content</returns>
        public static SpecificationAttributeLocalized GetSpecificationAttributeLocalizedBySpecificationAttributeIdAndLanguageId(int specificationAttributeId, int languageId)
        {
            if (specificationAttributeId == 0 || languageId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sal in context.SpecificationAttributeLocalized
                        orderby sal.SpecificationAttributeLocalizedId
                        where sal.SpecificationAttributeId == specificationAttributeId &&
                        sal.LanguageId == languageId
                        select sal;
            var specificationAttributeLocalized = query.FirstOrDefault();
            return specificationAttributeLocalized;
        }

        /// <summary>
        /// Inserts a localized specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">Specification attribute identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <returns>Specification attribute content</returns>
        public static SpecificationAttributeLocalized InsertSpecificationAttributeLocalized(int specificationAttributeId,
            int languageId, string name)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var specificationAttributeLocalized = context.SpecificationAttributeLocalized.CreateObject();
            specificationAttributeLocalized.SpecificationAttributeId = specificationAttributeId;
            specificationAttributeLocalized.LanguageId = languageId;
            specificationAttributeLocalized.Name = name;

            context.SpecificationAttributeLocalized.AddObject(specificationAttributeLocalized);
            context.SaveChanges();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }

            return specificationAttributeLocalized;
        }

        /// <summary>
        /// Update a localized specification attribute
        /// </summary>
        /// <param name="specificationAttributeLocalizedId">Localized specification attribute identifier</param>
        /// <param name="specificationAttributeId">Specification attribute identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <returns>Specification attribute content</returns>
        public static SpecificationAttributeLocalized UpdateSpecificationAttributeLocalized(int specificationAttributeLocalizedId,
            int specificationAttributeId, int languageId, string name)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);

            var specificationAttributeLocalized = GetSpecificationAttributeLocalizedById(specificationAttributeLocalizedId);
            if (specificationAttributeLocalized == null)
                return null;

            bool allFieldsAreEmpty = string.IsNullOrEmpty(name);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(specificationAttributeLocalized))
                context.SpecificationAttributeLocalized.Attach(specificationAttributeLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                context.DeleteObject(specificationAttributeLocalized);
                context.SaveChanges();
            }
            else
            {
                specificationAttributeLocalized.SpecificationAttributeId = specificationAttributeId;
                specificationAttributeLocalized.LanguageId = languageId;
                specificationAttributeLocalized.Name = name;
                context.SaveChanges();
            }

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }

            return specificationAttributeLocalized;
        }
        
        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        /// <returns>Specification attribute option</returns>
        public static SpecificationAttributeOption GetSpecificationAttributeOptionById(int specificationAttributeOptionId)
        {
            if (specificationAttributeOptionId == 0)
                return null;

            string key = string.Format(SPECIFICATIONATTRIBUTEOPTION_BY_ID_KEY, specificationAttributeOptionId);
            object obj2 = NopRequestCache.Get(key);
            if (SpecificationAttributeManager.CacheEnabled && (obj2 != null))
            {
                return (SpecificationAttributeOption)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sao in context.SpecificationAttributeOptions
                        where sao.SpecificationAttributeOptionId == specificationAttributeOptionId
                        select sao;
            var specificationAttributeOption = query.SingleOrDefault();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.Add(key, specificationAttributeOption);
            }
            return specificationAttributeOption;
        }

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        public static List<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sao in context.SpecificationAttributeOptions
                        orderby sao.DisplayOrder
                        where sao.SpecificationAttributeId == specificationAttributeId
                        select sao;
            var specificationAttributeOptions = query.ToList();

            return specificationAttributeOptions;
        }

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        public static void DeleteSpecificationAttributeOption(int specificationAttributeOptionId)
        {
            var specificationAttributeOption = GetSpecificationAttributeOptionById(specificationAttributeOptionId);
            if (specificationAttributeOption == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(specificationAttributeOption))
                context.SpecificationAttributeOptions.Attach(specificationAttributeOption);
            context.DeleteObject(specificationAttributeOption);
            context.SaveChanges();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <param name="name">The name</param>
        /// <param name="displayOrder">Display order</param>
        /// <returns>Specification attribute option</returns>
        public static SpecificationAttributeOption InsertSpecificationAttributeOption(int specificationAttributeId, 
            string name, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 500);

            var context = ObjectContextHelper.CurrentObjectContext;

            var specificationAttributeOption = context.SpecificationAttributeOptions.CreateObject();
            specificationAttributeOption.SpecificationAttributeId = specificationAttributeId;
            specificationAttributeOption.Name = name;
            specificationAttributeOption.DisplayOrder = displayOrder;

            context.SpecificationAttributeOptions.AddObject(specificationAttributeOption);
            context.SaveChanges();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }

            return specificationAttributeOption;
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <param name="name">The name</param>
        /// <param name="displayOrder">Display order</param>
        /// <returns>Specification attribute option</returns>
        public static SpecificationAttributeOption UpdateSpecificationAttributeOptions(int specificationAttributeOptionId, 
            int specificationAttributeId, string name, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 500);

            var specificationAttributeOption = GetSpecificationAttributeOptionById(specificationAttributeOptionId);
            if (specificationAttributeOption == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(specificationAttributeOption))
                context.SpecificationAttributeOptions.Attach(specificationAttributeOption);

            specificationAttributeOption.SpecificationAttributeId = specificationAttributeId;
            specificationAttributeOption.Name = name;
            specificationAttributeOption.DisplayOrder = displayOrder;
            context.SaveChanges();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }

            return specificationAttributeOption;
        }

        /// <summary>
        /// Gets localized specification attribute option by id
        /// </summary>
        /// <param name="specificationAttributeOptionLocalizedId">Localized specification attribute option identifier</param>
        /// <returns>Localized specification attribute option</returns>
        public static SpecificationAttributeOptionLocalized GetSpecificationAttributeOptionLocalizedById(int specificationAttributeOptionLocalizedId)
        {
            if (specificationAttributeOptionLocalizedId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from saol in context.SpecificationAttributeOptionLocalized
                        where saol.SpecificationAttributeOptionLocalizedId == specificationAttributeOptionLocalizedId
                        select saol;
            var specificationAttributeOptionLocalized = query.SingleOrDefault();
            return specificationAttributeOptionLocalized;
        }

        /// <summary>
        /// Gets localized specification attribute option by category id
        /// </summary>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier</param>
        /// <returns>Localized specification attribute option content</returns>
        public static List<SpecificationAttributeOptionLocalized> GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionId(int specificationAttributeOptionId)
        {
            if (specificationAttributeOptionId == 0)
                return new List<SpecificationAttributeOptionLocalized>();

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from saol in context.SpecificationAttributeOptionLocalized
                        where saol.SpecificationAttributeOptionId == specificationAttributeOptionId
                        select saol;
            var content = query.ToList();
            return content;
        }


        /// <summary>
        /// Gets localized specification attribute option by specification attribute option id and language id
        /// </summary>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized specification attribute option</returns>
        public static SpecificationAttributeOptionLocalized GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionIdAndLanguageId(int specificationAttributeOptionId, int languageId)
        {
            if (specificationAttributeOptionId == 0 || languageId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from saol in context.SpecificationAttributeOptionLocalized
                        orderby saol.SpecificationAttributeOptionLocalizedId
                        where saol.SpecificationAttributeOptionId == specificationAttributeOptionId &&
                        saol.LanguageId == languageId
                        select saol;
            var specificationAttributeOptionLocalized = query.FirstOrDefault();
            return specificationAttributeOptionLocalized;
        }

        /// <summary>
        /// Inserts a localized specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <returns>Localized specification attribute option</returns>
        public static SpecificationAttributeOptionLocalized InsertSpecificationAttributeOptionLocalized(int specificationAttributeOptionId,
            int languageId, string name)
        {
            name = CommonHelper.EnsureMaximumLength(name, 500);

            var context = ObjectContextHelper.CurrentObjectContext;

            var specificationAttributeOptionLocalized = context.SpecificationAttributeOptionLocalized.CreateObject();
            specificationAttributeOptionLocalized.SpecificationAttributeOptionId = specificationAttributeOptionId;
            specificationAttributeOptionLocalized.LanguageId = languageId;
            specificationAttributeOptionLocalized.Name = name;

            context.SpecificationAttributeOptionLocalized.AddObject(specificationAttributeOptionLocalized);
            context.SaveChanges();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }

            return specificationAttributeOptionLocalized;
        }

        /// <summary>
        /// Update a localized specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionLocalizedId">Localized specification attribute option identifier</param>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <returns>Localized specification attribute option</returns>
        public static SpecificationAttributeOptionLocalized UpdateSpecificationAttributeOptionLocalized(int specificationAttributeOptionLocalizedId,
            int specificationAttributeOptionId, int languageId, string name)
        {
            name = CommonHelper.EnsureMaximumLength(name, 500);

            var specificationAttributeOptionLocalized = GetSpecificationAttributeOptionLocalizedById(specificationAttributeOptionLocalizedId);
            if (specificationAttributeOptionLocalized == null)
                return null;

            bool allFieldsAreEmpty = string.IsNullOrEmpty(name);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(specificationAttributeOptionLocalized))
                context.SpecificationAttributeOptionLocalized.Attach(specificationAttributeOptionLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                context.DeleteObject(specificationAttributeOptionLocalized);
                context.SaveChanges();
            }
            else
            {
                specificationAttributeOptionLocalized.SpecificationAttributeOptionId = specificationAttributeOptionId;
                specificationAttributeOptionLocalized.LanguageId = languageId;
                specificationAttributeOptionLocalized.Name = name;
                context.SaveChanges();
            }

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }

            return specificationAttributeOptionLocalized;
        }
        
        #endregion

        #region Product specification attribute

        /// <summary>
        /// Deletes a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttributeId">Product specification attribute identifier</param>
        public static void DeleteProductSpecificationAttribute(int productSpecificationAttributeId)
        {
            var productSpecificationAttribute = GetProductSpecificationAttributeById(productSpecificationAttributeId);
            if (productSpecificationAttribute == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productSpecificationAttribute))
                context.ProductSpecificationAttributes.Attach(productSpecificationAttribute);
            context.DeleteObject(productSpecificationAttribute);
            context.SaveChanges();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public static List<ProductSpecificationAttribute> GetProductSpecificationAttributesByProductId(int productId)
        {
            return GetProductSpecificationAttributesByProductId(productId, null, null);
        }

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 0 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnProductPage">0 to load attributes with ShowOnProductPage set to false, 0 to load attributes with ShowOnProductPage set to true, null to load all attributes</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public static List<ProductSpecificationAttribute> GetProductSpecificationAttributesByProductId(int productId, 
            bool? allowFiltering, bool? showOnProductPage)
        {
            string allowFilteringCacheStr = "null";
            if (allowFiltering.HasValue)
                allowFilteringCacheStr = allowFiltering.ToString();
            string showOnProductPageCacheStr = "null";
            if (showOnProductPage.HasValue)
                showOnProductPageCacheStr = showOnProductPage.ToString();
            string key = string.Format(PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY, productId, allowFilteringCacheStr, showOnProductPageCacheStr);
            object obj2 = NopRequestCache.Get(key);
            if (SpecificationAttributeManager.CacheEnabled && (obj2 != null))
            {
                return (List<ProductSpecificationAttribute>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = (IQueryable<ProductSpecificationAttribute>)context.ProductSpecificationAttributes;
            query = query.Where(psa => psa.ProductId == productId);
            if (allowFiltering.HasValue)
                query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
            if (showOnProductPage.HasValue)
                query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
            query = query.OrderBy(psa => psa.DisplayOrder);

            var productSpecificationAttributes = query.ToList();
            
            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.Add(key, productSpecificationAttributes);
            }
            return productSpecificationAttributes;
        }

        /// <summary>
        /// Gets a product specification attribute mapping 
        /// </summary>
        /// <param name="productSpecificationAttributeId">Product specification attribute mapping identifier</param>
        /// <returns>Product specification attribute mapping</returns>
        public static ProductSpecificationAttribute GetProductSpecificationAttributeById(int productSpecificationAttributeId)
        {
            if (productSpecificationAttributeId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from psa in context.ProductSpecificationAttributes
                        where psa.ProductSpecificationAttributeId == productSpecificationAttributeId
                        select psa;
            var productSpecificationAttribute = query.SingleOrDefault();
            return productSpecificationAttribute;
        }

        /// <summary>
        /// Inserts a product specification attribute mapping
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier</param>
        /// <param name="allowFiltering">Allow product filtering by this attribute</param>
        /// <param name="showOnProductPage">Show the attribute on the product page</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Product specification attribute mapping</returns>
        public static ProductSpecificationAttribute InsertProductSpecificationAttribute(int productId,
            int specificationAttributeOptionId, bool allowFiltering, 
            bool showOnProductPage, int displayOrder)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var productSpecificationAttribute = context.ProductSpecificationAttributes.CreateObject();
            productSpecificationAttribute.ProductId = productId;
            productSpecificationAttribute.SpecificationAttributeOptionId = specificationAttributeOptionId;
            productSpecificationAttribute.AllowFiltering = allowFiltering;
            productSpecificationAttribute.ShowOnProductPage = showOnProductPage;
            productSpecificationAttribute.DisplayOrder = displayOrder;

            context.ProductSpecificationAttributes.AddObject(productSpecificationAttribute);
            context.SaveChanges();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
            return productSpecificationAttribute;
        }

        /// <summary>
        /// Updates the product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttributeId">product specification attribute mapping identifier</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="specificationAttributeOptionId">Specification attribute identifier</param>
        /// <param name="allowFiltering">Allow product filtering by this attribute</param>
        /// <param name="showOnProductPage">Show the attribute on the product page</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Product specification attribute mapping</returns>
        public static ProductSpecificationAttribute UpdateProductSpecificationAttribute(int productSpecificationAttributeId,
            int productId, int specificationAttributeOptionId, bool allowFiltering, bool showOnProductPage, int displayOrder)
        {
            var productSpecificationAttribute = GetProductSpecificationAttributeById(productSpecificationAttributeId);
            if (productSpecificationAttribute == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productSpecificationAttribute))
                context.ProductSpecificationAttributes.Attach(productSpecificationAttribute);

            productSpecificationAttribute.ProductId = productId;
            productSpecificationAttribute.SpecificationAttributeOptionId = specificationAttributeOptionId;
            productSpecificationAttribute.AllowFiltering = allowFiltering;
            productSpecificationAttribute.ShowOnProductPage = showOnProductPage;
            productSpecificationAttribute.DisplayOrder = displayOrder;
            context.SaveChanges();

            if (SpecificationAttributeManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
            return productSpecificationAttribute;
        }

        #endregion

        #region Specification attribute option filter
        
        /// <summary>
        /// Gets a filtered product specification attribute mapping collection by category id
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public static List<SpecificationAttributeOptionFilter> GetSpecificationAttributeOptionFilter(int categoryId)
        {
            int languageId = 0;
            if (NopContext.Current != null)
                languageId = NopContext.Current.WorkingLanguage.LanguageId;
            return GetSpecificationAttributeOptionFilter(categoryId, languageId);
        }

        /// <summary>
        /// Gets a filtered product specification attribute mapping collection by category id
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public static List<SpecificationAttributeOptionFilter> GetSpecificationAttributeOptionFilter(int categoryId, int languageId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var result = context.Sp_SpecificationAttributeOptionFilter_LoadByFilter(categoryId, languageId);
            return result;
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
                return SettingManager.GetSettingValueBoolean("Cache.SpecificationAttributeManager.CacheEnabled");
            }
        }
        #endregion
    }
}
