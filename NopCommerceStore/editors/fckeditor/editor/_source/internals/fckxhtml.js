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
 */

var FCKXHtml = new Object() ;

FCKXHtml.CurrentJobNum = 0 ;

FCKXHtml.GetXHTML = function( node, includeNode, format )
{
	FCKDomTools.CheckAndRemovePaddingNode( FCKTools.GetElementDocument( node ), FCKConfig.EnterMode ) ;
	FCKXHtmlEntities.Initialize() ;

	// Set the correct entity to use for empty blocks.
	this._NbspEntity = ( FCKConfig.ProcessHTMLEntities? 'nbsp' : '#160' ) ;

	// Save the current IsDirty state. The XHTML processor may change the
	// original HTML, dirtying it.
	var bIsDirty = FCK.IsDirty() ;

	// Special blocks are blocks of content that remain untouched during the
	// process. It is used for SCRIPTs and STYLEs.
	FCKXHtml.SpecialBlocks = new Array() ;

	// Create the XML DOMDocument object.
	this.XML = FCKTools.CreateXmlObject( 'DOMDocument' ) ;

	// Add a root element that holds all child nodes.
	this.MainNode = this.XML.appendChild( this.XML.createElement( 'xhtml' ) ) ;

	FCKXHtml.CurrentJobNum++ ;

//	var dTimer = new Date() ;

	if ( includeNode )
		this._AppendNode( this.MainNode, node ) ;
	else
		this._AppendChildNodes( this.MainNode, node, false ) ;

	/**
	 * FCKXHtml._AppendNode() marks DOM element objects it has
	 * processed by adding a property called _fckxhtmljob,
	 * setting it equal to the value of FCKXHtml.CurrentJobNum.
	 * On Internet Explorer, if an element object has such a
	 * property,  it will show up in the object's attributes
	 * NamedNodeMap, and the corresponding Attr object in
	 * that collection  will have is specified property set
	 * to true.  This trips up code elsewhere that checks to
	 * see if an element is free of attributes before proceeding
	 * with an edit operation (c.f. FCK.Style.RemoveFromRange())
	 *
	 * refs #2156 and #2834
	 */
	if ( FCKBrowserInfo.IsIE )
		FCKXHtml._RemoveXHtmlJobProperties( node ) ;

	// Get the resulting XHTML as a string.
	var sXHTML = this._GetMainXmlString() ;

//	alert( 'Time: ' + ( ( ( new Date() ) - dTimer ) ) + ' ms' ) ;

	this.XML = null ;

	// Safari adds xmlns="http://www.w3.org/1999/xhtml" to the root node (#963)
	if ( FCKBrowserInfo.IsSafari )
		sXHTML = sXHTML.replace( /^<xhtml.*?>/, '<xhtml>' ) ;

	// Strip the "XHTML" root node.
	sXHTML = sXHTML.substr( 7, sXHTML.length - 15 ).Trim() ;

	// According to the doctype set the proper end for self-closing tags
	// HTML: <br>
	// XHTML: Add a space, like <br/> -> <br />
	if (FCKConfig.DocType.length > 0 && FCKRegexLib.HtmlDocType.test( FCKConfig.DocType ) )
		sXHTML = sXHTML.replace( FCKRegexLib.SpaceNoClose, '>');
	else
		sXHTML = sXHTML.replace( FCKRegexLib.SpaceNoClose, ' />');

	if ( FCKConfig.ForceSimpleAmpersand )
		sXHTML = sXHTML.replace( FCKRegexLib.ForceSimpleAmpersand, '&' ) ;

	if ( format )
		sXHTML = FCKCodeFormatter.Format( sXHTML ) ;

	// Now we put back the SpecialBlocks contents.
	for ( var i = 0 ; i < FCKXHtml.SpecialBlocks.length ; i++ )
	{
		var oRegex = new RegExp( '___FCKsi___' + i ) ;
		sXHTML = sXHTML.replace( oRegex, FCKXHtml.SpecialBlocks[i] ) ;
	}

	// Replace entities marker with the ampersand.
	sXHTML = sXHTML.replace( FCKRegexLib.GeckoEntitiesMarker, '&' ) ;

	// Restore the IsDirty state if it was not dirty.
	if ( !bIsDirty )
		FCK.ResetIsDirty() ;

	FCKDomTools.EnforcePaddingNode( FCKTools.GetElementDocument( node ), FCKConfig.EnterMode ) ;
	return sXHTML ;
}

FCKXHtml._AppendAttribute = function( xmlNode, attributeName, attributeValue )
{
	try
	{
		if ( attributeValue == undefined || attributeValue == null )
			attributeValue = '' ;
		else if ( attributeValue.replace )
		{
			if ( FCKConfig.ForceSimpleAmpersand )
				attributeValue = attributeValue.replace( /&/g, '___FCKAmp___' ) ;

			// Entities must be replaced in the attribute values.
			attributeValue = attributeValue.replace( FCKXHtmlEntities.EntitiesRegex, FCKXHtml_GetEntity ) ;
		}

		// Create the attribute.
		var oXmlAtt = this.XML.createAttribute( attributeName ) ;
		oXmlAtt.value = attributeValue ;

		// Set the attribute in the node.
		xmlNode.attributes.setNamedItem( oXmlAtt ) ;
	}
	catch (e)
	{}
}

FCKXHtml._AppendChildNodes = function( xmlNode, htmlNode, isBlockElement )
{
	var oNode = htmlNode.firstChild ;

	while ( oNode )
	{
		this._AppendNode( xmlNode, oNode ) ;
		oNode = oNode.nextSibling ;
	}

	// Trim block elements. This is also needed to avoid Firefox leaving extra
	// BRs at the end of them.
	if ( isBlockElement && htmlNode.tagName && htmlNode.tagName.toLowerCase() != 'pre' )
	{
		FCKDomTools.TrimNode( xmlNode ) ;

		if ( FCKConfig.FillEmptyBlocks )
		{
			var lastChild = xmlNode.lastChild ;
			if ( lastChild && lastChild.nodeType == 1 && lastChild.nodeName == 'br' )
				this._AppendEntity( xmlNode, this._NbspEntity ) ;
		}
	}

	// If the resulting node is empty.
	if ( xmlNode.childNodes.length == 0 )
	{
		if ( isBlockElement && FCKConfig.FillEmptyBlocks )
		{
			this._AppendEntity( xmlNode, this._NbspEntity ) ;
			return xmlNode ;
		}

		var sNodeName = xmlNode.nodeName ;

		// Some inline elements are required to have something inside (span, strong, etc...).
		if ( FCKListsLib.InlineChildReqElements[ sNodeName ] )
			return null ;

		// We can't use short representation of empty elements that are not marked
		// as empty in th XHTML DTD.
		if ( !FCKListsLib.EmptyElements[ sNodeName ] )
			xmlNode.appendChild( this.XML.createTextNode('') ) ;
	}

	return xmlNode ;
}

FCKXHtml._AppendNode = function( xmlNode, htmlNode )
{
	if ( !htmlNode )
		return false ;

	switch ( htmlNode.nodeType )
	{
		// Element Node.
		case 1 :
			// If we detect a <br> inside a <pre> in Gecko, turn it into a line break instead.
			// This is a workaround for the Gecko bug here: https://bugzilla.mozilla.org/show_bug.cgi?id=92921
			if ( FCKBrowserInfo.IsGecko
					&& htmlNode.tagName.toLowerCase() == 'br'
					&& htmlNode.parentNode.tagName.toLowerCase() == 'pre' )
			{
				var val = '\r' ;
				if ( htmlNode == htmlNode.parentNode.firstChild )
					val += '\r' ;
				return FCKXHtml._AppendNode( xmlNode, this.XML.createTextNode( val ) ) ;
			}

			// Here we found an element that is not the real element, but a
			// fake one (like the Flash placeholder image), so we must get the real one.
			if ( htmlNode.getAttribute('_fckfakelement') )
				return FCKXHtml._AppendNode( xmlNode, FCK.GetRealElement( htmlNode ) ) ;

			// Ignore bogus BR nodes in the DOM.
			if ( FCKBrowserInfo.IsGecko &&
					( htmlNode.hasAttribute('_moz_editor_bogus_node') || htmlNode.getAttribute( 'type' ) == '_moz' ) )
			{
				if ( htmlNode.nextSibling )
					return false ;
				else
				{
					htmlNode.removeAttribute( '_moz_editor_bogus_node' ) ;
					htmlNode.removeAttribute( 'type' ) ;
				}
			}

			// This is for elements that are instrumental to FCKeditor and
			// must be removed from the final HTML.
			if ( htmlNode.getAttribute('_fcktemp') )
				return false ;

			// Get the element name.
			var sNodeName = htmlNode.tagName.toLowerCase()  ;

			if ( FCKBrowserInfo.IsIE )
			{
				// IE doens't include the scope name in the nodeName. So, add the namespace.
				if ( htmlNode.scopeName && htmlNode.scopeName != 'HTML' && htmlNode.scopeName != 'FCK' )
					sNodeName = htmlNode.scopeName.toLowerCase() + ':' + sNodeName ;
			}
			else
			{
				if ( sNodeName.StartsWith( 'fck:' ) )
					sNodeName = sNodeName.Remove( 0,4 ) ;
			}

			// Check if the node name is valid, otherwise ignore this tag.
			// If the nodeName starts with a slash, it is a orphan closing tag.
			// On some strange cases, the nodeName is empty, even if the node exists.
			if ( !FCKRegexLib.ElementName.test( sNodeName ) )
				return false ;

			// The already processed nodes must be marked to avoid then to be duplicated (bad formatted HTML).
			// So here, the "mark" is checked... if the element is Ok, then mark it.
			if ( htmlNode._fckxhtmljob && htmlNode._fckxhtmljob == FCKXHtml.CurrentJobNum )
				return false ;

			var oNode = this.XML.createElement( sNodeName ) ;

			// Add all attributes.
			FCKXHtml._AppendAttributes( xmlNode, htmlNode, oNode, sNodeName ) ;

			htmlNode._fckxhtmljob = FCKXHtml.CurrentJobNum ;

			// Tag specific processing.
			var oTagProcessor = FCKXHtml.TagProcessors[ sNodeName ] ;

			if ( oTagProcessor )
				oNode = oTagProcessor( oNode, htmlNode, xmlNode ) ;
			else
				oNode = this._AppendChildNodes( oNode, htmlNode, Boolean( FCKListsLib.NonEmptyBlockElements[ sNodeName ] ) ) ;

			if ( !oNode )
				return false ;

			xmlNode.appendChild( oNode ) ;

			break ;

		// Text Node.
		case 3 :
			if ( htmlNode.parentNode && htmlNode.parentNode.nodeName.IEquals( 'pre' ) )
				return this._AppendTextNode( xmlNode, htmlNode.nodeValue ) ;
			return this._AppendTextNode( xmlNode, htmlNode.nodeValue.ReplaceNewLineChars(' ') ) ;

		// Comment
		case 8 :
			// IE catches the <!DOTYPE ... > as a comment, but it has no
			// innerHTML, so we can catch it, and ignore it.
			if ( FCKBrowserInfo.IsIE && !htmlNode.innerHTML )
				break ;

			try { xmlNode.appendChild( this.XML.createComment( htmlNode.nodeValue ) ) ; }
			catch (e) { /* Do nothing... probably this is a wrong format comment. */ }
			break ;

		// Unknown Node type.
		default :
			xmlNode.appendChild( this.XML.createComment( "Element not supported - Type: " + htmlNode.nodeType + " Name: " + htmlNode.nodeName ) ) ;
			break ;
	}
	return true ;
}

// Append an item to the SpecialBlocks array and returns the tag to be used.
FCKXHtml._AppendSpecialItem = function( item )
{
	return '___FCKsi___' + ( FCKXHtml.SpecialBlocks.push( item ) - 1 ) ;
}

FCKXHtml._AppendEntity = function( xmlNode, entity )
{
	xmlNode.appendChild( this.XML.createTextNode( '#?-:' + entity + ';' ) ) ;
}

FCKXHtml._AppendTextNode = function( targetNode, textValue )
{
	var bHadText = textValue.length > 0 ;
	if ( bHadText )
		targetNode.appendChild( this.XML.createTextNode( textValue.replace( FCKXHtmlEntities.EntitiesRegex, FCKXHtml_GetEntity ) ) ) ;
	return bHadText ;
}

// Retrieves a entity (internal format) for a given character.
function FCKXHtml_GetEntity( character )
{
	// We cannot simply place the entities in the text, because the XML parser
	// will translate & to &amp;. So we use a temporary marker which is replaced
	// in the end of the processing.
	var sEntity = FCKXHtmlEntities.Entities[ character ] || ( '#' + character.charCodeAt(0) ) ;
	return '#?-:' + sEntity + ';' ;
}

// An object that hold tag specific operations.
FCKXHtml.TagProcessors =
{
	a : function( node, htmlNode )
	{
		// Firefox may create empty tags when deleting the selection in some special cases (SF-BUG 1556878).
		if ( htmlNode.innerHTML.Trim().length == 0 && !htmlNode.name )
			return false ;

		var sSavedUrl = htmlNode.getAttribute( '_fcksavedurl' ) ;
		if ( sSavedUrl != null )
			FCKXHtml._AppendAttribute( node, 'href', sSavedUrl ) ;


		// Anchors with content has been marked with an additional class, now we must remove it.
		if ( FCKBrowserInfo.IsIE )
		{
			// Buggy IE, doesn't copy the name of changed anchors.
			if ( htmlNode.name )
				FCKXHtml._AppendAttribute( node, 'name', htmlNode.name ) ;
		}

		node = FCKXHtml._AppendChildNodes( node, htmlNode, false ) ;

		return node ;
	},

	area : function( node, htmlNode )
	{
		var sSavedUrl = htmlNode.getAttribute( '_fcksavedurl' ) ;
		if ( sSavedUrl != null )
			FCKXHtml._AppendAttribute( node, 'href', sSavedUrl ) ;

		// IE ignores the "COORDS" and "SHAPE" attribute so we must add it manually.
		if ( FCKBrowserInfo.IsIE )
		{
			if ( ! node.attributes.getNamedItem( 'coords' ) )
			{
				var sCoords = htmlNode.getAttribute( 'coords', 2 ) ;
				if ( sCoords && sCoords != '0,0,0' )
					FCKXHtml._AppendAttribute( node, 'coords', sCoords ) ;
			}

			if ( ! node.attributes.getNamedItem( 'shape' ) )
			{
				var sShape = htmlNode.getAttribute( 'shape', 2 ) ;
				if ( sShape && sShape.length > 0 )
					FCKXHtml._AppendAttribute( node, 'shape', sShape.toLowerCase() ) ;
			}
		}

		return node ;
	},

	body : function( node, htmlNode )
	{
		node = FCKXHtml._AppendChildNodes( node, htmlNode, false ) ;
		// Remove spellchecker attributes added for Firefox when converting to HTML code (Bug #1351).
		node.removeAttribute( 'spellcheck' ) ;
		return node ;
	},

	// IE loses contents of iframes, and Gecko does give it back HtmlEncoded
	// Note: Opera does lose the content and doesn't provide it in the innerHTML string
	iframe : function( node, htmlNode )
	{
		var sHtml = htmlNode.innerHTML ;

		// Gecko does give back the encoded html
		if ( FCKBrowserInfo.IsGecko )
			sHtml = FCKTools.HTMLDecode( sHtml );

		// Remove the saved urls here as the data won't be processed as nodes
		sHtml = sHtml.replace( /\s_fcksavedurl="[^"]*"/g, '' ) ;

		node.appendChild( FCKXHtml.XML.createTextNode( FCKXHtml._AppendSpecialItem( sHtml ) ) ) ;

		return node ;
	},

	img : function( node, htmlNode )
	{
		// The "ALT" attribute is required in XHTML.
		if ( ! node.attributes.getNamedItem( 'alt' ) )
			FCKXHtml._AppendAttribute( node, 'alt', '' ) ;

		var sSavedUrl = htmlNode.getAttribute( '_fcksavedurl' ) ;
		if ( sSavedUrl != null )
			FCKXHtml._AppendAttribute( node, 'src', sSavedUrl ) ;

		// Bug #768 : If the width and height are defined inline CSS,
		// don't define it again in the HTML attributes.
		if ( htmlNode.style.width )
			node.removeAttribute( 'width' ) ;
		if ( htmlNode.style.height )
			node.removeAttribute( 'height' ) ;

		return node ;
	},

	// Fix orphaned <li> nodes (Bug #503).
	li : function( node, htmlNode, targetNode )
	{
		// If the XML parent node is already a <ul> or <ol>, then add the <li> as usual.
		if ( targetNode.nodeName.IEquals( ['ul', 'ol'] ) )
			return FCKXHtml._AppendChildNodes( node, htmlNode, true ) ;

		var newTarget = FCKXHtml.XML.createElement( 'ul' ) ;

		// Reset the _fckxhtmljob so the HTML node is processed again.
		htmlNode._fckxhtmljob = null ;

		// Loop through all sibling LIs, adding them to the <ul>.
		do
		{
			FCKXHtml._AppendNode( newTarget, htmlNode ) ;

			// Look for the next element following this <li>.
			do
			{
				htmlNode = FCKDomTools.GetNextSibling( htmlNode ) ;

			} while ( htmlNode && htmlNode.nodeType == 3 && htmlNode.nodeValue.Trim().length == 0 )

		}	while ( htmlNode && htmlNode.nodeName.toLowerCase() == 'li' )

		return newTarget ;
	},

	// Fix nested <ul> and <ol>.
	ol : function( node, htmlNode, targetNode )
	{
		if ( htmlNode.innerHTML.Trim().length == 0 )
			return false ;

		var ePSibling = targetNode.lastChild ;

		if ( ePSibling && ePSibling.nodeType == 3 )
			ePSibling = ePSibling.previousSibling ;

		if ( ePSibling && ePSibling.nodeName.toUpperCase() == 'LI' )
		{
			htmlNode._fckxhtmljob = null ;
			FCKXHtml._AppendNode( ePSibling, htmlNode ) ;
			return false ;
		}

		node = FCKXHtml._AppendChildNodes( node, htmlNode ) ;

		return node ;
	},

	pre : function ( node, htmlNode )
	{
		var firstChild = htmlNode.firstChild ;

		if ( firstChild && firstChild.nodeType == 3 )
			node.appendChild( FCKXHtml.XML.createTextNode( FCKXHtml._AppendSpecialItem( '\r\n' ) ) ) ;

		FCKXHtml._AppendChildNodes( node, htmlNode, true ) ;

		return node ;
	},

	script : function( node, htmlNode )
	{
		// The "TYPE" attribute is required in XHTML.
		if ( ! node.attributes.getNamedItem( 'type' ) )
			FCKXHtml._AppendAttribute( node, 'type', 'text/javascript' ) ;

		node.appendChild( FCKXHtml.XML.createTextNode( FCKXHtml._AppendSpecialItem( htmlNode.text ) ) ) ;

		return node ;
	},

	span : function( node, htmlNode )
	{
		// Firefox may create empty tags when deleting the selection in some special cases (SF-BUG 1084404).
		if ( htmlNode.innerHTML.length == 0 )
			return false ;

		node = FCKXHtml._AppendChildNodes( node, htmlNode, false ) ;

		return node ;
	},

	style : function( node, htmlNode )
	{
		// The "TYPE" attribute is required in XHTML.
		if ( ! node.attributes.getNamedItem( 'type' ) )
			FCKXHtml._AppendAttribute( node, 'type', 'text/css' ) ;

		var cssText = htmlNode.innerHTML ;
		if ( FCKBrowserInfo.IsIE )	// Bug #403 : IE always appends a \r\n to the beginning of StyleNode.innerHTML
			cssText = cssText.replace( /^(\r\n|\n|\r)/, '' ) ;

		node.appendChild( FCKXHtml.XML.createTextNode( FCKXHtml._AppendSpecialItem( cssText ) ) ) ;

		return node ;
	},

	title : function( node, htmlNode )
	{
		node.appendChild( FCKXHtml.XML.createTextNode( FCK.EditorDocument.title ) ) ;

		return node ;
	}
} ;

FCKXHtml.TagProcessors.ul = FCKXHtml.TagProcessors.ol ;
