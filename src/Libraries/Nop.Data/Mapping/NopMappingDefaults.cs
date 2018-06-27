
namespace Nop.Data.Mapping
{
    /// <summary>
    /// Represents default values related to data mapping
    /// </summary>
    public static partial class NopMappingDefaults
    {
        /// <summary>
        /// Gets a name of the Product-ProductAttribute mapping table
        /// </summary>
        public static string ProductProductAttributeTable => "Product_ProductAttribute_Mapping";

        /// <summary>
        /// Gets a name of the Product-Category mapping table
        /// </summary>
        public static string ProductCategoryTable => "Product_Category_Mapping";

        /// <summary>
        /// Gets a name of the Product-Manufacturer mapping table
        /// </summary>
        public static string ProductManufacturerTable => "Product_Manufacturer_Mapping";

        /// <summary>
        /// Gets a name of the Product-Picture mapping table
        /// </summary>
        public static string ProductPictureTable => "Product_Picture_Mapping";

        /// <summary>
        /// Gets a name of the Product-ProductTag mapping table
        /// </summary>
        public static string ProductProductTagTable => "Product_ProductTag_Mapping";
        
        /// <summary>
        /// Gets a name of the ProductReview_ReviewType mapping table
        /// </summary>
        public static string ProductReview_ReviewTypeTable => "ProductReview_ReviewType_Mapping";

        /// <summary>
        /// Gets a name of the Product-SpecificationAttribute mapping table
        /// </summary>
        public static string ProductSpecificationAttributeTable => "Product_SpecificationAttribute_Mapping";

        /// <summary>
        /// Gets a name of the Customer-Addresses mapping table
        /// </summary>
        public static string CustomerAddressesTable => "CustomerAddresses";

        /// <summary>
        /// Gets a name of the Customer-CustomerRole mapping table
        /// </summary>
        public static string CustomerCustomerRoleTable => "Customer_CustomerRole_Mapping";

        /// <summary>
        /// Gets a name of the Discount-AppliedToCategories mapping table
        /// </summary>
        public static string DiscountAppliedToCategoriesTable => "Discount_AppliedToCategories";

        /// <summary>
        /// Gets a name of the Discount-AppliedToManufacturers mapping table
        /// </summary>
        public static string DiscountAppliedToManufacturersTable => "Discount_AppliedToManufacturers";

        /// <summary>
        /// Gets a name of the Discount-AppliedToProducts mapping table
        /// </summary>
        public static string DiscountAppliedToProductsTable => "Discount_AppliedToProducts";

        /// <summary>
        /// Gets a name of the ForumsGroup mapping table
        /// </summary>
        public static string ForumsGroupTable => "Forums_Group";

        /// <summary>
        /// Gets a name of the Forum mapping table
        /// </summary>
        public static string ForumTable => "Forums_Forum";

        /// <summary>
        /// Gets a name of the ForumsPost mapping table
        /// </summary>
        public static string ForumsPostTable => "Forums_Post";

        /// <summary>
        /// Gets a name of the ForumsPostVote mapping table
        /// </summary>
        public static string ForumsPostVoteTable => "Forums_PostVote";

        /// <summary>
        /// Gets a name of the ForumsSubscription mapping table
        /// </summary>
        public static string ForumsSubscriptionTable => "Forums_Subscription";

        /// <summary>
        /// Gets a name of the ForumsTopic mapping table
        /// </summary>
        public static string ForumsTopicTable => "Forums_Topic";

        /// <summary>
        /// Gets a name of the PrivateMessage mapping table
        /// </summary>
        public static string PrivateMessageTable => "Forums_PrivateMessage";

        /// <summary>
        /// Gets a name of the NewsItem mapping table
        /// </summary>
        public static string NewsItemTable => "News";

        /// <summary>
        /// Gets a name of the PermissionRecord-CustomerRole mapping table
        /// </summary>
        public static string PermissionRecordRoleTable => "PermissionRecord_Role_Mapping";

        /// <summary>
        /// Gets a name of the ShippingMethod-Restrictions mapping table
        /// </summary>
        public static string ShippingMethodRestrictionsTable => "ShippingMethodRestrictions";
    }
}