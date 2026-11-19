var entityMap = {
  '&': '&amp;',
  '<': '&lt;',
  '>': '&gt;',
  '"': '&quot;',
  "'": '&#39;',
  '/': '&#x2F;',
  '`': '&#x60;',
  '=': '&#x3D;'
};

function escapeHtml(string) {
  if (string == null) {
    return '';
  }
  return String(string).replace(/[&<>"'`=\/]/g, function (s) {
    return entityMap[s];
  });
}

//selectedIds - This variable will be used on views. It can not be renamed
var selectedIds = [];


//tables which keep selectedIds when the page or the page size changes (keep registrations if this file is included twice)
var persistedSelectionTables = persistedSelectionTables || {};


function clearMasterCheckbox(tableSelector) {
  $($(tableSelector).parents('.dt-scroll').find('input.mastercheckbox')).prop('checked', false).change();
  selectedIds = [];
}


function isRowSelected(id) {
  return jQuery.inArray(String(id), selectedIds) > -1;
}


function initSelectionPersistence(tableSelector, checkboxName, infoSelector) {
  var state = { keepOnNextLoad: false, infoSelector: infoSelector };
  persistedSelectionTables[tableSelector] = state;
  var $table = $(tableSelector);

  //paging and page size changes keep the selection; any other reload (search, refresh, bulk action) clears it.
  //events of nested child tables bubble up here, so ignore them
  $table.on('page.dt length.dt', function (e) {
    if (e.target === this) {
      state.keepOnNextLoad = true;
    }
  });

  $table.on('draw.dt', function (e) {
    if (e.target === this) {
      updateMasterCheckbox($table.parents('.dt-scroll').first());
      updateSelectionInfo(tableSelector);
    }
  });

  $(infoSelector).on('click', 'a', function () {
    selectedIds = [];
    $(tableSelector + ' .checkboxGroups').prop('checked', false);
    updateMasterCheckbox($table.parents('.dt-scroll').first());
    updateSelectionInfo(tableSelector);
    return false;
  });

  //post the rows selected on other pages; the checkboxes of the current page are posted by the browser
  $table.closest('form').on('submit', function () {
    var $form = $(this);
    if (($form.attr('method') || '').toLowerCase() !== 'post') {
      return;
    }

    $form.find('input.dt-selection-hidden').remove();
    var visibleIds = $table.find('input.checkboxGroups').map(function () {
      return $(this).val();
    }).get();

    $.each(selectedIds, function (index, id) {
      if (jQuery.inArray(id, visibleIds) === -1) {
        $('<input type="hidden" class="dt-selection-hidden" />').attr('name', checkboxName).val(id).appendTo($form);
      }
    });
  });
}


function onTableLoading(tableSelector) {
  var state = persistedSelectionTables[tableSelector];
  if (!state || !state.keepOnNextLoad) {
    clearMasterCheckbox(tableSelector);
    return;
  }

  state.keepOnNextLoad = false;
  $($(tableSelector).parents('.dt-scroll').find('input.mastercheckbox')).prop('checked', false);
}


function updateSelectionInfo(tableSelector) {
  var state = persistedSelectionTables[tableSelector];
  if (!state) {
    return;
  }

  var $info = $(state.infoSelector);
  $info.find('.dt-selection-count').text($info.data('text').replace('{0}', selectedIds.length));
  $info.toggle(selectedIds.length > 0);
}

function clearMasterCheckbox(tableSelector) {
  $($(tableSelector).parents('.dt-scroll').find('input.mastercheckbox')).prop('checked', false).change();
  selectedIds = [];
}


function updateMasterCheckbox(tableSelector) {
  var selector = 'mastercheckbox';
  var numChkBoxes = $('input[type=checkbox][class!=' + selector + '][class=checkboxGroups]', $(tableSelector)).length;
  var numChkBoxesChecked = $('input[type=checkbox][class!=' + selector + '][class= checkboxGroups]:checked', $(tableSelector)).length;

  $('.mastercheckbox', $(tableSelector)).prop('checked', numChkBoxes == numChkBoxesChecked && numChkBoxes > 0);
}

function updateTableSrc(tableSelector, isMasterCheckBoxUsed) {
  var dataSrc = $(tableSelector).DataTable().data();
  $(tableSelector).DataTable().clear().rows.add(dataSrc).draw();
  $(tableSelector).DataTable().columns.adjust();
  
  if (isMasterCheckBoxUsed) {
    clearMasterCheckbox(tableSelector);
  }
}


function updateTable(tableSelector, isMasterCheckBoxUsed) {
  $(tableSelector).DataTable().ajax.reload();
  $(tableSelector).DataTable().columns.adjust();

  if (isMasterCheckBoxUsed) {
    clearMasterCheckbox(tableSelector);
  }
}


function updateTableWidth(tableSelector) {
  if ($.fn.DataTable.isDataTable(tableSelector)) {
    $(tableSelector).DataTable().columns.adjust();
  }
}