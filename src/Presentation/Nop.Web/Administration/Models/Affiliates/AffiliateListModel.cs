using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Affiliates
{
    public partial class AffiliateListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Affiliates.List.SearchFirstName")]
        [AllowHtml]
        public string SearchFirstName { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.List.SearchLastName")]
        [AllowHtml]
        public string SearchLastName { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.List.SearchFriendlyUrlName")]
        [AllowHtml]
        public string SearchFriendlyUrlName { get; set; }
    }
}