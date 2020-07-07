(function (factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD
        define(['jquery', 'datatables.net-zf', 'datatables.net-searchpanes'], function ($) {
            return factory($, window, document);
        });
    }
    else if (typeof exports === 'object') {
        // CommonJS
        module.exports = function (root, $) {
            if (!root) {
                root = window;
            }
            if (!$ || !$.fn.dataTable) {
                $ = require('datatables.net-zf')(root, $).$;
            }
            if (!$.fn.dataTable.searchPanes) {
                require('datatables.net-searchpanes')(root, $);
            }
            return factory($, root, root.document);
        };
    }
    else {
        // Browser
        factory(jQuery, window, document);
    }
}(function ($, window, document) {
    'use strict';
    var DataTable = $.fn.dataTable;
    $.extend(true, DataTable.SearchPane.classes, {
        buttonGroup: 'secondary button-group',
        disabledButton: 'disabled',
        dull: 'disabled',
        narrow: 'dtsp-narrow',
        narrowButton: 'dtsp-narrowButton',
        narrowSearch: 'dtsp-narrowSearch',
        paneButton: 'secondary button',
        pill: 'badge secondary',
        search: 'search',
        searchLabelCont: 'searchCont',
        show: 'col',
        table: 'unstriped'
    });
    $.extend(true, DataTable.SearchPanes.classes, {
        clearAll: 'dtsp-clearAll button secondary',
        panes: 'panes dtsp-panesContainer',
        title: 'dtsp-title'
    });
    return DataTable.searchPanes;
}));
