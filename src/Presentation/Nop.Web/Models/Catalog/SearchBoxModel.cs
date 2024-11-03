using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record SearchBoxModel : BaseNopModel
{
    public bool AutoCompleteEnabled { get; set; }
    public int AutoCompleteSearchThumbPictureSize { get; set; }
    public bool ShowProductImagesInSearchAutoComplete { get; set; }
    public int SearchTermMinimumLength { get; set; }
    public bool ShowSearchBox { get; set; }
    public bool ShowSearchBoxCategories { get; set; }
    public int SearchCategoryId { get; set; }
    public List<SelectListItem> AvailableCategories { get; set; } = new();
}