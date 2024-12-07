using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Support;

namespace Nop.Web.Models.Support;

public class SupportChatViewModel
{
    public int RequestId { get; set; }
    public string Subject { get; set; }
    public StatusEnum Status { get; set; }
    public List<SupportMessageModel> Messages { get; set; }
    
    [Required(ErrorMessage = "Message must be between 10 and 1000 characters.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 1000 characters.")]
    public string NewMessage { get; set; }
}