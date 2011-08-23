
using Nop.Core.Configuration;

namespace Nop.Plugin.Feed.Froogle
{
    public class FroogleSettings : ISettings
    {
        public int ProductPictureSize { get; set; }

        public int CurrencyId { get; set; }

        public string DefaultGoogleCategory { get; set; }
        
        public string FtpHostname { get; set; }
        public string FtpFilename { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }
    }
}