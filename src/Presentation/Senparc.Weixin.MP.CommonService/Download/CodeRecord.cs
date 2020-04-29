using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.Weixin.MP.AdvancedAPIs.QrCode;

namespace Senparc.Weixin.MP.CommonService.Download
{
    public class CodeRecord
    {
        public string Key { get; set; }
        public int QrCodeId { get; set; }
        public CreateQrCodeResult QrCodeTicket { get; set; }
        public string Version { get; set; }
        public bool Used { get; set; }
        public bool AllowDownload { get; set; }
        public bool IsWebVersion { get; set; }
    }
}
