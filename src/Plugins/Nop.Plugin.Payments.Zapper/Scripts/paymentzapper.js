(function ($) {

    //function doZapper(sel, mid, sid, amount, ref) {
    //    var opts = {
    //        selector: sel,
    //        merchantId: mid,
    //        siteId: sid,
    //        billAmount: amount,
    //        reference: ref,
    //        paymentComplete: function (paymentResult) {
    //            if (paymentResult.status === 1) {
    //                console.log(paymentResult);
    //                //$("#zapPaymentId").text(paymentResult.payment.paymentId);
    //                //$("#zapZapperId").text(paymentResult.payment.zapperId);
    //                //$("#zapMerchantOrderId").text(paymentResult.payment.reference);
    //                //$("#zapAmount").text(paymentResult.payment.amountPaid);

    //                //$(".cart-top").slideUp();

    //                //$("#zapperPaySummary").delay(800).slideDown();
    //            }
    //        }

    //    }

    //    zapper(opts)
    //}

    function doZapperTest() {
        var opts = {
            selector: "#zapperPayment",
            merchantId: 28798,
            siteId: 30564,
            billAmount: 11.00,
            reference: "REF001",
            isRetry: false,
            paymentComplete: function (paymentResult) {
                if (paymentResult.status === 1) {
                    console.log(paymentResult);
                    //$("#zapPaymentId").text(paymentResult.payment.paymentId);
                    //$("#zapZapperId").text(paymentResult.payment.zapperId);
                    //$("#zapMerchantOrderId").text(paymentResult.payment.reference);
                    //$("#zapAmount").text(paymentResult.payment.amountPaid);

                    //$(".cart-top").slideUp();

                    //$("#zapperPaySummary").delay(800).slideDown();
                }
            }

        }

        zapper(opts)
    }
})(jQuery);