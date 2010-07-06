using System;
using System.Collections.Generic;
using System.Text;
using UrlRewritingNet.Web;

namespace UrlRewritingNet.Configuration.Provider
{
    public class RegExUrlRewritingProvider : UrlRewritingProvider
    {
        public override UrlRewritingNet.Web.RewriteRule CreateRewriteRule()
        {
            return new RegExRewriteRule();
        }
    }
}
