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
 * (IE Implementation)
 */

FCKDomRange.prototype.MoveToSelection = function()
{
	this.Release( true ) ;

	this._Range = new FCKW3CRange( this.Window.document ) ;

	var oSel = this.Window.document.selection ;

	if ( oSel.type != 'Control' )
	{
		var eMarkerStart	= this._GetSelectionMarkerTag( true ) ;
		var eMarkerEnd		= this._GetSelectionMarkerTag( false ) ;

		if ( !eMarkerStart && !eMarkerEnd )
		{
			this._Range.setStart( this.Window.document.body, 0 ) ;
			this._UpdateElementInfo() ;
			return ;
		}

		// Set the start boundary.
		this._Range.setStart( eMarkerStart.parentNode, FCKDomTools.GetIndexOf( eMarkerStart ) ) ;
		eMarkerStart.parentNode.removeChild( eMarkerStart ) ;

		// Set the end boundary.
		this._Range.setEnd( eMarkerEnd.parentNode, FCKDomTools.GetIndexOf( eMarkerEnd ) ) ;
		eMarkerEnd.parentNode.removeChild( eMarkerEnd ) ;

		this._UpdateElementInfo() ;
	}
	else
	{
		var oControl = oSel.createRange().item(0) ;

		if ( oControl )
		{
			this._Range.setStartBefore( oControl ) ;
			this._Range.setEndAfter( oControl ) ;
			this._UpdateElementInfo() ;
		}
	}
}

FCKDomRange.prototype.Select = function( forceExpand )
{
	if ( this._Range )
		this.SelectBookmark( this.CreateBookmark( true ), forceExpand ) ;
}

// Not compatible with bookmark created with CreateBookmark2.
// The bookmark nodes will be deleted from the document.
FCKDomRange.prototype.SelectBookmark = function( bookmark, forceExpand )
{
	var bIsCollapsed = this.CheckIsCollapsed() ;
	var bIsStartMarkerAlone ;
	var dummySpan ;

	// Create marker tags for the start and end boundaries.
	var eStartMarker = this.GetBookmarkNode( bookmark, true ) ;

	if ( !eStartMarker )
		return ;

	var eEndMarker ;
	if ( !bIsCollapsed )
		eEndMarker = this.GetBookmarkNode( bookmark, false ) ;

	// Create the main range which will be used for the selection.
	var oIERange = this.Window.document.body.createTextRange() ;

	// Position the range at the start boundary.
	oIERange.moveToElementText( eStartMarker ) ;
	oIERange.moveStart( 'character', 1 ) ;

	if ( eEndMarker )
	{
		// Create a tool range for the end.
		var oIERangeEnd = this.Window.document.body.createTextRange() ;

		// Position the tool range at the end.
		oIERangeEnd.moveToElementText( eEndMarker ) ;

		// Move the end boundary of the main range to match the tool range.
		oIERange.setEndPoint( 'EndToEnd', oIERangeEnd ) ;
		oIERange.moveEnd( 'character', -1 ) ;
	}
	else
	{
		bIsStartMarkerAlone = forceExpand || !eStartMarker.previousSibling || eStartMarker.previousSibling.nodeName.toLowerCase() == 'br';

		// Append a temporary <span>&#65279;</span> before the selection.
		// This is needed to avoid IE destroying selections inside empty
		// inline elements, like <b></b> (#253).
		// It is also needed when placing the selection right after an inline
		// element to avoid the selection moving inside of it.
		dummySpan = this.Window.document.createElement( 'span' ) ;
		dummySpan.innerHTML = '&#65279;' ;	// Zero Width No-Break Space (U+FEFF). See #1359.
		eStartMarker.parentNode.insertBefore( dummySpan, eStartMarker ) ;

		if ( bIsStartMarkerAlone )
		{
			// To expand empty blocks or line spaces after <br>, we need
			// instead to have any char, which will be later deleted using the
			// selection.
			// \ufeff = Zero Width No-Break Space (U+FEFF). See #1359.
			eStartMarker.parentNode.insertBefore( this.Window.document.createTextNode( '\ufeff' ), eStartMarker ) ;
		}
	}

	if ( !this._Range )
		this._Range = this.CreateRange() ;

	// Remove the markers (reset the position, because of the changes in the DOM tree).
	this._Range.setStartBefore( eStartMarker ) ;
	eStartMarker.parentNode.removeChild( eStartMarker ) ;

	if ( bIsCollapsed )
	{
		if ( bIsStartMarkerAlone )
		{
			// Move the selection start to include the temporary &#65279;.
			oIERange.moveStart( 'character', -1 ) ;

			oIERange.select() ;

			// Remove our temporary stuff.
			this.Window.document.selection.clear() ;
		}
		else
			oIERange.select() ;

		FCKDomTools.RemoveNode( dummySpan ) ;
	}
	else
	{
		this._Range.setEndBefore( eEndMarker ) ;
		eEndMarker.parentNode.removeChild( eEndMarker ) ;
		oIERange.select() ;
	}
}

FCKDomRange.prototype._GetSelectionMarkerTag = function( toStart )
{
	var doc = this.Window.document ;
	var selection = doc.selection ;

	// Get a range for the start boundary.
	var oRange ;

	// IE may throw an "unspecified error" on some cases (it happened when
	// loading _samples/default.html), so try/catch.
	try
	{
		oRange = selection.createRange() ;
	}
	catch (e)
	{
		return null ;
	}

	// IE might take the range object to the main window instead of inside the editor iframe window.
	// This is known to happen when the editor window has not been selected before (See #933).
	// We need to avoid that.
	if ( oRange.parentElement().document != doc )
		return null ;

	oRange.collapse( toStart === true ) ;

	// Paste a marker element at the collapsed range and get it from the DOM.
	var sMarkerId = 'fck_dom_range_temp_' + (new Date()).valueOf() + '_' + Math.floor(Math.random()*1000) ;
	oRange.pasteHTML( '<span id="' + sMarkerId + '"></span>' ) ;

	return doc.getElementById( sMarkerId ) ;
}
