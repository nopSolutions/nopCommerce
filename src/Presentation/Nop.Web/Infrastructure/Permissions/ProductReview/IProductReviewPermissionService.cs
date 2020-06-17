using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;

namespace Nop.Web.Infrastructure.Permissions.ProductReview
{
    public interface IProductReviewPermissionService
    {
        bool CanAdd(Customer customer, Product product, Store store);
    }
}