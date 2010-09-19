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
 * (IE specific implementations)
 */

FCK.Description = "FCKeditor for Internet Explorer 5.5+" ;

FCK._GetBehaviorsStyle = function()
{
	if ( !FCK._BehaviorsStyle )
	{
		var sBasePath = FCKConfig.BasePath ;
		var sTableBehavior = '' ;
		var sStyle ;

		// The behaviors should be pointed using the BasePath to avoid security
		// errors when using a different BaseHref.
		sStyle = '<style type="text/css" _fcktemp="true">' ;

		if ( FCKConfig.ShowBorders )
			sTableBehavior = 'url(' + sBasePath + 'css/behaviors/showtableborders.htc)' ;

		// Disable resize handlers.
		sStyle += 'INPUT,TEXTAREA,SELECT,.FCK__Anchor,.FCK__PageBreak,.FCK__InputHidden' ;

		if ( FCKConfig.DisableObjectResizing )
		{
			sStyle += ',IMG' ;
			sTableBehavior += ' url(' + sBasePath + 'css/behaviors/disablehandles.htc)' ;
		}

		sStyle += ' { behavior: url(' + sBasePath + 'css/behaviors/disablehandles.htc) ; }' ;

		if ( sTableBehavior.length > 0 )
			sStyle += 'TABLE { behavior: ' + sTableBehavior + ' ; }' ;

		sStyle += '</style>' ;
		FCK._BehaviorsStyle = sStyle ;
	}

	return FCK._BehaviorsStyle ;
}

function Doc_OnMouseUp()
{
	if ( FCK.EditorWindow.event.srcElement.tagName == 'HTML' )
	{
		FCK.Focus() ;
		FCK.EditorWindow.event.cancelBubble	= true ;
		FCK.EditorWindow.event.returnValue	= false ;
	}
}

function Doc_OnPaste()
{
	var body = FCK.EditorDocument.body ;

	body.detachEvent( 'onpaste', Doc_OnPaste ) ;

	var ret = FCK.Paste( !FCKConfig.ForcePasteAsPlainText && !FCKConfig.AutoDetectPasteFromWord ) ;

	body.attachEvent( 'onpaste', Doc_OnPaste ) ;

	return ret ;
}

function Doc_OnDblClick()
{
	FCK.OnDoubleClick( FCK.EditorWindow.event.srcElement ) ;
	FCK.EditorWindow.event.cancelBubble = true ;
}

function Doc_OnSelectionChange()
{
	// Don't fire the event if no document is loaded.
	if ( !FCK.IsSelectionChangeLocked && FCK.EditorDocument )
		FCK.Events.FireEvent( "OnSelectionChange" ) ;
}

function Doc_OnDrop()
{
	if ( FCK.MouseDownFlag )
	{
		FCK.MouseDownFlag = false ;
		return ;
	}

	if ( FCKConfig.ForcePasteAsPlainText )
	{
		var evt = FCK.EditorWindow.event ;

		if ( FCK._CheckIsPastingEnabled() || FCKConfig.ShowDropDialog )
			FCK.PasteAsPlainText( evt.dataTransfer.getData( 'Text' ) ) ;

		evt.returnValue = false ;
		evt.cancelBubble = true ;
	}
}

FCK.InitializeBehaviors = function( dontReturn )
{
	// Set the focus to the editable area when clicking in the document area.
	// TODO: The cursor must be positioned at the end.
	this.EditorDocument.attachEvent( 'onmouseup', Doc_OnMouseUp ) ;

	// Intercept pasting operations
	this.EditorDocument.body.attachEvent( 'onpaste', Doc_OnPaste ) ;

	// Intercept drop operations
	this.EditorDocument.body.attachEvent( 'ondrop', Doc_OnDrop ) ;

	// Reset the context menu.
	FCK.ContextMenu._InnerContextMenu.AttachToElement( FCK.EditorDocument.body ) ;

	this.EditorDocument.attachEvent("onkeydown", FCK._KeyDownListener ) ;

	this.EditorDocument.attachEvent("ondblclick", Doc_OnDblClick ) ;

	this.EditorDocument.attachEvent("onbeforedeactivate", function(){ FCKSelection.Save() ; } ) ;

	// Catch cursor selection changes.
	this.EditorDocument.attachEvent("onselectionchange", Doc_OnSelectionChange ) ;

	FCKTools.AddEventListener( FCK.EditorDocument, 'mousedown', Doc_OnMouseDown ) ;
}

FCK.InsertHtml = function( html )
{
	html = FCKConfig.ProtectedSource.Protect( html ) ;
	html = FCK.ProtectEvents( html ) ;
	html = FCK.ProtectUrls( html ) ;
	html = FCK.ProtectTags( html ) ;

//	FCK.Focus() ;
	FCKSelection.Restore() ;
	FCK.EditorWindow.focus() ;

	FCKUndo.SaveUndoStep() ;

	// Gets the actual selection.
	var oSel = FCKSelection.GetSelection() ;

	// Deletes the actual selection contents.
	if ( oSel.type.toLowerCase() == 'control' )
		oSel.clear() ;

	// Using the following trick, any comment in the beginning of the HTML will
	// be preserved.
	html = '<span id="__fakeFCKRemove__" style="display:none;">fakeFCKRemove</span>' + html ;

	// Insert the HTML.
	oSel.createRange().pasteHTML( html ) ;

	// Remove the fake node
	var fake = FCK.EditorDocument.getElementById('__fakeFCKRemove__') ;
	// If the span is the only child of a node (so the inserted HTML is beyond that),
	// remove also that parent that isn't needed. #1537
	if (fake.parentNode.childNodes.length == 1)
		fake = fake.parentNode ;
	fake.removeNode( true ) ;

	FCKDocumentProcessor.Process( FCK.EditorDocument ) ;

	// For some strange reason the SaveUndoStep() call doesn't activate the undo button at the first InsertHtml() call.
	this.Events.FireEvent( "OnSelectionChange" ) ;
}

FCK.SetInnerHtml = function( html )		// IE Only
{
	var oDoc = FCK.EditorDocument ;
	// Using the following trick, any comment in the beginning of the HTML will
	// be preserved.
	oDoc.body.innerHTML = '<div id="__fakeFCKRemove__">&nbsp;</div>' + html ;
	oDoc.getElementById('__fakeFCKRemove__').removeNode( true ) ;
}

function FCK_PreloadImages()
{
	var oPreloader = new FCKImagePreloader() ;

	// Add the configured images.
	oPreloader.AddImages( FCKConfig.PreloadImages ) ;

	// Add the skin icons strip.
	oPreloader.AddImages( FCKConfig.SkinPath + 'fck_strip.gif' ) ;

	oPreloader.OnComplete = LoadToolbarSetup ;
	oPreloader.Start() ;
}

// Disable the context menu in the editor (outside the editing area).
function Document_OnContextMenu()
{
	return ( event.srcElement._FCKShowContextMenu == true ) ;
}
document.oncontextmenu = Document_OnContextMenu ;

function FCK_Cleanup()
{
	this.LinkedField = null ;
	this.EditorWindow = null ;
	this.EditorDocument = null ;
}

FCK._ExecPaste = function()
{
	// As we call ExecuteNamedCommand('Paste'), it would enter in a loop. So, let's use a semaphore.
	if ( FCK._PasteIsRunning )
		return true ;

	if ( FCKConfig.ForcePasteAsPlainText )
	{
		FCK.PasteAsPlainText() ;
		return false ;
	}

	var sHTML = FCK._CheckIsPastingEnabled( true ) ;

	if ( sHTML === false )
		FCKTools.RunFunction( FCKDialog.OpenDialog, FCKDialog, ['FCKDialog_Paste', FCKLang.Paste, 'dialog/fck_paste.html', 400, 330, 'Security'] ) ;
	else
	{
		if ( FCKConfig.AutoDetectPasteFromWord && sHTML.length > 0 )
		{
			var re = /<\w[^>]*(( class="?MsoNormal"?)|(="mso-))/gi ;
			if ( re.test( sHTML ) )
			{
				if ( confirm( FCKLang.PasteWordConfirm ) )
				{
					FCK.PasteFromWord() ;
					return false ;
				}
			}
		}

		// Instead of inserting the retrieved HTML, let's leave the OS work for us,
		// by calling FCK.ExecuteNamedCommand( 'Paste' ). It could give better results.

		// Enable the semaphore to avoid a loop.
		FCK._PasteIsRunning = true ;

		FCK.ExecuteNamedCommand( 'Paste' ) ;

		// Removes the semaphore.
		delete FCK._PasteIsRunning ;
	}

	// Let's always make a custom implementation (return false), otherwise
	// the new Keyboard Handler may conflict with this code, and the CTRL+V code
	// could result in a simple "V" being pasted.
	return false ;
}

FCK.PasteAsPlainText = function( forceText )
{
	if ( !FCK._CheckIsPastingEnabled() )
	{
		FCKDialog.OpenDialog( 'FCKDialog_Paste', FCKLang.PasteAsText, 'dialog/fck_paste.html', 400, 330, 'PlainText' ) ;
		return ;
	}

	// Get the data available in the clipboard in text format.
	var sText = null ;
	if ( ! forceText )
		sText = clipboardData.getData("Text") ;
	else
		sText = forceText ;

	if ( sText && sText.length > 0 )
	{
		// Replace the carriage returns with <BR>
		sText = FCKTools.HTMLEncode( sText ) ;
		sText = FCKTools.ProcessLineBreaks( window, FCKConfig, sText ) ;

		var closeTagIndex = sText.search( '</p>' ) ;
		var startTagIndex = sText.search( '<p>' ) ;

		if ( ( closeTagIndex != -1 && startTagIndex != -1 && closeTagIndex < startTagIndex )
				|| ( closeTagIndex != -1 && startTagIndex == -1 ) )
		{
			var prefix = sText.substr( 0, closeTagIndex ) ;
			sText = sText.substr( closeTagIndex + 4 ) ;
			this.InsertHtml( prefix ) ;
		}

		// Insert the resulting data in the editor.
		FCKUndo.SaveLocked = true ;
		this.InsertHtml( sText ) ;
		FCKUndo.SaveLocked = false ;
	}
}

FCK._CheckIsPastingEnabled = function( returnContents )
{
	// The following seams to be the only reliable way to check is script
	// pasting operations are enabled in the security settings of IE6 and IE7.
	// It adds a little bit of overhead to the check, but so far that's the
	// only way, mainly because of IE7.

	FCK._PasteIsEnabled = false ;

	document.body.attachEvent( 'onpaste', FCK_CheckPasting_Listener ) ;

	// The execCommand in GetClipboardHTML will fire the "onpaste", only if the
	// security settings are enabled.
	var oReturn = FCK.GetClipboardHTML() ;

	document.body.detachEvent( 'onpaste', FCK_CheckPasting_Listener ) ;

	if ( FCK._PasteIsEnabled )
	{
		if ( !returnContents )
			oReturn = true ;
	}
	else
		oReturn = false ;

	delete FCK._PasteIsEnabled ;

	return oReturn ;
}

function FCK_CheckPasting_Listener()
{
	FCK._PasteIsEnabled = true ;
}

FCK.GetClipboardHTML = function()
{
	var oDiv = document.getElementById( '___FCKHiddenDiv' ) ;

	if ( !oDiv )
	{
		oDiv = document.createElement( 'DIV' ) ;
		oDiv.id = '___FCKHiddenDiv' ;

		var oDivStyle = oDiv.style ;
		oDivStyle.position		= 'absolute' ;
		oDivStyle.visibility	= oDivStyle.overflow	= 'hidden' ;
		oDivStyle.width			= oDivStyle.height		= 1 ;

		document.body.appendChild( oDiv ) ;
	}

	oDiv.innerHTML = '' ;

	var oTextRange = document.body.createTextRange() ;
	oTextRange.moveToElementText( oDiv ) ;
	oTextRange.execCommand( 'Paste' ) ;

	var sData = oDiv.innerHTML ;
	oDiv.innerHTML = '' ;

	return sData ;
}

FCK.CreateLink = function( url, noUndo )
{
	// Creates the array that will be returned. It contains one or more created links (see #220).
	var aCreatedLinks = new Array() ;
	var isControl = FCKSelection.GetType() == 'Control' ;
	var selectedElement = isControl && FCKSelection.GetSelectedElement() ;

	// Remove any existing link in the selection.
	// IE BUG: Unlinking a floating control selection that is not inside a link
	// will collapse the selection. (#3677)
	if ( !( isControl && !FCKTools.GetElementAscensor( selectedElement, 'a' ) ) )
		FCK.ExecuteNamedCommand( 'Unlink', null, false, !!noUndo ) ;

	if ( url.length > 0 )
	{
		// If there are several images, and you try to link each one, all the images get inside the link:
		// <img><img> -> <a><img></a><img> -> <a><img><img></a> due to the call to 'CreateLink' (bug in IE)
		if ( isControl )
		{
			// Create a link
			var oLink = this.EditorDocument.createElement( 'A' ) ;
			oLink.href = url ;

			// Get the selected object
			var oControl = selectedElement ;
			// Put the link just before the object
			oControl.parentNode.insertBefore(oLink, oControl) ;
			// Move the object inside the link
			oControl.parentNode.removeChild( oControl ) ;
			oLink.appendChild( oControl ) ;

			return [ oLink ] ;
		}

		// Generate a temporary name for the link.
		var sTempUrl = 'javascript:void(0);/*' + ( new Date().getTime() ) + '*/' ;

		// Use the internal "CreateLink" command to create the link.
		FCK.ExecuteNamedCommand( 'CreateLink', sTempUrl, false, !!noUndo ) ;

		// Look for the just create link.
		var oLinks = this.EditorDocument.links ;

		for ( i = 0 ; i < oLinks.length ; i++ )
		{
			var oLink = oLinks[i] ;

			// Check it this a newly created link.
			// getAttribute must be used. oLink.url may cause problems with IE7 (#555).
			if ( oLink.getAttribute( 'href', 2 ) == sTempUrl )
			{
				var sInnerHtml = oLink.innerHTML ;	// Save the innerHTML (IE changes it if it is like an URL).
				oLink.href = url ;
				oLink.innerHTML = sInnerHtml ;		// Restore the innerHTML.

				// If the last child is a <br> move it outside the link or it
				// will be too easy to select this link again #388.
				var oLastChild = oLink.lastChild ;
				if ( oLastChild && oLastChild.nodeName == 'BR' )
				{
					// Move the BR after the link.
					FCKDomTools.InsertAfterNode( oLink, oLink.removeChild( oLastChild ) ) ;
				}

				aCreatedLinks.push( oLink ) ;
			}
		}
	}

	return aCreatedLinks ;
}

function _FCK_RemoveDisabledAtt()
{
	this.removeAttribute( 'disabled' ) ;
}

function Doc_OnMouseDown( evt )
{
	var e = evt.srcElement ;

	// Radio buttons and checkboxes should not be allowed to be triggered in IE
	// in editable mode. Otherwise the whole browser window may be locked by
	// the buttons. (#1782)
	if ( e.nodeName && e.nodeName.IEquals( 'input' ) && e.type.IEquals( ['radio', 'checkbox'] ) && !e.disabled )
	{
		e.disabled = true ;
		FCKTools.SetTimeout( _FCK_RemoveDisabledAtt, 1, e ) ;
	}
}
