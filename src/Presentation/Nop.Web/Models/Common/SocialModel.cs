using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Models.Common
{
    public partial class SocialModel : BaseNopModel
    {
        public string FacebookLink { get; set; }
        public string TwitterLink { get; set; }
        public string YoutubeLink { get; set; }
        public string GooglePlusLink { get; set; }
        public int WorkingLanguageId { get; set; }
        public bool NewsEnabled { get; set; }
    }
}