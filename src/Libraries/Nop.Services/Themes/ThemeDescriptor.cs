using Newtonsoft.Json;
using Nop.Services.Plugins;

namespace Nop.Services.Themes
{
    /// <summary>
    /// Represents a theme descriptor
    /// </summary>
    public class ThemeDescriptor : IDescriptor
    {
        /// <summary>
        /// Gets or sets the theme system name
        /// </summary>
        [JsonProperty(PropertyName = "SystemName")]
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the theme friendly name
        /// </summary>
        [JsonProperty(PropertyName = "FriendlyName")]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the theme supports RTL (right-to-left)
        /// </summary>
        [JsonProperty(PropertyName = "SupportRTL")]
        public bool SupportRtl { get; set; }

        /// <summary>
        /// Gets or sets the path to the preview image of the theme
        /// </summary>
        [JsonProperty(PropertyName = "PreviewImageUrl")]
        public string PreviewImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the preview text of the theme
        /// </summary>
        [JsonProperty(PropertyName = "PreviewText")]
        public string PreviewText { get; set; }
    }
}