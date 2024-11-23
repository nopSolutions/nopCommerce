namespace Nop.Web.Areas.Admin.Models.Support;

public class SupportMessageModel
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public int AuthorId { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public string Message { get; set; }
}