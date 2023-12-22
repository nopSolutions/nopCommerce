using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Web.Models.Customer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{

    public partial class CustomerModelFactory
    {
        #region methods

        protected virtual void CustomizeCustomerNavigationItemsAsync(CustomerNavigationModel model)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "PrivateMessages",
                Title = "Mails and Messages ",
                Tab = (int)CustomerNavigationEnum.PrivateMessages,
                ItemClass = "customer-PrivateMessages"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "ShortListed",
                Title = "Short Listed",
                Tab = (int)CustomerNavigationEnum.ShortListed,
                ItemClass = "customer-shortlisted"
            });

            //remove address item
            model.CustomerNavigationItems.RemoveAt(1);

            //sort by name
        }

        protected virtual async Task<IList<SpecificationAttributeOption>> GetCustomCustomerAttributeValuesAsync(int attributeId)
        {
            var specificationAttributeService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            return await specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(attributeId);
        }

        #endregion
    }
}
