
function createPopoverButton(popover, text, buttonClass, iconClass, clickListener, isForward) {
  let button = document.createElement("button");
  button.classList.add(buttonClass);

  let icon = document.createElement("i");
  icon.classList.add("fas", iconClass);
  icon.style.fontFamily = "FontAwesome";

  let buttonText = document.createElement("div");
  buttonText.classList.add("button-text");
  buttonText.textContent = text;

  var childrenElements = [icon, buttonText].sort((a, b) => isForward ? -1 : 1)
  button.append(...childrenElements);

  popover.footerButtons.appendChild(button);
  button.addEventListener("click", clickListener);
}

const AdminTourBuilder = {
  init: function (localizedData, nextPageEntityId, nextPageUrl, steps) {
    this.steps = steps;

    const tour = window.driver.js.driver({
      overlayOpacity: 0.5,
      disableActiveInteraction: true,
      popoverClass: 'admin-area-tour',
      popoverOffset: 15,
      stagePadding: 3,

      smoothScroll: true,

      showProgress: true,
      progressText: "{{current}}/{{total}}",

      steps: steps,

      onPopoverRender: (popover, { config, state }) => {

        var buttonsContainer = popover.footerButtons;
        while (buttonsContainer.firstChild) {
          buttonsContainer.removeChild(buttonsContainer.firstChild);
        }

        var isRightToLeft = getComputedStyle(document.getElementById('driver-popover-description')).direction == "rtl";

        if (tour.hasPreviousStep() && localizedData.Back) {
          createPopoverButton(popover, localizedData.Back, "button-back", "fa-chevron-" + (isRightToLeft ? "right" : "left"), () => tour.movePrevious(), false);
        }

        if (tour.hasNextStep() && localizedData.NextStep) {
          createPopoverButton(popover, localizedData.NextStep, "button-next", "fa-chevron-" + (isRightToLeft ? "left" : "right"), () => tour.moveNext(), true);
        }

        if (tour.isLastStep() && nextPageUrl && localizedData.NextPage) {
          createPopoverButton(popover, localizedData.NextPage, "button-next-page", "fa-angle-double-" + (isRightToLeft ? "left" : "right"), () => {
            if (nextPageUrl)
              window.location = nextPageUrl + nextPageEntityId;

            tour.destroy();
          }, true);
        }

        if (tour.isLastStep() && localizedData.Done) {
          createPopoverButton(popover, localizedData.Done, "button-done", "fa-check", () => tour.destroy());
        }
      }
    })

    return tour;
  }
}