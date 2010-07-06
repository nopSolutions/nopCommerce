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
    public partial class CategoryTemplateInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            CategoryTemplate categoryTemplate = TemplateManager.GetCategoryTemplateById(this.CategoryTemplateId);
            if (categoryTemplate != null)
            {
                this.txtName.Text = categoryTemplate.Name;
                this.txtTemplatePath.Text = categoryTemplate.TemplatePath;
                this.txtDisplayOrder.Value = categoryTemplate.DisplayOrder;
                this.pnlCreatedOn.Visible = true;
                this.pnlUpdatedOn.Visible = true;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(categoryTemplate.CreatedOn, DateTimeKind.Utc).ToString();
                this.lblUpdatedOn.Text = DateTimeHelper.ConvertToUserTime(categoryTemplate.UpdatedOn, DateTimeKind.Utc).ToString();
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

        public CategoryTemplate SaveInfo()
        {
            CategoryTemplate categoryTemplate = TemplateManager.GetCategoryTemplateById(this.CategoryTemplateId);

            if (categoryTemplate != null)
            {
                categoryTemplate = TemplateManager.UpdateCategoryTemplate(categoryTemplate.CategoryTemplateId, txtName.Text,
                    txtTemplatePath.Text, txtDisplayOrder.Value, categoryTemplate.CreatedOn, DateTime.UtcNow);                
            }
            else
            {
                DateTime now = DateTime.UtcNow;
                categoryTemplate = TemplateManager.InsertCategoryTemplate(txtName.Text,
                    txtTemplatePath.Text, txtDisplayOrder.Value, now, now);
            }

            return categoryTemplate;
        }

        public int CategoryTemplateId
        {
            get
            {
                return CommonHelper.QueryStringInt("CategoryTemplateId");
            }
        }
    }
}