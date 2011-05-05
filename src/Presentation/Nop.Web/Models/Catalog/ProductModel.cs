using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public class ProductModel : BaseNopEntityModel
    {
        public ProductModel()
        {
            ProductPrice = new ProductPriceModel();
            DefaultPictureModel = new PictureModel();
            PictureModels = new List<PictureModel>();
            ProductVariantModels = new List<ProductVariantModel>();
        }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string FullDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string MetaTitle { get; set; }

        public string SeName { get; set; }

        //price
        public ProductPriceModel ProductPrice { get; set; }

        //picture(s)
        public bool DefaultPictureZoomEnabled { get; set; }
        public PictureModel DefaultPictureModel { get; set; }
        public IList<PictureModel> PictureModels { get; set; }
        public IList<ProductVariantModel> ProductVariantModels { get; set; }

		#region Nested Classes
        
        public class ProductPriceModel
        {
            public string OldPrice { get; set; }

            public string Price {get;set;}

            public bool DisableBuyButton { get; set; }
        }

        public class ProductBreadcrumbModel
        {
            public ProductBreadcrumbModel()
            {
                CategoryBreadcrumb = new List<CategoryModel>();
            }

            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public bool DisplayBreadcrumb { get; set; }
            public IList<CategoryModel> CategoryBreadcrumb { get; set; }
        }


        public class ProductVariantModel : BaseNopEntityModel
        {
            public ProductVariantModel()
            {
                GiftCard = new GiftCardModel();
                ProductVariantPrice = new ProductVariantPriceModel();
                PictureModel = new PictureModel();
                AddToCart = new AddToCartModel();
            }

            public string Name { get; set; }

            public bool ShowSku { get; set; }

            public string Sku { get; set; }

            public string Description { get; set; }

            public bool ShowManufacturerPartNumber { get; set; }

            public string ManufacturerPartNumber { get; set; }

            public string DownloadSampleUrl { get; set; }

            public GiftCardModel GiftCard { get; set; }

            public string StockAvailablity { get; set; }

            public ProductVariantPriceModel ProductVariantPrice { get; set; }

            public AddToCartModel AddToCart { get; set; }

            public PictureModel PictureModel { get; set; }
            
            #region Nested Classes

            public class AddToCartModel
            {
                public int ProductVariantId { get; set; }

                [NopResourceDisplayName("Products.Qty")]
                public int EnteredQuantity { get; set; }

                [NopResourceDisplayName("Products.EnterProductPrice")]
                public bool CustomerEntersPrice { get; set; }
                [NopResourceDisplayName("Products.EnterProductPrice")]
                public decimal CustomerEnteredPrice { get; set; }
                public String CustomerEnteredPriceRange { get; set; }
                
                public bool DisableBuyButton { get; set; }
                public bool DisableWishlistButton { get; set; }
            }

            public class ProductVariantPriceModel
            {
                public string OldPrice { get; set; }

                public string Price { get; set; }

                public string PriceWithDiscount { get; set; }

                public bool CustomerEntersPrice { get; set; }

                public bool CallForPrice { get; set; }
            }

            public class GiftCardModel : BaseNopModel
            {
                public bool IsGiftCard { get; set; }

                [NopResourceDisplayName("Products.GiftCard.RecipientName")]
                public string RecipientName { get; set; }
                [NopResourceDisplayName("Products.GiftCard.RecipientEmail")]
                public string RecipientEmail { get; set; }
                [NopResourceDisplayName("Products.GiftCard.SenderName")]
                public string SenderName { get; set; }
                [NopResourceDisplayName("Products.GiftCard.SenderEmail")]
                public string SenderEmail { get; set; }
                [NopResourceDisplayName("Products.GiftCard.Message")]
                public string Message { get; set; }

                public GiftCardType GiftCardType { get; set; }
            }

            public class TierPriceModel : BaseNopModel
            {
                public string Price { get; set; }

                public int Quantity { get; set; }
            }

            #endregion
        }
		#endregion
    }
}