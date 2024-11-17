using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
    public partial class CatalogModelFactory
    {
        protected virtual async Task PrepareCatalogProductsCustomAsync(CatalogProductsModel model, IPagedList<ProductCustom> products, bool isFiltering = false)
        {
            if (!string.IsNullOrEmpty(model.WarningMessage))
                return;

            var customer = await _workContext.GetCurrentCustomerAsync();

            if (await _customerService.IsGuestAsync(customer))
            {
                //do not show any profiles to guest customers. show message to login to view the profiles
                model.NoResultMessage = await _localizationService.GetResourceAsync("Catalog.Products.GuestCustomerResult");
                return;
            }

            if (!products.Any() && isFiltering)
                model.NoResultMessage = await _localizationService.GetResourceAsync("Catalog.Products.NoResult");
            else if (!products.Any())
                model.NoResultMessage = await _localizationService.GetResourceAsync("Catalog.Products.NoResult");
            else
            {
                model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();
                model.LoadPagedList(products);
            }
        }

        protected virtual async Task<IPagedList<ProductCustom>> GetCustomProducts(IPagedList<Product> products, CatalogProductsCommand command)
        {
            var customerProfileTypeId = (await _workContext.GetCurrentCustomerAsync()).CustomerProfileTypeId;

            //If customer belongs to give support, show Take support profiles and vice versa
            if (customerProfileTypeId == 1) //give support
                customerProfileTypeId = 2;
            else if (customerProfileTypeId == 2) //take support
                customerProfileTypeId = 1;

            //productids
            var productIdsList = products.Select(x => x.Id).ToList();

            var productsCustomSimple = await _productService.SearchProductsSimpleCustomAsync(
                                        productIds: productIdsList,
                                        profileTypeId: customerProfileTypeId,
                                        customerId: (await _workContext.GetCurrentCustomerAsync()).Id,
                                        orderBy: (ProductSortingEnum)command.OrderBy,
                                        pageIndex: command.PageNumber - 1,
                                        pageSize: _catalogSettings.DefaultCategoryPageSize);

            return productsCustomSimple;
        }

        public virtual async Task<List<CategorySimpleModel>> PrepareCategorySimpleModelsAsync(int rootCategoryId, int loadSubCategories = 1)
        {
            var result = new List<CategorySimpleModel>();

            var store = await _storeContext.GetCurrentStoreAsync();
            //var allCategories = await _categoryService.GetAllCategoriesAsync(storeId: store.Id);
            //var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).OrderBy(c => c.DisplayOrder).ToList();

            //customization start
            var customer = await _workContext.GetCurrentCustomerAsync();
            IEnumerable<Category> categories = null;

            if (!await _customerService.IsGuestAsync(customer))
            {
                var customerProductCategories = await _categoryService.GetProductCategoryIdsAsync(new int[] { customer.VendorId });
                categories = await _categoryService.GetCategoriesByIdsAsync(customerProductCategories.First().Value);
            }
            else
            {
                var allCategories = await _categoryService.GetAllCategoriesAsync(storeId: store.Id);
                categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).OrderBy(c => c.DisplayOrder).ToList();
            }

            var customerProfileTypeId = 0;

            //If customer belongs to give support, hide Take support category and vice versa
            if (customer.CustomerProfileTypeId == 1) //give support
                customerProfileTypeId = 2;
            else if (customer.CustomerProfileTypeId == 2) //take support
                customerProfileTypeId = 1;

            foreach (var category in categories)
            {
                //remove give support/take support when user logged in
                if (category.Id == customerProfileTypeId)
                    continue;

                var categoryModel = new CategorySimpleModel
                {
                    Id = category.Id,
                    Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(category),
                    IncludeInTopMenu = category.IncludeInTopMenu
                };

                //number of products in each category
                if (_catalogSettings.ShowCategoryProductNumber)
                {
                    var categoryIds = new List<int> { category.Id };
                    //include subcategories
                    if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                        categoryIds.AddRange(
                            await _categoryService.GetChildCategoryIdsAsync(category.Id, store.Id));

                    categoryModel.NumberOfProducts =
                        await _productService.GetNumberOfProductsInCategoryAsync(categoryIds, store.Id);
                }

                categoryModel.HaveSubCategories = categoryModel.SubCategories.Count > 0 &
                                                  categoryModel.SubCategories.Any(x => x.IncludeInTopMenu);

                result.Add(categoryModel);
            }

            return result;
        }

    }
}
