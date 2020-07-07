/*! Semantic UI integration for DataTables' Editor
 * ©2018 SpryMedia Ltd - datatables.net/license
 */

(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net-se', 'datatables.net-editor'], function ( $ ) {
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
				$ = require('datatables.net-se')(root, $).$;
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


/*
 * Set the default display controller to be Semantic UI modal
 */
DataTable.Editor.defaults.display = "semanticui";

/*
 * Change the default classes from Editor to be classes for Bootstrap
 */
$.extend( true, $.fn.dataTable.Editor.classes, {
	"header": {
		"wrapper": "DTE_Header header"
	},
	"body": {
		"wrapper": "DTE_Body content"
	},
	"footer": {
		"wrapper": "DTE_Footer actions"
	},
	"form": {
		"tag": "ui form",
		"button": "ui button",
		"buttonInternal": "ui button",
		"content": 'DTE_Form_Content'
	},
	"field": {
		"wrapper": "DTE_Field inline fields",
		"label":   "right aligned five wide field",
		"input":   "eight wide field DTE_Field_Input",

		"error":   "error has-error",
		"msg-labelInfo": "ui small",
		"msg-info":      "ui small",
		"msg-message":   "ui message small",
		"msg-error":     "ui error message small",
		"multiValue":    "ui message multi-value",
		"multiInfo":     "small",
		"multiRestore":  "ui message multi-restore"
	},
	inline: {
		wrapper: "DTE DTE_Inline ui form"
	},
	bubble: {
		table: "DTE_Bubble_Table ui form",
		bg: "ui dimmer modals page transition visible active"
	}
} );


$.extend( true, DataTable.ext.buttons, {
	create: {
		formButtons: {
			className: 'primary'
		}
	},
	edit: {
		formButtons: {
			className: 'primary'
		}
	},
	remove: {
		formButtons: {
			className: 'negative'
		}
	}
} );

/*
 * Bootstrap display controller - this is effectively a proxy to the Bootstrap
 * modal control.
 */

var self;

DataTable.Editor.display.semanticui = $.extend( true, {}, DataTable.Editor.models.displayController, {
	/*
	 * API methods
	 */
	"init": function ( dte ) {
		// init can be called multiple times (one for each Editor instance), but
		// we only support a single construct here (shared between all Editor
		// instances)
		if ( ! self._dom.modal ) {
			self._dom.modal = $('<div class="ui modal DTED"></div>');

			self._dom.close = $('<i class="close icon"/>')
				.click( function (e) {
					self._dte.close('icon');
					return false;
				} );

			$(document).on('click', 'div.ui.dimmer.modals', function (e) {
				if ( $(e.target).hasClass('modal') && self._shown ) {
					self._dte.background();
				}
			} );
		}

		return self;
	},

	"open": function ( dte, append, callback ) {
		if ( self._shown ) {
			if ( callback ) {
				callback();
			}
			return;
		}

		self._dte = dte;
		self._shown = true;

		var modal = self._dom.modal;
		var appendChildren = $(append).children();

		// Clean up any existing elements and then insert the elements to
		// display. In Semantic UI we need to have the header, content and
		// actions at the top level of the modal rather than as children of a
		// wrapper.
		modal
			.children()
			.detach();

		modal
			.append( appendChildren )
			.prepend( modal.children('.header') ) // order is important
			.addClass( append.className )
			.prepend( self._dom.close );

		$(self._dom.modal)
			.modal( 'setting', {
				autofocus: false,
				dimmerSettings: {
					closable: false
				},
				onVisible: function () {
					// Can only give elements focus when shown
					if ( self._dte.s.setFocus ) {
						self._dte.s.setFocus.focus();
					}

					if ( callback ) {
						callback();
					}
				},
				onHidden: function () {
					$(append).append( appendChildren );
					self._shown = false;
				}
			} )
			.modal( 'show' );
	},

	"close": function ( dte, callback ) {
		var modal = self._dom.modal;

		if ( !self._shown ) {
			if ( callback ) {
				callback();
			}
			return;
		}

		modal.modal('hide');

		self._dte = dte;
		self._shown = false;

		if ( callback ) {
			callback();
		}
	},

	node: function ( dte ) {
		return self._dom.modal[0];
	},


	/*
	 * Private properties
	 */
	 "_shown": false,
	"_dte": null,
	"_dom": {}
} );

self = DataTable.Editor.display.semanticui;


return DataTable.Editor;
}));
