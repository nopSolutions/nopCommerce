using Nop.Core;

namespace Nop.Plugin.Misc.AbcCore.Mattresses.Domain
{
    public class AbcMattressModel : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ManufacturerId { get; set; }
        public string Comfort { get; set; }
        public int? ProductId { get; set; }
        public int? BrandCategoryId { get; set; }
        public string Sku { get; set; }
    }
}