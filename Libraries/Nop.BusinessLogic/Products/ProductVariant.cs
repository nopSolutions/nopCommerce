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
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Warehouses;

namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
    /// <summary>
    /// Represents a product variant
    /// </summary>
    public partial class ProductVariant : BaseEntity
    {
        #region Fields
        private List<ProductVariantLocalized> _pvLocalized;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the ProductVariant class
        /// </summary>
        public ProductVariant()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the product variant identifier
        /// </summary>
        public int ProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the SKU
        /// </summary>
        public string SKU { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer part number
        /// </summary>
        public string ManufacturerPartNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant is gift card
        /// </summary>
        public bool IsGiftCard { get; set; }

        /// <summary>
        /// Gets or sets the gift card type
        /// </summary>
        public int GiftCardType { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the product variant is download
        /// </summary>
        public bool IsDownload { get; set; }

        /// <summary>
        /// Gets or sets the download identifier
        /// </summary>
        public int DownloadId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this downloadable product can be downloaded unlimited number of times
        /// </summary>
        public bool UnlimitedDownloads { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of downloads
        /// </summary>
        public int MaxNumberOfDownloads { get; set; }

        /// <summary>
        /// Gets or sets the number of days during customers keeps access to the file.
        /// </summary>
        public int? DownloadExpirationDays { get; set; }

        /// <summary>
        /// Gets or sets the download activation type
        /// </summary>
        public int DownloadActivationType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant has a sample download file
        /// </summary>
        public bool HasSampleDownload { get; set; }

        /// <summary>
        /// Gets or sets the sample download identifier
        /// </summary>
        public int SampleDownloadId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product has user agreement
        /// </summary>
        public bool HasUserAgreement { get; set; }

        /// <summary>
        /// Gets or sets the text of license agreement
        /// </summary>
        public string UserAgreementText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant is recurring
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        /// Gets or sets the cycle length
        /// </summary>
        public int CycleLength { get; set; }

        /// <summary>
        /// Gets or sets the cycle period
        /// </summary>
        public int CyclePeriod { get; set; }

        /// <summary>
        /// Gets or sets the total cycles
        /// </summary>
        public int TotalCycles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is ship enabled
        /// </summary>
        public bool IsShipEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is free shipping
        /// </summary>
        public bool IsFreeShipping { get; set; }

        /// <summary>
        /// Gets or sets the additional shipping charge
        /// </summary>
        public decimal AdditionalShippingCharge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant is marked as tax exempt
        /// </summary>
        public bool IsTaxExempt { get; set; }

        /// <summary>
        /// Gets or sets the tax category identifier
        /// </summary>
        public int TaxCategoryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how to manage inventory
        /// </summary>
        public int ManageInventory { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display stock availability
        /// </summary>
        public bool DisplayStockAvailability { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display stock quantity
        /// </summary>
        public bool DisplayStockQuantity { get; set; }
        
        /// <summary>
        /// Gets or sets the minimum stock quantity
        /// </summary>
        public int MinStockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the low stock activity identifier
        /// </summary>
        public int LowStockActivityId { get; set; }

        /// <summary>
        /// Gets or sets the quantity when admin should be notified
        /// </summary>
        public int NotifyAdminForQuantityBelow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow orders when out of stock
        /// </summary>
        public int Backorders { get; set; }

        /// <summary>
        /// Gets or sets the order minimum quantity
        /// </summary>
        public int OrderMinimumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the order maximum quantity
        /// </summary>
        public int OrderMaximumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the warehouse identifier
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable buy button
        /// </summary>
        public bool DisableBuyButton { get; set; }

        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the old price
        /// </summary>
        public decimal OldPrice { get; set; }

        /// <summary>
        /// Gets or sets the product cost
        /// </summary>
        public decimal ProductCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a customer enters price
        /// </summary>
        public bool CustomerEntersPrice { get; set; }

        /// <summary>
        /// Gets or sets the minimum price entered by a customer
        /// </summary>
        public decimal MinimumCustomerEnteredPrice { get; set; }

        /// <summary>
        /// Gets or sets the maximum price entered by a customer
        /// </summary>
        public decimal MaximumCustomerEnteredPrice { get; set; }

        /// <summary>
        /// Gets or sets the weight
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the length
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the available start date and time
        /// </summary>
        public DateTime? AvailableStartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the shipped end date and time
        /// </summary>
        public DateTime? AvailableEndDateTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
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
                    if (_pvLocalized == null)
                        _pvLocalized = ProductManager.GetProductVariantLocalizedByProductVariantId(this.ProductVariantId);

                    var temp1 = _pvLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
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
        /// Gets the localized description 
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized description</returns>
        public string GetLocalizedDescription(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_pvLocalized == null)
                        _pvLocalized = ProductManager.GetProductVariantLocalizedByProductVariantId(this.ProductVariantId);

                    var temp1 = _pvLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.Description))
                        return temp1.Description;
                }
            }

            return this.Description;
        }

        /// <summary>
        /// Gets the localized description 
        /// </summary>
        public string LocalizedDescription
        {
            get
            {
                return GetLocalizedDescription(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        /// <summary>
        /// Gets the localized full product name
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized full product name</returns>
        public string GetLocalizedFullProductName(int languageId)
        {
            Product product = this.Product;
            if (product != null)
            {
                if (!String.IsNullOrEmpty(this.GetLocalizedName(languageId)))
                    return product.GetLocalizedName(languageId) + " (" + this.GetLocalizedName(languageId) + ")";
                
                return product.GetLocalizedName(languageId);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the localized full product name
        /// </summary>
        public string LocalizedFullProductName
        {
            get
            {
                return GetLocalizedFullProductName(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }
        
        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the warehouse
        /// </summary>
        public Warehouse Warehouse
        {
            get
            {
                return WarehouseManager.GetWarehouseById(this.WarehouseId);
            }
        }

        /// <summary>
        /// Gets the log type
        /// </summary>
        public LowStockActivityEnum LowStockActivity
        {
            get
            {
                return (LowStockActivityEnum)this.LowStockActivityId;
            }
            set
            {
                this.LowStockActivityId = (int)value;
            }
        }

        /// <summary>
        /// Gets the tax category
        /// </summary>
        public TaxCategory TaxCategory
        {
            get
            {
                return TaxCategoryManager.GetTaxCategoryById(this.TaxCategoryId);
            }
        }

        /// <summary>
        /// Gets the product
        /// </summary>
        public Product Product
        {
            get
            {
                return ProductManager.GetProductById(this.ProductId);
            }
        }

        /// <summary>
        /// Gets the discounts of the product variant
        /// </summary>
        public List<Discount> AllDiscounts
        {
            get
            {
                return DiscountManager.GetDiscountsByProductVariantId(this.ProductVariantId);
            }
        }

        /// <summary>
        /// Gets the full product name
        /// </summary>
        public string FullProductName
        {
            get
            {
                Product product = this.Product;
                if (product != null)
                {
                    if (!String.IsNullOrEmpty(this.Name))
                        return product.Name + " (" + this.Name + ")";
                    return product.Name;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the picture
        /// </summary>
        public Picture Picture
        {
            get
            {
                return PictureManager.GetPictureById(this.PictureId);
            }
        }

        /// <summary>
        /// Gets the download
        /// </summary>
        public Download Download
        {
            get
            {
                return DownloadManager.GetDownloadById(this.DownloadId);
            }
        }
                
        /// <summary>
        /// Gets the sample download
        /// </summary>
        public Download SampleDownload
        {
            get
            {
                return DownloadManager.GetDownloadById(this.SampleDownloadId);
            }
        }
        
        /// <summary>
        /// Gets the product variant attributes
        /// </summary>
        public List<ProductVariantAttribute> ProductVariantAttributes
        {
            get
            {
                return ProductAttributeManager.GetProductVariantAttributesByProductVariantId(this.ProductVariantId);
            }
        }

        /// <summary>
        /// Gets the tier prices of the product variant
        /// </summary>
        public List<TierPrice> TierPrices
        {
            get
            {
                return ProductManager.GetTierPricesByProductVariantId(this.ProductVariantId);
            }
        }

        /// <summary>
        /// Gets the tier prices of the product variant
        /// </summary>
        public List<CustomerRoleProductPrice> CustomerRoleProductPrices
        {
            get
            {
                return ProductManager.GetAllCustomerRoleProductPrices(this.ProductVariantId);
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the localized product variants
        /// </summary>
        public virtual ICollection<ProductVariantLocalized> NpProductVariantLocalized { get; set; }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual Product NpProduct { get; set; }
        
        /// <summary>
        /// Gets the tier prices
        /// </summary>
        public virtual ICollection<TierPrice> NpTierPrices { get; set; }

        /// <summary>
        /// Gets the restricted discounts
        /// </summary>
        public virtual ICollection<Discount> NpRestrictedDiscounts { get; set; }

        /// <summary>
        /// Gets the discounts
        /// </summary>
        public virtual ICollection<Discount> NpDiscounts { get; set; }

        /// <summary>
        /// Gets the product variant pricelists
        /// </summary>
        public virtual ICollection<ProductVariantPricelist> NpProductVariantPricelists { get; set; }

        /// <summary>
        /// Gets the product variant attribute combinations
        /// </summary>
        public virtual ICollection<ProductVariantAttributeCombination> NpProductVariantAttributeCombinations { get; set; }

        /// <summary>
        /// Gets the product variant attributes
        /// </summary>
        public virtual ICollection<ProductVariantAttribute> NpProductVariantAttributes { get; set; }

        /// <summary>
        /// Gets the order product variant
        /// </summary>
        public virtual ICollection<OrderProductVariant> NpOrderProductVariants { get; set; }
        
        #endregion
    }
}
