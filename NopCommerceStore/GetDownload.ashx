<%@ WebHandler Language="C#" Class="GetDownload" %>
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

public class GetDownload : IHttpHandler
{

    private void processOrderProductVariantDownload(HttpContext context, Guid orderProductVariantGuid)
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

        if (!OrderManager.IsDownloadAllowed(orderProductVariant))
        {
            returnError(context, "Downloads are not allowed");
            return;
        }

        if (SettingManager.GetSettingValueBoolean("Security.DownloadableProducts.ValidateUser"))
        {
            if (NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl();
                context.Response.Redirect(loginURL);
            }

            if (order.CustomerId != NopContext.Current.User.CustomerId)
            {
                returnError(context, "This is not your order.");
                return;
            }
        }

        ProductVariant productVariant = orderProductVariant.ProductVariant;
        if (productVariant == null)
        {
            returnError(context, "Product variant doesn't exist");
            return;
        }

        if(productVariant.HasUserAgreement)
        {
            bool isAgree = CommonHelper.QueryStringBool("Agree");
            if(!isAgree)
            {
                context.Response.Redirect(String.Format("{0}useragreement.aspx?orderproductvariantguid={1}", CommonHelper.GetStoreHost(false), orderProductVariantGuid));
            }
        }

        if (!productVariant.UnlimitedDownloads && orderProductVariant.DownloadCount >= productVariant.MaxNumberOfDownloads)
        {
            returnError(context, string.Format("You have reached maximum number of downloads {0}", productVariant.MaxNumberOfDownloads));
            return;
        }

        Download download = productVariant.Download;
        if (download == null)
        {
            returnError(context, "Download is not available any more.");
            return;
        }

        if (download.UseDownloadUrl)
        {
            //use URL
            if (String.IsNullOrEmpty(download.DownloadUrl))
            {
                returnError(context, "Download URL is empty.");
                return;
            }

            orderProductVariant = OrderManager.IncreaseOrderProductDownloadCount(orderProductVariant.OrderProductVariantId);

            context.Response.Redirect(download.DownloadUrl);
        }
        else
        {
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

            orderProductVariant = OrderManager.IncreaseOrderProductDownloadCount(orderProductVariant.OrderProductVariantId);
        }
    }

    private void processSampleDownloadProductVariant(HttpContext context, int productVariantId)
    {
        ProductVariant productVariant = ProductManager.GetProductVariantById(productVariantId);
        if (productVariant == null)
        {
            returnError(context, string.Format("Product variant doesn't exist."));
            return;
        }
        
        if (!productVariant.HasSampleDownload)
        {
            returnError(context, "Product variant doesn't have a sample download.");
            return;
        }

        Download sampleDownload = productVariant.SampleDownload;
        if (sampleDownload == null)
        {
            returnError(context, "Sample download is not available any more.");
            return;
        }

        if (sampleDownload.UseDownloadUrl)
        {
            //use URL
            if (String.IsNullOrEmpty(sampleDownload.DownloadUrl))
            {
                returnError(context, "Download URL is empty.");
                return;
            }

            context.Response.Redirect(sampleDownload.DownloadUrl);
        }
        else
        {
            if (sampleDownload.DownloadBinary == null)
            {
                returnError(context, "Download data is not available any more.");
                return;
            }

            string fileName = string.Empty;
            if (!string.IsNullOrEmpty(sampleDownload.Filename))
                fileName = sampleDownload.Filename;
            else
                fileName = productVariant.ProductVariantId.ToString();

            //use stored data
            context.Response.Clear();
            context.Response.ContentType = sampleDownload.ContentType;
            context.Response.AddHeader("Content-disposition",
                string.Format("attachment;filename=\"{0}{1}\"", fileName, sampleDownload.Extension));

            using (MemoryStream ms = new MemoryStream(sampleDownload.DownloadBinary))
            {
                int length;
                long dataToRead = sampleDownload.DownloadBinary.Length;
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

    public void ProcessRequest(HttpContext context)
    {
        Guid? orderProductVariantGuid = CommonHelper.QueryStringGuid("OrderProductVariantGUID");
        int sampleDownloadProductVariantId = CommonHelper.QueryStringInt("SampleDownloadProductVariantId");

        if (orderProductVariantGuid.HasValue)
            processOrderProductVariantDownload(context, orderProductVariantGuid.Value);
        else if (sampleDownloadProductVariantId > 0)
            processSampleDownloadProductVariant(context, sampleDownloadProductVariantId);
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