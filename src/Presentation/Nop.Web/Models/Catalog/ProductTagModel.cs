using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record ProductTagModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public string SeName { get; set; }

    public int ProductCount { get; set; }
}