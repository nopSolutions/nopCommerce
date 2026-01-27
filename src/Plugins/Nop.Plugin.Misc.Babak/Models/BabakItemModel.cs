using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Babak.Models;

public class BabakItemModel : BaseNopEntityModel
{
    [Required]
    [NopResourceDisplayName("Nop.Plugin.Misc.Babak.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Nop.Plugin.Misc.Babak.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Nop.Plugin.Misc.Babak.Fields.IsActive")]
    public bool IsActive { get; set; }
}
