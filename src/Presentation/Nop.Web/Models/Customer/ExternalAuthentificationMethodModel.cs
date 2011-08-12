using System.Web.Routing;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public class ExternalAuthentificationMethodModel : BaseNopModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}