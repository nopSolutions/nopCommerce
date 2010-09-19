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
 * Class for working with a selection range, much like the W3C DOM Range, but
 * it is not intended to be an implementation of the W3C interface.
 * (Gecko Implementation)
 */

FCKDomRange.prototype.MoveToSelection = function()
{
	this.Release( true ) ;

	var oSel = this.Window.getSelection() ;

	if ( oSel && oSel.rangeCount > 0 )
	{
		this._Range = FCKW3CRange.CreateFromRange( this.Window.document, oSel.getRangeAt(0) ) ;
		this._UpdateElementInfo() ;
	}
	else
		if ( this.Window.document )
			this.MoveToElementStart( this.Window.document.body ) ;
}

FCKDomRange.prototype.Select = function()
{
	var oRange = this._Range ;
	if ( oRange )
	{
		var startContainer = oRange.startContainer ;

		// If we have a collapsed range, inside an empty element, we must add
		// something to it, otherwise the caret will not be visible.
		if ( oRange.collapsed && startContainer.nodeType == 1 && startContainer.childNodes.length == 0 )
			startContainer.appendChild( oRange._Document.createTextNode('') ) ;

		var oDocRange = this.Window.document.createRange() ;
		oDocRange.setStart( startContainer, oRange.startOffset ) ;

		try
		{
			oDocRange.setEnd( oRange.endContainer, oRange.endOffset ) ;
		}
		catch ( e )
		{
			// There is a bug in Firefox implementation (it would be too easy
			// otherwise). The new start can't be after the end (W3C says it can).
			// So, let's create a new range and collapse it to the desired point.
			if ( e.toString().Contains( 'NS_ERROR_ILLEGAL_VALUE' ) )
			{
				oRange.collapse( true ) ;
				oDocRange.setEnd( oRange.endContainer, oRange.endOffset ) ;
			}
			else
				throw( e ) ;
		}

		var oSel = this.Window.getSelection() ;
		oSel.removeAllRanges() ;

		// We must add a clone otherwise Firefox will have rendering issues.
		oSel.addRange( oDocRange ) ;
	}
}

// Not compatible with bookmark created with CreateBookmark2.
// The bookmark nodes will be deleted from the document.
FCKDomRange.prototype.SelectBookmark = function( bookmark )
{
	var domRange = this.Window.document.createRange() ;

	var startNode	= this.GetBookmarkNode( bookmark, true ) ;
	var endNode		= this.GetBookmarkNode( bookmark, false ) ;

	domRange.setStart( startNode.parentNode, FCKDomTools.GetIndexOf( startNode ) ) ;
	FCKDomTools.RemoveNode( startNode ) ;

	if ( endNode )
	{
		domRange.setEnd( endNode.parentNode, FCKDomTools.GetIndexOf( endNode ) ) ;
		FCKDomTools.RemoveNode( endNode ) ;
	}

	var selection = this.Window.getSelection() ;
	selection.removeAllRanges() ;
	selection.addRange( domRange ) ;
}
