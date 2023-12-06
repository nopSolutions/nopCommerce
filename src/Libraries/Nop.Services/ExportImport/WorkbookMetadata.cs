using ClosedXML.Excel;
using Nop.Core.Domain.Localization;
using Nop.Services.ExportImport.Help;

namespace Nop.Services.ExportImport
{
    public class WorkbookMetadata<T>
    {
        public List<PropertyByName<T, Language>> DefaultProperties { get; set; }

        public List<PropertyByName<T, Language>> LocalizedProperties { get; set; }

        public IXLWorksheet DefaultWorksheet { get; set; }

        public List<IXLWorksheet> LocalizedWorksheets { get; set; }
    }
}
