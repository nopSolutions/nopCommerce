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
 * Defines the FCKXml object that is used for XML data calls
 * and XML processing.
 *
 * This script is shared by almost all pages that compose the
 * File Browser frameset.
 */

var FCKXml = function()
{}

FCKXml.prototype.GetHttpRequest = function()
{
	// Gecko / IE7
	try { return new XMLHttpRequest(); }
	catch(e) {}

	// IE6
	try { return new ActiveXObject( 'Msxml2.XMLHTTP' ) ; }
	catch(e) {}

	// IE5
	try { return new ActiveXObject( 'Microsoft.XMLHTTP' ) ; }
	catch(e) {}

	return null ;
}

FCKXml.prototype.LoadUrl = function( urlToCall, asyncFunctionPointer )
{
	var oFCKXml = this ;

	var bAsync = ( typeof(asyncFunctionPointer) == 'function' ) ;

	var oXmlHttp = this.GetHttpRequest() ;

	oXmlHttp.open( "GET", urlToCall, bAsync ) ;

	if ( bAsync )
	{
		oXmlHttp.onreadystatechange = function()
		{
			if ( oXmlHttp.readyState == 4 )
			{
				var oXml ;
				try
				{
					// this is the same test for an FF2 bug as in fckxml_gecko.js
					// but we've moved the responseXML assignment into the try{}
					// so we don't even have to check the return status codes.
					var test = oXmlHttp.responseXML.firstChild ;
					oXml = oXmlHttp.responseXML ;
				}
				catch ( e )
				{
					try
					{
						oXml = (new DOMParser()).parseFromString( oXmlHttp.responseText, 'text/xml' ) ;
					}
					catch ( e ) {}
				}

				if ( !oXml || !oXml.firstChild || oXml.firstChild.nodeName == 'parsererror' )
				{
					alert( 'The server didn\'t send back a proper XML response. Please contact your system administrator.\n\n' +
							'XML request error: ' + oXmlHttp.statusText + ' (' + oXmlHttp.status + ')\n\n' +
							'Requested URL:\n' + urlToCall + '\n\n' +
							'Response text:\n' + oXmlHttp.responseText ) ;
					return ;
				}

				oFCKXml.DOMDocument = oXml ;
				asyncFunctionPointer( oFCKXml ) ;
			}
		}
	}

	oXmlHttp.send( null ) ;

	if ( ! bAsync )
	{
		if ( oXmlHttp.status == 200 || oXmlHttp.status == 304 )
			this.DOMDocument = oXmlHttp.responseXML ;
		else
		{
			alert( 'XML request error: ' + oXmlHttp.statusText + ' (' + oXmlHttp.status + ')' ) ;
		}
	}
}

FCKXml.prototype.SelectNodes = function( xpath )
{
	if ( navigator.userAgent.indexOf('MSIE') >= 0 )		// IE
		return this.DOMDocument.selectNodes( xpath ) ;
	else					// Gecko
	{
		var aNodeArray = new Array();

		var xPathResult = this.DOMDocument.evaluate( xpath, this.DOMDocument,
				this.DOMDocument.createNSResolver(this.DOMDocument.documentElement), XPathResult.ORDERED_NODE_ITERATOR_TYPE, null) ;
		if ( xPathResult )
		{
			var oNode = xPathResult.iterateNext() ;
 			while( oNode )
 			{
 				aNodeArray[aNodeArray.length] = oNode ;
 				oNode = xPathResult.iterateNext();
 			}
		}
		return aNodeArray ;
	}
}

FCKXml.prototype.SelectSingleNode = function( xpath )
{
	if ( navigator.userAgent.indexOf('MSIE') >= 0 )		// IE
		return this.DOMDocument.selectSingleNode( xpath ) ;
	else					// Gecko
	{
		var xPathResult = this.DOMDocument.evaluate( xpath, this.DOMDocument,
				this.DOMDocument.createNSResolver(this.DOMDocument.documentElement), 9, null);

		if ( xPathResult && xPathResult.singleNodeValue )
			return xPathResult.singleNodeValue ;
		else
			return null ;
	}
}
