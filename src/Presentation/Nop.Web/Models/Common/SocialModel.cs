using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record SocialModel : BaseNopModel
{
    public string FacebookLink { get; set; }
    public string XLink { get; set; }
    public string YoutubeLink { get; set; }
    public string InstagramLink { get; set; }
    public int WorkingLanguageId { get; set; }
    public string TiktokLink { get; set; }
}