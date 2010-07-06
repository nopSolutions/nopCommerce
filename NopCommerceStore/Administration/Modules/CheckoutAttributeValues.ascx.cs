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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CheckoutAttributeValuesControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            var checkoutAttribute = CheckoutAttributeManager.GetCheckoutAttributeById(this.CheckoutAttributeId);
            if (checkoutAttribute != null)
            {
                if (checkoutAttribute.ShouldHaveValues)
                {
                    pnlData.Visible = true;
                    pnlMessage.Visible = false;

                    if (this.HasLocalizableContent)
                    {
                        var languages = this.GetLocalizableLanguagesSupported();
                        rptrLanguageTabs.DataSource = languages;
                        rptrLanguageTabs.DataBind();
                        rptrLanguageDivs.DataSource = languages;
                        rptrLanguageDivs.DataBind();
                    }

                    var values = CheckoutAttributeManager.GetCheckoutAttributeValues(checkoutAttribute.CheckoutAttributeId);
                    if (values.Count > 0)
                    {
                        gvValues.Visible = true;
                        gvValues.DataSource = values;
                        gvValues.DataBind();
                    }
                    else
                        gvValues.Visible = false;
                }
                else
                {
                    pnlData.Visible = false;
                    pnlMessage.Visible = true;
                    lblMessage.Text = GetLocaleResourceString("Admin.CheckoutAttributeInfo.ValuesNotRequiredForThisControlType");
                }
            }
            else
            {
                pnlData.Visible = false;
                pnlMessage.Visible = true;
                lblMessage.Text = GetLocaleResourceString("Admin.CheckoutAttributeValues.AvailableAfterSaving");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                gvValues.Columns[1].HeaderText = string.Format("{0} [{1}]", GetLocaleResourceString("Admin.CheckoutAttributeInfo.PriceAdjustment"), CurrencyManager.PrimaryStoreCurrency.CurrencyCode);
                this.BindData();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();
            BindJQueryIdTabs();

            base.OnPreRender(e);
        }

        public void SaveInfo()
        {

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var checkoutAttribute = CheckoutAttributeManager.GetCheckoutAttributeById(this.CheckoutAttributeId);
                if (checkoutAttribute != null)
                {
                    var cav = CheckoutAttributeManager.InsertCheckoutAttributeValue(checkoutAttribute.CheckoutAttributeId,
                        txtNewName.Text, txtNewPriceAdjustment.Value, txtNewWeightAdjustment.Value,
                        cbNewIsPreSelected.Checked, txtNewDisplayOrder.Value);

                    SaveLocalizableContent(cav);

                    string url = string.Format("CheckoutAttributeDetails.aspx?CheckoutAttributeID={0}&TabID={1}", checkoutAttribute.CheckoutAttributeId, "pnlValues");
                    Response.Redirect(url);
                }
            }
            catch (Exception exc)
            {
                processAjaxError(exc);
            }
        }

        protected void SaveLocalizableContent(CheckoutAttributeValue cav)
        {
            if (cav == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtNewLocalizedName = (TextBox)item.FindControl("txtNewLocalizedName");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtNewLocalizedName.Text;

                    bool allFieldsAreEmpty = string.IsNullOrEmpty(name);

                    var content = CheckoutAttributeManager.GetCheckoutAttributeValueLocalizedByCheckoutAttributeValueIdAndLanguageId(cav.CheckoutAttributeValueId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = CheckoutAttributeManager.InsertCheckoutAttributeValueLocalized(cav.CheckoutAttributeValueId,
                                   languageId, name);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content = CheckoutAttributeManager.UpdateCheckoutAttributeValueLocalized(content.CheckoutAttributeValueLocalizedId,
                                content.CheckoutAttributeValueId, languageId, name);
                        }
                    }
                }
            }
        }

        protected void SaveLocalizableContentGrid(CheckoutAttributeValue cav)
        {
            if (cav == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (GridViewRow row in gvValues.Rows)
            {
                Repeater rptrLanguageDivs2 = row.FindControl("rptrLanguageDivs2") as Repeater;
                if (rptrLanguageDivs2 != null)
                {
                    HiddenField hfCheckoutAttributeValueId = row.FindControl("hfCheckoutAttributeValueId") as HiddenField;
                    int cavId = int.Parse(hfCheckoutAttributeValueId.Value);
                    if (cavId == cav.CheckoutAttributeValueId)
                    {
                        foreach (RepeaterItem item in rptrLanguageDivs2.Items)
                        {
                            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                            {
                                var txtLocalizedName = (TextBox)item.FindControl("txtLocalizedName");
                                var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                                int languageId = int.Parse(lblLanguageId.Text);
                                string name = txtLocalizedName.Text;

                                bool allFieldsAreEmpty = string.IsNullOrEmpty(name);

                                var content = CheckoutAttributeManager.GetCheckoutAttributeValueLocalizedByCheckoutAttributeValueIdAndLanguageId(cav.CheckoutAttributeValueId, languageId);
                                if (content == null)
                                {
                                    if (!allFieldsAreEmpty && languageId > 0)
                                    {
                                        //only insert if one of the fields are filled out (avoid too many empty records in db...)
                                        content = CheckoutAttributeManager.InsertCheckoutAttributeValueLocalized(cav.CheckoutAttributeValueId,
                                            languageId, name);
                                    }
                                }
                                else
                                {
                                    if (languageId > 0)
                                    {
                                        content = CheckoutAttributeManager.UpdateCheckoutAttributeValueLocalized(content.CheckoutAttributeValueLocalizedId,
                                            content.CheckoutAttributeValueId, languageId, name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void rptrLanguageDivs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void gvValues_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateCheckoutAttributeValue")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvValues.Rows[index];

                HiddenField hfCheckoutAttributeValueId = row.FindControl("hfCheckoutAttributeValueId") as HiddenField;
                SimpleTextBox txtName = row.FindControl("txtName") as SimpleTextBox;
                DecimalTextBox txtPriceAdjustment = row.FindControl("txtPriceAdjustment") as DecimalTextBox;
                DecimalTextBox txtWeightAdjustment = row.FindControl("txtWeightAdjustment") as DecimalTextBox;
                CheckBox cbIsPreSelected = row.FindControl("cbIsPreSelected") as CheckBox;
                NumericTextBox txtDisplayOrder = row.FindControl("txtDisplayOrder") as NumericTextBox;

                int cavId = int.Parse(hfCheckoutAttributeValueId.Value);
                string name = txtName.Text;
                decimal priceAdjustment = txtPriceAdjustment.Value;
                decimal weightAdjustment = txtWeightAdjustment.Value;
                bool isPreSelected = cbIsPreSelected.Checked;
                int displayOrder = txtDisplayOrder.Value;

                var cav = CheckoutAttributeManager.GetCheckoutAttributeValueById(cavId);

                if (cav != null)
                {
                    cav = CheckoutAttributeManager.UpdateCheckoutAttributeValue(cav.CheckoutAttributeValueId,
                        cav.CheckoutAttributeId, name,
                        priceAdjustment, weightAdjustment, isPreSelected, displayOrder);

                    SaveLocalizableContentGrid(cav);
                }
                BindData();
            }
        }

        protected void gvValues_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int cavId = (int)gvValues.DataKeys[e.RowIndex]["CheckoutAttributeValueId"];
            CheckoutAttributeManager.DeleteCheckoutAttributeValue(cavId);
            BindData();
        }

        protected void gvValues_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var cav = (CheckoutAttributeValue)e.Row.DataItem;

                Button btnUpdate = e.Row.FindControl("btnUpdate") as Button;
                if (btnUpdate != null)
                    btnUpdate.CommandArgument = e.Row.RowIndex.ToString();

                Repeater rptrLanguageDivs2 = e.Row.FindControl("rptrLanguageDivs2") as Repeater;
                if (rptrLanguageDivs2 != null)
                {
                    if (this.HasLocalizableContent)
                    {
                        var languages = this.GetLocalizableLanguagesSupported();
                        rptrLanguageDivs2.DataSource = languages;
                        rptrLanguageDivs2.DataBind();
                    }
                }
            }
        }

        protected void rptrLanguageDivs2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var txtLocalizedName = (TextBox)e.Item.FindControl("txtLocalizedName");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");
                var hfCheckoutAttributeValueId = (HiddenField)e.Item.Parent.Parent.FindControl("hfCheckoutAttributeValueId");

                int languageId = int.Parse(lblLanguageId.Text);
                int cavId = Convert.ToInt32(hfCheckoutAttributeValueId.Value);
                var cav = CheckoutAttributeManager.GetCheckoutAttributeValueById(cavId);
                if (cav != null)
                {
                    var content = CheckoutAttributeManager.GetCheckoutAttributeValueLocalizedByCheckoutAttributeValueIdAndLanguageId(cavId, languageId);
                    if (content != null)
                    {
                        txtLocalizedName.Text = content.Name;
                    }
                }
            }
        }

        protected void processAjaxError(Exception exc)
        {
            ProcessException(exc, false);
            pnlError.Visible = true;
            lErrorTitle.Text = exc.Message;
        }

        public int CheckoutAttributeId
        {
            get
            {
                return CommonHelper.QueryStringInt("CheckoutAttributeId");
            }
        }

    }
}