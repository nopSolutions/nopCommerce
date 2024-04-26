using Nop.Web.Framework.Models;

namespace Nop.Web.Models.News;

public partial record NewsItemModel : BaseNopEntityModel
{
    public NewsItemModel()
    {
        Comments = new List<NewsCommentModel>();
        AddNewComment = new AddNewsCommentModel();
    }

    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string SeName { get; set; }

    public string Title { get; set; }
    public string Short { get; set; }
    public string Full { get; set; }
    public bool AllowComments { get; set; }
    public bool PreventNotRegisteredUsersToLeaveComments { get; set; }
    public int NumberOfComments { get; set; }
    public DateTime CreatedOn { get; set; }

    public IList<NewsCommentModel> Comments { get; set; }
    public AddNewsCommentModel AddNewComment { get; set; }
}