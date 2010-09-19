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
 * Implementation for the "Insert/Remove Ordered/Unordered List" commands.
 */

var FCKListCommand = function( name, tagName )
{
	this.Name = name ;
	this.TagName = tagName ;
}

FCKListCommand.prototype =
{
	GetState : function()
	{
		// Disabled if not WYSIWYG.
		if ( FCK.EditMode != FCK_EDITMODE_WYSIWYG || ! FCK.EditorWindow )
			return FCK_TRISTATE_DISABLED ;

		// We'll use the style system's convention to determine list state here...
		// If the starting block is a descendant of an <ol> or <ul> node, then we're in a list.
		var startContainer = FCKSelection.GetBoundaryParentElement( true ) ;
		var listNode = startContainer ;
		while ( listNode )
		{
			if ( listNode.nodeName.IEquals( [ 'ul', 'ol' ] ) )
				break ;
			listNode = listNode.parentNode ;
		}
		if ( listNode && listNode.nodeName.IEquals( this.TagName ) )
			return FCK_TRISTATE_ON ;
		else
			return FCK_TRISTATE_OFF ;
	},

	Execute : function()
	{
		FCKUndo.SaveUndoStep() ;

		var doc = FCK.EditorDocument ;
		var range = new FCKDomRange( FCK.EditorWindow ) ;
		range.MoveToSelection() ;
		var state = this.GetState() ;

		// Midas lists rule #1 says we can create a list even in an empty document.
		// But FCKDomRangeIterator wouldn't run if the document is really empty.
		// So create a paragraph if the document is empty and we're going to create a list.
		if ( state == FCK_TRISTATE_OFF )
		{
			FCKDomTools.TrimNode( doc.body ) ;
			if ( ! doc.body.firstChild )
			{
				var paragraph = doc.createElement( 'p' ) ;
				doc.body.appendChild( paragraph ) ;
				range.MoveToNodeContents( paragraph ) ;
			}
		}

		var bookmark = range.CreateBookmark() ;

		// Group the blocks up because there are many cases where multiple lists have to be created,
		// or multiple lists have to be cancelled.
		var listGroups = [] ;
		var markerObj = {} ;
		var iterator = new FCKDomRangeIterator( range ) ;
		var block ;

		iterator.ForceBrBreak = ( state == FCK_TRISTATE_OFF ) ;
		var nextRangeExists = true ;
		var rangeQueue = null ;
		while ( nextRangeExists )
		{
			while ( ( block = iterator.GetNextParagraph() ) )
			{
				var path = new FCKElementPath( block ) ;
				var listNode = null ;
				var processedFlag = false ;
				var blockLimit = path.BlockLimit ;

				// First, try to group by a list ancestor.
				for ( var i = path.Elements.length - 1 ; i >= 0 ; i-- )
				{
					var el = path.Elements[i] ;
					if ( el.nodeName.IEquals( ['ol', 'ul'] ) )
					{
						// If we've encountered a list inside a block limit
						// The last group object of the block limit element should
						// no longer be valid. Since paragraphs after the list
						// should belong to a different group of paragraphs before
						// the list. (Bug #1309)
						if ( blockLimit._FCK_ListGroupObject )
							blockLimit._FCK_ListGroupObject = null ;

						var groupObj = el._FCK_ListGroupObject ;
						if ( groupObj )
							groupObj.contents.push( block ) ;
						else
						{
							groupObj = { 'root' : el, 'contents' : [ block ] } ;
							listGroups.push( groupObj ) ;
							FCKDomTools.SetElementMarker( markerObj, el, '_FCK_ListGroupObject', groupObj ) ;
						}
						processedFlag = true ;
						break ;
					}
				}

				if ( processedFlag )
					continue ;

				// No list ancestor? Group by block limit.
				var root = blockLimit ;
				if ( root._FCK_ListGroupObject )
					root._FCK_ListGroupObject.contents.push( block ) ;
				else
				{
					var groupObj = { 'root' : root, 'contents' : [ block ] } ;
					FCKDomTools.SetElementMarker( markerObj, root, '_FCK_ListGroupObject', groupObj ) ;
					listGroups.push( groupObj ) ;
				}
			}

			if ( FCKBrowserInfo.IsIE )
				nextRangeExists = false ;
			else
			{
				if ( rangeQueue == null )
				{
					rangeQueue = [] ;
					var selectionObject = FCKSelection.GetSelection() ;
					if ( selectionObject && listGroups.length == 0 )
						rangeQueue.push( selectionObject.getRangeAt( 0 ) ) ;
					for ( var i = 1 ; selectionObject && i < selectionObject.rangeCount ; i++ )
						rangeQueue.push( selectionObject.getRangeAt( i ) ) ;
				}
				if ( rangeQueue.length < 1 )
					nextRangeExists = false ;
				else
				{
					var internalRange = FCKW3CRange.CreateFromRange( doc, rangeQueue.shift() ) ;
					range._Range = internalRange ;
					range._UpdateElementInfo() ;
					if ( range.StartNode.nodeName.IEquals( 'td' ) )
						range.SetStart( range.StartNode, 1 ) ;
					if ( range.EndNode.nodeName.IEquals( 'td' ) )
						range.SetEnd( range.EndNode, 2 ) ;
					iterator = new FCKDomRangeIterator( range ) ;
					iterator.ForceBrBreak = ( state == FCK_TRISTATE_OFF ) ;
				}
			}
		}

		// Now we have two kinds of list groups, groups rooted at a list, and groups rooted at a block limit element.
		// We either have to build lists or remove lists, for removing a list does not makes sense when we are looking
		// at the group that's not rooted at lists. So we have three cases to handle.
		var listsCreated = [] ;
		while ( listGroups.length > 0 )
		{
			var groupObj = listGroups.shift() ;
			if ( state == FCK_TRISTATE_OFF )
			{
				if ( groupObj.root.nodeName.IEquals( ['ul', 'ol'] ) )
					this._ChangeListType( groupObj, markerObj, listsCreated ) ;
				else
					this._CreateList( groupObj, listsCreated ) ;
			}
			else if ( state == FCK_TRISTATE_ON && groupObj.root.nodeName.IEquals( ['ul', 'ol'] ) )
				this._RemoveList( groupObj, markerObj ) ;
		}

		// For all new lists created, merge adjacent, same type lists.
		for ( var i = 0 ; i < listsCreated.length ; i++ )
		{
			var listNode = listsCreated[i] ;
			var stopFlag = false ;
			var currentNode = listNode ;
			while ( ! stopFlag )
			{
				currentNode = currentNode.nextSibling ;
				if ( currentNode && currentNode.nodeType == 3 && currentNode.nodeValue.search( /^[\n\r\t ]*$/ ) == 0 )
					continue ;
				stopFlag = true ;
			}

			if ( currentNode && currentNode.nodeName.IEquals( this.TagName ) )
			{
				currentNode.parentNode.removeChild( currentNode ) ;
				while ( currentNode.firstChild )
					listNode.appendChild( currentNode.removeChild( currentNode.firstChild ) ) ;
			}

			stopFlag = false ;
			currentNode = listNode ;
			while ( ! stopFlag )
			{
				currentNode = currentNode.previousSibling ;
				if ( currentNode && currentNode.nodeType == 3 && currentNode.nodeValue.search( /^[\n\r\t ]*$/ ) == 0 )
					continue ;
				stopFlag = true ;
			}
			if ( currentNode && currentNode.nodeName.IEquals( this.TagName ) )
			{
				currentNode.parentNode.removeChild( currentNode ) ;
				while ( currentNode.lastChild )
					listNode.insertBefore( currentNode.removeChild( currentNode.lastChild ),
						       listNode.firstChild ) ;
			}
		}

		// Clean up, restore selection and update toolbar button states.
		FCKDomTools.ClearAllMarkers( markerObj ) ;
		range.MoveToBookmark( bookmark ) ;
		range.Select() ;

		FCK.Focus() ;
		FCK.Events.FireEvent( 'OnSelectionChange' ) ;
	},

	_ChangeListType : function( groupObj, markerObj, listsCreated )
	{
		// This case is easy...
		// 1. Convert the whole list into a one-dimensional array.
		// 2. Change the list type by modifying the array.
		// 3. Recreate the whole list by converting the array to a list.
		// 4. Replace the original list with the recreated list.
		var listArray = FCKDomTools.ListToArray( groupObj.root, markerObj ) ;
		var selectedListItems = [] ;
		for ( var i = 0 ; i < groupObj.contents.length ; i++ )
		{
			var itemNode = groupObj.contents[i] ;
			itemNode = FCKTools.GetElementAscensor( itemNode, 'li' ) ;
			if ( ! itemNode || itemNode._FCK_ListItem_Processed )
				continue ;
			selectedListItems.push( itemNode ) ;
			FCKDomTools.SetElementMarker( markerObj, itemNode, '_FCK_ListItem_Processed', true ) ;
		}
		var fakeParent = FCKTools.GetElementDocument( groupObj.root ).createElement( this.TagName ) ;
		for ( var i = 0 ; i < selectedListItems.length ; i++ )
		{
			var listIndex = selectedListItems[i]._FCK_ListArray_Index ;
			listArray[listIndex].parent = fakeParent ;
		}
		var newList = FCKDomTools.ArrayToList( listArray, markerObj ) ;
		for ( var i = 0 ; i < newList.listNode.childNodes.length ; i++ )
		{
			if ( newList.listNode.childNodes[i].nodeName.IEquals( this.TagName ) )
				listsCreated.push( newList.listNode.childNodes[i] ) ;
		}
		groupObj.root.parentNode.replaceChild( newList.listNode, groupObj.root ) ;
	},

	_CreateList : function( groupObj, listsCreated )
	{
		var contents = groupObj.contents ;
		var doc = FCKTools.GetElementDocument( groupObj.root ) ;
		var listContents = [] ;

		// It is possible to have the contents returned by DomRangeIterator to be the same as the root.
		// e.g. when we're running into table cells.
		// In such a case, enclose the childNodes of contents[0] into a <div>.
		if ( contents.length == 1 && contents[0] == groupObj.root )
		{
			var divBlock = doc.createElement( 'div' );
			while ( contents[0].firstChild )
				divBlock.appendChild( contents[0].removeChild( contents[0].firstChild ) ) ;
			contents[0].appendChild( divBlock ) ;
			contents[0] = divBlock ;
		}

		// Calculate the common parent node of all content blocks.
		var commonParent = groupObj.contents[0].parentNode ;
		for ( var i = 0 ; i < contents.length ; i++ )
			commonParent = FCKDomTools.GetCommonParents( commonParent, contents[i].parentNode ).pop() ;

		// We want to insert things that are in the same tree level only, so calculate the contents again
		// by expanding the selected blocks to the same tree level.
		for ( var i = 0 ; i < contents.length ; i++ )
		{
			var contentNode = contents[i] ;
			while ( contentNode.parentNode )
			{
				if ( contentNode.parentNode == commonParent )
				{
					listContents.push( contentNode ) ;
					break ;
				}
				contentNode = contentNode.parentNode ;
			}
		}

		if ( listContents.length < 1 )
			return ;

		// Insert the list to the DOM tree.
		var insertAnchor = listContents[listContents.length - 1].nextSibling ;
		var listNode = doc.createElement( this.TagName ) ;
		listsCreated.push( listNode ) ;
		while ( listContents.length )
		{
			var contentBlock = listContents.shift() ;
			var docFrag = doc.createDocumentFragment() ;
			while ( contentBlock.firstChild )
				docFrag.appendChild( contentBlock.removeChild( contentBlock.firstChild ) ) ;
			contentBlock.parentNode.removeChild( contentBlock ) ;
			var listItem = doc.createElement( 'li' ) ;
			listItem.appendChild( docFrag ) ;
			listNode.appendChild( listItem ) ;
		}
		commonParent.insertBefore( listNode, insertAnchor ) ;
	},

	_RemoveList : function( groupObj, markerObj )
	{
		// This is very much like the change list type operation.
		// Except that we're changing the selected items' indent to -1 in the list array.
		var listArray = FCKDomTools.ListToArray( groupObj.root, markerObj ) ;
		var selectedListItems = [] ;
		for ( var i = 0 ; i < groupObj.contents.length ; i++ )
		{
			var itemNode = groupObj.contents[i] ;
			itemNode = FCKTools.GetElementAscensor( itemNode, 'li' ) ;
			if ( ! itemNode || itemNode._FCK_ListItem_Processed )
				continue ;
			selectedListItems.push( itemNode ) ;
			FCKDomTools.SetElementMarker( markerObj, itemNode, '_FCK_ListItem_Processed', true ) ;
		}

		var lastListIndex = null ;
		for ( var i = 0 ; i < selectedListItems.length ; i++ )
		{
			var listIndex = selectedListItems[i]._FCK_ListArray_Index ;
			listArray[listIndex].indent = -1 ;
			lastListIndex = listIndex ;
		}

		// After cutting parts of the list out with indent=-1, we still have to maintain the array list
		// model's nextItem.indent <= currentItem.indent + 1 invariant. Otherwise the array model of the
		// list cannot be converted back to a real DOM list.
		for ( var i = lastListIndex + 1; i < listArray.length ; i++ )
		{
			if ( listArray[i].indent > listArray[i-1].indent + 1 )
			{
				var indentOffset = listArray[i-1].indent + 1 - listArray[i].indent ;
				var oldIndent = listArray[i].indent ;
				while ( listArray[i] && listArray[i].indent >= oldIndent)
				{
					listArray[i].indent += indentOffset ;
					i++ ;
				}
				i-- ;
			}
		}

		var newList = FCKDomTools.ArrayToList( listArray, markerObj ) ;
		// If groupObj.root is the last element in its parent, or its nextSibling is a <br>, then we should
		// not add a <br> after the final item. So, check for the cases and trim the <br>.
		if ( groupObj.root.nextSibling == null || groupObj.root.nextSibling.nodeName.IEquals( 'br' ) )
		{
			if ( newList.listNode.lastChild.nodeName.IEquals( 'br' ) )
				newList.listNode.removeChild( newList.listNode.lastChild ) ;
		}
		groupObj.root.parentNode.replaceChild( newList.listNode, groupObj.root ) ;
	}
};
