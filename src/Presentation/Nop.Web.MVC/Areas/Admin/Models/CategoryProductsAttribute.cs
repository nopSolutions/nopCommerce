using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Web.MVC.Areas.Admin.Models
{
    public class CategoryProductsAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            IStatefulStorage storage = StatefulStorage.PerSession;
            filterContext.ActionParameters["addedCategoryProducts"] = Added();
            filterContext.ActionParameters["removedCategoryProducts"] = Removed();
        }

        public static IList<CategoryProductModel> Added()
        {
            return StatefulStorage.PerSession.GetOrAdd<IList<CategoryProductModel>>("product-categories-added",
                                                                                    () =>
                                                                                    new List<CategoryProductModel>());
        }

        public static IList<CategoryProductModel> Removed()
        {
            return StatefulStorage.PerSession.GetOrAdd<IList<CategoryProductModel>>("product-categories-removed",
                                                                                    () =>
                                                                                    new List<CategoryProductModel>());
        }

        public static void Add(CategoryProductModel categoryProduct)
        {
            Removed().Remove(categoryProduct);
            var added = Added();
            var current = added.SingleOrDefault(x => x.ProductId.Equals(categoryProduct.ProductId));
            if (current == null)
            {
                added.Add(categoryProduct);
            }
            else
            {
                current.DisplayOrder = categoryProduct.DisplayOrder;
                current.IsFeaturedProduct = categoryProduct.IsFeaturedProduct;
            }
        }

        public static void Remove(CategoryProductModel categoryProduct)
        {
            var removed = Removed();
            if (!removed.Contains(categoryProduct)) removed.Add(categoryProduct);
            Added().Remove(categoryProduct);
        }

        public static void Clear()
        {
            Added().Clear();
            Removed().Clear();
        }

        public static List<CategoryProductModel> MakeStateful(List<CategoryProductModel> categoryProducts)
        {
            var added = Added();
            var removed = Removed();

            foreach (var categoryProduct in categoryProducts)
            {
                var productId = categoryProduct.ProductId;
                //If the products exist in the added list, then it was updated and we should update the existing category product
                var updated = added.SingleOrDefault(x => x.ProductId.Equals(productId));
                if (updated != null)
                {
                    categoryProduct.DisplayOrder = updated.DisplayOrder;
                    categoryProduct.IsFeaturedProduct = updated.IsFeaturedProduct;
                }
            }
            foreach (var toAdd in added)
            {
                if (!categoryProducts.Contains(toAdd)) categoryProducts.Add(toAdd);
            }
            foreach (var toRemove in removed)
            {
                categoryProducts.Remove(toRemove);
            }
            return categoryProducts.OrderBy(x => x.DisplayOrder).ToList();
        }
    }
}
