using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Support;

namespace Nop.Web.Areas.Admin.Models.Support;

public class SupportListViewModel
{
    public List<SupportRequestModel> Requests { get; set; }
    public List<SelectListItem> SortOptions { get; } = new List<SelectListItem>()
    {
        new SelectListItem() { Text = "Oldest", Value = "date_asc" },
        new SelectListItem() { Text = "Newest", Value = "date_dsc" },
    };
    public string SelectedSortOption { get; set; }
    public List<SelectListItem> AvailableStatuses { get; set; }
    public string FilterByStatus { get; set; }
    public string SearchTerm { get; set; }
}