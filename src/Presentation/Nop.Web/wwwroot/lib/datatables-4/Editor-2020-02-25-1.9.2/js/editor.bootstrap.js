/*! Bootstrap integration for DataTables' Editor
 * ©2015 SpryMedia Ltd - datatables.net/license
 */

(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net-bs', 'datatables.net-editor'], function ( $ ) {
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
				$ = require('datatables.net-bs')(root, $).$;
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
i18nDefaults.create.title = "<h3>"+i18nDefaults.create.title+"</h3>";
i18nDefaults.edit.title = "<h3>"+i18nDefaults.edit.title+"</h3>";
i18nDefaults.remove.title = "<h3>"+i18nDefaults.remove.title+"</h3>";

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
		"button": "btn btn-default",
		"buttonInternal": "btn btn-default"
	},
	"field": {
		"wrapper": "DTE_Field",
		"label":   "col-lg-4 control-label",
		"input":   "col-lg-8 controls",
		"error":   "error has-error",
		"msg-labelInfo": "help-block",
		"msg-info":      "help-block",
		"msg-message":   "help-block",
		"msg-error":     "help-block",
		"multiValue":    "well well-sm multi-value",
		"multiInfo":     "small",
		"multiRestore":  "well well-sm multi-restore"
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
	"init": function ( dte ) {
		var content = $(
			'<div class="modal fade DTED">'+
				'<div class="modal-dialog">'+
					'<div class="modal-content"/>'+
				'</div>'+
			'</div>'
		);
		var conf = {
			content: content,
			close: $('<button class="close">&times;</div>')
				.on('click', function () {
					dte.close('icon');
				}),
			modalContent: content.find('div.modal-content'),
			shown: false
		};

		$(document).on('mousedown', 'div.modal', function (e) {
			if ( $(e.target).hasClass('modal') && conf.shown ) {
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

		if ( conf.shown ) {
			if ( callback ) {
				callback();
			}
			return;
		}

		conf.shown = true;

		var content = conf.modalContent;
		content.children().detach();
		content.append( append );

		$('div.modal-header', append).prepend( conf.close );

		$(conf.content)
			.one('shown.bs.modal', function () {
				// Can only give elements focus when shown
				if ( dte.s.setFocus ) {
					dte.s.setFocus.focus();
				}

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

		$(conf.content)
			.one( 'hidden.bs.modal', function () {
				$(this).detach();
			} )
			.modal('hide');

		conf.shown = false;

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
