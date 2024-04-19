//this method is used to show an element by removing the appropriate hiding class
//we don't use the jquery show/hide methods since they don't work with "display: flex" properly
$.fn.showElement = function () {
  this.removeClass('d-none');
}

//this method is used to hide an element by adding the appropriate hiding class
//we don't use the jquery show/hide methods since they don't work with "display: flex" properly
$.fn.hideElement = function () {
  this.addClass('d-none');
}

function setLocation(url) {
    window.location.href = url;
}

function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    winprops = 'resizable=1, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    var f = window.open(query, "_blank", winprops);
}

function showThrobber(message) {
    $('.throbber-header').html(message);
    window.setTimeout(function () {
        $(".throbber").show();
    }, 1000);
}

$(function() {
    $('.multi-store-override-option').each(function (k, v) {
        checkOverriddenStoreValue(v, $(v).attr('data-for-input-selector'));
    });

    //we must intercept all events of pressing the Enter button in the search bar to be sure that the input focus remains in the context of the search
    $("div.card-search").keypress(function (event) {
        if (event.which == 13 || event.keyCode == 13) {
          $("button.btn-search").trigger("click");
            return false;
        }
    });

    //pressing Enter in the tablex should not lead to any action
    $("div[id$='-grid']").keypress(function (event) {
        if (event.which == 13 || event.keyCode == 13) {
            return false;
        }
    });
});

function checkAllOverriddenStoreValue(item) {
    $('.multi-store-override-option').each(function (k, v) {
        $(v).attr('checked', item.checked);
        checkOverriddenStoreValue(v, $(v).attr('data-for-input-selector'));
    });
}

function checkOverriddenStoreValue(obj, selector) {
    var elementsArray = selector.split(",");

    // first toggle appropriate hidden inputs for checkboxes
    if ($(selector).is(':checkbox')) {
        var name = $(selector).attr('name');
        $('input:hidden[name="' + name + '"]').attr('disabled', !$(obj).is(':checked'));
    }

    if (!$(obj).is(':checked')) {
        $(selector).attr('disabled', true);
    }
    else {
        $(selector).removeAttr('disabled');
    }
}

function bindBootstrapTabSelectEvent(tabsId, inputId) {
    $('#' + tabsId + ' > div ul li a[data-toggle="pill"]').on('shown.bs.tab', function (e) {
        var tabName = $(e.target).attr("data-tab-name");
        $("#" + inputId).val(tabName);
    });
}

function display_nop_error(e) {
  if (e.error) {
    if ((typeof e.error) == 'string') {
      //single error
      //display the message
      alert(e.error);
    } else {
      //array of errors
      var message = "The following errors have occurred:";
      //create a message containing all errors.
      $.each(e.error, function (key, value) {
        if (value.errors) {
          message += "\n";
          message += value.errors.join("\n");
        }
      });
      //display the message
      alert(message);
    }
    //ignore empty error
  } else if (e.errorThrown) {
    alert('Error happened');
  }
}

// CSRF (XSRF) security
function addAntiForgeryToken(data) {
    //if the object is undefined, create a new one.
    if (!data) {
        data = {};
    }
    //add token
    var tokenInput = $('input[name=__RequestVerificationToken]');
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
};

function saveUserPreferences(url, name, value) {
    var postData = {
        name: name,
        value: value
    };
    addAntiForgeryToken(postData);
    $.ajax({
        cache: false,
        url: url,
        type: "POST",
        data: postData,
        dataType: "json",
        error: function (jqXHR, textStatus, errorThrown) {
          alert('Failed to save preferences.');
        },
        complete: function (jqXHR, textStatus) {
          $("#ajaxBusy span").removeClass("no-ajax-loader");
        }        
  });

};

function warningValidation(validationUrl, warningElementName, passedParameters) {
    addAntiForgeryToken(passedParameters);
    var element = $('[data-valmsg-for="' + warningElementName + '"]');

    var messageElement = element.siblings('.field-validation-custom');
    if (messageElement.length == 0) {
        messageElement = $(document.createElement("span"));
        messageElement.addClass('field-validation-custom');
        element.after(messageElement);
    }

    $.ajax({
        cache: false,
        url: validationUrl,
        type: "POST",
        dataType: "json",
        data: passedParameters,
        success: function (data, textStatus, jqXHR) {
            if (data.Result) {
                messageElement.addClass("warning");
                messageElement.html(data.Result);
            } else {
                messageElement.removeClass("warning");
                messageElement.html('');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            messageElement.removeClass("warning");
            messageElement.html('');
        }
    });
};

function toggleNestedSetting(parentSettingName, parentFormGroupId) {
    if ($('input[name="' + parentSettingName + '"]').is(':checked')) {
        $('#' + parentFormGroupId).addClass('opened');
    } else {
        $('#' + parentFormGroupId).removeClass('opened');
    }
}

function parentSettingClick(e) {
    toggleNestedSetting(e.data.parentSettingName, e.data.parentFormGroupId);
}

function initNestedSetting(parentSettingName, parentSettingId, nestedSettingId) {
    var parentFormGroup = $('input[name="' + parentSettingName +'"]').closest('.form-group');
    var parentFormGroupId = $(parentFormGroup).attr('id');
    if (!parentFormGroupId) {
        parentFormGroupId = parentSettingId;
    }
    $(parentFormGroup).addClass('parent-setting').attr('id', parentFormGroupId);
    if ($('#' + nestedSettingId + ' .form-group').length == $('#' + nestedSettingId + ' .form-group.advanced-setting').length) {
        $('#' + parentFormGroupId).addClass('parent-setting-advanced');
    }

    //$(document).on('click', 'input[name="' + parentSettingName + '"]', toggleNestedSetting(parentSettingName, parentFormGroupId));
    $('input[name="' + parentSettingName + '"]').click(
        { parentSettingName: parentSettingName, parentFormGroupId: parentFormGroupId }, parentSettingClick);
    toggleNestedSetting(parentSettingName, parentFormGroupId);
}

//scroll to top
(function ($) {
    $.fn.backTop = function () {
        var backBtn = this;

        var position = 1000;
        var speed = 900;

        $(document).scroll(function () {
            var pos = $(window).scrollTop();

            if (pos >= position) {
                backBtn.fadeIn(speed);
            } else {
                backBtn.fadeOut(speed);
            }
        });

        backBtn.click(function () {
            $("html, body").animate({ scrollTop: 0 }, 900);
        });
    }
}(jQuery));

// Ajax activity indicator bound to ajax start/stop document events
$(document).ajaxStart(function () {
    $('#ajaxBusy').show();
}).ajaxStop(function () {
    $('#ajaxBusy').hide();
    });

//no-tabs solution
$(function() {
  $(".card.card-secondary >.card-header").click(CardToggle);

  //expanded
  $('.card.card-secondary').on('expanded.lte.cardwidget', function () {
    WrapAndSaveBlockData($(this), false)
    
    if ($(this).find('table.dataTable').length > 0) {
      setTimeout(function () {
        ensureDataTablesRendered();
      }, 420);
    }
  });

  //collapsed
  $('.card.card-secondary').on('collapsed.lte.cardwidget', function () {
    WrapAndSaveBlockData($(this), true)
  });
});

function CardToggle() {
  var card = $(this).parent(".card.card-secondary");
  card.CardWidget('toggle'); 
}

function WrapAndSaveBlockData(card, collapsed) {
  var hideAttribute = card.attr("data-hideAttribute");
  saveUserPreferences(rootAppPath + 'admin/preferences/savepreference', hideAttribute, collapsed);
}

//collapse search block
$(function() {
  $(".row.search-row").click(ToggleSearchBlockAndSavePreferences);
});

function ToggleSearchBlockAndSavePreferences() {
    $(this).parents(".card-search").find(".search-body").slideToggle();
    var icon = $(this).find(".icon-collapse i");
    if ($(this).hasClass("opened")) {
      icon.removeClass("fa-angle-up");
      icon.addClass("fa-angle-down");
      saveUserPreferences(rootAppPath + 'admin/preferences/savepreference', $(this).attr("data-hideAttribute"), true);
    } else {
      icon.addClass("fa-angle-up");
      icon.removeClass("fa-angle-down");
      saveUserPreferences(rootAppPath + 'admin/preferences/savepreference', $(this).attr("data-hideAttribute"), false);
    }

    $(this).toggleClass("opened");
}

function ensureDataTablesRendered() {
  $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
}

function reloadAllDataTables(itemCount) {
  //depending on the number of elements, the time for animation of opening the menu should increase
  var timePause = 300;
  if (itemCount) {
    timePause = itemCount * 100;
  }
  $('table[class^="table"]').each(function () {
    setTimeout(function () {
      ensureDataTablesRendered();
    }, timePause);
  });
}

/**
 * @param {string} alertId Unique identifier of alert
 * @param {any} text Message text
 */
function showAlert(alertId, text)
{
    $('#' + alertId + '-info').text(text);
    $('#' + alertId).trigger("click");
}

//scrolling and hidden DataTables issue workaround
//More info - https://datatables.net/examples/api/tabs_and_scrolling.html
$(function() {
  $('button[data-card-widget="collapse"]').on('click', function (e) {
    //hack with waiting animation. 
    //when page is loaded, a box that should be collapsed have style 'display: none;'.that's why a table is not updated
    setTimeout(function () {
      ensureDataTablesRendered();
    }, 1);
  });

  // when tab item click
  $('.nav-tabs .nav-item').on('click', function (e) {
    setTimeout(function () {
      ensureDataTablesRendered();
    }, 1);
  });

  $('ul li a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    ensureDataTablesRendered();
  });

  $('#advanced-settings-mode').on('click', function (e) {
    ensureDataTablesRendered();
  });

  //when sidebar-toggle click
  $('#nopSideBarPusher').on('click', function (e) {
    reloadAllDataTables();
  });
});

/**
 * @param {string} masterCheckbox Master checkbox selector
 * @param {string} childCheckbox Child checkbox selector
 */
function prepareTableCheckboxes(masterCheckbox, childCheckbox) {
  //Handling the event of clicking on the master checkbox
  $(masterCheckbox).click(function () {
    $(childCheckbox).prop('checked', $(this).prop('checked'));
  });

  //Handling the event of clicking on a child checkbox
  $(childCheckbox).change(function () {
    $(masterCheckbox).prop('checked', $(childCheckbox + ':not(:checked)').length === 0 ? true : false);
  });

  //Determining the state of the master checkbox by the state of its children
  $(masterCheckbox).prop('checked', $(childCheckbox).length == $(childCheckbox + ':checked').length && $(childCheckbox).length > 0);
}
