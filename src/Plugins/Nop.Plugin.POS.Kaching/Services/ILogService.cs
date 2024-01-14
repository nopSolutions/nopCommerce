using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Nop.Plugin.POS.Kaching.Services
{
    public interface ILogService
    {
        Task<string> LogRawJsonAsync(string jsonString, string fileNameExtension);

        Task<IEnumerable<FileInfo>> GetRawJsonFileAsync(DateTime fromDate, DateTime toDate);

        void RenameFile(string accountingFileName, double reconciliationTime);
    }
}