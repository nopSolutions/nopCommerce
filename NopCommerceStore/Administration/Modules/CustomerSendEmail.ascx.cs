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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;
using System.Net.Mail;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerSendEmailControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var customer = IoCFactory.Resolve<ICustomerService>().GetCustomerById(this.CustomerId);

                    if (customer != null)
                    {
                        var emailAccount = IoCFactory.Resolve<IMessageService>().DefaultEmailAccount;

                        var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
                        var to = new MailAddress(customer.Email, customer.FullName);
                        var subject = txtSubject.Text;
                        var body = txtBody.Value;

                        var email = IoCFactory.Resolve<IMessageService>().InsertQueuedEmail(5, from, to, string.Empty,
                            string.Empty, subject, body, DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
                    }
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        public int CustomerId
        {
            get
            {
                return CommonHelper.QueryStringInt("CustomerId");
            }
        }
    }
}