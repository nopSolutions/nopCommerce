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
            cbAllowPublicFroogleAccess.Checked = SettingManager.GetSettingValueBoolean("Froogle.AllowPublicFroogleAccess");
            txtFroogleFTPHostname.Text = SettingManager.GetSettingValue("Froogle.FTPHostname");
            txtFroogleFTPFilename.Text = SettingManager.GetSettingValue("Froogle.FTPFilename");
            txtFroogleFTPUsername.Text = SettingManager.GetSettingValue("Froogle.FTPUsername");
            txtFroogleFTPPassword.Text = SettingManager.GetSettingValue("Froogle.FTPPassword");
        }

        private void FillDropDowns()
        {
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
                    SettingManager.SetParam("Froogle.AllowPublicFroogleAccess", cbAllowPublicFroogleAccess.Checked.ToString());
                    SettingManager.SetParam("Froogle.FTPHostname", txtFroogleFTPHostname.Text);
                    SettingManager.SetParam("Froogle.FTPFilename", txtFroogleFTPFilename.Text);
                    SettingManager.SetParam("Froogle.FTPUsername", txtFroogleFTPUsername.Text);
                    SettingManager.SetParam("Froogle.FTPPassword", txtFroogleFTPPassword.Text);

                    CustomerActivityManager.InsertActivity("EditPromotionProviders", GetLocaleResourceString("ActivityLog.EditPromotionProviders"));

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
                SettingManager.SetParam("Froogle.FTPHostname", txtFroogleFTPHostname.Text);
                SettingManager.SetParam("Froogle.FTPFilename", txtFroogleFTPFilename.Text);
                SettingManager.SetParam("Froogle.FTPUsername", txtFroogleFTPUsername.Text);
                SettingManager.SetParam("Froogle.FTPPassword", txtFroogleFTPPassword.Text);

                string hostname = SettingManager.GetSettingValue("Froogle.FTPHostname");
                string filename = SettingManager.GetSettingValue("Froogle.FTPFilename");
                string uri = String.Format("{0}/{1}", hostname, filename);
                string username = SettingManager.GetSettingValue("Froogle.FTPUsername");
                string password = SettingManager.GetSettingValue("Froogle.FTPPassword");


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