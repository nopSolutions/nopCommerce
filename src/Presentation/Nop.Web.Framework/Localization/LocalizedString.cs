using Microsoft.AspNetCore.Html;

namespace Nop.Web.Framework.Localization
{
    /// <summary>
    /// Localized string
    /// </summary>
    public class LocalizedString : HtmlString
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="localized">Localized value</param>
        public LocalizedString(string localized): base (localized)
        {
            Text = localized;
        }
        
        /// <summary>
        /// Text
        /// </summary>
        public string Text { get; }

        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// Conversion localized string to string
        /// </summary>
        /// <param name="localizedString">Localized string to convert</param>
        public static implicit operator string(LocalizedString localizedString)
        {
            return localizedString.ToString();
        }
    }
}