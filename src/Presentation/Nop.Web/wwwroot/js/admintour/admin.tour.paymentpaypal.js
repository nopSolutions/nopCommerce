$(function () {
  if ($('body').hasClass('basic-settings-mode')) {
    //$('.onoffswitch-checkbox').trigger('click');
  }

  var isConfigured = $('#onboardingButton').length == 0;

  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Tax/Providers?showtour=True' };

  if (isConfigured) {
    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentPayPalConfiguredTitle,
      text: AdminTourDataProvider.localized_data.PaymentPayPalConfiguredText,
      attachTo: {
        element: '#pnlOnboarding',
        on: 'bottom'
      },
      buttons: [AdminTourNextButton]
    });

    var isAdvancedMode = $('#advanced-settings-mode').is(':checked');
    var paymentTypeSelector = $('#PaymentTypeId');
    if (paymentTypeSelector.length) {
      tour.addStep({
        title: AdminTourDataProvider.localized_data.PaymentPayPalPaymentTypeTitle,
        text: AdminTourDataProvider.localized_data.PaymentPayPalPaymentTypeText,
        attachTo: {
          element: '#PaymentTypeId',
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, isAdvancedMode ? AdminTourNextButton : AdminTourNextPageButton]
      });
    }

    if (isAdvancedMode) {
      tour.addStep({
        title: AdminTourDataProvider.localized_data.PaymentPayPalProminentlyTitle,
        text: AdminTourDataProvider.localized_data.PaymentPayPalProminentlyText,
        attachTo: {
          element: '#prominently-card',
          on: 'bottom'
        },
        beforeShowPromise: function () {
          return new Promise(function (resolve) {
            resolve();
          });
        },
        buttons: [AdminTourBackButton, AdminTourNextPageButton]
      });
    }
  }
  else {
    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentPayPalRegisterTitle,
      text: AdminTourDataProvider.localized_data.PaymentPayPalRegisterText,
      attachTo: {
        element: '#onboardingButton',
        on: 'bottom'
      },
      buttons: [AdminTourNextButton]
    });

    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentPayPalSandboxTitle,
      text: AdminTourDataProvider.localized_data.PaymentPayPalSandboxText,
      attachTo: {
        element: '#UseSandbox',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextButton]
    });

    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentPayPalApiCredentialsTitle,
      text: AdminTourDataProvider.localized_data.PaymentPayPalApiCredentialsText,
      attachTo: {
        element: '#SetCredentialsManually',
        on: 'bottom'
      },
      beforeShowPromise: function () {
        return new Promise(function (resolve) {
          $('#SetCredentialsManually').trigger("click");
          if ($('body').hasClass('basic-settings-mode')) {
            $('.onoffswitch-checkbox').trigger('click');
          }
          resolve();
        });
      },
      buttons: [AdminTourBackButton, AdminTourNextButton]
    });

    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentPayPalCredentialsTitle,
      text: AdminTourDataProvider.localized_data.PaymentPayPalCredentialsText,
      attachTo: {
        element: '#pnlCredentials',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextPageButton]
    });
  }

  tour.start();
})