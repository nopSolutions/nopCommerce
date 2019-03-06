/*
** nopCommerce country select js functions
*/
+function ($) {
    'use strict';
    if ('undefined' == typeof (jQuery)) {
        throw new Error('jQuery JS required');
    }
    function countrySelectHandler() {
        var $this = $(this);
        var selectedItem = $this.val();
        var stateProvince = $($this.data('stateprovince'));
        var loading = $($this.data('loading'));
        loading.show();
        $.ajax({
            cache: false,
            type: 'GET',
            url: $this.data('url'),
            data: { 'countryId': selectedItem, 'addSelectStateItem': "true" },
            success: function (data) {
                stateProvince.html('');
                $.each(data,
                    function (id, option) {
                        stateProvince.append($('<option></option>').val(option.id).html(option.name));
                    });
                loading.hide();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to retrieve states.');
                loading.hide();
            }
        });
    }
    if ($(document).has('[data-trigger="country-select"]')) {
        $('select[data-trigger="country-select"]').change(countrySelectHandler);
    }
    $.fn.countrySelect = function () {
        this.each(function () {
            $(this).change(countrySelectHandler);
        });
    }
}(jQuery); 