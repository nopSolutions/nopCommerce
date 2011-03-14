using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public interface IImportManager
    {
        /// <summary>
        /// Import customer list from XLS file
        /// </summary>
        /// <param name="filePath">Excel file path</param>
        void ImportCustomersFromXls(string filePath);

        /// <summary>
        /// Import products from XLS file
        /// </summary>
        /// <param name="filePath">Excel file path</param>
        void ImportProductsFromXls(string filePath);
    }
}
