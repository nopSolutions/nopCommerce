$(document).ready(function () {
  $('#tax-providers-grid').on('draw.dt', function () {
    const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

    var manualMethodRowId = 'row_taxfixedorbycountrystatezip';
    var avalaraMethodRowId = 'row_taxavalara';

    var manualMethodExists = $('#' + manualMethodRowId).length;
    var avalaraMethodExists = $('#' + avalaraMethodRowId).length;

    if (manualMethodExists) {
      AdminTourNextPageButton.action = function () { window.location = '/Admin/FixedOrByCountryStateZip/Configure?showtour=true' };
    } else {
      AdminTourNextPageButton.action = function () { window.location = '/Admin/Product/Create?showtour=True' };
    }

    //'Tax providers' step
    var taxProvidersStepButtons = [];
    if (!manualMethodExists && !avalaraMethodExists) {
      taxProvidersStepButtons = [AdminTourNextPageButton]
    } else {
      taxProvidersStepButtons = [AdminTourNextButton]
    }

    tour.addStep({
      title: AdminTourDataProvider.localized_data.TaxProvidersTaxProvidersTitle,
      text: AdminTourDataProvider.localized_data.TaxProvidersTaxProvidersText,
      attachTo: {
        element: '#tax-providers-area',
        on: 'bottom'
      },
      buttons: taxProvidersStepButtons
    });

    if (avalaraMethodExists) {
      //'Avalara tax provider' step
      tour.addStep({
        title: AdminTourDataProvider.localized_data.TaxProvidersAvalaraTitle,
        text: AdminTourDataProvider.localized_data.TaxProvidersAvalaraText,
        attachTo: {
          element: '#' + avalaraMethodRowId,
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextButton]
      });

      //'Mark as a primary provider' step
      if (!manualMethodExists) {
        tour.addStep({
          title: AdminTourDataProvider.localized_data.TaxProvidersPrimaryProviderTitle,
          text: AdminTourDataProvider.localized_data.TaxProvidersPrimaryProviderText,
          attachTo: {
            element: '#' + avalaraMethodRowId + ' .column-primary .btn',
            on: 'bottom'
          },
          buttons: [AdminTourBackButton, AdminTourNextPageButton]
        });
      }
    }

    if (manualMethodExists) {
      //'Manual tax provider' step
      tour.addStep({
        title: AdminTourDataProvider.localized_data.TaxProvidersManualTitle,
        text: AdminTourDataProvider.localized_data.TaxProvidersManualText,
        attachTo: {
          element: '#' + manualMethodRowId,
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextButton]
      });

      //'Mark as a primary provider' step
      tour.addStep({
        title: AdminTourDataProvider.localized_data.TaxProvidersPrimaryProviderTitle,
        text: AdminTourDataProvider.localized_data.TaxProvidersPrimaryProviderText,
        attachTo: {
          element: '#' + manualMethodRowId + ' .column-primary .btn',
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextButton]
      });

      //Redirect to Manual
      tour.addStep({
        canClickTarget: true,
        title: AdminTourDataProvider.localized_data.TaxProvidersConfigureTitle,
        text: AdminTourDataProvider.localized_data.TaxProvidersConfigureText,
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