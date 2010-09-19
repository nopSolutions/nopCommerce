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
 * FCKStyle Class: contains a style definition, and all methods to work with
 * the style in a document.
 */

/**
 * @param {Object} styleDesc A "style descriptor" object, containing the raw
 * style definition in the following format:
 *		'<style name>' : {
 *			Element : '<element name>',
 *			Attributes : {
 *				'<att name>' : '<att value>',
 *				...
 *			},
 *			Styles : {
 *				'<style name>' : '<style value>',
 *				...
 *			},
 *			Overrides : '<element name>'|{
 *				Element : '<element name>',
 *				Attributes : {
 *					'<att name>' : '<att value>'|/<att regex>/
 *				},
 *				Styles : {
 *					'<style name>' : '<style value>'|/<style regex>/
 *				},
 *			}
 *		}
 */
var FCKStyle = function( styleDesc )
{
	this.Element = ( styleDesc.Element || 'span' ).toLowerCase() ;
	this._StyleDesc = styleDesc ;
}

FCKStyle.prototype =
{
	/**
	 * Get the style type, based on its element name:
	 *		- FCK_STYLE_BLOCK  (0): Block Style
	 *		- FCK_STYLE_INLINE (1): Inline Style
	 *		- FCK_STYLE_OBJECT (2): Object Style
	 */
	GetType : function()
	{
		var type = this.GetType_$ ;

		if ( type != undefined )
			return type ;

		var elementName = this.Element ;

		if ( elementName == '#' || FCKListsLib.StyleBlockElements[ elementName ] )
			type = FCK_STYLE_BLOCK ;
		else if ( FCKListsLib.StyleObjectElements[ elementName ] )
			type = FCK_STYLE_OBJECT ;
		else
			type = FCK_STYLE_INLINE ;

		return ( this.GetType_$ = type ) ;
	},

	/**
	 * Apply the style to the current selection.
	 */
	ApplyToSelection : function( targetWindow )
	{
		// Create a range for the current selection.
		var range = new FCKDomRange( targetWindow ) ;
		range.MoveToSelection() ;

		this.ApplyToRange( range, true ) ;
	},

	/**
	 * Apply the style to a FCKDomRange.
	 */
	ApplyToRange : function( range, selectIt, updateRange )
	{
		// ApplyToRange is not valid for FCK_STYLE_OBJECT types.
		// Use ApplyToObject instead.

		switch ( this.GetType() )
		{
			case FCK_STYLE_BLOCK :
				this.ApplyToRange = this._ApplyBlockStyle ;
				break ;
			case FCK_STYLE_INLINE :
				this.ApplyToRange = this._ApplyInlineStyle ;
				break ;
			default :
				return ;
		}

		this.ApplyToRange( range, selectIt, updateRange ) ;
	},

	/**
	 * Apply the style to an object. Valid for FCK_STYLE_BLOCK types only.
	 */
	ApplyToObject : function( objectElement )
	{
		if ( !objectElement )
			return ;

		this.BuildElement( null, objectElement ) ;
	},

	/**
	 * Remove the style from the current selection.
	 */
	RemoveFromSelection : function( targetWindow )
	{
		// Create a range for the current selection.
		var range = new FCKDomRange( targetWindow ) ;
		range.MoveToSelection() ;

		this.RemoveFromRange( range, true ) ;
	},

	/**
	 * Remove the style from a FCKDomRange. Block type styles will have no
	 * effect.
	 */
	RemoveFromRange : function( range, selectIt, updateRange )
	{
		var bookmark ;

		// Create the attribute list to be used later for element comparisons.
		var styleAttribs = this._GetAttribsForComparison() ;
		var styleOverrides = this._GetOverridesForComparison() ;

		// If collapsed, we are removing all conflicting styles from the range
		// parent tree.
		if ( range.CheckIsCollapsed() )
		{
			// Bookmark the range so we can re-select it after processing.
			var bookmark = range.CreateBookmark( true ) ;

			// Let's start from the bookmark <span> parent.
			var bookmarkStart = range.GetBookmarkNode( bookmark, true ) ;

			var path = new FCKElementPath( bookmarkStart.parentNode ) ;

			// While looping through the path, we'll be saving references to
			// parent elements if the range is in one of their boundaries. In
			// this way, we are able to create a copy of those elements when
			// removing a style if the range is in a boundary limit (see #1270).
			var boundaryElements = [] ;

			// Check if the range is in the boundary limits of an element
			// (related to #1270).
			var isBoundaryRight = !FCKDomTools.GetNextSibling( bookmarkStart ) ;
			var isBoundary = isBoundaryRight || !FCKDomTools.GetPreviousSibling( bookmarkStart ) ;

			// This is the last element to be removed in the boundary situation
			// described at #1270.
			var lastBoundaryElement ;
			var boundaryLimitIndex = -1 ;

			for ( var i = 0 ; i < path.Elements.length ; i++ )
			{
				var pathElement = path.Elements[i] ;
				if ( this.CheckElementRemovable( pathElement ) )
				{
					if ( isBoundary
						&& !FCKDomTools.CheckIsEmptyElement( pathElement,
								function( el )
								{
									return ( el != bookmarkStart ) ;
								} )
						)
					{
						lastBoundaryElement = pathElement ;

						// We'll be continuously including elements in the
						// boundaryElements array, but only those added before
						// setting lastBoundaryElement must be used later, so
						// let's mark the current index here.
						boundaryLimitIndex = boundaryElements.length - 1 ;
					}
					else
					{
						var pathElementName = pathElement.nodeName.toLowerCase() ;

						if ( pathElementName == this.Element )
						{
							// Remove any attribute that conflict with this style, no
							// matter their values.
							for ( var att in styleAttribs )
							{
								if ( FCKDomTools.HasAttribute( pathElement, att ) )
								{
									switch ( att )
									{
										case 'style' :
											this._RemoveStylesFromElement( pathElement ) ;
											break ;

										case 'class' :
											// The 'class' element value must match (#1318).
											if ( FCKDomTools.GetAttributeValue( pathElement, att ) != this.GetFinalAttributeValue( att ) )
												continue ;

											/*jsl:fallthru*/

										default :
											FCKDomTools.RemoveAttribute( pathElement, att ) ;
									}
								}
							}
						}

						// Remove overrides defined to the same element name.
						this._RemoveOverrides( pathElement, styleOverrides[ pathElementName ] ) ;

						// Remove the element if no more attributes are available and it's an inline style element
						if ( this.GetType() == FCK_STYLE_INLINE)
							this._RemoveNoAttribElement( pathElement ) ;
					}
				}
				else if ( isBoundary )
					boundaryElements.push( pathElement ) ;

				// Check if we are still in a boundary (at the same side).
				isBoundary = isBoundary && ( ( isBoundaryRight && !FCKDomTools.GetNextSibling( pathElement ) ) || ( !isBoundaryRight && !FCKDomTools.GetPreviousSibling( pathElement ) ) ) ;

				// If we are in an element that is not anymore a boundary, or
				// we are at the last element, let's move things outside the
				// boundary (if available).
				if ( lastBoundaryElement && ( !isBoundary || ( i == path.Elements.length - 1 ) ) )
				{
					// Remove the bookmark node from the DOM.
					var currentElement = FCKDomTools.RemoveNode( bookmarkStart ) ;

					// Build the collapsed group of elements that are not
					// removed by this style, but share the boundary.
					// (see comment 1 and 2 at #1270)
					for ( var j = 0 ; j <= boundaryLimitIndex ; j++ )
					{
						var newElement = FCKDomTools.CloneElement( boundaryElements[j] ) ;
						newElement.appendChild( currentElement ) ;
						currentElement = newElement ;
					}

					// Re-insert the bookmark node (and the collapsed elements)
					// in the DOM, in the new position next to the styled element.
					if ( isBoundaryRight )
						FCKDomTools.InsertAfterNode( lastBoundaryElement, currentElement ) ;
					else
						lastBoundaryElement.parentNode.insertBefore( currentElement, lastBoundaryElement ) ;

					isBoundary = false ;
					lastBoundaryElement = null ;
				}
			}

				// Re-select the original range.
			if ( selectIt )
				range.SelectBookmark( bookmark ) ;

			if ( updateRange )
				range.MoveToBookmark( bookmark ) ;

			return ;
		}

		// Expand the range, if inside inline element boundaries.
		range.Expand( 'inline_elements' ) ;

		// Bookmark the range so we can re-select it after processing.
		bookmark = range.CreateBookmark( true ) ;

		// The style will be applied within the bookmark boundaries.
		var startNode	= range.GetBookmarkNode( bookmark, true ) ;
		var endNode		= range.GetBookmarkNode( bookmark, false ) ;

		range.Release( true ) ;

		// We need to check the selection boundaries (bookmark spans) to break
		// the code in a way that we can properly remove partially selected nodes.
		// For example, removing a <b> style from
		//		<b>This is [some text</b> to show <b>the] problem</b>
		// ... where [ and ] represent the selection, must result:
		//		<b>This is </b>[some text to show the]<b> problem</b>
		// The strategy is simple, we just break the partial nodes before the
		// removal logic, having something that could be represented this way:
		//		<b>This is </b>[<b>some text</b> to show <b>the</b>]<b> problem</b>

		// Let's start checking the start boundary.
		var path = new FCKElementPath( startNode ) ;
		var pathElements = path.Elements ;
		var pathElement ;

		for ( var i = 1 ; i < pathElements.length ; i++ )
		{
			pathElement = pathElements[i] ;

			if ( pathElement == path.Block || pathElement == path.BlockLimit )
				break ;

			// If this element can be removed (even partially).
			if ( this.CheckElementRemovable( pathElement ) )
				FCKDomTools.BreakParent( startNode, pathElement, range ) ;
		}

		// Now the end boundary.
		path = new FCKElementPath( endNode ) ;
		pathElements = path.Elements ;

		for ( var i = 1 ; i < pathElements.length ; i++ )
		{
			pathElement = pathElements[i] ;

			if ( pathElement == path.Block || pathElement == path.BlockLimit )
				break ;

			elementName = pathElement.nodeName.toLowerCase() ;

			// If this element can be removed (even partially).
			if ( this.CheckElementRemovable( pathElement ) )
				FCKDomTools.BreakParent( endNode, pathElement, range ) ;
		}

		// Navigate through all nodes between the bookmarks.
		var currentNode = FCKDomTools.GetNextSourceNode( startNode, true ) ;

		while ( currentNode )
		{
			// Cache the next node to be processed. Do it now, because
			// currentNode may be removed.
			var nextNode = FCKDomTools.GetNextSourceNode( currentNode ) ;

			// Remove elements nodes that match with this style rules.
			if ( currentNode.nodeType == 1 )
			{
				var elementName = currentNode.nodeName.toLowerCase() ;

				var mayRemove = ( elementName == this.Element ) ;
				if ( mayRemove )
				{
					// Remove any attribute that conflict with this style, no matter
					// their values.
					for ( var att in styleAttribs )
					{
						if ( FCKDomTools.HasAttribute( currentNode, att ) )
						{
							switch ( att )
							{
								case 'style' :
									this._RemoveStylesFromElement( currentNode ) ;
									break ;

								case 'class' :
									// The 'class' element value must match (#1318).
									if ( FCKDomTools.GetAttributeValue( currentNode, att ) != this.GetFinalAttributeValue( att ) )
										continue ;

									/*jsl:fallthru*/

								default :
									FCKDomTools.RemoveAttribute( currentNode, att ) ;
							}
						}
					}
				}
				else
					mayRemove = !!styleOverrides[ elementName ] ;

				if ( mayRemove )
				{
					// Remove overrides defined to the same element name.
					this._RemoveOverrides( currentNode, styleOverrides[ elementName ] ) ;

					// Remove the element if no more attributes are available.
					this._RemoveNoAttribElement( currentNode ) ;
				}
			}

			// If we have reached the end of the selection, stop looping.
			if ( nextNode == endNode )
				break ;

			currentNode = nextNode ;
		}

		this._FixBookmarkStart( startNode ) ;

		// Re-select the original range.
		if ( selectIt )
			range.SelectBookmark( bookmark ) ;

		if ( updateRange )
			range.MoveToBookmark( bookmark ) ;
	},

	/**
	 * Checks if an element, or any of its attributes, is removable by the
	 * current style definition.
	 */
	CheckElementRemovable : function( element, fullMatch )
	{
		if ( !element )
			return false ;

		var elementName = element.nodeName.toLowerCase() ;

		// If the element name is the same as the style name.
		if ( elementName == this.Element )
		{
			// If no attributes are defined in the element.
			if ( !fullMatch && !FCKDomTools.HasAttributes( element ) )
				return true ;

			// If any attribute conflicts with the style attributes.
			var attribs = this._GetAttribsForComparison() ;
			var allMatched = ( attribs._length == 0 ) ;
			for ( var att in attribs )
			{
				if ( att == '_length' )
					continue ;

				if ( this._CompareAttributeValues( att, FCKDomTools.GetAttributeValue( element, att ), ( this.GetFinalAttributeValue( att ) || '' ) ) )
				{
					allMatched = true ;
					if ( !fullMatch )
						break ;
				}
				else
				{
					allMatched = false ;
					if ( fullMatch )
						return false ;
				}
			}
			if ( allMatched )
				return true ;
		}

		// Check if the element can be somehow overriden.
		var override = this._GetOverridesForComparison()[ elementName ] ;
		if ( override )
		{
			// If no attributes have been defined, remove the element.
			if ( !( attribs = override.Attributes ) ) // Only one "="
				return true ;

			for ( var i = 0 ; i < attribs.length ; i++ )
			{
				var attName = attribs[i][0] ;
				if ( FCKDomTools.HasAttribute( element, attName ) )
				{
					var attValue = attribs[i][1] ;

					// Remove the attribute if:
					//    - The override definition value is null ;
					//    - The override definition valie is a string that
					//      matches the attribute value exactly.
					//    - The override definition value is a regex that
					//      has matches in the attribute value.
					if ( attValue == null ||
							( typeof attValue == 'string' && FCKDomTools.GetAttributeValue( element, attName ) == attValue ) ||
							attValue.test( FCKDomTools.GetAttributeValue( element, attName ) ) )
						return true ;
				}
			}
		}

		return false ;
	},

	/**
	 * Get the style state for an element path. Returns "true" if the element
	 * is active in the path.
	 */
	CheckActive : function( elementPath )
	{
		switch ( this.GetType() )
		{
			case FCK_STYLE_BLOCK :
				return this.CheckElementRemovable( elementPath.Block || elementPath.BlockLimit, true ) ;

			case FCK_STYLE_INLINE :

				var elements = elementPath.Elements ;

				for ( var i = 0 ; i < elements.length ; i++ )
				{
					var element = elements[i] ;

					if ( element == elementPath.Block || element == elementPath.BlockLimit )
						continue ;

					if ( this.CheckElementRemovable( element, true ) )
						return true ;
				}
		}
		return false ;
	},

	/**
	 * Removes an inline style from inside an element tree. The element node
	 * itself is not checked or removed, only the child tree inside of it.
	 */
	RemoveFromElement : function( element )
	{
		var attribs = this._GetAttribsForComparison() ;
		var overrides = this._GetOverridesForComparison() ;

		// Get all elements with the same name.
		var innerElements = element.getElementsByTagName( this.Element ) ;

		for ( var i = innerElements.length - 1 ; i >= 0 ; i-- )
		{
			var innerElement = innerElements[i] ;

			// Remove any attribute that conflict with this style, no matter
			// their values.
			for ( var att in attribs )
			{
				if ( FCKDomTools.HasAttribute( innerElement, att ) )
				{
					switch ( att )
					{
						case 'style' :
							this._RemoveStylesFromElement( innerElement ) ;
							break ;

						case 'class' :
							// The 'class' element value must match (#1318).
							if ( FCKDomTools.GetAttributeValue( innerElement, att ) != this.GetFinalAttributeValue( att ) )
								continue ;

							/*jsl:fallthru*/

						default :
							FCKDomTools.RemoveAttribute( innerElement, att ) ;
					}
				}
			}

			// Remove overrides defined to the same element name.
			this._RemoveOverrides( innerElement, overrides[ this.Element ] ) ;

			// Remove the element if no more attributes are available.
			this._RemoveNoAttribElement( innerElement ) ;
		}

		// Now remove any other element with different name that is
		// defined to be overriden.
		for ( var overrideElement in overrides )
		{
			if ( overrideElement != this.Element )
			{
				// Get all elements.
				innerElements = element.getElementsByTagName( overrideElement ) ;

				for ( var i = innerElements.length - 1 ; i >= 0 ; i-- )
				{
					var innerElement = innerElements[i] ;
					this._RemoveOverrides( innerElement, overrides[ overrideElement ] ) ;
					this._RemoveNoAttribElement( innerElement ) ;
				}
			}
		}
	},

	_RemoveStylesFromElement : function( element )
	{
		var elementStyle = element.style.cssText ;
		var pattern = this.GetFinalStyleValue() ;

		if ( elementStyle.length > 0 && pattern.length == 0 )
			return ;

		pattern = '(^|;)\\s*(' +
			pattern.replace( /\s*([^ ]+):.*?(;|$)/g, '$1|' ).replace( /\|$/, '' ) +
			'):[^;]+' ;

		var regex = new RegExp( pattern, 'gi' ) ;

		elementStyle = elementStyle.replace( regex, '' ).Trim() ;

		if ( elementStyle.length == 0 || elementStyle == ';' )
			FCKDomTools.RemoveAttribute( element, 'style' ) ;
		else
			element.style.cssText = elementStyle.replace( regex, '' ) ;
	},

	/**
	 * Remove all attributes that are defined to be overriden,
	 */
	_RemoveOverrides : function( element, override )
	{
		var attributes = override && override.Attributes ;

		if ( attributes )
		{
			for ( var i = 0 ; i < attributes.length ; i++ )
			{
				var attName = attributes[i][0] ;

				if ( FCKDomTools.HasAttribute( element, attName ) )
				{
					var attValue	= attributes[i][1] ;

					// Remove the attribute if:
					//    - The override definition value is null ;
					//    - The override definition valie is a string that
					//      matches the attribute value exactly.
					//    - The override definition value is a regex that
					//      has matches in the attribute value.
					if ( attValue == null ||
							( attValue.test && attValue.test( FCKDomTools.GetAttributeValue( element, attName ) ) ) ||
							( typeof attValue == 'string' && FCKDomTools.GetAttributeValue( element, attName ) == attValue ) )
						FCKDomTools.RemoveAttribute( element, attName ) ;
				}
			}
		}
	},

	/**
	 * If the element has no more attributes, remove it.
	 */
	_RemoveNoAttribElement : function( element )
	{
		// If no more attributes remained in the element, remove it,
		// leaving its children.
		if ( !FCKDomTools.HasAttributes( element ) )
		{
			// Removing elements may open points where merging is possible,
			// so let's cache the first and last nodes for later checking.
			var firstChild	= element.firstChild ;
			var lastChild	= element.lastChild ;

			FCKDomTools.RemoveNode( element, true ) ;

			// Check the cached nodes for merging.
			this._MergeSiblings( firstChild ) ;

			if ( firstChild != lastChild )
				this._MergeSiblings( lastChild ) ;
		}
	},

	/**
	 * Creates a DOM element for this style object.
	 */
	BuildElement : function( targetDoc, element )
	{
		// Create the element.
		var el = element || targetDoc.createElement( this.Element ) ;

		// Assign all defined attributes.
		var attribs	= this._StyleDesc.Attributes ;
		var attValue ;
		if ( attribs )
		{
			for ( var att in attribs )
			{
				attValue = this.GetFinalAttributeValue( att ) ;

				if ( att.toLowerCase() == 'class' )
					el.className = attValue ;
				else
					el.setAttribute( att, attValue ) ;
			}
		}

		// Assign the style attribute.
		if ( this._GetStyleText().length > 0 )
			el.style.cssText = this.GetFinalStyleValue() ;

		return el ;
	},

	_CompareAttributeValues : function( attName, valueA, valueB )
	{
		if ( attName == 'style' && valueA && valueB )
		{
			valueA = valueA.replace( /;$/, '' ).toLowerCase() ;
			valueB = valueB.replace( /;$/, '' ).toLowerCase() ;
		}

		// Return true if they match or if valueA is null and valueB is an empty string
		return ( valueA == valueB || ( ( valueA === null || valueA === '' ) && ( valueB === null || valueB === '' ) ) )
	},

	GetFinalAttributeValue : function( attName )
	{
		var attValue = this._StyleDesc.Attributes ;
		var attValue = attValue ? attValue[ attName ] : null ;

		if ( !attValue && attName == 'style' )
			return this.GetFinalStyleValue() ;

		if ( attValue && this._Variables )
			// Using custom Replace() to guarantee the correct scope.
			attValue = attValue.Replace( FCKRegexLib.StyleVariableAttName, this._GetVariableReplace, this ) ;

		return attValue ;
	},

	GetFinalStyleValue : function()
	{
		var attValue = this._GetStyleText() ;

		if ( attValue.length > 0 && this._Variables )
		{
			// Using custom Replace() to guarantee the correct scope.
			attValue = attValue.Replace( FCKRegexLib.StyleVariableAttName, this._GetVariableReplace, this ) ;
			attValue = FCKTools.NormalizeCssText( attValue ) ;
		}

		return attValue ;
	},

	_GetVariableReplace : function()
	{
		// The second group in the regex is the variable name.
		return this._Variables[ arguments[2] ] || arguments[0] ;
	},

	/**
	 * Set the value of a variable attribute or style, to be used when
	 * appliying the style.
	 */
	SetVariable : function( name, value )
	{
		var variables = this._Variables ;

		if ( !variables )
			variables = this._Variables = {} ;

		this._Variables[ name ] = value ;
	},

	/**
	 * Converting from a PRE block to a non-PRE block in formatting operations.
	 */
	_FromPre : function( doc, block, newBlock )
	{
		var innerHTML = block.innerHTML ;

		// Trim the first and last linebreaks immediately after and before <pre>, </pre>,
		// if they exist.
		// This is done because the linebreaks are not rendered.
		innerHTML = innerHTML.replace( /(\r\n|\r)/g, '\n' ) ;
		innerHTML = innerHTML.replace( /^[ \t]*\n/, '' ) ;
		innerHTML = innerHTML.replace( /\n$/, '' ) ;

		// 1. Convert spaces or tabs at the beginning or at the end to &nbsp;
		innerHTML = innerHTML.replace( /^[ \t]+|[ \t]+$/g, function( match, offset, s )
				{
					if ( match.length == 1 )	// one space, preserve it
						return '&nbsp;' ;
					else if ( offset == 0 )		// beginning of block
						return new Array( match.length ).join( '&nbsp;' ) + ' ' ;
					else				// end of block
						return ' ' + new Array( match.length ).join( '&nbsp;' ) ;
				} ) ;

		// 2. Convert \n to <BR>.
		// 3. Convert contiguous (i.e. non-singular) spaces or tabs to &nbsp;
		var htmlIterator = new FCKHtmlIterator( innerHTML ) ;
		var results = [] ;
		htmlIterator.Each( function( isTag, value )
			{
				if ( !isTag )
				{
					value = value.replace( /\n/g, '<br>' ) ;
					value = value.replace( /[ \t]{2,}/g,
							function ( match )
							{
								return new Array( match.length ).join( '&nbsp;' ) + ' ' ;
							} ) ;
				}
				results.push( value ) ;
			} ) ;
		newBlock.innerHTML = results.join( '' ) ;
		return newBlock ;
	},

	/**
	 * Converting from a non-PRE block to a PRE block in formatting operations.
	 */
	_ToPre : function( doc, block, newBlock )
	{
		// Handle converting from a regular block to a <pre> block.
		var innerHTML = block.innerHTML.Trim() ;

		// 1. Delete ANSI whitespaces immediately before and after <BR> because
		//    they are not visible.
		// 2. Mark down any <BR /> nodes here so they can be turned into \n in
		//    the next step and avoid being compressed.
		innerHTML = innerHTML.replace( /[ \t\r\n]*(<br[^>]*>)[ \t\r\n]*/gi, '<br />' ) ;

		// 3. Compress other ANSI whitespaces since they're only visible as one
		//    single space previously.
		// 4. Convert &nbsp; to spaces since &nbsp; is no longer needed in <PRE>.
		// 5. Convert any <BR /> to \n. This must not be done earlier because
		//    the \n would then get compressed.
		var htmlIterator = new FCKHtmlIterator( innerHTML ) ;
		var results = [] ;
		htmlIterator.Each( function( isTag, value )
			{
				if ( !isTag )
					value = value.replace( /([ \t\n\r]+|&nbsp;)/g, ' ' ) ;
				else if ( isTag && value == '<br />' )
					value = '\n' ;
				results.push( value ) ;
			} ) ;

		// Assigning innerHTML to <PRE> in IE causes all linebreaks to be
		// reduced to spaces.
		// Assigning outerHTML to <PRE> in IE doesn't work if the <PRE> isn't
		// contained in another node since the node reference is changed after
		// outerHTML assignment.
		// So, we need some hacks to workaround IE bugs here.
		if ( FCKBrowserInfo.IsIE )
		{
			var temp = doc.createElement( 'div' ) ;
			temp.appendChild( newBlock ) ;
			newBlock.outerHTML = '<pre>\n' + results.join( '' ) + '</pre>' ;
			newBlock = temp.removeChild( temp.firstChild ) ;
		}
		else
			newBlock.innerHTML = results.join( '' ) ;

		return newBlock ;
	},

	/**
	 * Merge a <pre> block with a previous <pre> block, if available.
	 */
	_CheckAndMergePre : function( previousBlock, preBlock )
	{
		// Check if the previous block and the current block are next
		// to each other.
		if ( previousBlock != FCKDomTools.GetPreviousSourceElement( preBlock, true ) )
			return ;

		// Merge the previous <pre> block contents into the current <pre>
		// block.
		//
		// Another thing to be careful here is that currentBlock might contain
		// a '\n' at the beginning, and previousBlock might contain a '\n'
		// towards the end. These new lines are not normally displayed but they
		// become visible after merging.
		var innerHTML = previousBlock.innerHTML.replace( /\n$/, '' ) + '\n\n' +
				preBlock.innerHTML.replace( /^\n/, '' ) ;

		// Buggy IE normalizes innerHTML from <pre>, breaking whitespaces.
		if ( FCKBrowserInfo.IsIE )
			preBlock.outerHTML = '<pre>' + innerHTML + '</pre>' ;
		else
			preBlock.innerHTML = innerHTML ;

		// Remove the previous <pre> block.
		//
		// The preBlock must not be moved or deleted from the DOM tree. This
		// guarantees the FCKDomRangeIterator in _ApplyBlockStyle would not
		// get lost at the next iteration.
		FCKDomTools.RemoveNode( previousBlock ) ;
	},

	_CheckAndSplitPre : function( newBlock )
	{
		var lastNewBlock ;

		var cursor = newBlock.firstChild ;

		// We are not splitting <br><br> at the beginning of the block, so
		// we'll start from the second child.
		cursor = cursor && cursor.nextSibling ;

		while ( cursor )
		{
			var next = cursor.nextSibling ;

			// If we have two <BR>s, and they're not at the beginning or the end,
			// then we'll split up the contents following them into another block.
			// Stop processing if we are at the last child couple.
			if ( next && next.nextSibling && cursor.nodeName.IEquals( 'br' ) && next.nodeName.IEquals( 'br' ) )
			{
				// Remove the first <br>.
				FCKDomTools.RemoveNode( cursor ) ;

				// Move to the node after the second <br>.
				cursor = next.nextSibling ;

				// Remove the second <br>.
				FCKDomTools.RemoveNode( next ) ;

				// Create the block that will hold the child nodes from now on.
				lastNewBlock = FCKDomTools.InsertAfterNode( lastNewBlock || newBlock, FCKDomTools.CloneElement( newBlock ) ) ;

				continue ;
			}

			// If we split it, then start moving the nodes to the new block.
			if ( lastNewBlock )
			{
				cursor = cursor.previousSibling ;
				FCKDomTools.MoveNode(cursor.nextSibling, lastNewBlock ) ;
			}

			cursor = cursor.nextSibling ;
		}
	},

	/**
	 * Apply an inline style to a FCKDomRange.
	 *
	 * TODO
	 *	- Implement the "#" style handling.
	 *	- Properly handle block containers like <div> and <blockquote>.
	 */
	_ApplyBlockStyle : function( range, selectIt, updateRange )
	{
		// Bookmark the range so we can re-select it after processing.
		var bookmark ;

		if ( selectIt )
			bookmark = range.CreateBookmark() ;

		var iterator = new FCKDomRangeIterator( range ) ;
		iterator.EnforceRealBlocks = true ;

		var block ;
		var doc = range.Window.document ;
		var previousPreBlock ;

		while( ( block = iterator.GetNextParagraph() ) )		// Only one =
		{
			// Create the new node right before the current one.
			var newBlock = this.BuildElement( doc ) ;

			// Check if we are changing from/to <pre>.
			var newBlockIsPre	= newBlock.nodeName.IEquals( 'pre' ) ;
			var blockIsPre		= block.nodeName.IEquals( 'pre' ) ;

			var toPre	= newBlockIsPre && !blockIsPre ;
			var fromPre	= !newBlockIsPre && blockIsPre ;

			// Move everything from the current node to the new one.
			if ( toPre )
				newBlock = this._ToPre( doc, block, newBlock ) ;
			else if ( fromPre )
				newBlock = this._FromPre( doc, block, newBlock ) ;
			else	// Convering from a regular block to another regular block.
				FCKDomTools.MoveChildren( block, newBlock ) ;

			// Replace the current block.
			block.parentNode.insertBefore( newBlock, block ) ;
			FCKDomTools.RemoveNode( block ) ;

			// Complete other tasks after inserting the node in the DOM.
			if ( newBlockIsPre )
			{
				if ( previousPreBlock )
					this._CheckAndMergePre( previousPreBlock, newBlock ) ;	// Merge successive <pre> blocks.
				previousPreBlock = newBlock ;
			}
			else if ( fromPre )
				this._CheckAndSplitPre( newBlock ) ;	// Split <br><br> in successive <pre>s.
		}

		// Re-select the original range.
		if ( selectIt )
			range.SelectBookmark( bookmark ) ;

		if ( updateRange )
			range.MoveToBookmark( bookmark ) ;
	},

	/**
	 * Apply an inline style to a FCKDomRange.
	 *
	 * TODO
	 *	- Merge elements, when applying styles to similar elements that enclose
	 *    the entire selection, outputing:
	 *        <span style="color: #ff0000; background-color: #ffffff">XYZ</span>
	 *    instead of:
	 *        <span style="color: #ff0000;"><span style="background-color: #ffffff">XYZ</span></span>
	 */
	_ApplyInlineStyle : function( range, selectIt, updateRange )
	{
		var doc = range.Window.document ;

		if ( range.CheckIsCollapsed() )
		{
			// Create the element to be inserted in the DOM.
			var collapsedElement = this.BuildElement( doc ) ;
			range.InsertNode( collapsedElement ) ;
			range.MoveToPosition( collapsedElement, 2 ) ;
			range.Select() ;

			return ;
		}

		// The general idea here is navigating through all nodes inside the
		// current selection, working on distinct range blocks, defined by the
		// DTD compatibility between the style element and the nodes inside the
		// ranges.
		//
		// For example, suppose we have the following selection (where [ and ]
		// are the boundaries), and we apply a <b> style there:
		//
		//		<p>Here we [have <b>some</b> text.<p>
		//		<p>And some here] here.</p>
		//
		// Two different ranges will be detected:
		//
		//		"have <b>some</b> text."
		//		"And some here"
		//
		// Both ranges will be extracted, moved to a <b> element, and
		// re-inserted, resulting in the following output:
		//
		//		<p>Here we [<b>have some text.</b><p>
		//		<p><b>And some here</b>] here.</p>
		//
		// Note that the <b> element at <b>some</b> is also removed because it
		// is not needed anymore.

		var elementName = this.Element ;

		// Get the DTD definition for the element. Defaults to "span".
		var elementDTD = FCK.DTD[ elementName ] || FCK.DTD.span ;

		// Create the attribute list to be used later for element comparisons.
		var styleAttribs = this._GetAttribsForComparison() ;
		var styleNode ;

		// Expand the range, if inside inline element boundaries.
		range.Expand( 'inline_elements' ) ;

		// Bookmark the range so we can re-select it after processing.
		var bookmark = range.CreateBookmark( true ) ;

		// The style will be applied within the bookmark boundaries.
		var startNode	= range.GetBookmarkNode( bookmark, true ) ;
		var endNode		= range.GetBookmarkNode( bookmark, false ) ;

		// We'll be reusing the range to apply the styles. So, release it here
		// to indicate that it has not been initialized.
		range.Release( true ) ;

		// Let's start the nodes lookup from the node right after the bookmark
		// span.
		var currentNode = FCKDomTools.GetNextSourceNode( startNode, true ) ;

		while ( currentNode )
		{
			var applyStyle = false ;

			var nodeType = currentNode.nodeType ;
			var nodeName = nodeType == 1 ? currentNode.nodeName.toLowerCase() : null ;

			// Check if the current node can be a child of the style element.
			if ( !nodeName || elementDTD[ nodeName ] )
			{
				// Check if the style element can be a child of the current
				// node parent or if the element is not defined in the DTD.
				if ( ( FCK.DTD[ currentNode.parentNode.nodeName.toLowerCase() ] || FCK.DTD.span )[ elementName ] || !FCK.DTD[ elementName ] )
				{
					// This node will be part of our range, so if it has not
					// been started, place its start right before the node.
					if ( !range.CheckHasRange() )
						range.SetStart( currentNode, 3 ) ;

					// Non element nodes, or empty elements can be added
					// completely to the range.
					if ( nodeType != 1 || currentNode.childNodes.length == 0 )
					{
						var includedNode = currentNode ;
						var parentNode = includedNode.parentNode ;

						// This node is about to be included completelly, but,
						// if this is the last node in its parent, we must also
						// check if the parent itself can be added completelly
						// to the range.
						while ( includedNode == parentNode.lastChild
							&& elementDTD[ parentNode.nodeName.toLowerCase() ] )
						{
							includedNode = parentNode ;
						}

						range.SetEnd( includedNode, 4 ) ;

						// If the included node is the last node in its parent
						// and its parent can't be inside the style node, apply
						// the style immediately.
						if ( includedNode == includedNode.parentNode.lastChild && !elementDTD[ includedNode.parentNode.nodeName.toLowerCase() ] )
							applyStyle = true ;
					}
					else
					{
						// Element nodes will not be added directly. We need to
						// check their children because the selection could end
						// inside the node, so let's place the range end right
						// before the element.
						range.SetEnd( currentNode, 3 ) ;
					}
				}
				else
					applyStyle = true ;
			}
			else
				applyStyle = true ;

			// Get the next node to be processed.
			currentNode = FCKDomTools.GetNextSourceNode( currentNode ) ;

			// If we have reached the end of the selection, just apply the
			// style ot the range, and stop looping.
			if ( currentNode == endNode )
			{
				currentNode = null ;
				applyStyle = true ;
			}

			// Apply the style if we have something to which apply it.
			if ( applyStyle && range.CheckHasRange() && !range.CheckIsCollapsed() )
			{
				// Build the style element, based on the style object definition.
				styleNode = this.BuildElement( doc ) ;

				// Move the contents of the range to the style element.
				range.ExtractContents().AppendTo( styleNode ) ;

				// If it is not empty.
				if ( styleNode.innerHTML.RTrim().length > 0 )
				{
					// Insert it in the range position (it is collapsed after
					// ExtractContents.
					range.InsertNode( styleNode ) ;

					// Here we do some cleanup, removing all duplicated
					// elements from the style element.
					this.RemoveFromElement( styleNode ) ;

					// Let's merge our new style with its neighbors, if possible.
					this._MergeSiblings( styleNode, this._GetAttribsForComparison() ) ;

					// As the style system breaks text nodes constantly, let's normalize
					// things for performance.
					// With IE, some paragraphs get broken when calling normalize()
					// repeatedly. Also, for IE, we must normalize body, not documentElement.
					// IE is also known for having a "crash effect" with normalize().
					// We should try to normalize with IE too in some way, somewhere.
					if ( !FCKBrowserInfo.IsIE )
						styleNode.normalize() ;
				}

				// Style applied, let's release the range, so it gets marked to
				// re-initialization in the next loop.
				range.Release( true ) ;
			}
		}

		this._FixBookmarkStart( startNode ) ;

		// Re-select the original range.
		if ( selectIt )
			range.SelectBookmark( bookmark ) ;

		if ( updateRange )
			range.MoveToBookmark( bookmark ) ;
	},

	_FixBookmarkStart : function( startNode )
	{
		// After appliying or removing an inline style, the start boundary of
		// the selection must be placed inside all inline elements it is
		// bordering.
		var startSibling ;
		while ( ( startSibling = startNode.nextSibling ) )	// Only one "=".
		{
			if ( startSibling.nodeType == 1
				&& FCKListsLib.InlineNonEmptyElements[ startSibling.nodeName.toLowerCase() ] )
			{
				// If it is an empty inline element, we can safely remove it.
				if ( !startSibling.firstChild )
					FCKDomTools.RemoveNode( startSibling ) ;
				else
					FCKDomTools.MoveNode( startNode, startSibling, true ) ;
				continue ;
			}

			// Empty text nodes can be safely removed to not disturb.
			if ( startSibling.nodeType == 3 && startSibling.length == 0 )
			{
				FCKDomTools.RemoveNode( startSibling ) ;
				continue ;
			}

			break ;
		}
	},

	/**
	 * Merge an element with its similar siblings.
	 * "attribs" is and object computed with _CreateAttribsForComparison.
	 */
	_MergeSiblings : function( element, attribs )
	{
		if ( !element || element.nodeType != 1 || !FCKListsLib.InlineNonEmptyElements[ element.nodeName.toLowerCase() ] )
			return ;

		this._MergeNextSibling( element, attribs ) ;
		this._MergePreviousSibling( element, attribs ) ;
	},

	/**
	 * Merge an element with its similar siblings after it.
	 * "attribs" is and object computed with _CreateAttribsForComparison.
	 */
	_MergeNextSibling : function( element, attribs )
	{
		// Check the next sibling.
		var sibling = element.nextSibling ;

		// Check if the next sibling is a bookmark element. In this case, jump it.
		var hasBookmark = ( sibling && sibling.nodeType == 1 && sibling.getAttribute( '_fck_bookmark' ) ) ;
		if ( hasBookmark )
			sibling = sibling.nextSibling ;

		if ( sibling && sibling.nodeType == 1 && sibling.nodeName == element.nodeName )
		{
			if ( !attribs )
				attribs = this._CreateElementAttribsForComparison( element ) ;

			if ( this._CheckAttributesMatch( sibling, attribs ) )
			{
				// Save the last child to be checked too (to merge things like <b><i></i></b><b><i></i></b>).
				var innerSibling = element.lastChild ;

				if ( hasBookmark )
					FCKDomTools.MoveNode( element.nextSibling, element ) ;

				// Move contents from the sibling.
				FCKDomTools.MoveChildren( sibling, element ) ;
				FCKDomTools.RemoveNode( sibling ) ;

				// Now check the last inner child (see two comments above).
				if ( innerSibling )
					this._MergeNextSibling( innerSibling ) ;
			}
		}
	},

	/**
	 * Merge an element with its similar siblings before it.
	 * "attribs" is and object computed with _CreateAttribsForComparison.
	 */
	_MergePreviousSibling : function( element, attribs )
	{
		// Check the previous sibling.
		var sibling = element.previousSibling ;

		// Check if the previous sibling is a bookmark element. In this case, jump it.
		var hasBookmark = ( sibling && sibling.nodeType == 1 && sibling.getAttribute( '_fck_bookmark' ) ) ;
		if ( hasBookmark )
			sibling = sibling.previousSibling ;

		if ( sibling && sibling.nodeType == 1 && sibling.nodeName == element.nodeName )
		{
			if ( !attribs )
				attribs = this._CreateElementAttribsForComparison( element ) ;

			if ( this._CheckAttributesMatch( sibling, attribs ) )
			{
				// Save the first child to be checked too (to merge things like <b><i></i></b><b><i></i></b>).
				var innerSibling = element.firstChild ;

				if ( hasBookmark )
					FCKDomTools.MoveNode( element.previousSibling, element, true ) ;

				// Move contents to the sibling.
				FCKDomTools.MoveChildren( sibling, element, true ) ;
				FCKDomTools.RemoveNode( sibling ) ;

				// Now check the first inner child (see two comments above).
				if ( innerSibling )
					this._MergePreviousSibling( innerSibling ) ;
			}
		}
	},

	/**
	 * Build the cssText based on the styles definition.
	 */
	_GetStyleText : function()
	{
		var stylesDef = this._StyleDesc.Styles ;

		// Builds the StyleText.
		var stylesText = ( this._StyleDesc.Attributes ? this._StyleDesc.Attributes['style'] || '' : '' ) ;

		if ( stylesText.length > 0 )
			stylesText += ';' ;

		for ( var style in stylesDef )
			stylesText += style + ':' + stylesDef[style] + ';' ;

		// Browsers make some changes to the style when applying them. So, here
		// we normalize it to the browser format. We'll not do that if there
		// are variables inside the style.
		if ( stylesText.length > 0 && !( /#\(/.test( stylesText ) ) )
		{
			stylesText = FCKTools.NormalizeCssText( stylesText ) ;
		}

		return (this._GetStyleText = function() { return stylesText ; })() ;
	},

	/**
	 * Get the the collection used to compare the attributes defined in this
	 * style with attributes in an element. All information in it is lowercased.
	 */
	_GetAttribsForComparison : function()
	{
		// If we have already computed it, just return it.
		var attribs = this._GetAttribsForComparison_$ ;
		if ( attribs )
			return attribs ;

		attribs = new Object() ;

		// Loop through all defined attributes.
		var styleAttribs = this._StyleDesc.Attributes ;
		if ( styleAttribs )
		{
			for ( var styleAtt in styleAttribs )
			{
				attribs[ styleAtt.toLowerCase() ] = styleAttribs[ styleAtt ].toLowerCase() ;
			}
		}

		// Includes the style definitions.
		if ( this._GetStyleText().length > 0 )
		{
			attribs['style'] = this._GetStyleText().toLowerCase() ;
		}

		// Appends the "length" information to the object.
		FCKTools.AppendLengthProperty( attribs, '_length' ) ;

		// Return it, saving it to the next request.
		return ( this._GetAttribsForComparison_$ = attribs ) ;
	},

	/**
	 * Get the the collection used to compare the elements and attributes,
	 * defined in this style overrides, with other element. All information in
	 * it is lowercased.
	 */
	_GetOverridesForComparison : function()
	{
		// If we have already computed it, just return it.
		var overrides = this._GetOverridesForComparison_$ ;
		if ( overrides )
			return overrides ;

		overrides = new Object() ;

		var overridesDesc = this._StyleDesc.Overrides ;

		if ( overridesDesc )
		{
			// The override description can be a string, object or array.
			// Internally, well handle arrays only, so transform it if needed.
			if ( !FCKTools.IsArray( overridesDesc ) )
				overridesDesc = [ overridesDesc ] ;

			// Loop through all override definitions.
			for ( var i = 0 ; i < overridesDesc.length ; i++ )
			{
				var override = overridesDesc[i] ;
				var elementName ;
				var overrideEl ;
				var attrs ;

				// If can be a string with the element name.
				if ( typeof override == 'string' )
					elementName = override.toLowerCase() ;
				// Or an object.
				else
				{
					elementName = override.Element ? override.Element.toLowerCase() : this.Element ;
					attrs = override.Attributes ;
				}

				// We can have more than one override definition for the same
				// element name, so we attempt to simply append information to
				// it if it already exists.
				overrideEl = overrides[ elementName ] || ( overrides[ elementName ] = {} ) ;

				if ( attrs )
				{
					// The returning attributes list is an array, because we
					// could have different override definitions for the same
					// attribute name.
					var overrideAttrs = ( overrideEl.Attributes = overrideEl.Attributes || new Array() ) ;
					for ( var attName in attrs )
					{
						// Each item in the attributes array is also an array,
						// where [0] is the attribute name and [1] is the
						// override value.
						overrideAttrs.push( [ attName.toLowerCase(), attrs[ attName ] ] ) ;
					}
				}
			}
		}

		return ( this._GetOverridesForComparison_$ = overrides ) ;
	},

	/*
	 * Create and object containing all attributes specified in an element,
	 * added by a "_length" property. All values are lowercased.
	 */
	_CreateElementAttribsForComparison : function( element )
	{
		var attribs = new Object() ;
		var attribsCount = 0 ;

		for ( var i = 0 ; i < element.attributes.length ; i++ )
		{
			var att = element.attributes[i] ;

			if ( att.specified )
			{
				attribs[ att.nodeName.toLowerCase() ] = FCKDomTools.GetAttributeValue( element, att ).toLowerCase() ;
				attribsCount++ ;
			}
		}

		attribs._length = attribsCount ;

		return attribs ;
	},

	/**
	 * Checks is the element attributes have a perfect match with the style
	 * attributes.
	 */
	_CheckAttributesMatch : function( element, styleAttribs )
	{
		// Loop through all specified attributes. The same number of
		// attributes must be found and their values must match to
		// declare them as equal.

		var elementAttrbs = element.attributes ;
		var matchCount = 0 ;

		for ( var i = 0 ; i < elementAttrbs.length ; i++ )
		{
			var att = elementAttrbs[i] ;
			if ( att.specified )
			{
				var attName = att.nodeName.toLowerCase() ;
				var styleAtt = styleAttribs[ attName ] ;

				// The attribute is not defined in the style.
				if ( !styleAtt )
					break ;

				// The values are different.
				if ( styleAtt != FCKDomTools.GetAttributeValue( element, att ).toLowerCase() )
					break ;

				matchCount++ ;
			}
		}

		return ( matchCount == styleAttribs._length ) ;
	}
} ;
