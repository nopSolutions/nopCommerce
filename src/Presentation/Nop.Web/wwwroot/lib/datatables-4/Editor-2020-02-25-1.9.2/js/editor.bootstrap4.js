/*! Bootstrap integration for DataTables' Editor
 * ©2015 SpryMedia Ltd - datatables.net/license
 */

(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net-bs4', 'datatables.net-editor'], function ( $ ) {
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
				$ = require('datatables.net-bs4')(root, $).$;
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
 * Set the default display controller to be our bootstrap control 
 */
DataTable.Editor.defaults.display = "bootstrap";


/*
 * Alter the buttons that Editor adds to TableTools so they are suitable for bootstrap
 */
var i18nDefaults = DataTable.Editor.defaults.i18n;
i18nDefaults.create.title = '<h5 class="modal-title">'+i18nDefaults.create.title+'</h5>';
i18nDefaults.edit.title = '<h5 class="modal-title">'+i18nDefaults.edit.title+'</h5>';
i18nDefaults.remove.title = '<h5 class="modal-title">'+i18nDefaults.remove.title+'</h5>';

var tt = DataTable.TableTools;
if ( tt ) {
	tt.BUTTONS.editor_create.formButtons[0].className = "btn btn-primary";
	tt.BUTTONS.editor_edit.formButtons[0].className = "btn btn-primary";
	tt.BUTTONS.editor_remove.formButtons[0].className = "btn btn-danger";
}


/*
 * Change the default classes from Editor to be classes for Bootstrap
 */
$.extend( true, $.fn.dataTable.Editor.classes, {
	"header": {
		"wrapper": "DTE_Header modal-header"
	},
	"body": {
		"wrapper": "DTE_Body modal-body"
	},
	"footer": {
		"wrapper": "DTE_Footer modal-footer"
	},
	"form": {
		"tag": "form-horizontal",
		"button": "btn",
		"buttonInternal": "btn btn-outline-secondary"
	},
	"field": {
		"wrapper": "DTE_Field form-group row",
		"label":   "col-lg-4 col-form-label",
		"input":   "col-lg-8",
		"error":   "error is-invalid",
		"msg-labelInfo": "form-text text-secondary small",
		"msg-info":      "form-text text-secondary small",
		"msg-message":   "form-text text-secondary small",
		"msg-error":     "form-text text-danger small",
		"multiValue":    "card multi-value",
		"multiInfo":     "small",
		"multiRestore":  "card multi-restore"
	}
} );

$.extend( true, DataTable.ext.buttons, {
	create: {
		formButtons: {
			className: 'btn-primary'
		}
	},
	edit: {
		formButtons: {
			className: 'btn-primary'
		}
	},
	remove: {
		formButtons: {
			className: 'btn-danger'
		}
	}
} );


/*
 * Bootstrap display controller - this is effectively a proxy to the Bootstrap
 * modal control.
 */

DataTable.Editor.display.bootstrap = $.extend( true, {}, DataTable.Editor.models.displayController, {
	/*
	 * API methods
	 */
	"init": function ( dte ) {
		var conf = {
			// Note that `modal-dialog-scrollable` is BS4.3+ only. It has no effect on 4.0-4.2
			content: $(
				'<div class="modal fade DTED">'+
					'<div class="modal-dialog modal-dialog-scrollable" />'+
				'</div>'
			),
			close: $('<button class="close">&times;</div>')
				.on('click', function () {
					dte.close('icon');
				}),
			shown: false,
			fullyShow: false
		}

		// This is a bit horrible, but if you mousedown and then drag out of the modal container, we don't
		// want to trigger a background action.
		var allowBackgroundClick = false;
		$(document).on('mousedown', 'div.modal', function (e) {
			allowBackgroundClick = $(e.target).hasClass('modal') && conf.shown
				? true
				: false;
		} );

		$(document).on('click', 'div.modal', function (e) {
			if ( $(e.target).hasClass('modal') && allowBackgroundClick ) {
				dte.background();
			}
		} );

		// Add `form-control` to required elements
		dte.on( 'displayOrder.dtebs', function ( e, display, action, form ) {
			$.each( dte.s.fields, function ( key, field ) {
				$('input:not([type=checkbox]):not([type=radio]), select, textarea', field.node() )
					.addClass( 'form-control' );
			} );
		} );

		dte._bootstrapDisplay = conf;

		return DataTable.Editor.display.bootstrap;
	},

	"open": function ( dte, append, callback ) {
		var conf = dte._bootstrapDisplay;

		$(append).addClass('modal-content');

		if ( conf._shown ) {
			// Modal already up, so just draw in the new content
			var content = conf.content.find('div.modal-dialog');
			content.children().detach();
			content.append( append );

			if ( callback ) {
				callback();
			}
			return;
		}

		conf.shown = true;
		conf.fullyDisplayed = false;

		var content = conf.content.find('div.modal-dialog');
		content.children().detach();
		content.append( append );

		$('div.modal-header', append).append( conf.close );

		$(conf.content)
			.one('shown.bs.modal', function () {
				// Can only give elements focus when shown
				if ( dte.s.setFocus ) {
					dte.s.setFocus.focus();
				}

				conf.fullyDisplayed = true;

				if ( callback ) {
					callback();
				}
			})
			.one('hidden', function () {
				conf.shown = false;
			})
			.appendTo( 'body' )
			.modal( {
				backdrop: "static",
				keyboard: false
			} );
	},

	"close": function ( dte, callback ) {
		var conf = dte._bootstrapDisplay;

		if ( !conf.shown ) {
			if ( callback ) {
				callback();
			}
			return;
		}

		// Check if actually displayed or not before hiding. BS4 doesn't like `hide`
		// before it has been fully displayed
		if ( ! conf.fullyDisplayed ) {
			$(conf.content)
				.one('shown.bs.modal', function () {
					conf.close( dte, callback );
				} );

			return;
		}

		$(conf.content)
			.one( 'hidden.bs.modal', function () {
				$(this).detach();
			} )
			.modal('hide');

		conf.shown = false;
		conf.fullyDisplayed = false;

		if ( callback ) {
			callback();
		}
	},

	node: function ( dte ) {
		return dte._bootstrapDisplay.content[0];
	}
} );


return DataTable.Editor;
}));
