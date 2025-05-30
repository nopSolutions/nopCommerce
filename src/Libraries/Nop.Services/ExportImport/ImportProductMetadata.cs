using ClosedXML.Excel;
using Nop.Core.Domain.Catalog;
using Nop.Services.ExportImport.Help;

namespace Nop.Services.ExportImport;

public partial class ImportProductMetadata
{
    public int EndRow { get; internal set; }

    public PropertyManager<Product> Manager { get; internal set; }

    public IList<PropertyByName<Product>> Properties { get; set; }

    public int CountProductsInFile => ProductsInFile.Count;

    public PropertyManager<ExportProductAttribute> ProductAttributeManager { get; internal set; }

    public PropertyManager<ExportSpecificationAttribute> SpecificationAttributeManager { get; internal set; }

    public PropertyManager<ExportTierPrice> TierPriceManager { get; internal set; }

    public IXLWorksheet DefaultWorksheet { get; set; }

    public List<IXLWorksheet> LocalizedWorksheets { get; set; }

    public int SkuCellNum { get; internal set; }

    public List<string> AllSku { get; set; }

    public List<int> ProductsInFile { get; set; }
}