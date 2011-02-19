using System;
using System.Web;
using Autofac;
using Nop.Core.Infrastructure;

namespace Nop.Core.Web
{
    public class InitializerModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            EventBroker.Instance.Attach(context);

            EventBroker.Instance.EndRequest += ContextEndRequest;

            Context.Initialize(false);
        }

        private static void ContextEndRequest(object sender, EventArgs e)
        {
            var scope = (ILifetimeScope)HttpContext.Current.Items[typeof(ILifetimeScope)];
            if (scope != null)
                scope.Dispose();
        }

        public void Dispose()
        {
        }
    }
}
