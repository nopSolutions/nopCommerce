using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class UserAgreementControl: BaseNopFrontendUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                if (!this.OrderProductVariantGuid.HasValue)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                var opv = this.OrderService.GetOrderProductVariantByGuid(this.OrderProductVariantGuid.Value);
                if (opv == null)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                var productVariant = this.ProductService.GetProductVariantById(opv.ProductVariantId);
                if (productVariant == null || !productVariant.HasUserAgreement)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                lblUserAgreementText.Text = productVariant.UserAgreementText;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.cbIsAgree.Attributes.Add("onclick", "toggleContinueButton();");

            base.OnPreRender(e);
        }

        protected void BtnContinue_OnClick(object sender, EventArgs e)
        {
            if(cbIsAgree.Checked)
            {
                if (!this.OrderProductVariantGuid.HasValue)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                var opv = this.OrderService.GetOrderProductVariantByGuid(this.OrderProductVariantGuid.Value);
                if (opv != null)
                {
                    string url = this.DownloadService.GetDownloadUrl(opv);
                    url = CommonHelper.ModifyQueryString(url, "Agree=true", null);
                    Response.Redirect(url);
                }
            }
        }

        public Guid? OrderProductVariantGuid
        {
            get
            {
                return CommonHelper.QueryStringGuid("OrderProductVariantGUID");
            }
        }
    }
}