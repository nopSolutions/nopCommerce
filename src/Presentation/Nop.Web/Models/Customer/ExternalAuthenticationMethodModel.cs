using System.Web.Routing;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public class ExternalAuthenticationMethodModel : BaseNopModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}