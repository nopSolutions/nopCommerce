using System;
using System.Collections;
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
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Controls;

namespace NopSolutions.NopCommerce.Web
{
    public partial class CaptchaImagePage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonHelper.SetResponseNoCache(Response);

            if (this.Session["CaptchaImageText"] == null)
                this.Session["CaptchaImageText"] = CommonHelper.GenerateRandomDigitCode(6);

            Captcha ci = new Captcha(this.Session["CaptchaImageText"].ToString(), 200, 50, "Century Schoolbook");
            this.Response.Clear();
            this.Response.ContentType = "image/jpeg";
            ci.Image.Save(this.Response.OutputStream, ImageFormat.Jpeg);
            ci.Dispose();
        }

        public override bool AllowGuestNavigation
        {
            get
            {
                return true;
            }
        }
    }
}