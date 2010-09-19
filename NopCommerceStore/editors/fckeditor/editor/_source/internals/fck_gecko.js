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
 * Creation and initialization of the "FCK" object. This is the main
 * object that represents an editor instance.
 * (Gecko specific implementations)
 */

FCK.Description = "FCKeditor for Gecko Browsers" ;

FCK.InitializeBehaviors = function()
{
	// When calling "SetData", the editing area IFRAME gets a fixed height. So we must recalculate it.
	if ( window.onresize )		// Not for Safari/Opera.
		window.onresize() ;

	FCKFocusManager.AddWindow( this.EditorWindow ) ;

	this.ExecOnSelectionChange = function()
	{
		FCK.Events.FireEvent( "OnSelectionChange" ) ;
	}

	this._ExecDrop = function( evt )
	{
		if ( FCK.MouseDownFlag )
		{
			FCK.MouseDownFlag = false ;
			return ;
		}

		if ( FCKConfig.ForcePasteAsPlainText )
		{
			if ( evt.dataTransfer )
			{
				var text = evt.dataTransfer.getData( 'Text' ) ;
				text = FCKTools.HTMLEncode( text ) ;
				text = FCKTools.ProcessLineBreaks( window, FCKConfig, text ) ;
				FCK.InsertHtml( text ) ;
			}
			else if ( FCKConfig.ShowDropDialog )
				FCK.PasteAsPlainText() ;

			evt.preventDefault() ;
			evt.stopPropagation() ;
		}
	}

	this._ExecCheckCaret = function( evt )
	{
		if ( FCK.EditMode != FCK_EDITMODE_WYSIWYG )
			return ;

		if ( evt.type == 'keypress' )
		{
			var keyCode = evt.keyCode ;
			// ignore if positioning key is not pressed.
			// left or up arrow keys need to be processed as well, since <a> links can be expanded in Gecko's editor
			// when the caret moved left or up from another block element below.
			if ( keyCode < 33 || keyCode > 40 )
				return ;
		}

		var blockEmptyStop = function( node )
		{
			if ( node.nodeType != 1 )
				return false ;
			var tag = node.tagName.toLowerCase() ;
			return ( FCKListsLib.BlockElements[tag] || FCKListsLib.EmptyElements[tag] ) ;
		}

		var moveCursor = function()
		{
			var selection = FCKSelection.GetSelection() ;
			var range = selection.getRangeAt(0) ;
			if ( ! range || ! range.collapsed )
				return ;

			var node = range.endContainer ;

			// only perform the patched behavior if we're at the end of a text node.
			if ( node.nodeType != 3 )
				return ;

			if ( node.nodeValue.length != range.endOffset )
				return ;

			// only perform the patched behavior if we're in an <a> tag, or the End key is pressed.
			var parentTag = node.parentNode.tagName.toLowerCase() ;
			if ( ! (  parentTag == 'a' || ( !FCKBrowserInfo.IsOpera && String(node.parentNode.contentEditable) == 'false' ) ||
					( ! ( FCKListsLib.BlockElements[parentTag] || FCKListsLib.NonEmptyBlockElements[parentTag] )
					  && keyCode == 35 ) ) )
				return ;

			// our caret has moved to just after the last character of a text node under an unknown tag, how to proceed?
			// first, see if there are other text nodes by DFS walking from this text node.
			// 	- if the DFS has scanned all nodes under my parent, then go the next step.
			//	- if there is a text node after me but still under my parent, then do nothing and return.
			var nextTextNode = FCKTools.GetNextTextNode( node, node.parentNode, blockEmptyStop ) ;
			if ( nextTextNode )
				return ;

			// we're pretty sure we need to move the caret forcefully from here.
			range = FCK.EditorDocument.createRange() ;

			nextTextNode = FCKTools.GetNextTextNode( node, node.parentNode.parentNode, blockEmptyStop ) ;
			if ( nextTextNode )
			{
				// Opera thinks the dummy empty text node we append beyond the end of <a> nodes occupies a caret
				// position. So if the user presses the left key and we reset the caret position here, the user
				// wouldn't be able to go back.
				if ( FCKBrowserInfo.IsOpera && keyCode == 37 )
					return ;

				// now we want to get out of our current parent node, adopt the next parent, and move the caret to
				// the appropriate text node under our new parent.
				// our new parent might be our current parent's siblings if we are lucky.
				range.setStart( nextTextNode, 0 ) ;
				range.setEnd( nextTextNode, 0 ) ;
			}
			else
			{
				// no suitable next siblings under our grandparent! what to do next?
				while ( node.parentNode
					&& node.parentNode != FCK.EditorDocument.body
					&& node.parentNode != FCK.EditorDocument.documentElement
					&& node == node.parentNode.lastChild
					&& ( ! FCKListsLib.BlockElements[node.parentNode.tagName.toLowerCase()]
					  && ! FCKListsLib.NonEmptyBlockElements[node.parentNode.tagName.toLowerCase()] ) )
					node = node.parentNode ;


				if ( FCKListsLib.BlockElements[ parentTag ]
						|| FCKListsLib.EmptyElements[ parentTag ]
						|| node == FCK.EditorDocument.body )
				{
					// if our parent is a block node, move to the end of our parent.
					range.setStart( node, node.childNodes.length ) ;
					range.setEnd( node, node.childNodes.length ) ;
				}
				else
				{
					// things are a little bit more interesting if our parent is not a block node
					// due to the weired ways how Gecko's caret acts...
					var stopNode = node.nextSibling ;

					// find out the next block/empty element at our grandparent, we'll
					// move the caret just before it.
					while ( stopNode )
					{
						if ( stopNode.nodeType != 1 )
						{
							stopNode = stopNode.nextSibling ;
							continue ;
						}

						var stopTag = stopNode.tagName.toLowerCase() ;
						if ( FCKListsLib.BlockElements[stopTag] || FCKListsLib.EmptyElements[stopTag]
							|| FCKListsLib.NonEmptyBlockElements[stopTag] )
							break ;
						stopNode = stopNode.nextSibling ;
					}

					// note that the dummy marker below is NEEDED, otherwise the caret's behavior will
					// be broken in Gecko.
					var marker = FCK.EditorDocument.createTextNode( '' ) ;
					if ( stopNode )
						node.parentNode.insertBefore( marker, stopNode ) ;
					else
						node.parentNode.appendChild( marker ) ;
					range.setStart( marker, 0 ) ;
					range.setEnd( marker, 0 ) ;
				}
			}

			selection.removeAllRanges() ;
			selection.addRange( range ) ;
			FCK.Events.FireEvent( "OnSelectionChange" ) ;
		}

		setTimeout( moveCursor, 1 ) ;
	}

	this.ExecOnSelectionChangeTimer = function()
	{
		if ( FCK.LastOnChangeTimer )
			window.clearTimeout( FCK.LastOnChangeTimer ) ;

		FCK.LastOnChangeTimer = window.setTimeout( FCK.ExecOnSelectionChange, 100 ) ;
	}

	this.EditorDocument.addEventListener( 'mouseup', this.ExecOnSelectionChange, false ) ;

	// On Gecko, firing the "OnSelectionChange" event on every key press started to be too much
	// slow. So, a timer has been implemented to solve performance issues when typing to quickly.
	this.EditorDocument.addEventListener( 'keyup', this.ExecOnSelectionChangeTimer, false ) ;

	this._DblClickListener = function( e )
	{
		FCK.OnDoubleClick( e.target ) ;
		e.stopPropagation() ;
	}
	this.EditorDocument.addEventListener( 'dblclick', this._DblClickListener, true ) ;

	// Record changes for the undo system when there are key down events.
	this.EditorDocument.addEventListener( 'keydown', this._KeyDownListener, false ) ;

	// Hooks for data object drops
	if ( FCKBrowserInfo.IsGecko )
	{
		this.EditorWindow.addEventListener( 'dragdrop', this._ExecDrop, true ) ;
	}
	else if ( FCKBrowserInfo.IsSafari )
	{
		this.EditorDocument.addEventListener( 'dragover', function ( evt )
				{ if ( !FCK.MouseDownFlag && FCK.Config.ForcePasteAsPlainText ) evt.returnValue = false ; }, true ) ;
		this.EditorDocument.addEventListener( 'drop', this._ExecDrop, true ) ;
		this.EditorDocument.addEventListener( 'mousedown',
			function( ev )
			{
				var element = ev.srcElement ;

				if ( element.nodeName.IEquals( 'IMG', 'HR', 'INPUT', 'TEXTAREA', 'SELECT' ) )
				{
					FCKSelection.SelectNode( element ) ;
				}
			}, true ) ;

		this.EditorDocument.addEventListener( 'mouseup',
			function( ev )
			{
				if ( ev.srcElement.nodeName.IEquals( 'INPUT', 'TEXTAREA', 'SELECT' ) )
					ev.preventDefault()
			}, true ) ;

		this.EditorDocument.addEventListener( 'click',
			function( ev )
			{
				if ( ev.srcElement.nodeName.IEquals( 'INPUT', 'TEXTAREA', 'SELECT' ) )
					ev.preventDefault()
			}, true ) ;
	}

	// Kludge for buggy Gecko caret positioning logic (Bug #393 and #1056)
	if ( FCKBrowserInfo.IsGecko || FCKBrowserInfo.IsOpera )
	{
		this.EditorDocument.addEventListener( 'keypress', this._ExecCheckCaret, false ) ;
		this.EditorDocument.addEventListener( 'click', this._ExecCheckCaret, false ) ;
	}

	// Reset the context menu.
	FCK.ContextMenu._InnerContextMenu.SetMouseClickWindow( FCK.EditorWindow ) ;
	FCK.ContextMenu._InnerContextMenu.AttachToElement( FCK.EditorDocument ) ;
}

FCK.MakeEditable = function()
{
	this.EditingArea.MakeEditable() ;
}

// Disable the context menu in the editor (outside the editing area).
function Document_OnContextMenu( e )
{
	if ( !e.target._FCKShowContextMenu )
		e.preventDefault() ;
}
document.oncontextmenu = Document_OnContextMenu ;

// GetNamedCommandState overload for Gecko.
FCK._BaseGetNamedCommandState = FCK.GetNamedCommandState ;
FCK.GetNamedCommandState = function( commandName )
{
	switch ( commandName )
	{
		case 'Unlink' :
			return FCKSelection.HasAncestorNode('A') ? FCK_TRISTATE_OFF : FCK_TRISTATE_DISABLED ;
		default :
			return FCK._BaseGetNamedCommandState( commandName ) ;
	}
}

// Named commands to be handled by this browsers specific implementation.
FCK.RedirectNamedCommands =
{
	Print	: true,
	Paste	: true
} ;

// ExecuteNamedCommand overload for Gecko.
FCK.ExecuteRedirectedNamedCommand = function( commandName, commandParameter )
{
	switch ( commandName )
	{
		case 'Print' :
			FCK.EditorWindow.print() ;
			break ;
		case 'Paste' :
			try
			{
				// Force the paste dialog for Safari (#50).
				if ( FCKBrowserInfo.IsSafari )
					throw '' ;

				if ( FCK.Paste() )
					FCK.ExecuteNamedCommand( 'Paste', null, true ) ;
			}
			catch (e)	{
				if ( FCKConfig.ForcePasteAsPlainText )
					FCK.PasteAsPlainText() ;
				else
					FCKDialog.OpenDialog( 'FCKDialog_Paste', FCKLang.Paste, 'dialog/fck_paste.html', 400, 330, 'Security' ) ;
			}
			break ;
		default :
			FCK.ExecuteNamedCommand( commandName, commandParameter ) ;
	}
}

FCK._ExecPaste = function()
{
	// Save a snapshot for undo before actually paste the text
	FCKUndo.SaveUndoStep() ;

	if ( FCKConfig.ForcePasteAsPlainText )
	{
		FCK.PasteAsPlainText() ;
		return false ;
	}

	/* For now, the AutoDetectPasteFromWord feature is IE only. */
	return true ;
}

//**
// FCK.InsertHtml: Inserts HTML at the current cursor location. Deletes the
// selected content if any.
FCK.InsertHtml = function( html )
{
	var doc = FCK.EditorDocument,
		range;

	html = FCKConfig.ProtectedSource.Protect( html ) ;
	html = FCK.ProtectEvents( html ) ;
	html = FCK.ProtectUrls( html ) ;
	html = FCK.ProtectTags( html ) ;

	// Save an undo snapshot first.
	FCKUndo.SaveUndoStep() ;

	if ( FCKBrowserInfo.IsGecko )
	{
		html = html.replace( /&nbsp;$/, '$&<span _fcktemp="1"/>' ) ;

		var docFrag = new FCKDocumentFragment( this.EditorDocument ) ;
		docFrag.AppendHtml( html ) ;

		var lastNode = docFrag.RootNode.lastChild ;

		range = new FCKDomRange( this.EditorWindow ) ;
		range.MoveToSelection() ;

		// If the first element (if exists) of the document fragment is a block
		// element, then split the current block. (#1537)
		var currentNode = docFrag.RootNode.firstChild ;
		while ( currentNode && currentNode.nodeType != 1 )
			currentNode = currentNode.nextSibling ;

		if ( currentNode && FCKListsLib.BlockElements[ currentNode.nodeName.toLowerCase() ] )
			range.SplitBlock() ;

		range.DeleteContents() ;
		range.InsertNode( docFrag.RootNode ) ;

		range.MoveToPosition( lastNode, 4 ) ;
	}
	else
		doc.execCommand( 'inserthtml', false, html ) ;

	this.Focus() ;

	// Save the caret position before calling document processor.
	if ( !range )
	{
		range = new FCKDomRange( this.EditorWindow ) ;
		range.MoveToSelection() ;
	}
	var bookmark = range.CreateBookmark() ;

	FCKDocumentProcessor.Process( doc ) ;

	// Restore caret position, ignore any errors in case the document
	// processor removed the bookmark <span>s for some reason.
	try
	{
		range.MoveToBookmark( bookmark ) ;
		range.Select() ;
	}
	catch ( e ) {}

	// For some strange reason the SaveUndoStep() call doesn't activate the undo button at the first InsertHtml() call.
	this.Events.FireEvent( "OnSelectionChange" ) ;
}

FCK.PasteAsPlainText = function()
{
	// TODO: Implement the "Paste as Plain Text" code.

	// If the function is called immediately Firefox 2 does automatically paste the contents as soon as the new dialog is created
	// so we run it in a Timeout and the paste event can be cancelled
	FCKTools.RunFunction( FCKDialog.OpenDialog, FCKDialog, ['FCKDialog_Paste', FCKLang.PasteAsText, 'dialog/fck_paste.html', 400, 330, 'PlainText'] ) ;

/*
	var sText = FCKTools.HTMLEncode( clipboardData.getData("Text") ) ;
	sText = sText.replace( /\n/g, '<BR>' ) ;
	this.InsertHtml( sText ) ;
*/
}
/*
FCK.PasteFromWord = function()
{
	// TODO: Implement the "Paste as Plain Text" code.

	FCKDialog.OpenDialog( 'FCKDialog_Paste', FCKLang.PasteFromWord, 'dialog/fck_paste.html', 400, 330, 'Word' ) ;

//	FCK.CleanAndPaste( FCK.GetClipboardHTML() ) ;
}
*/
FCK.GetClipboardHTML = function()
{
	return '' ;
}

FCK.CreateLink = function( url, noUndo )
{
	// Creates the array that will be returned. It contains one or more created links (see #220).
	var aCreatedLinks = new Array() ;

	// Only for Safari, a collapsed selection may create a link. All other
	// browser will have no links created. So, we check it here and return
	// immediatelly, having the same cross browser behavior.
	if ( FCKSelection.GetSelection().isCollapsed )
		return aCreatedLinks ;

	FCK.ExecuteNamedCommand( 'Unlink', null, false, !!noUndo ) ;

	if ( url.length > 0 )
	{
		// Generate a temporary name for the link.
		var sTempUrl = 'javascript:void(0);/*' + ( new Date().getTime() ) + '*/' ;

		// Use the internal "CreateLink" command to create the link.
		FCK.ExecuteNamedCommand( 'CreateLink', sTempUrl, false, !!noUndo ) ;

		// Retrieve the just created links using XPath.
		var oLinksInteractor = this.EditorDocument.evaluate("//a[@href='" + sTempUrl + "']", this.EditorDocument.body, null, XPathResult.UNORDERED_NODE_SNAPSHOT_TYPE, null) ;

		// Add all links to the returning array.
		for ( var i = 0 ; i < oLinksInteractor.snapshotLength ; i++ )
		{
			var oLink = oLinksInteractor.snapshotItem( i ) ;
			oLink.href = url ;

			aCreatedLinks.push( oLink ) ;
		}
	}

	return aCreatedLinks ;
}

FCK._FillEmptyBlock = function( emptyBlockNode )
{
	if ( ! emptyBlockNode || emptyBlockNode.nodeType != 1 )
		return ;
	var nodeTag = emptyBlockNode.tagName.toLowerCase() ;
	if ( nodeTag != 'p' && nodeTag != 'div' )
		return ;
	if ( emptyBlockNode.firstChild )
		return ;
	FCKTools.AppendBogusBr( emptyBlockNode ) ;
}

FCK._ExecCheckEmptyBlock = function()
{
	FCK._FillEmptyBlock( FCK.EditorDocument.body.firstChild ) ;
	var sel = FCKSelection.GetSelection() ;
	if ( !sel || sel.rangeCount < 1 )
		return ;
	var range = sel.getRangeAt( 0 );
	FCK._FillEmptyBlock( range.startContainer ) ;
}
