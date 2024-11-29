using Nop.Core.Domain.Support;

namespace Nop.Web.Areas.Admin.Models.Support;

public class SupportRequestModel
{
    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public StatusEnum Status { get; private set; }
    public string Subject { get; set; }
    public bool Read { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime UpdatedOnUtc { get; private set; }
    
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