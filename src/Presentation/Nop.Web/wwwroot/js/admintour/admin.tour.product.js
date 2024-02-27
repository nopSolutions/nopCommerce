$(function() {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/EmailAccount/List?showtour=True' };

  //'Settings button' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ProductSettingsButtonTitle,
    text: AdminTourDataProvider.localized_data.ProductSettingsButtonText,
    attachTo: {
      element: '#product-editor-settings',
      on: 'bottom'
    },
    buttons: [AdminTourNextButton]
  });

  //'Product details' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ProductDetailsTitle,
    text: AdminTourDataProvider.localized_data.ProductDetailsText,
    attachTo: {
      element: '#product-details-area',
      on: 'bottom'
    },
    classes: 'step-with-image',
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Product price' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ProductPriceTitle,
    text: AdminTourDataProvider.localized_data.ProductPriceText,
    attachTo: {
      element: '#product-price-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Product tax category' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ProductTaxTitle,
    text: AdminTourDataProvider.localized_data.ProductTaxText,
    attachTo: {
      element: '#product-tax-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Product shipping info' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ProductShippingTitle,
    text: AdminTourDataProvider.localized_data.ProductShippingText,
    attachTo: {
      element: '#product-shipping-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Product inventory' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ProductInventoryTitle,
    text: AdminTourDataProvider.localized_data.ProductInventoryText,
    attachTo: {
      element: '#product-inventory-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Product pictures' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.ProductPicturesTitle,
    text: AdminTourDataProvider.localized_data.ProductPicturesText,
    attachTo: {
      element: '#product-pictures-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextPageButton]
  });

  tour.start();
})