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
 * Manage table operations (IE specific).
 */

FCKTableHandler.GetSelectedCells = function()
{
	if ( FCKSelection.GetType() == 'Control' )
	{
		var td = FCKSelection.MoveToAncestorNode( ['TD', 'TH'] ) ;
		return td ? [ td ] : [] ;
	}

	var aCells = new Array() ;

	var oRange = FCKSelection.GetSelection().createRange() ;
//	var oParent = oRange.parentElement() ;
	var oParent = FCKSelection.GetParentElement() ;

	if ( oParent && oParent.tagName.Equals( 'TD', 'TH' ) )
		aCells[0] = oParent ;
	else
	{
		oParent = FCKSelection.MoveToAncestorNode( 'TABLE' ) ;

		if ( oParent )
		{
			// Loops throw all cells checking if the cell is, or part of it, is inside the selection
			// and then add it to the selected cells collection.
			for ( var i = 0 ; i < oParent.cells.length ; i++ )
			{
				var oCellRange = FCK.EditorDocument.body.createTextRange() ;
				oCellRange.moveToElementText( oParent.cells[i] ) ;

				if ( oRange.inRange( oCellRange )
					|| ( oRange.compareEndPoints('StartToStart',oCellRange) >= 0 &&  oRange.compareEndPoints('StartToEnd',oCellRange) <= 0 )
					|| ( oRange.compareEndPoints('EndToStart',oCellRange) >= 0 &&  oRange.compareEndPoints('EndToEnd',oCellRange) <= 0 ) )
				{
					aCells[aCells.length] = oParent.cells[i] ;
				}
			}
		}
	}

	return aCells ;
}
