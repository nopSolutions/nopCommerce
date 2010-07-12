using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Modules;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class GiftCardAttributesControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected void BindData()
        {
            var pv = ProductManager.GetProductVariantById(this.ProductVariantId);
            if (pv == null || !pv.IsGiftCard)
            {
                this.Visible = false;
            }
            else
            {
                //pre-entered data
                if (NopContext.Current.User != null && !NopContext.Current.User.IsGuest)
                {
                    txtSenderName.Text = NopContext.Current.User.FullName;
                    txtSenderEmail.Text = NopContext.Current.User.Email;
                }

                //gift card type
                phRecipientEmail.Visible = (GiftCardTypeEnum)pv.GiftCardType == GiftCardTypeEnum.Virtual;
                phSenderEmail.Visible = (GiftCardTypeEnum)pv.GiftCardType == GiftCardTypeEnum.Virtual;
            }
        }

        public int ProductVariantId
        {
            get
            {
                if (ViewState["ProductVariantId"] == null)
                    return 0;
                else
                    return (int)ViewState["ProductVariantId"];
            }
            set
            {
                ViewState["ProductVariantId"] = value;
            }
        }

        public string RecipientName
        {
            get
            {
                return txtRecipientName.Text;
            }
        }

        public string RecipientEmail
        {
            get
            {
                return txtRecipientEmail.Text;
            }
        }

        public string SenderName
        {
            get
            {
                return txtSenderName.Text;
            }
        }

        public string SenderEmail
        {
            get
            {
                return txtSenderEmail.Text;
            }
        }

        public string GiftCardMessage
        {
            get
            {
                return txtGiftCardMessage.Text;
            }
        }
    }
}