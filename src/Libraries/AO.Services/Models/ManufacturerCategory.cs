using Nop.Core;

namespace AO.Services.Models
{
    public class ManufacturerCategory : BaseEntity
    {
        public int ManufacturerId { get; set; }

        public int CategoryId { get; set; }

        public string ManufacturerName { get; set; }

        public string ManufacturerSeoName { get; set; }

        public string CategoryName { get; set; }

        public string CategorySeoName { get; set; }       

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }
    }
}