using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Official feed manager (official plugins from www.nopCommerce.com site)
    /// </summary>
    public partial class OfficialFeedManager : IOfficialFeedManager
    {
        private static string MakeUrl(string query, params object[] args)
        {
            var url = "https://www.nopcommerce.com/extensionsxml.aspx?" + query;

            return string.Format(url, args);
        }

        private static XmlDocument GetDocument(string feedQuery, params object[] args)
        {
            var request = WebRequest.Create(MakeUrl(feedQuery, args));
            request.Timeout = 5000;
            using (var response = request.GetResponse())
            {
                using (var dataStream = response.GetResponseStream())
                using (var reader = new StreamReader(dataStream))
                {
                    var responseFromServer = reader.ReadToEnd();

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(responseFromServer);
                    return xmlDoc;
                }
            }
        }

        /// <summary>
        /// Get element value
        /// </summary>
        /// <param name="node">XML node</param>
        /// <param name="elName">Element name</param>
        /// <returns>Value (text)</returns>
        private static string ElText(XmlNode node, string elName)
        {
            return node?.SelectSingleNode(elName)?.InnerText;
        }

        /// <summary>
        /// Get categories
        /// </summary>
        /// <returns>Result</returns>
        public virtual IList<OfficialFeedCategory> GetCategories()
        {
            return GetDocument("getCategories=1").SelectNodes(@"//categories/category").Cast<XmlNode>().Select(node => new OfficialFeedCategory
            {
                Id = int.Parse(ElText(node, @"id")),
                ParentCategoryId = int.Parse(ElText(node, @"parentCategoryId")),
                Name = ElText(node, @"name")
            }).ToList();
        }

        /// <summary>
        /// Get versions
        /// </summary>
        /// <returns>Result</returns>
        public virtual IList<OfficialFeedVersion> GetVersions()
        {
            return GetDocument("getVersions=1").SelectNodes(@"//versions/version").Cast<XmlNode>().Select(node => new OfficialFeedVersion
            {
                Id = int.Parse(ElText(node, @"id")),
                Name = ElText(node, @"name")
            }).ToList();            
        }

        /// <summary>
        /// Get all plugins
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="versionId">Version identifier</param>
        /// <param name="price">Price; 0 - all, 10 - free, 20 - paid</param>
        /// <param name="searchTerm">Search term</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Plugins</returns>
        public virtual IPagedList<OfficialFeedPlugin> GetAllPlugins(int categoryId = 0,
            int versionId = 0, int price = 0,
            string searchTerm = "",
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            //pageSize parameter is currently ignored by official site (set to 15)
            var xmlDoc = GetDocument("category={0}&version={1}&price={2}&pageIndex={3}&pageSize={4}&searchTerm={5}",
                categoryId, versionId, price, pageIndex, pageSize, WebUtility.UrlEncode(searchTerm));

            var list = xmlDoc.SelectNodes(@"//extensions/extension").Cast<XmlNode>().Select(node => new OfficialFeedPlugin
            {
                Name = ElText(node, @"name"),
                Url = ElText(node, @"url"),
                PictureUrl = ElText(node, @"picture"),
                Category = ElText(node, @"category"),
                SupportedVersions = ElText(node, @"versions"),
                Price = ElText(node, @"price")
            }).ToList();

            var totalRecords = int.Parse(ElText(xmlDoc.SelectNodes(@"//totalRecords")[0], @"value"));
                        
            return new PagedList<OfficialFeedPlugin>(list, pageIndex, pageSize, totalRecords);
        }
    }
}