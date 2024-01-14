using System.ComponentModel.DataAnnotations;
using Nop.Core;

namespace AO.Services.Orders.Models
{
    public class AOProductAttributeValue : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int ProductAttributeMappingId { get; set; }
        public string Name { get; set; }
    }
}
