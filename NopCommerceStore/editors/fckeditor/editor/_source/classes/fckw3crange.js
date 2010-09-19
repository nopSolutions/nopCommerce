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
 * This class partially implements the W3C DOM Range for browser that don't
 * support the standards (like IE):
 * http://www.w3.org/TR/DOM-Level-2-Traversal-Range/ranges.html
 */

var FCKW3CRange = function( parentDocument )
{
	this._Document = parentDocument ;

	this.startContainer	= null ;
	this.startOffset	= null ;
	this.endContainer	= null ;
	this.endOffset		= null ;
	this.collapsed		= true ;
}

FCKW3CRange.CreateRange = function( parentDocument )
{
	// We could opt to use the Range implementation of the browsers. The problem
	// is that every browser have different bugs on their implementations,
	// mostly related to different interpretations of the W3C specifications.
	// So, for now, let's use our implementation and pray for browsers fixings
	// soon. Otherwise will go crazy on trying to find out workarounds.
	/*
	// Get the browser implementation of the range, if available.
	if ( parentDocument.createRange )
	{
		var range = parentDocument.createRange() ;
		if ( typeof( range.startContainer ) != 'undefined' )
			return range ;
	}
	*/
	return new FCKW3CRange( parentDocument ) ;
}

FCKW3CRange.CreateFromRange = function( parentDocument, sourceRange )
{
	var range = FCKW3CRange.CreateRange( parentDocument ) ;
	range.setStart( sourceRange.startContainer, sourceRange.startOffset ) ;
	range.setEnd( sourceRange.endContainer, sourceRange.endOffset ) ;
	return range ;
}

FCKW3CRange.prototype =
{

	_UpdateCollapsed : function()
	{
      this.collapsed = ( this.startContainer == this.endContainer && this.startOffset == this.endOffset ) ;
	},

	// W3C requires a check for the new position. If it is after the end
	// boundary, the range should be collapsed to the new start. It seams we
	// will not need this check for our use of this class so we can ignore it for now.
	setStart : function( refNode, offset )
	{
		this.startContainer	= refNode ;
		this.startOffset	= offset ;

		if ( !this.endContainer )
		{
			this.endContainer	= refNode ;
			this.endOffset		= offset ;
		}

		this._UpdateCollapsed() ;
	},

	// W3C requires a check for the new position. If it is before the start
	// boundary, the range should be collapsed to the new end. It seams we
	// will not need this check for our use of this class so we can ignore it for now.
	setEnd : function( refNode, offset )
	{
		this.endContainer	= refNode ;
		this.endOffset		= offset ;

		if ( !this.startContainer )
		{
			this.startContainer	= refNode ;
			this.startOffset	= offset ;
		}

		this._UpdateCollapsed() ;
	},

	setStartAfter : function( refNode )
	{
		this.setStart( refNode.parentNode, FCKDomTools.GetIndexOf( refNode ) + 1 ) ;
	},

	setStartBefore : function( refNode )
	{
		this.setStart( refNode.parentNode, FCKDomTools.GetIndexOf( refNode ) ) ;
	},

	setEndAfter : function( refNode )
	{
		this.setEnd( refNode.parentNode, FCKDomTools.GetIndexOf( refNode ) + 1 ) ;
	},

	setEndBefore : function( refNode )
	{
		this.setEnd( refNode.parentNode, FCKDomTools.GetIndexOf( refNode ) ) ;
	},

	collapse : function( toStart )
	{
		if ( toStart )
		{
			this.endContainer	= this.startContainer ;
			this.endOffset		= this.startOffset ;
		}
		else
		{
			this.startContainer	= this.endContainer ;
			this.startOffset	= this.endOffset ;
		}

		this.collapsed = true ;
	},

	selectNodeContents : function( refNode )
	{
		this.setStart( refNode, 0 ) ;
		this.setEnd( refNode, refNode.nodeType == 3 ? refNode.data.length : refNode.childNodes.length ) ;
	},

	insertNode : function( newNode )
	{
		var startContainer = this.startContainer ;
		var startOffset = this.startOffset ;

		// If we are in a text node.
		if ( startContainer.nodeType == 3 )
		{
			startContainer.splitText( startOffset ) ;

			// Check if it is necessary to update the end boundary.
			if ( startContainer == this.endContainer )
				this.setEnd( startContainer.nextSibling, this.endOffset - this.startOffset ) ;

			// Insert the new node it after the text node.
			FCKDomTools.InsertAfterNode( startContainer, newNode ) ;

			return ;
		}
		else
		{
			// Simply insert the new node before the current start node.
			startContainer.insertBefore( newNode, startContainer.childNodes[ startOffset ] || null ) ;

			// Check if it is necessary to update the end boundary.
			if ( startContainer == this.endContainer )
			{
				this.endOffset++ ;
				this.collapsed = false ;
			}
		}
	},

	deleteContents : function()
	{
		if ( this.collapsed )
			return ;

		this._ExecContentsAction( 0 ) ;
	},

	extractContents : function()
	{
		var docFrag = new FCKDocumentFragment( this._Document ) ;

		if ( !this.collapsed )
			this._ExecContentsAction( 1, docFrag ) ;

		return docFrag ;
	},

	// The selection may be lost when cloning (due to the splitText() call).
	cloneContents : function()
	{
		var docFrag = new FCKDocumentFragment( this._Document ) ;

		if ( !this.collapsed )
			this._ExecContentsAction( 2, docFrag ) ;

		return docFrag ;
	},

	_ExecContentsAction : function( action, docFrag )
	{
		var startNode	= this.startContainer ;
		var endNode		= this.endContainer ;

		var startOffset	= this.startOffset ;
		var endOffset	= this.endOffset ;

		var removeStartNode	= false ;
		var removeEndNode	= false ;

		// Check the start and end nodes and make the necessary removals or changes.

		// Start from the end, otherwise DOM mutations (splitText) made in the
		// start boundary may interfere on the results here.

		// For text containers, we must simply split the node and point to the
		// second part. The removal will be handled by the rest of the code .
		if ( endNode.nodeType == 3 )
			endNode = endNode.splitText( endOffset ) ;
		else
		{
			// If the end container has children and the offset is pointing
			// to a child, then we should start from it.
			if ( endNode.childNodes.length > 0 )
			{
				// If the offset points after the last node.
				if ( endOffset > endNode.childNodes.length - 1 )
				{
					// Let's create a temporary node and mark it for removal.
					endNode = FCKDomTools.InsertAfterNode( endNode.lastChild, this._Document.createTextNode('') ) ;
					removeEndNode = true ;
				}
				else
					endNode = endNode.childNodes[ endOffset ] ;
			}
		}

		// For text containers, we must simply split the node. The removal will
		// be handled by the rest of the code .
		if ( startNode.nodeType == 3 )
		{
			startNode.splitText( startOffset ) ;

			// In cases the end node is the same as the start node, the above
			// splitting will also split the end, so me must move the end to
			// the second part of the split.
			if ( startNode == endNode )
				endNode = startNode.nextSibling ;
		}
		else
		{
			// If the start container has children and the offset is pointing
			// to a child, then we should start from its previous sibling.

			// If the offset points to the first node, we don't have a
			// sibling, so let's use the first one, but mark it for removal.
			if ( startOffset == 0 )
			{
				// Let's create a temporary node and mark it for removal.
				startNode = startNode.insertBefore( this._Document.createTextNode(''), startNode.firstChild ) ;
				removeStartNode = true ;
			}
			else if ( startOffset > startNode.childNodes.length - 1 )
			{
				// Let's create a temporary node and mark it for removal.
				startNode = startNode.appendChild( this._Document.createTextNode('') ) ;
				removeStartNode = true ;
			}
			else
				startNode = startNode.childNodes[ startOffset ].previousSibling ;
		}

		// Get the parent nodes tree for the start and end boundaries.
		var startParents	= FCKDomTools.GetParents( startNode ) ;
		var endParents		= FCKDomTools.GetParents( endNode ) ;

		// Compare them, to find the top most siblings.
		var i, topStart, topEnd ;

		for ( i = 0 ; i < startParents.length ; i++ )
		{
			topStart	= startParents[i] ;
			topEnd		= endParents[i] ;

			// The compared nodes will match until we find the top most
			// siblings (different nodes that have the same parent).
			// "i" will hold the index in the parents array for the top
			// most element.
			if ( topStart != topEnd )
				break ;
		}

		var clone, levelStartNode, levelClone, currentNode, currentSibling ;

		if ( docFrag )
			clone = docFrag.RootNode ;

		// Remove all successive sibling nodes for every node in the
		// startParents tree.
		for ( var j = i ; j < startParents.length ; j++ )
		{
			levelStartNode = startParents[j] ;

			// For Extract and Clone, we must clone this level.
			if ( clone && levelStartNode != startNode )		// action = 0 = Delete
				levelClone = clone.appendChild( levelStartNode.cloneNode( levelStartNode == startNode ) ) ;

			currentNode = levelStartNode.nextSibling ;

			while( currentNode )
			{
				// Stop processing when the current node matches a node in the
				// endParents tree or if it is the endNode.
				if ( currentNode == endParents[j] || currentNode == endNode )
					break ;

				// Cache the next sibling.
				currentSibling = currentNode.nextSibling ;

				// If cloning, just clone it.
				if ( action == 2 )	// 2 = Clone
					clone.appendChild( currentNode.cloneNode( true ) ) ;
				else
				{
					// Both Delete and Extract will remove the node.
					currentNode.parentNode.removeChild( currentNode ) ;

					// When Extracting, move the removed node to the docFrag.
					if ( action == 1 )	// 1 = Extract
						clone.appendChild( currentNode ) ;
				}

				currentNode = currentSibling ;
			}

			if ( clone )
				clone = levelClone ;
		}

		if ( docFrag )
			clone = docFrag.RootNode ;

		// Remove all previous sibling nodes for every node in the
		// endParents tree.
		for ( var k = i ; k < endParents.length ; k++ )
		{
			levelStartNode = endParents[k] ;

			// For Extract and Clone, we must clone this level.
			if ( action > 0 && levelStartNode != endNode )		// action = 0 = Delete
				levelClone = clone.appendChild( levelStartNode.cloneNode( levelStartNode == endNode ) ) ;

			// The processing of siblings may have already been done by the parent.
			if ( !startParents[k] || levelStartNode.parentNode != startParents[k].parentNode )
			{
				currentNode = levelStartNode.previousSibling ;

				while( currentNode )
				{
					// Stop processing when the current node matches a node in the
					// startParents tree or if it is the startNode.
					if ( currentNode == startParents[k] || currentNode == startNode )
						break ;

					// Cache the next sibling.
					currentSibling = currentNode.previousSibling ;

					// If cloning, just clone it.
					if ( action == 2 )	// 2 = Clone
						clone.insertBefore( currentNode.cloneNode( true ), clone.firstChild ) ;
					else
					{
						// Both Delete and Extract will remove the node.
						currentNode.parentNode.removeChild( currentNode ) ;

						// When Extracting, mode the removed node to the docFrag.
						if ( action == 1 )	// 1 = Extract
							clone.insertBefore( currentNode, clone.firstChild ) ;
					}

					currentNode = currentSibling ;
				}
			}

			if ( clone )
				clone = levelClone ;
		}

		if ( action == 2 )		// 2 = Clone.
		{
			// No changes in the DOM should be done, so fix the split text (if any).

			var startTextNode = this.startContainer ;
			if ( startTextNode.nodeType == 3 )
			{
				startTextNode.data += startTextNode.nextSibling.data ;
				startTextNode.parentNode.removeChild( startTextNode.nextSibling ) ;
			}

			var endTextNode = this.endContainer ;
			if ( endTextNode.nodeType == 3 && endTextNode.nextSibling )
			{
				endTextNode.data += endTextNode.nextSibling.data ;
				endTextNode.parentNode.removeChild( endTextNode.nextSibling ) ;
			}
		}
		else
		{
			// Collapse the range.

			// If a node has been partially selected, collapse the range between
			// topStart and topEnd. Otherwise, simply collapse it to the start. (W3C specs).
			if ( topStart && topEnd && ( startNode.parentNode != topStart.parentNode || endNode.parentNode != topEnd.parentNode ) )
			{
				var endIndex = FCKDomTools.GetIndexOf( topEnd ) ;

				// If the start node is to be removed, we must correct the
				// index to reflect the removal.
				if ( removeStartNode && topEnd.parentNode == startNode.parentNode )
					endIndex-- ;

				this.setStart( topEnd.parentNode, endIndex ) ;
			}

			// Collapse it to the start.
			this.collapse( true ) ;
		}

		// Cleanup any marked node.
		if( removeStartNode )
			startNode.parentNode.removeChild( startNode ) ;

		if( removeEndNode && endNode.parentNode )
			endNode.parentNode.removeChild( endNode ) ;
	},

	cloneRange : function()
	{
		return FCKW3CRange.CreateFromRange( this._Document, this ) ;
	}
} ;
