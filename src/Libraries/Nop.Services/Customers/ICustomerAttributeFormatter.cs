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
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>Attributes</returns>
        string FormatAttributes(string attributesXml, string separator = "<br />", bool htmlEncode = true);
    }
}
