using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing.Imaging;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CaptchaControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                RegenerateCode();
        }

        public void RegenerateCode()
        {
            this.Session["CaptchaImageText"] = CommonHelper.GenerateRandomDigitCode(6);
        }

        public bool ValidateCaptcha()
        {
            if (String.IsNullOrEmpty(this.txtCode.Text))
            {
                this.txtMessageLabel.Text = GetLocaleResourceString("Captcha.Incorrect");
                return false;
            }

            if (this.Session["CaptchaImageText"] == null)
                return false;

            if (this.txtCode.Text == this.Session["CaptchaImageText"].ToString())
            {
                return true;
            }
            else
            {
                this.txtMessageLabel.Text = GetLocaleResourceString("Captcha.Incorrect");
                this.txtCode.Text = "";
                RegenerateCode();
                return false;
            }

        }
    }
}