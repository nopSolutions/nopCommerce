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
 * Utility functions.
 */

var FCKTools = new Object() ;

FCKTools.CreateBogusBR = function( targetDocument )
{
	var eBR = targetDocument.createElement( 'br' ) ;
//	eBR.setAttribute( '_moz_editor_bogus_node', 'TRUE' ) ;
	eBR.setAttribute( 'type', '_moz' ) ;
	return eBR ;
}

/**
 * Fixes relative URL entries defined inside CSS styles by appending a prefix
 * to them.
 * @param (String) cssStyles The CSS styles definition possibly containing url()
 *		paths.
 * @param (String) urlFixPrefix The prefix to append to relative URLs.
 */
FCKTools.FixCssUrls = function( urlFixPrefix, cssStyles )
{
	if ( !urlFixPrefix || urlFixPrefix.length == 0 )
		return cssStyles ;

	return cssStyles.replace( /url\s*\(([\s'"]*)(.*?)([\s"']*)\)/g, function( match, opener, path, closer )
		{
			if ( /^\/|^\w?:/.test( path ) )
				return match ;
			else
				return 'url(' + opener + urlFixPrefix + path + closer + ')' ;
		} ) ;
}

FCKTools._GetUrlFixedCss = function( cssStyles, urlFixPrefix )
{
	var match = cssStyles.match( /^([^|]+)\|([\s\S]*)/ ) ;

	if ( match )
		return FCKTools.FixCssUrls( match[1], match[2] ) ;
	else
		return cssStyles ;
}

/**
 * Appends a <link css> or <style> element to the document.
 * @param (Object) documentElement The DOM document object to which append the
 *		stylesheet.
 * @param (Variant) cssFileOrDef A String pointing to the CSS file URL or an
 *		Array with many CSS file URLs or the CSS definitions for the <style>
 *		element.
 * @return {Array} An array containing all elements created in the target
 *		document. It may include <link> or <style> elements, depending on the
 *		value passed with cssFileOrDef.
 */
FCKTools.AppendStyleSheet = function( domDocument, cssFileOrArrayOrDef )
{
	if ( !cssFileOrArrayOrDef )
		return [] ;

	if ( typeof( cssFileOrArrayOrDef ) == 'string' )
	{
		// Test if the passed argument is an URL.
		if ( /[\\\/\.][^{}]*$/.test( cssFileOrArrayOrDef ) )
		{
			// The string may have several URLs separated by comma.
			return this.AppendStyleSheet( domDocument, cssFileOrArrayOrDef.split(',') ) ;
		}
		else
			return [ this.AppendStyleString( domDocument, FCKTools._GetUrlFixedCss( cssFileOrArrayOrDef ) ) ] ;
	}
	else
	{
		var styles = [] ;
		for ( var i = 0 ; i < cssFileOrArrayOrDef.length ; i++ )
			styles.push( this._AppendStyleSheet( domDocument, cssFileOrArrayOrDef[i] ) ) ;
		return styles ;
	}
}

FCKTools.GetStyleHtml = (function()
{
	var getStyle = function( styleDef, markTemp )
	{
		if ( styleDef.length == 0 )
			return '' ;

		var temp = markTemp ? ' _fcktemp="true"' : '' ;
		return '<' + 'style type="text/css"' + temp + '>' + styleDef + '<' + '/style>' ;
	}

	var getLink = function( cssFileUrl, markTemp )
	{
		if ( cssFileUrl.length == 0 )
			return '' ;

		var temp = markTemp ? ' _fcktemp="true"' : '' ;
		return '<' + 'link href="' + cssFileUrl + '" type="text/css" rel="stylesheet" ' + temp + '/>' ;
	}

	return function( cssFileOrArrayOrDef, markTemp )
	{
		if ( !cssFileOrArrayOrDef )
			return '' ;

		if ( typeof( cssFileOrArrayOrDef ) == 'string' )
		{
			// Test if the passed argument is an URL.
			if ( /[\\\/\.][^{}]*$/.test( cssFileOrArrayOrDef ) )
			{
				// The string may have several URLs separated by comma.
				return this.GetStyleHtml( cssFileOrArrayOrDef.split(','), markTemp ) ;
			}
			else
				return getStyle( this._GetUrlFixedCss( cssFileOrArrayOrDef ), markTemp ) ;
		}
		else
		{
			var html = '' ;

			for ( var i = 0 ; i < cssFileOrArrayOrDef.length ; i++ )
				html += getLink( cssFileOrArrayOrDef[i], markTemp ) ;

			return html ;
		}
	}
})() ;

FCKTools.GetElementDocument = function ( element )
{
	return element.ownerDocument || element.document ;
}

// Get the window object where the element is placed in.
FCKTools.GetElementWindow = function( element )
{
	return this.GetDocumentWindow( this.GetElementDocument( element ) ) ;
}

FCKTools.GetDocumentWindow = function( document )
{
	// With Safari, there is not way to retrieve the window from the document, so we must fix it.
	if ( FCKBrowserInfo.IsSafari && !document.parentWindow )
		this.FixDocumentParentWindow( window.top ) ;

	return document.parentWindow || document.defaultView ;
}

/*
	This is a Safari specific function that fix the reference to the parent
	window from the document object.
*/
FCKTools.FixDocumentParentWindow = function( targetWindow )
{
	if ( targetWindow.document )
		targetWindow.document.parentWindow = targetWindow ;

	for ( var i = 0 ; i < targetWindow.frames.length ; i++ )
		FCKTools.FixDocumentParentWindow( targetWindow.frames[i] ) ;
}

FCKTools.HTMLEncode = function( text )
{
	if ( !text )
		return '' ;

	text = text.replace( /&/g, '&amp;' ) ;
	text = text.replace( /</g, '&lt;' ) ;
	text = text.replace( />/g, '&gt;' ) ;

	return text ;
}

FCKTools.HTMLDecode = function( text )
{
	if ( !text )
		return '' ;

	text = text.replace( /&gt;/g, '>' ) ;
	text = text.replace( /&lt;/g, '<' ) ;
	text = text.replace( /&amp;/g, '&' ) ;

	return text ;
}

FCKTools._ProcessLineBreaksForPMode = function( oEditor, text, liState, node, strArray )
{
	var closeState = 0 ;
	var blockStartTag = "<p>" ;
	var blockEndTag = "</p>" ;
	var lineBreakTag = "<br />" ;
	if ( liState )
	{
		blockStartTag = "<li>" ;
		blockEndTag = "</li>" ;
		closeState = 1 ;
	}

	// Are we currently inside a <p> tag now?
	// If yes, close it at the next double line break.
	while ( node && node != oEditor.FCK.EditorDocument.body )
	{
		if ( node.tagName.toLowerCase() == 'p' )
		{
			closeState = 1 ;
			break;
		}
		node = node.parentNode ;
	}

	for ( var i = 0 ; i < text.length ; i++ )
	{
		var c = text.charAt( i ) ;
		if ( c == '\r' )
			continue ;

		if ( c != '\n' )
		{
			strArray.push( c ) ;
			continue ;
		}

		// Now we have encountered a line break.
		// Check if the next character is also a line break.
		var n = text.charAt( i + 1 ) ;
		if ( n == '\r' )
		{
			i++ ;
			n = text.charAt( i + 1 ) ;
		}
		if ( n == '\n' )
		{
			i++ ;	// ignore next character - we have already processed it.
			if ( closeState )
				strArray.push( blockEndTag ) ;
			strArray.push( blockStartTag ) ;
			closeState = 1 ;
		}
		else
			strArray.push( lineBreakTag ) ;
	}
}

FCKTools._ProcessLineBreaksForDivMode = function( oEditor, text, liState, node, strArray )
{
	var closeState = 0 ;
	var blockStartTag = "<div>" ;
	var blockEndTag = "</div>" ;
	if ( liState )
	{
		blockStartTag = "<li>" ;
		blockEndTag = "</li>" ;
		closeState = 1 ;
	}

	// Are we currently inside a <div> tag now?
	// If yes, close it at the next double line break.
	while ( node && node != oEditor.FCK.EditorDocument.body )
	{
		if ( node.tagName.toLowerCase() == 'div' )
		{
			closeState = 1 ;
			break ;
		}
		node = node.parentNode ;
	}

	for ( var i = 0 ; i < text.length ; i++ )
	{
		var c = text.charAt( i ) ;
		if ( c == '\r' )
			continue ;

		if ( c != '\n' )
		{
			strArray.push( c ) ;
			continue ;
		}

		if ( closeState )
		{
			if ( strArray[ strArray.length - 1 ] == blockStartTag )
			{
				// A div tag must have some contents inside for it to be visible.
				strArray.push( "&nbsp;" ) ;
			}
			strArray.push( blockEndTag ) ;
		}
		strArray.push( blockStartTag ) ;
		closeState = 1 ;
	}
	if ( closeState )
		strArray.push( blockEndTag ) ;
}

FCKTools._ProcessLineBreaksForBrMode = function( oEditor, text, liState, node, strArray )
{
	var closeState = 0 ;
	var blockStartTag = "<br />" ;
	var blockEndTag = "" ;
	if ( liState )
	{
		blockStartTag = "<li>" ;
		blockEndTag = "</li>" ;
		closeState = 1 ;
	}

	for ( var i = 0 ; i < text.length ; i++ )
	{
		var c = text.charAt( i ) ;
		if ( c == '\r' )
			continue ;

		if ( c != '\n' )
		{
			strArray.push( c ) ;
			continue ;
		}

		if ( closeState && blockEndTag.length )
			strArray.push ( blockEndTag ) ;
		strArray.push( blockStartTag ) ;
		closeState = 1 ;
	}
}

FCKTools.ProcessLineBreaks = function( oEditor, oConfig, text )
{
	var enterMode = oConfig.EnterMode.toLowerCase() ;
	var strArray = [] ;

	// Is the caret or selection inside an <li> tag now?
	var liState = 0 ;
	var range = new oEditor.FCKDomRange( oEditor.FCK.EditorWindow ) ;
	range.MoveToSelection() ;
	var node = range._Range.startContainer ;
	while ( node && node.nodeType != 1 )
		node = node.parentNode ;
	if ( node && node.tagName.toLowerCase() == 'li' )
		liState = 1 ;

	if ( enterMode == 'p' )
		this._ProcessLineBreaksForPMode( oEditor, text, liState, node, strArray ) ;
	else if ( enterMode == 'div' )
		this._ProcessLineBreaksForDivMode( oEditor, text, liState, node, strArray ) ;
	else if ( enterMode == 'br' )
		this._ProcessLineBreaksForBrMode( oEditor, text, liState, node, strArray ) ;
	return strArray.join( "" ) ;
}

/**
 * Adds an option to a SELECT element.
 */
FCKTools.AddSelectOption = function( selectElement, optionText, optionValue )
{
	var oOption = FCKTools.GetElementDocument( selectElement ).createElement( "OPTION" ) ;

	oOption.text	= optionText ;
	oOption.value	= optionValue ;

	selectElement.options.add(oOption) ;

	return oOption ;
}

FCKTools.RunFunction = function( func, thisObject, paramsArray, timerWindow )
{
	if ( func )
		this.SetTimeout( func, 0, thisObject, paramsArray, timerWindow ) ;
}

FCKTools.SetTimeout = function( func, milliseconds, thisObject, paramsArray, timerWindow )
{
	return ( timerWindow || window ).setTimeout(
		function()
		{
			if ( paramsArray )
				func.apply( thisObject, [].concat( paramsArray ) ) ;
			else
				func.apply( thisObject ) ;
		},
		milliseconds ) ;
}

FCKTools.SetInterval = function( func, milliseconds, thisObject, paramsArray, timerWindow )
{
	return ( timerWindow || window ).setInterval(
		function()
		{
			func.apply( thisObject, paramsArray || [] ) ;
		},
		milliseconds ) ;
}

FCKTools.ConvertStyleSizeToHtml = function( size )
{
	return size.EndsWith( '%' ) ? size : parseInt( size, 10 ) ;
}

FCKTools.ConvertHtmlSizeToStyle = function( size )
{
	return size.EndsWith( '%' ) ? size : ( size + 'px' ) ;
}

// START iCM MODIFICATIONS
// Amended to accept a list of one or more ascensor tag names
// Amended to check the element itself before working back up through the parent hierarchy
FCKTools.GetElementAscensor = function( element, ascensorTagNames )
{
//	var e = element.parentNode ;
	var e = element ;
	var lstTags = "," + ascensorTagNames.toUpperCase() + "," ;

	while ( e )
	{
		if ( lstTags.indexOf( "," + e.nodeName.toUpperCase() + "," ) != -1 )
			return e ;

		e = e.parentNode ;
	}
	return null ;
}
// END iCM MODIFICATIONS

FCKTools.CreateEventListener = function( func, params )
{
	var f = function()
	{
		var aAllParams = [] ;

		for ( var i = 0 ; i < arguments.length ; i++ )
			aAllParams.push( arguments[i] ) ;

		func.apply( this, aAllParams.concat( params ) ) ;
	}

	return f ;
}

FCKTools.IsStrictMode = function( document )
{
	// There is no compatMode in Safari, but it seams that it always behave as
	// CSS1Compat, so let's assume it as the default for that browser.
	return ( 'CSS1Compat' == ( document.compatMode || ( FCKBrowserInfo.IsSafari ? 'CSS1Compat' : null ) ) ) ;
}

// Transforms a "arguments" object to an array.
FCKTools.ArgumentsToArray = function( args, startIndex, maxLength )
{
	startIndex = startIndex || 0 ;
	maxLength = maxLength || args.length ;

	var argsArray = new Array() ;

	for ( var i = startIndex ; i < startIndex + maxLength && i < args.length ; i++ )
		argsArray.push( args[i] ) ;

	return argsArray ;
}

FCKTools.CloneObject = function( sourceObject )
{
	var fCloneCreator = function() {} ;
	fCloneCreator.prototype = sourceObject ;
	return new fCloneCreator ;
}

// Appends a bogus <br> at the end of the element, if not yet available.
FCKTools.AppendBogusBr = function( element )
{
	if ( !element )
		return ;

	var eLastChild = this.GetLastItem( element.getElementsByTagName('br') ) ;

	if ( !eLastChild || ( eLastChild.getAttribute( 'type', 2 ) != '_moz' && eLastChild.getAttribute( '_moz_dirty' ) == null ) )
	{
		var doc = this.GetElementDocument( element ) ;

		if ( FCKBrowserInfo.IsOpera )
			element.appendChild( doc.createTextNode('') ) ;
		else
			element.appendChild( this.CreateBogusBR( doc ) ) ;
	}
}

FCKTools.GetLastItem = function( list )
{
	if ( list.length > 0 )
		return list[ list.length - 1 ] ;

	return null ;
}

FCKTools.GetDocumentPosition = function( w, node )
{
	var x = 0 ;
	var y = 0 ;
	var curNode = node ;
	var prevNode = null ;
	var curWindow = FCKTools.GetElementWindow( curNode ) ;
	while ( curNode && !( curWindow == w && ( curNode == w.document.body || curNode == w.document.documentElement ) ) )
	{
		x += curNode.offsetLeft - curNode.scrollLeft ;
		y += curNode.offsetTop - curNode.scrollTop ;

		if ( ! FCKBrowserInfo.IsOpera )
		{
			var scrollNode = prevNode ;
			while ( scrollNode && scrollNode != curNode )
			{
				x -= scrollNode.scrollLeft ;
				y -= scrollNode.scrollTop ;
				scrollNode = scrollNode.parentNode ;
			}
		}

		prevNode = curNode ;
		if ( curNode.offsetParent )
			curNode = curNode.offsetParent ;
		else
		{
			if ( curWindow != w )
			{
				curNode = curWindow.frameElement ;
				prevNode = null ;
				if ( curNode )
					curWindow = curNode.contentWindow.parent ;
			}
			else
				curNode = null ;
		}
	}

	// document.body is a special case when it comes to offsetTop and offsetLeft values.
	// 1. It matters if document.body itself is a positioned element;
	// 2. It matters is when we're in IE and the element has no positioned ancestor.
	// Otherwise the values should be ignored.
	if ( FCKDomTools.GetCurrentElementStyle( w.document.body, 'position') != 'static'
			|| ( FCKBrowserInfo.IsIE && FCKDomTools.GetPositionedAncestor( node ) == null ) )
	{
		x += w.document.body.offsetLeft ;
		y += w.document.body.offsetTop ;
	}

	return { "x" : x, "y" : y } ;
}

FCKTools.GetWindowPosition = function( w, node )
{
	var pos = this.GetDocumentPosition( w, node ) ;
	var scroll = FCKTools.GetScrollPosition( w ) ;
	pos.x -= scroll.X ;
	pos.y -= scroll.Y ;
	return pos ;
}

FCKTools.ProtectFormStyles = function( formNode )
{
	if ( !formNode || formNode.nodeType != 1 || formNode.tagName.toLowerCase() != 'form' )
		return [] ;
	var hijackRecord = [] ;
	var hijackNames = [ 'style', 'className' ] ;
	for ( var i = 0 ; i < hijackNames.length ; i++ )
	{
		var name = hijackNames[i] ;
		if ( formNode.elements.namedItem( name ) )
		{
			var hijackNode = formNode.elements.namedItem( name ) ;
			hijackRecord.push( [ hijackNode, hijackNode.nextSibling ] ) ;
			formNode.removeChild( hijackNode ) ;
		}
	}
	return hijackRecord ;
}

FCKTools.RestoreFormStyles = function( formNode, hijackRecord )
{
	if ( !formNode || formNode.nodeType != 1 || formNode.tagName.toLowerCase() != 'form' )
		return ;
	if ( hijackRecord.length > 0 )
	{
		for ( var i = hijackRecord.length - 1 ; i >= 0 ; i-- )
		{
			var node = hijackRecord[i][0] ;
			var sibling = hijackRecord[i][1] ;
			if ( sibling )
				formNode.insertBefore( node, sibling ) ;
			else
				formNode.appendChild( node ) ;
		}
	}
}

// Perform a one-step DFS walk.
FCKTools.GetNextNode = function( node, limitNode )
{
	if ( node.firstChild )
		return node.firstChild ;
	else if ( node.nextSibling )
		return node.nextSibling ;
	else
	{
		var ancestor = node.parentNode ;
		while ( ancestor )
		{
			if ( ancestor == limitNode )
				return null ;
			if ( ancestor.nextSibling )
				return ancestor.nextSibling ;
			else
				ancestor = ancestor.parentNode ;
		}
	}
	return null ;
}

FCKTools.GetNextTextNode = function( textnode, limitNode, checkStop )
{
	node = this.GetNextNode( textnode, limitNode ) ;
	if ( checkStop && node && checkStop( node ) )
		return null ;
	while ( node && node.nodeType != 3 )
	{
		node = this.GetNextNode( node, limitNode ) ;
		if ( checkStop && node && checkStop( node ) )
			return null ;
	}
	return node ;
}

/**
 * Merge all objects passed by argument into a single object.
 */
FCKTools.Merge = function()
{
	var args = arguments ;
	var o = args[0] ;

	for ( var i = 1 ; i < args.length ; i++ )
	{
		var arg = args[i] ;
		for ( var p in arg )
			o[p] = arg[p] ;
	}

	return o ;
}

/**
 * Check if the passed argument is a real Array. It may not working when
 * calling it cross windows.
 */
FCKTools.IsArray = function( it )
{
	return ( it instanceof Array ) ;
}

/**
 * Appends a "length" property to an object, containing the number of
 * properties available on it, excluded the append property itself.
 */
FCKTools.AppendLengthProperty = function( targetObject, propertyName )
{
	var counter = 0 ;

	for ( var n in targetObject )
		counter++ ;

	return targetObject[ propertyName || 'length' ] = counter ;
}

/**
 * Gets the browser parsed version of a css text (style attribute value). On
 * some cases, the browser makes changes to the css text, returning a different
 * value. For example, hexadecimal colors get transformed to rgb().
 */
FCKTools.NormalizeCssText = function( unparsedCssText )
{
	// Injects the style in a temporary span object, so the browser parses it,
	// retrieving its final format.
	var tempSpan = document.createElement( 'span' ) ;
	tempSpan.style.cssText = unparsedCssText ;
	return tempSpan.style.cssText ;
}

/**
 * Binding the "this" reference to an object for a function.
 */
FCKTools.Bind = function( subject, func )
{
  return function(){ return func.apply( subject, arguments ) ; } ;
}

/**
 * Retrieve the correct "empty iframe" URL for the current browser, which
 * causes the minimum fuzz (e.g. security warnings in HTTPS, DNS error in
 * IE5.5, etc.) for that browser, making the iframe ready to DOM use whithout
 * having to loading an external file.
 */
FCKTools.GetVoidUrl = function()
{
	if ( FCK_IS_CUSTOM_DOMAIN )
		return "javascript: void( function(){" +
			"document.open();" +
			"document.write('<html><head><title></title></head><body></body></html>');" +
			"document.domain = '" + FCK_RUNTIME_DOMAIN + "';" +
			"document.close();" +
			"}() ) ;";

	if ( FCKBrowserInfo.IsIE )
	{
		if ( FCKBrowserInfo.IsIE7 || !FCKBrowserInfo.IsIE6 )
			return "" ;					// IE7+ / IE5.5
		else
			return "javascript: '';" ;	// IE6+
	}

	return "javascript: void(0);" ;		// All other browsers.
}

FCKTools.ResetStyles = function( element )
{
	element.style.cssText = 'margin:0;' +
		'padding:0;' +
		'border:0;' +
		'background-color:transparent;' +
		'background-image:none;' ;
}
