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
    public partial class ManufacturerTemplateInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            ManufacturerTemplate manufacturerTemplate = TemplateManager.GetManufacturerTemplateById(this.ManufacturerTemplateId);
            if (manufacturerTemplate != null)
            {
                this.txtName.Text = manufacturerTemplate.Name;
                this.txtTemplatePath.Text = manufacturerTemplate.TemplatePath;
                this.txtDisplayOrder.Value = manufacturerTemplate.DisplayOrder;
                this.pnlCreatedOn.Visible = true;
                this.pnlUpdatedOn.Visible = true;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(manufacturerTemplate.CreatedOn, DateTimeKind.Utc).ToString();
                this.lblUpdatedOn.Text = DateTimeHelper.ConvertToUserTime(manufacturerTemplate.UpdatedOn, DateTimeKind.Utc).ToString();
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

        public ManufacturerTemplate SaveInfo()
        {
            ManufacturerTemplate manufacturerTemplate = TemplateManager.GetManufacturerTemplateById(this.ManufacturerTemplateId);
            if (manufacturerTemplate != null)
            {
                manufacturerTemplate = TemplateManager.UpdateManufacturerTemplate(manufacturerTemplate.ManufacturerTemplateId, txtName.Text,
                    txtTemplatePath.Text, txtDisplayOrder.Value, manufacturerTemplate.CreatedOn, DateTime.UtcNow);
            }
            else
            {
                DateTime now = DateTime.UtcNow;
                manufacturerTemplate = TemplateManager.InsertManufacturerTemplate(txtName.Text,
                    txtTemplatePath.Text, txtDisplayOrder.Value, now, now);
            }

            return manufacturerTemplate;
        }

        public int ManufacturerTemplateId
        {
            get
            {
                return CommonHelper.QueryStringInt("ManufacturerTemplateId");
            }
        }
    }
}