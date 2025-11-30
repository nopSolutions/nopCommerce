using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Infrastructure;
using Nop.Services.Vendors;

namespace Nop.Plugin.Api.Attributes
{
    public class ValidateVendor : BaseValidationAttribute
    {
        private readonly Dictionary<string, string> _errors;

        private IVendorService _vendorService;

        public ValidateVendor()
        {
            _errors = new Dictionary<string, string>();
        }

        private IVendorService VendorService => _vendorService ?? (_vendorService = EngineContext.Current.Resolve<IVendorService>());

        public override async Task ValidateAsync(object instance)
        {
            if (instance != null && int.TryParse(instance.ToString(), out var vendorId))
            {
                if (vendorId > 0)
                {
                    var vendor = await VendorService.GetVendorByIdAsync(vendorId);

                    if (vendor == null)
                    {
                        _errors.Add("Invalid vendor id", "Non existing vendor");
                    }
                }
            }
        }

        public override Dictionary<string, string> GetErrors()
        {
            return _errors;
        }
    }
}
