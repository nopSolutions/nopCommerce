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
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.Web.Templates.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Shipping.ShippingByWeightConfigure
{
    public partial class ConfigureShipping : BaseNopAdministrationUserControl, IConfigureShippingRateComputationMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                gvShippingByWeights.Columns[1].HeaderText = string.Format("From [{0}]", IoC.Resolve<IMeasureService>().BaseWeightIn.Name);
                gvShippingByWeights.Columns[2].HeaderText = string.Format("To [{0}]", IoC.Resolve<IMeasureService>().BaseWeightIn.Name);
                gvShippingByWeights.Columns[5].HeaderText = "Charge amount";
                if (IoC.Resolve<IShippingByWeightService>().CalculatePerWeightUnit)
                {
                    gvShippingByWeights.Columns[5].HeaderText += string.Format(" per {0}", IoC.Resolve<IMeasureService>().BaseWeightIn.Name);
                }

                FillDropDowns();
                BindSettings();
                BindData();
            }
        }

        private void FillDropDowns()
        {
            ddlShippingMethod.Items.Clear();
            var shippingMethodCollection = IoC.Resolve<IShippingService>().GetAllShippingMethods();
            foreach (ShippingMethod shippingMethod in shippingMethodCollection)
            {
                ListItem item = new ListItem(shippingMethod.Name, shippingMethod.ShippingMethodId.ToString());
                ddlShippingMethod.Items.Add(item);
            }
        }

        private void BindData()
        {
            var shippingByWeightCollection = IoC.Resolve<IShippingByWeightService>().GetAll();
            gvShippingByWeights.DataSource = shippingByWeightCollection;
            gvShippingByWeights.DataBind();
        }
        
        private void BindSettings()
        {
            cbLimitMethodsToCreated.Checked = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("ShippingByWeight.LimitMethodsToCreated");
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                int shippingMethodId = int.Parse(this.ddlShippingMethod.SelectedItem.Value);
                var shippingByWeight = new ShippingByWeight()
                {
                    ShippingMethodId = shippingMethodId,
                    From = txtFrom.Value,
                    To = txtTo.Value,
                    UsePercentage = cbUsePercentage.Checked,
                    ShippingChargePercentage = txtShippingChargePercentage.Value,
                    ShippingChargeAmount = txtShippingChargeAmount.Value
                };
                IoC.Resolve<IShippingByWeightService>().InsertShippingByWeight(shippingByWeight);

                BindData();
            }
            catch (Exception exc)
            {
                processAjaxError(exc);
            }
        }

        protected void gvShippingByWeights_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateShippingByWeight")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvShippingByWeights.Rows[index];

                HiddenField hfShippingByWeightId = row.FindControl("hfShippingByWeightId") as HiddenField;
                DropDownList ddlShippingMethod = row.FindControl("ddlShippingMethod") as DropDownList;
                DecimalTextBox txtFrom = row.FindControl("txtFrom") as DecimalTextBox;
                DecimalTextBox txtTo = row.FindControl("txtTo") as DecimalTextBox;
                CheckBox cbUsePercentage = row.FindControl("cbUsePercentage") as CheckBox;
                DecimalTextBox txtShippingChargePercentage = row.FindControl("txtShippingChargePercentage") as DecimalTextBox;
                DecimalTextBox txtShippingChargeAmount = row.FindControl("txtShippingChargeAmount") as DecimalTextBox;

                int shippingByWeightId = int.Parse(hfShippingByWeightId.Value);
                int shippingMethodId = int.Parse(ddlShippingMethod.SelectedItem.Value);
                ShippingByWeight shippingByWeight = IoC.Resolve<IShippingByWeightService>().GetById(shippingByWeightId);

                if (shippingByWeight != null)
                {
                    shippingByWeight.ShippingMethodId = shippingMethodId;
                    shippingByWeight.From = txtFrom.Value;
                    shippingByWeight.To =  txtTo.Value;
                    shippingByWeight.UsePercentage = cbUsePercentage.Checked;
                    shippingByWeight.ShippingChargePercentage = txtShippingChargePercentage.Value;
                    shippingByWeight.ShippingChargeAmount = txtShippingChargeAmount.Value;

                    IoC.Resolve<IShippingByWeightService>().UpdateShippingByWeight(shippingByWeight);
                }

                BindData();
            }
        }

        protected void gvShippingByWeights_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ShippingByWeight shippingByWeight = (ShippingByWeight)e.Row.DataItem;

                Button btnUpdate = e.Row.FindControl("btnUpdate") as Button;
                if (btnUpdate != null)
                    btnUpdate.CommandArgument = e.Row.RowIndex.ToString();

                DropDownList ddlShippingMethod = e.Row.FindControl("ddlShippingMethod") as DropDownList;
                ddlShippingMethod.Items.Clear();
                var shippingMethodCollection = IoC.Resolve<IShippingService>().GetAllShippingMethods();
                foreach (ShippingMethod shippingMethod in shippingMethodCollection)
                {
                    ListItem item = new ListItem(shippingMethod.Name, shippingMethod.ShippingMethodId.ToString());
                    ddlShippingMethod.Items.Add(item);
                    if (shippingByWeight.ShippingMethodId == shippingMethod.ShippingMethodId)
                        item.Selected = true;
                }
            }
        }

        protected void gvShippingByWeights_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int shippingByWeightId = (int)gvShippingByWeights.DataKeys[e.RowIndex]["ShippingByWeightId"];
            ShippingByWeight shippingByWeight = IoC.Resolve<IShippingByWeightService>().GetById(shippingByWeightId);
            if (shippingByWeight != null)
            {
                IoC.Resolve<IShippingByWeightService>().DeleteShippingByWeight(shippingByWeight.ShippingByWeightId);
                BindData();
            }
        }

        protected void processAjaxError(Exception exc)
        {
            ProcessException(exc, false);
            pnlError.Visible = true;
            lErrorTitle.Text = exc.Message;
        }
        
        public void Save()
        {
            IoC.Resolve<ISettingManager>().SetParam("ShippingByWeight.LimitMethodsToCreated", cbLimitMethodsToCreated.Checked.ToString());
        }
    }
}
