using Nop.Core;

namespace Nop.Plugin.Widgets.CustomProductReviews.Domains
{
    public partial class Video : BaseEntity
    {
        /// <summary>
        /// Gets or sets the video identifier
        /// </summary>
        public int Id { get; set; }
        public string MimeType { get; set; }
        public string SeoFilename { get; set; }
        public string AltAttribute { get; set; }
        public string TitleAttribute { get; set; }
        public bool IsNew { get; set; }
        public string VirtualPath { get; set; }


    }
}
