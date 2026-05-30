using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Home;

/// <summary>
/// Represents a nopCommerce news details model
/// </summary>
public partial record NopCommerceNewsDetailsModel : BaseNopModel
{
    #region Properties

    public string Title { get; set; }

    public string Url { get; set; }

    public string Summary { get; set; }

    public DateTimeOffset PublishDate { get; set; }

    #endregion
}