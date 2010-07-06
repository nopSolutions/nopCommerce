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
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
 

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CategoryInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Category category = CategoryManager.GetCategoryById(this.CategoryId);

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
                this.txtDescription.Content = category.Description;
                CommonHelper.SelectListItem(this.ddlTemplate, category.TemplateId);
                ParentCategory.SelectedCategoryId = category.ParentCategoryId;

                Picture categoryPicture = category.Picture;
                this.btnRemoveCategoryImage.Visible = categoryPicture != null;
                string pictureUrl = PictureManager.GetPictureUrl(categoryPicture, 100);
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
            var categoryTemplates = TemplateManager.GetAllCategoryTemplates();
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
            Category category = CategoryManager.GetCategoryById(this.CategoryId);

            if (category != null)
            {
                Picture categoryPicture = category.Picture;
                HttpPostedFile categoryPictureFile = fuCategoryPicture.PostedFile;
                if ((categoryPictureFile != null) && (!String.IsNullOrEmpty(categoryPictureFile.FileName)))
                {
                    byte[] categoryPictureBinary = PictureManager.GetPictureBits(categoryPictureFile.InputStream, categoryPictureFile.ContentLength);
                    if (categoryPicture != null)
                        categoryPicture = PictureManager.UpdatePicture(categoryPicture.PictureId, categoryPictureBinary, categoryPictureFile.ContentType, true);
                    else
                        categoryPicture = PictureManager.InsertPicture(categoryPictureBinary, categoryPictureFile.ContentType, true);
                }
                int categoryPictureId = 0;
                if (categoryPicture != null)
                    categoryPictureId = categoryPicture.PictureId;

                category = CategoryManager.UpdateCategory(category.CategoryId, txtName.Text, txtDescription.Content, int.Parse(this.ddlTemplate.SelectedItem.Value),
                     category.MetaKeywords, category.MetaDescription, category.MetaTitle, category.SEName, ParentCategory.SelectedCategoryId,
                    categoryPictureId, category.PageSize, txtPriceRanges.Text, cbShowOnHomePage.Checked, cbPublished.Checked, category.Deleted, 
                    txtDisplayOrder.Value, category.CreatedOn, DateTime.UtcNow);
            }
            else
            {
                Picture categoryPicture = null;
                HttpPostedFile categoryPictureFile = fuCategoryPicture.PostedFile;
                if ((categoryPictureFile != null) && (!String.IsNullOrEmpty(categoryPictureFile.FileName)))
                {
                    byte[] categoryPictureBinary = PictureManager.GetPictureBits(categoryPictureFile.InputStream, categoryPictureFile.ContentLength);
                    categoryPicture = PictureManager.InsertPicture(categoryPictureBinary, categoryPictureFile.ContentType, true);
                }
                int categoryPictureId = 0;
                if (categoryPicture != null)
                    categoryPictureId = categoryPicture.PictureId;

                DateTime nowDT = DateTime.UtcNow;
                category = CategoryManager.InsertCategory(txtName.Text, txtDescription.Content, int.Parse(this.ddlTemplate.SelectedItem.Value),
                         string.Empty, string.Empty, string.Empty, string.Empty, ParentCategory.SelectedCategoryId,
                         categoryPictureId, 10, txtPriceRanges.Text, cbShowOnHomePage.Checked, cbPublished.Checked, false, txtDisplayOrder.Value, nowDT, nowDT);
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
                    var txtLocalizedDescription = (AjaxControlToolkit.HTMLEditor.Editor)item.FindControl("txtLocalizedDescription");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtLocalizedCategoryName.Text;
                    string description = txtLocalizedDescription.Content;

                    bool allFieldsAreEmpty = (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description));

                    var content = CategoryManager.GetCategoryLocalizedByCategoryIdAndLanguageId(category.CategoryId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = CategoryManager.InsertCategoryLocalized(category.CategoryId,
                                   languageId, name, description, string.Empty, string.Empty,
                                   string.Empty, string.Empty);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content = CategoryManager.UpdateCategoryLocalized(content.CategoryLocalizedId, content.CategoryId,
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
                var txtLocalizedCategoryName = (TextBox)e.Item.FindControl("txtLocalizedCategoryName");
                var txtLocalizedDescription = (AjaxControlToolkit.HTMLEditor.Editor)e.Item.FindControl("txtLocalizedDescription");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                var content = CategoryManager.GetCategoryLocalizedByCategoryIdAndLanguageId(this.CategoryId, languageId);

                if (content != null)
                {
                    txtLocalizedCategoryName.Text = content.Name;
                    txtLocalizedDescription.Content = content.Description;
                }

            }
        }

        protected void btnRemoveCategoryImage_Click(object sender, EventArgs e)
        {
            try
            {
                var category = CategoryManager.GetCategoryById(this.CategoryId);
                if (category != null)
                {
                    PictureManager.DeletePicture(category.PictureId);
                    CategoryManager.RemoveCategoryPicture(category.CategoryId);
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