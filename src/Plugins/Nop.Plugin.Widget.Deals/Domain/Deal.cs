using Nop.Core;

namespace Nop.Plugin.Widget.Deals.Domain;

public class Deal : BaseEntity
{
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
}