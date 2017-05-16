using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Admin.Models.Settings
{
    public partial class ModeModel : BaseNopModel
    {
        public string ModeName { get; set; }
        public bool Enabled { get; set; }
    }
}