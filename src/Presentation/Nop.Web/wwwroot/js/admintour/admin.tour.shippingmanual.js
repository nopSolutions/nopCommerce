$(document).ready(function () {
  $('#shipping-rate-grid').on('draw.dt', function () {
    if ($('body').hasClass('advanced-settings-mode')) {
      $('.onoffswitch-checkbox').trigger('click');
    }

    const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

    AdminTourNextPageButton.action = function () { window.location = '/Admin/Payment/Methods?showtour=True' };

    //'Fixed Rate/By Weight' switch steps
    tour.addStep({
      title: AdminTourDataProvider.localized_data.ConfigureManualSwitchTitle,
      text: AdminTourDataProvider.localized_data.ConfigureManualSwitchText,
      attachTo: {
        element: '.onoffswitch',
        on: 'bottom'
      },
      buttons: [AdminTourNextButton]
    });

    tour.addStep({
      title: AdminTourDataProvider.localized_data.ConfigureManualFixedRateTitle,
      text: AdminTourDataProvider.localized_data.ConfigureManualFixedRateText,
      attachTo: {
        element: '.onoffswitch',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextButton]
    });

    tour.addStep({
      title: AdminTourDataProvider.localized_data.ConfigureManualByWeightTitle,
      text: AdminTourDataProvider.localized_data.ConfigureManualByWeightText,
      attachTo: {
        element: '.onoffswitch',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextButton]
    });


    //'Shipping methods' step
    tour.addStep({
      title: AdminTourDataProvider.localized_data.ConfigureManualMethodsTitle,
      text: AdminTourDataProvider.localized_data.ConfigureManualMethodsText,
      attachTo: {
        element: '#shipping-rate-grid_wrapper',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextButton]
    });

    //'Edit rate' step
    var firstEditButtonId = "buttonEdit_shipping_rate_grid1";

    if ($('#' + firstEditButtonId).length) {
      tour.addStep({
        title: AdminTourDataProvider.localized_data.ConfigureManualEditTitle,
        text: AdminTourDataProvider.localized_data.ConfigureManualEditText,
        attachTo: {
          element: '#' + firstEditButtonId,
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextButton]
      });
    }

    //'Manage shipping methods' step
    tour.addStep({
      title: AdminTourDataProvider.localized_data.ConfigureManualManageTitle,
      text: AdminTourDataProvider.localized_data.ConfigureManualManageText,
      attachTo: {
        element: '#manage-shipping-methods-button',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextPageButton]
    });

    tour.start();
  });
})