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
 * Active selection functions. (Gecko specific implementation)
 */

// Get the selection type (like document.select.type in IE).
FCKSelection.GetType = function()
{
	// By default set the type to "Text".
	var type = 'Text' ;

	// Check if the actual selection is a Control (IMG, TABLE, HR, etc...).

	var sel ;
	try { sel = this.GetSelection() ; } catch (e) {}

	if ( sel && sel.rangeCount == 1 )
	{
		var range = sel.getRangeAt(0) ;
		if ( range.startContainer == range.endContainer
			&& ( range.endOffset - range.startOffset ) == 1
			&& range.startContainer.nodeType == 1
			&& FCKListsLib.StyleObjectElements[ range.startContainer.childNodes[ range.startOffset ].nodeName.toLowerCase() ] )
		{
			type = 'Control' ;
		}
	}

	return type ;
}

// Retrieves the selected element (if any), just in the case that a single
// element (object like and image or a table) is selected.
FCKSelection.GetSelectedElement = function()
{
	var selection = !!FCK.EditorWindow && this.GetSelection() ;
	if ( !selection || selection.rangeCount < 1 )
		return null ;

	var range = selection.getRangeAt( 0 ) ;
	if ( range.startContainer != range.endContainer || range.startContainer.nodeType != 1 || range.startOffset != range.endOffset - 1 )
		return null ;

	var node = range.startContainer.childNodes[ range.startOffset ] ;
	if ( node.nodeType != 1 )
		return null ;

	return node ;
}

FCKSelection.GetParentElement = function()
{
	if ( this.GetType() == 'Control' )
		return FCKSelection.GetSelectedElement().parentNode ;
	else
	{
		var oSel = this.GetSelection() ;
		if ( oSel )
		{
			// if anchorNode == focusNode, see if the selection is text only or including nodes.
			// if text only, return the parent node.
			// if the selection includes DOM nodes, then the anchorNode is the nearest container.
			if ( oSel.anchorNode && oSel.anchorNode == oSel.focusNode )
			{
				var oRange = oSel.getRangeAt( 0 ) ;
				if ( oRange.collapsed || oRange.startContainer.nodeType == 3 )
					return oSel.anchorNode.parentNode ;
				else
					return oSel.anchorNode ;
			}

			// looks like we're having a large selection here. To make the behavior same as IE's TextRange.parentElement(),
			// we need to find the nearest ancestor node which encapsulates both the beginning and the end of the selection.
			// TODO: A simpler logic can be found.
			var anchorPath = new FCKElementPath( oSel.anchorNode ) ;
			var focusPath = new FCKElementPath( oSel.focusNode ) ;
			var deepPath = null ;
			var shallowPath = null ;
			if ( anchorPath.Elements.length > focusPath.Elements.length )
			{
				deepPath = anchorPath.Elements ;
				shallowPath = focusPath.Elements ;
			}
			else
			{
				deepPath = focusPath.Elements ;
				shallowPath = anchorPath.Elements ;
			}

			var deepPathBase = deepPath.length - shallowPath.length ;
			for( var i = 0 ; i < shallowPath.length ; i++)
			{
				if ( deepPath[deepPathBase + i] == shallowPath[i])
					return shallowPath[i];
			}
			return null ;
		}
	}
	return null ;
}

FCKSelection.GetBoundaryParentElement = function( startBoundary )
{
	if ( ! FCK.EditorWindow )
		return null ;
	if ( this.GetType() == 'Control' )
		return FCKSelection.GetSelectedElement().parentNode ;
	else
	{
		var oSel = this.GetSelection() ;
		if ( oSel && oSel.rangeCount > 0 )
		{
			var range = oSel.getRangeAt( startBoundary ? 0 : ( oSel.rangeCount - 1 ) ) ;

			var element = startBoundary ? range.startContainer : range.endContainer ;

			return ( element.nodeType == 1 ? element : element.parentNode ) ;
		}
	}
	return null ;
}

FCKSelection.SelectNode = function( element )
{
	var oRange = FCK.EditorDocument.createRange() ;
	oRange.selectNode( element ) ;

	var oSel = this.GetSelection() ;
	oSel.removeAllRanges() ;
	oSel.addRange( oRange ) ;
}

FCKSelection.Collapse = function( toStart )
{
	var oSel = this.GetSelection() ;

	if ( toStart == null || toStart === true )
		oSel.collapseToStart() ;
	else
		oSel.collapseToEnd() ;
}

// The "nodeTagName" parameter must be Upper Case.
FCKSelection.HasAncestorNode = function( nodeTagName )
{
	var oContainer = this.GetSelectedElement() ;
	if ( ! oContainer && FCK.EditorWindow )
	{
		try		{ oContainer = this.GetSelection().getRangeAt(0).startContainer ; }
		catch(e){}
	}

	while ( oContainer )
	{
		if ( oContainer.nodeType == 1 && oContainer.nodeName.IEquals( nodeTagName ) ) return true ;
		oContainer = oContainer.parentNode ;
	}

	return false ;
}

// The "nodeTagName" parameter must be Upper Case.
FCKSelection.MoveToAncestorNode = function( nodeTagName )
{
	var oNode ;

	var oContainer = this.GetSelectedElement() ;
	if ( ! oContainer )
		oContainer = this.GetSelection().getRangeAt(0).startContainer ;

	while ( oContainer )
	{
		if ( oContainer.nodeName.IEquals( nodeTagName ) )
			return oContainer ;

		oContainer = oContainer.parentNode ;
	}
	return null ;
}

FCKSelection.Delete = function()
{
	// Gets the actual selection.
	var oSel = this.GetSelection() ;

	// Deletes the actual selection contents.
	for ( var i = 0 ; i < oSel.rangeCount ; i++ )
	{
		oSel.getRangeAt(i).deleteContents() ;
	}

	return oSel ;
}

/**
 * Returns the native selection object.
 */
FCKSelection.GetSelection = function()
{
	return FCK.EditorWindow.getSelection() ;
}

// The following are IE only features (we don't need then in other browsers
// currently).
FCKSelection.Save = function()
{}
FCKSelection.Restore = function()
{}
FCKSelection.Release = function()
{}
