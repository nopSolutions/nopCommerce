using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Factories
{
    public partial interface IProductModelFactory
    {
        string PrepareProductTemplateViewPath(Product product);

        IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false);

        ProductDetailsModel PrepareProductDetailsModel(Product product, ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false);

        ProductReviewsModel PrepareProductReviewsModel(ProductReviewsModel model, Product product);
        
        CustomerProductReviewsModel PrepareCustomerProductReviewsModel(int? page);

        ProductEmailAFriendModel PrepareProductEmailAFriendModel(ProductEmailAFriendModel model, Product product, bool excludeProperties);

        IList<ProductSpecificationModel> PrepareProductSpecificationModel(Product product);
    }
}
