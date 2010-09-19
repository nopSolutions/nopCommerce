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
 * FCKShowBlockCommand Class: the "Show Blocks" command.
 */

var FCKShowBlockCommand = function( name, defaultState )
{
	this.Name = name ;
	if ( defaultState != undefined )
		this._SavedState = defaultState ;
	else
		this._SavedState = null ;
}

FCKShowBlockCommand.prototype.Execute = function()
{
	var state = this.GetState() ;

	if ( state == FCK_TRISTATE_DISABLED )
		return ;

	var body = FCK.EditorDocument.body ;

	if ( state == FCK_TRISTATE_ON )
		body.className = body.className.replace( /(^| )FCK__ShowBlocks/g, '' ) ;
	else
		body.className += ' FCK__ShowBlocks' ;

	if ( FCKBrowserInfo.IsIE )
	{
		try
		{
			FCK.EditorDocument.selection.createRange().select() ;
		}
		catch ( e )
		{}
	}
	else
	{
		var focus = FCK.EditorWindow.getSelection().focusNode ;
		if ( focus )
		{
			if ( focus.nodeType != 1 )
				focus = focus.parentNode ;
			FCKDomTools.ScrollIntoView( focus, false ) ;
		}
	}

	FCK.Events.FireEvent( 'OnSelectionChange' ) ;
}

FCKShowBlockCommand.prototype.GetState = function()
{
	if ( FCK.EditMode != FCK_EDITMODE_WYSIWYG )
		return FCK_TRISTATE_DISABLED ;

	// On some cases FCK.EditorDocument.body is not yet available
	if ( !FCK.EditorDocument )
		return FCK_TRISTATE_OFF ;

	if ( /FCK__ShowBlocks(?:\s|$)/.test( FCK.EditorDocument.body.className ) )
		return FCK_TRISTATE_ON ;

	return FCK_TRISTATE_OFF ;
}

FCKShowBlockCommand.prototype.SaveState = function()
{
	this._SavedState = this.GetState() ;
}

FCKShowBlockCommand.prototype.RestoreState = function()
{
	if ( this._SavedState != null && this.GetState() != this._SavedState )
		this.Execute() ;
}
