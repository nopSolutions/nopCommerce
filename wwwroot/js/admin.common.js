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

$(document).ready(function () {
    $('.multi-store-override-option').each(function (k, v) {
        checkOverriddenStoreValue(v, $(v).attr('data-for-input-selector'));
    });

    //we must intercept all events of pressing the Enter button in the search bar to be sure that the input focus remains in the context of the search
    $("div.panel-search").keypress(function (event) {
        if (event.which == 13 || event.keyCode == 13) {
            $("button.btn-search").click();
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
    if (!$(obj).is(':checked')) {
        $(selector).attr('disabled', true);
        //Kendo UI elements are enabled/disabled some other way
        $.each(elementsArray, function(key, value) {
            var kenoduiElement = $(value).data("kendoNumericTextBox") || $(value).data("kendoMultiSelect");
            if (kenoduiElement !== undefined && kenoduiElement !== null) {
                kenoduiElement.enable(false);
            }
        }); 
    }
    else {
        $(selector).removeAttr('disabled');
        //Kendo UI elements are enabled/disabled some other way
        $.each(elementsArray, function(key, value) {
            var kenoduiElement = $(value).data("kendoNumericTextBox") || $(value).data("kendoMultiSelect");
            if (kenoduiElement !== undefined && kenoduiElement !== null) {
                kenoduiElement.enable();
            }
        });
    };
}

function bindBootstrapTabSelectEvent(tabsId, inputId) {
    $('#' + tabsId + ' > ul li a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
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
$(document).ready(function () {
    $(".panel.collapsible-panel >.panel-heading").click(WrapAndSaveBlockData);
});

function WrapAndSaveBlockData() {
    $(this).parents(".panel").find(">.panel-container").slideToggle();
    $("#ajaxBusy span").addClass("no-ajax-loader");
    var icon = $(this).find("i.toggle-icon");
    if ($(this).hasClass("opened")) {
        icon.removeClass("fa-minus");
        icon.addClass("fa-plus");
        saveUserPreferences(rootAppPath + 'admin/preferences/savepreference', $(this).attr("data-hideAttribute"), true);
    } else {
        icon.addClass("fa-minus");
        icon.removeClass("fa-plus");
        saveUserPreferences(rootAppPath + 'admin/preferences/savepreference', $(this).attr("data-hideAttribute"), false);
    }

    $(this).toggleClass("opened");
}

//collapse search block
$(document).ready(function () {
  $(".row.search-row").click(ToggleSearchBlockAndSavePreferences);
});

function ToggleSearchBlockAndSavePreferences() {
    $(this).parents(".panel-search").find(".search-body").slideToggle();
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

//scrolling and hidden DataTables issue workaround
//More info - https://datatables.net/examples/api/tabs_and_scrolling.html
$(document).ready(function () {
  $('ul li a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    ensureDataTablesRendered();
  });
  $(".panel.collapsible-panel >.panel-heading").click(function () {
    ensureDataTablesRendered();
  });
  $('#advanced-settings-mode').on('click', function (e) {
    ensureDataTablesRendered();
  });
});

//Recalculate the column widths
$(document).ready(function () {
  // when menu item click
  $('.treeview').on('click', function (e) {
    var itemCount = $(e.currentTarget).find('ul').children('li:not([class])').length;
       
    reloadAllDataTables(itemCount);
  });
  //when sidebar-toggle click
  $('#nopSideBarPusher').on('click', function (e) {
    reloadAllDataTables();
  });
});