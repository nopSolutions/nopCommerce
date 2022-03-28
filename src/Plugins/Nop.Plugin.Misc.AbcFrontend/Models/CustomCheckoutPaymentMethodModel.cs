using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Misc.AbcFrontend.Models
{
    public record CustomCheckoutPaymentMethodModel : CheckoutPaymentMethodModel
    {
        public string Description { get; set; }

        public static CustomCheckoutPaymentMethodModel FromBase(CheckoutPaymentMethodModel model)
        {
            return new CustomCheckoutPaymentMethodModel()
            {
                PaymentMethods = model.PaymentMethods,
                RewardPointsBalance = model.RewardPointsBalance,
                RewardPointsAmount = model.RewardPointsAmount,
                RewardPointsEnoughToPayForOrder = model.RewardPointsEnoughToPayForOrder,
                UseRewardPoints = model.UseRewardPoints
            };
        }
    }
}
