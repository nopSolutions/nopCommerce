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
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CheckoutAttributeAddControl : BaseNopAdministrationUserControl
    {
        protected CheckoutAttribute Save()
        {
            CheckoutAttribute attribute = ctrlCheckoutAttributeInfo.SaveInfo();
            ctrlCheckoutAttributeValues.SaveInfo();

            IoCFactory.Resolve<ICustomerActivityService>().InsertActivity(
                "AddNewCheckoutAttribute",
                GetLocaleResourceString("ActivityLog.AddNewCheckoutAttribute"),
                attribute.Name);

            return attribute;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    CheckoutAttribute attribute = Save();
                    Response.Redirect("CheckoutAttributes.aspx");
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void SaveAndStayButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    CheckoutAttribute attribute = Save();
                    Response.Redirect("CheckoutAttributeDetails.aspx?CheckoutAttributeID=" + attribute.CheckoutAttributeId);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }
    }
}