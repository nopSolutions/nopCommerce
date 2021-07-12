$(document).on('click', '.timerange', function(e) {
    e.stopPropagation();
  if ($('.timerangepicker-container').not($(this).next('.timerangepicker-container')).length > 0) {
        //$('.timerangepicker-container').not($(this).next('.timerangepicker-container')).remove();
      $(document).click();
    }
    var input = $(this).find('input');

    var now = new Date();
    var hours = now.getHours();
    var minutes = now.getMinutes();
    var second = now.getSeconds();

    var range = {
      from: {
        hour: hours,
        minute: minutes,
        second: second
      },
      to: {
        hour: hours,
        minute: minutes,
        second: second
      },
      delivery: {
        hour: hours,
        minute: minutes,
        second: second
      }
    };

    if (input.val() !== "") {
      var timerange = input.val();
      var matches = timerange.match(/([0-9]{2}):([0-9]{2}):([0-9]{2})-([0-9]{2}):([0-9]{2}):([0-9]{2})-([0-9]{2}):([0-9]{2}):([0-9]{2})/);
      if( matches != undefined && matches != null && matches.length === 10) {
        range = {
          from: {
            hour: matches[1],
            minute: matches[2],
            second: matches[3]
          },
          to: {
            hour: matches[4],
            minute: matches[5],
            second: matches[6]
          },
          delivery: {
            hour: matches[7],
            minute: matches[8],
            second: matches[9]
          }
        }
      }
    };

    var html = '<div class="timerangepicker-container">'+
      '<div class="timerangepicker-from">'+
      '<label class="timerangepicker-label">From:</label>' +
      '<div class="timerangepicker-display hour">' +
          '<span class="increment fa fa-angle-up"></span>' +
          '<span class="value">'+('0' + range.from.hour).substr(-2)+'</span>' +
          '<span class="decrement fa fa-angle-down"></span>' +
      '</div>' +
      ':' +
      '<div class="timerangepicker-display minute">' +
          '<span class="increment fa fa-angle-up"></span>' +
          '<span class="value">'+('0' + range.from.minute).substr(-2)+'</span>' +
          '<span class="decrement fa fa-angle-down"></span>' +
      '</div>' +
      ':' +
      '<div class="timerangepicker-display second">' +
          '<span class="increment fa fa-angle-up"></span>' +
          '<span class="value">'+('0' + range.from.second).substr(-2)+'</span>' +
          '<span class="decrement fa fa-angle-down"></span>' +
      '</div>' +
      '</div>' +
      '<div class="timerangepicker-to">' +
      '<label class="timerangepicker-label">To:</label>' +
      '<div class="timerangepicker-display hour">' +
          '<span class="increment fa fa-angle-up"></span>' +
          '<span class="value">'+('0' + range.to.hour).substr(-2)+'</span>' +
          '<span class="decrement fa fa-angle-down"></span>' +
      '</div>' +
      ':' +
      '<div class="timerangepicker-display minute">' +
          '<span class="increment fa fa-angle-up"></span>' +
          '<span class="value">'+('0' + range.to.minute).substr(-2)+'</span>' +
          '<span class="decrement fa fa-angle-down"></span>' +
      '</div>' +
      ':' +
      '<div class="timerangepicker-display second">' +
          '<span class="increment fa fa-angle-up"></span>' +
          '<span class="value">'+('0' + range.from.second).substr(-2)+'</span>' +
          '<span class="decrement fa fa-angle-down"></span>' +
      '</div>' +
      '</div>' +
      '<div class="timerangepicker-delivery">'+
      '<label class="timerangepicker-label">Delivery:</label>' +
      '<div class="timerangepicker-display hour">' +
          '<span class="increment fa fa-angle-up"></span>' +
          '<span class="value">'+('0' + range.delivery.hour).substr(-2)+'</span>' +
          '<span class="decrement fa fa-angle-down"></span>' +
      '</div>' +
      ':' +
      '<div class="timerangepicker-display minute">' +
          '<span class="increment fa fa-angle-up"></span>' +
          '<span class="value">'+('0' + range.delivery.minute).substr(-2)+'</span>' +
          '<span class="decrement fa fa-angle-down"></span>' +
      '</div>' +
      ':' +
      '<div class="timerangepicker-display second">' +
          '<span class="increment fa fa-angle-up"></span>' +
          '<span class="value">'+('0' + range.from.second).substr(-2)+'</span>' +
          '<span class="decrement fa fa-angle-down"></span>' +
      '</div>' +
      '</div>' +
    '</div>';

    $(html).insertAfter(this);
    $('.timerangepicker-container').on(
      'click',
      '.timerangepicker-display.hour .increment',
      function(){
        var value = $(this).siblings('.value');
        value.text(
          increment(value.text(), 24, 1, 2)
        );
      }
    );

    $('.timerangepicker-container').on(
      'click',
      '.timerangepicker-display.hour .decrement',
      function(){
        var value = $(this).siblings('.value');
        value.text(
          decrement(value.text(), 24, 1, 2)
        );
      }
    );

    $('.timerangepicker-container').on(
      'click',
      '.timerangepicker-display.minute .increment, .timerangepicker-display.second .increment',
      function(){
        var value = $(this).siblings('.value');
        value.text(
          increment(value.text(), 59, 0 , 2)
        );
      }
    );

    $('.timerangepicker-container').on(
      'click',
      '.timerangepicker-display.minute .decrement, .timerangepicker-display.second .decrement',
      function(){
        var value = $(this).siblings('.value');
        value.text(
          decrement(value.text(), 59, 0, 2)
        );
      }
    );

});

  $(document).on('keyup, change', '.timerange', function (e) {
    if(!$(e.target).closest('.timerangepicker-container').length) {
      if($('.timerangepicker-container').is(":visible")) {
        var timerangeContainer = $('.timerangepicker-container');
        if (timerangeContainer.length > 0) {
          if ($(this).val() !== "") {
            var timerange = input.val();
            var matches = timerange.match(/([0-9]{2}):([0-9]{2}):([0-9]{2})-([0-9]{2}):([0-9]{2}):([0-9]{2})-([0-9]{2}):([0-9]{2}):([0-9]{2})/);
            if( matches != undefined && matches != null && matches.length === 10) {
              var timeRange = {
                from: {
                  hour: matches[1],
                  minute: matches[2],
                  second: matches[3]
                },
                to: {
                  hour: matches[4],
                  minute: matches[5],
                  second: matches[6]
                },
                delivery: {
                  hour: matches[7],
                  minute: matches[8],
                  second: matches[9]
                }
              }
            }
            timerangeContainer.parent().find('input').val(
              timeRange.from.hour+":"+
              timeRange.from.minute+":"+    
              timeRange.from.second+"-"+
              timeRange.to.hour+":"+
              timeRange.to.minute+":"+
              timeRange.to.second+"-"+
              timeRange.delivery.hour+":"+
              timeRange.delivery.minute+":"+
              timeRange.delivery.second
            );
          };
          
          timerangeContainer.remove();
        }
      }
    }
  });

  $(document).on('click', e => {

    if(!$(e.target).closest('.timerangepicker-container').length) {
      if($('.timerangepicker-container').is(":visible")) {
        var timerangeContainer = $('.timerangepicker-container');
        if(timerangeContainer.length > 0) {
          var timeRange = {
            from: {
              hour: timerangeContainer.find('.value')[0].innerText,
              minute: timerangeContainer.find('.value')[1].innerText,
              second: timerangeContainer.find('.value')[2].innerText
            },
            to: {
              hour: timerangeContainer.find('.value')[3].innerText,
              minute: timerangeContainer.find('.value')[4].innerText,
              second: timerangeContainer.find('.value')[5].innerText
            },
            delivery: {
              hour: timerangeContainer.find('.value')[6].innerText,
              minute: timerangeContainer.find('.value')[7].innerText,
              second: timerangeContainer.find('.value')[8].innerText
            },
          };

          timerangeContainer.parent().find('input').val(
            timeRange.from.hour+":"+
            timeRange.from.minute+":"+    
            timeRange.from.second+"-"+
            timeRange.to.hour+":"+
            timeRange.to.minute+":"+
            timeRange.to.second+"-"+
            timeRange.delivery.hour+":"+
            timeRange.delivery.minute+":"+
            timeRange.delivery.second
          );
          timerangeContainer.remove();
        }
      }
    }
    
  });

  function increment(value, max, min, size) {
    var intValue = parseInt(value);
    if (intValue == max) {
      return ('0' + min).substr(-size);
    } else {
      var next = intValue + 1;
      return ('0' + next).substr(-size);
    }
  }

  function decrement(value, max, min, size) {
    var intValue = parseInt(value);
    if (intValue == min) {
      return ('0' + max).substr(-size);
    } else {
      var next = intValue - 1;
      return ('0' + next).substr(-size);
    }
  }