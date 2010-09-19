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
 * Debug window control and operations.
 */

// Public function defined here must be declared in fckdebug_empty.js.

var FCKDebug =
{
	Output : function( message, color, noParse )
	{
		if ( ! FCKConfig.Debug )
			return ;

		try
		{
			this._GetWindow().Output( message, color, noParse ) ;
		}
		catch ( e ) {}	 // Ignore errors
	},

	OutputObject : function( anyObject, color )
	{
		if ( ! FCKConfig.Debug )
			return ;

		try
		{
			this._GetWindow().OutputObject( anyObject, color ) ;
		}
		catch ( e ) {}	 // Ignore errors
	},

	_GetWindow : function()
	{
		if ( !this.DebugWindow || this.DebugWindow.closed )
			this.DebugWindow = window.open( FCKConfig.BasePath + 'fckdebug.html', 'FCKeditorDebug', 'menubar=no,scrollbars=yes,resizable=yes,location=no,toolbar=no,width=600,height=500', true ) ;

		return this.DebugWindow ;
	}
} ;
