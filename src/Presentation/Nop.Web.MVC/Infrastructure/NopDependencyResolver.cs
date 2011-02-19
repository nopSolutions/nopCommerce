using System;
using System.Collections;
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
                return Core.Context.Current.Resolve(serviceType);
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
                var type = typeof(IEnumerable<>).MakeGenericType(serviceType);
                return (IEnumerable<object>)Core.Context.Current.Resolve(type);
            }
            catch
            {
                return null;
            }
            
        }
    }
}