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
 * Tool object to manage HTML lists items (UL, OL and LI).
 */

var FCKListHandler =
{
	OutdentListItem : function( listItem )
	{
		var eParent = listItem.parentNode ;

		// It may happen that a LI is not in a UL or OL (Orphan).
		if ( eParent.tagName.toUpperCase().Equals( 'UL','OL' ) )
		{
			var oDocument = FCKTools.GetElementDocument( listItem ) ;
			var oDogFrag = new FCKDocumentFragment( oDocument ) ;

			// All children and successive siblings will be moved to a a DocFrag.
			var eNextSiblings = oDogFrag.RootNode ;
			var eHasLiSibling = false ;

			// If we have nested lists inside it, let's move it to the list of siblings.
			var eChildList = FCKDomTools.GetFirstChild( listItem, ['UL','OL'] ) ;
			if ( eChildList )
			{
				eHasLiSibling = true ;

				var eChild ;
				// The extra () is to avoid a warning with strict error checking. This is ok.
				while ( (eChild = eChildList.firstChild) )
					eNextSiblings.appendChild( eChildList.removeChild( eChild ) ) ;

				FCKDomTools.RemoveNode( eChildList ) ;
			}

			// Move all successive siblings.
			var eSibling ;
			var eHasSuccessiveLiSibling = false ;
			// The extra () is to avoid a warning with strict error checking. This is ok.
			while ( (eSibling = listItem.nextSibling) )
			{
				if ( !eHasLiSibling && eSibling.nodeType == 1 && eSibling.nodeName.toUpperCase() == 'LI' )
					eHasSuccessiveLiSibling = eHasLiSibling = true ;

				eNextSiblings.appendChild( eSibling.parentNode.removeChild( eSibling ) ) ;

				// If a sibling is a incorrectly nested UL or OL, consider only its children.
				if ( !eHasSuccessiveLiSibling && eSibling.nodeType == 1 && eSibling.nodeName.toUpperCase().Equals( 'UL','OL' ) )
					FCKDomTools.RemoveNode( eSibling, true ) ;
			}

			// If we are in a list chain.
			var sParentParentTag = eParent.parentNode.tagName.toUpperCase() ;
			var bWellNested = ( sParentParentTag == 'LI' ) ;
			if ( bWellNested || sParentParentTag.Equals( 'UL','OL' ) )
			{
				if ( eHasLiSibling )
				{
					var eChildList = eParent.cloneNode( false ) ;
					oDogFrag.AppendTo( eChildList ) ;
					listItem.appendChild( eChildList ) ;
				}
				else if ( bWellNested )
					oDogFrag.InsertAfterNode( eParent.parentNode ) ;
				else
					oDogFrag.InsertAfterNode( eParent ) ;

				// Move the LI after its parent.parentNode (the upper LI in the hierarchy).
				if ( bWellNested )
					FCKDomTools.InsertAfterNode( eParent.parentNode, eParent.removeChild( listItem ) ) ;
				else
					FCKDomTools.InsertAfterNode( eParent, eParent.removeChild( listItem ) ) ;
			}
			else
			{
				if ( eHasLiSibling )
				{
					var eNextList = eParent.cloneNode( false ) ;
					oDogFrag.AppendTo( eNextList ) ;
					FCKDomTools.InsertAfterNode( eParent, eNextList ) ;
				}

				var eBlock = oDocument.createElement( FCKConfig.EnterMode == 'p' ? 'p' : 'div' ) ;
				FCKDomTools.MoveChildren( eParent.removeChild( listItem ), eBlock ) ;
				FCKDomTools.InsertAfterNode( eParent, eBlock ) ;

				if ( FCKConfig.EnterMode == 'br' )
				{
					// We need the bogus to make it work properly. In Gecko, we
					// need it before the new block, on IE, after it.
					if ( FCKBrowserInfo.IsGecko )
						eBlock.parentNode.insertBefore( FCKTools.CreateBogusBR( oDocument ), eBlock ) ;
					else
						FCKDomTools.InsertAfterNode( eBlock, FCKTools.CreateBogusBR( oDocument ) ) ;

					FCKDomTools.RemoveNode( eBlock, true ) ;
				}
			}

			if ( this.CheckEmptyList( eParent ) )
				FCKDomTools.RemoveNode( eParent, true ) ;
		}
	},

	CheckEmptyList : function( listElement )
	{
		return ( FCKDomTools.GetFirstChild( listElement, 'LI' ) == null ) ;
	},

	// Check if the list has contents (excluding nested lists).
	CheckListHasContents : function( listElement )
	{
		var eChildNode = listElement.firstChild ;

		while ( eChildNode )
		{
			switch ( eChildNode.nodeType )
			{
				case 1 :
					if ( !eChildNode.nodeName.IEquals( 'UL','LI' ) )
						return true ;
					break ;

				case 3 :
					if ( eChildNode.nodeValue.Trim().length > 0 )
						return true ;
			}

			eChildNode = eChildNode.nextSibling ;
		}

		return false ;
	}
} ;
