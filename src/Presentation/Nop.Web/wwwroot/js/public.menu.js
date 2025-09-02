var mainMenu = {
  init: function (togleSelector, activeClassName) {
    const menuDropdowns = document.querySelectorAll(togleSelector);

    let toggleClass = function (event) {
      this.parentNode.classList.toggle(activeClassName);
    };

    menuDropdowns.forEach((e) => {
      e.addEventListener("click", toggleClass.bind(e));
    })
  }
};
