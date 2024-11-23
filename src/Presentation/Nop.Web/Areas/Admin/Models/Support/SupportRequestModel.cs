using Nop.Core.Domain.Support;

namespace Nop.Web.Areas.Admin.Models.Support;

public class SupportRequestModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public StatusEnum Status { get; set; }
    public string Subject { get; set; }
    public bool Read { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
}