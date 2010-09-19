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
 * FCKToolbarButtonUI Class: interface representation of a toolbar button.
 */

var FCKToolbarButtonUI = function( name, label, tooltip, iconPathOrStripInfoArray, style, state )
{
	this.Name		= name ;
	this.Label		= label || name ;
	this.Tooltip	= tooltip || this.Label ;
	this.Style		= style || FCK_TOOLBARITEM_ONLYICON ;
	this.State		= state || FCK_TRISTATE_OFF ;

	this.Icon = new FCKIcon( iconPathOrStripInfoArray ) ;

	if ( FCK.IECleanup )
		FCK.IECleanup.AddItem( this, FCKToolbarButtonUI_Cleanup ) ;
}


FCKToolbarButtonUI.prototype._CreatePaddingElement = function( document )
{
	var oImg = document.createElement( 'IMG' ) ;
	oImg.className = 'TB_Button_Padding' ;
	oImg.src = FCK_SPACER_PATH ;
	return oImg ;
}

FCKToolbarButtonUI.prototype.Create = function( parentElement )
{
	var oDoc = FCKTools.GetElementDocument( parentElement ) ;

	// Create the Main Element.
	var oMainElement = this.MainElement = oDoc.createElement( 'DIV' ) ;
	oMainElement.title = this.Tooltip ;

	// The following will prevent the button from catching the focus.
	if ( FCKBrowserInfo.IsGecko )
		 oMainElement.onmousedown	= FCKTools.CancelEvent ;

	FCKTools.AddEventListenerEx( oMainElement, 'mouseover', FCKToolbarButtonUI_OnMouseOver, this ) ;
	FCKTools.AddEventListenerEx( oMainElement, 'mouseout', FCKToolbarButtonUI_OnMouseOut, this ) ;
	FCKTools.AddEventListenerEx( oMainElement, 'click', FCKToolbarButtonUI_OnClick, this ) ;

	this.ChangeState( this.State, true ) ;

	if ( this.Style == FCK_TOOLBARITEM_ONLYICON && !this.ShowArrow )
	{
		// <td><div class="TB_Button_On" title="Smiley">{Image}</div></td>

		oMainElement.appendChild( this.Icon.CreateIconElement( oDoc ) ) ;
	}
	else
	{
		// <td><div class="TB_Button_On" title="Smiley"><table cellpadding="0" cellspacing="0"><tr><td>{Image}</td><td nowrap>Toolbar Button</td><td><img class="TB_Button_Padding"></td></tr></table></div></td>
		// <td><div class="TB_Button_On" title="Smiley"><table cellpadding="0" cellspacing="0"><tr><td><img class="TB_Button_Padding"></td><td nowrap>Toolbar Button</td><td><img class="TB_Button_Padding"></td></tr></table></div></td>

		var oTable = oMainElement.appendChild( oDoc.createElement( 'TABLE' ) ) ;
		oTable.cellPadding = 0 ;
		oTable.cellSpacing = 0 ;

		var oRow = oTable.insertRow(-1) ;

		// The Image cell (icon or padding).
		var oCell = oRow.insertCell(-1) ;

		if ( this.Style == FCK_TOOLBARITEM_ONLYICON || this.Style == FCK_TOOLBARITEM_ICONTEXT )
			oCell.appendChild( this.Icon.CreateIconElement( oDoc ) ) ;
		else
			oCell.appendChild( this._CreatePaddingElement( oDoc ) ) ;

		if ( this.Style == FCK_TOOLBARITEM_ONLYTEXT || this.Style == FCK_TOOLBARITEM_ICONTEXT )
		{
			// The Text cell.
			oCell = oRow.insertCell(-1) ;
			oCell.className = 'TB_Button_Text' ;
			oCell.noWrap = true ;
			oCell.appendChild( oDoc.createTextNode( this.Label ) ) ;
		}

		if ( this.ShowArrow )
		{
			if ( this.Style != FCK_TOOLBARITEM_ONLYICON )
			{
				// A padding cell.
				oRow.insertCell(-1).appendChild( this._CreatePaddingElement( oDoc ) ) ;
			}

			oCell = oRow.insertCell(-1) ;
			var eImg = oCell.appendChild( oDoc.createElement( 'IMG' ) ) ;
			eImg.src	= FCKConfig.SkinPath + 'images/toolbar.buttonarrow.gif' ;
			eImg.width	= 5 ;
			eImg.height	= 3 ;
		}

		// The last padding cell.
		oCell = oRow.insertCell(-1) ;
		oCell.appendChild( this._CreatePaddingElement( oDoc ) ) ;
	}

	parentElement.appendChild( oMainElement ) ;
}

FCKToolbarButtonUI.prototype.ChangeState = function( newState, force )
{
	if ( !force && this.State == newState )
		return ;

	var e = this.MainElement ;

	// In IE it can happen when the page is reloaded that MainElement is null, so exit here
	if ( !e )
		return ;

	switch ( parseInt( newState, 10 ) )
	{
		case FCK_TRISTATE_OFF :
			e.className		= 'TB_Button_Off' ;
			break ;

		case FCK_TRISTATE_ON :
			e.className		= 'TB_Button_On' ;
			break ;

		case FCK_TRISTATE_DISABLED :
			e.className		= 'TB_Button_Disabled' ;
			break ;
	}

	this.State = newState ;
}

function FCKToolbarButtonUI_OnMouseOver( ev, button )
{
	if ( button.State == FCK_TRISTATE_OFF )
		this.className = 'TB_Button_Off_Over' ;
	else if ( button.State == FCK_TRISTATE_ON )
		this.className = 'TB_Button_On_Over' ;
}

function FCKToolbarButtonUI_OnMouseOut( ev, button )
{
	if ( button.State == FCK_TRISTATE_OFF )
		this.className = 'TB_Button_Off' ;
	else if ( button.State == FCK_TRISTATE_ON )
		this.className = 'TB_Button_On' ;
}

function FCKToolbarButtonUI_OnClick( ev, button )
{
	if ( button.OnClick && button.State != FCK_TRISTATE_DISABLED )
		button.OnClick( button ) ;
}

function FCKToolbarButtonUI_Cleanup()
{
	// This one should not cause memory leak, but just for safety, let's clean
	// it up.
	this.MainElement = null ;
}

/*
	Sample outputs:

	This is the base structure. The variation is the image that is marked as {Image}:
		<td><div class="TB_Button_On" title="Smiley">{Image}</div></td>
		<td><div class="TB_Button_On" title="Smiley"><table cellpadding="0" cellspacing="0"><tr><td>{Image}</td><td nowrap>Toolbar Button</td><td><img class="TB_Button_Padding"></td></tr></table></div></td>
		<td><div class="TB_Button_On" title="Smiley"><table cellpadding="0" cellspacing="0"><tr><td><img class="TB_Button_Padding"></td><td nowrap>Toolbar Button</td><td><img class="TB_Button_Padding"></td></tr></table></div></td>

	These are samples of possible {Image} values:

		Strip - IE version:
			<div class="TB_Button_Image"><img src="strip.gif" style="top:-16px"></div>

		Strip : Firefox, Safari and Opera version
			<img class="TB_Button_Image" style="background-position: 0px -16px;background-image: url(strip.gif);">

		No-Strip : Browser independent:
			<img class="TB_Button_Image" src="smiley.gif">
*/
