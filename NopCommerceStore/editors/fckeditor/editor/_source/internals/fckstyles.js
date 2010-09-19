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
 * Handles styles in a give document.
 */

var FCKStyles = FCK.Styles =
{
	_Callbacks : {},
	_ObjectStyles : {},

	ApplyStyle : function( style )
	{
		if ( typeof style == 'string' )
			style = this.GetStyles()[ style ] ;

		if ( style )
		{
			if ( style.GetType() == FCK_STYLE_OBJECT )
				style.ApplyToObject( FCKSelection.GetSelectedElement() ) ;
			else
				style.ApplyToSelection( FCK.EditorWindow ) ;

			FCK.Events.FireEvent( 'OnSelectionChange' ) ;
		}
	},

	RemoveStyle : function( style )
	{
		if ( typeof style == 'string' )
			style = this.GetStyles()[ style ] ;

		if ( style )
		{
			style.RemoveFromSelection( FCK.EditorWindow ) ;
			FCK.Events.FireEvent( 'OnSelectionChange' ) ;
		}
	},

	/**
	 * Defines a callback function to be called when the current state of a
	 * specific style changes.
	 */
	AttachStyleStateChange : function( styleName, callback, callbackOwner )
	{
		var callbacks = this._Callbacks[ styleName ] ;

		if ( !callbacks )
			callbacks = this._Callbacks[ styleName ] = [] ;

		callbacks.push( [ callback, callbackOwner ] ) ;
	},

	CheckSelectionChanges : function()
	{
		var startElement = FCKSelection.GetBoundaryParentElement( true ) ;

		if ( !startElement )
			return ;

		// Walks the start node parents path, checking all styles that are being listened.
		var path = new FCKElementPath( startElement ) ;
		var styles = this.GetStyles() ;

		for ( var styleName in styles )
		{
			var callbacks = this._Callbacks[ styleName ] ;

			if ( callbacks )
			{
				var style = styles[ styleName ] ;
				var state = style.CheckActive( path ) ;

				if ( state != ( style._LastState || null ) )
				{
					style._LastState = state ;

					for ( var i = 0 ; i < callbacks.length ; i++ )
					{
						var callback = callbacks[i][0] ;
						var callbackOwner = callbacks[i][1] ;

						callback.call( callbackOwner || window, styleName, state ) ;
					}
				}
			}
		}
	},

	CheckStyleInSelection : function( styleName )
	{
		return false ;
	},

	_GetRemoveFormatTagsRegex : function ()
	{
		var regex = new RegExp( '^(?:' + FCKConfig.RemoveFormatTags.replace( /,/g,'|' ) + ')$', 'i' ) ;

		return (this._GetRemoveFormatTagsRegex = function()
		{
			return regex ;
		})
		&& regex  ;
	},

	/**
	 * Remove all styles from the current selection.
	 * TODO:
	 *  - This is almost a duplication of FCKStyle.RemoveFromRange. We should
	 *    try to merge things.
	 */
	RemoveAll : function()
	{
		var range = new FCKDomRange( FCK.EditorWindow ) ;
		range.MoveToSelection() ;

		if ( range.CheckIsCollapsed() )
			return ;

			// Expand the range, if inside inline element boundaries.
		range.Expand( 'inline_elements' ) ;

		// Get the bookmark nodes.
		// Bookmark the range so we can re-select it after processing.
		var bookmark = range.CreateBookmark( true ) ;

		// The style will be applied within the bookmark boundaries.
		var startNode	= range.GetBookmarkNode( bookmark, true ) ;
		var endNode		= range.GetBookmarkNode( bookmark, false ) ;

		range.Release( true ) ;

		var tagsRegex = this._GetRemoveFormatTagsRegex() ;

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
			if ( tagsRegex.test( pathElement.nodeName ) )
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
			if ( tagsRegex.test( pathElement.nodeName ) )
				FCKDomTools.BreakParent( endNode, pathElement, range ) ;
		}

		// Navigate through all nodes between the bookmarks.
		var currentNode = FCKDomTools.GetNextSourceNode( startNode, true, 1 ) ;

		while ( currentNode )
		{
			// If we have reached the end of the selection, stop looping.
			if ( currentNode == endNode )
				break ;

			// Cache the next node to be processed. Do it now, because
			// currentNode may be removed.
			var nextNode = FCKDomTools.GetNextSourceNode( currentNode, false, 1 ) ;

			// Remove elements nodes that match with this style rules.
			if ( tagsRegex.test( currentNode.nodeName ) )
				FCKDomTools.RemoveNode( currentNode, true ) ;
			else
				FCKDomTools.RemoveAttributes( currentNode, FCKConfig.RemoveAttributesArray );

			currentNode = nextNode ;
		}

		range.SelectBookmark( bookmark ) ;

		FCK.Events.FireEvent( 'OnSelectionChange' ) ;
	},

	GetStyle : function( styleName )
	{
		return this.GetStyles()[ styleName ] ;
	},

	GetStyles : function()
	{
		var styles = this._GetStyles ;
		if ( !styles )
		{
			styles = this._GetStyles = FCKTools.Merge(
				this._LoadStylesCore(),
				this._LoadStylesCustom(),
				this._LoadStylesXml() ) ;
		}
		return styles ;
	},

	CheckHasObjectStyle : function( elementName )
	{
		return !!this._ObjectStyles[ elementName ] ;
	},

	_LoadStylesCore : function()
	{
		var styles = {};
		var styleDefs = FCKConfig.CoreStyles ;

		for ( var styleName in styleDefs )
		{
			// Core styles are prefixed with _FCK_.
			var style = styles[ '_FCK_' + styleName ] = new FCKStyle( styleDefs[ styleName ] ) ;
			style.IsCore = true ;
		}
		return styles ;
	},

	_LoadStylesCustom : function()
	{
		var styles = {};
		var styleDefs = FCKConfig.CustomStyles ;

		if ( styleDefs )
		{
			for ( var styleName in styleDefs )
			{
				var style = styles[ styleName ] = new FCKStyle( styleDefs[ styleName ] ) ;
				style.Name = styleName ;
			}
		}

		return styles ;
	},

	_LoadStylesXml : function()
	{
		var styles = {};

		var stylesXmlPath = FCKConfig.StylesXmlPath ;

		if ( !stylesXmlPath || stylesXmlPath.length == 0 )
			return styles ;

		// Load the XML file into a FCKXml object.
		var xml = new FCKXml() ;
		xml.LoadUrl( stylesXmlPath ) ;

		var stylesXmlObj = FCKXml.TransformToObject( xml.SelectSingleNode( 'Styles' ) ) ;

		// Get the "Style" nodes defined in the XML file.
		var styleNodes = stylesXmlObj.$Style ;

		// Check that it did contain some valid nodes
		if ( !styleNodes )
			return styles ;

		// Add each style to our "Styles" collection.
		for ( var i = 0 ; i < styleNodes.length ; i++ )
		{
			var styleNode = styleNodes[i] ;

			var element = ( styleNode.element || '' ).toLowerCase() ;

			if ( element.length == 0 )
				throw( 'The element name is required. Error loading "' + stylesXmlPath + '"' ) ;

			var styleDef = {
				Element : element,
				Attributes : {},
				Styles : {},
				Overrides : []
			} ;

			// Get the attributes defined for the style (if any).
			var attNodes = styleNode.$Attribute || [] ;

			// Add the attributes to the style definition object.
			for ( var j = 0 ; j < attNodes.length ; j++ )
			{
				styleDef.Attributes[ attNodes[j].name ] = attNodes[j].value ;
			}

			// Get the styles defined for the style (if any).
			var cssStyleNodes = styleNode.$Style || [] ;

			// Add the attributes to the style definition object.
			for ( j = 0 ; j < cssStyleNodes.length ; j++ )
			{
				styleDef.Styles[ cssStyleNodes[j].name ] = cssStyleNodes[j].value ;
			}

			// Load override definitions.
			var cssStyleOverrideNodes = styleNode.$Override ;
			if ( cssStyleOverrideNodes )
			{
				for ( j = 0 ; j < cssStyleOverrideNodes.length ; j++ )
				{
					var overrideNode = cssStyleOverrideNodes[j] ;
					var overrideDef =
					{
						Element : overrideNode.element
					} ;

					var overrideAttNode = overrideNode.$Attribute ;
					if ( overrideAttNode )
					{
						overrideDef.Attributes = {} ;
						for ( var k = 0 ; k < overrideAttNode.length ; k++ )
						{
							var overrideAttValue = overrideAttNode[k].value || null ;
							if ( overrideAttValue )
							{
								// Check if the override attribute value is a regular expression.
								var regexMatch = overrideAttValue && FCKRegexLib.RegExp.exec( overrideAttValue ) ;
								if ( regexMatch )
									overrideAttValue = new RegExp( regexMatch[1], regexMatch[2] || '' ) ;
							}
							overrideDef.Attributes[ overrideAttNode[k].name ] = overrideAttValue ;
						}
					}

					styleDef.Overrides.push( overrideDef ) ;
				}
			}

			var style = new FCKStyle( styleDef ) ;
			style.Name = styleNode.name || element ;

			if ( style.GetType() == FCK_STYLE_OBJECT )
				this._ObjectStyles[ element ] = true ;

			// Add the style to the "Styles" collection using it's name as the key.
			styles[ style.Name ] = style ;
		}

		return styles ;
	}
} ;
