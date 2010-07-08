using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;

namespace NopSolutions.NopCommerce.Web.MasterPages
{
    public partial class TwoColumn : BaseNopNestedMasterPage
    {
        protected override void OnPreRender(EventArgs e)
        {
            if (ctrlMiniShoppingCartBox != null)
                ctrlMiniShoppingCartBox.Visible = SettingManager.GetSettingValueBoolean("Common.ShowMiniShoppingCart");
            ctrlNewsLetterSubscriptionBoxControl.Visible = !SettingManager.GetSettingValueBoolean("Display.HideNewsletterBox");
            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}
