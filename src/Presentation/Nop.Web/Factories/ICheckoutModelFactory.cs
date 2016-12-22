using System.Collections.Generic;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using Nop.Web.Models.Checkout;

namespace Nop.Web.Factories
{
    public partial interface ICheckoutModelFactory
    {
        CheckoutBillingAddressModel PrepareBillingAddressModel(IList<ShoppingCartItem> cart,
            int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false,
            string overrideAttributesXml = "");

        CheckoutShippingAddressModel PrepareShippingAddressModel(int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false, string overrideAttributesXml = "");

        CheckoutShippingMethodModel PrepareShippingMethodModel(IList<ShoppingCartItem> cart, Address shippingAddress);

        CheckoutPaymentMethodModel PreparePaymentMethodModel(IList<ShoppingCartItem> cart, int filterByCountryId);

        CheckoutPaymentInfoModel PreparePaymentInfoModel(IPaymentMethod paymentMethod);

        CheckoutConfirmModel PrepareConfirmOrderModel(IList<ShoppingCartItem> cart);

        CheckoutCompletedModel PrepareCheckoutCompletedModel(Order order);

        CheckoutProgressModel PrepareCheckoutProgressModel(CheckoutProgressStep step);

        OnePageCheckoutModel PrepareOnePageCheckoutModel(IList<ShoppingCartItem> cart);
    }
}
