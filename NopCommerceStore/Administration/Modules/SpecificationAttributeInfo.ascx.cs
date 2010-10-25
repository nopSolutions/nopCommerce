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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class SpecificationAttributeInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            var specificationAttribute = IoCFactory.Resolve<ISpecificationAttributeManager>().GetSpecificationAttributeById(this.SpecificationAttributeId);

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
            SpecificationAttribute specificationAttribute = IoCFactory.Resolve<ISpecificationAttributeManager>().GetSpecificationAttributeById(this.SpecificationAttributeId);

            if (specificationAttribute != null)
            {
                specificationAttribute.Name = txtName.Text;
                specificationAttribute.DisplayOrder =  txtDisplayOrder.Value;
                IoCFactory.Resolve<ISpecificationAttributeManager>().UpdateSpecificationAttribute(specificationAttribute);
            }
            else
            {
                specificationAttribute = new SpecificationAttribute()
                {
                    Name = txtName.Text,
                    DisplayOrder = txtDisplayOrder.Value
                };
                IoCFactory.Resolve<ISpecificationAttributeManager>().InsertSpecificationAttribute(specificationAttribute);
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

                    var content = IoCFactory.Resolve<ISpecificationAttributeManager>().GetSpecificationAttributeLocalizedBySpecificationAttributeIdAndLanguageId(specificationAttribute.SpecificationAttributeId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = new SpecificationAttributeLocalized()
                            {
                                SpecificationAttributeId = specificationAttribute.SpecificationAttributeId,
                                LanguageId = languageId,
                                Name = name
                            };
                            IoCFactory.Resolve<ISpecificationAttributeManager>().InsertSpecificationAttributeLocalized(content);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content.LanguageId = languageId;
                            content.Name = name;
                            IoCFactory.Resolve<ISpecificationAttributeManager>().UpdateSpecificationAttributeLocalized(content);
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

                var content = IoCFactory.Resolve<ISpecificationAttributeManager>().GetSpecificationAttributeLocalizedBySpecificationAttributeIdAndLanguageId(this.SpecificationAttributeId, languageId);

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