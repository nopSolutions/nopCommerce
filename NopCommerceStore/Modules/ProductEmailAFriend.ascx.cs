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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ProductEmailAFriend : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SettingManager.GetSettingValueBoolean("Common.EnableEmailAFirend"))
                Response.Redirect(CommonHelper.GetStoreLocation());

            if (!Page.IsPostBack)
                BindData();
        }

        protected void BindData()
        {
            var product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                hlProduct.NavigateUrl = SEOHelper.GetProductUrl(product);
                hlProduct.Text = Server.HtmlEncode(product.LocalizedName);
                lShortDescription.Text = product.LocalizedShortDescription;

                if(NopContext.Current.User == null && CustomerManager.AllowAnonymousUsersToEmailAFriend)
                {
                    CustomerManager.CreateAnonymousUser();
                }

                if(NopContext.Current.User == null || (NopContext.Current.User.IsGuest && !CustomerManager.AllowAnonymousUsersToEmailAFriend))
                {
                    lblEmailAFriend.Text = GetLocaleResourceString("Products.OnlyRegisteredUsersCanEmailAFriend");
                    txtFriendsEmail.Enabled = false;
                    txtPersonalMessage.Enabled = false;
                    btnEmail.Enabled = false;
                }
                else
                {
                    lblEmailAFriend.Text = string.Empty;
                    txtFriendsEmail.Enabled = true;
                    lblFrom.Text = NopContext.Current.User.Email;
                    txtPersonalMessage.Enabled = true;
                    btnEmail.Enabled = true;
                }
            }
            else
                this.Visible = false;
        }

        protected void btnEmail_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var product = ProductManager.GetProductById(this.ProductId);
                    if (product != null)
                    {
                        if(NopContext.Current.User == null && CustomerManager.AllowAnonymousUsersToEmailAFriend)
                        {
                            CustomerManager.CreateAnonymousUser();
                        }

                        if(NopContext.Current.User == null || (NopContext.Current.User.IsGuest && !CustomerManager.AllowAnonymousUsersToEmailAFriend))
                        {
                            lblEmailAFriend.Text = GetLocaleResourceString("Products.OnlyRegisteredUsersCanEmailAFriend");
                            return;
                        }

                        string friendsEmail = txtFriendsEmail.Text.Trim();
                        string personalMessage = txtPersonalMessage.Text.Trim();
                        personalMessage = ProductManager.FormatEmailAFriendText(personalMessage);

                        MessageManager.SendEmailAFriendMessage(NopContext.Current.User,
                            NopContext.Current.WorkingLanguage.LanguageId, product,
                            friendsEmail, personalMessage);

                        txtFriendsEmail.Text = string.Empty;
                        txtPersonalMessage.Text = string.Empty;
                        lblEmailAFriend.Text = GetLocaleResourceString("Products.EmailAFriend.YourMessageHasBeenSent");
                    }
                    else
                        Response.Redirect(CommonHelper.GetStoreLocation());
                }
                catch (Exception exc)
                {
                    lblEmailAFriend.Text = exc.Message;
                }
            }
        }

        public int ProductId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductId");
            }
        }
    }
}