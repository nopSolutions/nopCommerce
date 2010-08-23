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
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Warehouses;
using NopSolutions.NopCommerce.Web.Administration.Modules;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductVariantDiscountsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            List<int> _discountIds = new List<int>();

            ProductVariant productVariant = ProductManager.GetProductVariantById(this.ProductVariantId);
            if (productVariant != null)
            {
                var discounts = productVariant.AllDiscounts;

                foreach (Discount dis in discounts)
                    _discountIds.Add(dis.DiscountId);
            }

            DiscountMappingControl.SelectedDiscountIds = _discountIds;
            DiscountMappingControl.BindData(DiscountTypeEnum.AssignedToSKUs);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        public void SaveInfo()
        {
            SaveInfo(this.ProductVariantId);
        }

        public void SaveInfo(int pvId)
        {
            ProductVariant productVariant = ProductManager.GetProductVariantById(pvId);
            if (productVariant != null)
            {
                List<int> selectedDiscountIds = this.DiscountMappingControl.SelectedDiscountIds;
                var existingDiscounts = DiscountManager.GetDiscountsByProductVariantId(productVariant.ProductVariantId);

                var allDiscounts = DiscountManager.GetAllDiscounts(DiscountTypeEnum.AssignedToSKUs);
                foreach (Discount discount in allDiscounts)
                {
                    if (selectedDiscountIds.Contains(discount.DiscountId))
                    {
                        if (existingDiscounts.Find(d => d.DiscountId == discount.DiscountId) == null)
                        {
                            DiscountManager.AddDiscountToProductVariant(productVariant.ProductVariantId, discount.DiscountId);
                        }
                    }
                    else
                    {
                        if (existingDiscounts.Find(d => d.DiscountId == discount.DiscountId) != null)
                        {
                            DiscountManager.RemoveDiscountFromProductVariant(productVariant.ProductVariantId, discount.DiscountId);
                        }
                    }
                }
            }
        }

        public int ProductVariantId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductVariantId");
            }
        }
    }
}