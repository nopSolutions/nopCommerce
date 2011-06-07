
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    public class SeoSettings : ISettings
    {
        public string PageTitleSeparator { get; set; }
        public string DefaultTitle { get; set; }
        public string DefaultMetaKeywords { get; set; }
        public string DefaultMetaDescription { get; set; }
    }
}