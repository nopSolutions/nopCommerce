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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CheckoutAttributeDetailsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.SelectTab(this.AttributeTabs, this.TabId);
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    CheckoutAttribute attribute = null;
                    attribute = ctrlCheckoutAttributeInfo.SaveInfo();
                    ctrlCheckoutAttributeValues.SaveInfo();

                    CustomerActivityManager.InsertActivity(
                        "EditCheckoutAttribute",
                        GetLocaleResourceString("ActivityLog.EditCheckoutAttribute"),
                        attribute.Name);

                    if (attribute != null)
                        Response.Redirect(string.Format("CheckoutAttributeDetails.aspx?CheckoutAttributeID={0}&TabID={1}", attribute.CheckoutAttributeId, this.GetActiveTabId(this.AttributeTabs)));
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                var attribute = CheckoutAttributeManager.GetCheckoutAttributeById(this.CheckoutAttributeId);
                if (attribute != null)
                {
                    CheckoutAttributeManager.DeleteCheckoutAttribute(this.CheckoutAttributeId);

                    CustomerActivityManager.InsertActivity(
                        "DeleteCheckoutAttribute",
                        GetLocaleResourceString("ActivityLog.DeleteCheckoutAttribute"),
                        attribute.Name);

                }
                Response.Redirect("checkoutattributes.aspx");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int CheckoutAttributeId
        {
            get
            {
                return CommonHelper.QueryStringInt("CheckoutAttributeId");
            }
        }

        protected string TabId
        {
            get
            {
                return CommonHelper.QueryString("TabId");
            }
        }
    }
}