using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Nop.Services.Common
{
    public abstract partial class BaseAttributeParser
    {
        #region Properties

        protected abstract string RootElementName { get; set; }

        protected abstract string ChildElementName { get; set; }

        #endregion

        #region Utilities

        /// <summary>
        /// Remove an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="attributeValueId">Attribute value id</param>
        /// <returns>Updated result (XML format)</returns>
        protected virtual string RemoveAttribute(string attributesXml, int attributeValueId)
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(attributesXml))
                return string.Empty;

            try
            {
                var xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(attributesXml);

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode($@"//{RootElementName}");

                if (rootElement == null)
                    return string.Empty;

                XmlElement attributeElement = null;
                //find existing
                var childNodes = xmlDoc.SelectNodes($@"//{RootElementName}/{ChildElementName}");

                if (childNodes == null)
                    return string.Empty;

                var count = childNodes.Count;

                foreach (XmlElement childNode in childNodes)
                {
                    if (!int.TryParse(childNode.Attributes["ID"]?.InnerText.Trim(), out var id))
                        continue;

                    if (id != attributeValueId)
                        continue;

                    attributeElement = childNode;
                    break;
                }

                //found
                if (attributeElement != null)
                {
                    rootElement.RemoveChild(attributeElement);
                    count -= 1;
                }

                result = count == 0 ? string.Empty : xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return result;
        }

        /// <summary>
        /// Gets selected attribute identifiers
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected attribute identifiers</returns>
        protected virtual IList<int> ParseAttributeIds(string attributesXml)
        {
            var ids = new List<int>();
            if (string.IsNullOrEmpty(attributesXml))
                return ids;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                var elements = xmlDoc.SelectNodes(@$"//{RootElementName}/{ChildElementName}");

                if (elements == null)
                    return Array.Empty<int>();

                foreach (XmlNode node in elements)
                {
                    if (node.Attributes?["ID"] == null)
                        continue;

                    var attributeValue = node.Attributes["ID"].InnerText.Trim();
                    if (int.TryParse(attributeValue, out var id))
                        ids.Add(id);
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return ids;
        }

        #endregion
    }
}
