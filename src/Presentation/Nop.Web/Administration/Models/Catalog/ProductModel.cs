using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Catalog
{
    [Validator(typeof(ProductValidator))]
    public class ProductModel : BaseNopEntityModel, ILocalizedModel<ProductLocalizedModel>
    {
        public ProductModel()
        {
            Locales = new List<ProductLocalizedModel>();
            ProductVariantModels = new List<ProductVariantModel>();
            ProductPictureModels = new List<ProductPictureModel>();
            CopyProductModel = new CopyProductModel();
            AvailableProductTemplates = new List<SelectListItem>();
        }

        //picture thumbnail
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ProductTemplate")]
        [AllowHtml]
        public int ProductTemplateId { get; set; }
        public IList<SelectListItem> AvailableProductTemplates { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.AllowCustomerReviews")]
        public bool AllowCustomerReviews { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ProductTags")]
        public string ProductTags { get; set; }
        
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Published")]
        public bool Published { get; set; }
        


        public IList<ProductLocalizedModel> Locales { get; set; }



        //existing product variants
        public IList<ProductVariantModel> ProductVariantModels { get; set; }
        //default variant (the first one) 
        public ProductVariantModel FirstProductVariantModel { get; set; }



        //categories
        public int NumberOfAvailableCategories { get; set; }

        //manufacturers
        public int NumberOfAvailableManufacturers { get; set; }
        




        //pictures
        public ProductPictureModel AddPictureModel { get; set; }
        public IList<ProductPictureModel> ProductPictureModels { get; set; }





        //add specification attribute model
        public AddProductSpecificationAttributeModel AddSpecificationAttributeModel { get; set; }



        //copy product
        public CopyProductModel CopyProductModel { get; set; }
        
        #region Nested classes
        
        public class AddProductSpecificationAttributeModel : BaseNopEntityModel
        {
            public AddProductSpecificationAttributeModel()
            {
                AvailableAttributes = new List<SelectListItem>();
                AvailableOptions = new List<SelectListItem>();
            }
            
            [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttribute")]
            public int SpecificationAttributeId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttributeOption")]
            public int SpecificationAttributeOptionId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.AllowFiltering")]
            public bool AllowFiltering { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.ShowOnProductPage")]
            public bool ShowOnProductPage { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            public IList<SelectListItem> AvailableAttributes { get; set; }
            public IList<SelectListItem> AvailableOptions { get; set; }
        }
        
        public class ProductPictureModel : BaseNopEntityModel
        {
            public int ProductId { get; set; }

            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
            public int PictureId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
            public string PictureUrl { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }
        
        public class ProductCategoryModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Products.Categories.Fields.Category")]
            [UIHint("ProductCategory")]
            public string Category { get; set; }

            public int ProductId { get; set; }

            public int CategoryId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Categories.Fields.IsFeaturedProduct")]
            public bool IsFeaturedProduct { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Categories.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public class ProductManufacturerModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Products.Manufacturers.Fields.Manufacturer")]
            [UIHint("ProductManufacturer")]
            public string Manufacturer { get; set; }

            public int ProductId { get; set; }

            public int ManufacturerId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Manufacturers.Fields.IsFeaturedProduct")]
            public bool IsFeaturedProduct { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Manufacturers.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public class RelatedProductModel : BaseNopEntityModel
        {
            public int ProductId1 { get; set; }

            public int ProductId2 { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.RelatedProducts.Fields.Product")]
            public string Product2Name { get; set; }
            
            [NopResourceDisplayName("Admin.Catalog.Products.RelatedProducts.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public class AddRelatedProductModel : BaseNopModel
        {
            public AddRelatedProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
            }
            public GridModel<ProductModel> Products { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            [AllowHtml]
            public string SearchProductName { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }

            public int ProductId { get; set; }

            public int[] SelectedProductIds { get; set; }
        }

        public class CrossSellProductModel : BaseNopEntityModel
        {
            public int ProductId1 { get; set; }

            public int ProductId2 { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.CrossSells.Fields.Product")]
            public string Product2Name { get; set; }
        }

        public class AddCrossSellProductModel : BaseNopModel
        {
            public AddCrossSellProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
            }
            public GridModel<ProductModel> Products { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            [AllowHtml]
            public string SearchProductName { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }

            public int ProductId { get; set; }

            public int[] SelectedProductIds { get; set; }
        }

        #endregion
    }

    public class ProductLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}