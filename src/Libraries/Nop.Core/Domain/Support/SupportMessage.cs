namespace Nop.Core.Domain.Support;

public class SupportMessage : BaseEntity
{
    public int RequestId { get; set; }
    public int AuthorId { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public string Message { get; set; }
}