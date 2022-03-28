using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.AbcSliExport.Models
{
    public class SliExportModel
    {
        [Required]
        [NopResourceDisplayName(SliExport.LocaleKey.XMLFilename)]
        public string XMLFilename { get; set; }

        [NopResourceDisplayName(SliExport.LocaleKey.ExportABCWarehouse)]
        public bool ExportAbcWarehouse { get; set; }

        [NopResourceDisplayName(SliExport.LocaleKey.ExportHawthorne)]
        public bool ExportHawthorne { get; set; }

        [NopResourceDisplayName(SliExport.LocaleKey.FTPUsername)]
        public string FTPUsername { get; set; }

        [DataType(DataType.Password)]
        [NopResourceDisplayName(SliExport.LocaleKey.FTPPassword)]
        public string FTPPassword { get; set; }

        [NopResourceDisplayName(SliExport.LocaleKey.FTPServer)]
        public string FTPServer { get; set; }

        [NopResourceDisplayName(SliExport.LocaleKey.FTPPath)]
        public string FTPPath { get; set; }
    }
}