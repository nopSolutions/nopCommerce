using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Api.DTO.Customers
{
    public class CustomerAttributeMappingDto
    {
        public Customer Customer { get; set; }
        public GenericAttribute Attribute { get; set; }
    }
}
