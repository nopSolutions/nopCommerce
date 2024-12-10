using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Support;

namespace Nop.Web.Areas.Admin.Models.Support;

public class SupportChatViewModel
{
    public int RequestId { get; set; }
    
    public string Subject { get; set; }
    
    public StatusEnum Status { get; set; }
    
    public string NewStatus { get; set; }
    
    public static List<SelectListItem> AvailableStatuses { get; private set; } = GetAvailableStatuses();
    
    public List<SupportMessage> Messages { get; set; }
    
    [Required(ErrorMessage = "Message must be between 10 and 1000 characters.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 1000 characters.")]
    public string NewMessage { get; set; }
    
    private static List<SelectListItem> GetAvailableStatuses()
    {
        var availableStatuses = new List<SelectListItem>();
        var enumValues = Enum.GetValues(typeof(StatusEnum));
        
        foreach (var status in enumValues)
        {
            var value = (int)status;
            availableStatuses.Add(new SelectListItem(status.ToString(), value.ToString()));
        }
        
        return availableStatuses;
    }
}