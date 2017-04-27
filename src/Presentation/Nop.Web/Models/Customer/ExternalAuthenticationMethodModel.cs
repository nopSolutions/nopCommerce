using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Models.Customer
{
    public partial class ExternalAuthenticationMethodModel : BaseNopModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}