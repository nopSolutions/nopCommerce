 using System.Xml;

namespace Nop.Core.Extensions
{
    /// <summary>
    /// Common extensions
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// Is null or default
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="value">Value to evaluate</param>
        /// <returns>Result</returns>
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }  

        /// <summary>
        /// Get element value
        /// </summary>
        /// <param name="node">XML node</param>
        /// <param name="elName">Eelement name</param>
        /// <returns>Value (text)</returns>
        public static string ElText(this XmlNode node, string elName)
        {
            return node?.SelectSingleNode(elName)?.InnerText;
        }
    }
}
