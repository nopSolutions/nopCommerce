var PAYMENT_MOMO = {
    /**
     * Initialize payment status checker
     */
    init: function () {
        $(document).ready(function () {
            // Listen for payment method selection
            if ($('#payment-method-block').length) {
                // Wait for checkout form submission
                $('form').on('submit', function (e) {
                    // Only handle if MTN MoMo is selected
                    if ($('input[name="paymentmethod"]:checked').val() === 'Payments.Momo') {
                        // Get the MoMo reference ID from the server response
                        $(document).ajaxComplete(function (event, xhr, settings) {
                            if (settings.url.indexOf('OpcSavePaymentInfo') >= 0 ||
                                settings.url.indexOf('SavePaymentInfo') >= 0) {
                                
                                try {
                                    var response = JSON.parse(xhr.responseText);
                                    if (response.custom_values && response.custom_values.MomoReferenceId) {
                                        // Trigger payment initiated event with reference ID
                                        $(document).trigger('momo:paymentInitiated', [response.custom_values.MomoReferenceId]);
                                    }
                                } catch (e) {
                                    console.error('Error parsing payment response:', e);
                                }
                            }
                        });
                    }
                });
            }
        });
    }
};

// Initialize the payment handler
PAYMENT_MOMO.init();
