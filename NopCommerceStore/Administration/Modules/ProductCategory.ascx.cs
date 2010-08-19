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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductCategoryControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            List<ProductCategoryMappingHelperClass> productCategoryMappings = null;

            Product product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                var existingProductCategoryCollection = product.ProductCategories;
                productCategoryMappings = GetProductCategoryMappings(existingProductCategoryCollection);
            }
            else
            {
                productCategoryMappings = GetProductCategoryMappings(null);
            }
            
            gvCategoryMappings.DataSource = productCategoryMappings;
            gvCategoryMappings.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
            else
            {
                UpdateState();
            }
        }

        public void SaveInfo()
        {
            SaveInfo(this.ProductId);
        }

        public void SaveInfo(int prodId)
        {
            Product product = ProductManager.GetProductById(prodId);
            if (product != null)
            {
                foreach (var item in this.GridState.Values)
                {
                    if (item.ProductCategoryId > 0 && !item.IsMapped)
                        CategoryManager.DeleteProductCategory(item.ProductCategoryId);
                    if (item.ProductCategoryId > 0 && item.IsMapped)
                        CategoryManager.UpdateProductCategory(item.ProductCategoryId, product.ProductId, item.CategoryId, item.IsFeatured, item.DisplayOrder);
                    if (item.ProductCategoryId == 0 && item.IsMapped)
                        CategoryManager.InsertProductCategory(product.ProductId, item.CategoryId, item.IsFeatured, item.DisplayOrder);
                }
            }
        }

        protected void gvCategoryMappings_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvCategoryMappings.PageIndex = e.NewPageIndex;
            this.BindData();
        }
       
        private List<ProductCategoryMappingHelperClass> GetProductCategoryMappings(List<ProductCategory> existingProductCategoryCollection)
        {
            var categories = CategoryManager.GetAllCategories();
            List<ProductCategoryMappingHelperClass> result = new List<ProductCategoryMappingHelperClass>();
            for (int i = 0; i < categories.Count; i++)
            {
                Category category = categories[i];
                ProductCategory existingProductCategory = null;
                if (existingProductCategoryCollection != null)
                    existingProductCategory = existingProductCategoryCollection.FindProductCategory(this.ProductId, category.CategoryId);
                ProductCategoryMappingHelperClass pcm = new ProductCategoryMappingHelperClass();
                if (existingProductCategory != null)
                {
                    pcm.ProductCategoryId = existingProductCategory.ProductCategoryId;
                    pcm.IsMapped = true;
                    pcm.IsFeatured = existingProductCategory.IsFeaturedProduct;
                    pcm.DisplayOrder = existingProductCategory.DisplayOrder;
                }
                else
                {
                    pcm.DisplayOrder = 1;
                }
                pcm.CategoryId = category.CategoryId;
                pcm.CategoryInfo = GetCategoryFullName(category);

                MapState(category.CategoryId, pcm);

                result.Add(pcm);
            }

            return result;
        }

        protected string GetCategoryFullName(Category category)
        {
            string result = string.Empty;

            while (category != null && !category.Deleted)
            {
                if (String.IsNullOrEmpty(result))
                {
                    result = category.Name;
                }
                else
                {
                    result = category.Name + " >> " + result;
                }
                category = category.ParentCategory;
            }
            return result;
        }

        private Dictionary<int, ProductCategoryMappingHelperClass> _gridState;
        private Dictionary<int, ProductCategoryMappingHelperClass> GridState
        {
            get
            {
                if (_gridState == null)
                {
                    _gridState = ViewState["ProductCategoryState"] as Dictionary<int, ProductCategoryMappingHelperClass>;
                    if (_gridState == null)
                    {
                        _gridState = new Dictionary<int, ProductCategoryMappingHelperClass>();
                        ViewState["ProductCategoryState"] = _gridState;
                    }
                }
                return _gridState;
            }

            set
            {
                _gridState = value;
                ViewState["ProductCategoryState"] = value;
            }
        }

        private void UpdateState()
        {
            Dictionary<int, ProductCategoryMappingHelperClass> state = this.GridState;
            foreach (GridViewRow row in gvCategoryMappings.Rows)
            {
                CheckBox cbCategoryInfo = row.FindControl("cbCategoryInfo") as CheckBox;
                HiddenField hfCategoryId = row.FindControl("hfCategoryId") as HiddenField;
                HiddenField hfProductCategoryId = row.FindControl("hfProductCategoryId") as HiddenField;
                CheckBox cbFeatured = row.FindControl("cbFeatured") as CheckBox;
                NumericTextBox txtRowDisplayOrder = row.FindControl("txtDisplayOrder") as NumericTextBox;
                int productCategoryId = int.Parse(hfProductCategoryId.Value);
                int categoryId = int.Parse(hfCategoryId.Value);
                int displayOrder = txtRowDisplayOrder.Value;

                if (cbCategoryInfo.Checked || (productCategoryId > 0))
                {
                    state[categoryId] = new ProductCategoryMappingHelperClass()
                    {
                        CategoryId = categoryId,
                        ProductCategoryId = productCategoryId,
                        IsMapped = cbCategoryInfo.Checked,
                        DisplayOrder = displayOrder,
                        IsFeatured = cbFeatured.Checked
                    };
                }
                else if (state.ContainsKey(categoryId))
                {
                    state.Remove(categoryId);
                }
            }
            this.GridState = state;
        }

        private void MapState(int Id, ProductCategoryMappingHelperClass rp)
        {
            if (this.GridState.ContainsKey(Id))
            {
                ProductCategoryMappingHelperClass srp = this.GridState[Id];
                rp.IsMapped = srp.IsMapped;
                rp.DisplayOrder = srp.DisplayOrder;
                rp.IsFeatured = srp.IsFeatured;
            }
        }

        public int ProductId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductId");
            }
        }

        [Serializable]
        private class ProductCategoryMappingHelperClass
        {
            public int ProductCategoryId { get; set; }
            public int CategoryId { get; set; }
            public string CategoryInfo { get; set; }
            public bool IsMapped { get; set; }
            public bool IsFeatured { get; set; }
            public int DisplayOrder { get; set; }
        }
    }
}