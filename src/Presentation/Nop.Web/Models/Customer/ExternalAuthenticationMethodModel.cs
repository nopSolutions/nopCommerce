using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Models.Customer
{
    public partial class ExternalAuthenticationMethodModel : BaseNopModel
    {
        public string ViewComponentName { get; set; }
        public object ViewComponentArguments { get; set; }
    }
}