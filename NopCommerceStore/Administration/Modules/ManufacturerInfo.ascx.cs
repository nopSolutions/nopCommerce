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
using FredCK.FCKeditorV2;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ManufacturerInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            var manufacturer = this.ManufacturerService.GetManufacturerById(this.ManufacturerId);

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
                this.txtDescription.Value = manufacturer.Description;
                CommonHelper.SelectListItem(this.ddlTemplate, manufacturer.TemplateId);

                var manufacturerPicture = manufacturer.Picture;
                btnRemoveManufacturerImage.Visible = manufacturerPicture != null;
                string pictureUrl = this.PictureService.GetPictureUrl(manufacturerPicture, 100);
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
            var manufacturerTemplates = this.TemplateService.GetAllManufacturerTemplates();
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
            var manufacturer = this.ManufacturerService.GetManufacturerById(this.ManufacturerId);

            if (manufacturer != null)
            {
                Picture manufacturerPicture = manufacturer.Picture;
                HttpPostedFile manufacturerPictureFile = fuManufacturerPicture.PostedFile;
                if ((manufacturerPictureFile != null) && (!String.IsNullOrEmpty(manufacturerPictureFile.FileName)))
                {
                    byte[] manufacturerPictureBinary = manufacturerPictureFile.GetPictureBits();
                    if (manufacturerPicture != null)
                        manufacturerPicture = this.PictureService.UpdatePicture(manufacturerPicture.PictureId, manufacturerPictureBinary, manufacturerPictureFile.ContentType, true);
                    else
                        manufacturerPicture = this.PictureService.InsertPicture(manufacturerPictureBinary, manufacturerPictureFile.ContentType, true);
                }
                int manufacturerPictureId = 0;
                if (manufacturerPicture != null)
                    manufacturerPictureId = manufacturerPicture.PictureId;

                manufacturer.Name = txtName.Text;
                manufacturer.Description = txtDescription.Value;
                manufacturer.TemplateId = int.Parse(this.ddlTemplate.SelectedItem.Value);
                manufacturer.PictureId = manufacturerPictureId;
                manufacturer.PriceRanges = txtPriceRanges.Text;
                manufacturer.Published = cbPublished.Checked;
                manufacturer.DisplayOrder = txtDisplayOrder.Value;
                manufacturer.UpdatedOn = DateTime.UtcNow;
                this.ManufacturerService.UpdateManufacturer(manufacturer);
            }
            else
            {
                Picture manufacturerPicture = null;
                HttpPostedFile manufacturerPictureFile = fuManufacturerPicture.PostedFile;
                if ((manufacturerPictureFile != null) && (!String.IsNullOrEmpty(manufacturerPictureFile.FileName)))
                {
                    byte[] manufacturerPictureBinary = manufacturerPictureFile.GetPictureBits();
                    manufacturerPicture = this.PictureService.InsertPicture(manufacturerPictureBinary, manufacturerPictureFile.ContentType, true);
                }
                int manufacturerPictureId = 0;
                if (manufacturerPicture != null)
                    manufacturerPictureId = manufacturerPicture.PictureId;

                DateTime nowDt = DateTime.UtcNow;

                manufacturer = new Manufacturer()
                {
                    Name = txtName.Text,
                    Description = txtDescription.Value,
                    TemplateId = int.Parse(this.ddlTemplate.SelectedItem.Value),
                    PictureId = manufacturerPictureId,
                    PageSize = 10,
                    PriceRanges = txtPriceRanges.Text,
                    Published = cbPublished.Checked,
                    DisplayOrder = txtDisplayOrder.Value,
                    CreatedOn = nowDt,
                    UpdatedOn = nowDt
                };
                this.ManufacturerService.InsertManufacturer(manufacturer);
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
                    var txtLocalizedDescription = (FCKeditor)item.FindControl("txtLocalizedDescription");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtLocalizedName.Text;
                    string description = txtLocalizedDescription.Value;

                    bool allFieldsAreEmpty = (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description));

                    var content = this.ManufacturerService.GetManufacturerLocalizedByManufacturerIdAndLanguageId(manufacturer.ManufacturerId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = new ManufacturerLocalized()
                            {
                                ManufacturerId = manufacturer.ManufacturerId,
                                LanguageId = languageId,
                                Name = name,
                                Description = description
                            };

                            this.ManufacturerService.InsertManufacturerLocalized(content);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content.LanguageId = languageId;
                            content.Name = name;
                            content.Description = description;

                            this.ManufacturerService.UpdateManufacturerLocalized(content);
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
                var txtLocalizedDescription = (FCKeditor)e.Item.FindControl("txtLocalizedDescription");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                var content = this.ManufacturerService.GetManufacturerLocalizedByManufacturerIdAndLanguageId(this.ManufacturerId, languageId);

                if (content != null)
                {
                    txtLocalizedName.Text = content.Name;
                    txtLocalizedDescription.Value = content.Description;
                }

            }
        }

        protected void btnRemoveManufacturerImage_Click(object sender, EventArgs e)
        {
            try
            {
                Manufacturer manufacturer = this.ManufacturerService.GetManufacturerById(this.ManufacturerId);
                if (manufacturer != null)
                {
                    this.PictureService.DeletePicture(manufacturer.PictureId);

                    manufacturer.PictureId = 0;
                    this.ManufacturerService.UpdateManufacturer(manufacturer);
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