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
            var added = Added();
            if (!added.Contains(categoryProduct)) added.Add(categoryProduct);
            Removed().Remove(categoryProduct);
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

        public static void MergeList(IList<CategoryProductModel> categoryProducts)
        {
            var added = Added();
            var removed = Removed();
            foreach (var categoryProduct in categoryProducts)
            {
                if (added.Contains(categoryProduct))
                    added.Remove(categoryProduct);
            }
            foreach (var toAdd in added)
            {
                categoryProducts.Add(toAdd);
            }
            foreach (var toRemove in removed)
            {
                categoryProducts.Remove(toRemove);
            }
        }
    }
}
