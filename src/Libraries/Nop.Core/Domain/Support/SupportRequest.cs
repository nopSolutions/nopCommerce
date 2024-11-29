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
    public string Subject { get; set; }
    public bool Read { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
}