<%@ WebHandler Language="C#" Class="GetLicense" %>
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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

public class GetLicense : IHttpHandler
{
    private void processLicenseDownload(HttpContext context, Guid orderProductVariantGuid)
    {
        OrderProductVariant orderProductVariant = OrderManager.GetOrderProductVariantByGuid(orderProductVariantGuid);
        if (orderProductVariant == null)
        {
            returnError(context, "Order product variant doesn't exist.");
            return;
        }

        Order order = OrderManager.GetOrderById(orderProductVariant.OrderId);
        if (order == null)
        {
            returnError(context, "Order doesn't exist.");
            return;
        }

        if (!OrderManager.IsLicenseDownloadAllowed(orderProductVariant))
        {
            returnError(context, "Downloads are not allowed");
            return;
        }

        if (SettingManager.GetSettingValueBoolean("Security.DownloadableProducts.ValidateUser"))
        {
            if (NopContext.Current.User == null)
            {
                string loginUrl = SEOHelper.GetLoginPageUrl();
                context.Response.Redirect(loginUrl);
            }

            if (order.CustomerId != NopContext.Current.User.CustomerId)
            {
                returnError(context, "This is not your order.");
                return;
            }
        }

        Download download = orderProductVariant.LicenseDownload;
        if (download == null)
        {
            returnError(context, "Download is not available any more.");
            return;
        }

        if (download.DownloadBinary == null)
        {
            returnError(context, "Download data is not available any more.");
            return;
        }

        string fileName = string.Empty;
        if (!string.IsNullOrEmpty(download.Filename))
            fileName = download.Filename;
        else
            fileName = orderProductVariant.OrderProductVariantId.ToString();

        //use stored data
        context.Response.Clear();
        context.Response.ContentType = download.ContentType;
        context.Response.AddHeader("Content-disposition",
            string.Format("attachment;filename=\"{0}{1}\"", fileName, download.Extension));

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

    public void ProcessRequest(HttpContext context)
    {
        Guid? orderProductVariantGuid = CommonHelper.QueryStringGuid("OrderProductVariantGUID");

        if (orderProductVariantGuid.HasValue)
            processLicenseDownload(context, orderProductVariantGuid.Value);
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