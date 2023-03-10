using Nop.Core.Domain.Attributes;

namespace Nop.Services.Attributes
{
    /// <summary>
    /// Represents an attribute formatter
    /// </summary>
    /// <typeparam name="TAttribute">Type of the attribute (see <see cref="BaseAttribute"/>)</typeparam>
    /// <typeparam name="TAttributeValue">Type of the attribute value (see <see cref="BaseAttributeValue"/>)</typeparam>
    public partial interface IAttributeFormatter<TAttribute, TAttributeValue>
        where TAttribute : BaseAttribute
        where TAttributeValue : BaseAttributeValue
    {
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the attributes
        /// </returns>
        Task<string> FormatAttributesAsync(string attributesXml,
            string separator = "<br />",
            bool htmlEncode = true);
    }
}
