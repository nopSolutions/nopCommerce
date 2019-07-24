using System.Collections.Generic;

namespace Nop.Web.Infrastructure.Installation
{
    /// <summary>
    /// Language class for installation process
    /// </summary>
    public partial class InstallationLanguage
    {
        public InstallationLanguage()
        {
            Resources = new List<InstallationLocaleResource>();
        }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsDefault { get; set; }
        public bool IsRightToLeft { get; set; }

        public List<InstallationLocaleResource> Resources { get; protected set; }
    }

    public partial class InstallationLocaleResource
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
