using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Plugin.Misc.MailChimp.Models {
    public class MailChimpSettingsModel {
        private IList<SelectListItem> _listOptions;

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        [NopResourceDisplayName("Nop.Plugin.Misc.MailChimp.ApiKey")]
        public virtual string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the default list id.
        /// </summary>
        /// <value>
        /// The default list id.
        /// </value>
        [NopResourceDisplayName("Nop.Plugin.Misc.MailChimp.DefaultListId")]
        public virtual string DefaultListId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [auto sync].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [auto sync]; otherwise, <c>false</c>.
        /// </value>
        [NopResourceDisplayName("Nop.Plugin.Misc.MailChimp.AutoSync")]
        public virtual bool AutoSync { get; set; }

        /// <summary>
        /// Gets or sets the list options.
        /// </summary>
        /// <value>
        /// The list options.
        /// </value>
        public virtual IList<SelectListItem> ListOptions {
            get { return _listOptions ?? (_listOptions = new List<SelectListItem>()); }
            set { _listOptions = value; }
        }
    }
}