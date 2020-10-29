using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Nop.Core
{
    /// <summary>
    /// XML helper class
    /// </summary>
    public partial class XmlHelper
    {
        #region Methods

        /// <summary>
        /// XML Encode
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Encoded string</returns>
        public static async Task<string> XmlEncodeAsync(string str)
        {
            if (str == null)
                return null;
            str = Regex.Replace(str, @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD]", string.Empty, RegexOptions.Compiled);
            
            return await XmlEncodeAsIsAsync(str);
        }

        /// <summary>
        /// XML Encode as is
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Encoded string</returns>
        public static async Task<string> XmlEncodeAsIsAsync(string str)
        {
            if (str == null)
                return null;

            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var sw = new StringWriter();
            using (var xwr = XmlWriter.Create(sw, settings))
            {
                await xwr.WriteStringAsync(str);
                await xwr.FlushAsync();
            }

            return sw.ToString();
        }

        /// <summary>
        /// Encodes an attribute
        /// </summary>
        /// <param name="str">Attribute</param>
        /// <returns>Encoded attribute</returns>
        public static async Task<string> XmlEncodeAttributeAsync(string str)
        {
            if (str == null)
                return null;
            str = Regex.Replace(str, @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD]", string.Empty, RegexOptions.Compiled);
            
            return await XmlEncodeAttributeAsIsAsync(str);
        }

        /// <summary>
        /// Encodes an attribute as is
        /// </summary>
        /// <param name="str">Attribute</param>
        /// <returns>Encoded attribute</returns>
        public static async Task<string> XmlEncodeAttributeAsIsAsync(string str)
        {
            var rez = await XmlEncodeAsIsAsync(str);

            return rez.Replace("\"", "&quot;");
        }

        /// <summary>
        /// Decodes an attribute
        /// </summary>
        /// <param name="str">Attribute</param>
        /// <returns>Decoded attribute</returns>
        public static Task<string> XmlDecodeAsync(string str)
        {
            var sb = new StringBuilder(str);
            
            var rez = sb.Replace("&quot;", "\"").Replace("&apos;", "'").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").ToString();

            return Task.FromResult(rez);
        }

        /// <summary>
        /// Serializes a datetime
        /// </summary>
        /// <param name="dateTime">Datetime</param>
        /// <returns>Serialized datetime</returns>
        public static async Task<string> SerializeDateTimeAsync(DateTime dateTime)
        {
            var xmlS = new XmlSerializer(typeof(DateTime));
            var sb = new StringBuilder();
            await using var sw = new StringWriter(sb);
            xmlS.Serialize(sw, dateTime);

            return sb.ToString();
        }

        /// <summary>
        /// Deserializes a datetime
        /// </summary>
        /// <param name="dateTime">Datetime</param>
        /// <returns>Deserialized datetime</returns>
        public static Task<DateTime> DeserializeDateTimeAsync(string dateTime)
        {
            var xmlS = new XmlSerializer(typeof(DateTime));
            using var sr = new StringReader(dateTime);
            var test = xmlS.Deserialize(sr);

            return Task.FromResult((DateTime)test);
        }

        #endregion
    }
}
