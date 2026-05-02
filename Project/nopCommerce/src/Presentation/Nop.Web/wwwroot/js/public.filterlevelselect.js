/*
** nopCommerce filter level select js functions
*/
+function ($) {
    'use strict';
    if ('undefined' == typeof (jQuery)) {
        throw new Error('jQuery JS required');
    }

    function filterLevel1SelectHandler() {
        var $this = $(this);
        var selectedItem = $this.val();
        var filterLevel2 = $($this.data('filterlevel2'));
        var filterLevel3 = $($this.data('filterlevel3'));

        if (filterLevel2.length == 0)
            return;

        var loading = $($this.data('loading'));
        loading.show();

        $.ajax({
            cache: false,
            type: "GET",
            url: $this.data('url'),
            data: {
                'filterLevel1Value': selectedItem,
                'filterLevel2Value': '',
                'filterLevel3Value': ''
            },
            success: function (data, textStatus, jqXHR) {
              filterLevel2.html('');
              if (selectedItem != '') {
                filterLevel2.append($('<option></option>').val('').html(data[0].defaultItemText));
              }                
              $.each(data, function (index, item) {
                if (selectedItem === item.filterLevel1Value) {
                  filterLevel2.append($('<option></option>').val(item.filterLevel2Value).html(item.filterLevel2Value));
                }
              });

                // Clear level 3 dropdown
                filterLevel3.html('');
                filterLevel3.append($('<option></option>').val('0').html(''));
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to retrieve filter level 2 values.');
            },
            complete: function (jqXHR, textStatus) {
                loading.hide();
            }
        });
    }

    function filterLevel2SelectHandler() {
        var $this = $(this);
        var selectedItem = $this.val();
        var filterLevel1 = $($this.data('filterlevel1'));
        var filterLevel3 = $($this.data('filterlevel3'));
        var selectedLevel1 = filterLevel1.val();

        if (filterLevel3.length == 0)
            return;

        var loading = $($this.data('loading'));
        loading.show();

        $.ajax({
            cache: false,
            type: "GET",
            url: $this.data('url'),
            data: {
                'filterLevel1Value': selectedLevel1,
                'filterLevel2Value': selectedItem,
                'filterLevel3Value': ''
            },
            success: function (data, textStatus, jqXHR) {
                filterLevel3.html('');
                filterLevel3.append($('<option></option>').val('').html(data[0].defaultItemText));
                $.each(data, function (index, item) {
                    if (selectedLevel1 === item.filterLevel1Value && selectedItem === item.filterLevel2Value) {
                        filterLevel3.append($('<option></option>').val(item.filterLevel3Value).html(item.filterLevel3Value));
                    }
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to retrieve filter level 3 values.');
            },
            complete: function (jqXHR, textStatus) {
                loading.hide();
            }
        });
    }

    if ($(document).has('[data-trigger="filter-level1-select"]')) {
        $('select[data-trigger="filter-level1-select"]').change(filterLevel1SelectHandler);
    }
    if ($(document).has('[data-trigger="filter-level2-select"]')) {
        $('select[data-trigger="filter-level2-select"]').change(filterLevel2SelectHandler);
    }

    $.fn.filterLevelSelect = function () {
        this.each(function () {
            $(this).change(filterLevel1SelectHandler);
        });
    }
}(jQuery);