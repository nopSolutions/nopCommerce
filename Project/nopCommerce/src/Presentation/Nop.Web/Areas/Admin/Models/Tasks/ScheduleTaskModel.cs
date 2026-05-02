using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Tasks;

/// <summary>
/// Represents a schedule task model
/// </summary>
public partial record ScheduleTaskModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Admin.System.ScheduleTasks.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.System.ScheduleTasks.Seconds")]
    public int Seconds { get; set; }

    [NopResourceDisplayName("Admin.System.ScheduleTasks.Enabled")]
    public bool Enabled { get; set; }

    [NopResourceDisplayName("Admin.System.ScheduleTasks.StopOnError")]
    public bool StopOnError { get; set; }

    [NopResourceDisplayName("Admin.System.ScheduleTasks.LastStart")]
    public string LastStartUtc { get; set; }

    [NopResourceDisplayName("Admin.System.ScheduleTasks.LastEnd")]
    public string LastEndUtc { get; set; }

    [NopResourceDisplayName("Admin.System.ScheduleTasks.LastSuccess")]
    public string LastSuccessUtc { get; set; }

    #endregion
}