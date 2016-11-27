using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Factories
{
    public partial interface IProductModelFactory
    {
        IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false);

        IList<ProductSpecificationModel> PrepareProductSpecificationModel(Product product);
        
        ProductDetailsModel PrepareProductDetailsPageModel(Product product, ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false);

        ProductReviewOverviewModel PrepareProductReviewOverviewModel(Product product);

        ProductReviewsModel PrepareProductReviewsModel(ProductReviewsModel model, Product product);

        CustomerProductReviewsModel PrepareCustomerProductReviewsModel(int? page);

        ProductEmailAFriendModel PrepareProductEmailAFriendModel(ProductEmailAFriendModel model, Product product, bool excludeProperties);
    }
}
