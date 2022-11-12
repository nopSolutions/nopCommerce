<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Html;

namespace Nop.Web.Framework.Localization
{
    /// <summary>
    /// Localized string
    /// </summary>
    public partial class LocalizedString : HtmlString
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
    }
=======
﻿using Microsoft.AspNetCore.Html;

namespace Nop.Web.Framework.Localization
{
    /// <summary>
    /// Localized string
    /// </summary>
    public partial class LocalizedString : HtmlString
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
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}