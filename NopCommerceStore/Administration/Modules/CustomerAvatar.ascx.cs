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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerAvatarControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Customer customer = IoCFactory.Resolve<ICustomerManager>().GetCustomerById(this.CustomerId);
            if(customer != null)
            {
                var customerAvatar = customer.Avatar;
                int avatarSize = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("Media.Customer.AvatarSize", 85);
                string pictureUrl = string.Empty;
                if(customerAvatar != null)
                {
                    pictureUrl = IoCFactory.Resolve<IPictureManager>().GetPictureUrl(customerAvatar, avatarSize, false);
                    this.btnRemoveAvatar.Visible = true;
                }
                else
                {
                    pictureUrl = IoCFactory.Resolve<IPictureManager>().GetDefaultPictureUrl(PictureTypeEnum.Avatar, avatarSize);
                    this.btnRemoveAvatar.Visible = false;
                }
                this.imgAvatar.ImageUrl = pictureUrl;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {              
                this.BindData();
            }
        }

        protected void btnUploadAvatar_Click(object sender, EventArgs e)
        {
            try
            {
                if(Page.IsValid)
                {
                    Customer customer = IoCFactory.Resolve<ICustomerManager>().GetCustomerById(this.CustomerId);
                    if(customer != null)
                    {
                        var customerAvatar = customer.Avatar;
                        var customerPictureFile = uplAvatar.PostedFile;

                        if((customerPictureFile != null) && (!String.IsNullOrEmpty(customerPictureFile.FileName)))
                        {
                            int avatarMaxSize = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("Media.Customer.AvatarMaxSizeBytes", 20000);
                            if(customerPictureFile.ContentLength > avatarMaxSize)
                                throw new NopException(string.Format("Maximum avatar size is {0} bytes", avatarMaxSize));

                            byte[] customerPictureBinary = IoCFactory.Resolve<IPictureManager>().GetPictureBits(customerPictureFile.InputStream, customerPictureFile.ContentLength);
                            if(customerAvatar != null)
                                customerAvatar = IoCFactory.Resolve<IPictureManager>().UpdatePicture(customerAvatar.PictureId, customerPictureBinary, customerPictureFile.ContentType, true);
                            else
                                customerAvatar = IoCFactory.Resolve<IPictureManager>().InsertPicture(customerPictureBinary, customerPictureFile.ContentType, true);
                        }
                        int customerAvatarId = 0;
                        if(customerAvatar != null)
                            customerAvatarId = customerAvatar.PictureId;

                        customer.AvatarId = customerAvatarId;
                        IoCFactory.Resolve<ICustomerManager>().UpdateCustomer(customer);

                        BindData();
                    }
                }
            }
            catch(Exception exc)
            {
                ShowError(exc.Message);
            }
        }

        protected void btnRemoveAvatar_Click(object sender, EventArgs e)
        {
            try
            {
                Customer customer = IoCFactory.Resolve<ICustomerManager>().GetCustomerById(this.CustomerId);
                if(customer != null)
                {
                    IoCFactory.Resolve<IPictureManager>().DeletePicture(customer.AvatarId);
                    customer.AvatarId = 0;
                    IoCFactory.Resolve<ICustomerManager>().UpdateCustomer(customer);
                    BindData();
                }
            }
            catch(Exception exc)
            {
                ShowError(exc.Message);
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