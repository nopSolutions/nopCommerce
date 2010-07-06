<%@ WebHandler Language="C#" Class="GetDownloadAdmin" %>
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
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.SEO;

public class GetDownloadAdmin : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        if (NopContext.Current.User == null || !NopContext.Current.User.IsAdmin)
        {
            string loginURL = SEOHelper.GetAdminAreaLoginPageUrl();
            context.Response.Redirect(loginURL);
        }

        int downloadId = CommonHelper.QueryStringInt("DownloadId");
        Download download = DownloadManager.GetDownloadById(downloadId);
        if (download == null)
        {
            returnError(context, string.Format("Download is not available any more. Download ID={0}", downloadId));
            return;
        }

        if (download.UseDownloadUrl)
        {
            //use URL
            if (String.IsNullOrEmpty(download.DownloadUrl))
            {
                returnError(context, string.Format("Download URL is empty. Download ID={0}", downloadId));
                return;
            }

            context.Response.Redirect(download.DownloadUrl);
        }
        else
        {
            //use stored data
            if (download.DownloadBinary == null)
            {
                returnError(context, string.Format("Download data is not available any more. Download ID={0}", downloadId));
                return;
            }

            string fileName = string.Empty;
            if (!string.IsNullOrEmpty(download.Filename))
                fileName = download.Filename;
            else
                fileName = downloadId.ToString();

            context.Response.Clear();
            context.Response.ContentType = download.ContentType;
            context.Response.AddHeader("Content-disposition", string.Format("attachment;filename=\"{0}{1}\"", fileName, download.Extension));

            using (MemoryStream ms = new MemoryStream(download.DownloadBinary))
            {
                int length;
                long dataToRead = download.DownloadBinary.Length;
                byte[] buffer = new Byte[10000];

                while (dataToRead > 0)
                {
                    if (context.Response.IsClientConnected)
                    {
                        length = ms.Read(buffer, 0, 10000);
                        context.Response.OutputStream.Write(buffer, 0, length);
                        context.Response.Flush();
                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                        dataToRead = -1;
                }
            }
        }
    }
    
    private void returnError(HttpContext context, string message)
    {
        context.Response.Clear();
        context.Response.Write(message);
        context.Response.Flush();
    }
    
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}