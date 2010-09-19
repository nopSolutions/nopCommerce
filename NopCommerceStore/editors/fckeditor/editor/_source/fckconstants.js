/*
 * FCKeditor - The text editor for Internet - http://www.fckeditor.net
 * Copyright (C) 2003-2010 Frederico Caldeira Knabben
 *
 * == BEGIN LICENSE ==
 *
 * Licensed under the terms of any of the following licenses at your
 * choice:
 *
 *  - GNU General Public License Version 2 or later (the "GPL")
 *    http://www.gnu.org/licenses/gpl.html
 *
 *  - GNU Lesser General Public License Version 2.1 or later (the "LGPL")
 *    http://www.gnu.org/licenses/lgpl.html
 *
 *  - Mozilla Public License Version 1.1 or later (the "MPL")
 *    http://www.mozilla.org/MPL/MPL-1.1.html
 *
 * == END LICENSE ==
 *
 * Defines some constants used by the editor. These constants are also
 * globally available in the page where the editor is placed.
 */

// Editor Instance Status.
var FCK_STATUS_NOTLOADED	= window.parent.FCK_STATUS_NOTLOADED	= 0 ;
var FCK_STATUS_ACTIVE		= window.parent.FCK_STATUS_ACTIVE		= 1 ;
var FCK_STATUS_COMPLETE		= window.parent.FCK_STATUS_COMPLETE		= 2 ;

// Tristate Operations.
var FCK_TRISTATE_OFF		= window.parent.FCK_TRISTATE_OFF		= 0 ;
var FCK_TRISTATE_ON			= window.parent.FCK_TRISTATE_ON			= 1 ;
var FCK_TRISTATE_DISABLED	= window.parent.FCK_TRISTATE_DISABLED	= -1 ;

// For unknown values.
var FCK_UNKNOWN				= window.parent.FCK_UNKNOWN				= -9 ;

// Toolbar Items Style.
var FCK_TOOLBARITEM_ONLYICON	= window.parent.FCK_TOOLBARITEM_ONLYICON	= 0 ;
var FCK_TOOLBARITEM_ONLYTEXT	= window.parent.FCK_TOOLBARITEM_ONLYTEXT	= 1 ;
var FCK_TOOLBARITEM_ICONTEXT	= window.parent.FCK_TOOLBARITEM_ICONTEXT	= 2 ;

// Edit Mode
var FCK_EDITMODE_WYSIWYG	= window.parent.FCK_EDITMODE_WYSIWYG	= 0 ;
var FCK_EDITMODE_SOURCE		= window.parent.FCK_EDITMODE_SOURCE		= 1 ;

var FCK_IMAGES_PATH = 'images/' ;		// Check usage.
var FCK_SPACER_PATH = 'images/spacer.gif' ;

var CTRL	= 1000 ;
var SHIFT	= 2000 ;
var ALT		= 4000 ;

var FCK_STYLE_BLOCK		= 0 ;
var FCK_STYLE_INLINE	= 1 ;
var FCK_STYLE_OBJECT	= 2 ;
