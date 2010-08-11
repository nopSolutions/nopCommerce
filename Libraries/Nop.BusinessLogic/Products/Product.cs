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
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common;


namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
    /// <summary>
    /// Represents a product
    /// </summary>
    public partial class Product : BaseEntity
    {
        #region Fields
        private List<ProductLocalized> _productLocalized;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the Product class
        /// </summary>
        public Product()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the full description
        /// </summary>
        public string FullDescription { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets the template identifier
        /// </summary>
        public int TemplateId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the product on home page
        /// </summary>
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the search-engine name
        /// </summary>
        public string SEName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product allows customer reviews
        /// </summary>
        public bool AllowCustomerReviews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product allows customer ratings
        /// </summary>
        public bool AllowCustomerRatings { get; set; }

        /// <summary>
        /// Gets or sets the rating sum
        /// </summary>
        public int RatingSum { get; set; }

        /// <summary>
        /// Gets or sets the total rating votes
        /// </summary>
        public int TotalRatingVotes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of product creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of product update
        /// </summary>
        public DateTime UpdatedOn { get; set; }
        #endregion

        #region Localizable methods/properties

        /// <summary>
        /// Gets the localized name
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized name</returns>
        public string GetLocalizedName(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_productLocalized == null)
                        _productLocalized = ProductManager.GetProductLocalizedByProductId(this.ProductId);

                    var temp1 = _productLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.Name))
                        return temp1.Name;
                }
            }

            return this.Name;
        }

        /// <summary>
        /// Gets the localized name 
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return GetLocalizedName(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        /// <summary>
        /// Gets the localized short description 
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized short description</returns>
        public string GetLocalizedShortDescription(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_productLocalized == null)
                        _productLocalized = ProductManager.GetProductLocalizedByProductId(this.ProductId);

                    var temp1 = _productLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.ShortDescription))
                        return temp1.ShortDescription;
                }
            }

            return this.ShortDescription;
        }

        /// <summary>
        /// Gets the localized short description 
        /// </summary>
        public string LocalizedShortDescription
        {
            get
            {
                return GetLocalizedShortDescription(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        /// <summary>
        /// Gets the localized full description 
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized full description</returns>
        public string GetLocalizedFullDescription(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_productLocalized == null)
                        _productLocalized = ProductManager.GetProductLocalizedByProductId(this.ProductId);

                    var temp1 = _productLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.FullDescription))
                        return temp1.FullDescription;
                }
            }

            return this.FullDescription;
        }

        /// <summary>
        /// Gets the localized full description 
        /// </summary>
        public string LocalizedFullDescription
        {
            get
            {
                return GetLocalizedFullDescription(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        /// <summary>
        /// Gets the localized meta keywords
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized meta keywords</returns>
        public string GetLocalizedMetaKeywords(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_productLocalized == null)
                        _productLocalized = ProductManager.GetProductLocalizedByProductId(this.ProductId);

                    var temp1 = _productLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.MetaKeywords))
                        return temp1.MetaKeywords;
                }
            }

            return this.MetaKeywords;
        }

        /// <summary>
        /// Gets the localized meta keywords 
        /// </summary>
        public string LocalizedMetaKeywords
        {
            get
            {
                return GetLocalizedMetaKeywords(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        /// <summary>
        /// Gets the localized meta description
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized meta description</returns>
        public string GetLocalizedMetaDescription(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_productLocalized == null)
                        _productLocalized = ProductManager.GetProductLocalizedByProductId(this.ProductId);

                    var temp1 = _productLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.MetaDescription))
                        return temp1.MetaDescription;
                }
            }

            return this.MetaDescription;
        }

        /// <summary>
        /// Gets the localized meta description
        /// </summary>
        public string LocalizedMetaDescription
        {
            get
            {
                return GetLocalizedMetaDescription(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        /// <summary>
        /// Gets the localized meta title 
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized meta title </returns>
        public string GetLocalizedMetaTitle(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_productLocalized == null)
                        _productLocalized = ProductManager.GetProductLocalizedByProductId(this.ProductId);

                    var temp1 = _productLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.MetaTitle))
                        return temp1.MetaTitle;
                }
            }

            return this.MetaTitle;
        }

        /// <summary>
        /// Gets the localized meta title 
        /// </summary>
        public string LocalizedMetaTitle
        {
            get
            {
                return GetLocalizedMetaTitle(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        /// <summary>
        /// Gets the localized search-engine name 
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized search-engine name</returns>
        public string GetLocalizedSEName(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_productLocalized == null)
                        _productLocalized = ProductManager.GetProductLocalizedByProductId(this.ProductId);

                    var temp1 = _productLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.SEName))
                        return temp1.SEName;
                }
            }

            return this.SEName;
        }

        /// <summary>
        /// Gets the localized search-engine name 
        /// </summary>
        public string LocalizedSEName
        {
            get
            {
                return GetLocalizedSEName(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        #endregion

        #region Custom Properties
        /// <summary>
        /// Gets the product variants
        /// </summary>
        public List<ProductVariant> ProductVariants
        {
            get
            {
                return ProductManager.GetProductVariantsByProductId(this.ProductId);
            }
        }

        /// <summary>
        /// Indicates whether Product has more than one variant
        /// </summary>
        public bool HasMultipleVariants
        {
            get
            {
                return (this.ProductVariants.Count > 1);
            }
        }

        /// <summary>
        /// Gets the product template
        /// </summary>
        public ProductTemplate ProductTemplate
        {
            get
            {
                return TemplateManager.GetProductTemplateById(this.TemplateId);
            }
        }

        /// <summary>
        /// Gets the related products
        /// </summary>
        public List<RelatedProduct> RelatedProducts
        {
            get
            {
                return ProductManager.GetRelatedProductsByProductId1(this.ProductId);
            }
        }

        /// <summary>
        /// Gets the cross-sell products
        /// </summary>
        public List<CrossSellProduct> CrossSellProducts
        {
            get
            {
                return ProductManager.GetCrossSellProductsByProductId1(this.ProductId);
            }
        }

        /// <summary>
        /// Gets the default product pictures
        /// </summary>
        public Picture DefaultPicture
        {
            get
            {
                var picture = PictureManager.GetPicturesByProductId(this.ProductId, 1).FirstOrDefault();
                return picture;
            }
        }

        /// <summary>
        /// Gets the product pictures
        /// </summary>
        public List<ProductPicture> ProductPictures
        {
            get
            {
                return ProductManager.GetProductPicturesByProductId(this.ProductId);
            }
        }

        /// <summary>
        /// Gets the product categories
        /// </summary>
        public List<ProductCategory> ProductCategories
        {
            get
            {
                return CategoryManager.GetProductCategoriesByProductId(this.ProductId);
            }
        }

        /// <summary>
        /// Gets the product manufacturers
        /// </summary>
        public List<ProductManufacturer> ProductManufacturers
        {
            get
            {
                return ManufacturerManager.GetProductManufacturersByProductId(this.ProductId);
            }
        }

        /// <summary>
        /// Gets the product reviews
        /// </summary>
        public List<ProductReview> ProductReviews
        {
            get
            {
                return ProductManager.GetProductReviewByProductId(this.ProductId);
            }
        }

        /// <summary>
        /// Returns the product variant with minimal price
        /// </summary>
        public ProductVariant MinimalPriceProductVariant
        {
            get
            {
                var productVariants = this.ProductVariants;
                productVariants.Sort(new GenericComparer<ProductVariant>
                    ("Price", GenericComparer<ProductVariant>.SortOrder.Ascending));
                if (productVariants.Count > 0)
                    return productVariants[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns the price range for product variants
        /// </summary>
        public PriceRange ProductPriceRange
        {
            get
            {
                var productVariants = this.ProductVariants;
                productVariants.Sort(new GenericComparer<ProductVariant>
                    ("Price", GenericComparer<ProductVariant>.SortOrder.Ascending));
                if (productVariants.Count > 0)
                {
                    return new PriceRange
                    {
                        From = productVariants[0].Price,
                        To = productVariants[(productVariants.Count - 1)].Price
                    };
                }
                else
                {
                    return new PriceRange(); 
                }
            }
        }
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the localized products
        /// </summary>
        public virtual ICollection<ProductLocalized> NpProductLocalized { get; set; }

        /// <summary>
        /// Gets the product variants
        /// </summary>
        public virtual ICollection<ProductVariant> NpProductVariants { get; set; }

        /// <summary>
        /// Gets the product categories
        /// </summary>
        public virtual ICollection<ProductCategory> NpProductCategories { get; set; }

        /// <summary>
        /// Gets the product manufacturers
        /// </summary>
        public virtual ICollection<ProductManufacturer> NpProductManufacturers { get; set; }

        /// <summary>
        /// Gets the product specification attributes
        /// </summary>
        public virtual ICollection<ProductSpecificationAttribute> NpProductSpecificationAttributes { get; set; }
        
        /// <summary>
        /// Gets the product tags
        /// </summary>
        public virtual ICollection<ProductTag> NpProductTags { get; set; }
        
        /// <summary>
        /// Gets the product pictures
        /// </summary>
        public virtual ICollection<ProductPicture> NpProductPictures { get; set; }
        
        /// <summary>
        /// Gets the product review
        /// </summary>
        public virtual ICollection<ProductReview> NpProductReviews { get; set; }
        
        #endregion
    }

}
