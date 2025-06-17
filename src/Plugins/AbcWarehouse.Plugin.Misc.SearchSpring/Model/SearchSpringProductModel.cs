using System.Collections.Generic;

namespace AbcWarehouse.Plugin.Misc.SearchSpring.Models
{
    public class SearchSpringProductModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Price { get; set; }
        public string ProductUrl { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string ItemNumber { get; set; }
        public string RetailPrice { get; set; }
        public string Sku { get; set; }
    }

    public class SearchSpringResponse
    {
        public List<SearchSpringProductModel> Results { get; set; }
    }
}
