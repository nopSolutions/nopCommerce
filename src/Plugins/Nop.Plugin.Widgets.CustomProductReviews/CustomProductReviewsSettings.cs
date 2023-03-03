using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.CustomProductReviews
{
    /// <summary>
    /// Represents a EcbExchangeRate plugin settings
    /// </summary>
    public class CustomProductReviewsSettings: ISettings
    {
       
        public string data { get; set; }
        public bool license { get; set; }
        public string WidgetZone { get; set; }
        public int MaximumFile { get; set; }
        public int MaximumSize { get; set; }
    }
}