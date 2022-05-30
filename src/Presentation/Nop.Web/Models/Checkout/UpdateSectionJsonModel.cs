namespace Nop.Web.Models.Checkout
{
    public record UpdateSectionJsonModel
    {
        public string name { get; set; }
        public string html { get; set; }
    }
}