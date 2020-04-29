using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Senparc.Weixin.MP.CommonService.Download
{
    public class Config
    {
        public int QrCodeId { get; set; }
        /// <summary>
        /// chm版
        /// </summary>
        public List<string> Versions { get; set; }
        /// <summary>
        /// 网页版
        /// </summary>
        public List<string> WebVersions { get; set; }
        public int DownloadCount { get; set; }

    }
}
