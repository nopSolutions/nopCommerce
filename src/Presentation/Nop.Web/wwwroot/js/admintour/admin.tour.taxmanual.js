$(function() {
  $('#tax-categories-grid').on('draw.dt', function () {
    if ($('body').hasClass('advanced-settings-mode')) {
      $('.onoffswitch-checkbox').trigger('click');
    }

    const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

    AdminTourNextPageButton.action = function () { window.location = '/Admin/Product/Create?showtour=True' };

    //'Fixed Rate/By country' switch steps
    tour.addStep({
      title: AdminTourDataProvider.localized_data.TaxManualSwitchTitle,
      text: AdminTourDataProvider.localized_data.TaxManualSwitchText,
      attachTo: {
        element: '#onoffswitch-rate',
        on: 'bottom'
      },
      buttons: [AdminTourNextButton]
    });

    tour.addStep({
      title: AdminTourDataProvider.localized_data.TaxManualFixedTitle,
      text: AdminTourDataProvider.localized_data.TaxManualFixedText,
      attachTo: {
        element: '#onoffswitch-rate',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextButton]
    });

    tour.addStep({
      title: AdminTourDataProvider.localized_data.TaxManualByCountryTitle,
      text: AdminTourDataProvider.localized_data.TaxManualByCountryText,
      attachTo: {
        element: '#onoffswitch-rate',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextButton]
    });

    var firstEditButtonSelector = '#tax-categories-grid tr .column-edit .btn-default';
    var buttons = [];

    if ($(firstEditButtonSelector).length) {
      buttons = [AdminTourBackButton, AdminTourNextButton]
    } else {
      buttons = [AdminTourBackButton, AdminTourNextPageButton]
    }

    //'Tax categories' step
    tour.addStep({
      title: AdminTourDataProvider.localized_data.TaxManualCategoriesTitle,
      text: AdminTourDataProvider.localized_data.TaxManualCategoriesText,
      attachTo: {
        element: '#tax-categories-grid_wrapper',
        on: 'bottom'
      },
      buttons: buttons
    });

    //'Edit rate' step
    if ($(firstEditButtonSelector).length) {
      tour.addStep({
        title: AdminTourDataProvider.localized_data.TaxManualEditTitle,
        text: AdminTourDataProvider.localized_data.TaxManualEditText,
        attachTo: {
          element: firstEditButtonSelector,
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextPageButton]
      });
    }

    //TODO: 'Manage tax categories' step
    //tour.addStep({
    //  title: 'Manage tax categories',
    //  text: 'Using the <b>Manage shipping methods</b> button you can add new shipping methods or delete the existing ones',
    //  attachTo: {
    //    element: '#manage-shipping-methods-button',
    //    on: 'bottom'
    //  },
    //  buttons: [
    //    {
    //      action() {
    //        return tour.back();
    //      },
    //      classes: 'button-back',
    //      text: '<i class="far fa-arrow-left"></i> &nbsp; Back'
    //    },
    //    {
    //      action() {
    //        return tour.cancel();
    //      },
    //      classes: 'button-done',
    //      text: 'Done',
    //      secondary: true
    //    }
    //  ],
    //});

    tour.start();

  });
})