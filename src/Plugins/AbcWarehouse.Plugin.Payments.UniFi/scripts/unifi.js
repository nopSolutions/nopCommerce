$(document).ready(function () {
	// Auto-open the modal
	$('#checkouthtml').trigger("click");
	$('.payment-info-next-step-button').hide();

    window.addEventListener("message",function(event) {
		if ((typeof event.data == 'string' || typeof event.data == 'object') && (event.data == 'Close Model' || event.data == 'Return To Merchant Shipping' || event.data == 'Close' || event.data.action == 'setPayCloseModal' || event.data.event == 'return-to-partner')) {
				var transactionToken = document.getElementById('transactionToken').value;
				$.ajax({
					cache: false,
					type: 'Post',
					url: `/checkout/UniFi/TransactionLookup/${transactionToken}`,
					data: null,
					dataType: 'json',
					success: function (response) {
						$('#checkouthtml').hide();
						var transactionMessage = response.transactionMessage;
						if (transactionMessage == 'Customer Approval Success') {
							var form = document.querySelector('.section.payment-info').getElementsByTagName('form')[0];
							form.action = "/checkout/paymentinfo";
							form.removeAttribute('target');
							form.submit();
						}
						else {
							if (transactionMessage !== 'Address verification check Fail' || transactionMessage !== 'Customer Terminated') {
								alert(`${transactionMessage}`);						
							}
							
							$('#synchrony-error-button').show();
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