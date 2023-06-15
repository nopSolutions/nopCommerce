using Nop.Core;

namespace Nop.Plugin.Widgets.Deals.Domain;

public class TireDealEntity : BaseEntity
{
    public string Title { get; set; }
    public string LongDescription { get; set; }
    public string ShortDescription { get; set; }
    public bool IsActive { get; set; }
}