using System.Xml;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Represents a theme configuration
    /// </summary>
    public class ThemeConfiguration
    {
        public ThemeConfiguration(string systemName, XmlDocument doc)
        {
            SystemName = systemName;
            var node = doc.SelectSingleNode("Theme");
            if (node != null)
            {
                var attribute = node.Attributes["title"];
                Title = attribute == null ? string.Empty : attribute.Value;
                attribute = node.Attributes["supportRTL"];
                SupportRtl = attribute != null && bool.Parse(attribute.Value);
                attribute = node.Attributes["previewImageUrl"];
                PreviewImageUrl = attribute == null ? string.Empty : attribute.Value;
                attribute = node.Attributes["previewText"];
                PreviewText = attribute == null ? string.Empty : attribute.Value;
            }
        }

        /// <summary>
        /// Gets or sets the theme system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the theme title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the theme supports RTL (right-to-left)
        /// </summary>
        public bool SupportRtl { get; set; }

        /// <summary>
        /// Gets or sets the path to the preview image of the theme
        /// </summary>
        public string PreviewImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the preview text of the theme
        /// </summary>
        public string PreviewText { get; set; }
    }
}