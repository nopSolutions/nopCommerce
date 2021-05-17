using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.BuyAmScraper.Models
{
    public record ProductDTO
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public byte[] Image { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
    }
}