namespace Nop.Plugin.Api.Rest.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Sku { get; set; }
        public decimal Price { get; set; }
    }
}
