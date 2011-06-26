using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Security
{
    public class PermissionRecordModel : BaseNopModel
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
    }
}