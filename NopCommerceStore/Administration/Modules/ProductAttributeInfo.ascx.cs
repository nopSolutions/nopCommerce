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
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductAttributeInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            ProductAttribute productAttribute = ProductAttributeManager.GetProductAttributeById(this.ProductAttributeId);

            if (this.HasLocalizableContent)
            {
                var languages = this.GetLocalizableLanguagesSupported();
                rptrLanguageTabs.DataSource = languages;
                rptrLanguageTabs.DataBind();
                rptrLanguageDivs.DataSource = languages;
                rptrLanguageDivs.DataBind();
            }
            
            if (productAttribute != null)
            {
                this.txtName.Text = productAttribute.Name;
                this.txtDescription.Text = productAttribute.Description;
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

        public ProductAttribute SaveInfo()
        {
            ProductAttribute productAttribute = ProductAttributeManager.GetProductAttributeById(this.ProductAttributeId);

            if (productAttribute != null)
            {
                productAttribute = ProductAttributeManager.UpdateProductAttribute(productAttribute.ProductAttributeId,
                    txtName.Text, txtDescription.Text);
            }
            else
            {
                productAttribute = ProductAttributeManager.InsertProductAttribute(txtName.Text, txtDescription.Text);
            }

            SaveLocalizableContent(productAttribute);

            return productAttribute;
        }

        protected void SaveLocalizableContent(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtLocalizedName = (TextBox)item.FindControl("txtLocalizedName");
                    var txtLocalizedDescription = (TextBox)item.FindControl("txtLocalizedDescription");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtLocalizedName.Text;
                    string description = txtLocalizedDescription.Text;

                    bool allFieldsAreEmpty = (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description));

                    var content = ProductAttributeManager.GetProductAttributeLocalizedByProductAttributeIdAndLanguageId(productAttribute.ProductAttributeId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = ProductAttributeManager.InsertProductAttributeLocalized(productAttribute.ProductAttributeId,
                                   languageId, name, description);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content = ProductAttributeManager.UpdateProductAttributeLocalized(content.ProductAttributeLocalizedId, content.ProductAttributeId,
                                languageId, name, description);
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
                var txtLocalizedDescription = (TextBox)e.Item.FindControl("txtLocalizedDescription");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                var content = ProductAttributeManager.GetProductAttributeLocalizedByProductAttributeIdAndLanguageId(this.ProductAttributeId, languageId);

                if (content != null)
                {
                    txtLocalizedName.Text = content.Name;
                    txtLocalizedDescription.Text = content.Description;
                }

            }
        }

        public int ProductAttributeId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductAttributeId");
            }
        }
    }
}