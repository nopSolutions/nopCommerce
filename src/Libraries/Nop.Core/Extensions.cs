using System.Xml;

namespace Nop.Core
{
    public static class Extensions
    {
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        public static string ElText(this XmlNode node, string elName)
        {
            return node.SelectSingleNode(elName).InnerText;
        }
    }
}
