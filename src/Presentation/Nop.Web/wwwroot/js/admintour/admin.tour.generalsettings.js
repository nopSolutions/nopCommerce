$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Store/Edit/' + AdminTourDataProvider.next_button_entity_id + '?showtour=True' };

  //'Welcome' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PersonalizeStoreIntroTitle,
    text: AdminTourDataProvider.localized_data.PersonalizeStoreIntroText,
    buttons: [AdminTourNextButton]
  });

  //'Basic/Advanced mode' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PersonalizeStoreBasicAdvancedTitle,
    text: AdminTourDataProvider.localized_data.PersonalizeStoreBasicAdvancedText,
    attachTo: {
      element: '.onoffswitch',
      on: 'auto'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Choose a theme' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PersonalizeStoreThemeTitle,
    text: AdminTourDataProvider.localized_data.PersonalizeStoreThemeText,
    attachTo: {
      element: '#theme-area',
      on: 'auto'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Upload your logo' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.PersonalizeStoreLogoTitle,
    text: AdminTourDataProvider.localized_data.PersonalizeStoreLogoText,
    attachTo: {
      element: '#logo-area',
      on: 'auto'
    },
    buttons: [AdminTourBackButton, AdminTourNextPageButton]
  });

  tour.start();
})