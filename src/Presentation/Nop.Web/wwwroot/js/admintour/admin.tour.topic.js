$(function() {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  //'Title and content' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.TopicTitleContentTitle,
    text: AdminTourDataProvider.localized_data.TopicTitleContentText,
    attachTo: {
      element: '#info-area',
      on: 'bottom'
    },
    buttons: [AdminTourNextButton]
  });

  //'Preview the page' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.TopicPreviewTitle,
    text: AdminTourDataProvider.localized_data.TopicPreviewText,
    attachTo: {
      element: '#preview-topic-button',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourDoneButton]
  });

  tour.start();
})