using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.MediaMigration.Models;
public class OldProductMedia
{
    public decimal PM_ID { get; set; }
    public long PM_ProductID { get; set; }
    public string? PM_Picture { get; set; }
    public string? PM_Video { get; set; }
    public decimal PM_MediaTypeID { get; set; }
    public bool PM_Deleted { get; set; }

}
