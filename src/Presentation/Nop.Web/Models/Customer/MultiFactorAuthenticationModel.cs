using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Customer
{
    public partial record MultiFactorAuthenticationModel : BaseNopModel
    {
        public MultiFactorAuthenticationModel()
        {
            Providers = new List<MultiFactorAuthenticationProviderModel>();
        }

        [NopResourceDisplayName("Account.MultiFactorAuthentication.Fields.IsEnabled")]
        public bool IsEnabled { get; set; }

        public List<MultiFactorAuthenticationProviderModel> Providers { get; set; }

        public string Message { get; set; }
        
    }
}
