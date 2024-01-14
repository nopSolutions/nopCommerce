using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Nop.Plugin.POS.Kaching.Extensions;
using Nop.Services.Logging;

namespace Nop.Plugin.POS.Kaching.Services
{
    public class LogService : ILogService
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _folderPath = "";
        private readonly string _reconciliationFileNameMask = "ReconciliationJson-{0}.txt";


        public LogService(ILogger logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _folderPath = $"{_webHostEnvironment.WebRootPath}\\Logs\\";
        }

        public async Task<IEnumerable<FileInfo>> GetRawJsonFileAsync(DateTime fromDate, DateTime toDate)
        {
            var folderPath = $"{_webHostEnvironment.WebRootPath}\\Logs\\Accounting\\";
            Directory.CreateDirectory(folderPath);
            if (Directory.Exists(folderPath) == false)
            {
                throw new FileNotFoundException($"folderPath '{folderPath}' does not exist");
            }

            var jsonFiles  = GetFilesBetween(folderPath, fromDate, toDate);
            if (jsonFiles == null || jsonFiles.Count() == 0)
            {
                jsonFiles  = GetFilesBetween(folderPath, fromDate.AddDays(-1), DateTime.Now);
            }
            return jsonFiles;
        }

        public async Task<string> LogRawJsonAsync(string jsonString, string fileNameExtension)
        {
            // Ensure folder exists            
            Directory.CreateDirectory(Path.Combine(_folderPath, @"Accounting\"));
            string accountingFileName = "";
            try
            {
                // Ensure folder exists
                _folderPath = $"{_webHostEnvironment.WebRootPath}\\Logs\\";
                Directory.CreateDirectory(Path.Combine(_folderPath, @"Accounting\"));

                // Log to use in accounting (must be clean and raw)
                accountingFileName = $"{fileNameExtension}RawJson-{DateTime.Now:dd-MM-yyyy}-{DateTime.Now.Ticks}.txt";
                File.AppendAllText(Path.Combine(_folderPath, @"Accounting\", accountingFileName), jsonString);                
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Error logging raw json for accounting in KaChing: {ex}");
            }

            try
            {
                // Log for readability
                jsonString = Environment.NewLine + Environment.NewLine + "------------------------- " + DateTime.Now.ToString() + " ------------------------------" + Environment.NewLine + jsonString;
                var fileName = $"{fileNameExtension}LogRawJson-{DateTime.Now:dd-MM-yyyy}.log";
                File.AppendAllText(Path.Combine(_folderPath, fileName), jsonString);

                // Delete old files
                Cleanup(_folderPath);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Error logging raw json in KaChing: {ex}");
            }

            return accountingFileName;
        }

        private static IEnumerable<FileInfo> GetFilesBetween(string path, DateTime start, DateTime end)
        {
            DirectoryInfo di = new(path);
            FileInfo[] files = di.GetFiles()
                             .Where(f => f.Name.Contains("ReconciliationJson") && f.Length > 0)
                             .ToArray();

            return files.Where(f => f.CreationTime.Between(start, end) ||
                                    f.LastWriteTime.Between(start, end));
        }

        private void Cleanup(string folderPath)
        {
            var files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                var fi = new FileInfo(file);

                if (fi.LastAccessTime < DateTime.Now.AddYears(-2))
                {
                    fi.Delete();
                }
            }
        }

        public void RenameFile(string accountingFileName, double reconciliationTime)
        {
            string sourceFileName = "", destinationFileName = "";
            try
            {
                string path = Path.Combine(_folderPath, @"Accounting\");
                sourceFileName = Path.Combine(path, accountingFileName);
                destinationFileName = Path.Combine(path, string.Format(_reconciliationFileNameMask, $"{UnixTimeStampToDateTime((long)reconciliationTime)}"));
                File.Move(@sourceFileName, @destinationFileName, true);
            }
            catch (IOException ex)
            {
                _logger.ErrorAsync($"IOException: {ex.Message}{Environment.NewLine}SourceFileName: '{@sourceFileName}'{Environment.NewLine}DestinationFileName: '{@destinationFileName}'{Environment.NewLine}{Environment.NewLine}{ex}");
                throw;
            }
        }

        private string UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime.ToString("yy-MM-dd_hh:MM:ss").Replace(".", "-").Replace(":", "-");
        }
    }
}