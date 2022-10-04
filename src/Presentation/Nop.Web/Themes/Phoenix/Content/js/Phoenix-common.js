/* Left Sidebar  */


$(window).load(function () {
  $(function () {
    if ($(this).width() <= 991) {
      var text = $('#sidebar-button').text();
      if (text.trim() === 'Hide Filters') {
        $('#sidebar-button').html('Show Filters');
      }
    }
    $(".sidebar-button").click(function () {
      $(".generalLeftSide").toggleClass("col-sidebar");
      $(".generalSideRight").toggleClass("col-full");
      var text = $('#sidebar-button').text();
      if (text.trim() === 'Hide Filters') {
        $('#sidebar-button').html('Show Filters');
      }
      else {
        $('#sidebar-button').html('Hide Filters');
      }
    });

    $(window).resize(function () {
      if ($(this).width() <= 991) {
        var text = $('#sidebar-button').text();
        if (text.trim() === 'Hide Filters') {
          $('#sidebar-button').html('Show Filters');
        }
      }
      else {
        var text = $('#sidebar-button').text();
        if (text.trim() === 'Show Filters') {
          $('#sidebar-button').html('Hide Filters');
        }
      }
    });

  });
});

$(document).ready(function () {
  $('#topcartlink').click(function () {
    $('.flyout-cart').addClass("slideright active");
    $('.px_cart_overlay').addClass("overlayadded");
    $('body').addClass("overflowhidden");
  });

  $('.px_mini_shopping_cart_title .pi-cart-cancel').click(function () {
    $('.flyout-cart').removeClass("slideright active");
    $('.px_cart_overlay').removeClass("overlayadded");
    $('body').removeClass("overflowhidden");
  });
});