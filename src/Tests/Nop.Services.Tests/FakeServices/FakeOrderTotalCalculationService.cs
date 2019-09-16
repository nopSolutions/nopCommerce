using Moq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace Nop.Services.Tests.FakeServices
{
    public class FakeOrderTotalCalculationService : OrderTotalCalculationService
    {
        public FakeOrderTotalCalculationService(CatalogSettings catalogSettings = null,
            IAddressService addressService = null,
            ICheckoutAttributeParser checkoutAttributeParser = null,
            ICustomerService customerService = null,
            IDiscountService discountService = null,
            IGenericAttributeService genericAttributeService = null,
            IGiftCardService giftCardService = null,
            IOrderService orderService = null,
            IPaymentService paymentService = null,
            IPriceCalculationService priceCalculationService = null,
            IProductService productService = null,
            IRewardPointService rewardPointService = null,
            IShippingPluginManager shippingPluginManager = null,
            IShippingService shippingService = null,
            IShoppingCartService shoppingCartService = null,
            IStoreContext storeContext = null,
            ITaxService taxService = null,
            IWorkContext workContext = null,
            RewardPointsSettings rewardPointsSettings = null,
            ShippingSettings shippingSettings = null,
            ShoppingCartSettings shoppingCartSettings = null,
            TaxSettings taxSettings = null) : base(
                catalogSettings ?? new CatalogSettings(),
                addressService ?? new Mock<IAddressService>().Object,
                checkoutAttributeParser ?? new Mock<ICheckoutAttributeParser>().Object,
                customerService ?? new Mock<ICustomerService>().Object,
                discountService ?? new Mock<IDiscountService>().Object,
                genericAttributeService ?? new Mock<IGenericAttributeService>().Object,
                giftCardService ?? new Mock<IGiftCardService>().Object,
                orderService ?? new Mock<IOrderService>().Object,
                paymentService ?? new Mock<IPaymentService>().Object,
                priceCalculationService ?? new Mock<IPriceCalculationService>().Object,
                productService ?? new Mock<IProductService>().Object,
                rewardPointService ?? new Mock<IRewardPointService>().Object,
                shippingPluginManager ?? new Mock<IShippingPluginManager>().Object,
                shippingService ?? new Mock<IShippingService>().Object,
                shoppingCartService ?? new Mock<IShoppingCartService>().Object,
                storeContext ?? new Mock<IStoreContext>().Object,
                taxService ?? new Mock<ITaxService>().Object,
                workContext ?? new Mock<IWorkContext>().Object,
                rewardPointsSettings ?? new RewardPointsSettings(),
                shippingSettings ?? new ShippingSettings(),
                shoppingCartSettings ?? new ShoppingCartSettings(),
                taxSettings ?? new TaxSettings())
        {
        }
    }
}
