using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Common
{
    public class MenuModel : BaseNopModel
    {
        public bool BlogEnabled { get; set; }
        public bool RecentlyAddedProductsEnabled { get; set; }
        public bool ForumEnabled { get; set; }
    }
}