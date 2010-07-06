using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;

namespace UrlRewritingNet.Configuration.Provider
{
    public class UrlRewritingProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (!(provider is UrlRewritingProvider))
            {
                string msg = string.Format("Provider must implement type {0}", typeof(UrlRewritingProvider).ToString());
                throw new ArgumentException(msg, "provider");
            }
            base.Add(provider);
        }
        public void CopyTo(UrlRewritingProvider[] providers, int index)
        {
            base.CopyTo(providers, index);
        }
        new public UrlRewritingProvider this[string name]
        {
            get
            {
                return (UrlRewritingProvider)base[name];
            }
        }
    }
}
