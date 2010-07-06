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
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.Common.Utils;
using System.Web.UI.WebControls;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class SpecificationAttributeOptionsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            var specificationAttribute = SpecificationAttributeManager.GetSpecificationAttributeById(this.SpecificationAttributeId);
            if (specificationAttribute != null)
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

                var saoCol = SpecificationAttributeManager.GetSpecificationAttributeOptionsBySpecificationAttribute(SpecificationAttributeId);
                if (saoCol.Count > 0)
                {
                    grdSpecificationAttributeOptions.Visible = true;
                    grdSpecificationAttributeOptions.DataSource = saoCol;
                    grdSpecificationAttributeOptions.DataBind(); 
                
                }
                else
                    grdSpecificationAttributeOptions.Visible = false;
            }
            else
            {
                pnlData.Visible = false;
                pnlMessage.Visible = true;
                lblMessage.Text = GetLocaleResourceString("Admin.SpecificationAttributeOptions.AvailableAfterSaving");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
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
                var specificationAttribute = SpecificationAttributeManager.GetSpecificationAttributeById(this.SpecificationAttributeId);
                if (specificationAttribute != null)
                {
                    var sao = SpecificationAttributeManager.InsertSpecificationAttributeOption(specificationAttribute.SpecificationAttributeId,
                        txtNewOptionName.Text, txtNewOptionDisplayOrder.Value);

                    SaveLocalizableContent(sao);

                    string url = string.Format("SpecificationAttributeDetails.aspx?SpecificationAttributeID={0}&TabID={1}", specificationAttribute.SpecificationAttributeId, "pnlOptions");
                    Response.Redirect(url);
                }
            }
            catch (Exception exc)
            {
                processAjaxError(exc);
            }
        }

        protected void SaveLocalizableContent(SpecificationAttributeOption sao)
        {
            if (sao == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtNewLocalizedOptionName = (TextBox)item.FindControl("txtNewLocalizedOptionName");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtNewLocalizedOptionName.Text;

                    bool allFieldsAreEmpty = string.IsNullOrEmpty(name);

                    var content = SpecificationAttributeManager.GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionIdAndLanguageId(sao.SpecificationAttributeOptionId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = SpecificationAttributeManager.InsertSpecificationAttributeOptionLocalized(sao.SpecificationAttributeOptionId,
                                   languageId, name);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content = SpecificationAttributeManager.UpdateSpecificationAttributeOptionLocalized(content.SpecificationAttributeOptionLocalizedId,
                                content.SpecificationAttributeOptionId, languageId, name);
                        }
                    }
                }
            }
        }

        protected void SaveLocalizableContentGrid(SpecificationAttributeOption sao)
        {
            if (sao == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (GridViewRow row in grdSpecificationAttributeOptions.Rows)
            {
                Repeater rptrLanguageDivs2 = row.FindControl("rptrLanguageDivs2") as Repeater;
                if (rptrLanguageDivs2 != null)
                {
                    HiddenField hfSpecificationAttributeOptionId = row.FindControl("hfSpecificationAttributeOptionId") as HiddenField;
                    int saoId = int.Parse(hfSpecificationAttributeOptionId.Value);
                    if (saoId == sao.SpecificationAttributeOptionId)
                    {
                        foreach (RepeaterItem item in rptrLanguageDivs2.Items)
                        {
                            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                            {
                                var txtLocalizedOptionName = (TextBox)item.FindControl("txtLocalizedOptionName");
                                var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                                int languageId = int.Parse(lblLanguageId.Text);
                                string name = txtLocalizedOptionName.Text;

                                bool allFieldsAreEmpty = string.IsNullOrEmpty(name);

                                var content = SpecificationAttributeManager.GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionIdAndLanguageId(sao.SpecificationAttributeOptionId, languageId);
                                if (content == null)
                                {
                                    if (!allFieldsAreEmpty && languageId > 0)
                                    {
                                        //only insert if one of the fields are filled out (avoid too many empty records in db...)
                                        content = SpecificationAttributeManager.InsertSpecificationAttributeOptionLocalized(sao.SpecificationAttributeOptionId,
                                               languageId, name);
                                    }
                                }
                                else
                                {
                                    if (languageId > 0)
                                    {
                                        content = SpecificationAttributeManager.UpdateSpecificationAttributeOptionLocalized(content.SpecificationAttributeOptionLocalizedId,
                                            content.SpecificationAttributeOptionId, languageId, name);
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

        protected void rptrLanguageDivs2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var txtLocalizedOptionName = (TextBox)e.Item.FindControl("txtLocalizedOptionName");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");
                var hfSpecificationAttributeOptionId = (HiddenField)e.Item.Parent.Parent.FindControl("hfSpecificationAttributeOptionId");

                int languageId = int.Parse(lblLanguageId.Text);
                int saoId = Convert.ToInt32(hfSpecificationAttributeOptionId.Value);
                SpecificationAttributeOption sao = SpecificationAttributeManager.GetSpecificationAttributeOptionById(saoId);
                if (sao != null)
                {
                    var content = SpecificationAttributeManager.GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionIdAndLanguageId(saoId, languageId);
                    if (content != null)
                    {
                        txtLocalizedOptionName.Text = content.Name;
                    }
                }
            }
        }

        protected void OnSpecificationAttributeOptionsCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateOption")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = grdSpecificationAttributeOptions.Rows[index];
                SimpleTextBox txtName = row.FindControl("txtOptionName") as SimpleTextBox;
                NumericTextBox txtDisplayOrder = row.FindControl("txtOptionDisplayOrder") as NumericTextBox;
                HiddenField hfSpecificationAttributeOptionId = row.FindControl("hfSpecificationAttributeOptionId") as HiddenField;

                string name = txtName.Text;
                int displayOrder = txtDisplayOrder.Value;
                int saoId = int.Parse(hfSpecificationAttributeOptionId.Value);

                SpecificationAttributeOption sao = SpecificationAttributeManager.GetSpecificationAttributeOptionById(saoId);
                if (sao != null)
                {
                    sao = SpecificationAttributeManager.UpdateSpecificationAttributeOptions(saoId, SpecificationAttributeId, name, displayOrder);
                    SaveLocalizableContentGrid(sao);
                }

                BindData();
            }
        }
        
        protected void OnSpecificationAttributeOptionsDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int saoId = (int)grdSpecificationAttributeOptions.DataKeys[e.RowIndex]["SpecificationAttributeOptionId"];
            SpecificationAttributeManager.DeleteSpecificationAttributeOption(saoId);
            BindData();
        }

        protected void OnSpecificationAttributeOptionsDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
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

        protected void processAjaxError(Exception exc)
        {
            ProcessException(exc, false);
            pnlError.Visible = true;
            lErrorTitle.Text = exc.Message;
        }

        public int SpecificationAttributeId
        {
            get
            {
                return CommonHelper.QueryStringInt("SpecificationAttributeId");
            }
        }
    }
}