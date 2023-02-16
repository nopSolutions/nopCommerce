$(document).ready(function () {
  if ($('body').hasClass('basic-settings-mode')) {
    //$('.onoffswitch-checkbox').trigger('click');
  }

  var isConfigured = $('#SetCredentialsManually').attr('Checked');

  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Tax/Providers?showtour=True' };

  if (isConfigured) {
    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentPayPalConfiguredTitle,
      text: AdminTourDataProvider.localized_data.PaymentPayPalConfiguredText,
      attachTo: {
        element: '#SetCredentialsManually',
        on: 'bottom'
      },
      buttons: [AdminTourNextPageButton]
    });
  }
  else {
    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentPayPalSignUpTitle,
      text: AdminTourDataProvider.localized_data.PaymentPayPalSignUpText,
      attachTo: {
        element: '#pnlOnboarding',
        on: 'bottom'
      },
      buttons: [AdminTourNextButton]
    });

    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentPayPalRegisterTitle,
      text: AdminTourDataProvider.localized_data.PaymentPayPalRegisterText,
      attachTo: {
        element: '#Email',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextButton]
    });

    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentPayPalRegisterTitle,
      text: AdminTourDataProvider.localized_data.PaymentPayPalRegisterText2,
      attachTo: {
        element: '#Email',
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
          $('#SetCredentialsManually').click();
          if ($('body').hasClass('basic-settings-mode')) {
            $('.onoffswitch-checkbox').trigger('click');
          }
          resolve();
        });
      },
      buttons: [AdminTourNextButton]
    });

    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentPayPalSandboxTitle,
      text: AdminTourDataProvider.localized_data.PaymentPayPalSandboxText,
      attachTo: {
        element: '#pnlCredentials > div:nth-child(2)',
        on: 'bottom'
      },
      buttons: [AdminTourNextButton]
    });

    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentPayPalCredentialsTitle,
      text: AdminTourDataProvider.localized_data.PaymentPayPalCredentialsText,
      attachTo: {
        element: '#pnlCredentials',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextButton]
    });

    var firstEditButtonSelector = 'body > div.wrapper > div.content-wrapper > section > div > div > form > div > div:nth-child(2) > div > div.card.card-default.advanced-setting';
    var buttons = [];

    if ($(firstEditButtonSelector).length) {
      buttons = [AdminTourBackButton, AdminTourNextButton]
    } else {
      buttons = [AdminTourBackButton, AdminTourNextPageButton]
    }

    tour.addStep({
      title: AdminTourDataProvider.localized_data.PaymentPayPalPaymentTypeTitle,
      text: AdminTourDataProvider.localized_data.PaymentPayPalPaymentTypeText,
      attachTo: {
        element: '#PaymentTypeId',
        on: 'bottom'
      },
      buttons: buttons
    });

    if ($(firstEditButtonSelector).length) {
      tour.addStep({
        title: AdminTourDataProvider.localized_data.PaymentPayPalProminentlyTitle,
        text: AdminTourDataProvider.localized_data.PaymentPayPalProminentlyText,
        attachTo: {
          element: firstEditButtonSelector,
          on: 'bottom'
        },
        beforeShowPromise: function () {
          return new Promise(function (resolve) {
            //$('#SetCredentialsManually').click();
            resolve();
          });
        },
        buttons: [AdminTourBackButton, AdminTourNextPageButton]
      });
    }
  }

  tour.start();
})