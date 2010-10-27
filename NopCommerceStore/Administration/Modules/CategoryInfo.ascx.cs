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
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
 

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CategoryInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            var category = IoCFactory.Resolve<ICategoryManager>().GetCategoryById(this.CategoryId);

            if (this.HasLocalizableContent)
            {
                var languages = this.GetLocalizableLanguagesSupported();
                rptrLanguageTabs.DataSource = languages;
                rptrLanguageTabs.DataBind();
                rptrLanguageDivs.DataSource = languages;
                rptrLanguageDivs.DataBind();
            }

            if (category != null)
            {
                this.txtName.Text = category.Name;
                this.txtDescription.Value = category.Description;
                CommonHelper.SelectListItem(this.ddlTemplate, category.TemplateId);
                ParentCategory.SelectedCategoryId = category.ParentCategoryId;

                Picture categoryPicture = category.Picture;
                this.btnRemoveCategoryImage.Visible = categoryPicture != null;
                string pictureUrl = IoCFactory.Resolve<IPictureManager>().GetPictureUrl(categoryPicture, 100);
                this.iCategoryPicture.Visible = true;
                this.iCategoryPicture.ImageUrl = pictureUrl;

                this.txtPriceRanges.Text = category.PriceRanges;
                this.cbShowOnHomePage.Checked = category.ShowOnHomePage;
                this.cbPublished.Checked = category.Published;
                this.txtDisplayOrder.Value = category.DisplayOrder;
                this.ParentCategory.BindData();
            }
            else
            {
                this.btnRemoveCategoryImage.Visible = false;
                this.iCategoryPicture.Visible = false;

                ParentCategory.SelectedCategoryId = this.ParentCategoryId;
                ParentCategory.BindData();
            }
        }

        private void FillDropDowns()
        {
            this.ddlTemplate.Items.Clear();
            var categoryTemplates = IoCFactory.Resolve<ITemplateManager>().GetAllCategoryTemplates();
            foreach (CategoryTemplate categoryTemplate in categoryTemplates)
            {
                ListItem item2 = new ListItem(categoryTemplate.Name, categoryTemplate.CategoryTemplateId.ToString());
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
        
        public Category SaveInfo()
        {
            var category = IoCFactory.Resolve<ICategoryManager>().GetCategoryById(this.CategoryId);

            if (category != null)
            {
                Picture categoryPicture = category.Picture;
                HttpPostedFile categoryPictureFile = fuCategoryPicture.PostedFile;
                if ((categoryPictureFile != null) && (!String.IsNullOrEmpty(categoryPictureFile.FileName)))
                {
                    byte[] categoryPictureBinary = IoCFactory.Resolve<IPictureManager>().GetPictureBits(categoryPictureFile.InputStream, categoryPictureFile.ContentLength);
                    if (categoryPicture != null)
                        categoryPicture = IoCFactory.Resolve<IPictureManager>().UpdatePicture(categoryPicture.PictureId, categoryPictureBinary, categoryPictureFile.ContentType, true);
                    else
                        categoryPicture = IoCFactory.Resolve<IPictureManager>().InsertPicture(categoryPictureBinary, categoryPictureFile.ContentType, true);
                }
                int categoryPictureId = 0;
                if (categoryPicture != null)
                    categoryPictureId = categoryPicture.PictureId;

                category.Name = txtName.Text.Trim();
                category.Description = txtDescription.Value;
                category.TemplateId = int.Parse(this.ddlTemplate.SelectedItem.Value);
                category.ParentCategoryId = ParentCategory.SelectedCategoryId;
                category.PictureId = categoryPictureId;
                category.PriceRanges = txtPriceRanges.Text;
                category. ShowOnHomePage= cbShowOnHomePage.Checked;
                category.Published = cbPublished.Checked;
                category.DisplayOrder = txtDisplayOrder.Value;
                category.UpdatedOn = DateTime.UtcNow;

                IoCFactory.Resolve<ICategoryManager>().UpdateCategory(category);
            }
            else
            {
                Picture categoryPicture = null;
                HttpPostedFile categoryPictureFile = fuCategoryPicture.PostedFile;
                if ((categoryPictureFile != null) && (!String.IsNullOrEmpty(categoryPictureFile.FileName)))
                {
                    byte[] categoryPictureBinary = IoCFactory.Resolve<IPictureManager>().GetPictureBits(categoryPictureFile.InputStream, categoryPictureFile.ContentLength);
                    categoryPicture = IoCFactory.Resolve<IPictureManager>().InsertPicture(categoryPictureBinary, categoryPictureFile.ContentType, true);
                }
                int categoryPictureId = 0;
                if (categoryPicture != null)
                    categoryPictureId = categoryPicture.PictureId;

                DateTime nowDT = DateTime.UtcNow;

                category = new Category()
                {
                    Name = txtName.Text.Trim(),
                    Description = txtDescription.Value,
                    TemplateId = int.Parse(this.ddlTemplate.SelectedItem.Value),
                    ParentCategoryId = ParentCategory.SelectedCategoryId,
                    PictureId = categoryPictureId,
                    PageSize = 10,
                    PriceRanges = txtPriceRanges.Text,
                    ShowOnHomePage = cbShowOnHomePage.Checked,
                    Published = cbPublished.Checked,
                    DisplayOrder = txtDisplayOrder.Value,
                    CreatedOn = nowDT,
                    UpdatedOn = nowDT
                };

                IoCFactory.Resolve<ICategoryManager>().InsertCategory(category);
            }

            SaveLocalizableContent(category);

            return category;
        }

        protected void SaveLocalizableContent(Category category)
        {
            if (category == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtLocalizedCategoryName = (TextBox)item.FindControl("txtLocalizedCategoryName");
                    var txtLocalizedDescription = (FCKeditor)item.FindControl("txtLocalizedDescription");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtLocalizedCategoryName.Text;
                    string description = txtLocalizedDescription.Value;

                    bool allFieldsAreEmpty = (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description));

                    var content = IoCFactory.Resolve<ICategoryManager>().GetCategoryLocalizedByCategoryIdAndLanguageId(category.CategoryId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = new CategoryLocalized()
                            {
                                CategoryId = category.CategoryId,
                                LanguageId = languageId,
                                Name = name,
                                Description = description
                            };

                            IoCFactory.Resolve<ICategoryManager>().InsertCategoryLocalized(content);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content.LanguageId = languageId;
                            content.Name = name;
                            content.Description = description;

                            IoCFactory.Resolve<ICategoryManager>().UpdateCategoryLocalized(content);
                        }
                    }
                }
            }
        }

        protected void rptrLanguageDivs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var txtLocalizedCategoryName = (TextBox)e.Item.FindControl("txtLocalizedCategoryName");
                var txtLocalizedDescription = (FCKeditor)e.Item.FindControl("txtLocalizedDescription");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                var content = IoCFactory.Resolve<ICategoryManager>().GetCategoryLocalizedByCategoryIdAndLanguageId(this.CategoryId, languageId);

                if (content != null)
                {
                    txtLocalizedCategoryName.Text = content.Name;
                    txtLocalizedDescription.Value = content.Description;
                }

            }
        }

        protected void btnRemoveCategoryImage_Click(object sender, EventArgs e)
        {
            try
            {
                var category = IoCFactory.Resolve<ICategoryManager>().GetCategoryById(this.CategoryId);
                if (category != null)
                {
                    IoCFactory.Resolve<IPictureManager>().DeletePicture(category.PictureId);

                    category.PictureId = 0;
                    IoCFactory.Resolve<ICategoryManager>().UpdateCategory(category);
                    BindData();
                }
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int ParentCategoryId
        {
            get
            {
                return CommonHelper.QueryStringInt("ParentCategoryId");
            }
        }

        public int CategoryId
        {
            get
            {
                return CommonHelper.QueryStringInt("CategoryId");
            }
        }
    }
}