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
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductTemplateInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            ProductTemplate productTemplate = TemplateManager.GetProductTemplateById(this.ProductTemplateId);
            if (productTemplate != null)
            {
                this.txtName.Text = productTemplate.Name;
                this.txtTemplatePath.Text = productTemplate.TemplatePath;
                this.txtDisplayOrder.Value = productTemplate.DisplayOrder;
                this.pnlCreatedOn.Visible = true;
                this.pnlUpdatedOn.Visible = true;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(productTemplate.CreatedOn, DateTimeKind.Utc).ToString();
                this.lblUpdatedOn.Text = DateTimeHelper.ConvertToUserTime(productTemplate.UpdatedOn, DateTimeKind.Utc).ToString();
            }
            else
            {
                this.pnlCreatedOn.Visible = false;
                this.pnlUpdatedOn.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        public ProductTemplate SaveInfo()
        {
            ProductTemplate productTemplate = TemplateManager.GetProductTemplateById(this.ProductTemplateId);

            if (productTemplate != null)
            {
                productTemplate = TemplateManager.UpdateProductTemplate(productTemplate.ProductTemplateId, txtName.Text,
                    txtTemplatePath.Text, txtDisplayOrder.Value, productTemplate.CreatedOn, DateTime.UtcNow);
            }
            else
            {
                DateTime now = DateTime.UtcNow;
                productTemplate = TemplateManager.InsertProductTemplate(txtName.Text,
                    txtTemplatePath.Text, txtDisplayOrder.Value, now, now);

            }

            return productTemplate;
        }


        public int ProductTemplateId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductTemplateId");
            }
        }
    }
}