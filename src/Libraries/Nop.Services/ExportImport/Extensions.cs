using System.Xml;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Write string
        /// </summary>
        /// <param name="xmlWriter">XML writer</param>
        /// <param name="nodeName">Node name</param>
        /// <param name="nodeValue">Node value</param>
        /// <param name="ignore">Ignore</param>
        /// <param name="defaulValue">Default value</param>
        public static void WriteString(this XmlWriter xmlWriter, string nodeName, object nodeValue, bool ignore = false, string defaulValue = "")
        {
            if (ignore)
                return;

            xmlWriter.WriteElementString(nodeName, nodeValue?.ToString() ?? defaulValue);
        }
    }
}
