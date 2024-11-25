$(function() {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Shipping/Providers?showtour=True' };

  //'Your store name' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.StoreNameTitle,
    text: AdminTourDataProvider.localized_data.StoreNameText,
    attachTo: {
      element: '#store-name-area',
      on: 'bottom'
    },
    buttons: [AdminTourNextButton]
  });

  //'Your store URL' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.StoreUrlTitle,
    text: AdminTourDataProvider.localized_data.StoreUrlText,
    attachTo: {
      element: '#store-url-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Enable SSL' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.StoreSslTitle,
    text: AdminTourDataProvider.localized_data.StoreSslText,
    attachTo: {
      element: '#ssl-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextPageButton]
  });

  tour.start();
})