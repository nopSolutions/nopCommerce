using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nop.Core
{
    public class SessionWrapper
    {
        public static object GetObject(string key)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                return HttpContext.Current.Session[key];
            }

            return null;
        }

        public static void SetObject(string key, object val)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[key] = val;
            }
        }

        public static void RemoveObject(string key)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Remove(key);
            }
        }
    }
}
