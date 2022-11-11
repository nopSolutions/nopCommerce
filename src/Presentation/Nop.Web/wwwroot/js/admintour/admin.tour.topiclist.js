$(document).ready(function () {
  $('#topics-grid').on('draw.dt', function () {
    const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

    AdminTourNextPageButton.action = function () { window.location = '/Admin/Topic/Edit/' + AdminTourDataProvider.next_button_entity_id + '?showtour=True' };

    //'Topics (pages)' step
    tour.addStep({
      title: AdminTourDataProvider.localized_data.TopicListTopics1Title,
      text: AdminTourDataProvider.localized_data.TopicListTopics1Text,
      attachTo: {
        element: '#topics-area',
        on: 'bottom'
      },
      buttons: [AdminTourNextButton]
    });

    var topicRowId = 'row_shippinginfo';

    if ($('#' + topicRowId).length) {
      //'Topics (pages)' step
      tour.addStep({
        title: AdminTourDataProvider.localized_data.TopicListTopics2Title,
        text: AdminTourDataProvider.localized_data.TopicListTopics2Text,
        attachTo: {
          element: '#topics-area',
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextButton]
      });

      //'Topic row' step
      tour.addStep({
        title: AdminTourDataProvider.localized_data.TopicListShippingTitle,
        text: AdminTourDataProvider.localized_data.TopicListShippingText,
        attachTo: {
          element: '#' + topicRowId,
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextButton]
      });

      //'Link location' step
      tour.addStep({
        title: AdminTourDataProvider.localized_data.TopicListLocationTitle,
        text: AdminTourDataProvider.localized_data.TopicListLocationText,
        attachTo: {
          element: '#' + topicRowId + ' .column-footer-column1',
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextButton]
      });

      //'Edit the page' step
      tour.addStep({
        canClickTarget: true,
        title: AdminTourDataProvider.localized_data.TopicListEditTitle,
        text: AdminTourDataProvider.localized_data.TopicListEditText,
        attachTo: {
          element: '#' + topicRowId + ' .column-edit .btn',
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourNextPageButton]
      });
    } else {
      //'Topics (pages)' step
      tour.addStep({
        title: AdminTourDataProvider.localized_data.TopicListTopics2Title,
        text: AdminTourDataProvider.localized_data.TopicListTopics2Text,
        attachTo: {
          element: '#topics-area',
          on: 'bottom'
        },
        buttons: [AdminTourBackButton, AdminTourDoneButton]
      });
    }

    tour.start();
  });
})