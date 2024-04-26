using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Forums;

/// <summary>
/// Represents a forum list model
/// </summary>
public partial record ForumModel : BaseNopEntityModel
{
    #region Ctor

    public ForumModel()
    {
        ForumGroups = new List<ForumGroupModel>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.ForumGroupId")]
    public int ForumGroupId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    public List<ForumGroupModel> ForumGroups { get; set; }

    #endregion
}