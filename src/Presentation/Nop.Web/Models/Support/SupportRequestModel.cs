using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Support;

namespace Nop.Web.Models.Support;

public class SupportRequestModel
{
    public int Id { get; private set; }
    
    public int CustomerId { get; private set; }
    
    public StatusEnum Status { get; private set; }
    
    [Required(ErrorMessage = "Subject must be between 10 and 100 characters.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Subject must be between 10 and 100 characters.")]
    public string Subject { get; set; }
    
    public bool Read { get; private set; }
    
    public DateTime CreatedOnUtc { get; private set; }
    
    public DateTime UpdatedOnUtc { get; private set; }
    
    public SupportRequestModel(){}
    
    public SupportRequestModel(SupportRequest supportRequest)
    {
        Id = supportRequest.Id;
        CustomerId = supportRequest.CustomerId;
        Status = supportRequest.Status;
        Subject = supportRequest.Subject;
        Read = supportRequest.Read;
        CreatedOnUtc = supportRequest.CreatedOnUtc;
        UpdatedOnUtc = supportRequest.UpdatedOnUtc;
    }
}