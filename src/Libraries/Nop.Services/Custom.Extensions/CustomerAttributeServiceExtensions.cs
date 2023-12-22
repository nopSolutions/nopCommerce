using Nop.Core;
using Nop.Core.Domain.Attributes;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Attributes
{
    public partial interface IAttributeService<TAttribute, TAttributeValue>
        where TAttribute : BaseAttribute
        where TAttributeValue : BaseAttributeValue
    {
        Task<IList<TAttributeValue>> GetCustomerAttributeValuesAsync(List<int> customerAttributeValueIds);

        Task<IList<CustomerAttributeValue>> GetCustomCustomerAttributeValuesAsync(int attributeId);
    }

    public partial class AttributeService<TAttribute, TAttributeValue> : IAttributeService<TAttribute, TAttributeValue>
        where TAttribute : BaseAttribute
        where TAttributeValue : BaseAttributeValue
    {

        #region Fields

        #endregion

        #region Ctor

        #endregion

        #region Methods

        public virtual async Task<IList<TAttributeValue>> GetCustomerAttributeValuesAsync(List<int> customerAttributeValueIds)
        {

            var query = from cav in _attributeValueRepository.Table
                        orderby cav.DisplayOrder, cav.Id
                        where customerAttributeValueIds.Contains(cav.AttributeId)
                        select cav;

            var customerAttributeValues = await query.ToListAsync();
            return customerAttributeValues;
        }

        async Task<IList<CustomerAttributeValue>> IAttributeService<TAttribute, TAttributeValue>.GetCustomCustomerAttributeValuesAsync(int attributeId)
        {
            var specificationAttributeService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            var specAttributeOptions = await specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(attributeId);

            var customAttributes = new List<CustomerAttributeValue>();
            foreach (var item in specAttributeOptions)
            {
                var value = new CustomerAttributeValue
                {
                    Id = item.Id,
                    AttributeId = item.SpecificationAttributeId,
                    Name = item.Name,
                    DisplayOrder = item.DisplayOrder,
                    IsPreSelected = false
                };

                customAttributes.Add(value);
            }

            return customAttributes;
        }

        #endregion
    }
}
