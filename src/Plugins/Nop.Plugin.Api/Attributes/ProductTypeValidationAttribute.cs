using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Api.Attributes
{
    public class ProductTypeValidationAttribute : BaseValidationAttribute
    {
        private readonly Dictionary<string, string> _errors = new Dictionary<string, string>();

        public override Task ValidateAsync(object instance)
        {
            // Product Type is not required so it could be null
            // and there is nothing to validate in this case
            if (instance == null)
            {
                return Task.CompletedTask;
            }

            var isDefined = Enum.IsDefined(typeof(ProductType), instance);

            if (!isDefined)
            {
                _errors.Add("ProductType", "Invalid product type");
            }

            return Task.CompletedTask;
        }

        public override Dictionary<string, string> GetErrors()
        {
            return _errors;
        }
    }
}
