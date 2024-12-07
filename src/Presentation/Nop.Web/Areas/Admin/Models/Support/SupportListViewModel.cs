#nullable enable
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Support;

namespace Nop.Web.Areas.Admin.Models.Support;

public class SupportListViewModel
{
    public List<SupportRequestModel>? Requests { get; set; }
    
    public bool HasPreviousPage { get; set; }
    
    public bool HasNextPage { get; set; }
    
    public int TotalPages { get; set; }
    
    public int CurrentPage { get; set; }
    
    public int PageSize { get; set; }

    public static List<SelectListItem> SortOptions { get; } = new()
    {
        new SelectListItem() { Text = "Oldest", Value = "date_asc" },
        new SelectListItem() { Text = "Newest", Value = "date_dsc" },
    };
    
    public string? SelectedSortOption { get; set; }
    
    public static List<SelectListItem> AvailableStatuses { get; private set; } = GetAvailableStatuses();
    
    public string? FilterByStatus { get; set; }
    
    public string? SearchTerm { get; set; }
    
    private static List<SelectListItem> GetAvailableStatuses()
    {
        var availableStatuses = new List<SelectListItem>();
        var enumValues = Enum.GetValues(typeof(StatusEnum));

        foreach (var status in enumValues)
        {
            availableStatuses.Add(new SelectListItem(status.ToString(), status.ToString()));
        }
        
        // Empty status to allow removal of filter
        availableStatuses.Add(new SelectListItem("", ""));
        
        return availableStatuses;
    }
}