
namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer attribute helper
    /// </summary>
    public partial interface ICustomerAttributeFormatter
    {
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>Attributes</returns>
        string FormatAttributes(string attributesXml, string serapator = "<br />", bool htmlEncode = true);
    }
}
