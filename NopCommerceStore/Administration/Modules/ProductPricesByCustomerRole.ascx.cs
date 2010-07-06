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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Warehouses;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductPricesByCustomerRoleControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            ProductVariant productVariant = ProductManager.GetProductVariantById(this.ProductVariantId);
            if (productVariant != null)
            {
                var customerRoles = CustomerManager.GetAllCustomerRoles();
                if (customerRoles.Count > 0)
                {
                    pnlData.Visible = true;
                    pnlMessage.Visible = false;

                    var prices = ProductManager.GetAllCustomerRoleProductPrices(productVariant.ProductVariantId);
                    if (prices.Count > 0)
                    {
                        gvPrices.Visible = true;
                        gvPrices.DataSource = prices;
                        gvPrices.DataBind();
                    }
                    else
                        gvPrices.Visible = false;
                }
                else
                {
                    pnlData.Visible = false;
                    pnlMessage.Visible = true;
                    lblMessage.Text = GetLocaleResourceString("Admin.ProductPricesByCustomerRole.NoCustomerRoleDefined");
                }
            }
            else
            {
                pnlData.Visible = false;
                pnlMessage.Visible = true;
                lblMessage.Text = GetLocaleResourceString("Admin.ProductPricesByCustomerRole.AvailableAfterSaving");
            }
        }

        private void FillDropDowns()
        {
            this.ddlNewCustomerRole.Items.Clear();
            var customerRoles = CustomerManager.GetAllCustomerRoles();
            foreach (var cr in customerRoles)
            {
                ListItem item2 = new ListItem(cr.Name, cr.CustomerRoleId.ToString());
                this.ddlNewCustomerRole.Items.Add(item2);
            }
        }
                
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.gvPrices.Columns[1].HeaderText = string.Format("{0} [{1}]", GetLocaleResourceString("Admin.ProductPricesByCustomerRole.Price"), CurrencyManager.PrimaryStoreCurrency.CurrencyCode);
                this.FillDropDowns();
                this.BindData();
            }
        }

        public void SaveInfo()
        {
            
        }

        protected void btnNewPrice_Click(object sender, EventArgs e)
        {
            try
            {
                var productVariant = ProductManager.GetProductVariantById(this.ProductVariantId);
                if (productVariant != null)
                {
                    int customerRoleId = int.Parse(ddlNewCustomerRole.SelectedItem.Value);
                    decimal price = txtNewPrice.Value;
                    var crpp = ProductManager.InsertCustomerRoleProductPrice(customerRoleId,
                        productVariant.ProductVariantId,
                        price);

                    BindData();
                }
            }
            catch (Exception exc)
            {
                processAjaxError(exc);
            }
        }

        protected void gvPrices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateCustomerRolePrice")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvPrices.Rows[index];

                HiddenField hfCustomerRoleProductPriceId = row.FindControl("hfCustomerRoleProductPriceId") as HiddenField;
                DecimalTextBox txtPrice = row.FindControl("txtPrice") as DecimalTextBox;

                int crppId = int.Parse(hfCustomerRoleProductPriceId.Value);
                decimal price = txtPrice.Value;

                var crpp = ProductManager.GetCustomerRoleProductPriceById(crppId);
                if (crpp != null)
                {
                    ProductManager.UpdateCustomerRoleProductPrice(crpp.CustomerRoleProductPriceId,
                       crpp.CustomerRoleId,
                       crpp.ProductVariantId, price);
                }

                BindData();
            }
        }

        protected void gvPrices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var tierPrice = (CustomerRoleProductPrice)e.Row.DataItem;

                Button btnUpdate = e.Row.FindControl("btnUpdate") as Button;
                if (btnUpdate != null)
                    btnUpdate.CommandArgument = e.Row.RowIndex.ToString();

                Label lblCustomerRole = e.Row.FindControl("lblCustomerRole") as Label;
                if (lblCustomerRole != null)
                {
                    CustomerRole cr = CustomerManager.GetCustomerRoleById(tierPrice.CustomerRoleId);
                    if (cr != null)
                    {
                        lblCustomerRole.Text = Server.HtmlEncode(cr.Name);
                    }
                }

            }
        }

        protected void gvPrices_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int crppId = (int)gvPrices.DataKeys[e.RowIndex]["CustomerRoleProductPriceId"];
            ProductManager.DeleteCustomerRoleProductPrice(crppId);
            BindData();
        }

        protected void processAjaxError(Exception exc)
        {
            ProcessException(exc, false);
            pnlError.Visible = true;
            lErrorTitle.Text = exc.Message;
        }

        public int ProductVariantId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductVariantId");
            }
        }
    }
}