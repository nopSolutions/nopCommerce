$(function() {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/Topic/List?showtour=True' };

  //'Email address' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.EmailAccountEmailAddressTitle,
    text: AdminTourDataProvider.localized_data.EmailAccountEmailAddressText,
    attachTo: {
      element: '#email-area',
      on: 'bottom'
    },
    buttons: [AdminTourNextButton]
  });

  //'Email display name' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.EmailAccountDisplayNameTitle,
    text: AdminTourDataProvider.localized_data.EmailAccountDisplayNameText,
    attachTo: {
      element: '#display-name-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Host' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.EmailAccountHostTitle,
    text: AdminTourDataProvider.localized_data.EmailAccountHostText,
    attachTo: {
      element: '#host-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Port' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.EmailAccountPortTitle,
    text: AdminTourDataProvider.localized_data.EmailAccountPortText,
    attachTo: {
      element: '#port-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'SSL' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.EmailAccountUseSslTitle,
    text: AdminTourDataProvider.localized_data.EmailAccountUseSslText,
    attachTo: {
      element: '#ssl-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Authentication' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.EmailAccountAuthenticationMethodTitle,
    text: AdminTourDataProvider.localized_data.EmailAccountAuthenticationMethodText,
    attachTo: {
      element: '#authentication-method-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'Send test email' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.EmailAccountTestEmailTitle,
    text: AdminTourDataProvider.localized_data.EmailAccountTestEmailText,
    attachTo: {
      element: '#test-email-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextPageButton]
  });

  tour.start();
})