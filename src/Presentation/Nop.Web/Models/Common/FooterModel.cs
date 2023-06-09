using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial record FooterModel : BaseNopModel
    {
        public FooterModel()
        {
            Topics = new List<FooterTopicModel>();
        }

        public string StoreName { get; set; }
        public bool IsHomePage { get; set; }
        public bool WishlistEnabled { get; set; }
        public bool ShoppingCartEnabled { get; set; }
        public bool SitemapEnabled { get; set; }
        public bool SearchEnabled { get; set; }
        public bool NewsEnabled { get; set; }
        public bool BlogEnabled { get; set; }
        public bool CompareProductsEnabled { get; set; }
        public bool ForumEnabled { get; set; }
        public bool RecentlyViewedProductsEnabled { get; set; }
        public bool NewProductsEnabled { get; set; }
        public bool AllowCustomersToApplyForVendorAccount { get; set; }
        public bool AllowCustomersToCheckGiftCardBalance { get; set; }
        public bool DisplayTaxShippingInfoFooter { get; set; }
        public bool HidePoweredByNopCommerce { get; set; }

        public int WorkingLanguageId { get; set; }

        public IList<FooterTopicModel> Topics { get; set; }

        public bool DisplaySitemapFooterItem { get; set; }
        public bool DisplayContactUsFooterItem { get; set; }
        public bool DisplayProductSearchFooterItem { get; set; }
        public bool DisplayNewsFooterItem { get; set; }
        public bool DisplayBlogFooterItem { get; set; }
        public bool DisplayForumsFooterItem { get; set; }
        public bool DisplayRecentlyViewedProductsFooterItem { get; set; }
        public bool DisplayCompareProductsFooterItem { get; set; }
        public bool DisplayNewProductsFooterItem { get; set; }
        public bool DisplayCustomerInfoFooterItem { get; set; }
        public bool DisplayCustomerOrdersFooterItem { get; set; }
        public bool DisplayCustomerAddressesFooterItem { get; set; }
        public bool DisplayShoppingCartFooterItem { get; set; }
        public bool DisplayWishlistFooterItem { get; set; }
        public bool DisplayApplyVendorAccountFooterItem { get; set; }

        #region Nested classes

        public partial record FooterTopicModel : BaseNopEntityModel
        {
            public string Name { get; set; }
            public string SeName { get; set; }

            public bool IncludeInFooterColumn1 { get; set; }
            public bool IncludeInFooterColumn2 { get; set; }
            public bool IncludeInFooterColumn3 { get; set; }
        }

        #endregion
    }
}