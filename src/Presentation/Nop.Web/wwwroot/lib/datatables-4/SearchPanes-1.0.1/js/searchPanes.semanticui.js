(function (factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD
        define(['jquery', 'datatables.net-se', 'datatables.net-searchpanes'], function ($) {
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
                $ = require('datatables.net-se')(root, $).$;
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
        buttonGroup: 'right floated ui buttons column',
        container: 'dtsp-searchPane column ui grid',
        dull: 'disabled',
        narrowSearch: 'dtsp-narrowSearch',
        narrowSub: 'dtsp-narrow',
        paneButton: 'ui button',
        paneInputButton: 'circular search link icon',
        searchCont: 'ui icon input eight wide column',
        topRow: 'row dtsp-topRow'
    });
    $.extend(true, DataTable.SearchPanes.classes, {
        clearAll: 'dtsp-clearAll ui button',
        container: 'dtsp-searchPanes ui grid'
    });
    // This override is required for the integrated search Icon in sematic ui
    DataTable.SearchPane.prototype._searchContSetup = function () {
        $('<i class="' + this.classes.paneInputButton + '"></i>').appendTo(this.dom.searchCont);
    };
    return DataTable.searchPanes;
}));
