using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Senparc.CO2NET.Utilities;
using Senparc.CO2NET.Trace;

#if NET45
using System.Web;
#else
using Senparc.Weixin.MP.CommonService.Utilities;
using Microsoft.AspNetCore.Http;
#endif


namespace Senparc.Weixin.MP.CommonService.Download
{
    public class ConfigHelper
    {
        //Key：guid，Value：<QrCodeId,Version>
        public static Dictionary<string, CodeRecord> CodeCollection = new Dictionary<string, CodeRecord>(StringComparer.OrdinalIgnoreCase);
        public static object Lock = new object();


        public ConfigHelper()
        {
        }

        private string GetDatabaseFilePath()
        {
            return ServerUtility.ContentRootMapPath("~/App_Data/Document/Config.xml");
        }

        private XDocument GetXDocument()
        {
            var databaseFilePath = GetDatabaseFilePath();
            if (!File.Exists(databaseFilePath))
            {
                SenparcTrace.SendCustomLog("Config.xml", $"初始化新建：{databaseFilePath}");

                //如果不存在则新建
                var config = new Config()
                {
                    QrCodeId = 0,
                    DownloadCount = 0,
                    Versions = new List<string>() { "0.0.0"},
                    WebVersions = new List<string>() { "0.0.0"}
                };

                XDocument newDoc = new XDocument();
                var root = new XElement("Config");
                root.Add(new XElement("QrCodeId", config.QrCodeId));
                root.Add(new XElement("DownloadCount", config.DownloadCount));
                root.Add(new XElement("Versions", new XElement("Version", config.Versions.First())));
                root.Add(new XElement("WebVersions", new XElement("Version", config.Versions.First())));
                newDoc.Add(root);
                using (FileStream fs = new FileStream(databaseFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    newDoc.Save(fs);
                }
                SenparcTrace.SendCustomLog("Config.xml", $"初始化完成");
                return newDoc;
            }

            var doc = XDocument.Load(GetDatabaseFilePath());
            return doc;
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Config GetConfig()
        {
            var doc = GetXDocument();
            var config = new Config()
            {
                QrCodeId = int.Parse(doc.Root.Element("QrCodeId").Value),
                DownloadCount = int.Parse(doc.Root.Element("DownloadCount").Value),
                Versions = doc.Root.Element("Versions").Elements("Version").Select(z => z.Value).ToList(),
                WebVersions = doc.Root.Element("WebVersions").Elements("Version").Select(z => z.Value).ToList()
            };
            return config;
        }

        /// <summary>
        /// 获取一个二维码场景标示（自增，唯一）
        /// </summary>
        /// <returns></returns>
        public int GetQrCodeId()
        {
            lock (Lock)
            {
                var config = GetConfig();
                config.QrCodeId++;
                Save(config);
                return config.QrCodeId;
            }
        }

        public void Save(Config config)
        {
            var doc = GetXDocument();
            doc.Root.Element("QrCodeId").Value = config.QrCodeId.ToString();
            doc.Root.Element("DownloadCount").Value = config.DownloadCount.ToString();
            doc.Root.Element("Versions").Elements().Remove();
            foreach (var version in config.Versions)
            {
                doc.Root.Element("Versions").Add(new XElement("Version", version));
            }
#if NET45
            doc.Save(GetDatabaseFilePath());
#else
            using (FileStream fs = new FileStream(GetDatabaseFilePath(), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                doc.Save(fs);
            }
#endif
        }

        public string Download(string version, bool isWebVersion)
        {
            lock (Lock)
            {
                var config = GetConfig();
                config.DownloadCount++;
                Save(config);
            }

            //打包下载文件
            //FileStream fs = new FileStream(_context.ServerUtility.ContentRootMapPath(string.Format("~/App_Data/Document/Files/Senparc.Weixin-v{0}.rar", version)), FileMode.Open);
            //return fs;

            var filePath = ServerUtility.ContentRootMapPath(string.Format("~/App_Data/Document/Files/Senparc.Weixin{0}-v{1}.rar", isWebVersion ? "-Web" : "", version));
            if (!File.Exists(filePath))
            {
                //使用.zip文件
                filePath = filePath.Replace(".rar", ".zip");
            }
            return filePath;
        }
    }
}
