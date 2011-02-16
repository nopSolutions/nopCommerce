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
            try
            {
                return Nop.Core.Context.Current.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return (IEnumerable<object>)Nop.Core.Context.Current.ResolveAll(serviceType);
            }
            catch
            {
                return new List<Object>().AsEnumerable();
            }
        }
    }
}