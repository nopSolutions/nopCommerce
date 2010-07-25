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
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Polls;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Security;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ACLControl : BaseNopAdministrationUserControl
    {
        public class NopGridViewCustomTemplate : ITemplate
        {
            #region Fields
            private DataControlRowType templateType;
            private string columnName;
            private string dataType;
            private int customerRoleId;
            #endregion

            #region Ctor
            public NopGridViewCustomTemplate(DataControlRowType type,
                string columnName, string DataType)
                : this(type, columnName, DataType, 0)
            {
            }

            public NopGridViewCustomTemplate(DataControlRowType type,
                string columnName, string DataType, int customerRoleId)
            {
                this.templateType = type;
                this.columnName = columnName;
                this.dataType = DataType;
                this.customerRoleId = customerRoleId;
            }
            #endregion

            #region Utilities
            private void ctrl_DataBinding(Object sender, EventArgs e)
            {
                WebControl ctrl = (WebControl)sender;
                GridViewRow row = (GridViewRow)ctrl.NamingContainer;
                switch (dataType)
                {
                    case "String":
                        {
                            object RawValue = DataBinder.Eval(row.DataItem, columnName);
                            ((Label)ctrl).Text = RawValue.ToString();
                        }
                        break;
                    case "Checkbox":
                        {
                            CustomerActionACLMappingHelperClass map1 = (CustomerActionACLMappingHelperClass)row.DataItem;
                            ((CheckBox)ctrl).Checked = map1.Allow[this.customerRoleId];
                        }
                        break;
                }
            }

            private void hfCustomerActionId_DataBinding(Object sender, EventArgs e)
            {
                HiddenField hf = (HiddenField)sender;
                GridViewRow row = (GridViewRow)hf.NamingContainer;

                CustomerActionACLMappingHelperClass map1 = (CustomerActionACLMappingHelperClass)row.DataItem;
                hf.Value = map1.CustomerActionId.ToString();
            }
            #endregion

            #region Methods
            public void InstantiateIn(Control container)
            {
                switch (templateType)
                {
                    case DataControlRowType.Header:
                        Literal lc = new Literal();
                        lc.Text = "<b>" + columnName + "</b>";
                        container.Controls.Add(lc);
                        break;
                    case DataControlRowType.DataRow:
                        WebControl ctrl = null;
                        switch (dataType)
                        {
                            case "String":
                                ctrl = new Label();
                                break;
                            case "Checkbox":
                                ctrl = new CheckBox();
                                ctrl.ID = string.Format("cbAllow_{0}", customerRoleId);

                                HiddenField hfCustomerActionId = new HiddenField();
                                hfCustomerActionId.ID = string.Format("hfCustomerActionId_{0}", customerRoleId);
                                hfCustomerActionId.DataBinding += new EventHandler(this.hfCustomerActionId_DataBinding);
                                container.Controls.Add(hfCustomerActionId);
                                break;
                            default:
                                throw new Exception("Not supported column type");
                        }
                        ctrl.DataBinding += new EventHandler(this.ctrl_DataBinding);
                        container.Controls.Add(ctrl);
                        break;
                    default:
                        break;
                }
            }
            #endregion
        }

        protected class CustomerActionACLMappingHelperClass
        {
            public int CustomerActionId { get; set; }
            public string CustomerActionName { get; set; }
            public Dictionary<int, bool> Allow { get; set; }
        }

        protected void BuildColumnsDynamically()
        {
            gvACL.Columns.Clear();

            TemplateField tfAction = new TemplateField();
            tfAction.ItemTemplate = new NopGridViewCustomTemplate(DataControlRowType.DataRow, "CustomerActionName", "String");
            tfAction.HeaderTemplate = new NopGridViewCustomTemplate(DataControlRowType.Header, GetLocaleResourceString("Admin.ACL.Grid.CustomerAction"), "String");
            gvACL.Columns.Add(tfAction);

            var roles = CustomerManager.GetAllCustomerRoles();
            foreach (CustomerRole cr in roles)
            {
                TemplateField tf = new TemplateField();
                tf.ItemTemplate = new NopGridViewCustomTemplate(DataControlRowType.DataRow, "Allow", "Checkbox", cr.CustomerRoleId);
                tf.HeaderTemplate = new NopGridViewCustomTemplate(DataControlRowType.Header, cr.Name, "String");
                gvACL.Columns.Add(tf);
            }
        }

        protected void BindGrid()
        {
            var actions = ACLManager.GetAllCustomerActions();
            if (actions.Count == 0)
            {
                lblMessage.Text = GetLocaleResourceString("Admin.ACL.NoActionDefined");
                return;
            }
            var roles = CustomerManager.GetAllCustomerRoles();
            if (roles.Count == 0)
            {
                lblMessage.Text = GetLocaleResourceString("Admin.ACL.NoRolesDefined");
                return;
            }
            List<CustomerActionACLMappingHelperClass> dt = new List<CustomerActionACLMappingHelperClass>();
            foreach (CustomerAction ca in actions)
            {
                CustomerActionACLMappingHelperClass map1 = new CustomerActionACLMappingHelperClass();
                map1.CustomerActionId = ca.CustomerActionId;
                map1.CustomerActionName = ca.Name;
                map1.Allow = new Dictionary<int, bool>();

                foreach (CustomerRole cr in roles)
                {
                    var acls = ACLManager.GetAllAcl(ca.CustomerActionId, cr.CustomerRoleId, null);
                    if (acls.Count > 0)
                    {
                        ACL acl = acls[0];
                        map1.Allow.Add(cr.CustomerRoleId, acl.Allow);
                    }
                    else
                    {
                        map1.Allow.Add(cr.CustomerRoleId, false);
                    }
                }
                dt.Add(map1);
            }
            
            gvACL.DataSource = dt;
            gvACL.DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            BuildColumnsDynamically();
            base.OnInit(e);
        }
                
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGrid();
                BindData();
            }
        }

        private void BindData()
        {
            cbACLEnabled.Checked = SettingManager.GetSettingValueBoolean("ACL.Enabled");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var roles = CustomerManager.GetAllCustomerRoles();
                    if (roles.Count == 0)
                    {
                        lblMessage.Text = GetLocaleResourceString("Admin.ACL.NoRolesDefined");
                        return;
                    }
                    SettingManager.SetParam("ACL.Enabled", cbACLEnabled.Checked.ToString());

                    foreach (GridViewRow row in gvACL.Rows)
                    {
                        foreach (CustomerRole cr in roles)
                        {
                            CheckBox cbAllow = row.FindControl(string.Format("cbAllow_{0}", cr.CustomerRoleId)) as CheckBox;
                            HiddenField hfCustomerActionId = row.FindControl(string.Format("hfCustomerActionId_{0}", cr.CustomerRoleId)) as HiddenField;
                            if (cbAllow == null || hfCustomerActionId == null || String.IsNullOrEmpty(hfCustomerActionId.Value))
                                return;

                            bool allow = cbAllow.Checked;
                            int customerActionId = int.Parse(hfCustomerActionId.Value);

                            var acls = ACLManager.GetAllAcl(customerActionId, cr.CustomerRoleId, null);
                            if (acls.Count > 0)
                            {
                                ACL acl = acls[0];
                                ACLManager.UpdateAcl(acl.AclId, customerActionId, cr.CustomerRoleId, allow);
                            }
                            else
                            {
                                ACL acl = ACLManager.InsertAcl(customerActionId, cr.CustomerRoleId, allow);
                            }
                        }
                    } 

                    Response.Redirect("ACL.aspx");
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }
    }
}