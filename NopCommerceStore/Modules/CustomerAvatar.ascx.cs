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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerAvatarControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            if (!Page.IsPostBack)
            {
                this.BindData();
            }

        }

        private void BindData()
        {
            pnlCustomerAvatarError.Visible = false;

            var customerAvatar = NopContext.Current.User.Avatar;
            int avatarSize = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("Media.Customer.AvatarSize", 85);
            string pictureUrl = string.Empty;
            if (customerAvatar != null)
            {
                pictureUrl = IoCFactory.Resolve<IPictureService>().GetPictureUrl(customerAvatar, avatarSize, false);
                this.btnRemoveAvatar.Visible = true;
            }
            else
            {
                pictureUrl = IoCFactory.Resolve<IPictureService>().GetDefaultPictureUrl(PictureTypeEnum.Avatar, avatarSize);
                this.btnRemoveAvatar.Visible = false;
            }
            this.iAvatar.ImageUrl = pictureUrl;
        }

        protected void btnRemoveAvatar_Click(object sender, EventArgs e)
        {
            try
            {
                IoCFactory.Resolve<IPictureService>().DeletePicture(NopContext.Current.User.AvatarId);

                NopContext.Current.User.AvatarId = 0;
                IoCFactory.Resolve<ICustomerService>().UpdateCustomer(NopContext.Current.User);
                BindData();
            }
            catch (Exception exc)
            {
                pnlCustomerAvatarError.Visible = true;
                lCustomerAvatarErrorMessage.Text = Server.HtmlEncode(exc.Message);
            }
        }

        protected void btnUploadAvatar_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid)
                {
                    if (!IoCFactory.Resolve<ICustomerService>().AllowCustomersToUploadAvatars)
                        throw new NopException("Uploading avatars is not allowed");

                    var customerAvatar = NopContext.Current.User.Avatar;
                    var customerPictureFile = fuAvatar.PostedFile;
                    
                    if ((customerPictureFile != null) && (!String.IsNullOrEmpty(customerPictureFile.FileName)))
                    {
                        int avatarMaxSize = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("Media.Customer.AvatarMaxSizeBytes", 20000);
                        if (customerPictureFile.ContentLength > avatarMaxSize)
                            throw new NopException(string.Format("Maximum avatar size is {0} bytes", avatarMaxSize));

                        byte[] customerPictureBinary = IoCFactory.Resolve<IPictureService>().GetPictureBits(customerPictureFile.InputStream, customerPictureFile.ContentLength);
                        if (customerAvatar != null)
                            customerAvatar = IoCFactory.Resolve<IPictureService>().UpdatePicture(customerAvatar.PictureId, customerPictureBinary, customerPictureFile.ContentType, true);
                        else
                            customerAvatar = IoCFactory.Resolve<IPictureService>().InsertPicture(customerPictureBinary, customerPictureFile.ContentType, true);
                    }
                    int customerAvatarId = 0;
                    if (customerAvatar != null)
                        customerAvatarId = customerAvatar.PictureId;

                    NopContext.Current.User.AvatarId = customerAvatarId;
                    IoCFactory.Resolve<ICustomerService>().UpdateCustomer(NopContext.Current.User);

                    BindData();
                }
            }
            catch (Exception exc)
            {
                pnlCustomerAvatarError.Visible = true;
                lCustomerAvatarErrorMessage.Text = Server.HtmlEncode(exc.Message);
            }
        }
    }
}