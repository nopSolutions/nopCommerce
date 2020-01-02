namespace Nop.Services.ExportImport
{
    public class ExportSpecificationAttribute
    {
        public int AttributeTypeId { get; set; }
        public string CustomValue { get; set; }
        public bool AllowFiltering { get; set; }
        public bool ShowOnProductPage { get; set; }
        public int DisplayOrder { get; set; }
        public int SpecificationAttributeOptionId { get; set; }
        public int SpecificationAttributeId { get; set; }
    }
}