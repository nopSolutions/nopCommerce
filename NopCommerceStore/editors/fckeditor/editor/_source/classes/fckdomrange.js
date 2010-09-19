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
 */

var FCKDomRange = function( sourceWindow )
{
	this.Window = sourceWindow ;
	this._Cache = {} ;
}

FCKDomRange.prototype =
{

	_UpdateElementInfo : function()
	{
		var innerRange = this._Range ;

		if ( !innerRange )
			this.Release( true ) ;
		else
		{
			// For text nodes, the node itself is the StartNode.
			var eStart	= innerRange.startContainer ;

			var oElementPath = new FCKElementPath( eStart ) ;
			this.StartNode			= eStart.nodeType == 3 ? eStart : eStart.childNodes[ innerRange.startOffset ] ;
			this.StartContainer		= eStart ;
			this.StartBlock			= oElementPath.Block ;
			this.StartBlockLimit	= oElementPath.BlockLimit ;

			if ( innerRange.collapsed )
			{
				this.EndNode		= this.StartNode ;
				this.EndContainer	= this.StartContainer ;
				this.EndBlock		= this.StartBlock ;
				this.EndBlockLimit	= this.StartBlockLimit ;
			}
			else
			{
				var eEnd	= innerRange.endContainer ;

				if ( eStart != eEnd )
					oElementPath = new FCKElementPath( eEnd ) ;

				// The innerRange.endContainer[ innerRange.endOffset ] is not
				// usually part of the range, but the marker for the range end. So,
				// let's get the previous available node as the real end.
				var eEndNode = eEnd ;
				if ( innerRange.endOffset == 0 )
				{
					while ( eEndNode && !eEndNode.previousSibling )
						eEndNode = eEndNode.parentNode ;

					if ( eEndNode )
						eEndNode = eEndNode.previousSibling ;
				}
				else if ( eEndNode.nodeType == 1 )
					eEndNode = eEndNode.childNodes[ innerRange.endOffset - 1 ] ;

				this.EndNode			= eEndNode ;
				this.EndContainer		= eEnd ;
				this.EndBlock			= oElementPath.Block ;
				this.EndBlockLimit		= oElementPath.BlockLimit ;
			}
		}

		this._Cache = {} ;
	},

	CreateRange : function()
	{
		return new FCKW3CRange( this.Window.document ) ;
	},

	DeleteContents : function()
	{
		if ( this._Range )
		{
			this._Range.deleteContents() ;
			this._UpdateElementInfo() ;
		}
	},

	ExtractContents : function()
	{
		if ( this._Range )
		{
			var docFrag = this._Range.extractContents() ;
			this._UpdateElementInfo() ;
			return docFrag ;
		}
		return null ;
	},

	CheckIsCollapsed : function()
	{
		if ( this._Range )
			return this._Range.collapsed ;

		return false ;
	},

	Collapse : function( toStart )
	{
		if ( this._Range )
			this._Range.collapse( toStart ) ;

		this._UpdateElementInfo() ;
	},

	Clone : function()
	{
		var oClone = FCKTools.CloneObject( this ) ;

		if ( this._Range )
			oClone._Range = this._Range.cloneRange() ;

		return oClone ;
	},

	MoveToNodeContents : function( targetNode )
	{
		if ( !this._Range )
			this._Range = this.CreateRange() ;

		this._Range.selectNodeContents( targetNode ) ;

		this._UpdateElementInfo() ;
	},

	MoveToElementStart : function( targetElement )
	{
		this.SetStart(targetElement,1) ;
		this.SetEnd(targetElement,1) ;
	},

	// Moves to the first editing point inside a element. For example, in a
	// element tree like "<p><b><i></i></b> Text</p>", the start editing point
	// is "<p><b><i>^</i></b> Text</p>" (inside <i>).
	MoveToElementEditStart : function( targetElement )
	{
		var editableElement ;

		while ( targetElement && targetElement.nodeType == 1 )
		{
			if ( FCKDomTools.CheckIsEditable( targetElement ) )
				editableElement = targetElement ;
			else if ( editableElement )
				break ;		// If we already found an editable element, stop the loop.

			targetElement = targetElement.firstChild ;
		}

		if ( editableElement )
			this.MoveToElementStart( editableElement ) ;
	},

	InsertNode : function( node )
	{
		if ( this._Range )
			this._Range.insertNode( node ) ;
	},

	CheckIsEmpty : function()
	{
		if ( this.CheckIsCollapsed() )
			return true ;

		// Inserts the contents of the range in a div tag.
		var eToolDiv = this.Window.document.createElement( 'div' ) ;
		this._Range.cloneContents().AppendTo( eToolDiv ) ;

		FCKDomTools.TrimNode( eToolDiv ) ;

		return ( eToolDiv.innerHTML.length == 0 ) ;
	},

	/**
	 * Checks if the start boundary of the current range is "visually" (like a
	 * selection caret) at the beginning of the block. It means that some
	 * things could be brefore the range, like spaces or empty inline elements,
	 * but it would still be considered at the beginning of the block.
	 */
	CheckStartOfBlock : function()
	{
		var cache = this._Cache ;
		var bIsStartOfBlock = cache.IsStartOfBlock ;

		if ( bIsStartOfBlock != undefined )
			return bIsStartOfBlock ;

		// Take the block reference.
		var block = this.StartBlock || this.StartBlockLimit ;

		var container	= this._Range.startContainer ;
		var offset		= this._Range.startOffset ;
		var currentNode ;

		if ( offset > 0 )
		{
			// First, check the start container. If it is a text node, get the
			// substring of the node value before the range offset.
			if ( container.nodeType == 3 )
			{
				var textValue = container.nodeValue.substr( 0, offset ).Trim() ;

				// If we have some text left in the container, we are not at
				// the end for the block.
				if ( textValue.length != 0 )
					return cache.IsStartOfBlock = false ;
			}
			else
				currentNode = container.childNodes[ offset - 1 ] ;
		}

		// We'll not have a currentNode if the container was a text node, or
		// the offset is zero.
		if ( !currentNode )
			currentNode = FCKDomTools.GetPreviousSourceNode( container, true, null, block ) ;

		while ( currentNode )
		{
			switch ( currentNode.nodeType )
			{
				case 1 :
					// It's not an inline element.
					if ( !FCKListsLib.InlineChildReqElements[ currentNode.nodeName.toLowerCase() ] )
						return cache.IsStartOfBlock = false ;

					break ;

				case 3 :
					// It's a text node with real text.
					if ( currentNode.nodeValue.Trim().length > 0 )
						return cache.IsStartOfBlock = false ;
			}

			currentNode = FCKDomTools.GetPreviousSourceNode( currentNode, false, null, block ) ;
		}

		return cache.IsStartOfBlock = true ;
	},

	/**
	 * Checks if the end boundary of the current range is "visually" (like a
	 * selection caret) at the end of the block. It means that some things
	 * could be after the range, like spaces, empty inline elements, or a
	 * single <br>, but it would still be considered at the end of the block.
	 */
	CheckEndOfBlock : function( refreshSelection )
	{
		var isEndOfBlock = this._Cache.IsEndOfBlock ;

		if ( isEndOfBlock != undefined )
			return isEndOfBlock ;

		// Take the block reference.
		var block = this.EndBlock || this.EndBlockLimit ;

		var container	= this._Range.endContainer ;
		var offset			= this._Range.endOffset ;
		var currentNode ;

		// First, check the end container. If it is a text node, get the
		// substring of the node value after the range offset.
		if ( container.nodeType == 3 )
		{
			var textValue = container.nodeValue ;
			if ( offset < textValue.length )
			{
				textValue = textValue.substr( offset ) ;

				// If we have some text left in the container, we are not at
				// the end for the block.
				if ( textValue.Trim().length != 0 )
					return this._Cache.IsEndOfBlock = false ;
			}
		}
		else
			currentNode = container.childNodes[ offset ] ;

		// We'll not have a currentNode if the container was a text node, of
		// the offset is out the container children limits (after it probably).
		if ( !currentNode )
			currentNode = FCKDomTools.GetNextSourceNode( container, true, null, block ) ;

		var hadBr = false ;

		while ( currentNode )
		{
			switch ( currentNode.nodeType )
			{
				case 1 :
					var nodeName = currentNode.nodeName.toLowerCase() ;

					// It's an inline element.
					if ( FCKListsLib.InlineChildReqElements[ nodeName ] )
						break ;

					// It is the first <br> found.
					if ( nodeName == 'br' && !hadBr )
					{
						hadBr = true ;
						break ;
					}

					return this._Cache.IsEndOfBlock = false ;

				case 3 :
					// It's a text node with real text.
					if ( currentNode.nodeValue.Trim().length > 0 )
						return this._Cache.IsEndOfBlock = false ;
			}

			currentNode = FCKDomTools.GetNextSourceNode( currentNode, false, null, block ) ;
		}

		if ( refreshSelection )
			this.Select() ;

		return this._Cache.IsEndOfBlock = true ;
	},

	// This is an "intrusive" way to create a bookmark. It includes <span> tags
	// in the range boundaries. The advantage of it is that it is possible to
	// handle DOM mutations when moving back to the bookmark.
	// Attention: the inclusion of nodes in the DOM is a design choice and
	// should not be changed as there are other points in the code that may be
	// using those nodes to perform operations. See GetBookmarkNode.
	// For performance, includeNodes=true if intended to SelectBookmark.
	CreateBookmark : function( includeNodes )
	{
		// Create the bookmark info (random IDs).
		var oBookmark =
		{
			StartId	: (new Date()).valueOf() + Math.floor(Math.random()*1000) + 'S',
			EndId	: (new Date()).valueOf() + Math.floor(Math.random()*1000) + 'E'
		} ;

		var oDoc = this.Window.document ;
		var eStartSpan ;
		var eEndSpan ;
		var oClone ;

		// For collapsed ranges, add just the start marker.
		if ( !this.CheckIsCollapsed() )
		{
			eEndSpan = oDoc.createElement( 'span' ) ;
			eEndSpan.style.display = 'none' ;
			eEndSpan.id = oBookmark.EndId ;
			eEndSpan.setAttribute( '_fck_bookmark', true ) ;

			// For IE, it must have something inside, otherwise it may be
			// removed during DOM operations.
//			if ( FCKBrowserInfo.IsIE )
				eEndSpan.innerHTML = '&nbsp;' ;

			oClone = this.Clone() ;
			oClone.Collapse( false ) ;
			oClone.InsertNode( eEndSpan ) ;
		}

		eStartSpan = oDoc.createElement( 'span' ) ;
		eStartSpan.style.display = 'none' ;
		eStartSpan.id = oBookmark.StartId ;
		eStartSpan.setAttribute( '_fck_bookmark', true ) ;

		// For IE, it must have something inside, otherwise it may be removed
		// during DOM operations.
//		if ( FCKBrowserInfo.IsIE )
			eStartSpan.innerHTML = '&nbsp;' ;

		oClone = this.Clone() ;
		oClone.Collapse( true ) ;
		oClone.InsertNode( eStartSpan ) ;

		if ( includeNodes )
		{
			oBookmark.StartNode = eStartSpan ;
			oBookmark.EndNode = eEndSpan ;
		}

		// Update the range position.
		if ( eEndSpan )
		{
			this.SetStart( eStartSpan, 4 ) ;
			this.SetEnd( eEndSpan, 3 ) ;
		}
		else
			this.MoveToPosition( eStartSpan, 4 ) ;

		return oBookmark ;
	},

	// This one should be a part of a hypothetic "bookmark" object.
	GetBookmarkNode : function( bookmark, start )
	{
		var doc = this.Window.document ;

		if ( start )
			return bookmark.StartNode || doc.getElementById( bookmark.StartId ) ;
		else
			return bookmark.EndNode || doc.getElementById( bookmark.EndId ) ;
	},

	MoveToBookmark : function( bookmark, preserveBookmark )
	{
		var eStartSpan	= this.GetBookmarkNode( bookmark, true ) ;
		var eEndSpan	= this.GetBookmarkNode( bookmark, false ) ;

		this.SetStart( eStartSpan, 3 ) ;

		if ( !preserveBookmark )
			FCKDomTools.RemoveNode( eStartSpan ) ;

		// If collapsed, the end span will not be available.
		if ( eEndSpan )
		{
			this.SetEnd( eEndSpan, 3 ) ;

			if ( !preserveBookmark )
				FCKDomTools.RemoveNode( eEndSpan ) ;
		}
		else
			this.Collapse( true ) ;

		this._UpdateElementInfo() ;
	},

	// Non-intrusive bookmark algorithm
	CreateBookmark2 : function()
	{
		// If there is no range then get out of here.
		// It happens on initial load in Safari #962 and if the editor it's hidden also in Firefox
		if ( ! this._Range )
			return { "Start" : 0, "End" : 0 } ;

		// First, we record down the offset values
		var bookmark =
		{
			"Start" : [ this._Range.startOffset ],
			"End" : [ this._Range.endOffset ]
		} ;
		// Since we're treating the document tree as normalized, we need to backtrack the text lengths
		// of previous text nodes into the offset value.
		var curStart = this._Range.startContainer.previousSibling ;
		var curEnd = this._Range.endContainer.previousSibling ;

		// Also note that the node that we use for "address base" would change during backtracking.
		var addrStart = this._Range.startContainer ;
		var addrEnd = this._Range.endContainer ;
		while ( curStart && curStart.nodeType == 3 && addrStart.nodeType == 3 )
		{
			bookmark.Start[0] += curStart.length ;
			addrStart = curStart ;
			curStart = curStart.previousSibling ;
		}
		while ( curEnd && curEnd.nodeType == 3 && addrEnd.nodeType == 3 )
		{
			bookmark.End[0] += curEnd.length ;
			addrEnd = curEnd ;
			curEnd = curEnd.previousSibling ;
		}

		// If the object pointed to by the startOffset and endOffset are text nodes, we need
		// to backtrack and add in the text offset to the bookmark addresses.
		if ( addrStart.nodeType == 1 && addrStart.childNodes[bookmark.Start[0]] && addrStart.childNodes[bookmark.Start[0]].nodeType == 3 )
		{
			var curNode = addrStart.childNodes[bookmark.Start[0]] ;
			var offset = 0 ;
			while ( curNode.previousSibling && curNode.previousSibling.nodeType == 3 )
			{
				curNode = curNode.previousSibling ;
				offset += curNode.length ;
			}
			addrStart = curNode ;
			bookmark.Start[0] = offset ;
		}
		if ( addrEnd.nodeType == 1 && addrEnd.childNodes[bookmark.End[0]] && addrEnd.childNodes[bookmark.End[0]].nodeType == 3 )
		{
			var curNode = addrEnd.childNodes[bookmark.End[0]] ;
			var offset = 0 ;
			while ( curNode.previousSibling && curNode.previousSibling.nodeType == 3 )
			{
				curNode = curNode.previousSibling ;
				offset += curNode.length ;
			}
			addrEnd = curNode ;
			bookmark.End[0] = offset ;
		}

		// Then, we record down the precise position of the container nodes
		// by walking up the DOM tree and counting their childNode index
		bookmark.Start = FCKDomTools.GetNodeAddress( addrStart, true ).concat( bookmark.Start ) ;
		bookmark.End = FCKDomTools.GetNodeAddress( addrEnd, true ).concat( bookmark.End ) ;
		return bookmark;
	},

	MoveToBookmark2 : function( bookmark )
	{
		// Reverse the childNode counting algorithm in CreateBookmark2()
		var curStart = FCKDomTools.GetNodeFromAddress( this.Window.document, bookmark.Start.slice( 0, -1 ), true ) ;
		var curEnd = FCKDomTools.GetNodeFromAddress( this.Window.document, bookmark.End.slice( 0, -1 ), true ) ;

		// Generate the W3C Range object and update relevant data
		this.Release( true ) ;
		this._Range = new FCKW3CRange( this.Window.document ) ;
		var startOffset = bookmark.Start[ bookmark.Start.length - 1 ] ;
		var endOffset = bookmark.End[ bookmark.End.length - 1 ] ;
		while ( curStart.nodeType == 3 && startOffset > curStart.length )
		{
			if ( ! curStart.nextSibling || curStart.nextSibling.nodeType != 3 )
				break ;
			startOffset -= curStart.length ;
			curStart = curStart.nextSibling ;
		}
		while ( curEnd.nodeType == 3 && endOffset > curEnd.length )
		{
			if ( ! curEnd.nextSibling || curEnd.nextSibling.nodeType != 3 )
				break ;
			endOffset -= curEnd.length ;
			curEnd = curEnd.nextSibling ;
		}
		this._Range.setStart( curStart, startOffset ) ;
		this._Range.setEnd( curEnd, endOffset ) ;
		this._UpdateElementInfo() ;
	},

	MoveToPosition : function( targetElement, position )
	{
		this.SetStart( targetElement, position ) ;
		this.Collapse( true ) ;
	},

	/*
	 * Moves the position of the start boundary of the range to a specific position
	 * relatively to a element.
	 *		@position:
	 *			1 = After Start		<target>^contents</target>
	 *			2 = Before End		<target>contents^</target>
	 *			3 = Before Start	^<target>contents</target>
	 *			4 = After End		<target>contents</target>^
	 */
	SetStart : function( targetElement, position, noInfoUpdate )
	{
		var oRange = this._Range ;
		if ( !oRange )
			oRange = this._Range = this.CreateRange() ;

		switch( position )
		{
			case 1 :		// After Start		<target>^contents</target>
				oRange.setStart( targetElement, 0 ) ;
				break ;

			case 2 :		// Before End		<target>contents^</target>
				oRange.setStart( targetElement, targetElement.childNodes.length ) ;
				break ;

			case 3 :		// Before Start		^<target>contents</target>
				oRange.setStartBefore( targetElement ) ;
				break ;

			case 4 :		// After End		<target>contents</target>^
				oRange.setStartAfter( targetElement ) ;
		}

		if ( !noInfoUpdate )
			this._UpdateElementInfo() ;
	},

	/*
	 * Moves the position of the start boundary of the range to a specific position
	 * relatively to a element.
	 *		@position:
	 *			1 = After Start		<target>^contents</target>
	 *			2 = Before End		<target>contents^</target>
	 *			3 = Before Start	^<target>contents</target>
	 *			4 = After End		<target>contents</target>^
	 */
	SetEnd : function( targetElement, position, noInfoUpdate )
	{
		var oRange = this._Range ;
		if ( !oRange )
			oRange = this._Range = this.CreateRange() ;

		switch( position )
		{
			case 1 :		// After Start		<target>^contents</target>
				oRange.setEnd( targetElement, 0 ) ;
				break ;

			case 2 :		// Before End		<target>contents^</target>
				oRange.setEnd( targetElement, targetElement.childNodes.length ) ;
				break ;

			case 3 :		// Before Start		^<target>contents</target>
				oRange.setEndBefore( targetElement ) ;
				break ;

			case 4 :		// After End		<target>contents</target>^
				oRange.setEndAfter( targetElement ) ;
		}

		if ( !noInfoUpdate )
			this._UpdateElementInfo() ;
	},

	Expand : function( unit )
	{
		var oNode, oSibling ;

		switch ( unit )
		{
			// Expand the range to include all inline parent elements if we are
			// are in their boundary limits.
			// For example (where [ ] are the range limits):
			//	Before =>		Some <b>[<i>Some sample text]</i></b>.
			//	After =>		Some [<b><i>Some sample text</i></b>].
			case 'inline_elements' :
				// Expand the start boundary.
				if ( this._Range.startOffset == 0 )
				{
					oNode = this._Range.startContainer ;

					if ( oNode.nodeType != 1 )
						oNode = oNode.previousSibling ? null : oNode.parentNode ;

					if ( oNode )
					{
						while ( FCKListsLib.InlineNonEmptyElements[ oNode.nodeName.toLowerCase() ] )
						{
							this._Range.setStartBefore( oNode ) ;

							if ( oNode != oNode.parentNode.firstChild )
								break ;

							oNode = oNode.parentNode ;
						}
					}
				}

				// Expand the end boundary.
				oNode = this._Range.endContainer ;
				var offset = this._Range.endOffset ;

				if ( ( oNode.nodeType == 3 && offset >= oNode.nodeValue.length ) || ( oNode.nodeType == 1 && offset >= oNode.childNodes.length ) || ( oNode.nodeType != 1 && oNode.nodeType != 3 ) )
				{
					if ( oNode.nodeType != 1 )
						oNode = oNode.nextSibling ? null : oNode.parentNode ;

					if ( oNode )
					{
						while ( FCKListsLib.InlineNonEmptyElements[ oNode.nodeName.toLowerCase() ] )
						{
							this._Range.setEndAfter( oNode ) ;

							if ( oNode != oNode.parentNode.lastChild )
								break ;

							oNode = oNode.parentNode ;
						}
					}
				}

				break ;

			case 'block_contents' :
			case 'list_contents' :
				var boundarySet = FCKListsLib.BlockBoundaries ;
				if ( unit == 'list_contents' || FCKConfig.EnterMode == 'br' )
					boundarySet = FCKListsLib.ListBoundaries ;

				if ( this.StartBlock && FCKConfig.EnterMode != 'br' && unit == 'block_contents' )
					this.SetStart( this.StartBlock, 1 ) ;
				else
				{
					// Get the start node for the current range.
					oNode = this._Range.startContainer ;

					// If it is an element, get the node right before of it (in source order).
					if ( oNode.nodeType == 1 )
					{
						var lastNode = oNode.childNodes[ this._Range.startOffset ] ;
						if ( lastNode )
							oNode = FCKDomTools.GetPreviousSourceNode( lastNode, true ) ;
						else
							oNode = oNode.lastChild || oNode ;
					}

					// We must look for the left boundary, relative to the range
					// start, which is limited by a block element.
					while ( oNode
							&& ( oNode.nodeType != 1
								|| ( oNode != this.StartBlockLimit
									&& !boundarySet[ oNode.nodeName.toLowerCase() ] ) ) )
					{
						this._Range.setStartBefore( oNode ) ;
						oNode = oNode.previousSibling || oNode.parentNode ;
					}
				}

				if ( this.EndBlock && FCKConfig.EnterMode != 'br' && unit == 'block_contents' && this.EndBlock.nodeName.toLowerCase() != 'li' )
					this.SetEnd( this.EndBlock, 2 ) ;
				else
				{
					oNode = this._Range.endContainer ;
					if ( oNode.nodeType == 1 )
						oNode = oNode.childNodes[ this._Range.endOffset ] || oNode.lastChild ;

					// We must look for the right boundary, relative to the range
					// end, which is limited by a block element.
					while ( oNode
							&& ( oNode.nodeType != 1
								|| ( oNode != this.StartBlockLimit
									&& !boundarySet[ oNode.nodeName.toLowerCase() ] ) ) )
					{
						this._Range.setEndAfter( oNode ) ;
						oNode = oNode.nextSibling || oNode.parentNode ;
					}

					// In EnterMode='br', the end <br> boundary element must
					// be included in the expanded range.
					if ( oNode && oNode.nodeName.toLowerCase() == 'br' )
						this._Range.setEndAfter( oNode ) ;
				}

				this._UpdateElementInfo() ;
		}
	},

	/**
	 * Split the block element for the current range. It deletes the contents
	 * of the range and splits the block in the collapsed position, resulting
	 * in two sucessive blocks. The range is then positioned in the middle of
	 * them.
	 *
	 * It returns and object with the following properties:
	 *		- PreviousBlock	: a reference to the block element that preceeds
	 *		  the range after the split.
	 *		- NextBlock : a reference to the block element that follows the
	 *		  range after the split.
	 *		- WasStartOfBlock : a boolean indicating that the range was
	 *		  originaly at the start of the block.
	 *		- WasEndOfBlock : a boolean indicating that the range was originaly
	 *		  at the end of the block.
	 *
	 * If the range was originaly at the start of the block, no split will happen
	 * and the PreviousBlock value will be null. The same is valid for the
	 * NextBlock value if the range was at the end of the block.
	 */
	SplitBlock : function( forceBlockTag )
	{
		var blockTag = forceBlockTag || FCKConfig.EnterMode ;

		if ( !this._Range )
			this.MoveToSelection() ;

		// The range boundaries must be in the same "block limit" element.
		if ( this.StartBlockLimit == this.EndBlockLimit )
		{
			// Get the current blocks.
			var eStartBlock		= this.StartBlock ;
			var eEndBlock		= this.EndBlock ;
			var oElementPath	= null ;

			if ( blockTag != 'br' )
			{
				if ( !eStartBlock )
				{
					eStartBlock = this.FixBlock( true, blockTag ) ;
					eEndBlock	= this.EndBlock ;	// FixBlock may have fixed the EndBlock too.
				}

				if ( !eEndBlock )
					eEndBlock = this.FixBlock( false, blockTag ) ;
			}

			// Get the range position.
			var bIsStartOfBlock	= ( eStartBlock != null && this.CheckStartOfBlock() ) ;
			var bIsEndOfBlock	= ( eEndBlock != null && this.CheckEndOfBlock() ) ;

			// Delete the current contents.
			if ( !this.CheckIsEmpty() )
				this.DeleteContents() ;

			if ( eStartBlock && eEndBlock && eStartBlock == eEndBlock )
			{
				if ( bIsEndOfBlock )
				{
					oElementPath = new FCKElementPath( this.StartContainer ) ;
					this.MoveToPosition( eEndBlock, 4 ) ;
					eEndBlock = null ;
				}
				else if ( bIsStartOfBlock )
				{
					oElementPath = new FCKElementPath( this.StartContainer ) ;
					this.MoveToPosition( eStartBlock, 3 ) ;
					eStartBlock = null ;
				}
				else
				{
					// Extract the contents of the block from the selection point to the end of its contents.
					this.SetEnd( eStartBlock, 2 ) ;
					var eDocFrag = this.ExtractContents() ;

					// Duplicate the block element after it.
					eEndBlock = eStartBlock.cloneNode( false ) ;
					eEndBlock.removeAttribute( 'id', false ) ;

					// Place the extracted contents in the duplicated block.
					eDocFrag.AppendTo( eEndBlock ) ;

					FCKDomTools.InsertAfterNode( eStartBlock, eEndBlock ) ;

					this.MoveToPosition( eStartBlock, 4 ) ;

					// In Gecko, the last child node must be a bogus <br>.
					// Note: bogus <br> added under <ul> or <ol> would cause lists to be incorrectly rendered.
					if ( FCKBrowserInfo.IsGecko &&
							! eStartBlock.nodeName.IEquals( ['ul', 'ol'] ) )
						FCKTools.AppendBogusBr( eStartBlock ) ;
				}
			}

			return {
				PreviousBlock	: eStartBlock,
				NextBlock		: eEndBlock,
				WasStartOfBlock : bIsStartOfBlock,
				WasEndOfBlock	: bIsEndOfBlock,
				ElementPath		: oElementPath
			} ;
		}

		return null ;
	},

	// Transform a block without a block tag in a valid block (orphan text in the body or td, usually).
	FixBlock : function( isStart, blockTag )
	{
		// Bookmark the range so we can restore it later.
		var oBookmark = this.CreateBookmark() ;

		// Collapse the range to the requested ending boundary.
		this.Collapse( isStart ) ;

		// Expands it to the block contents.
		this.Expand( 'block_contents' ) ;

		// Create the fixed block.
		var oFixedBlock = this.Window.document.createElement( blockTag ) ;

		// Move the contents of the temporary range to the fixed block.
		this.ExtractContents().AppendTo( oFixedBlock ) ;
		FCKDomTools.TrimNode( oFixedBlock ) ;

		// If the fixed block is empty (not counting bookmark nodes)
		// Add a <br /> inside to expand it.
		if ( FCKDomTools.CheckIsEmptyElement(oFixedBlock, function( element ) { return element.getAttribute('_fck_bookmark') != 'true' ; } )
				&& FCKBrowserInfo.IsGeckoLike )
				FCKTools.AppendBogusBr( oFixedBlock ) ;

		// Insert the fixed block into the DOM.
		this.InsertNode( oFixedBlock ) ;

		// Move the range back to the bookmarked place.
		this.MoveToBookmark( oBookmark ) ;

		return oFixedBlock ;
	},

	Release : function( preserveWindow )
	{
		if ( !preserveWindow )
			this.Window = null ;

		this.StartNode = null ;
		this.StartContainer = null ;
		this.StartBlock = null ;
		this.StartBlockLimit = null ;
		this.EndNode = null ;
		this.EndContainer = null ;
		this.EndBlock = null ;
		this.EndBlockLimit = null ;
		this._Range = null ;
		this._Cache = null ;
	},

	CheckHasRange : function()
	{
		return !!this._Range ;
	},

	GetTouchedStartNode : function()
	{
		var range = this._Range ;
		var container = range.startContainer ;

		if ( range.collapsed || container.nodeType != 1 )
			return container ;

		return container.childNodes[ range.startOffset ] || container ;
	},

	GetTouchedEndNode : function()
	{
		var range = this._Range ;
		var container = range.endContainer ;

		if ( range.collapsed || container.nodeType != 1 )
			return container ;

		return container.childNodes[ range.endOffset - 1 ] || container ;
	}
} ;
