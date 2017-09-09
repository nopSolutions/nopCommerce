using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Security
{
    public partial class PermissionRecordModel : BaseNopModel
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
    }
}