var AdminTourCommonTourOptions = {
  useModalOverlay: true,
    defaultStepOptions: {
    canClickTarget: false,
      popperOptions: {
      modifiers: [{
        name: 'offset',
        options: {
          offset: [0, 15],
        },
      }],
      },
    classes: 'admin-area-tour',
      cancelIcon: {
      enabled: true
    },
    modalOverlayOpeningPadding: '3',
      scrollTo: { behavior: 'smooth', block: 'center' },
    when: {
      show() {
        const tour = Shepherd.activeTour;
        const currentStep = tour.currentStep;
        const currentStepElement = currentStep.el;
        const header = currentStepElement.querySelector('.shepherd-header');
        const progress = document.createElement('span');
        progress.className = "shepherd-progress";
        progress.innerText = `${tour.steps.indexOf(tour.currentStep) + 1}/${tour.steps.length}`;
        header.insertBefore(progress, currentStepElement.querySelector('.shepherd-title'));

        //disable arrow navigation on the last step
        if (tour.steps.indexOf(tour.currentStep) + 1 == tour.steps.length) {
          const nextPageButton = $.grep(currentStep.options.buttons, function (button, i) {
            if (button.classes.indexOf('button-next') >= 0) {
              return button;
            }
          });

          if (nextPageButton.length) {
            tour.options.keyboardNavigation = false;

            $(document).on('keydown.admintour', function (e) {
              if (e.keyCode == 37) {
                tour.back();
                tour.options.keyboardNavigation = true;
                $(document).off('keydown.admintour');
              }
            });
          }
        } else {
          tour.options.keyboardNavigation = true;
        }
      }
    }
  }
}

var AdminTourDataProvider = {
  localized_data: false,
  next_button_entity_id: null,

  init: function (localized_data, next_button_entity_id) {
    this.localized_data = localized_data;
    this.next_button_entity_id = next_button_entity_id;
  }
}

var AdminTourBackButton = {}
var AdminTourNextButton = {}
var AdminTourNextPageButton = {}
var AdminTourDoneButton = {}

$(document).ready(function () {
  AdminTourBackButton = {
    classes: 'button-back',
    text: '<i class="fas fa-chevron-left"></i>' + '<div class="button-text">' + AdminTourDataProvider.localized_data.Back + '</div>',
    secondary: true,
    action() { return Shepherd.activeTour.back(); }
  }

  AdminTourNextButton = {
    classes: 'button-next',
    text: '<div class="button-text">' + AdminTourDataProvider.localized_data.NextStep + '</div>' + '<i class="fas fa-chevron-right"></i>',
    action() { return Shepherd.activeTour.next(); }
  };

  AdminTourNextPageButton = {
    classes: 'button-next-page',
    text: '<div class="button-text">' + AdminTourDataProvider.localized_data.NextPage + '</div>' + ' <i class="fas fa-angle-double-right"></i>',
    action() {
      //need to specify an action for each page separately
    },
  };

  AdminTourDoneButton = {
    classes: 'button-done',
    text: AdminTourDataProvider.localized_data.Done,
    action() { return Shepherd.activeTour.cancel(); }
  };
});