using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.AbcCore.Infrastructure
{
    public static class CustomPublicWidgetZones
    {
        private static string Below480px => "_below_480px";
        private static string Min480px => "_min_480px";
        private static string Min768px => "_min_768px";
        private static string Above1000px => "_above_1000px";

        private static string HomepageBeforeCategories => PublicWidgetZones.HomepageBeforeCategories;
        public static string HomepageBeforeCategoriesBelow480px => HomepageBeforeCategories + Below480px;
        public static string HomepageBeforeCategoriesMin480px => HomepageBeforeCategories + Min480px;
        public static string HomepageBeforeCategoriesMin768px => HomepageBeforeCategories + Min768px;
        public static string HomepageBeforeCategoriesAbove1000px => HomepageBeforeCategories + Above1000px;

        private static string HomepageBottom => PublicWidgetZones.HomepageBottom;
        public static string HomepageBottomBelow480px => HomepageBottom + Below480px;
        public static string HomepageBottomMin480px => HomepageBottom + Min480px;
        public static string HomepageBottomMin768px => HomepageBottom + Min768px;
        public static string HomepageBottomAbove1000px => HomepageBottom + Above1000px;

        private static string Footer => PublicWidgetZones.Footer;
        public static string FooterBelow480px => Footer + Below480px;
        public static string FooterMin480px => Footer + Min480px;
        public static string FooterMin768px => Footer + Min768px;
        public static string FooterAbove1000px => Footer + Above1000px;

        private static string TopicPage => "topic_page_";
        public static string TopicPageBeforeBody => TopicPage + "before_body";
        public static string TopicPageAfterBody => TopicPage + "after_body";

        private static string CategoryDetailsTop => PublicWidgetZones.CategoryDetailsTop;
        public static string CategoryDetailsTopBelow480px => CategoryDetailsTop + Below480px;
        public static string CategoryDetailsTopMin480px => CategoryDetailsTop + Min480px;
        public static string CategoryDetailsTopMin768px => CategoryDetailsTop + Min768px;
        public static string CategoryDetailsTopAbove1000px => CategoryDetailsTop + Above1000px;

        private static string CategoryDetailsBeforeSubcategories => PublicWidgetZones.CategoryDetailsBeforeSubcategories;
        public static string CategoryDetailsBeforeSubcategoriesBelow480px => CategoryDetailsBeforeSubcategories + Below480px;
        public static string CategoryDetailsBeforeSubcategoriesMin480px => CategoryDetailsBeforeSubcategories + Min480px;
        public static string CategoryDetailsBeforeSubcategoriesMin768px => CategoryDetailsBeforeSubcategories + Min768px;
        public static string CategoryDetailsBeforeSubcategoriesAbove1000px => CategoryDetailsBeforeSubcategories + Above1000px;

        public static string CategoryDetailsBeforeFilters => "categorydetails_before_filters";
        public static string CategoryDetailsBeforeProductList => "categorydetails_before_product_list";

        private static string CategoryDetailsBottom => PublicWidgetZones.CategoryDetailsBottom;
        public static string CategoryDetailsBottomBelow480px => CategoryDetailsBottom + Below480px;
        public static string CategoryDetailsBottomMin480px => CategoryDetailsBottom + Min480px;
        public static string CategoryDetailsBottomMin768px => CategoryDetailsBottom + Min768px;
        public static string CategoryDetailsBottomAbove1000px => CategoryDetailsBottom + Above1000px;



        public static string Cart => "cart";

        public static string OpCheckoutWarrantyTop => "op_checkout_warranty_top";
        public static string OpCheckoutWarrantyBottom => "op_checkout_warranty_bottom";

        public static string ManufacturerDetailsTop => PublicWidgetZones.ManufacturerDetailsTop;
        public static string ManufacturerDetailsTopBelow480px => ManufacturerDetailsTop + Below480px;
        public static string ManufacturerDetailsTopMin480px => ManufacturerDetailsTop + Min480px;
        public static string ManufacturerDetailsTopMin768px => ManufacturerDetailsTop + Min768px;
        public static string ManufacturerDetailsTopAbove1000px => ManufacturerDetailsTop + Above1000px;

        public static string ManufacturerDetailsTopFiltered => PublicWidgetZones.ManufacturerDetailsTop + "_filtered";
        public static string ManufacturerDetailsTopFilteredBelow480px => ManufacturerDetailsTopFiltered + Below480px;
        public static string ManufacturerDetailsTopFilteredMin480px => ManufacturerDetailsTopFiltered + Min480px;
        public static string ManufacturerDetailsTopFilteredMin768px => ManufacturerDetailsTopFiltered + Min768px;
        public static string ManufacturerDetailsTopFilteredAbove1000px => ManufacturerDetailsTopFiltered + Above1000px;

        public static string ManufacturerDetailsBottom => PublicWidgetZones.ManufacturerDetailsBottom;
        public static string ManufacturerDetailsBottomBelow480px => ManufacturerDetailsBottom + Below480px;
        public static string ManufacturerDetailsBottomMin480px => ManufacturerDetailsBottom + Min480px;
        public static string ManufacturerDetailsBottomMin768px => ManufacturerDetailsBottom + Min768px;
        public static string ManufacturerDetailsBottomAbove1000px => ManufacturerDetailsBottom + Above1000px;

        public static string ProductBoxAddinfoReviews => "productbox_addinfo_reviews";
        public static string ProductBoxAfter => "productbox_after";

        public static string ProductDetailsAfterPictures => "productdetails_after_pictures";
        public static string ProductDetailsBeforeAddToCart => "productdetails_before_addtocart";
        public static string ProductDetailsBeforeTabs => "productdetails_before_tabs";
        public static string ProductDetailsReviews => "productdetails_reviews";
        public static string ProductDetailsReviewsTabContent => "productdetails_reviews_tab_content";
        public const string ProductDetailsAfterPrice = "productdetails_after_price";

        public static string OrderSummaryAfterProductMiniDescription => "order_summary_after_product_mini_description";

        public static string ProductDetailsDescriptionTabTop => "productdetails_description_tab_top";

        public static string PromoProductsBelowBanner => "promo_products_below_banner";
    }
}