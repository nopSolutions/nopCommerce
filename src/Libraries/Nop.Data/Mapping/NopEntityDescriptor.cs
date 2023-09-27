namespace Nop.Data.Mapping
{
    public partial class NopEntityDescriptor
    {
        public NopEntityDescriptor()
        {
            Fields = new List<NopEntityFieldDescriptor>();
        }

        public string EntityName { get; set; }
        public string SchemaName { get; set; }
        public ICollection<NopEntityFieldDescriptor> Fields { get; set; }
    }
}