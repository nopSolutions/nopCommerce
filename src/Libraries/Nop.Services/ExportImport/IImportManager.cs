
namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public interface IImportManager
    {
        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="filePath">Excel file path</param>
        void ImportProductsFromXlsx(string filePath);
    }
}
