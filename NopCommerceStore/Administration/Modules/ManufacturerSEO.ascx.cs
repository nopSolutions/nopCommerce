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
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ManufacturerSEOControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Manufacturer manufacturer = ManufacturerManager.GetManufacturerById(this.ManufacturerId);

            if (this.HasLocalizableContent)
            {
                var languages = this.GetLocalizableLanguagesSupported();
                rptrLanguageTabs.DataSource = languages;
                rptrLanguageTabs.DataBind();
                rptrLanguageDivs.DataSource = languages;
                rptrLanguageDivs.DataBind();
            }
            
            if (manufacturer != null)
            {
                this.txtMetaKeywords.Text = manufacturer.MetaKeywords;
                this.txtMetaDescription.Text = manufacturer.MetaDescription;
                this.txtMetaTitle.Text = manufacturer.MetaTitle;
                this.txtSEName.Text = manufacturer.SEName;
                this.txtPageSize.Value = manufacturer.PageSize;
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
            SaveInfo(this.ManufacturerId);
        }

        public void SaveInfo(int manId)
        {
            Manufacturer manufacturer = ManufacturerManager.GetManufacturerById(manId);

            if (manufacturer != null)
            {
                manufacturer = ManufacturerManager.UpdateManufacturer(manufacturer.ManufacturerId, manufacturer.Name, manufacturer.Description,
                   manufacturer.TemplateId, txtMetaKeywords.Text, txtMetaDescription.Text,
                   txtMetaTitle.Text, txtSEName.Text, manufacturer.PictureId, txtPageSize.Value,
                   manufacturer.PriceRanges, manufacturer.Published,
                   manufacturer.Deleted, manufacturer.DisplayOrder, manufacturer.CreatedOn, manufacturer.UpdatedOn);
            }

            SaveLocalizableContent(manufacturer);
        }

        protected void SaveLocalizableContent(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtLocalizedMetaKeywords = (TextBox)item.FindControl("txtLocalizedMetaKeywords");
                    var txtLocalizedMetaDescription = (TextBox)item.FindControl("txtLocalizedMetaDescription");
                    var txtLocalizedMetaTitle = (TextBox)item.FindControl("txtLocalizedMetaTitle");
                    var txtLocalizedSEName = (TextBox)item.FindControl("txtLocalizedSEName");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string metaKeywords = txtLocalizedMetaKeywords.Text;
                    string metaDescription = txtLocalizedMetaDescription.Text;
                    string metaTitle = txtLocalizedMetaTitle.Text;
                    string seName = txtLocalizedSEName.Text;

                    bool allFieldsAreEmpty = (string.IsNullOrEmpty(metaKeywords) &&
                        string.IsNullOrEmpty(metaDescription) &&
                        string.IsNullOrEmpty(metaTitle) &&
                        string.IsNullOrEmpty(seName));

                    var content = ManufacturerManager.GetManufacturerLocalizedByManufacturerIdAndLanguageId(manufacturer.ManufacturerId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = ManufacturerManager.InsertManufacturerLocalized(manufacturer.ManufacturerId,
                                   languageId, string.Empty, string.Empty,
                                   metaKeywords, metaDescription, metaTitle, seName);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content = ManufacturerManager.UpdateManufacturerLocalized(content.ManufacturerLocalizedId, content.ManufacturerId,
                                languageId, content.Name, content.Description,
                                metaKeywords, metaDescription,
                                metaTitle, seName);
                        }
                    }
                }
            }
        }

        protected void rptrLanguageDivs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var txtLocalizedMetaKeywords = (TextBox)e.Item.FindControl("txtLocalizedMetaKeywords");
                var txtLocalizedMetaDescription = (TextBox)e.Item.FindControl("txtLocalizedMetaDescription");
                var txtLocalizedMetaTitle = (TextBox)e.Item.FindControl("txtLocalizedMetaTitle");
                var txtLocalizedSEName = (TextBox)e.Item.FindControl("txtLocalizedSEName");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                var content = ManufacturerManager.GetManufacturerLocalizedByManufacturerIdAndLanguageId(this.ManufacturerId, languageId);
                if (content != null)
                {
                    txtLocalizedMetaKeywords.Text = content.MetaKeywords;
                    txtLocalizedMetaDescription.Text = content.MetaDescription;
                    txtLocalizedMetaTitle.Text = content.MetaTitle;
                    txtLocalizedSEName.Text = content.SEName;
                }
            }
        }
        
        public int ManufacturerId
        {
            get
            {
                return CommonHelper.QueryStringInt("ManufacturerId");
            }
        }
    }
}