using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record SocialModel : BaseNopModel
{
    public string FacebookLink { get; set; }
    public string XLink { get; set; }
    public string YoutubeLink { get; set; }
    public string InstagramLink { get; set; }
    public string TikTokLink { get; set; }
    public string SnapchatLink { get; set; }
    public string PinterestLink { get; set; }
    public string TumblrLink { get; set; }
    public string DiscordLink { get; set; }
    public int WorkingLanguageId { get; set; }
}