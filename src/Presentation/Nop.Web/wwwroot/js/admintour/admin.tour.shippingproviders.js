$(function() {
  $('#shippingproviders-grid').on('draw.dt', function () {
    const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

    var manualMethodRowId = 'row_shippingfixedbyweightbytotal';
    
    var manualMethodExists = $('#' + manualMethodRowId).length;

    if (manualMethodExists) {
      AdminTourNextPageButton.action = function () { window.location = '/Admin/FixedByWeightByTotal/Configure?showtour=true' };
    } else {
      AdminTourNextPageButton.action = function () { window.location = '/Admin/Payment/Methods?showtour=True' };
    }

    //'Set up shipping' step
    var shippingMethodStepButtons = [];
    if (!manualMethodExists) {
      shippingMethodStepButtons = [AdminTourNextPageButton]
    } else {
      shippingMethodStepButtons = [AdminTourNextButton]
    }

    tour.addStep({
      title: AdminTourDataProvider.localized_data.ShippingProvidersProvidersTitle,
      text: AdminTourDataProvider.localized_data.ShippingProvidersProvidersText,
      attachTo: {
        element: '#shipping-methods-area',
        on: 'bottom'
      },
      buttons: shippingMethodStepButtons
    });

    //'Manual shipping provider' step
    if (manualMethodExists) {
      tour.addStep({
        title: AdminTourDataProvider.localized_data.ShippingProvidersManualTitle,
        text: AdminTourDataProvider.localized_data.ShippingProvidersManualText,
        attachTo: {
          element: '#' + manualMethodRowId,
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextButton]
      });
    }
       

    if (manualMethodExists) {      
        //'Activate provider' step
        tour.addStep({
          title: AdminTourDataProvider.localized_data.ShippingProvidersActivateTitle,
          text: AdminTourDataProvider.localized_data.ShippingProvidersActivateText,
          attachTo: {
            element: '#' + manualMethodRowId + ' .column-edit .btn-default',
            on: 'bottom'
          },
          buttons: [AdminTourBackButton, AdminTourNextButton],
        });

      //Redirect to Manual
      tour.addStep({
        canClickTarget: true,
        title: AdminTourDataProvider.localized_data.ShippingProvidersConfigureTitle,
        text: AdminTourDataProvider.localized_data.ShippingProvidersConfigureText,
        attachTo: {
          element: '#' + manualMethodRowId + ' .column-configure .btn-default',
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextPageButton]
      });
    }

    tour.start();
  });
})