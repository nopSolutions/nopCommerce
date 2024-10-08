using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class ArchiveService
    {
        private static readonly string _backendInvItemNum = "ITEM_NUMBER";
        private static readonly string _backendInvTable = "DA1_INVENTORY_MASTER";

        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEmailSender _emailSender;
        private readonly INopDataProvider _nopDbContext;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;

        public ArchiveService(
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            IEmailSender emailSender,
            INopDataProvider nopDbContext,
            ISettingService settingService,
            ILogger logger)
        {
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
            _emailSender = emailSender;
            _nopDbContext = nopDbContext;
            _settingService = settingService;
            _logger = logger;
        }

        /// <summary>
        /// returns an IEnumberable of files in the directory that match the search pattern. will not search subdirectories
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        private IEnumerable<string> GetFileNames(string path, string searchPattern = "*")
        {
            return Directory.EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly).Select(Path.GetFileName);
        }

        /// <summary>
        /// moves files from targetDirectory whose name does not start with an item number in allowedItemNumbers to an "Archive" directory in the target
        /// </summary>
        /// <param name="allowedItemNumbers"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="fileNames">must start with an item number followed by '_'</param>
        private void ArchiveFiles(HashSet<string> allowedItemNumbers, string targetDirectory, IEnumerable<string> fileNames, ref HashSet<string> archivedItemNumbers, string archivePath = null)
        {
            var archiveDate = DateTime.Today;
            var archivePrefix = $"{archiveDate.Year}.{archiveDate.Month}.{archiveDate.Day}_";
            if (!targetDirectory.EndsWith(@"\"))
            {
                targetDirectory += @"\";
            }

            var archiveFolderPath = archivePath ?? targetDirectory + "Archive";
            if (!Directory.Exists(archiveFolderPath))
            {
                Directory.CreateDirectory(archiveFolderPath);
            }

            HashSet<string> filesToMove = new HashSet<string>();

            foreach (var fileName in fileNames)
            {
                var itemNumber = fileName.Split('_')[0];
                if (!allowedItemNumbers.Contains(itemNumber))
                {
                    filesToMove.Add(fileName);
                    archivedItemNumbers.Add(itemNumber);
                }
            }

            foreach (var file in filesToMove)
            {
                var archiveFilePath = Path.Combine(archiveFolderPath, String.Concat(archivePrefix, file));
                File.Move(Path.Combine(targetDirectory, file), archiveFilePath);
            }
        }

        /// <summary>
        /// moves old energy guides, product specs, and images to archive folders. Deletes pictures attached to deleted products
        /// </summary>
        /// <param name="backendConn">connection to the ISAM backend to check valid products</param>
        public async Task ArchiveProductContentAsync(IDbConnection backendConn)
        {
            var allowedItemNumbers = GetAllowedItemNumbers(backendConn);

            var archivedItemsSet = new HashSet<string>();
            var ProcessedImageDirectory = $"{ImportSettings.GetLocalPicturesDirectory()}";

            ArchiveFiles(allowedItemNumbers, ImportSettings.GetEnergyGuidePdfPath(), GetFileNames(ImportSettings.GetEnergyGuidePdfPath()), ref archivedItemsSet);
            ArchiveFiles(allowedItemNumbers, ImportSettings.GetSpecificationPdfPath(), GetFileNames(ImportSettings.GetSpecificationPdfPath()), ref archivedItemsSet);
            ArchiveFiles(allowedItemNumbers, ProcessedImageDirectory, GetFileNames(ProcessedImageDirectory, "*_large.*"),
                ref archivedItemsSet, $"{ImportSettings.GetLocalPicturesDirectory()}/Archive");

            bool hasArchivedItems = archivedItemsSet.Any();
            if (hasArchivedItems)
            {
                await _logger.InformationAsync($"Archived items: {string.Join(",", archivedItemsSet)}");
            }
            else
            {
                await _logger.InformationAsync("No archived items.");
            }
        }


        private HashSet<string> GetAllowedItemNumbers(IDbConnection backendConn)
        {
            backendConn.Open();
            IDbCommand query = backendConn.CreateCommand();
            query.CommandText = $"SELECT DISTINCT Inv.{_backendInvItemNum} FROM {_backendInvTable} Inv;";

            var itemNoList = new List<string>();
            var itemNoReader = query.ExecuteReader();
            while (itemNoReader.Read())
            {
                var itemNum = itemNoReader[_backendInvItemNum] as string;
                if (!string.IsNullOrEmpty(itemNum))
                {
                    itemNoList.Add(itemNum);
                }
            }
            var allowedItemNumbers = new HashSet<string>(itemNoList);
            backendConn.Close();
            return allowedItemNumbers;
        }
    }
}