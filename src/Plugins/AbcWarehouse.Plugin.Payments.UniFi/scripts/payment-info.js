$(document).ready(function () {
	// Auto-open the modal
	//$('#checkouthtml').trigger("click");
	$('.payment-info-next-step-button').hide();

    window.addEventListener("message",function(event) {
		if ((typeof event.data == 'string' || typeof event.data == 'object') && (event.data == 'Close Model' || event.data == 'Return To Merchant Shipping' || event.data == 'Close' || event.data.action == 'setPayCloseModal')) {
				console.log('SYNCHRONY UNIFI MODAL CLOSED', event);
				var transactionToken = document.getElementById('transactionToken').value;
				$.ajax({
					cache: false,
					type: 'Post',
					url: `/checkout/UniFi/TransactionLookup/${transactionToken}`,
					data: null,
					dataType: 'json',
					success: function (response) {
						var transactionMessage = response.transactionMessage;
						if (transactionMessage == 'Customer Approval Success') {
							var form = document.querySelector('form[action=\'https://spdpone.syfpos.com/mppcore/mppcheckout\']');
							form.action = "/checkout/paymentinfo";
							form.removeAttribute('target');
							form.submit();
						}
						else {
							alert(`Synchrony modal error: ${transactionMessage}.`);
						}
					},
					failure: function () {
						alert('Failed to perform Transaction Lookup.');
					}
				});
			}
		});
	}
);