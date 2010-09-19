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
 * Creation and initialization of the "FCK" object. This is the main object
 * that represents an editor instance.
 */

// FCK represents the active editor instance.
var FCK =
{
	Name			: FCKURLParams[ 'InstanceName' ],
	Status			: FCK_STATUS_NOTLOADED,
	EditMode		: FCK_EDITMODE_WYSIWYG,
	Toolbar			: null,
	HasFocus		: false,
	DataProcessor	: new FCKDataProcessor(),

	GetInstanceObject	: (function()
	{
		var w = window ;
		return function( name )
		{
			return w[name] ;
		}
	})(),

	AttachToOnSelectionChange : function( functionPointer )
	{
		this.Events.AttachEvent( 'OnSelectionChange', functionPointer ) ;
	},

	GetLinkedFieldValue : function()
	{
		return this.LinkedField.value ;
	},

	GetParentForm : function()
	{
		return this.LinkedField.form ;
	} ,

	// # START : IsDirty implementation

	StartupValue : '',

	IsDirty : function()
	{
		if ( this.EditMode == FCK_EDITMODE_SOURCE )
			return ( this.StartupValue != this.EditingArea.Textarea.value ) ;
		else
		{
			// It can happen switching between design and source mode in Gecko
			if ( ! this.EditorDocument )
				return false ;

			return ( this.StartupValue != this.EditorDocument.body.innerHTML ) ;
		}
	},

	ResetIsDirty : function()
	{
		if ( this.EditMode == FCK_EDITMODE_SOURCE )
			this.StartupValue = this.EditingArea.Textarea.value ;
		else if ( this.EditorDocument.body )
			this.StartupValue = this.EditorDocument.body.innerHTML ;
	},

	// # END : IsDirty implementation

	StartEditor : function()
	{
		this.TempBaseTag = FCKConfig.BaseHref.length > 0 ? '<base href="' + FCKConfig.BaseHref + '" _fcktemp="true"></base>' : '' ;

		// Setup the keystroke handler.
		var oKeystrokeHandler = FCK.KeystrokeHandler = new FCKKeystrokeHandler() ;
		oKeystrokeHandler.OnKeystroke = _FCK_KeystrokeHandler_OnKeystroke ;

		// Set the config keystrokes.
		oKeystrokeHandler.SetKeystrokes( FCKConfig.Keystrokes ) ;

		// In IE7, if the editor tries to access the clipboard by code, a dialog is
		// shown to the user asking if the application is allowed to access or not.
		// Due to the IE implementation of it, the KeystrokeHandler will not work
		//well in this case, so we must leave the pasting keys to have their default behavior.
		if ( FCKBrowserInfo.IsIE7 )
		{
			if ( ( CTRL + 86 /*V*/ ) in oKeystrokeHandler.Keystrokes )
				oKeystrokeHandler.SetKeystrokes( [ CTRL + 86, true ] ) ;

			if ( ( SHIFT + 45 /*INS*/ ) in oKeystrokeHandler.Keystrokes )
				oKeystrokeHandler.SetKeystrokes( [ SHIFT + 45, true ] ) ;
		}

		// Retain default behavior for Ctrl-Backspace. (Bug #362)
		oKeystrokeHandler.SetKeystrokes( [ CTRL + 8, true ] ) ;

		this.EditingArea = new FCKEditingArea( document.getElementById( 'xEditingArea' ) ) ;
		this.EditingArea.FFSpellChecker = FCKConfig.FirefoxSpellChecker ;

		// Set the editor's startup contents.
		this.SetData( this.GetLinkedFieldValue(), true ) ;

		// Tab key handling for source mode.
		FCKTools.AddEventListener( document, "keydown", this._TabKeyHandler ) ;

		// Add selection change listeners. They must be attached only once.
		this.AttachToOnSelectionChange( _FCK_PaddingNodeListener ) ;
		if ( FCKBrowserInfo.IsGecko )
			this.AttachToOnSelectionChange( this._ExecCheckEmptyBlock ) ;

	},

	Focus : function()
	{
		FCK.EditingArea.Focus() ;
	},

	SetStatus : function( newStatus )
	{
		this.Status = newStatus ;

		if ( newStatus == FCK_STATUS_ACTIVE )
		{
			FCKFocusManager.AddWindow( window, true ) ;

			if ( FCKBrowserInfo.IsIE )
				FCKFocusManager.AddWindow( window.frameElement, true ) ;

			// Force the focus in the editor.
			if ( FCKConfig.StartupFocus )
				FCK.Focus() ;
		}

		this.Events.FireEvent( 'OnStatusChange', newStatus ) ;

	},

	// Fixes the body by moving all inline and text nodes to appropriate block
	// elements.
	FixBody : function()
	{
		var sBlockTag = FCKConfig.EnterMode ;

		// In 'br' mode, no fix must be done.
		if ( sBlockTag != 'p' && sBlockTag != 'div' )
			return ;

		var oDocument = this.EditorDocument ;

		if ( !oDocument )
			return ;

		var oBody = oDocument.body ;

		if ( !oBody )
			return ;

		FCKDomTools.TrimNode( oBody ) ;

		var oNode = oBody.firstChild ;
		var oNewBlock ;

		while ( oNode )
		{
			var bMoveNode = false ;

			switch ( oNode.nodeType )
			{
				// Element Node.
				case 1 :
					var nodeName = oNode.nodeName.toLowerCase() ;
					if ( !FCKListsLib.BlockElements[ nodeName ] &&
							nodeName != 'li' &&
							!oNode.getAttribute('_fckfakelement') &&
							oNode.getAttribute('_moz_dirty') == null )
						bMoveNode = true ;
					break ;

				// Text Node.
				case 3 :
					// Ignore space only or empty text.
					if ( oNewBlock || oNode.nodeValue.Trim().length > 0 )
						bMoveNode = true ;
					break;

				// Comment Node
				case 8 :
					if ( oNewBlock )
						bMoveNode = true ;
					break;
			}

			if ( bMoveNode )
			{
				var oParent = oNode.parentNode ;

				if ( !oNewBlock )
					oNewBlock = oParent.insertBefore( oDocument.createElement( sBlockTag ), oNode ) ;

				oNewBlock.appendChild( oParent.removeChild( oNode ) ) ;

				oNode = oNewBlock.nextSibling ;
			}
			else
			{
				if ( oNewBlock )
				{
					FCKDomTools.TrimNode( oNewBlock ) ;
					oNewBlock = null ;
				}
				oNode = oNode.nextSibling ;
			}
		}

		if ( oNewBlock )
			FCKDomTools.TrimNode( oNewBlock ) ;
	},

	GetData : function( format )
	{
		FCK.Events.FireEvent("OnBeforeGetData") ;

		// We assume that if the user is in source editing, the editor value must
		// represent the exact contents of the source, as the user wanted it to be.
		if ( FCK.EditMode == FCK_EDITMODE_SOURCE )
				return FCK.EditingArea.Textarea.value ;

		this.FixBody() ;

		var oDoc = FCK.EditorDocument ;
		if ( !oDoc )
			return null ;

		var isFullPage = FCKConfig.FullPage ;

		// Call the Data Processor to generate the output data.
		var data = FCK.DataProcessor.ConvertToDataFormat(
			isFullPage ? oDoc.documentElement : oDoc.body,
			!isFullPage,
			FCKConfig.IgnoreEmptyParagraphValue,
			format ) ;

		// Restore protected attributes.
		data = FCK.ProtectEventsRestore( data ) ;

		if ( FCKBrowserInfo.IsIE )
			data = data.replace( FCKRegexLib.ToReplace, '$1' ) ;

		if ( isFullPage )
		{
			if ( FCK.DocTypeDeclaration && FCK.DocTypeDeclaration.length > 0 )
				data = FCK.DocTypeDeclaration + '\n' + data ;

			if ( FCK.XmlDeclaration && FCK.XmlDeclaration.length > 0 )
				data = FCK.XmlDeclaration + '\n' + data ;
		}

		data = FCKConfig.ProtectedSource.Revert( data ) ;

		setTimeout( function() { FCK.Events.FireEvent("OnAfterGetData") ; }, 0 ) ;

		return data ;
	},

	UpdateLinkedField : function()
	{
		var value = FCK.GetXHTML( FCKConfig.FormatOutput ) ;

		if ( FCKConfig.HtmlEncodeOutput )
			value = FCKTools.HTMLEncode( value ) ;

		FCK.LinkedField.value = value ;
		FCK.Events.FireEvent( 'OnAfterLinkedFieldUpdate' ) ;
	},

	RegisteredDoubleClickHandlers : new Object(),

	OnDoubleClick : function( element )
	{
		var oCalls = FCK.RegisteredDoubleClickHandlers[ element.tagName.toUpperCase() ] ;

		if ( oCalls )
		{
			for ( var i = 0 ; i < oCalls.length ; i++ )
				oCalls[ i ]( element ) ;
		}

		// Generic handler for any element
		oCalls = FCK.RegisteredDoubleClickHandlers[ '*' ] ;

		if ( oCalls )
		{
			for ( var i = 0 ; i < oCalls.length ; i++ )
				oCalls[ i ]( element ) ;
		}

	},

	// Register objects that can handle double click operations.
	RegisterDoubleClickHandler : function( handlerFunction, tag )
	{
		var nodeName = tag || '*' ;
		nodeName = nodeName.toUpperCase() ;

		var aTargets ;

		if ( !( aTargets = FCK.RegisteredDoubleClickHandlers[ nodeName ] ) )
			FCK.RegisteredDoubleClickHandlers[ nodeName ] = [ handlerFunction ] ;
		else
		{
			// Check that the event handler isn't already registered with the same listener
			// It doesn't detect function pointers belonging to an object (at least in Gecko)
			if ( aTargets.IndexOf( handlerFunction ) == -1 )
				aTargets.push( handlerFunction ) ;
		}

	},

	OnAfterSetHTML : function()
	{
		FCKDocumentProcessor.Process( FCK.EditorDocument ) ;
		FCKUndo.SaveUndoStep() ;

		FCK.Events.FireEvent( 'OnSelectionChange' ) ;
		FCK.Events.FireEvent( 'OnAfterSetHTML' ) ;
	},

	// Saves URLs on links and images on special attributes, so they don't change when
	// moving around.
	ProtectUrls : function( html )
	{
		// <A> href
		html = html.replace( FCKRegexLib.ProtectUrlsA	, '$& _fcksavedurl=$1' ) ;

		// <IMG> src
		html = html.replace( FCKRegexLib.ProtectUrlsImg	, '$& _fcksavedurl=$1' ) ;

		// <AREA> href
		html = html.replace( FCKRegexLib.ProtectUrlsArea	, '$& _fcksavedurl=$1' ) ;

		return html ;
	},

	// Saves event attributes (like onclick) so they don't get executed while
	// editing.
	ProtectEvents : function( html )
	{
		return html.replace( FCKRegexLib.TagsWithEvent, _FCK_ProtectEvents_ReplaceTags ) ;
	},

	ProtectEventsRestore : function( html )
	{
		return html.replace( FCKRegexLib.ProtectedEvents, _FCK_ProtectEvents_RestoreEvents ) ;
	},

	ProtectTags : function( html )
	{
		var sTags = FCKConfig.ProtectedTags ;

		// IE doesn't support <abbr> and it breaks it. Let's protect it.
		if ( FCKBrowserInfo.IsIE )
			sTags += sTags.length > 0 ? '|ABBR|XML|EMBED|OBJECT' : 'ABBR|XML|EMBED|OBJECT' ;

		var oRegex ;
		if ( sTags.length > 0 )
		{
			oRegex = new RegExp( '<(' + sTags + ')(?!\w|:)', 'gi' ) ;
			html = html.replace( oRegex, '<FCK:$1' ) ;

			oRegex = new RegExp( '<\/(' + sTags + ')>', 'gi' ) ;
			html = html.replace( oRegex, '<\/FCK:$1>' ) ;
		}

		// Protect some empty elements. We must do it separately because the
		// original tag may not contain the closing slash, like <hr>:
		//		- <meta> tags get executed, so if you have a redirect meta, the
		//		  content will move to the target page.
		//		- <hr> may destroy the document structure if not well
		//		  positioned. The trick is protect it here and restore them in
		//		  the FCKDocumentProcessor.
		sTags = 'META' ;
		if ( FCKBrowserInfo.IsIE )
			sTags += '|HR' ;

		oRegex = new RegExp( '<((' + sTags + ')(?=\\s|>|/)[\\s\\S]*?)/?>', 'gi' ) ;
		html = html.replace( oRegex, '<FCK:$1 />' ) ;

		return html ;
	},

	SetData : function( data, resetIsDirty )
	{
		this.EditingArea.Mode = FCK.EditMode ;

		// If there was an onSelectionChange listener in IE we must remove it to avoid crashes #1498
		if ( FCKBrowserInfo.IsIE && FCK.EditorDocument )
		{
			FCK.EditorDocument.detachEvent("onselectionchange", Doc_OnSelectionChange ) ;
		}

		FCKTempBin.Reset() ;

		// Bug #2469: SelectionData.createRange becomes undefined after the editor
		// iframe is changed by FCK.SetData().
		FCK.Selection.Release() ;

		if ( FCK.EditMode == FCK_EDITMODE_WYSIWYG )
		{
			// Save the resetIsDirty for later use (async)
			this._ForceResetIsDirty = ( resetIsDirty === true ) ;

			// Protect parts of the code that must remain untouched (and invisible)
			// during editing.
			data = FCKConfig.ProtectedSource.Protect( data ) ;

			// Call the Data Processor to transform the data.
			data = FCK.DataProcessor.ConvertToHtml( data ) ;

			// Fix for invalid self-closing tags (see #152).
			data = data.replace( FCKRegexLib.InvalidSelfCloseTags, '$1></$2>' ) ;

			// Protect event attributes (they could get fired in the editing area).
			data = FCK.ProtectEvents( data ) ;

			// Protect some things from the browser itself.
			data = FCK.ProtectUrls( data ) ;
			data = FCK.ProtectTags( data ) ;

			// Insert the base tag (FCKConfig.BaseHref), if not exists in the source.
			// The base must be the first tag in the HEAD, to get relative
			// links on styles, for example.
			if ( FCK.TempBaseTag.length > 0 && !FCKRegexLib.HasBaseTag.test( data ) )
				data = data.replace( FCKRegexLib.HeadOpener, '$&' + FCK.TempBaseTag ) ;

			// Build the HTML for the additional things we need on <head>.
			var sHeadExtra = '' ;

			if ( !FCKConfig.FullPage )
				sHeadExtra += _FCK_GetEditorAreaStyleTags() ;

			if ( FCKBrowserInfo.IsIE )
				sHeadExtra += FCK._GetBehaviorsStyle() ;
			else if ( FCKConfig.ShowBorders )
				sHeadExtra += FCKTools.GetStyleHtml( FCK_ShowTableBordersCSS, true ) ;

			sHeadExtra += FCKTools.GetStyleHtml( FCK_InternalCSS, true ) ;

			// Attention: do not change it before testing it well (sample07)!
			// This is tricky... if the head ends with <meta ... content type>,
			// Firefox will break. But, it works if we place our extra stuff as
			// the last elements in the HEAD.
			data = data.replace( FCKRegexLib.HeadCloser, sHeadExtra + '$&' ) ;

			// Load the HTML in the editing area.
			this.EditingArea.OnLoad = _FCK_EditingArea_OnLoad ;
			this.EditingArea.Start( data ) ;
		}
		else
		{
			// Remove the references to the following elements, as the editing area
			// IFRAME will be removed.
			FCK.EditorWindow	= null ;
			FCK.EditorDocument	= null ;
			FCKDomTools.PaddingNode = null ;

			this.EditingArea.OnLoad = null ;
			this.EditingArea.Start( data ) ;

			// Enables the context menu in the textarea.
			this.EditingArea.Textarea._FCKShowContextMenu = true ;

			// Removes the enter key handler.
			FCK.EnterKeyHandler = null ;

			if ( resetIsDirty )
				this.ResetIsDirty() ;

			// Listen for keystroke events.
			FCK.KeystrokeHandler.AttachToElement( this.EditingArea.Textarea ) ;

			this.EditingArea.Textarea.focus() ;

			FCK.Events.FireEvent( 'OnAfterSetHTML' ) ;
		}

		if ( window.onresize )
			window.onresize() ;
	},

	// This collection is used by the browser specific implementations to tell
	// which named commands must be handled separately.
	RedirectNamedCommands : new Object(),

	ExecuteNamedCommand : function( commandName, commandParameter, noRedirect, noSaveUndo )
	{
		if ( !noSaveUndo )
			FCKUndo.SaveUndoStep() ;

		if ( !noRedirect && FCK.RedirectNamedCommands[ commandName ] != null )
			FCK.ExecuteRedirectedNamedCommand( commandName, commandParameter ) ;
		else
		{
			FCK.Focus() ;
			FCK.EditorDocument.execCommand( commandName, false, commandParameter ) ;
			FCK.Events.FireEvent( 'OnSelectionChange' ) ;
		}

		if ( !noSaveUndo )
		FCKUndo.SaveUndoStep() ;
	},

	GetNamedCommandState : function( commandName )
	{
		try
		{

			// Bug #50 : Safari never returns positive state for the Paste command, override that.
			if ( FCKBrowserInfo.IsSafari && FCK.EditorWindow && commandName.IEquals( 'Paste' ) )
				return FCK_TRISTATE_OFF ;

			if ( !FCK.EditorDocument.queryCommandEnabled( commandName ) )
				return FCK_TRISTATE_DISABLED ;
			else
			{
				return FCK.EditorDocument.queryCommandState( commandName ) ? FCK_TRISTATE_ON : FCK_TRISTATE_OFF ;
			}
		}
		catch ( e )
		{
			return FCK_TRISTATE_OFF ;
		}
	},

	GetNamedCommandValue : function( commandName )
	{
		var sValue = '' ;
		var eState = FCK.GetNamedCommandState( commandName ) ;

		if ( eState == FCK_TRISTATE_DISABLED )
			return null ;

		try
		{
			sValue = this.EditorDocument.queryCommandValue( commandName ) ;
		}
		catch(e) {}

		return sValue ? sValue : '' ;
	},

	Paste : function( _callListenersOnly )
	{
		// First call 'OnPaste' listeners.
		if ( FCK.Status != FCK_STATUS_COMPLETE || !FCK.Events.FireEvent( 'OnPaste' ) )
			return false ;

		// Then call the default implementation.
		return _callListenersOnly || FCK._ExecPaste() ;
	},

	PasteFromWord : function()
	{
		FCKDialog.OpenDialog( 'FCKDialog_Paste', FCKLang.PasteFromWord, 'dialog/fck_paste.html', 400, 330, 'Word' ) ;
	},

	Preview : function()
	{
		var sHTML ;

		if ( FCKConfig.FullPage )
		{
			if ( FCK.TempBaseTag.length > 0 )
				sHTML = FCK.TempBaseTag + FCK.GetXHTML() ;
			else
				sHTML = FCK.GetXHTML() ;
		}
		else
		{
			sHTML =
				FCKConfig.DocType +
				'<html dir="' + FCKConfig.ContentLangDirection + '">' +
				'<head>' +
				FCK.TempBaseTag +
				'<title>' + FCKLang.Preview + '</title>' +
				_FCK_GetEditorAreaStyleTags() +
				'</head><body' + FCKConfig.GetBodyAttributes() + '>' +
				FCK.GetXHTML() +
				'</body></html>' ;
		}

		var iWidth	= FCKConfig.ScreenWidth * 0.8 ;
		var iHeight	= FCKConfig.ScreenHeight * 0.7 ;
		var iLeft	= ( FCKConfig.ScreenWidth - iWidth ) / 2 ;

		var sOpenUrl = '' ;
		if ( FCK_IS_CUSTOM_DOMAIN && FCKBrowserInfo.IsIE)
		{
			window._FCKHtmlToLoad = sHTML ;
			sOpenUrl = 'javascript:void( (function(){' +
				'document.open() ;' +
				'document.domain="' + document.domain + '" ;' +
				'document.write( window.opener._FCKHtmlToLoad );' +
				'document.close() ;' +
				'window.opener._FCKHtmlToLoad = null ;' +
				'})() )' ;
		}

		var oWindow = window.open( sOpenUrl, null, 'toolbar=yes,location=no,status=yes,menubar=yes,scrollbars=yes,resizable=yes,width=' + iWidth + ',height=' + iHeight + ',left=' + iLeft ) ;

		if ( !FCK_IS_CUSTOM_DOMAIN || !FCKBrowserInfo.IsIE)
		{
			oWindow.document.write( sHTML );
			oWindow.document.close();
		}

	},

	SwitchEditMode : function( noUndo )
	{
		var bIsWysiwyg = ( FCK.EditMode == FCK_EDITMODE_WYSIWYG ) ;

		// Save the current IsDirty state, so we may restore it after the switch.
		var bIsDirty = FCK.IsDirty() ;

		var sHtml ;

		// Update the HTML in the view output to show, also update
		// FCKTempBin for IE to avoid #2263.
		if ( bIsWysiwyg )
		{
			FCKCommands.GetCommand( 'ShowBlocks' ).SaveState() ;
			if ( !noUndo && FCKBrowserInfo.IsIE )
				FCKUndo.SaveUndoStep() ;

			sHtml = FCK.GetXHTML( FCKConfig.FormatSource ) ;

			if ( FCKBrowserInfo.IsIE )
				FCKTempBin.ToHtml() ;

			if ( sHtml == null )
				return false ;
		}
		else
			sHtml = this.EditingArea.Textarea.value ;

		FCK.EditMode = bIsWysiwyg ? FCK_EDITMODE_SOURCE : FCK_EDITMODE_WYSIWYG ;

		FCK.SetData( sHtml, !bIsDirty ) ;

		// Set the Focus.
		FCK.Focus() ;

		// Update the toolbar (Running it directly causes IE to fail).
		FCKTools.RunFunction( FCK.ToolbarSet.RefreshModeState, FCK.ToolbarSet ) ;

		return true ;
	},

	InsertElement : function( element )
	{
		// The parameter may be a string (element name), so transform it in an element.
		if ( typeof element == 'string' )
			element = this.EditorDocument.createElement( element ) ;

		var elementName = element.nodeName.toLowerCase() ;

		FCKSelection.Restore() ;

		// Create a range for the selection. V3 will have a new selection
		// object that may internally supply this feature.
		var range = new FCKDomRange( this.EditorWindow ) ;

		// Move to the selection and delete it.
		range.MoveToSelection() ;
		range.DeleteContents() ;

		if ( FCKListsLib.BlockElements[ elementName ] != null )
		{
			if ( range.StartBlock )
			{
				if ( range.CheckStartOfBlock() )
					range.MoveToPosition( range.StartBlock, 3 ) ;
				else if ( range.CheckEndOfBlock() )
					range.MoveToPosition( range.StartBlock, 4 ) ;
				else
					range.SplitBlock() ;
			}

			range.InsertNode( element ) ;

			var next = FCKDomTools.GetNextSourceElement( element, false, null, [ 'hr','br','param','img','area','input' ], true ) ;

			// Be sure that we have something after the new element, so we can move the cursor there.
			if ( !next && FCKConfig.EnterMode != 'br')
			{
				next = this.EditorDocument.body.appendChild( this.EditorDocument.createElement( FCKConfig.EnterMode ) ) ;

				if ( FCKBrowserInfo.IsGeckoLike )
					FCKTools.AppendBogusBr( next ) ;
			}

			if ( FCKListsLib.EmptyElements[ elementName ] == null )
				range.MoveToElementEditStart( element ) ;
			else if ( next )
				range.MoveToElementEditStart( next ) ;
			else
				range.MoveToPosition( element, 4 ) ;

			if ( FCKBrowserInfo.IsGeckoLike )
			{
				if ( next )
					FCKDomTools.ScrollIntoView( next, false );
				FCKDomTools.ScrollIntoView( element, false );
			}
		}
		else
		{
			// Insert the node.
			range.InsertNode( element ) ;

			// Move the selection right after the new element.
			// DISCUSSION: Should we select the element instead?
			range.SetStart( element, 4 ) ;
			range.SetEnd( element, 4 ) ;
		}

		range.Select() ;
		range.Release() ;

		// REMOVE IT: The focus should not really be set here. It is up to the
		// calling code to reset the focus if needed.
		this.Focus() ;

		return element ;
	},

	_InsertBlockElement : function( blockElement )
	{
	},

	_IsFunctionKey : function( keyCode )
	{
		// keys that are captured but do not change editor contents
		if ( keyCode >= 16 && keyCode <= 20 )
			// shift, ctrl, alt, pause, capslock
			return true ;
		if ( keyCode == 27 || ( keyCode >= 33 && keyCode <= 40 ) )
			// esc, page up, page down, end, home, left, up, right, down
			return true ;
		if ( keyCode == 45 )
			// insert, no effect on FCKeditor, yet
			return true ;
		return false ;
	},

	_KeyDownListener : function( evt )
	{
		if (! evt)
			evt = FCK.EditorWindow.event ;
		if ( FCK.EditorWindow )
		{
			if ( !FCK._IsFunctionKey(evt.keyCode) // do not capture function key presses, like arrow keys or shift/alt/ctrl
					&& !(evt.ctrlKey || evt.metaKey) // do not capture Ctrl hotkeys, as they have their snapshot capture logic
					&& !(evt.keyCode == 46) ) // do not capture Del, it has its own capture logic in fckenterkey.js
				FCK._KeyDownUndo() ;
		}
		return true ;
	},

	_KeyDownUndo : function()
	{
		if ( !FCKUndo.Typing )
		{
			FCKUndo.SaveUndoStep() ;
			FCKUndo.Typing = true ;
			FCK.Events.FireEvent( "OnSelectionChange" ) ;
		}

		FCKUndo.TypesCount++ ;
		FCKUndo.Changed = 1 ;

		if ( FCKUndo.TypesCount > FCKUndo.MaxTypes )
		{
			FCKUndo.TypesCount = 0 ;
			FCKUndo.SaveUndoStep() ;
		}
	},

	_TabKeyHandler : function( evt )
	{
		if ( ! evt )
			evt = window.event ;

		var keystrokeValue = evt.keyCode ;

		// Pressing <Tab> in source mode should produce a tab space in the text area, not
		// changing the focus to something else.
		if ( keystrokeValue == 9 && FCK.EditMode != FCK_EDITMODE_WYSIWYG )
		{
			if ( FCKBrowserInfo.IsIE )
			{
				var range = document.selection.createRange() ;
				if ( range.parentElement() != FCK.EditingArea.Textarea )
					return true ;
				range.text = '\t' ;
				range.select() ;
			}
			else
			{
				var a = [] ;
				var el = FCK.EditingArea.Textarea ;
				var selStart = el.selectionStart ;
				var selEnd = el.selectionEnd ;
				a.push( el.value.substr(0, selStart ) ) ;
				a.push( '\t' ) ;
				a.push( el.value.substr( selEnd ) ) ;
				el.value = a.join( '' ) ;
				el.setSelectionRange( selStart + 1, selStart + 1 ) ;
			}

			if ( evt.preventDefault )
				return evt.preventDefault() ;

			return evt.returnValue = false ;
		}

		return true ;
	}
} ;

FCK.Events = new FCKEvents( FCK ) ;

// DEPRECATED in favor or "GetData".
FCK.GetHTML	= FCK.GetXHTML = FCK.GetData ;

// DEPRECATED in favor of "SetData".
FCK.SetHTML = FCK.SetData ;

// InsertElementAndGetIt and CreateElement are Deprecated : returns the same value as InsertElement.
FCK.InsertElementAndGetIt = FCK.CreateElement = FCK.InsertElement ;

// Replace all events attributes (like onclick).
function _FCK_ProtectEvents_ReplaceTags( tagMatch )
{
	return tagMatch.replace( FCKRegexLib.EventAttributes, _FCK_ProtectEvents_ReplaceEvents ) ;
}

// Replace an event attribute with its respective __fckprotectedatt attribute.
// The original event markup will be encoded and saved as the value of the new
// attribute.
function _FCK_ProtectEvents_ReplaceEvents( eventMatch, attName )
{
	return ' ' + attName + '_fckprotectedatt="' + encodeURIComponent( eventMatch ) + '"' ;
}

function _FCK_ProtectEvents_RestoreEvents( match, encodedOriginal )
{
	return decodeURIComponent( encodedOriginal ) ;
}

function _FCK_MouseEventsListener( evt )
{
	if ( ! evt )
		evt = window.event ;
	if ( evt.type == 'mousedown' )
		FCK.MouseDownFlag = true ;
	else if ( evt.type == 'mouseup' )
		FCK.MouseDownFlag = false ;
	else if ( evt.type == 'mousemove' )
		FCK.Events.FireEvent( 'OnMouseMove', evt ) ;
}

function _FCK_PaddingNodeListener()
{
	if ( FCKConfig.EnterMode.IEquals( 'br' ) )
		return ;
	FCKDomTools.EnforcePaddingNode( FCK.EditorDocument, FCKConfig.EnterMode ) ;

	if ( ! FCKBrowserInfo.IsIE && FCKDomTools.PaddingNode )
	{
		// Prevent the caret from going between the body and the padding node in Firefox.
		// i.e. <body>|<p></p></body>
		var sel = FCKSelection.GetSelection() ;
		if ( sel && sel.rangeCount == 1 )
		{
			var range = sel.getRangeAt( 0 ) ;
			if ( range.collapsed && range.startContainer == FCK.EditorDocument.body && range.startOffset == 0 )
			{
				range.selectNodeContents( FCKDomTools.PaddingNode ) ;
				range.collapse( true ) ;
				sel.removeAllRanges() ;
				sel.addRange( range ) ;
			}
		}
	}
	else if ( FCKDomTools.PaddingNode )
	{
		// Prevent the caret from going into an empty body but not into the padding node in IE.
		// i.e. <body><p></p>|</body>
		var parentElement = FCKSelection.GetParentElement() ;
		var paddingNode = FCKDomTools.PaddingNode ;
		if ( parentElement && parentElement.nodeName.IEquals( 'body' ) )
		{
			if ( FCK.EditorDocument.body.childNodes.length == 1
					&& FCK.EditorDocument.body.firstChild == paddingNode )
			{
				/*
				 * Bug #1764: Don't move the selection if the
				 * current selection isn't in the editor
				 * document.
				 */
				if ( FCKSelection._GetSelectionDocument( FCK.EditorDocument.selection ) != FCK.EditorDocument )
					return ;

				var range = FCK.EditorDocument.body.createTextRange() ;
				var clearContents = false ;
				if ( !paddingNode.childNodes.firstChild )
				{
					paddingNode.appendChild( FCKTools.GetElementDocument( paddingNode ).createTextNode( '\ufeff' ) ) ;
					clearContents = true ;
				}
				range.moveToElementText( paddingNode ) ;
				range.select() ;
				if ( clearContents )
					range.pasteHTML( '' ) ;
			}
		}
	}
}

function _FCK_EditingArea_OnLoad()
{
	// Get the editor's window and document (DOM)
	FCK.EditorWindow	= FCK.EditingArea.Window ;
	FCK.EditorDocument	= FCK.EditingArea.Document ;

	if ( FCKBrowserInfo.IsIE )
		FCKTempBin.ToElements() ;

	FCK.InitializeBehaviors() ;

	// Listen for mousedown and mouseup events for tracking drag and drops.
	FCK.MouseDownFlag = false ;
	FCKTools.AddEventListener( FCK.EditorDocument, 'mousemove', _FCK_MouseEventsListener ) ;
	FCKTools.AddEventListener( FCK.EditorDocument, 'mousedown', _FCK_MouseEventsListener ) ;
	FCKTools.AddEventListener( FCK.EditorDocument, 'mouseup', _FCK_MouseEventsListener ) ;
	if ( FCKBrowserInfo.IsSafari )
	{
		// #3481: WebKit has a bug with paste where the paste contents may leak
		// outside table cells. So add padding nodes before and after the paste.
		FCKTools.AddEventListener( FCK.EditorDocument, 'paste', function( evt )
		{
			var range = new FCKDomRange( FCK.EditorWindow );
			var nodeBefore = FCK.EditorDocument.createTextNode( '\ufeff' );
			var nodeAfter = FCK.EditorDocument.createElement( 'a' );
			nodeAfter.id = 'fck_paste_padding';
			nodeAfter.innerHTML = '&#65279;';
			range.MoveToSelection();
			range.DeleteContents();

			// Insert padding nodes.
			range.InsertNode( nodeBefore );
			range.Collapse();
			range.InsertNode( nodeAfter );

			// Move the selection to between the padding nodes.
			range.MoveToPosition( nodeAfter, 3 );
			range.Select();

			// Remove the padding nodes after the paste is done.
			setTimeout( function()
				{
					nodeBefore.parentNode.removeChild( nodeBefore );
					nodeAfter = FCK.EditorDocument.getElementById( 'fck_paste_padding' );
					nodeAfter.parentNode.removeChild( nodeAfter );
				}, 0 );
		} );
	}

	// Most of the CTRL key combos do not work under Safari for onkeydown and onkeypress (See #1119)
	// But we can use the keyup event to override some of these...
	if ( FCKBrowserInfo.IsSafari )
	{
		var undoFunc = function( evt )
		{
			if ( ! ( evt.ctrlKey || evt.metaKey ) )
				return ;
			if ( FCK.EditMode != FCK_EDITMODE_WYSIWYG )
				return ;
			switch ( evt.keyCode )
			{
				case 89:
					FCKUndo.Redo() ;
					break ;
				case 90:
					FCKUndo.Undo() ;
					break ;
			}
		}

		FCKTools.AddEventListener( FCK.EditorDocument, 'keyup', undoFunc ) ;
	}

	// Create the enter key handler
	FCK.EnterKeyHandler = new FCKEnterKey( FCK.EditorWindow, FCKConfig.EnterMode, FCKConfig.ShiftEnterMode, FCKConfig.TabSpaces ) ;

	// Listen for keystroke events.
	FCK.KeystrokeHandler.AttachToElement( FCK.EditorDocument ) ;

	if ( FCK._ForceResetIsDirty )
		FCK.ResetIsDirty() ;

	// This is a tricky thing for IE. In some cases, even if the cursor is
	// blinking in the editing, the keystroke handler doesn't catch keyboard
	// events. We must activate the editing area to make it work. (#142).
	if ( FCKBrowserInfo.IsIE && FCK.HasFocus )
		FCK.EditorDocument.body.setActive() ;

	FCK.OnAfterSetHTML() ;

	// Restore show blocks status.
	FCKCommands.GetCommand( 'ShowBlocks' ).RestoreState() ;

	// Check if it is not a startup call, otherwise complete the startup.
	if ( FCK.Status != FCK_STATUS_NOTLOADED )
		return ;

	FCK.SetStatus( FCK_STATUS_ACTIVE ) ;
}

function _FCK_GetEditorAreaStyleTags()
{
	return FCKTools.GetStyleHtml( FCKConfig.EditorAreaCSS ) +
		FCKTools.GetStyleHtml( FCKConfig.EditorAreaStyles ) ;
}

function _FCK_KeystrokeHandler_OnKeystroke( keystroke, keystrokeValue )
{
	if ( FCK.Status != FCK_STATUS_COMPLETE )
		return false ;

	if ( FCK.EditMode == FCK_EDITMODE_WYSIWYG )
	{
		switch ( keystrokeValue )
		{
			case 'Paste' :
				return !FCK.Paste() ;

			case 'Cut' :
				FCKUndo.SaveUndoStep() ;
				return false ;
		}
	}
	else
	{
		// In source mode, some actions must have their default behavior.
		if ( keystrokeValue.Equals( 'Paste', 'Undo', 'Redo', 'SelectAll', 'Cut' ) )
			return false ;
	}

	// The return value indicates if the default behavior of the keystroke must
	// be cancelled. Let's do that only if the Execute() call explicitly returns "false".
	var oCommand = FCK.Commands.GetCommand( keystrokeValue ) ;

	// If the command is disabled then ignore the keystroke
	if ( oCommand.GetState() == FCK_TRISTATE_DISABLED )
		return false ;

	return ( oCommand.Execute.apply( oCommand, FCKTools.ArgumentsToArray( arguments, 2 ) ) !== false ) ;
}

// Set the FCK.LinkedField reference to the field that will be used to post the
// editor data.
(function()
{
	// There is a bug on IE... getElementById returns any META tag that has the
	// name set to the ID you are looking for. So the best way in to get the array
	// by names and look for the correct one.
	// As ASP.Net generates a ID that is different from the Name, we must also
	// look for the field based on the ID (the first one is the ID).

	var oDocument = window.parent.document ;

	// Try to get the field using the ID.
	var eLinkedField = oDocument.getElementById( FCK.Name ) ;

	var i = 0;
	while ( eLinkedField || i == 0 )
	{
		if ( eLinkedField && eLinkedField.tagName.toLowerCase().Equals( 'input', 'textarea' ) )
		{
			FCK.LinkedField = eLinkedField ;
			break ;
		}

		eLinkedField = oDocument.getElementsByName( FCK.Name )[i++] ;
	}
})() ;

var FCKTempBin =
{
	Elements : new Array(),

	AddElement : function( element )
	{
		var iIndex = this.Elements.length ;
		this.Elements[ iIndex ] = element ;
		return iIndex ;
	},

	RemoveElement : function( index )
	{
		var e = this.Elements[ index ] ;
		this.Elements[ index ] = null ;
		return e ;
	},

	Reset : function()
	{
		var i = 0 ;
		while ( i < this.Elements.length )
			this.Elements[ i++ ] = null ;
		this.Elements.length = 0 ;
	},

	ToHtml : function()
	{
		for ( var i = 0 ; i < this.Elements.length ; i++ )
		{
			this.Elements[i] = '<div>&nbsp;' + this.Elements[i].outerHTML + '</div>' ;
			this.Elements[i].isHtml = true ;
		}
	},

	ToElements : function()
	{
		var node = FCK.EditorDocument.createElement( 'div' ) ;
		for ( var i = 0 ; i < this.Elements.length ; i++ )
		{
			if ( this.Elements[i].isHtml )
			{
				node.innerHTML = this.Elements[i] ;
				this.Elements[i] = node.firstChild.removeChild( node.firstChild.lastChild ) ;
			}
		}
	}
} ;



// # Focus Manager: Manages the focus in the editor.
var FCKFocusManager = FCK.FocusManager =
{
	IsLocked : false,

	AddWindow : function( win, sendToEditingArea )
	{
		var oTarget ;

		if ( FCKBrowserInfo.IsIE )
			oTarget = win.nodeType == 1 ? win : win.frameElement ? win.frameElement : win.document ;
		else if ( FCKBrowserInfo.IsSafari )
			oTarget = win ;
		else
			oTarget = win.document ;

		FCKTools.AddEventListener( oTarget, 'blur', FCKFocusManager_Win_OnBlur ) ;
		FCKTools.AddEventListener( oTarget, 'focus', sendToEditingArea ? FCKFocusManager_Win_OnFocus_Area : FCKFocusManager_Win_OnFocus ) ;
	},

	RemoveWindow : function( win )
	{
		if ( FCKBrowserInfo.IsIE )
			oTarget = win.nodeType == 1 ? win : win.frameElement ? win.frameElement : win.document ;
		else
			oTarget = win.document ;

		FCKTools.RemoveEventListener( oTarget, 'blur', FCKFocusManager_Win_OnBlur ) ;
		FCKTools.RemoveEventListener( oTarget, 'focus', FCKFocusManager_Win_OnFocus_Area ) ;
		FCKTools.RemoveEventListener( oTarget, 'focus', FCKFocusManager_Win_OnFocus ) ;
	},

	Lock : function()
	{
		this.IsLocked = true ;
	},

	Unlock : function()
	{
		if ( this._HasPendingBlur )
			FCKFocusManager._Timer = window.setTimeout( FCKFocusManager_FireOnBlur, 100 ) ;

		this.IsLocked = false ;
	},

	_ResetTimer : function()
	{
		this._HasPendingBlur = false ;

		if ( this._Timer )
		{
			window.clearTimeout( this._Timer ) ;
			delete this._Timer ;
		}
	}
} ;

function FCKFocusManager_Win_OnBlur()
{
	if ( typeof(FCK) != 'undefined' && FCK.HasFocus )
	{
		FCKFocusManager._ResetTimer() ;
		FCKFocusManager._Timer = window.setTimeout( FCKFocusManager_FireOnBlur, 100 ) ;
	}
}

function FCKFocusManager_FireOnBlur()
{
	if ( FCKFocusManager.IsLocked )
		FCKFocusManager._HasPendingBlur = true ;
	else
	{
		FCK.HasFocus = false ;
		FCK.Events.FireEvent( "OnBlur" ) ;
	}
}

function FCKFocusManager_Win_OnFocus_Area()
{
	// Check if we are already focusing the editor (to avoid loops).
	if ( FCKFocusManager._IsFocusing )
		return ;

	FCKFocusManager._IsFocusing = true ;

	FCK.Focus() ;
	FCKFocusManager_Win_OnFocus() ;

	// The above FCK.Focus() call may trigger other focus related functions.
	// So, to avoid a loop, we delay the focusing mark removal, so it get
	// executed after all othre functions have been run.
	FCKTools.RunFunction( function()
		{
			delete FCKFocusManager._IsFocusing ;
		} ) ;
}

function FCKFocusManager_Win_OnFocus()
{
	FCKFocusManager._ResetTimer() ;

	if ( !FCK.HasFocus && !FCKFocusManager.IsLocked )
	{
		FCK.HasFocus = true ;
		FCK.Events.FireEvent( "OnFocus" ) ;
	}
}

/*
 * #1633 : Protect the editor iframe from external styles.
 * Notice that we can't use FCKTools.ResetStyles here since FCKTools isn't
 * loaded yet.
 */
(function()
{
	var el = window.frameElement ;
	var width = el.width ;
	var height = el.height ;
	if ( /^\d+$/.test( width ) ) width += 'px' ;
	if ( /^\d+$/.test( height ) ) height += 'px' ;
	var style = el.style ;
	style.border = style.padding = style.margin = 0 ;
	style.backgroundColor = 'transparent';
	style.backgroundImage = 'none';
	style.width = width ;
	style.height = height ;
})() ;
