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
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductManufacturerControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            List<ProductManufacturerMappingHelperClass> productManufacturerMappings = null;

            Product product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                var existingProductManufacturerCollection = product.ProductManufacturers;
                productManufacturerMappings = GetProductManufacturerMappings(existingProductManufacturerCollection);
            }
            else
            {
                productManufacturerMappings = GetProductManufacturerMappings(null);
            }

            gvManufacturerMappings.DataSource = productManufacturerMappings;
            gvManufacturerMappings.DataBind();
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
                    if (item.ProductManufacturerId > 0 && !item.IsMapped)
                        ManufacturerManager.DeleteProductManufacturer(item.ProductManufacturerId);
                    if (item.ProductManufacturerId > 0 && item.IsMapped)
                        ManufacturerManager.UpdateProductManufacturer(item.ProductManufacturerId, product.ProductId, item.ManufacturerId, item.IsFeatured, item.DisplayOrder);
                    if (item.ProductManufacturerId == 0 && item.IsMapped)
                        ManufacturerManager.InsertProductManufacturer(product.ProductId, item.ManufacturerId, item.IsFeatured, item.DisplayOrder);
                }
            }
        }

        protected void gvManufacturerMappings_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvManufacturerMappings.PageIndex = e.NewPageIndex;
            this.BindData();
        }

        private List<ProductManufacturerMappingHelperClass> GetProductManufacturerMappings(List<ProductManufacturer> ExistingProductManufacturerCollection)
        {
            var manufacturerCollection = ManufacturerManager.GetAllManufacturers();
            List<ProductManufacturerMappingHelperClass> result = new List<ProductManufacturerMappingHelperClass>();
            for (int i = 0; i < manufacturerCollection.Count; i++)
            {
                Manufacturer manufacturer = manufacturerCollection[i];
                ProductManufacturer existingProductManufacturer = null;
                if (ExistingProductManufacturerCollection != null)
                    existingProductManufacturer = ExistingProductManufacturerCollection.FindProductManufacturer(this.ProductId, manufacturer.ManufacturerId);
                ProductManufacturerMappingHelperClass pmm = new ProductManufacturerMappingHelperClass();
                if (existingProductManufacturer != null)
                {
                    pmm.ProductManufacturerId = existingProductManufacturer.ProductManufacturerId;
                    pmm.IsMapped = true;
                    pmm.IsFeatured = existingProductManufacturer.IsFeaturedProduct;
                    pmm.DisplayOrder = existingProductManufacturer.DisplayOrder;
                }
                else
                {
                    pmm.DisplayOrder = 1;
                }
                pmm.ManufacturerId = manufacturer.ManufacturerId;
                pmm.ManufacturerInfo = manufacturer.Name;

                MapState(manufacturer.ManufacturerId, pmm);

                result.Add(pmm);
            }

            return result;
        }

        private Dictionary<int, ProductManufacturerMappingHelperClass> _gridState;
        private Dictionary<int, ProductManufacturerMappingHelperClass> GridState
        {
            get
            {
                if (_gridState == null)
                {
                    _gridState = ViewState["ProductManufacturerState"] as Dictionary<int, ProductManufacturerMappingHelperClass>;
                    if (_gridState == null)
                    {
                        _gridState = new Dictionary<int, ProductManufacturerMappingHelperClass>();
                        ViewState["ProductManufacturerState"] = _gridState;
                    }
                }
                return _gridState;
            }

            set
            {
                _gridState = value;
                ViewState["ProductManufacturerState"] = value;
            }
        }

        private void UpdateState()
        {
            Dictionary<int, ProductManufacturerMappingHelperClass> state = this.GridState;
            foreach (GridViewRow row in gvManufacturerMappings.Rows)
            {
                CheckBox cbManufacturerInfo = row.FindControl("cbManufacturerInfo") as CheckBox;
                HiddenField hfManufacturerId = row.FindControl("hfManufacturerId") as HiddenField;
                HiddenField hfProductManufacturerId = row.FindControl("hfProductManufacturerId") as HiddenField;
                CheckBox cbFeatured = row.FindControl("cbFeatured") as CheckBox;
                NumericTextBox txtRowDisplayOrder = row.FindControl("txtDisplayOrder") as NumericTextBox;
                int productManufacturerId = int.Parse(hfProductManufacturerId.Value);
                int manufacturerId = int.Parse(hfManufacturerId.Value);
                int displayOrder = txtRowDisplayOrder.Value;

                if (cbManufacturerInfo.Checked || (productManufacturerId > 0))
                {
                    state[manufacturerId] = new ProductManufacturerMappingHelperClass()
                    {
                        ManufacturerId = manufacturerId,
                        ProductManufacturerId = productManufacturerId,
                        IsMapped = cbManufacturerInfo.Checked,
                        DisplayOrder = displayOrder,
                        IsFeatured = cbFeatured.Checked
                    };
                }
                else if (state.ContainsKey(manufacturerId))
                {
                    state.Remove(manufacturerId);
                }
            }
            this.GridState = state;
        }

        private void MapState(int Id, ProductManufacturerMappingHelperClass rp)
        {
            if (this.GridState.ContainsKey(Id))
            {
                ProductManufacturerMappingHelperClass srp = this.GridState[Id];
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
        private class ProductManufacturerMappingHelperClass
        {
            public int ProductManufacturerId { get; set; }
            public int ManufacturerId { get; set; }
            public string ManufacturerInfo { get; set; }
            public bool IsMapped { get; set; }
            public bool IsFeatured { get; set; }
            public int DisplayOrder { get; set; }
        }
    }
}