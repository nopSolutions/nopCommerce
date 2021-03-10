using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Customer
{
    public partial record ExternalAuthenticationMethodModel : BaseNopModel
    {
        public string ViewComponentName { get; set; }
    }
}