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
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ReturnRequestDetailsControl : BaseNopAdministrationUserControl
    {
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    ReturnRequest rr = ctrlReturnRequestInfo.SaveInfo();

                    CustomerActivityManager.InsertActivity(
                        "EditReturnRequest",
                        GetLocaleResourceString("ActivityLog.EditReturnRequest"),
                        rr.ReturnRequestId);

                    Response.Redirect("ReturnRequestDetails.aspx?ReturnRequestID=" + rr.ReturnRequestId.ToString());
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
                ReturnRequest rr = OrderManager.GetReturnRequestById(this.ReturnRequestId);
                if (rr != null)
                {
                    OrderManager.DeleteReturnRequest(rr.ReturnRequestId);

                    CustomerActivityManager.InsertActivity(
                        "DeleteReturnRequest",
                        GetLocaleResourceString("ActivityLog.DeleteReturnRequest"),
                        rr.ReturnRequestId);
                }

                Response.Redirect("ReturnRequests.aspx");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }


        public int ReturnRequestId
        {
            get
            {
                return CommonHelper.QueryStringInt("ReturnRequestId");
            }
        }
    }
}