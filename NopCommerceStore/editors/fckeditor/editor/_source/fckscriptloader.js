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
 * This is a utility object which can be used to load specific components of
 * FCKeditor, including all dependencies.
 */

var FCK_GENERIC = 1 ;
var FCK_GENERIC_SPECIFIC = 2 ;
var FCK_SPECIFIC = 3 ;

var FCKScriptLoader = new Object() ;
FCKScriptLoader.FCKeditorPath = '/fckeditor/' ;

FCKScriptLoader._Scripts = new Object() ;
FCKScriptLoader._LoadedScripts = new Object() ;

FCKScriptLoader._IsIE = (/msie/).test( navigator.userAgent.toLowerCase() ) ;

FCKScriptLoader.Load = function( scriptName )
{
	// Check if the script has already been loaded.
	if ( scriptName in FCKScriptLoader._LoadedScripts )
		return ;

	FCKScriptLoader._LoadedScripts[ scriptName ] = true ;

	var oScriptInfo = this._Scripts[ scriptName ] ;

	if ( !oScriptInfo )
	{
		alert( 'FCKScriptLoader: The script "' + scriptName + '" could not be loaded' ) ;
		return ;
	}

	for ( var i = 0 ; i < oScriptInfo.Dependency.length ; i++ )
	{
		this.Load( oScriptInfo.Dependency[i] ) ;
	}

	var sBaseScriptName = oScriptInfo.BasePath + scriptName.toLowerCase() ;

	if ( oScriptInfo.Compatibility == FCK_GENERIC || oScriptInfo.Compatibility == FCK_GENERIC_SPECIFIC )
		this._LoadScript( sBaseScriptName + '.js' ) ;

	if ( oScriptInfo.Compatibility == FCK_SPECIFIC || oScriptInfo.Compatibility == FCK_GENERIC_SPECIFIC )
	{
		if ( this._IsIE )
			this._LoadScript( sBaseScriptName + '_ie.js' ) ;
		else
			this._LoadScript( sBaseScriptName + '_gecko.js' ) ;
	}
}

FCKScriptLoader._LoadScript = function( scriptPathFromSource )
{
	document.write( '<script type="text/javascript" src="' + this.FCKeditorPath + 'editor/_source/' + scriptPathFromSource + '"><\/script>' ) ;
}

FCKScriptLoader.AddScript = function( scriptName, scriptBasePath, dependency, compatibility )
{
	this._Scripts[ scriptName ] =
	{
		BasePath : scriptBasePath || '',
		Dependency : dependency || [],
		Compatibility : compatibility || FCK_GENERIC
	} ;
}

/*
 * ####################################
 * ### Scripts Definition List
 */

FCKScriptLoader.AddScript( 'FCKConstants' ) ;
FCKScriptLoader.AddScript( 'FCKJSCoreExtensions' ) ;

FCKScriptLoader.AddScript( 'FCK_Xhtml10Transitional', '../dtd/' ) ;

FCKScriptLoader.AddScript( 'FCKDataProcessor'	, 'classes/'	, ['FCKConfig','FCKBrowserInfo','FCKRegexLib','FCKXHtml'] ) ;
FCKScriptLoader.AddScript( 'FCKDocumentFragment', 'classes/'	, ['FCKDomTools'], FCK_SPECIFIC ) ;
FCKScriptLoader.AddScript( 'FCKDomRange'		, 'classes/'	, ['FCKBrowserInfo','FCKJSCoreExtensions','FCKW3CRange','FCKElementPath','FCKDomTools','FCKTools','FCKDocumentFragment'], FCK_GENERIC_SPECIFIC ) ;
FCKScriptLoader.AddScript( 'FCKDomRangeIterator', 'classes/'	, ['FCKDomRange','FCKListsLib'], FCK_GENERIC ) ;
FCKScriptLoader.AddScript( 'FCKElementPath'		, 'classes/'	, ['FCKListsLib'], FCK_GENERIC ) ;
FCKScriptLoader.AddScript( 'FCKEnterKey'		, 'classes/'	, ['FCKDomRange','FCKDomTools','FCKTools','FCKKeystrokeHandler','FCKListHandler'], FCK_GENERIC ) ;
FCKScriptLoader.AddScript( 'FCKPanel'			, 'classes/'	, ['FCKBrowserInfo','FCKConfig','FCKTools'], FCK_GENERIC ) ;
FCKScriptLoader.AddScript( 'FCKImagePreloader'	, 'classes/' ) ;
FCKScriptLoader.AddScript( 'FCKKeystrokeHandler', 'classes/'	, ['FCKConstants','FCKBrowserInfo','FCKTools'], FCK_GENERIC ) ;
FCKScriptLoader.AddScript( 'FCKStyle'			, 'classes/'	, ['FCKConstants','FCKDomRange','FCKDomRangeIterator','FCKDomTools','FCKListsLib','FCK_Xhtml10Transitional'], FCK_GENERIC ) ;
FCKScriptLoader.AddScript( 'FCKW3CRange'		, 'classes/'	, ['FCKDomTools','FCKTools','FCKDocumentFragment'], FCK_GENERIC ) ;

FCKScriptLoader.AddScript( 'FCKBrowserInfo'		, 'internals/'	, ['FCKJSCoreExtensions'] ) ;
FCKScriptLoader.AddScript( 'FCKCodeFormatter'	, 'internals/' ) ;
FCKScriptLoader.AddScript( 'FCKConfig'			, 'internals/'	, ['FCKBrowserInfo','FCKConstants'] ) ;
FCKScriptLoader.AddScript( 'FCKDebug'			, 'internals/'	, ['FCKConfig'] ) ;
FCKScriptLoader.AddScript( 'FCKDomTools'		, 'internals/'	, ['FCKJSCoreExtensions','FCKBrowserInfo','FCKTools','FCKDomRange'], FCK_GENERIC ) ;
FCKScriptLoader.AddScript( 'FCKListsLib'		, 'internals/' ) ;
FCKScriptLoader.AddScript( 'FCKListHandler'		, 'internals/'	, ['FCKConfig', 'FCKDocumentFragment', 'FCKJSCoreExtensions','FCKDomTools'], FCK_GENERIC ) ;
FCKScriptLoader.AddScript( 'FCKRegexLib'		, 'internals/' ) ;
FCKScriptLoader.AddScript( 'FCKStyles'			, 'internals/'	, ['FCKConfig', 'FCKDocumentFragment', 'FCKDomRange','FCKDomTools','FCKElementPath','FCKTools'], FCK_GENERIC ) ;
FCKScriptLoader.AddScript( 'FCKTools'			, 'internals/'	, ['FCKJSCoreExtensions','FCKBrowserInfo'], FCK_GENERIC_SPECIFIC ) ;
FCKScriptLoader.AddScript( 'FCKXHtml'			, 'internals/'	, ['FCKBrowserInfo','FCKCodeFormatter','FCKConfig','FCKDomTools','FCKListsLib','FCKRegexLib','FCKTools','FCKXHtmlEntities'], FCK_GENERIC_SPECIFIC ) ;
FCKScriptLoader.AddScript( 'FCKXHtmlEntities'	, 'internals/'	, ['FCKConfig'] ) ;

// ####################################
