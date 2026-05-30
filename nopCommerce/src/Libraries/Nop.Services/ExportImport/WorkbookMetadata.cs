using ClosedXML.Excel;
using Nop.Services.ExportImport.Help;

namespace Nop.Services.ExportImport;

public partial class WorkbookMetadata<T>
{
    public List<PropertyByName<T>> DefaultProperties { get; set; }

    public List<PropertyByName<T>> LocalizedProperties { get; set; }

    public IXLWorksheet DefaultWorksheet { get; set; }

    public List<IXLWorksheet> LocalizedWorksheets { get; set; }
}