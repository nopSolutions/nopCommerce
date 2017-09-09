using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Common
{
    public partial class UrlRecordModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.System.SeNames.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.System.SeNames.EntityId")]
        public int EntityId { get; set; }

        [NopResourceDisplayName("Admin.System.SeNames.EntityName")]
        public string EntityName { get; set; }

        [NopResourceDisplayName("Admin.System.SeNames.IsActive")]
        public bool IsActive { get; set; }

        [NopResourceDisplayName("Admin.System.SeNames.Language")]
        public string Language { get; set; }

        [NopResourceDisplayName("Admin.System.SeNames.Details")]
        public string DetailsUrl { get; set; }
    }
}