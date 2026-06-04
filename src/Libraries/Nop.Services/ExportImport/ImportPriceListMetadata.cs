using Nop.Core.Domain.PriceLists;
using Nop.Services.ExportImport.Help;

namespace Nop.Services.ExportImport;

public partial class ImportPriceListMetadata
{
    public int EndRow { get; internal set; }

    public PropertyManager<PriceList> Manager { get; internal set; }

    public IList<PropertyByName<PriceList>> Properties { get; set; }

    public int CountPriceListsInFile { get; set; }

    public PropertyManager<PriceListItem> PriceListItemManager { get; internal set; }
}