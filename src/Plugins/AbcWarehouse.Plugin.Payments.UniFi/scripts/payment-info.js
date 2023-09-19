$(document).ready(function () {
	// Auto-open the modal
	$('#checkouthtml').trigger("click");
	$('.payment-info-next-step-button').hide();

    window.addEventListener("message",function(event) {
		if ((typeof event.data == 'string' || typeof event.data == 'object') && (event.data == 'Close Model' || event.data == 'Return To Merchant Shipping' || event.data == 'Close' || event.data.action == 'setPayCloseModal')) {
				console.log('SYNCHRONY UNIFI MODAL CLOSED', event);
			}
		});
	}
);