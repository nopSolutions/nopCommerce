using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    public class AdminAreaSettings : ISettings
    {
        /// <summary>
        /// Default grid page size
        /// </summary>
        public int DefaultGridPageSize { get; set; }

        /// <summary>
        /// Popup grid page size (for popup pages)
        /// </summary>
        public int PopupGridPageSize { get; set; }

        /// <summary>
        /// A comma-separated list of available grid page sizes
        /// </summary>
        public string GridPageSizes { get; set; }

        /// <summary>
        /// Additional settings for rich editor
        /// </summary>
        public string RichEditorAdditionalSettings { get; set; }

        /// <summary>
        ///A value indicating whether to javascript is supported in rich editor
        /// </summary>
        public bool RichEditorAllowJavaScript { get; set; }

        /// <summary>
        ///A value indicating whether to use rich editor on message templates and campaigns details pages
        /// </summary>
        public bool UseRichEditorInMessageTemplates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether advertisements (news) should be hidden
        /// </summary>
        public bool HideAdvertisementsOnAdminArea { get; set; }

        /// <summary>
        /// Gets or sets title of last news (admin area)
        /// </summary>
        public string LastNewsTitleAdminArea { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use IsoDateTimeConverter in Json results (used for avoiding issue with dates in KendoUI grids)
        /// </summary>
        public bool UseIsoDateTimeConverterInJson { get; set; }
    }
}