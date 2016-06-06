using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Common
{
    public partial class AdminHeaderLinksModel : BaseNopModel
    {
        public string ImpersonatedCustomerEmailUsername { get; set; }
        public bool IsCustomerImpersonated { get; set; }
        public bool DisplayAdminLink { get; set; }
        public string EditPageUrl { get; set; }
    }
}