using Nop.Core;

namespace Nop.Plugin.Widgets.CustomProductReviews.Domains
{
    /// <summary>
    /// Represents a video binary data
    /// </summary>
    public partial class UploadDataBinary 
    {
        /// <summary>
        /// Gets or sets the video binary
        /// </summary>
        public byte[] BinaryData { get; set; }
       
        /// <summary>
        /// Gets or sets the video identifier
        /// </summary>
        public string Extentions { get; set; }
    }
}
