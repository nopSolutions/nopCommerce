using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.MasterPages
{
    public partial class root : BaseNopFrontendMasterPage
    {
        protected void RenderAnalyticsScript()
        {
            if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Analytics.GoogleEnabled"))
            {
                string googleJS = IoC.Resolve<ISettingManager>().GetSettingValue("Analytics.GoogleJS");
                //string googleId = IoC.Resolve<ISettingManager>().GetSettingValue("Analytics.GoogleId");
                //string analyticsString = string.Format(googleJS, googleId);

                Literal script = new Literal() { Text = googleJS };
                string placement = IoC.Resolve<ISettingManager>().GetSettingValue("Analytics.GooglePlacement").ToLowerInvariant();
                switch (placement)
                {
                    case "head":
                        {
                            this.phAnalyticsHead.Controls.AddAt(0, script);
                        }
                        break;
                    case "body":
                    default:
                        {
                            this.phAnalyticsBody.Controls.AddAt(0, script);
                        }
                        break;
                }
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            //bind analytics scripts
            this.RenderAnalyticsScript();

            base.OnPreRender(e);
        }
    }
}
