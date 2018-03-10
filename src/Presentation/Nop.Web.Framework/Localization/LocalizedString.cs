using Microsoft.AspNetCore.Html;

namespace Nop.Web.Framework.Localization
{
    /// <summary>
    /// Localized string
    /// </summary>
    public class LocalizedString : HtmlString
    {
        private readonly string _localized;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="localized">Localized value</param>
        public LocalizedString(string localized): base (localized)
        {
            _localized = localized;
        }
        
        /// <summary>
        /// Text
        /// </summary>
        public string Text
        {
            get { return _localized; }
        }
    }
}