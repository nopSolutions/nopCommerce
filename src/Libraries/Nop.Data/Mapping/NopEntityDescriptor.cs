using System.Collections.Generic;

namespace Nop.Data.Mapping
{
    public class NopEntityDescriptor
    {
        public NopEntityDescriptor()
        {
            Fields = new List<NopEntityFieldDescriptor>();
        }

        public string EntityName { get; set; }
        public ICollection<NopEntityFieldDescriptor> Fields { get; set; }
    }
}