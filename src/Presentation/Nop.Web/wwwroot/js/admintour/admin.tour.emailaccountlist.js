$(document).ready(function () {
  $('#email-accounts-grid').on('draw.dt', function () {
    const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

    AdminTourNextPageButton.action = function () { window.location = '/Admin/EmailAccount/Edit/' + AdminTourDataProvider.next_button_entity_id + '?showtour=True' };

    //'Email accounts' step
    tour.addStep({
      title: AdminTourDataProvider.localized_data.EmailAccountListEmailAccounts1Title,
      text: AdminTourDataProvider.localized_data.EmailAccountListEmailAccounts1Text,
      attachTo: {
        element: '#email-accounts-area',
        on: 'bottom'
      },
      buttons: [AdminTourNextButton]
    });


    //'Email accounts' step
    tour.addStep({
      title: AdminTourDataProvider.localized_data.EmailAccountListEmailAccounts2Title,
      text: AdminTourDataProvider.localized_data.EmailAccountListEmailAccounts2Text,
      attachTo: {
        element: '#email-accounts-area',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextButton]
    });

    var defaultEmailAccountRowId = 'row_testmailcom';

    //'Default email account' step
    tour.addStep({
      title: AdminTourDataProvider.localized_data.EmailAccountListDefaultEmailAccountTitle,
      text: AdminTourDataProvider.localized_data.EmailAccountListDefaultEmailAccountText,
      attachTo: {
        element: '#' + defaultEmailAccountRowId + ' .column-default .btn',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextButton]
    });

    //'Edit an email account' step
    tour.addStep({
      canClickTarget: true,
      title: AdminTourDataProvider.localized_data.EmailAccountListEditTitle,
      text: AdminTourDataProvider.localized_data.EmailAccountListEditText,
      attachTo: {
        element: '#' + defaultEmailAccountRowId + ' .column-edit .btn',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextPageButton]
    });

    tour.start();
  });
})