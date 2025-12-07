using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Areas.Admin.Models.Services;

/// <summary>
/// Represents the Services Provided configuration model.
/// </summary>
public partial record ServicesProvidedModel
{
    [Display(Name = "Serialized services payload")]
    public string ServicesJson { get; set; }

    /// <summary>
    /// Starter JSON the UI can use when a tenant wants to prefill example content.
    /// </summary>
    public string SampleJson { get; set; }
}
