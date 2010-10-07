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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
 

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CategoryACLControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            List<int> _customerRoleIds = new List<int>();

            var category = CategoryManager.GetCategoryById(this.CategoryId);
            if (category != null)
            {
                var aclRules = ACLManager.GetAllAclPerObject(this.CategoryId, (int)ObjectTypeEnum.Category, 0, true);
                foreach (var aclPerObject in aclRules)
                    _customerRoleIds.Add(aclPerObject.CustomerRoleId);
            }

            ctrlRoles.SelectedCustomerRoleIds = _customerRoleIds;
            ctrlRoles.BindData();
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
            SaveInfo(this.CategoryId);
        }

        public void SaveInfo(int catId)
        {
            Category category = CategoryManager.GetCategoryById(catId);

            if (category != null)
            {
                List<int> selectedCustomerRoleIds = this.ctrlRoles.SelectedCustomerRoleIds;
                var existingAclRules = ACLManager.GetAllAclPerObject(catId, (int)ObjectTypeEnum.Category, 0, true);

                var allCustomerRoles = CustomerManager.GetAllCustomerRoles();
                foreach (var cr in allCustomerRoles)
                {
                    if (selectedCustomerRoleIds.Contains(cr.CustomerRoleId))
                    {
                        if (existingAclRules.Find(a => a.CustomerRoleId == cr.CustomerRoleId) == null)
                        {
                            ACLManager.InsertAclPerObject(category.CategoryId, ObjectTypeEnum.Category, cr.CustomerRoleId, true);
                        }
                    }
                    else
                    {
                        var aclToDelete = existingAclRules.Find(a => a.CustomerRoleId == cr.CustomerRoleId);
                        if (aclToDelete != null)
                        {
                            ACLManager.DeleteAclPerObject(aclToDelete.ACLPerObjectId);
                        }
                    }
                }
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