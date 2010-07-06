using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;

namespace NopSolutions.NopCommerce.Web.KeepAlive
{
    public class Ping : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Ping");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
