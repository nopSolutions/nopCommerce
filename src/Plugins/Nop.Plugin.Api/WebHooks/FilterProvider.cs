//using System.Collections.ObjectModel;
//using System.Threading.Tasks;
//using Nop.Plugin.Api.Constants;

//namespace Nop.Plugin.Api.WebHooks
//{
//    using Microsoft.AspNet.WebHooks;

//    public class FilterProvider : IWebHookFilterProvider
//    {
//        private readonly Collection<WebHookFilter> filters = new Collection<WebHookFilter>
//    {
//        new WebHookFilter { Name = WebHookNames.CustomersCreate, Description = "A customer has been registered."},
//        new WebHookFilter { Name = WebHookNames.CustomersUpdate, Description = "A customer has been updated."},
//        new WebHookFilter { Name = WebHookNames.CustomersDelete, Description = "A customer has been deleted."},
//        new WebHookFilter { Name = WebHookNames.ProductsCreate, Description = "A product has been created."},
//        new WebHookFilter { Name = WebHookNames.ProductsUpdate, Description = "A product has been updated."},
//        new WebHookFilter { Name = WebHookNames.ProductsDelete, Description = "A product has been deleted."},
//        new WebHookFilter { Name = WebHookNames.ProductsUnmap, Description = "A product has been unmapped from the store."},
//        new WebHookFilter { Name = WebHookNames.CategoriesCreate, Description = "A category has been created."},
//        new WebHookFilter { Name = WebHookNames.CategoriesUpdate, Description = "A category has been updated."},
//        new WebHookFilter { Name = WebHookNames.CategoriesDelete, Description = "A category has been deleted."},
//        new WebHookFilter { Name = WebHookNames.CategoriesUnmap, Description = "A category has been unmapped from the store."},
//        new WebHookFilter { Name = WebHookNames.OrdersCreate, Description = "An order has been created."},
//        new WebHookFilter { Name = WebHookNames.OrdersUpdate, Description = "An order has been updated."},
//        new WebHookFilter { Name = WebHookNames.OrdersDelete, Description = "An order has been deleted."},
//        new WebHookFilter { Name = WebHookNames.ProductCategoryMapsCreate, Description = "A product category map has been created."},
//        new WebHookFilter { Name = WebHookNames.ProductCategoryMapsUpdate, Description = "A product category map has been updated."},
//        new WebHookFilter { Name = WebHookNames.ProductCategoryMapsDelete, Description = "A product category map has been deleted."},
//        new WebHookFilter { Name = WebHookNames.StoresUpdate, Description = "A store has been updated."},
//        new WebHookFilter { Name = WebHookNames.LanguagesCreate, Description = "A language has been created."},
//        new WebHookFilter { Name = WebHookNames.LanguagesUpdate, Description = "A language has been updated."},
//        new WebHookFilter { Name = WebHookNames.LanguagesDelete, Description = "A language has been deleted."},
//        new WebHookFilter { Name = WebHookNames.NewsLetterSubscriptionCreate, Description = "A news letter subscription has been created."},
//        new WebHookFilter { Name = WebHookNames.NewsLetterSubscriptionUpdate, Description = "A news letter subscription has been updated."},
//        new WebHookFilter { Name = WebHookNames.NewsLetterSubscriptionDelete, Description = "A news letter subscription has been deleted."}
//    };

//        public Task<Collection<WebHookFilter>> GetFiltersAsync()
//        {
//            return Task.FromResult(this.filters);
//        }
//    }
//}
