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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ProductEmailAFriend : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Common.EnableEmailAFirend"))
                Response.Redirect(CommonHelper.GetStoreLocation());

            if (!Page.IsPostBack)
                BindData();
        }

        protected void BindData()
        {
            var product = IoCFactory.Resolve<IProductService>().GetProductById(this.ProductId);
            if (product != null)
            {
                hlProduct.NavigateUrl = SEOHelper.GetProductUrl(product);
                hlProduct.Text = Server.HtmlEncode(product.LocalizedName);
                lShortDescription.Text = product.LocalizedShortDescription;

                if(NopContext.Current.User == null && IoCFactory.Resolve<ICustomerService>().AllowAnonymousUsersToEmailAFriend)
                {
                    IoCFactory.Resolve<ICustomerService>().CreateAnonymousUser();
                }

                if(NopContext.Current.User == null || (NopContext.Current.User.IsGuest && !IoCFactory.Resolve<ICustomerService>().AllowAnonymousUsersToEmailAFriend))
                {
                    lblEmailAFriend.Text = GetLocaleResourceString("Products.OnlyRegisteredUsersCanEmailAFriend");
                    pnlFriendsEmail.Visible = false;
                    pnlFrom.Visible = false;
                    pnlPersonalMessage.Visible = false;
                    btnEmail.Enabled = false;
                }
                else
                {
                    lblEmailAFriend.Text = string.Empty;
                    pnlFriendsEmail.Visible = true;
                    pnlFrom.Visible = true;
                    lblFrom.Text = NopContext.Current.User.Email;
                    pnlPersonalMessage.Visible = true;
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
                    var product = IoCFactory.Resolve<IProductService>().GetProductById(this.ProductId);
                    if (product != null)
                    {
                        if (NopContext.Current.User == null && IoCFactory.Resolve<ICustomerService>().AllowAnonymousUsersToEmailAFriend)
                        {
                            IoCFactory.Resolve<ICustomerService>().CreateAnonymousUser();
                        }

                        if (NopContext.Current.User == null || (NopContext.Current.User.IsGuest && !IoCFactory.Resolve<ICustomerService>().AllowAnonymousUsersToEmailAFriend))
                        {
                            lblEmailAFriend.Text = GetLocaleResourceString("Products.OnlyRegisteredUsersCanEmailAFriend");
                            return;
                        }

                        string friendsEmail = txtFriendsEmail.Text.Trim();
                        string personalMessage = txtPersonalMessage.Text.Trim();
                        personalMessage = personalMessage.FormatEmailAFriendText();

                        IoCFactory.Resolve<IMessageService>().SendProductEmailAFriendMessage(NopContext.Current.User,
                            NopContext.Current.WorkingLanguage.LanguageId, product,
                            friendsEmail, personalMessage);

                        txtFriendsEmail.Text = string.Empty;
                        txtPersonalMessage.Text = string.Empty;
                        lblEmailAFriend.Text = GetLocaleResourceString("Products.EmailAFriend.YourMessageHasBeenSent");
                    }
                    else
                    {
                        Response.Redirect(CommonHelper.GetStoreLocation());
                    }
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