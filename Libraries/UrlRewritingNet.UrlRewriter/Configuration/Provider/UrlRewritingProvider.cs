using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using UrlRewritingNet.Web;

namespace UrlRewritingNet.Configuration.Provider
{
    public abstract class UrlRewritingProvider : ProviderBase
    {
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
        }
        public abstract RewriteRule CreateRewriteRule();
    }
}
