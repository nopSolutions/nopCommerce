using Nop.Web.Areas.Admin.Models.Books;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Books;

/// <summary>
/// Represents a blog post list model
/// </summary>
public partial record BookListModel : BasePagedListModel<BookModel>
{
}