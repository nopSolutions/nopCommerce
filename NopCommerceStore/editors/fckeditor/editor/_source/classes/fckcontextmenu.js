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
 * FCKContextMenu Class: renders an control a context menu.
 */

var FCKContextMenu = function( parentWindow, langDir )
{
	this.CtrlDisable = false ;

	var oPanel = this._Panel = new FCKPanel( parentWindow ) ;
	oPanel.AppendStyleSheet( FCKConfig.SkinEditorCSS ) ;
	oPanel.IsContextMenu = true ;

	// The FCKTools.DisableSelection doesn't seems to work to avoid dragging of the icons in Mozilla
	// so we stop the start of the dragging
	if ( FCKBrowserInfo.IsGecko )
		oPanel.Document.addEventListener( 'draggesture', function(e) {e.preventDefault(); return false;}, true ) ;

	var oMenuBlock = this._MenuBlock = new FCKMenuBlock() ;
	oMenuBlock.Panel = oPanel ;
	oMenuBlock.OnClick = FCKTools.CreateEventListener( FCKContextMenu_MenuBlock_OnClick, this ) ;

	this._Redraw = true ;
}


FCKContextMenu.prototype.SetMouseClickWindow = function( mouseClickWindow )
{
	if ( !FCKBrowserInfo.IsIE )
	{
		this._Document = mouseClickWindow.document ;
		if ( FCKBrowserInfo.IsOpera && !( 'oncontextmenu' in document.createElement('foo') ) )
		{
			this._Document.addEventListener( 'mousedown', FCKContextMenu_Document_OnMouseDown, false ) ;
			this._Document.addEventListener( 'mouseup', FCKContextMenu_Document_OnMouseUp, false ) ;
		}
		this._Document.addEventListener( 'contextmenu', FCKContextMenu_Document_OnContextMenu, false ) ;
	}
}

/**
 The customData parameter is just a value that will be send to the command that is executed,
 so it's possible to reuse the same command for several items just by assigning different data for each one.
*/
FCKContextMenu.prototype.AddItem = function( name, label, iconPathOrStripInfoArrayOrIndex, isDisabled, customData )
{
	var oItem = this._MenuBlock.AddItem( name, label, iconPathOrStripInfoArrayOrIndex, isDisabled, customData ) ;
	this._Redraw = true ;
	return oItem ;
}

FCKContextMenu.prototype.AddSeparator = function()
{
	this._MenuBlock.AddSeparator() ;
	this._Redraw = true ;
}

FCKContextMenu.prototype.RemoveAllItems = function()
{
	this._MenuBlock.RemoveAllItems() ;
	this._Redraw = true ;
}

FCKContextMenu.prototype.AttachToElement = function( element )
{
	if ( FCKBrowserInfo.IsIE )
		FCKTools.AddEventListenerEx( element, 'contextmenu', FCKContextMenu_AttachedElement_OnContextMenu, this ) ;
	else
		element._FCKContextMenu = this ;
}

function FCKContextMenu_Document_OnContextMenu( e )
{
	if ( FCKConfig.BrowserContextMenu )
		return true ;

	var el = e.target ;

	while ( el )
	{
		if ( el._FCKContextMenu )
		{
			if ( el._FCKContextMenu.CtrlDisable && ( e.ctrlKey || e.metaKey ) )
				return true ;

			FCKTools.CancelEvent( e ) ;
			FCKContextMenu_AttachedElement_OnContextMenu( e, el._FCKContextMenu, el ) ;
			return false ;
		}
		el = el.parentNode ;
	}
	return true ;
}

var FCKContextMenu_OverrideButton ;

function FCKContextMenu_Document_OnMouseDown( e )
{
	if( !e || e.button != 2 )
		return false ;

	if ( FCKConfig.BrowserContextMenu )
		return true ;

	var el = e.target ;

	while ( el )
	{
		if ( el._FCKContextMenu )
		{
			if ( el._FCKContextMenu.CtrlDisable && ( e.ctrlKey || e.metaKey ) )
				return true ;

			var overrideButton = FCKContextMenu_OverrideButton ;
			if( !overrideButton )
			{
				var doc = FCKTools.GetElementDocument( e.target ) ;
				overrideButton = FCKContextMenu_OverrideButton = doc.createElement('input') ;
				overrideButton.type = 'button' ;
				var buttonHolder = doc.createElement('p') ;
				doc.body.appendChild( buttonHolder ) ;
				buttonHolder.appendChild( overrideButton ) ;
			}

			overrideButton.style.cssText = 'position:absolute;top:' + ( e.clientY - 2 ) +
				'px;left:' + ( e.clientX - 2 ) +
				'px;width:5px;height:5px;opacity:0.01' ;
		}
		el = el.parentNode ;
	}
	return false ;
}

function FCKContextMenu_Document_OnMouseUp( e )
{
	if ( FCKConfig.BrowserContextMenu )
		return true ;

	var overrideButton = FCKContextMenu_OverrideButton ;

	if ( overrideButton )
	{
		var parent = overrideButton.parentNode ;
		parent.parentNode.removeChild( parent ) ;
		FCKContextMenu_OverrideButton = undefined ;

		if( e && e.button == 2 )
		{
			FCKContextMenu_Document_OnContextMenu( e ) ;
			return false ;
		}
	}
	return true ;
}

function FCKContextMenu_AttachedElement_OnContextMenu( ev, fckContextMenu, el )
{
	if ( ( fckContextMenu.CtrlDisable && ( ev.ctrlKey || ev.metaKey ) ) || FCKConfig.BrowserContextMenu )
		return true ;

	var eTarget = el || this ;

	if ( fckContextMenu.OnBeforeOpen )
		fckContextMenu.OnBeforeOpen.call( fckContextMenu, eTarget ) ;

	if ( fckContextMenu._MenuBlock.Count() == 0 )
		return false ;

	if ( fckContextMenu._Redraw )
	{
		fckContextMenu._MenuBlock.Create( fckContextMenu._Panel.MainNode ) ;
		fckContextMenu._Redraw = false ;
	}

	// This will avoid that the content of the context menu can be dragged in IE
	// as the content of the panel is recreated we need to do it every time
	FCKTools.DisableSelection( fckContextMenu._Panel.Document.body ) ;

	var x = 0 ;
	var y = 0 ;
	if ( FCKBrowserInfo.IsIE )
	{
		x = ev.screenX ;
		y = ev.screenY ;
	}
	else if ( FCKBrowserInfo.IsSafari )
	{
		x = ev.clientX ;
		y = ev.clientY ;
	}
	else
	{
		x = ev.pageX ;
		y = ev.pageY ;
	}
	fckContextMenu._Panel.Show( x, y, ev.currentTarget || null ) ;

	return false ;
}

function FCKContextMenu_MenuBlock_OnClick( menuItem, contextMenu )
{
	contextMenu._Panel.Hide() ;
	FCKTools.RunFunction( contextMenu.OnItemClick, contextMenu, menuItem ) ;
}
