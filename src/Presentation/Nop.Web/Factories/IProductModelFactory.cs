using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Factories
{
    public partial interface IProductModelFactory
    {
        ProductDetailsModel PrepareProductDetailsPageModel(Product product, ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false);

        ProductReviewsModel PrepareProductReviewsModel(ProductReviewsModel model, Product product);

        CustomerProductReviewsModel PrepareCustomerProductReviewsModel(int? page);

        ProductEmailAFriendModel PrepareProductEmailAFriendModel(ProductEmailAFriendModel model, Product product, bool excludeProperties);
    }
}
