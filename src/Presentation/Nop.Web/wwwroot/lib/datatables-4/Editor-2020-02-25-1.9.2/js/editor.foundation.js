/*! Foundation integration for DataTables' Editor
 * ©2015 SpryMedia Ltd - datatables.net/license
 */

(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net-zf', 'datatables.net-editor'], function ( $ ) {
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
				$ = require('datatables.net-zf')(root, $).$;
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
 * Set the default display controller to be our foundation control 
 */
DataTable.Editor.defaults.display = "foundation";


/*
 * Change the default classes from Editor to be classes for Foundation
 */
$.extend( true, $.fn.dataTable.Editor.classes, {
	field: {
		wrapper:         "DTE_Field row",
		label:           "small-4 columns inline",
		input:           "small-8 columns",
		error:           "error",
		multiValue:      "panel radius multi-value",
		multiInfo:       "small",
		multiRestore:    "panel radius multi-restore",
		"msg-labelInfo": "label secondary",
		"msg-info":      "label secondary",
		"msg-message":   "label secondary",
		"msg-error":     "label alert"
	},
	form: {
		button:  "button small",
		buttonInternal:  "button small"
	}
} );


/*
 * Foundation display controller - this is effectively a proxy to the Foundation
 * modal control.
 */
var self;

DataTable.Editor.display.foundation = $.extend( true, {}, DataTable.Editor.models.displayController, {
	/*
	 * API methods
	 */
	"init": function ( dte ) {
		self._dom.content = $(
			'<div class="reveal reveal-modal DTED" data-reveal />'
		);
		self._dom.close = $('<button class="close close-button">&times;</div>');

		self._dom.close.click( function () {
			self._dte.close('icon');
		} );

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

		var content = self._dom.content;
		content.children().detach();
		content.append( append );
		content.prepend( self._dom.close );

		$(self._dom.content)
			.one('open.zf.reveal', function () {
				if ( callback ) {
					callback();
				}
			})
			.one('closed.zf.reveal', function () {
				self._shown = false;
			});

		if ( window.Foundation && window.Foundation.Reveal ) {
			// Foundation 6
			if ( ! self._reveal ) {
				self._reveal = new window.Foundation.Reveal( self._dom.content, {
					closeOnClick: false
				} );
			}

			//$(self._dom.content).appendTo('body');
			self._reveal.open();
		}
		else {
			// Foundation 5
			$(self._dom.content).foundation( 'reveal','open' );
		}

		$(document).on('click.dte-zf', 'div.reveal-modal-bg, div.reveal-overlay', function (e) {
			if ( $(e.target).closest(self._dom.content).length ) {
				return;
			}
			self._dte.background();
		} );
	},

	"close": function ( dte, callback ) {
		if ( !self._shown ) {
			if ( callback ) {
				callback();
			}
			return;
		}

		if ( self._reveal ) {
			self._reveal.close();
		}
		else {
			$(self._dom.content).foundation( 'reveal', 'close' );
		}

		$(document).off( 'click.dte-zf' );

		self._dte = dte;
		self._shown = false;

		if ( callback ) {
			callback();
		}
	},

	node: function ( dte ) {
		return self._dom.content[0];
	},


	/*
	 * Private properties
	 */
	 "_shown": false,
	"_dte": null,
	"_dom": {}
} );

self = DataTable.Editor.display.foundation;


return DataTable.Editor;
}));
