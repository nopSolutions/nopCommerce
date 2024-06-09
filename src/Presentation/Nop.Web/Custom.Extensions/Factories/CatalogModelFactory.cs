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

    }
}
