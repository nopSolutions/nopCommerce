//code from Telerik MVC Extensions

using System;
using System.IO;
using System.Linq;
using System.Web.Routing;
using System.Xml;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Security;

namespace Nop.Web.Framework.Menu
{
    public class XmlSiteMap
    {
        public XmlSiteMap()
        {
            RootNode = new SiteMapNode();
        }

        public SiteMapNode RootNode { get; set; }

        public virtual void LoadFrom(string physicalPath)
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            string filePath = CommonHelper.MapPath(physicalPath);
            string content = File.ReadAllText(filePath);

            if (!string.IsNullOrEmpty(content))
            {
                using (var sr = new StringReader(content))
                {
                    using (var xr = XmlReader.Create(sr,
                            new XmlReaderSettings
                            {
                                CloseInput = true,
                                IgnoreWhitespace = true,
                                IgnoreComments = true,
                                IgnoreProcessingInstructions = true
                            }))
                    {
                        var doc = new XmlDocument();
                        doc.Load(xr);

                        if ((doc.DocumentElement != null) && doc.HasChildNodes)
                        {
                            XmlNode xmlRootNode = doc.DocumentElement.FirstChild;
                            Iterate(RootNode, xmlRootNode);
                        }
                    }
                }
            }
        }

        private static void Iterate(SiteMapNode siteMapNode, XmlNode xmlNode)
        {
            PopulateNode(siteMapNode, xmlNode);

            foreach (XmlNode xmlChildNode in xmlNode.ChildNodes)
            {
                if (xmlChildNode.LocalName.Equals("siteMapNode", StringComparison.InvariantCultureIgnoreCase))
                {
                    var siteMapChildNode = new SiteMapNode();
                    siteMapNode.ChildNodes.Add(siteMapChildNode);

                    Iterate(siteMapChildNode, xmlChildNode);
                }
            }
        }

        private static void PopulateNode(SiteMapNode siteMapNode, XmlNode xmlNode)
        {
            //system name
            siteMapNode.SystemName = GetStringValueFromAttribute(xmlNode, "SystemName");

            //title
            var nopResource = GetStringValueFromAttribute(xmlNode, "nopResource");
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            siteMapNode.Title = localizationService.GetResource(nopResource);

            //routes, url
            string controllerName = GetStringValueFromAttribute(xmlNode, "controller");
            string actionName = GetStringValueFromAttribute(xmlNode, "action");
            string url = GetStringValueFromAttribute(xmlNode, "url");
            if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName))
            {
                siteMapNode.ControllerName = controllerName;
                siteMapNode.ActionName = actionName;

                //apply admin area as described here - http://www.nopcommerce.com/boards/t/20478/broken-menus-in-admin-area-whilst-trying-to-make-a-plugin-admin-page.aspx
                siteMapNode.RouteValues = new RouteValueDictionary { { "area", "Admin" } };
            }
            else if (!string.IsNullOrEmpty(url))
            {
                siteMapNode.Url = url;
            }

            //image URL
            siteMapNode.IconClass = GetStringValueFromAttribute(xmlNode, "IconClass");

            //permission name
            var permissionNames = GetStringValueFromAttribute(xmlNode, "PermissionNames");
            if (!string.IsNullOrEmpty(permissionNames))
            {
                var permissionService = EngineContext.Current.Resolve<IPermissionService>();
                siteMapNode.Visible = permissionNames.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                   .Any(permissionName => permissionService.Authorize(permissionName.Trim()));
            }
            else
            {
                siteMapNode.Visible = true;
            }
        }

        private static string GetStringValueFromAttribute(XmlNode node, string attributeName)
        {
            string value = null;

            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                XmlAttribute attribute = node.Attributes[attributeName];

                if (attribute != null)
                {
                    value = attribute.Value;
                }
            }

            return value;
        }
    }
}
