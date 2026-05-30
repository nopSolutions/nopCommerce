using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Topics;

public partial record AuthenticatedTopicModel : BaseNopModel
{
    public bool Authenticated { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string Error { get; set; }
}
