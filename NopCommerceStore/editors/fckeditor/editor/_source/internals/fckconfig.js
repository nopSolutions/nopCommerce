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
 * Creates and initializes the FCKConfig object.
 */

var FCKConfig = FCK.Config = new Object() ;

/*
	For the next major version (probably 3.0) we should move all this stuff to
	another dedicated object and leave FCKConfig as a holder object for settings only).
*/

// Editor Base Path
if ( document.location.protocol == 'file:' )
{
	FCKConfig.BasePath = decodeURIComponent( document.location.pathname.substr(1) ) ;
	FCKConfig.BasePath = FCKConfig.BasePath.replace( /\\/gi, '/' ) ;

	// The way to address local files is different according to the OS.
	// In Windows it is file:// but in MacOs it is file:/// so let's get it automatically
	var sFullProtocol = document.location.href.match( /^(file\:\/{2,3})/ )[1] ;
	// #945 Opera does strange things with files loaded from the disk, and it fails in Mac to load xml files
	if ( FCKBrowserInfo.IsOpera )
		sFullProtocol += 'localhost/' ;

	FCKConfig.BasePath = sFullProtocol + FCKConfig.BasePath.substring( 0, FCKConfig.BasePath.lastIndexOf( '/' ) + 1) ;
}
else
	FCKConfig.BasePath = document.location.protocol + '//' + document.location.host +
		document.location.pathname.substring( 0, document.location.pathname.lastIndexOf( '/' ) + 1) ;

FCKConfig.FullBasePath = FCKConfig.BasePath ;

FCKConfig.EditorPath = FCKConfig.BasePath.replace( /editor\/$/, '' ) ;

// There is a bug in Gecko. If the editor is hidden on startup, an error is
// thrown when trying to get the screen dimensions.
try
{
	FCKConfig.ScreenWidth	= screen.width ;
	FCKConfig.ScreenHeight	= screen.height ;
}
catch (e)
{
	FCKConfig.ScreenWidth	= 800 ;
	FCKConfig.ScreenHeight	= 600 ;
}

// Override the actual configuration values with the values passed throw the
// hidden field "<InstanceName>___Config".
FCKConfig.ProcessHiddenField = function()
{
	this.PageConfig = new Object() ;

	// Get the hidden field.
	var oConfigField = window.parent.document.getElementById( FCK.Name + '___Config' ) ;

	// Do nothing if the config field was not defined.
	if ( ! oConfigField ) return ;

	var aCouples = oConfigField.value.split('&') ;

	for ( var i = 0 ; i < aCouples.length ; i++ )
	{
		if ( aCouples[i].length == 0 )
			continue ;

		var aConfig = aCouples[i].split( '=' ) ;
		var sKey = decodeURIComponent( aConfig[0] ) ;
		var sVal = decodeURIComponent( aConfig[1] ) ;

		if ( sKey == 'CustomConfigurationsPath' )	// The Custom Config File path must be loaded immediately.
			FCKConfig[ sKey ] = sVal ;

		else if ( sVal.toLowerCase() == "true" )	// If it is a boolean TRUE.
			this.PageConfig[ sKey ] = true ;

		else if ( sVal.toLowerCase() == "false" )	// If it is a boolean FALSE.
			this.PageConfig[ sKey ] = false ;

		else if ( sVal.length > 0 && !isNaN( sVal ) )	// If it is a number.
			this.PageConfig[ sKey ] = parseFloat( sVal ) ;

		else										// In any other case it is a string.
			this.PageConfig[ sKey ] = sVal ;
	}
}

function FCKConfig_LoadPageConfig()
{
	var oPageConfig = FCKConfig.PageConfig ;
	for ( var sKey in oPageConfig )
		FCKConfig[ sKey ] = oPageConfig[ sKey ] ;
}

function FCKConfig_PreProcess()
{
	var oConfig = FCKConfig ;

	// Force debug mode if fckdebug=true in the QueryString (main page).
	if ( oConfig.AllowQueryStringDebug )
	{
		try
		{
			if ( (/fckdebug=true/i).test( window.top.location.search ) )
				oConfig.Debug = true ;
		}
		catch (e) { /* Ignore it. Much probably we are inside a FRAME where the "top" is in another domain (security error). */ }
	}

	// Certifies that the "PluginsPath" configuration ends with a slash.
	if ( !oConfig.PluginsPath.EndsWith('/') )
		oConfig.PluginsPath += '/' ;

	// If no ToolbarComboPreviewCSS, point it to EditorAreaCSS.
	var sComboPreviewCSS = oConfig.ToolbarComboPreviewCSS ;
	if ( !sComboPreviewCSS || sComboPreviewCSS.length == 0 )
		oConfig.ToolbarComboPreviewCSS = oConfig.EditorAreaCSS ;

	// Turn the attributes that will be removed in the RemoveFormat from a string to an array
	oConfig.RemoveAttributesArray = (oConfig.RemoveAttributes || '').split( ',' );

	if ( !FCKConfig.SkinEditorCSS || FCKConfig.SkinEditorCSS.length == 0 )
		FCKConfig.SkinEditorCSS = FCKConfig.SkinPath + 'fck_editor.css' ;

	if ( !FCKConfig.SkinDialogCSS || FCKConfig.SkinDialogCSS.length == 0 )
		FCKConfig.SkinDialogCSS = FCKConfig.SkinPath + 'fck_dialog.css' ;
}

// Define toolbar sets collection.
FCKConfig.ToolbarSets = new Object() ;

// Defines the plugins collection.
FCKConfig.Plugins = new Object() ;
FCKConfig.Plugins.Items = new Array() ;

FCKConfig.Plugins.Add = function( name, langs, path )
{
	FCKConfig.Plugins.Items.push( [name, langs, path] ) ;
}

// FCKConfig.ProtectedSource: object that holds a collection of Regular
// Expressions that defined parts of the raw HTML that must remain untouched
// like custom tags, scripts, server side code, etc...
FCKConfig.ProtectedSource = new Object() ;

// Generates a string used to identify and locate the Protected Tags comments.
FCKConfig.ProtectedSource._CodeTag = (new Date()).valueOf() ;

// Initialize the regex array with the default ones.
FCKConfig.ProtectedSource.RegexEntries = [
	// First of any other protection, we must protect all comments to avoid
	// loosing them (of course, IE related).
	/<!--[\s\S]*?-->/g ,

	// Script tags will also be forced to be protected, otherwise IE will execute them.
	/<script[\s\S]*?<\/script>/gi,

	// <noscript> tags (get lost in IE and messed up in FF).
	/<noscript[\s\S]*?<\/noscript>/gi
] ;

FCKConfig.ProtectedSource.Add = function( regexPattern )
{
	this.RegexEntries.push( regexPattern ) ;
}

FCKConfig.ProtectedSource.Protect = function( html )
{
	var codeTag = this._CodeTag ;
	function _Replace( protectedSource )
	{
		var index = FCKTempBin.AddElement( protectedSource ) ;
		return '<!--{' + codeTag + index + '}-->' ;
	}

	for ( var i = 0 ; i < this.RegexEntries.length ; i++ )
	{
		html = html.replace( this.RegexEntries[i], _Replace ) ;
	}

	return html ;
}

FCKConfig.ProtectedSource.Revert = function( html, clearBin )
{
	function _Replace( m, opener, index )
	{
		var protectedValue = clearBin ? FCKTempBin.RemoveElement( index ) : FCKTempBin.Elements[ index ] ;
		// There could be protected source inside another one.
		return FCKConfig.ProtectedSource.Revert( protectedValue, clearBin ) ;
	}

	var regex = new RegExp( "(<|&lt;)!--\\{" + this._CodeTag + "(\\d+)\\}--(>|&gt;)", "g" ) ;
	return html.replace( regex, _Replace ) ;
}

// Returns a string with the attributes that must be appended to the body
FCKConfig.GetBodyAttributes = function()
{
	var bodyAttributes = '' ;
	// Add id and class to the body.
	if ( this.BodyId && this.BodyId.length > 0 )
		bodyAttributes += ' id="' + this.BodyId + '"' ;
	if ( this.BodyClass && this.BodyClass.length > 0 )
		bodyAttributes += ' class="' + this.BodyClass + '"' ;

	return bodyAttributes ;
}

// Sets the body attributes directly on the node
FCKConfig.ApplyBodyAttributes = function( oBody )
{
	// Add ID and Class to the body
	if ( this.BodyId && this.BodyId.length > 0 )
		oBody.id = FCKConfig.BodyId ;
	if ( this.BodyClass && this.BodyClass.length > 0 )
		oBody.className += ' ' + FCKConfig.BodyClass ;
}
