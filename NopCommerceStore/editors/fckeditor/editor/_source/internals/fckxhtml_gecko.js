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
 * Defines the FCKXHtml object, responsible for the XHTML operations.
 * Gecko specific.
 */

FCKXHtml._GetMainXmlString = function()
{
	return ( new XMLSerializer() ).serializeToString( this.MainNode ) ;
}

FCKXHtml._AppendAttributes = function( xmlNode, htmlNode, node )
{
	var aAttributes = htmlNode.attributes ;

	for ( var n = 0 ; n < aAttributes.length ; n++ )
	{
		var oAttribute = aAttributes[n] ;

		if ( oAttribute.specified )
		{
			var sAttName = oAttribute.nodeName.toLowerCase() ;
			var sAttValue ;

			// Ignore any attribute starting with "_fck".
			if ( sAttName.StartsWith( '_fck' ) )
				continue ;
			// There is a bug in Mozilla that returns '_moz_xxx' attributes as specified.
			else if ( sAttName.indexOf( '_moz' ) == 0 )
				continue ;
			// There are one cases (on Gecko) when the oAttribute.nodeValue must be used:
			//		- for the "class" attribute
			else if ( sAttName == 'class' )
			{
				sAttValue = oAttribute.nodeValue.replace( FCKRegexLib.FCK_Class, '' ) ;
				if ( sAttValue.length == 0 )
					continue ;
			}
			// XHTML doens't support attribute minimization like "CHECKED". It must be transformed to checked="checked".
			else if ( oAttribute.nodeValue === true )
				sAttValue = sAttName ;
			else
				sAttValue = htmlNode.getAttribute( sAttName, 2 ) ;	// We must use getAttribute to get it exactly as it is defined.

			this._AppendAttribute( node, sAttName, sAttValue ) ;
		}
	}
}

if ( FCKBrowserInfo.IsOpera )
{
	// Opera moves the <FCK:meta> element outside head (#1166).

	// Save a reference to the XML <head> node, so we can use it for
	// orphan <meta>s.
	FCKXHtml.TagProcessors['head'] = function( node, htmlNode )
	{
		FCKXHtml.XML._HeadElement = node ;

		node = FCKXHtml._AppendChildNodes( node, htmlNode, true ) ;

		return node ;
	}

	// Check whether a <meta> element is outside <head>, and move it to the
	// proper place.
	FCKXHtml.TagProcessors['meta'] = function( node, htmlNode, xmlNode )
	{
		if ( htmlNode.parentNode.nodeName.toLowerCase() != 'head' )
		{
			var headElement = FCKXHtml.XML._HeadElement ;

			if ( headElement && xmlNode != headElement )
			{
				delete htmlNode._fckxhtmljob ;
				FCKXHtml._AppendNode( headElement, htmlNode ) ;
				return null ;
			}
		}

		return node ;
	}
}

if ( FCKBrowserInfo.IsGecko )
{
	// #2162, some Firefox extensions might add references to internal links
	FCKXHtml.TagProcessors['link'] = function( node, htmlNode )
	{
		if ( htmlNode.href.substr(0, 9).toLowerCase() == 'chrome://' )
			return false ;

		return node ;
	}

}
