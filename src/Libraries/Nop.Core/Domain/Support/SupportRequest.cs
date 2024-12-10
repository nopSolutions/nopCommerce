using System.ComponentModel.DataAnnotations;

namespace Nop.Core.Domain.Support;

public enum StatusEnum
{
    AwaitingSupport,
    AwaitingCustomer,
    Resolved
}

public class SupportRequest : BaseEntity
{
    public int CustomerId { get; set; }
    public StatusEnum Status { get; set; }
    
    [Required(ErrorMessage = "Subject must be between 10 and 100 characters.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Subject must be between 10 and 100 characters.")]
    public string Subject { get; set; }
    public bool Read { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
}