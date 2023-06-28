using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents the product details
    /// </summary>
    public class Product : ApiResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the ETag
        /// </summary>
        [JsonProperty(PropertyName = "etag")]
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the categories
        /// </summary>
        [JsonProperty(PropertyName = "categories")]
        public List<string> Categories { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the external reference
        /// </summary>
        [JsonProperty(PropertyName = "externalReference")]
        public string ExternalReference { get; set; }

        /// <summary>
        /// Gets or sets the unit name
        /// </summary>
        [JsonProperty(PropertyName = "unitName")]
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets the VAT percentage
        /// </summary>
        [JsonProperty(PropertyName = "vatPercentage")]
        public decimal? VatPercentage { get; set; }

        /// <summary>
        /// Gets or sets the tax code
        /// </summary>
        [JsonProperty(PropertyName = "taxCode")]
        public string TaxCode { get; set; }

        /// <summary>
        /// Gets or sets the tax rates
        /// </summary>
        [JsonProperty(PropertyName = "taxRates")]
        public List<string> TaxRates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is tax exempt
        /// </summary>
        [JsonProperty(PropertyName = "taxExempt")]
        public bool? TaxExempt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is create with default tax
        /// </summary>
        [JsonProperty(PropertyName = "createWithDefaultTax")]
        public bool? CreateWithDefaultTax { get; set; }

        /// <summary>
        /// Gets or sets the image lookup keys
        /// </summary>
        [JsonProperty(PropertyName = "imageLookupKeys")]
        public List<string> ImageLookupKeys { get; set; }

        /// <summary>
        /// Gets or sets the presentation
        /// </summary>
        [JsonProperty(PropertyName = "presentation")]
        public ProductPresentation Presentation { get; set; }

        /// <summary>
        /// Gets or sets the variants
        /// </summary>
        [JsonProperty(PropertyName = "variants")]
        public List<ProductVariant> Variants { get; set; }

        /// <summary>
        /// Gets or sets the online product info
        /// </summary>
        [JsonProperty(PropertyName = "online")]
        public ProductOnlineInfo Online { get; set; }

        /// <summary>
        /// Gets or sets the variant option definitions
        /// </summary>
        [JsonProperty(PropertyName = "variantOptionDefinitions")]
        public ProductVariantDefinitions VariantOptionDefinitions { get; set; }

        /// <summary>
        /// Gets or sets the category
        /// </summary>
        [JsonProperty(PropertyName = "category")]
        public ProductCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the metadata
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public ProductMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1 of a user who updated the product
        /// </summary>
        [JsonProperty(PropertyName = "updatedBy")]
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets the updated date
        /// </summary>
        [JsonProperty(PropertyName = "updated")]
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; set; }

        #endregion

        #region Nested classes

        /// <summary>
        /// Represents the product category details
        /// </summary>
        public class ProductCategory
        {
            /// <summary>
            /// Gets or sets the unique identifier as UUID version 1
            /// </summary>
            [JsonProperty(PropertyName = "uuid")]
            public string Uuid { get; set; }

            /// <summary>
            /// Gets or sets the name
            /// </summary>
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
        }

        /// <summary>
        /// Represents the product presentation details
        /// </summary>
        public class ProductPresentation
        {
            /// <summary>
            /// Gets or sets the image URL
            /// </summary>
            [JsonProperty(PropertyName = "imageUrl")]
            public string ImageUrl { get; set; }

            /// <summary>
            /// Gets or sets the background color
            /// </summary>
            [JsonProperty(PropertyName = "backgroundColor")]
            public string BackgroundColor { get; set; }

            /// <summary>
            /// Gets or sets the text color
            /// </summary>
            [JsonProperty(PropertyName = "textColor")]
            public string TextColor { get; set; }
        }

        /// <summary>
        /// Represents the product variant details
        /// </summary>
        public class ProductVariant
        {
            #region Properties

            /// <summary>
            /// Gets or sets the unique identifier as UUID version 1
            /// </summary>
            [JsonProperty(PropertyName = "uuid")]
            public string Uuid { get; set; }

            /// <summary>
            /// Gets or sets the name
            /// </summary>
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the description
            /// </summary>
            [JsonProperty(PropertyName = "description")]
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the SKU
            /// </summary>
            [JsonProperty(PropertyName = "sku")]
            public string Sku { get; set; }

            /// <summary>
            /// Gets or sets the barcode
            /// </summary>
            [JsonProperty(PropertyName = "barcode")]
            public string Barcode { get; set; }

            /// <summary>
            /// Gets or sets the VAT percentage
            /// </summary>
            [JsonProperty(PropertyName = "vatPercentage")]
            public decimal? VatPercentage { get; set; }

            /// <summary>
            /// Gets or sets the price
            /// </summary>
            [JsonProperty(PropertyName = "price")]
            public ProductPrice Price { get; set; }

            /// <summary>
            /// Gets or sets the cost price
            /// </summary>
            [JsonProperty(PropertyName = "costPrice")]
            public ProductPrice CostPrice { get; set; }

            /// <summary>
            /// Gets or sets the options
            /// </summary>
            [JsonProperty(PropertyName = "options")]
            public List<ProductVariantOption> Options { get; set; }

            /// <summary>
            /// Gets or sets the presentation
            /// </summary>
            [JsonProperty(PropertyName = "presentation")]
            public ProductPresentation Presentation { get; set; }

            #endregion

            #region Nested classes

            /// <summary>
            /// Represents the product price details
            /// </summary>
            public class ProductPrice
            {
                /// <summary>
                /// Gets or sets the amount
                /// </summary>
                [JsonProperty(PropertyName = "amount")]
                public int? Amount { get; set; }

                /// <summary>
                /// Gets or sets the currency id
                /// </summary>
                [JsonProperty(PropertyName = "currencyId")]
                public string CurrencyId { get; set; }
            }

            /// <summary>
            /// Represents the product variant option property details
            /// </summary>
            public class ProductVariantOption
            {
                /// <summary>
                /// Gets or sets the name
                /// </summary>
                [JsonProperty(PropertyName = "name")]
                public string Name { get; set; }

                /// <summary>
                /// Gets or sets the value
                /// </summary>
                [JsonProperty(PropertyName = "value")]
                public string Value { get; set; }
            }

            #endregion
        }

        /// <summary>
        /// Represents the product variant options details
        /// </summary>
        public class ProductVariantDefinitions
        {
            #region Properties

            /// <summary>
            /// Gets or sets the product variant options
            /// </summary>
            [JsonProperty(PropertyName = "definitions")]
            public List<ProductVariantOptionDefinition> Definitions { get; set; }

            #endregion

            #region Nested classes

            /// <summary>
            /// Represents the product variant option details
            /// </summary>
            public class ProductVariantOptionDefinition
            {
                #region Properties

                /// <summary>
                /// Gets or sets the name
                /// </summary>
                [JsonProperty(PropertyName = "name")]
                public string Name { get; set; }

                /// <summary>
                /// Gets or sets the product variant option properties
                /// </summary>
                [JsonProperty(PropertyName = "properties")]
                public List<ProductVariantOptionProperty> Properties { get; set; }

                #endregion

                #region Nested classes

                /// <summary>
                /// Represents the product variant option property details
                /// </summary>
                public class ProductVariantOptionProperty
                {
                    /// <summary>
                    /// Gets or sets the value
                    /// </summary>
                    [JsonProperty(PropertyName = "value")]
                    public string Value { get; set; }

                    /// <summary>
                    /// Gets or sets the image URL
                    /// </summary>
                    [JsonProperty(PropertyName = "imageUrl")]
                    public string ImageUrl { get; set; }
                }

                #endregion
            }

            #endregion
        }

        /// <summary>
        /// Represents the product online info details
        /// </summary>
        public class ProductOnlineInfo
        {
            #region Properties

            /// <summary>
            /// Gets or sets the status
            /// </summary>
            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            /// <summary>
            /// Gets or sets the title
            /// </summary>
            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the description
            /// </summary>
            [JsonProperty(PropertyName = "description")]
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the shipping details
            /// </summary>
            [JsonProperty(PropertyName = "shipping")]
            public ProductShippingInfo Shipping { get; set; }

            /// <summary>
            /// Gets or sets the presentation
            /// </summary>
            [JsonProperty(PropertyName = "presentation")]
            public ProductOnlinePresentation Presentation { get; set; }

            /// <summary>
            /// Gets or sets the SEO details
            /// </summary>
            [JsonProperty(PropertyName = "seo")]
            public ProductSeo Seo { get; set; }

            #endregion

            #region Nested classes

            /// <summary>
            /// Represents the product shipping details
            /// </summary>
            public class ProductShippingInfo
            {
                #region Properties

                /// <summary>
                /// Gets or sets the shipping pricing model
                /// </summary>
                [JsonProperty(PropertyName = "shippingPricingModel")]
                public string ShippingPricingModel { get; set; }

                /// <summary>
                /// Gets or sets the weight (in grams)
                /// </summary>
                [JsonProperty(PropertyName = "weightInGrams")]
                public int? WeightInGrams { get; set; }

                /// <summary>
                /// Gets or sets the weight info
                /// </summary>
                [JsonProperty(PropertyName = "weight")]
                public ProductWeight Weight { get; set; }

                #endregion

                #region Nested classes

                /// <summary>
                /// Represents the product weight details
                /// </summary>
                public class ProductWeight
                {
                    /// <summary>
                    /// Gets or sets the weight
                    /// </summary>
                    [JsonProperty(PropertyName = "weight")]
                    public decimal? Weight { get; set; }

                    /// <summary>
                    /// Gets or sets the unit
                    /// </summary>
                    [JsonProperty(PropertyName = "unit")]
                    public string Unit { get; set; }
                }

                #endregion
            }

            /// <summary>
            /// Represents the product online presentation details
            /// </summary>
            public class ProductOnlinePresentation
            {
                /// <summary>
                /// Gets or sets the display image URL
                /// </summary>
                [JsonProperty(PropertyName = "displayImageUrl")]
                public string DisplayImageUrl { get; set; }

                /// <summary>
                /// Gets or sets the additional image URLs
                /// </summary>
                [JsonProperty(PropertyName = "additionalImageUrls")]
                public List<string> AdditionalImageUrls { get; set; }

                /// <summary>
                /// Gets or sets the media URLs
                /// </summary>
                [JsonProperty(PropertyName = "mediaUrls")]
                public List<string> MediaUrls { get; set; }
            }

            /// <summary>
            /// Represents the product SEO details
            /// </summary>
            public class ProductSeo
            {
                /// <summary>
                /// Gets or sets the title
                /// </summary>
                [JsonProperty(PropertyName = "title")]
                public string Title { get; set; }

                /// <summary>
                /// Gets or sets the meta description
                /// </summary>
                [JsonProperty(PropertyName = "metaDescription")]
                public string MetaDescription { get; set; }

                /// <summary>
                /// Gets or sets the slug
                /// </summary>
                [JsonProperty(PropertyName = "slug")]
                public string Slug { get; set; }
            }

            #endregion
        }

        /// <summary>
        /// Represents the product metadata details
        /// </summary>
        public class ProductMetadata
        {
            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether the product is in POS
            /// </summary>
            [JsonProperty(PropertyName = "inPos")]
            public bool? InPos { get; set; }

            /// <summary>
            /// Gets or sets the source
            /// </summary>
            [JsonProperty(PropertyName = "source")]
            public ProductSource Source { get; set; }

            #endregion

            #region Nested classes

            /// <summary>
            /// Represents the product source details
            /// </summary>
            public class ProductSource
            {
                /// <summary>
                /// Gets or sets the name
                /// </summary>
                [JsonProperty(PropertyName = "name")]
                public string Name { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the source of product is external
                /// </summary>
                [JsonProperty(PropertyName = "external")]
                public bool? External { get; set; }
            }

            #endregion
        }

        #endregion
    }
}