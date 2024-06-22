using DocumentFormat.OpenXml.Spreadsheet;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Helpers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Web.Models.Boards;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Customer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{

    public partial interface ICustomerModelFactory
    {
        Task<IList<CustomerInfoModel>> PrepareAffiliatedCustomersModelAsync(Customer customer);
    }

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

        public virtual async Task<IList<CustomerInfoModel>> PrepareAffiliatedCustomersModelAsync(Customer customer)
        {
            var model = new List<CustomerInfoModel>() { };
            var affiliateId = customer.VatNumberStatusId;

            if (affiliateId > 0)
            {
                //get customers
                var customers = await _customerService.GetAllCustomersAsync(affiliateId: affiliateId, pageIndex: 0, pageSize: 1000);

                foreach (var item in customers)
                {
                    model.Add(new CustomerInfoModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Email = (await _dateTimeHelper.ConvertToUserTimeAsync(item.CreatedOnUtc, DateTimeKind.Utc)).ToString()
                    });
                }
            }

            return model;
        }

        #endregion
    }
}

namespace Nop.Web.Areas.Admin.Factories
{

    public partial class CustomerModelFactory
    {
        #region methods

        protected virtual async Task<IList<SpecificationAttributeOption>> GetCustomCustomerAttributeValuesAsync(int attributeId)
        {
            var specificationAttributeService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            return await specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(attributeId);
        }

        #endregion
    }
}
