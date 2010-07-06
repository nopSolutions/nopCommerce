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
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductSpecificationsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Product product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                pnlData.Visible = true;
                pnlMessage.Visible = false;

                var productSpecificationAttributes = SpecificationAttributeManager.GetProductSpecificationAttributesByProductId(product.ProductId);
                if (productSpecificationAttributes.Count > 0)
                {
                    gvProductSpecificationAttributes.Visible = true;
                    gvProductSpecificationAttributes.DataSource = productSpecificationAttributes;
                    gvProductSpecificationAttributes.DataBind();
                }
                else
                    gvProductSpecificationAttributes.Visible = false;
            }
            else
            {
                pnlData.Visible = false;
                pnlMessage.Visible = true;
            }
        }

        private void FillDropDowns()
        {
            this.ddlNewProductSpecificationAttribute.Items.Clear();
            var productSpecificationAttributes = SpecificationAttributeManager.GetSpecificationAttributes();
            foreach (SpecificationAttribute sa in productSpecificationAttributes)
            {
                ListItem item2 = new ListItem(sa.Name, sa.SpecificationAttributeId.ToString());
                this.ddlNewProductSpecificationAttribute.Items.Add(item2);
            }

            ddlNewProductSpecificationAttributeOption.Items.Clear();
            if (!String.IsNullOrEmpty(ddlNewProductSpecificationAttribute.SelectedValue))
            {
                int saId = Convert.ToInt32(ddlNewProductSpecificationAttribute.SelectedValue.ToString());
                var saoCol = SpecificationAttributeManager.GetSpecificationAttributeOptionsBySpecificationAttribute(saId);
                foreach (SpecificationAttributeOption sao in saoCol)
                {
                    ListItem item2 = new ListItem(sao.Name, sao.SpecificationAttributeOptionId.ToString());
                    this.ddlNewProductSpecificationAttributeOption.Items.Add(item2);
                }
            }
        }

        protected void OnSpecificationAttributeIndexChanged(object sender, EventArgs e)
        {
            ddlNewProductSpecificationAttributeOption.Items.Clear();
            int saId = Convert.ToInt32(ddlNewProductSpecificationAttribute.SelectedValue.ToString());
            var saoCol = SpecificationAttributeManager.GetSpecificationAttributeOptionsBySpecificationAttribute(saId);
            foreach (SpecificationAttributeOption sao in saoCol)
            {
                ListItem item2 = new ListItem(sao.Name, sao.SpecificationAttributeOptionId.ToString());
                this.ddlNewProductSpecificationAttributeOption.Items.Add(item2);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {                
                this.BindData();
                this.FillDropDowns();
            }            
        }

        public void SaveInfo()
        {
        }

        protected void btnNewProductSpecification_Click(object sender, EventArgs e)
        {
            try
            {
                Product product = ProductManager.GetProductById(this.ProductId);
                if (product != null)
                {
                    if (String.IsNullOrEmpty(ddlNewProductSpecificationAttribute.SelectedValue))
                        throw new NopException("Please select specification attribute");
                    if (String.IsNullOrEmpty(ddlNewProductSpecificationAttributeOption.SelectedValue))
                        throw new NopException("Please select specification attribute option");

                    int specificationAttributeId = int.Parse(ddlNewProductSpecificationAttribute.SelectedItem.Value);
                    int productSpecificationAttributeOptionId = Convert.ToInt32(ddlNewProductSpecificationAttributeOption.SelectedValue);
                    bool allowFiltering = chkNewAllowFiltering.Checked;
                    bool showOnProductPage = chkNewShowOnProductPage.Checked;
                    int productSpecificationAttributeDisplayOrder = txtNewProductSpecificationAttributeDisplayOrder.Value;
                   
                    ProductSpecificationAttribute productSpecificationAttribute = SpecificationAttributeManager.InsertProductSpecificationAttribute(
                        product.ProductId,
                        productSpecificationAttributeOptionId,
                        allowFiltering,
                        showOnProductPage,
                        productSpecificationAttributeDisplayOrder);

                    BindData();
                }
            }
            catch (Exception exc)
            {
                processAjaxError(exc);
            }
        }

        protected void gvProductSpecificationAttributes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateProductSpecificationAttribute")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvProductSpecificationAttributes.Rows[index];

                HiddenField hfProductSpecificationAttributeId = row.FindControl("hfProductSpecificationAttributeId") as HiddenField;
                Literal lblSpecificationAttributeName = row.FindControl("lblSpecificationAttributeName") as Literal;
                DropDownList ddlSpecificationAttributeOption = row.FindControl("ddlSpecificationAttributeOption") as DropDownList;
                CheckBox chkAllowFiltering = row.FindControl("chkAllowFiltering") as CheckBox;
                CheckBox chkShowOnProductPage = row.FindControl("chkShowOnProductPage") as CheckBox;
                NumericTextBox txtProductSpecificationAttributeDisplayOrder = row.FindControl("txtProductSpecificationAttributeDisplayOrder") as NumericTextBox;

                int productSpecificationAttributeId = int.Parse(hfProductSpecificationAttributeId.Value);
                int saoId = int.Parse(ddlSpecificationAttributeOption.SelectedItem.Value);
                int displayOrder = txtProductSpecificationAttributeDisplayOrder.Value;

                ProductSpecificationAttribute productSpecificationAttribute = SpecificationAttributeManager.GetProductSpecificationAttributeById(productSpecificationAttributeId);

                if (productSpecificationAttribute != null)
                    SpecificationAttributeManager.UpdateProductSpecificationAttribute(
                        productSpecificationAttribute.ProductSpecificationAttributeId,
                        productSpecificationAttribute.ProductId,
                        saoId,
                        chkAllowFiltering.Checked,
                        chkShowOnProductPage.Checked,
                        displayOrder);

                BindData();
            }
        }

        protected void gvProductSpecificationAttributes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ProductSpecificationAttribute productSpecificationAttribute = (ProductSpecificationAttribute)e.Row.DataItem;

                Button btnUpdate = e.Row.FindControl("btnUpdate") as Button;
                if (btnUpdate != null)
                    btnUpdate.CommandArgument = e.Row.RowIndex.ToString();

                SpecificationAttributeOption sao = SpecificationAttributeManager.GetSpecificationAttributeOptionById(productSpecificationAttribute.SpecificationAttributeOptionId);
                SpecificationAttribute sa = SpecificationAttributeManager.GetSpecificationAttributeById(sao.SpecificationAttributeId);                
                Literal lblSpecificationAttributeName = e.Row.FindControl("lblSpecificationAttributeName") as Literal;
                lblSpecificationAttributeName.Text = sa.Name;

                DropDownList ddlSpecificationAttributeOption = e.Row.FindControl("ddlSpecificationAttributeOption") as DropDownList;
                ddlSpecificationAttributeOption.Items.Clear();
                var saoCol = SpecificationAttributeManager.GetSpecificationAttributeOptionsBySpecificationAttribute(sao.SpecificationAttributeId);
                foreach (SpecificationAttributeOption sao1 in saoCol)
                {
                    ListItem item = new ListItem(sao1.Name, sao1.SpecificationAttributeOptionId.ToString());
                    ddlSpecificationAttributeOption.Items.Add(item);
                    if (productSpecificationAttribute.SpecificationAttributeOptionId == sao1.SpecificationAttributeOptionId)
                        item.Selected = true;
                }

                CheckBox chkAllowFiltering = e.Row.FindControl("chkAllowFiltering") as CheckBox;
                chkAllowFiltering.Checked = productSpecificationAttribute.AllowFiltering;

                CheckBox chkShowOnProductPage = e.Row.FindControl("chkShowOnProductPage") as CheckBox;
                chkShowOnProductPage.Checked = productSpecificationAttribute.ShowOnProductPage;
            }
        }

        protected void gvProductSpecificationAttributes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int productSpecificationAttributeId = (int)gvProductSpecificationAttributes.DataKeys[e.RowIndex]["ProductSpecificationAttributeId"];
            ProductSpecificationAttribute productSpecificationAttribute = SpecificationAttributeManager.GetProductSpecificationAttributeById(productSpecificationAttributeId);
            if (productSpecificationAttribute != null)
            {
                SpecificationAttributeManager.DeleteProductSpecificationAttribute(productSpecificationAttribute.ProductSpecificationAttributeId);
                BindData();
            }
        }
        
        protected void processAjaxError(Exception exc)
        {
            ProcessException(exc, false);
            pnlError.Visible = true;
            lErrorTitle.Text = exc.Message;
        }

        public int ProductId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductId");
            }
        }
    }
}