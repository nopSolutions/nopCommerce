using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product attribute value model
    /// </summary>
    public partial class ProductAttributeValueModel : BaseNopEntityModel, ILocalizedModel<ProductAttributeValueLocalizedModel>
    {
        #region Ctor

        public ProductAttributeValueModel()
        {
            ProductPictureModels = new List<ProductPictureModel>();
            Locales = new List<ProductAttributeValueLocalizedModel>();
        }

        #endregion

        #region Properties

        public int ProductAttributeMappingId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AttributeValueType")]
        public int AttributeValueTypeId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AttributeValueType")]
        public string AttributeValueTypeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct")]
        public int AssociatedProductId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct")]
        public string AssociatedProductName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.ColorSquaresRgb")]
        public string ColorSquaresRgb { get; set; }

        public bool DisplayColorSquaresRgb { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.ImageSquaresPicture")]
        [UIHint("Picture")]
        public int ImageSquaresPictureId { get; set; }

        public bool DisplayImageSquaresPicture { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.PriceAdjustment")]
        public decimal PriceAdjustment { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.PriceAdjustment")]
        //used only on the values list page
        public string PriceAdjustmentStr { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.PriceAdjustmentUsePercentage")]
        public bool PriceAdjustmentUsePercentage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.WeightAdjustment")]
        public decimal WeightAdjustment { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.WeightAdjustment")]
        //used only on the values list page
        public string WeightAdjustmentStr { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Cost")]
        public decimal Cost { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.CustomerEntersQty")]
        public bool CustomerEntersQty { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Quantity")]
        public int Quantity { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Picture")]
        public string PictureThumbnailUrl { get; set; }

        public IList<ProductPictureModel> ProductPictureModels { get; set; }

        public IList<ProductAttributeValueLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial class ProductAttributeValueLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Name")]
        public string Name { get; set; }
    }
}