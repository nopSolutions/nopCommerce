using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping
{
    /// <summary>
    /// Helper class for maintaining backward compatibility of table naming
    /// </summary>
    public static partial class NameCompatibilityManager
    {
        #region Fields

        private static readonly Dictionary<Type, string> _tableNames;

        #endregion

        #region Ctor

        static NameCompatibilityManager()
        {
            _tableNames = new Dictionary<Type, string>
            {
                { typeof(ProductAttributeMapping), "Product_ProductAttribute_Mapping" },
                { typeof(ProductProductTagMapping), "Product_ProductTag_Mapping" },
                { typeof(ProductReviewReviewTypeMapping), "ProductReview_ReviewType_Mapping" },
                { typeof(CustomerAddressMapping), "CustomerAddresses" },
                { typeof(CustomerCustomerRoleMapping), "Customer_CustomerRole_Mapping" },
                { typeof(DiscountCategoryMapping), "Discount_AppliedToCategories" },
                { typeof(DiscountManufacturerMapping), "Discount_AppliedToManufacturers" },
                { typeof(DiscountProductMapping), "Discount_AppliedToProducts" },
                { typeof(PermissionRecordCustomerRoleMapping), "PermissionRecord_Role_Mapping" },
                { typeof(ShippingMethodCountryMapping), "ShippingMethodRestrictions" },
                { typeof(ProductCategory), "Product_Category_Mapping" },
                { typeof(ProductManufacturer), "Product_Manufacturer_Mapping" },
                { typeof(ProductPicture), "Product_Picture_Mapping" },
                { typeof(ProductSpecificationAttribute), "Product_SpecificationAttribute_Mapping" },
                { typeof(ForumGroup), "Forums_Group" },
                { typeof(Forum), "Forums_Forum" },
                { typeof(ForumPost), "Forums_Post" },
                { typeof(ForumPostVote), "Forums_PostVote" },
                { typeof(ForumSubscription), "Forums_Subscription" },
                { typeof(ForumTopic), "Forums_Topic" },
                { typeof(PrivateMessage), "Forums_PrivateMessage" },
                { typeof(NewsItem), "News" }
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets table name for mapping with the type
        /// </summary>
        /// <param name="type">Type to get the table name</param>
        /// <returns>Table name</returns>
        public static string GetTableName(Type type)
        {
            return _tableNames.ContainsKey(type) ? _tableNames[type] : type.Name;
        }

        /// <summary>
        /// Gets column name for mapping with the entity's property
        /// </summary>
        /// <param name="type">Type of entity</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Column name</returns>
        public static string GetColumnName(Type type, string propertyName)
        {
            var columnName = propertyName;

            if (typeof(Customer) == type)
            {
                if (columnName.Equals("BillingAddressId"))
                    columnName = "BillingAddress_Id";
                if (columnName.Equals("ShippingAddressId"))
                    columnName = "ShippingAddress_Id";
            }

            if (typeof(CustomerCustomerRoleMapping) == type)
            {
                if (columnName.Equals("CustomerId"))
                    columnName = "Customer_Id";
                if (columnName.Equals("CustomerRoleId"))
                    columnName = "CustomerRole_Id";
            }

            if (typeof(PermissionRecordCustomerRoleMapping) == type)
            {
                if (columnName.Equals("PermissionRecordId"))
                    columnName = "PermissionRecord_Id";
                if (columnName.Equals("CustomerRoleId"))
                    columnName = "CustomerRole_Id";
            }

            if (typeof(ProductProductTagMapping) == type)
            {
                if (columnName.Equals("ProductId"))
                    columnName = "Product_Id";
                if (columnName.Equals("ProductTagId"))
                    columnName = "ProductTag_Id";
            }

            if (typeof(DiscountCategoryMapping) == type)
            {
                if (columnName.Equals("DiscountId"))
                    columnName = "Discount_Id";
                if (columnName.Equals("EntityId"))
                    columnName = "Category_Id";
            }

            if (typeof(DiscountManufacturerMapping) == type)
            {
                if (columnName.Equals("DiscountId"))
                    columnName = "Discount_Id";
                if (columnName.Equals("EntityId"))
                    columnName = "Manufacturer_Id";
            }

            if (typeof(DiscountProductMapping) == type)
            {
                if (columnName.Equals("DiscountId"))
                    columnName = "Discount_Id";
                if (columnName.Equals("EntityId"))
                    columnName = "Product_Id";
            }

            if (typeof(CustomerAddressMapping) == type)
            {
                if (columnName.Equals("AddressId"))
                    columnName = "Address_Id";
                if (columnName.Equals("CustomerId"))
                    columnName = "Customer_Id";
            }

            if (typeof(ShippingMethodCountryMapping) == type)
            {
                if (columnName.Equals("ShippingMethodId"))
                    columnName = "ShippingMethod_Id";
                if (columnName.Equals("CountryId"))
                    columnName = "Country_Id";
            }

            return columnName;
        }

        #endregion
    }
}
