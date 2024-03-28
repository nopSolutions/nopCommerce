using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record TopicTagModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public string SeName { get; set; }
    public int TopicCount { get; set; }
}
