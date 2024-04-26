using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Templates;

/// <summary>
/// Represents a topic template model
/// </summary>
public partial record TopicTemplateModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Admin.System.Templates.Topic.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.System.Templates.Topic.ViewPath")]
    public string ViewPath { get; set; }

    [NopResourceDisplayName("Admin.System.Templates.Topic.DisplayOrder")]
    public int DisplayOrder { get; set; }

    #endregion
}