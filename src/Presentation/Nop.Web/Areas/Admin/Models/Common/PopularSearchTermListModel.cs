using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents a popular search term list model
    /// </summary>
    public partial record PopularSearchTermListModel : BasePagedListModel<PopularSearchTermModel>
    {
    }
}