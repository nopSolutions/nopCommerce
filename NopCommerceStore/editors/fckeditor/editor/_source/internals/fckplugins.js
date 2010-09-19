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
 * Defines the FCKPlugins object that is responsible for loading the Plugins.
 */

var FCKPlugins = FCK.Plugins = new Object() ;
FCKPlugins.ItemsCount = 0 ;
FCKPlugins.Items = new Object() ;

FCKPlugins.Load = function()
{
	var oItems = FCKPlugins.Items ;

	// build the plugins collection.
	for ( var i = 0 ; i < FCKConfig.Plugins.Items.length ; i++ )
	{
		var oItem = FCKConfig.Plugins.Items[i] ;
		var oPlugin = oItems[ oItem[0] ] = new FCKPlugin( oItem[0], oItem[1], oItem[2] ) ;
		FCKPlugins.ItemsCount++ ;
	}

	// Load all items in the plugins collection.
	for ( var s in oItems )
		oItems[s].Load() ;

	// This is a self destroyable function (must be called once).
	FCKPlugins.Load = null ;
}
