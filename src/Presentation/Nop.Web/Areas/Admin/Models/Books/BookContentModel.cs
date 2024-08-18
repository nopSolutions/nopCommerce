using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Books;

/// <summary>
/// Represents a blog content model
/// </summary>
public partial record BookContentModel : BaseNopModel
{
    #region Ctor

    public BookContentModel()
    {
        Book = new BookSearchModel();
        SearchName = new BookSearchModel().SearchName;
    }

    #endregion

    #region Properties

    public string SearchName { get; set; }

    public BookSearchModel Book { get; set; }


    #endregion
}