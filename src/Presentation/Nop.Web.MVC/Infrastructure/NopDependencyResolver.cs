using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.MVC.Infrastructure
{
    public class NopDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            return Nop.Core.Context.Current.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return (IEnumerable<object>)Nop.Core.Context.Current.ResolveAll(serviceType);
        }
    }
}