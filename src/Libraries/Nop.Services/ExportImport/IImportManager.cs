<<<<<<< HEAD
﻿using System.IO;
using System.Threading.Tasks;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public partial interface IImportManager
    {
        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ImportProductsFromXlsxAsync(Stream stream);

        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of imported subscribers
        /// </returns>
        Task<int> ImportNewsletterSubscribersFromTxtAsync(Stream stream);

        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="writeLog">Indicates whether to add logging</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of imported states
        /// </returns>
        Task<int> ImportStatesFromTxtAsync(Stream stream, bool writeLog = true);

        /// <summary>
        /// Import manufacturers from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ImportManufacturersFromXlsxAsync(Stream stream);

        /// <summary>
        /// Import categories from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ImportCategoriesFromXlsxAsync(Stream stream);

        /// <summary>
        /// Import orders from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ImportOrdersFromXlsxAsync(Stream stream);
    }
}
=======
﻿using System.IO;
using System.Threading.Tasks;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public partial interface IImportManager
    {
        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ImportProductsFromXlsxAsync(Stream stream);

        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of imported subscribers
        /// </returns>
        Task<int> ImportNewsletterSubscribersFromTxtAsync(Stream stream);

        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="writeLog">Indicates whether to add logging</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of imported states
        /// </returns>
        Task<int> ImportStatesFromTxtAsync(Stream stream, bool writeLog = true);

        /// <summary>
        /// Import manufacturers from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ImportManufacturersFromXlsxAsync(Stream stream);

        /// <summary>
        /// Import categories from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ImportCategoriesFromXlsxAsync(Stream stream);

        /// <summary>
        /// Import orders from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ImportOrdersFromXlsxAsync(Stream stream);
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
