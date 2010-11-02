using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.MasterPages
{
    public partial class root : BaseNopMasterPage
    {
        protected void RenderAnalyticsScript()
        {
            if (IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Analytics.GoogleEnabled"))
            {
                string googleJS = IoCFactory.Resolve<ISettingManager>().GetSettingValue("Analytics.GoogleJS");
                //string googleId = IoCFactory.Resolve<ISettingManager>().GetSettingValue("Analytics.GoogleId");
                //string analyticsString = string.Format(googleJS, googleId);

                Literal script = new Literal() { Text = googleJS };
                string placement = IoCFactory.Resolve<ISettingManager>().GetSettingValue("Analytics.GooglePlacement").ToLowerInvariant();
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
