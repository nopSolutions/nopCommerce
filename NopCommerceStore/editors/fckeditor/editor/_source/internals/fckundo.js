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
 */

var FCKUndo = new Object() ;

FCKUndo.SavedData = new Array() ;
FCKUndo.CurrentIndex = -1 ;
FCKUndo.TypesCount = 0 ;
FCKUndo.Changed = false ;	// Is the document changed in respect to its initial image?
FCKUndo.MaxTypes = 25 ;
FCKUndo.Typing = false ;
FCKUndo.SaveLocked = false ;

FCKUndo._GetBookmark = function()
{
	FCKSelection.Restore() ;

	var range = new FCKDomRange( FCK.EditorWindow ) ;
	try
	{
		// There are some tricky cases where this might fail (e.g. having a lone empty table in IE)
		range.MoveToSelection() ;
	}
	catch ( e )
	{
		return null ;
	}
	if ( FCKBrowserInfo.IsIE )
	{
		var bookmark = range.CreateBookmark() ;
		var dirtyHtml = FCK.EditorDocument.body.innerHTML ;
		range.MoveToBookmark( bookmark ) ;
		return [ bookmark, dirtyHtml ] ;
	}
	return range.CreateBookmark2() ;
}

FCKUndo._SelectBookmark = function( bookmark )
{
	if ( ! bookmark )
		return ;

	var range = new FCKDomRange( FCK.EditorWindow ) ;
	if ( bookmark instanceof Object )
	{
		if ( FCKBrowserInfo.IsIE )
			range.MoveToBookmark( bookmark[0] ) ;
		else
			range.MoveToBookmark2( bookmark ) ;
		try
		{
			// this does not always succeed, there are still some tricky cases where it fails
			// e.g. add a special character at end of document, undo, redo -> error
			range.Select() ;
		}
		catch ( e )
		{
			// if select restore fails, put the caret at the end of the document
			range.MoveToPosition( FCK.EditorDocument.body, 4 ) ;
			range.Select() ;
		}
	}
}

FCKUndo._CompareCursors = function( cursor1, cursor2 )
{
	for ( var i = 0 ; i < Math.min( cursor1.length, cursor2.length ) ; i++ )
	{
		if ( cursor1[i] < cursor2[i] )
			return -1;
		else if (cursor1[i] > cursor2[i] )
			return 1;
	}
	if ( cursor1.length < cursor2.length )
		return -1;
	else if (cursor1.length > cursor2.length )
		return 1;
	return 0;
}

FCKUndo._CheckIsBookmarksEqual = function( bookmark1, bookmark2 )
{
	if ( ! ( bookmark1 && bookmark2 ) )
		return false ;
	if ( FCKBrowserInfo.IsIE )
	{
		var startOffset1 = bookmark1[1].search( bookmark1[0].StartId ) ;
		var startOffset2 = bookmark2[1].search( bookmark2[0].StartId ) ;
		var endOffset1 = bookmark1[1].search( bookmark1[0].EndId ) ;
		var endOffset2 = bookmark2[1].search( bookmark2[0].EndId ) ;
		return startOffset1 == startOffset2 && endOffset1 == endOffset2 ;
	}
	else
	{
		return this._CompareCursors( bookmark1.Start, bookmark2.Start ) == 0
			&& this._CompareCursors( bookmark1.End, bookmark2.End ) == 0 ;
	}
}

FCKUndo.SaveUndoStep = function()
{
	if ( FCK.EditMode != FCK_EDITMODE_WYSIWYG || this.SaveLocked )
		return ;

	// Assume the editor content is changed when SaveUndoStep() is called after the first time.
	// This also enables the undo button in toolbar.
	if ( this.SavedData.length )
		this.Changed = true ;

	// Get the HTML content.
	var sHtml = FCK.EditorDocument.body.innerHTML ;
	var bookmark = this._GetBookmark() ;

	// Shrink the array to the current level.
	this.SavedData = this.SavedData.slice( 0, this.CurrentIndex + 1 ) ;

	// Cancel operation if the new step is identical to the previous one.
	if ( this.CurrentIndex > 0
			&& sHtml == this.SavedData[ this.CurrentIndex ][0]
			&& this._CheckIsBookmarksEqual( bookmark, this.SavedData[ this.CurrentIndex ][1] ) )
		return ;
	// Save the selection and caret position in the first undo level for the first change.
	else if ( this.CurrentIndex == 0 && this.SavedData.length && sHtml == this.SavedData[0][0] )
	{
		this.SavedData[0][1] = bookmark ;
		return ;
	}

	// If we reach the Maximum number of undo levels, we must remove the first
	// entry of the list shifting all elements.
	if ( this.CurrentIndex + 1 >= FCKConfig.MaxUndoLevels )
		this.SavedData.shift() ;
	else
		this.CurrentIndex++ ;

	// Save the new level in front of the actual position.
	this.SavedData[ this.CurrentIndex ] = [ sHtml, bookmark ] ;

	FCK.Events.FireEvent( "OnSelectionChange" ) ;
}

FCKUndo.CheckUndoState = function()
{
	return ( this.Changed || this.CurrentIndex > 0 ) ;
}

FCKUndo.CheckRedoState = function()
{
	return ( this.CurrentIndex < ( this.SavedData.length - 1 ) ) ;
}

FCKUndo.Undo = function()
{
	if ( this.CheckUndoState() )
	{
		// If it is the first step.
		if ( this.CurrentIndex == ( this.SavedData.length - 1 ) )
		{
			// Save the actual state for a possible "Redo" call.
			this.SaveUndoStep() ;
		}

		// Go a step back.
		this._ApplyUndoLevel( --this.CurrentIndex ) ;

		FCK.Events.FireEvent( "OnSelectionChange" ) ;
	}
}

FCKUndo.Redo = function()
{
	if ( this.CheckRedoState() )
	{
		// Go a step forward.
		this._ApplyUndoLevel( ++this.CurrentIndex ) ;

		FCK.Events.FireEvent( "OnSelectionChange" ) ;
	}
}

FCKUndo._ApplyUndoLevel = function( level )
{
	var oData = this.SavedData[ level ] ;

	if ( !oData )
		return ;

	// Update the editor contents with that step data.
	if ( FCKBrowserInfo.IsIE )
	{
		if ( oData[1] && oData[1][1] )
			FCK.SetInnerHtml( oData[1][1] ) ;
		else
			FCK.SetInnerHtml( oData[0] ) ;
	}
	else
		FCK.EditorDocument.body.innerHTML = oData[0] ;

	// Restore the selection
	this._SelectBookmark( oData[1] ) ;

	this.TypesCount = 0 ;
	this.Changed = false ;
	this.Typing = false ;
}
