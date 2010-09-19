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
 * Utility functions. (Gecko version).
 */

FCKTools.CancelEvent = function( e )
{
	if ( e )
		e.preventDefault() ;
}

FCKTools.DisableSelection = function( element )
{
	if ( FCKBrowserInfo.IsGecko )
		element.style.MozUserSelect		= 'none' ;	// Gecko only.
	else if ( FCKBrowserInfo.IsSafari )
		element.style.KhtmlUserSelect	= 'none' ;	// WebKit only.
	else
		element.style.userSelect		= 'none' ;	// CSS3 (not supported yet).
}

// Appends a CSS file to a document.
FCKTools._AppendStyleSheet = function( documentElement, cssFileUrl )
{
	var e = documentElement.createElement( 'LINK' ) ;
	e.rel	= 'stylesheet' ;
	e.type	= 'text/css' ;
	e.href	= cssFileUrl ;
	documentElement.getElementsByTagName("HEAD")[0].appendChild( e ) ;
	return e ;
}

// Appends a CSS style string to a document.
FCKTools.AppendStyleString = function( documentElement, cssStyles )
{
	if ( !cssStyles )
		return null ;

	var e = documentElement.createElement( "STYLE" ) ;
	e.appendChild( documentElement.createTextNode( cssStyles ) ) ;
	documentElement.getElementsByTagName( "HEAD" )[0].appendChild( e ) ;
	return e ;
}

// Removes all attributes and values from the element.
FCKTools.ClearElementAttributes = function( element )
{
	// Loop throw all attributes in the element
	for ( var i = 0 ; i < element.attributes.length ; i++ )
	{
		// Remove the element by name.
		element.removeAttribute( element.attributes[i].name, 0 ) ;	// 0 : Case Insensitive
	}
}

// Returns an Array of strings with all defined in the elements inside another element.
FCKTools.GetAllChildrenIds = function( parentElement )
{
	// Create the array that will hold all Ids.
	var aIds = new Array() ;

	// Define a recursive function that search for the Ids.
	var fGetIds = function( parent )
	{
		for ( var i = 0 ; i < parent.childNodes.length ; i++ )
		{
			var sId = parent.childNodes[i].id ;

			// Check if the Id is defined for the element.
			if ( sId && sId.length > 0 ) aIds[ aIds.length ] = sId ;

			// Recursive call.
			fGetIds( parent.childNodes[i] ) ;
		}
	}

	// Start the recursive calls.
	fGetIds( parentElement ) ;

	return aIds ;
}

// Replaces a tag with its contents. For example "<span>My <b>tag</b></span>"
// will be replaced with "My <b>tag</b>".
FCKTools.RemoveOuterTags = function( e )
{
	var oFragment = e.ownerDocument.createDocumentFragment() ;

	for ( var i = 0 ; i < e.childNodes.length ; i++ )
		oFragment.appendChild( e.childNodes[i].cloneNode(true) ) ;

	e.parentNode.replaceChild( oFragment, e ) ;
}

FCKTools.CreateXmlObject = function( object )
{
	switch ( object )
	{
		case 'XmlHttp' :
			return new XMLHttpRequest() ;

		case 'DOMDocument' :
			// Originaly, we were had the following here:
			// return document.implementation.createDocument( '', '', null ) ;
			// But that doesn't work if we're running under domain relaxation mode, so we need a workaround.
			// See http://ajaxian.com/archives/xml-messages-with-cross-domain-json about the trick we're using.
			var doc = ( new DOMParser() ).parseFromString( '<tmp></tmp>', 'text/xml' ) ;
			FCKDomTools.RemoveNode( doc.firstChild ) ;
			return doc ;
	}
	return null ;
}

FCKTools.GetScrollPosition = function( relativeWindow )
{
	return { X : relativeWindow.pageXOffset, Y : relativeWindow.pageYOffset } ;
}

FCKTools.AddEventListener = function( sourceObject, eventName, listener )
{
	sourceObject.addEventListener( eventName, listener, false ) ;
}

FCKTools.RemoveEventListener = function( sourceObject, eventName, listener )
{
	sourceObject.removeEventListener( eventName, listener, false ) ;
}

// Listeners attached with this function cannot be detached.
FCKTools.AddEventListenerEx = function( sourceObject, eventName, listener, paramsArray )
{
	sourceObject.addEventListener(
		eventName,
		function( e )
		{
			listener.apply( sourceObject, [ e ].concat( paramsArray || [] ) ) ;
		},
		false
	) ;
}

// Returns and object with the "Width" and "Height" properties.
FCKTools.GetViewPaneSize = function( win )
{
	return { Width : win.innerWidth, Height : win.innerHeight } ;
}

FCKTools.SaveStyles = function( element )
{
	var data = FCKTools.ProtectFormStyles( element ) ;

	var oSavedStyles = new Object() ;

	if ( element.className.length > 0 )
	{
		oSavedStyles.Class = element.className ;
		element.className = '' ;
	}

	var sInlineStyle = element.getAttribute( 'style' ) ;

	if ( sInlineStyle && sInlineStyle.length > 0 )
	{
		oSavedStyles.Inline = sInlineStyle ;
		element.setAttribute( 'style', '', 0 ) ;	// 0 : Case Insensitive
	}

	FCKTools.RestoreFormStyles( element, data ) ;
	return oSavedStyles ;
}

FCKTools.RestoreStyles = function( element, savedStyles )
{
	var data = FCKTools.ProtectFormStyles( element ) ;
	element.className = savedStyles.Class || '' ;

	if ( savedStyles.Inline )
		element.setAttribute( 'style', savedStyles.Inline, 0 ) ;	// 0 : Case Insensitive
	else
		element.removeAttribute( 'style', 0 ) ;
	FCKTools.RestoreFormStyles( element, data ) ;
}

FCKTools.RegisterDollarFunction = function( targetWindow )
{
	targetWindow.$ = function( id )
	{
		return targetWindow.document.getElementById( id ) ;
	} ;
}

FCKTools.AppendElement = function( target, elementName )
{
	return target.appendChild( target.ownerDocument.createElement( elementName ) ) ;
}

// Get the coordinates of an element.
//		@el : The element to get the position.
//		@relativeWindow: The window to which we want the coordinates relative to.
FCKTools.GetElementPosition = function( el, relativeWindow )
{
	// Initializes the Coordinates object that will be returned by the function.
	var c = { X:0, Y:0 } ;

	var oWindow = relativeWindow || window ;

	var oOwnerWindow = FCKTools.GetElementWindow( el ) ;

	var previousElement = null ;
	// Loop throw the offset chain.
	while ( el )
	{
		var sPosition = oOwnerWindow.getComputedStyle(el, '').position ;

		// Check for non "static" elements.
		// 'FCKConfig.FloatingPanelsZIndex' -- Submenus are under a positioned IFRAME.
		if ( sPosition && sPosition != 'static' && el.style.zIndex != FCKConfig.FloatingPanelsZIndex )
			break ;

		/*
		FCKDebug.Output( el.tagName + ":" + "offset=" + el.offsetLeft + "," + el.offsetTop + "  "
				+ "scroll=" + el.scrollLeft + "," + el.scrollTop ) ;
		*/

		c.X += el.offsetLeft - el.scrollLeft ;
		c.Y += el.offsetTop - el.scrollTop  ;

		// Backtrack due to offsetParent's calculation by the browser ignores scrollLeft and scrollTop.
		// Backtracking is not needed for Opera
		if ( ! FCKBrowserInfo.IsOpera )
		{
			var scrollElement = previousElement ;
			while ( scrollElement && scrollElement != el )
			{
				c.X -= scrollElement.scrollLeft ;
				c.Y -= scrollElement.scrollTop ;
				scrollElement = scrollElement.parentNode ;
			}
		}

		previousElement = el ;
		if ( el.offsetParent )
			el = el.offsetParent ;
		else
		{
			if ( oOwnerWindow != oWindow )
			{
				el = oOwnerWindow.frameElement ;
				previousElement = null ;
				if ( el )
					oOwnerWindow = FCKTools.GetElementWindow( el ) ;
			}
			else
			{
				c.X += el.scrollLeft ;
				c.Y += el.scrollTop  ;
				break ;
			}
		}
	}

	// Return the Coordinates object
	return c ;
}
