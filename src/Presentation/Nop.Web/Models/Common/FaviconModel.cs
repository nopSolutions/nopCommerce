using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Common
{
    public class FaviconModel : BaseNopModel
    {
        public bool Uploaded { get; set; }
        public string FaviconUrl { get; set; }
    }
}