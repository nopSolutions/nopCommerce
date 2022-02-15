namespace Nop.Web.Models.Api.Order
{
    public class ReOrderResponseApiModel
    {
        public bool Success { get; set; }
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string Message { get; set; }
    }
}