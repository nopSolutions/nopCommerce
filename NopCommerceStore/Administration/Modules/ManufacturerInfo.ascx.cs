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
    public partial class ManufacturerInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            var manufacturer = ManufacturerManager.GetManufacturerById(this.ManufacturerId);

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
                this.txtName.Text = manufacturer.Name;
                this.txtDescription.Content = manufacturer.Description;
                CommonHelper.SelectListItem(this.ddlTemplate, manufacturer.TemplateId);

                var manufacturerPicture = manufacturer.Picture;
                btnRemoveManufacturerImage.Visible = manufacturerPicture != null;
                string pictureUrl = PictureManager.GetPictureUrl(manufacturerPicture, 100);
                this.iManufacturerPicture.Visible = true;
                this.iManufacturerPicture.ImageUrl = pictureUrl;

                this.txtPriceRanges.Text = manufacturer.PriceRanges;
                this.cbPublished.Checked = manufacturer.Published;
                this.txtDisplayOrder.Value = manufacturer.DisplayOrder;
            }
            else
            {
                this.btnRemoveManufacturerImage.Visible = false;
                this.iManufacturerPicture.Visible = false;
            }
        }

        private void FillDropDowns()
        {
            this.ddlTemplate.Items.Clear();
            var manufacturerTemplates = TemplateManager.GetAllManufacturerTemplates();
            foreach (var manufacturerTemplate in manufacturerTemplates)
            {
                ListItem item2 = new ListItem(manufacturerTemplate.Name, manufacturerTemplate.ManufacturerTemplateId.ToString());
                this.ddlTemplate.Items.Add(item2);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();
            BindJQueryIdTabs();

            base.OnPreRender(e);
        }
        
        public Manufacturer SaveInfo()
        {
            var manufacturer = ManufacturerManager.GetManufacturerById(this.ManufacturerId);

            if (manufacturer != null)
            {
                Picture manufacturerPicture = manufacturer.Picture;
                HttpPostedFile manufacturerPictureFile = fuManufacturerPicture.PostedFile;
                if ((manufacturerPictureFile != null) && (!String.IsNullOrEmpty(manufacturerPictureFile.FileName)))
                {
                    byte[] manufacturerPictureBinary = PictureManager.GetPictureBits(manufacturerPictureFile.InputStream, manufacturerPictureFile.ContentLength);
                    if (manufacturerPicture != null)
                        manufacturerPicture = PictureManager.UpdatePicture(manufacturerPicture.PictureId, manufacturerPictureBinary, manufacturerPictureFile.ContentType, true);
                    else
                        manufacturerPicture = PictureManager.InsertPicture(manufacturerPictureBinary, manufacturerPictureFile.ContentType, true);
                }
                int manufacturerPictureId = 0;
                if (manufacturerPicture != null)
                    manufacturerPictureId = manufacturerPicture.PictureId;

                manufacturer = ManufacturerManager.UpdateManufacturer(manufacturer.ManufacturerId, txtName.Text,
                    txtDescription.Content, int.Parse(this.ddlTemplate.SelectedItem.Value),
                    manufacturer.MetaKeywords, manufacturer.MetaDescription,
                    manufacturer.MetaTitle, manufacturer.SEName,
                    manufacturerPictureId, manufacturer.PageSize, txtPriceRanges.Text,
                    cbPublished.Checked, manufacturer.Deleted, txtDisplayOrder.Value,
                    manufacturer.CreatedOn, DateTime.UtcNow);
            }
            else
            {
                Picture manufacturerPicture = null;
                HttpPostedFile manufacturerPictureFile = fuManufacturerPicture.PostedFile;
                if ((manufacturerPictureFile != null) && (!String.IsNullOrEmpty(manufacturerPictureFile.FileName)))
                {
                    byte[] manufacturerPictureBinary = PictureManager.GetPictureBits(manufacturerPictureFile.InputStream, manufacturerPictureFile.ContentLength);
                    manufacturerPicture = PictureManager.InsertPicture(manufacturerPictureBinary, manufacturerPictureFile.ContentType, true);
                }
                int manufacturerPictureId = 0;
                if (manufacturerPicture != null)
                    manufacturerPictureId = manufacturerPicture.PictureId;

                DateTime nowDt = DateTime.UtcNow;
                manufacturer = ManufacturerManager.InsertManufacturer(txtName.Text, txtDescription.Content,
                    int.Parse(this.ddlTemplate.SelectedItem.Value),
                    string.Empty, string.Empty, string.Empty, string.Empty,
                    manufacturerPictureId, 10, txtPriceRanges.Text, cbPublished.Checked, false, txtDisplayOrder.Value, nowDt, nowDt);
            }

            SaveLocalizableContent(manufacturer);

            return manufacturer;
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
                    var txtLocalizedName = (TextBox)item.FindControl("txtLocalizedName");
                    var txtLocalizedDescription = (AjaxControlToolkit.HTMLEditor.Editor)item.FindControl("txtLocalizedDescription");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtLocalizedName.Text;
                    string description = txtLocalizedDescription.Content;

                    bool allFieldsAreEmpty = (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description));

                    var content = ManufacturerManager.GetManufacturerLocalizedByManufacturerIdAndLanguageId(manufacturer.ManufacturerId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = ManufacturerManager.InsertManufacturerLocalized(manufacturer.ManufacturerId,
                                   languageId, name, description, string.Empty, string.Empty,
                                   string.Empty, string.Empty);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content = ManufacturerManager.UpdateManufacturerLocalized(content.ManufacturerLocalizedId, content.ManufacturerId,
                                languageId, name, description,
                                content.MetaKeywords, content.MetaDescription,
                                content.MetaTitle, content.SEName);
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
                var txtLocalizedDescription = (AjaxControlToolkit.HTMLEditor.Editor)e.Item.FindControl("txtLocalizedDescription");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                var content = ManufacturerManager.GetManufacturerLocalizedByManufacturerIdAndLanguageId(this.ManufacturerId, languageId);

                if (content != null)
                {
                    txtLocalizedName.Text = content.Name;
                    txtLocalizedDescription.Content = content.Description;
                }

            }
        }

        protected void btnRemoveManufacturerImage_Click(object sender, EventArgs e)
        {
            try
            {
                Manufacturer manufacturer = ManufacturerManager.GetManufacturerById(this.ManufacturerId);
                if (manufacturer != null)
                {
                    PictureManager.DeletePicture(manufacturer.PictureId);
                    ManufacturerManager.RemoveManufacturerPicture(manufacturer.ManufacturerId);
                    BindData();
                }
            }
            catch (Exception exc)
            {
                ProcessException(exc);
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