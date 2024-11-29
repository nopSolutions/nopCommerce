using Nop.Core.Domain.Support;

namespace Nop.Web.Models.Support;

public class SupportMessageModel
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public int AuthorId { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public string Message { get; set; }

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