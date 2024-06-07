//code from Telerik MVC Extensions

using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Security;

namespace Nop.Web.Framework.Menu;

/// <summary>
/// XML sitemap
/// </summary>
public partial class XmlSiteMap : IXmlSiteMap
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IPermissionService _permissionService;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    public XmlSiteMap(ILocalizationService localizationService,
        INopFileProvider fileProvider,
        IPermissionService permissionService)
    {
        _localizationService = localizationService;
        _fileProvider = fileProvider;
        _permissionService = permissionService;
        RootNode = new SiteMapNode();
    }

    #endregion

    #region Utilities

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task IterateAsync(SiteMapNode siteMapNode, XmlNode xmlNode)
    {
        await PopulateNodeAsync(siteMapNode, xmlNode);

        foreach (XmlNode xmlChildNode in xmlNode.ChildNodes)
            if (xmlChildNode.LocalName.Equals("siteMapNode", StringComparison.InvariantCultureIgnoreCase))
            {
                var siteMapChildNode = new SiteMapNode();
                siteMapNode.ChildNodes.Add(siteMapChildNode);

                await IterateAsync(siteMapChildNode, xmlChildNode);
            }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PopulateNodeAsync(SiteMapNode siteMapNode, XmlNode xmlNode)
    {
        //system name
        siteMapNode.SystemName = GetStringValueFromAttribute(xmlNode, "SystemName");

        //title
        var nopResource = GetStringValueFromAttribute(xmlNode, "nopResource");
        siteMapNode.Title = await _localizationService.GetResourceAsync(nopResource);

        //routes, url
        var controllerName = GetStringValueFromAttribute(xmlNode, "controller");
        var actionName = GetStringValueFromAttribute(xmlNode, "action");
        var url = GetStringValueFromAttribute(xmlNode, "url");
        if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName))
        {
            siteMapNode.ControllerName = controllerName;
            siteMapNode.ActionName = actionName;

            //apply admin area as described here - https://www.nopcommerce.com/boards/topic/20478/broken-menus-in-admin-area-whilst-trying-to-make-a-plugin-admin-page
            siteMapNode.RouteValues = new RouteValueDictionary { { "area", AreaNames.ADMIN } };
        }
        else if (!string.IsNullOrEmpty(url))
            siteMapNode.Url = url;

        //image URL
        siteMapNode.IconClass = GetStringValueFromAttribute(xmlNode, "IconClass");

        //permission name
        var permissionNames = GetStringValueFromAttribute(xmlNode, "PermissionNames");
        if (!string.IsNullOrEmpty(permissionNames))
        {
            var permissions = permissionNames.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

            async ValueTask<bool> predicate(string permissionName) =>
                await _permissionService.AuthorizeAsync(permissionName.Trim());

            siteMapNode.Visible = xmlNode.ChildNodes.Count > 0 ? await permissions.AnyAwaitAsync(predicate) : await permissions.AllAwaitAsync(predicate);
        }
        else
            siteMapNode.Visible = true;

        // Open URL in new tab
        var openUrlInNewTabValue = GetStringValueFromAttribute(xmlNode, "OpenUrlInNewTab");
        if (!string.IsNullOrWhiteSpace(openUrlInNewTabValue) && bool.TryParse(openUrlInNewTabValue, out var booleanResult))
            siteMapNode.OpenUrlInNewTab = booleanResult;
    }

    protected static string GetStringValueFromAttribute(XmlNode node, string attributeName)
    {
        string value = null;

        if (node.Attributes != null && node.Attributes.Count > 0)
        {
            var attribute = node.Attributes[attributeName];

            if (attribute != null)
                value = attribute.Value;
        }

        return value;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Load sitemap
    /// </summary>
    /// <param name="physicalPath">Filepath to load a sitemap</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task LoadFromAsync(string physicalPath)
    {
        var filePath = _fileProvider.MapPath(physicalPath);
        var content = await _fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);

        if (!string.IsNullOrEmpty(content))
        {
            var doc = new XmlDocument();
            using (var sr = new StringReader(content))
            {
                using var xr = XmlReader.Create(sr,
                    new XmlReaderSettings
                    {
                        CloseInput = true,
                        IgnoreWhitespace = true,
                        IgnoreComments = true,
                        IgnoreProcessingInstructions = true
                    });

                doc.Load(xr);
            }
            if ((doc.DocumentElement != null) && doc.HasChildNodes)
            {
                var xmlRootNode = doc.DocumentElement.FirstChild;
                await IterateAsync(RootNode, xmlRootNode);
            }
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Root node
    /// </summary>
    public SiteMapNode RootNode { get; set; }

    #endregion
}