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
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Shipping.Methods.UPS;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.Web.Templates.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;

namespace NopSolutions.NopCommerce.Web.Administration.Shipping.FixedRateConfigure
{
    public partial class ConfigureShipping : BaseNopAdministrationUserControl, IConfigureShippingRateComputationMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        private void BindData()
        {
            var shippingMethods = IoC.Resolve<IShippingService>().GetAllShippingMethods();
            gvShippingMethods.DataSource = shippingMethods;
            gvShippingMethods.DataBind();
        }

        protected void gvShippingMethods_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ShippingMethod shippingMethod = (ShippingMethod)e.Row.DataItem;

                DecimalTextBox txtRate = e.Row.FindControl("txtRate") as DecimalTextBox;
                if (txtRate != null)
                {
                    txtRate.Value = IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative(string.Format("ShippingRateComputationMethod.FixedRate.Rate.ShippingMethodId{0}", shippingMethod.ShippingMethodId));
                }
            }
        }

        public void Save()
        {
            foreach (GridViewRow row in gvShippingMethods.Rows)
            {
                DecimalTextBox txtRate = row.FindControl("txtRate") as DecimalTextBox;
                HiddenField hfShippingMethodId = row.FindControl("hfShippingMethodId") as HiddenField;

                int shippingMethodId = int.Parse(hfShippingMethodId.Value);
                decimal rate = txtRate.Value;

                IoC.Resolve<ISettingManager>().SetParamNative(string.Format("ShippingRateComputationMethod.FixedRate.Rate.ShippingMethodId{0}", shippingMethodId), rate);
            }
        }
    }
}
