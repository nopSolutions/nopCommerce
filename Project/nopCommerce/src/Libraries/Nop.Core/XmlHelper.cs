using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Nop.Core;

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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the encoded string
    /// </returns>
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the encoded string
    /// </returns>
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
        await using var xwr = XmlWriter.Create(sw, settings);
        await xwr.WriteStringAsync(str);
        await xwr.FlushAsync();

        return sw.ToString();
    }

    /// <summary>
    /// Decodes an attribute
    /// </summary>
    /// <param name="str">Attribute</param>
    /// <returns>Decoded attribute</returns>
    public static string XmlDecode(string str)
    {
        var sb = new StringBuilder(str);

        var rez = sb.Replace("&quot;", "\"").Replace("&apos;", "'").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").ToString();

        return rez;
    }

    /// <summary>
    /// Serializes a datetime
    /// </summary>
    /// <param name="dateTime">Datetime</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the serialized datetime
    /// </returns>
    public static async Task<string> SerializeDateTimeAsync(DateTime dateTime)
    {
        var xmlS = new XmlSerializer(typeof(DateTime));
        var sb = new StringBuilder();
        await using var sw = new StringWriter(sb);
        xmlS.Serialize(sw, dateTime);

        return sb.ToString();
    }

    #endregion
}