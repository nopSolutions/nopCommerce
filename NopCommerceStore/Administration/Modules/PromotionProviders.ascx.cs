//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using System.IO;
using NopSolutions.NopCommerce.Froogle;
using NopSolutions.NopCommerce.PriceGrabber;
using NopSolutions.NopCommerce.Become;
using System.Net;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;


namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class PromotionProvidersControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SelectTab(PromotionProvidersTabs, TabId);
                FillDropDowns();
                BindData();
            }
        }

        private void BindData()
        {
            //froogle
            cbAllowPublicFroogleAccess.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Froogle.AllowPublicFroogleAccess");
            txtFroogleProductThumbSize.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("PromotionProvider.Froogle.ProductThumbnailImageSize");
            txtFroogleFTPHostname.Text = IoC.Resolve<ISettingManager>().GetSettingValue("Froogle.FTPHostname");
            txtFroogleFTPFilename.Text = IoC.Resolve<ISettingManager>().GetSettingValue("Froogle.FTPFilename");
            txtFroogleFTPUsername.Text = IoC.Resolve<ISettingManager>().GetSettingValue("Froogle.FTPUsername");
            txtFroogleFTPPassword.Text = IoC.Resolve<ISettingManager>().GetSettingValue("Froogle.FTPPassword");
            CommonHelper.SelectListItem(this.ddlFroogleCurrency, FroogleService.UsedCurrency.CurrencyId);

            //price grabber
            txtPriceGrabberProductThumbSize.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("PromotionProvider.PriceGrabber.ProductThumbnailImageSize");
            CommonHelper.SelectListItem(this.ddlPriceGrabberCurrency, PriceGrabberService.UsedCurrency.CurrencyId);

            //become.com
            txtBecomeProductThumbSize.Value = IoC.Resolve<ISettingManager>().GetSettingValueInteger("PromotionProvider.BecomeCom.ProductThumbnailImageSize");
            CommonHelper.SelectListItem(this.ddlBecomeCurrency, BecomeService.UsedCurrency.CurrencyId);
        }

        private void FillDropDowns()
        {
            var currencies = IoC.Resolve<ICurrencyService>().GetAllCurrencies(false);

            //Froogle
            this.ddlFroogleCurrency.Items.Clear();
            foreach (var currency in currencies)
            {
                ListItem item2 = new ListItem(currency.Name, currency.CurrencyId.ToString());
                this.ddlFroogleCurrency.Items.Add(item2);
            }

            //Price Grabber
            this.ddlPriceGrabberCurrency.Items.Clear();
            foreach (var currency in currencies)
            {
                ListItem item2 = new ListItem(currency.Name, currency.CurrencyId.ToString());
                this.ddlPriceGrabberCurrency.Items.Add(item2);
            }

            //Become.com
            this.ddlBecomeCurrency.Items.Clear();
            foreach (var currency in currencies)
            {
                ListItem item2 = new ListItem(currency.Name, currency.CurrencyId.ToString());
                this.ddlBecomeCurrency.Items.Add(item2);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();

            base.OnPreRender(e);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    //froogle
                    IoC.Resolve<ISettingManager>().SetParam("Froogle.AllowPublicFroogleAccess", cbAllowPublicFroogleAccess.Checked.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("PromotionProvider.Froogle.ProductThumbnailImageSize", txtFroogleProductThumbSize.Value.ToString());
                    IoC.Resolve<ISettingManager>().SetParam("Froogle.FTPHostname", txtFroogleFTPHostname.Text);
                    IoC.Resolve<ISettingManager>().SetParam("Froogle.FTPFilename", txtFroogleFTPFilename.Text);
                    IoC.Resolve<ISettingManager>().SetParam("Froogle.FTPUsername", txtFroogleFTPUsername.Text);
                    IoC.Resolve<ISettingManager>().SetParam("Froogle.FTPPassword", txtFroogleFTPPassword.Text);
                    FroogleService.UsedCurrency = IoC.Resolve<ICurrencyService>().GetCurrencyById(int.Parse(this.ddlFroogleCurrency.SelectedItem.Value));

                    //price grabber
                    IoC.Resolve<ISettingManager>().SetParam("PromotionProvider.PriceGrabber.ProductThumbnailImageSize", txtPriceGrabberProductThumbSize.Value.ToString());
                    PriceGrabberService.UsedCurrency = IoC.Resolve<ICurrencyService>().GetCurrencyById(int.Parse(this.ddlPriceGrabberCurrency.SelectedItem.Value));

                    //become.com
                    IoC.Resolve<ISettingManager>().SetParam("PromotionProvider.BecomeCom.ProductThumbnailImageSize", txtBecomeProductThumbSize.Value.ToString());
                    BecomeService.UsedCurrency = IoC.Resolve<ICurrencyService>().GetCurrencyById(int.Parse(this.ddlBecomeCurrency.SelectedItem.Value));

                    //activity log
                    IoC.Resolve<ICustomerActivityService>().InsertActivity("EditPromotionProviders", GetLocaleResourceString("ActivityLog.EditPromotionProviders"));

                    //redirect
                    Response.Redirect(string.Format("PromotionProviders.aspx?TabID={0}", GetActiveTabId(PromotionProvidersTabs)));
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnFroogleGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = string.Format("froogle_{0}_{1}.xml", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
                string filePath = string.Format("{0}files\\froogle\\{1}", HttpContext.Current.Request.PhysicalApplicationPath, fileName);
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    FroogleService.GenerateFeed(fs);
                }

                string clickhereStr = string.Format("<a href=\"{0}files/froogle/{1}\" target=\"_blank\">{2}</a>", CommonHelper.GetStoreLocation(false), fileName, GetLocaleResourceString("Admin.PromotionProviders.Froogle.ClickHere"));
                string result = string.Format(GetLocaleResourceString("Admin.PromotionProviders.Froogle.SuccessResult"), clickhereStr);
                ShowMessage(result);
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void btnPriceGrabberGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = string.Format("pricegrabber_{0}_{1}.csv", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
                string filePath = string.Format("{0}files\\pricegrabber\\{1}", HttpContext.Current.Request.PhysicalApplicationPath, fileName);
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    PriceGrabberService.GenerateFeed(fs);
                }

                string clickhereStr = string.Format("<a href=\"{0}files/pricegrabber/{1}\" target=\"_blank\">{2}</a>", CommonHelper.GetStoreLocation(false), fileName, GetLocaleResourceString("Admin.PromotionProviders.PriceGrabber.ClickHere"));
                string result = string.Format(GetLocaleResourceString("Admin.PromotionProviders.PriceGrabber.SuccessResult"), clickhereStr);
                ShowMessage(result);
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void btnBecomeGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = string.Format("become_{0}_{1}.csv", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
                string filePath = string.Format("{0}files\\become\\{1}", HttpContext.Current.Request.PhysicalApplicationPath, fileName);
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    BecomeService.GenerateFeed(fs);
                }

                string clickhereStr = string.Format("<a href=\"{0}files/become/{1}\" target=\"_blank\">{2}</a>", CommonHelper.GetStoreLocation(false), fileName, GetLocaleResourceString("Admin.PromotionProviders.Become.ClickHere"));
                string result = string.Format(GetLocaleResourceString("Admin.PromotionProviders.Become.SuccessResult"), clickhereStr);
                ShowMessage(result);
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void btnFroogleFTPUpload_OnClick(object sender, EventArgs e)
        {
            try
            {
                IoC.Resolve<ISettingManager>().SetParam("Froogle.FTPHostname", txtFroogleFTPHostname.Text);
                IoC.Resolve<ISettingManager>().SetParam("Froogle.FTPFilename", txtFroogleFTPFilename.Text);
                IoC.Resolve<ISettingManager>().SetParam("Froogle.FTPUsername", txtFroogleFTPUsername.Text);
                IoC.Resolve<ISettingManager>().SetParam("Froogle.FTPPassword", txtFroogleFTPPassword.Text);

                string hostname = IoC.Resolve<ISettingManager>().GetSettingValue("Froogle.FTPHostname");
                string filename = IoC.Resolve<ISettingManager>().GetSettingValue("Froogle.FTPFilename");
                string uri = String.Format("{0}/{1}", hostname, filename);
                string username = IoC.Resolve<ISettingManager>().GetSettingValue("Froogle.FTPUsername");
                string password = IoC.Resolve<ISettingManager>().GetSettingValue("Froogle.FTPPassword");


                FtpWebRequest req = WebRequest.Create(uri) as FtpWebRequest;
                req.Credentials = new NetworkCredential(username, password);
                req.KeepAlive = true;
                req.UseBinary = true;
                req.Method = WebRequestMethods.Ftp.UploadFile;

                using (Stream reqStream = req.GetRequestStream())
                {
                    FroogleService.GenerateFeed(reqStream);
                }

                FtpWebResponse rsp = req.GetResponse() as FtpWebResponse;

                ShowMessage(String.Format(GetLocaleResourceString("Admin.PromotionProviders.Froogle.FTPUploadStatus"), rsp.StatusDescription));
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected string TabId
        {
            get
            {
                return CommonHelper.QueryString("TabId");
            }
        }
    }
}