
using Nop.Core.Configuration;

namespace Nop.Plugin.Feed.Become
{
    public class BecomeSettings : ISettings
    {
        public int ProductPictureSize { get; set; }

        public int CurrencyId { get; set; }
    }
}