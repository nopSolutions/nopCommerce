using Nop.Core.Domain.Customers;
using Nop.Web.Models.Profile;

namespace Nop.Web.Factories
{
    public partial interface IProfileModelFactory
    {
        ProfileIndexModel PrepareProfileIndexModel(Customer customer, int? page);

        ProfileInfoModel PrepareProfileInfoModel(Customer customer);

        ProfilePostsModel PrepareProfilePostsModel(Customer customer, int page);
    }
}
