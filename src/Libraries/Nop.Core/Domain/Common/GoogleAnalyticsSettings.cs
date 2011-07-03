
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    //TODO implement Google Analytics as plugin
    public class GoogleAnalyticsSettings : ISettings
    {
        public bool Enabled { get; set; }
        public string GoogleId { get; set; }
        public string JavaScript { get; set; }
        public string Placement { get; set; }
    }
}