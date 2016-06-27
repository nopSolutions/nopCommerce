using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    public partial class ModeModel : BaseNopModel
    {
        public string ModeName { get; set; }
        public bool Enabled { get; set; }
    }
}