using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Common
{
    public partial class FaviconModel : BaseNopModel
    {
        public bool Uploaded { get; set; }
        public string FaviconUrl { get; set; }
    }
}