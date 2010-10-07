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
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Security;


namespace NopSolutions.NopCommerce.BusinessLogic.Categories
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns a ProductCategory that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>A ProductCategory that has the specified values; otherwise null</returns>
        public static ProductCategory FindProductCategory(this List<ProductCategory> source,
            int productId, int categoryId)
        {
            foreach (ProductCategory productCategory in source)
                if (productCategory.ProductId == productId && productCategory.CategoryId == categoryId)
                    return productCategory;
            return null;
        }

        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>Sorted categories</returns>
        public static List<Category> SortCategoriesForTree(this List<Category> source, int parentId)
        {
            var result = new List<Category>();

            var temp = source.FindAll(c => c.ParentCategoryId == parentId);
            foreach (var cat in temp)
            {
                result.Add(cat);
                result.AddRange(SortCategoriesForTree(source, cat.CategoryId));
            }
            return result;
        }

        /// <summary>
        /// Filter query results by ACL per object
        /// </summary>
        /// <param name="query">Source query</param>
        /// <param name="context">context</param>
        /// <returns>Result</returns>
        public static IQueryable<Category> WhereAclPerObjectNotDenied(this IQueryable<Category> query, NopObjectContext context)
        {
            List<int> customerRoleIds = new List<int>();
            if (NopContext.Current.User != null)
            {
                //customer roles
                var crQuery = from cr in NopContext.Current.User.CustomerRoles
                              orderby cr.CustomerRoleId
                              select cr.CustomerRoleId;

                //ACL query
                query = query.Where(c => (from acl in context.ACLPerObject
                                          where acl.ObjectId == c.CategoryId &&
                                          acl.ObjectTypeId == (int)ObjectTypeEnum.Category &&
                                          acl.Deny == true &&
                                          //crQuery.Count > 0 &&
                                          crQuery.Contains(acl.CustomerRoleId)
                                          select acl.CustomerRoleId).Count() == 0);
            }

            return query;
        }

        /// <summary>
        /// Is access denied
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="context">context</param>
        /// <returns>true - access is denied; otherwise, false</returns>
        public static bool IsAccessDenied(this Category category, NopObjectContext context)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            bool result = false;
            if (NopContext.Current.User != null)
            {
                //customer roles
                var crQuery = from cr in NopContext.Current.User.CustomerRoles
                              orderby cr.CustomerRoleId
                              select cr.CustomerRoleId;

                //ACL query
                var query = from acl in context.ACLPerObject
                            where acl.ObjectId == category.CategoryId &&
                            acl.ObjectTypeId == (int)ObjectTypeEnum.Category &&
                            acl.Deny == true &&
                            //crQuery.Count > 0 &&
                            crQuery.Contains(acl.CustomerRoleId)
                            select acl.CustomerRoleId;

                result = query.ToList().Count > 0;
            }

            return result;
        }
    }
}
