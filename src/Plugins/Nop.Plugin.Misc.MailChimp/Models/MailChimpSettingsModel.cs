using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Plugin.Misc.MailChimp.Models
{
    public class MailChimpSettingsModel
    {
        private IList<SelectListItem> _listOptions;

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        [NopResourceDisplayName("Plugin.Misc.MailChimp.ApiKey")]
        public virtual string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the web hook key.
        /// </summary>
        /// <value>
        /// The web hook key.
        /// </value>
        [NopResourceDisplayName("Plugin.Misc.MailChimp.WebHookKey")]
        public virtual string WebHookKey { get; set; }

        /// <summary>
        /// Gets or sets the default list id.
        /// </summary>
        /// <value>
        /// The default list id.
        /// </value>
        [NopResourceDisplayName("Plugin.Misc.MailChimp.DefaultListId")]
        public virtual string DefaultListId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [auto sync].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [auto sync]; otherwise, <c>false</c>.
        /// </value>
        [NopResourceDisplayName("Plugin.Misc.MailChimp.AutoSync")]
        public virtual bool AutoSync { get; set; }

        [NopResourceDisplayName("Plugin.Misc.MailChimp.AutoSyncEachMinutes")]
        public virtual int AutoSyncEachMinutes { get; set; }

        /// <summary>
        /// Gets or sets the list options.
        /// </summary>
        /// <value>
        /// The list options.
        /// </value>
        public virtual IList<SelectListItem> ListOptions
        {
            get { return _listOptions ?? (_listOptions = new List<SelectListItem>()); }
            set { _listOptions = value; }
        }

        public string SaveResult { get; set; }

        public string SyncResult { get; set; }
    }
}