using NUnit.Framework;

namespace Nop.Services.Tests.Customers
{
    [TestFixture]
    public class CustomerExtensionTests : ServiceTest
    {
        //[Test]
        //public void Can_get_add_remove_giftCardCouponCodes()
        //{
        //    var customer = new Customer();
        //    customer.ApplyGiftCardCouponCode("code1");
        //    customer.ApplyGiftCardCouponCode("code2");
        //    customer.RemoveGiftCardCouponCode("code2");
        //    customer.ApplyGiftCardCouponCode("code3");

        //    var codes = customer.ParseAppliedGiftCardCouponCodes();
        //    codes.Length.ShouldEqual(2);
        //    codes[0].ShouldEqual("code1");
        //    codes[1].ShouldEqual("code3");
        //}
        //[Test]
        //public void Can_not_add_duplicate_giftCardCouponCodes()
        //{
        //    var customer = new Customer();
        //    customer.ApplyGiftCardCouponCode("code1");
        //    customer.ApplyGiftCardCouponCode("code2");
        //    customer.ApplyGiftCardCouponCode("code1");

        //    var codes = customer.ParseAppliedGiftCardCouponCodes();
        //    codes.Length.ShouldEqual(2);
        //    codes[0].ShouldEqual("code1");
        //    codes[1].ShouldEqual("code2");
        //}
    }
}
