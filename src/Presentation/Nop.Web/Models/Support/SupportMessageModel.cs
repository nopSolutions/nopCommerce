using Nop.Core.Domain.Support;

namespace Nop.Web.Models.Support;

public class SupportMessageModel
{
    public int Id { get; private set; }
    public int RequestId { get; private set; }
    public int AuthorId { get; private set; }
    public bool IsAdmin { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public string Message { get; private set; }

    public SupportMessageModel() { }

    public SupportMessageModel(SupportMessage supportMessage)
    {
        Id = supportMessage.Id;
        RequestId = supportMessage.RequestId;
        AuthorId = supportMessage.AuthorId;
        IsAdmin = supportMessage.IsAdmin;
        CreatedOnUtc = supportMessage.CreatedOnUtc;
        Message = supportMessage.Message;
    }
}