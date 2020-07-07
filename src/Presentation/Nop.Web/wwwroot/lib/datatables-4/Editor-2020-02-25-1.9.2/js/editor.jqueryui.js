/*! jQuery UI integration for DataTables' Editor
 * ©2015 SpryMedia Ltd - datatables.net/license
 */

(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net-jqui', 'datatables.net-editor'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		module.exports = function (root, $) {
			if ( ! root ) {
				root = window;
			}

			if ( ! $ || ! $.fn.dataTable ) {
				$ = require('datatables.net-jqui')(root, $).$;
			}

			if ( ! $.fn.dataTable.Editor ) {
				require('datatables.net-editor')(root, $);
			}

			return factory( $, root, root.document );
		};
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}(function( $, window, document, undefined ) {
'use strict';
var DataTable = $.fn.dataTable;


var Editor = DataTable.Editor;
var doingClose = false;

/*
 * Set the default display controller to be our foundation control 
 */
Editor.defaults.display = "jqueryui";

/*
 * Change the default classes from Editor to be classes for Bootstrap
 */
var buttonClass = "btn ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only";
$.extend( true, $.fn.dataTable.Editor.classes, {
	form: {
		button:  buttonClass,
		buttonInternal:  buttonClass
	}
} );

/*
 * jQuery UI display controller - this is effectively a proxy to the jQuery UI
 * modal control.
 */
Editor.display.jqueryui = $.extend( true, {}, Editor.models.displayController, {
	init: function ( dte ) {
		dte.__dialouge = $('<div class="DTED"/>')
			.css('display', 'none')
			.appendTo('body')
			.dialog( $.extend( true, Editor.display.jqueryui.modalOptions, {
				autoOpen: false,
				buttons: { "A": function () {} }, // fake button so the button container is created
				closeOnEscape: false // allow editor's escape function to run
			} ) );

		// Need to know when the dialogue is closed using its own trigger
		// so we can reset the form
		$(dte.__dialouge).on( 'dialogclose', function (e) {
			if ( ! doingClose ) {
				dte.close();
			}
		} );

		return Editor.display.jqueryui;
	},

	open: function ( dte, append, callback ) {
		dte.__dialouge
			.append( append )
			.dialog( 'open' );

		$(dte.dom.formError).appendTo(
			dte.__dialouge.parent().find('div.ui-dialog-buttonpane')
		);

		dte.__dialouge.parent().find('.ui-dialog-title').html( dte.dom.header.innerHTML );
		dte.__dialouge.parent().addClass('DTED');

		// Modify the Editor buttons to be jQuery UI suitable
		var buttons = $(dte.dom.buttons)
			.children()
			.addClass( 'ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' )
			.each( function () {
				$(this).wrapInner( '<span class="ui-button-text" />' );
			} );

		// Move the buttons into the jQuery UI button set
		dte.__dialouge.parent().find('div.ui-dialog-buttonset')
			.empty()
			.append( buttons.parent() );

		if ( callback ) {
			callback();
		}
	},

	close: function ( dte, callback ) {
		if ( dte.__dialouge ) {
			// Don't want to trigger a close() call from dialogclose!
			doingClose = true;
			dte.__dialouge.dialog( 'close' );
			doingClose = false;
		}

		if ( callback ) {
			callback();
		}
	},

	node: function ( dte ) {
		return dte.__dialouge[0];
	},

	// jQuery UI dialogues perform their own focus capture
	captureFocus: false
} );


Editor.display.jqueryui.modalOptions = {
	width: 600,
	modal: true
};


return DataTable.Editor;
}));
