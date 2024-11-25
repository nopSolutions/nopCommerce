$(function() {
  $('#paymentmethods-grid').on('draw.dt', function () {
    const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

    var checkMoneyMethodRowId = 'row_paymentscheckmoneyorder';
    var manualMethodRowId = 'row_paymentsmanual';
    var paypalCommerceMethodCardId = 'card-paypalcommerce';

    var checkMoneyMethodExists = $('#' + checkMoneyMethodRowId).length;
    var manualMethodExists = $('#' + manualMethodRowId).length;
    var paypalCommerceMethodExists = $('#' + paypalCommerceMethodCardId).length;

    if (paypalCommerceMethodExists) {
      AdminTourNextPageButton.action = function () { window.location = '/Admin/PayPalCommerce/Configure?showtour=true' };
    } else {
      AdminTourNextPageButton.action = function () { window.location = '/Admin/Tax/Providers?showtour=True' }
    }

    //'Payment methods' step
    var paymentMethodsStepButtons = [];
    if (!checkMoneyMethodExists && !manualMethodExists && paypalCommerceMethodExists) {
      paymentMethodsStepButtons = [AdminTourNextPageButton]
    } else {
      paymentMethodsStepButtons = [AdminTourNextButton]
    }

    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentMethodsPaymentMethodsTitle,
      text: AdminTourDataProvider.localized_data.PaymentMethodsPaymentMethodsText,
      attachTo: {
        element: '#payment-methods-area',
        on: 'bottom'
      },
      buttons: paymentMethodsStepButtons
    });

    //'Check/Money Order' step
    if (checkMoneyMethodExists) {
      tour.addStep({
        title: AdminTourDataProvider.localized_data.PaymentMethodsCheckMoneyTitle,
        text: AdminTourDataProvider.localized_data.PaymentMethodsCheckMoneyText,
        attachTo: {
          element: '#' + checkMoneyMethodRowId,
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextButton]
      });

      if (!manualMethodExists && !paypalCommerceMethodExists) {
        //'Activate a payment method' step
        tour.addStep({
          title: AdminTourDataProvider.localized_data.PaymentMethodsActivateTitle,
          text: AdminTourDataProvider.localized_data.PaymentMethodsActivateText,
          attachTo: {
            element: '#' + checkMoneyMethodRowId + ' .column-edit .btn-default',
            on: 'bottom'
          },
          buttons: [AdminTourBackButton, AdminTourNextButton]
        });

        //'Configure a payment method' step
        tour.addStep({
          title: AdminTourDataProvider.localized_data.PaymentMethodsConfigureTitle,
          text: AdminTourDataProvider.localized_data.PaymentMethodsConfigureText,
          attachTo: {
            element: '#' + checkMoneyMethodRowId + ' .column-configure .btn-default',
            on: 'bottom'
          },
          buttons: [AdminTourBackButton, AdminTourNextPageButton]
        });
      }
    }

    //'Manual' step
    if (manualMethodExists) {
      tour.addStep({
        title: AdminTourDataProvider.localized_data.PaymentMethodsManualTitle,
        text: AdminTourDataProvider.localized_data.PaymentMethodsManualText,
        attachTo: {
          element: '#' + manualMethodRowId,
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextButton]
      });

      //'Activate a payment method' step
      tour.addStep({
        title: AdminTourDataProvider.localized_data.PaymentMethodsActivateTitle,
        text: AdminTourDataProvider.localized_data.PaymentMethodsActivateText,
        attachTo: {
          element: '#' + manualMethodRowId + ' .column-edit .btn-default',
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextButton]
      });

      //'Configure a payment method' step
      tour.addStep({
        title: AdminTourDataProvider.localized_data.PaymentMethodsConfigureTitle,
        text: AdminTourDataProvider.localized_data.PaymentMethodsConfigureText,
        attachTo: {
          element: '#' + manualMethodRowId + ' .column-configure .btn-default',
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, paypalCommerceMethodExists ? AdminTourNextButton : AdminTourNextPageButton]
      });
    }

    //'PayPal Commerce' step
    if (paypalCommerceMethodExists) {
      tour.addStep({
        title: AdminTourDataProvider.localized_data.PaymentMethodsPayPalTitle,
        text: AdminTourDataProvider.localized_data.PaymentMethodsPayPalText,
        attachTo: {
          element: '#' + paypalCommerceMethodCardId,
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextButton]
      });

      //'Configure a payment method' step
      tour.addStep({
        title: AdminTourDataProvider.localized_data.PaymentMethodsConfigurePayPalCommerceTitle,
        text: AdminTourDataProvider.localized_data.PaymentMethodsConfigurePayPalCommerceText,
        attachTo: {
          element: '#configure-paypalcommerce-button',
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextPageButton]
      });
    }

    tour.start();
  });
})