using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Checkout
{
    public partial class CheckoutPaymentMethodModel : BaseNopModel
    {
        public CheckoutPaymentMethodModel()
        {
            PaymentMethods = new List<PaymentMethodModel>();
        }

        public IList<PaymentMethodModel> PaymentMethods { get; set; }

        public bool DisplayRewardPoints { get; set; }
        public int RewardPointsBalance { get; set; }
        public string RewardPointsAmount { get; set; }
        public bool UseRewardPoints { get; set; }

        #region Nested classes

        public partial class PaymentMethodModel : BaseNopModel
        {
            public string PaymentMethodSystemName { get; set; }
            public string Name { get; set; }
            public string Fee { get; set; }
            public bool Selected { get; set; }
            public string LogoUrl { get; set; }
        }
        #endregion
    }
}