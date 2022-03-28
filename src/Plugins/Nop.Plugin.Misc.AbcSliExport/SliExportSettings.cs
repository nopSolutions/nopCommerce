using Nop.Core.Configuration;
using Nop.Plugin.Misc.AbcSliExport.Models;

namespace Nop.Plugin.Misc.AbcSliExport
{
    public class SliExportSettings : ISettings
    {
        public string XMLFilename { get; private set; }

        public bool ExportAbcWarehouse { get; private set; }
        public bool ExportHawthorne { get; private set; }

        public string FTPServer { get; private set; }
        public string FTPUsername { get; private set; }
        public string FTPPassword { get; private set; }
        public string FTPPath { get; private set; }

        public static SliExportSettings Default()
        {
            return new SliExportSettings()
            {
                XMLFilename = "SliExport.xml",
                ExportAbcWarehouse = true
            };
        }

        public static SliExportSettings FromModel(SliExportModel model)
        {
            return new SliExportSettings()
            {
                XMLFilename = model.XMLFilename,
                ExportAbcWarehouse = model.ExportAbcWarehouse,
                ExportHawthorne = model.ExportHawthorne,
                FTPServer = model.FTPServer,
                FTPUsername = model.FTPUsername,
                FTPPassword = model.FTPPassword,
                FTPPath = model.FTPPath
            };
        }

        public SliExportModel ToModel()
        {
            return new SliExportModel()
            {
                XMLFilename = XMLFilename,
                ExportAbcWarehouse = ExportAbcWarehouse,
                ExportHawthorne = ExportHawthorne,
                FTPUsername = FTPUsername,
                FTPPassword = FTPPassword,
                FTPServer = FTPServer,
                FTPPath = FTPPath
            };
        }

        // If all FTP fields are not filled out, consider FTP disabled.
        public bool IsFTPEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(FTPServer) &&
                       !string.IsNullOrWhiteSpace(FTPUsername) &&
                       !string.IsNullOrWhiteSpace(FTPPassword) &&
                       !string.IsNullOrWhiteSpace(FTPPath);
            }
        }

        public bool IsExportSelected
        {
            get
            {
                return ExportAbcWarehouse || ExportHawthorne;
            }
        }
    }
}