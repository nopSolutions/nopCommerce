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
    public partial class SpecificationAttributeInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            var specificationAttribute = SpecificationAttributeManager.GetSpecificationAttributeById(this.SpecificationAttributeId);

            if (this.HasLocalizableContent)
            {
                var languages = this.GetLocalizableLanguagesSupported();
                rptrLanguageTabs.DataSource = languages;
                rptrLanguageTabs.DataBind();
                rptrLanguageDivs.DataSource = languages;
                rptrLanguageDivs.DataBind();
            }

            if (specificationAttribute != null)
            {
                this.txtName.Text = specificationAttribute.Name;
                this.txtDisplayOrder.Value = specificationAttribute.DisplayOrder;
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

        public SpecificationAttribute SaveInfo()
        {
            SpecificationAttribute specificationAttribute = SpecificationAttributeManager.GetSpecificationAttributeById(this.SpecificationAttributeId);

            if (specificationAttribute != null)
            {
                specificationAttribute = SpecificationAttributeManager.UpdateSpecificationAttribute(specificationAttribute.SpecificationAttributeId, txtName.Text, txtDisplayOrder.Value);
            }
            else
            {
                specificationAttribute = SpecificationAttributeManager.InsertSpecificationAttribute(txtName.Text, txtDisplayOrder.Value);
            }

            SaveLocalizableContent(specificationAttribute);

            return specificationAttribute;
        }

        protected void SaveLocalizableContent(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtLocalizedName = (TextBox)item.FindControl("txtLocalizedName");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtLocalizedName.Text;

                    bool allFieldsAreEmpty = string.IsNullOrEmpty(name);

                    var content = SpecificationAttributeManager.GetSpecificationAttributeLocalizedBySpecificationAttributeIdAndLanguageId(specificationAttribute.SpecificationAttributeId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = SpecificationAttributeManager.InsertSpecificationAttributeLocalized(specificationAttribute.SpecificationAttributeId,
                                   languageId, name);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content = SpecificationAttributeManager.UpdateSpecificationAttributeLocalized(content.SpecificationAttributeLocalizedId, content.SpecificationAttributeId,
                                languageId, name);
                        }
                    }
                }
            }
        }

        protected void rptrLanguageDivs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var txtLocalizedName = (TextBox)e.Item.FindControl("txtLocalizedName");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                var content = SpecificationAttributeManager.GetSpecificationAttributeLocalizedBySpecificationAttributeIdAndLanguageId(this.SpecificationAttributeId, languageId);

                if (content != null)
                {
                    txtLocalizedName.Text = content.Name;
                }

            }
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