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

    var shippingTopicRowId = 'row_shippinginfo';

    //'Shipping info' step
    tour.addStep({
      title: AdminTourDataProvider.localized_data.TopicListShippingTitle,
      text: AdminTourDataProvider.localized_data.TopicListShippingText,
      attachTo: {
        element: '#' + shippingTopicRowId,
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextButton]
    });

    //'Link location' step
    tour.addStep({
      title: AdminTourDataProvider.localized_data.TopicListLocationTitle,
      text: AdminTourDataProvider.localized_data.TopicListLocationText,
      attachTo: {
        element: '#' + shippingTopicRowId + ' .column-footer-column1',
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
        element: '#' + shippingTopicRowId + ' .column-edit .btn',
        on: 'bottom'
      },
      buttons: [AdminTourBackButton, AdminTourNextPageButton]
    });

    tour.start();
  });
})