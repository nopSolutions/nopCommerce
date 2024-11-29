using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Web.Areas.Admin.Models.Support;

public class SupportListViewModel
{
    public List<SupportRequestModel> Requests { get; set; }
    public List<SelectListItem> SortOptions { get; } = new List<SelectListItem>()
    {
        new SelectListItem() { Text = "Date Created Asc.", Value = "date_asc" },
        new SelectListItem() { Text = "Date Created Dsc.", Value = "date_dsc" },
    };
    public string SelectedSortOption { get; set; }
}