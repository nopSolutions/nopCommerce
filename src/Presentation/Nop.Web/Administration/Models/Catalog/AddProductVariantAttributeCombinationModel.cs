using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class AddProductVariantAttributeCombinationModel : BaseNopModel
    {
        public AddProductVariantAttributeCombinationModel()
        {
            ProductVariantAttributes = new List<ProductVariantAttributeModel>();
            Warnings = new List<string>();
        }
        
        [NopResourceDisplayName("Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.StockQuantity")]
        public int StockQuantity { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Variants.ProductVariantAttributes.AttributeCombinations.Fields.AllowOutOfStockOrders")]
        public bool AllowOutOfStockOrders { get; set; }

        public IList<ProductVariantAttributeModel> ProductVariantAttributes { get; set; }

        public IList<string> Warnings { get; set; }

        public int ProductVariantId { get; set; }

        #region Nested classes

        public partial class ProductVariantAttributeModel : BaseNopEntityModel
        {
            public ProductVariantAttributeModel()
            {
                Values = new List<ProductVariantAttributeValueModel>();
            }

            public int ProductAttributeId { get; set; }

            public string Name { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<ProductVariantAttributeValueModel> Values { get; set; }
        }

        public partial class ProductVariantAttributeValueModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }
        }
        #endregion
    }
}