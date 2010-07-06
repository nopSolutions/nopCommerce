<%@ WebHandler Language="C#" Class="Froogle" %>
using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Froogle;

public class Froogle : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/rss+xml";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;

        context.Response.Cache.SetCacheability(HttpCacheability.Public);
        context.Response.Cache.SetExpires(DateTime.Now.AddHours(1));

        if (SettingManager.GetSettingValueBoolean("Froogle.AllowPublicFroogleAccess"))
        {
            FroogleService.GenerateFeed(context.Response.OutputStream);
        }
    }
    
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}