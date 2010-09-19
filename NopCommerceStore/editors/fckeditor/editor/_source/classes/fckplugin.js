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
 * FCKPlugin Class: Represents a single plugin.
 */

var FCKPlugin = function( name, availableLangs, basePath )
{
	this.Name = name ;
	this.BasePath = basePath ? basePath : FCKConfig.PluginsPath ;
	this.Path = this.BasePath + name + '/' ;

	if ( !availableLangs || availableLangs.length == 0 )
		this.AvailableLangs = new Array() ;
	else
		this.AvailableLangs = availableLangs.split(',') ;
}

FCKPlugin.prototype.Load = function()
{
	// Load the language file, if defined.
	if ( this.AvailableLangs.length > 0 )
	{
		var sLang ;

		// Check if the plugin has the language file for the active language.
		if ( this.AvailableLangs.IndexOf( FCKLanguageManager.ActiveLanguage.Code ) >= 0 )
			sLang = FCKLanguageManager.ActiveLanguage.Code ;
		else
			// Load the default language file (first one) if the current one is not available.
			sLang = this.AvailableLangs[0] ;

		// Add the main plugin script.
		LoadScript( this.Path + 'lang/' + sLang + '.js' ) ;
	}

	// Add the main plugin script.
	LoadScript( this.Path + 'fckplugin.js' ) ;
}
