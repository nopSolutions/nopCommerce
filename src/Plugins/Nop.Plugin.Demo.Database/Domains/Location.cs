using Nop.Core;

namespace Nop.Plugin.Demo.Database.Domains
{
    public class Location : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int LocationCategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}