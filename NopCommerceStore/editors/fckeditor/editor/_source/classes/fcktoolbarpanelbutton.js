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
 * FCKToolbarPanelButton Class: represents a special button in the toolbar
 * that shows a panel when pressed.
 */

var FCKToolbarPanelButton = function( commandName, label, tooltip, style, icon )
{
	this.CommandName = commandName ;

	var oIcon ;

	if ( icon == null )
		oIcon = FCKConfig.SkinPath + 'toolbar/' + commandName.toLowerCase() + '.gif' ;
	else if ( typeof( icon ) == 'number' )
		oIcon = [ FCKConfig.SkinPath + 'fck_strip.gif', 16, icon ] ;

	var oUIButton = this._UIButton = new FCKToolbarButtonUI( commandName, label, tooltip, oIcon, style ) ;
	oUIButton._FCKToolbarPanelButton = this ;
	oUIButton.ShowArrow = true ;
	oUIButton.OnClick = FCKToolbarPanelButton_OnButtonClick ;
}

FCKToolbarPanelButton.prototype.TypeName = 'FCKToolbarPanelButton' ;

FCKToolbarPanelButton.prototype.Create = function( parentElement )
{
	parentElement.className += 'Menu' ;

	this._UIButton.Create( parentElement ) ;

	var oPanel = FCK.ToolbarSet.CurrentInstance.Commands.GetCommand( this.CommandName )._Panel ;
	this.RegisterPanel( oPanel ) ;
}

FCKToolbarPanelButton.prototype.RegisterPanel = function( oPanel )
{
	if ( oPanel._FCKToolbarPanelButton )
		return ;

	oPanel._FCKToolbarPanelButton = this ;

	var eLineDiv = oPanel.Document.body.appendChild( oPanel.Document.createElement( 'div' ) ) ;
	eLineDiv.style.position = 'absolute' ;
	eLineDiv.style.top = '0px' ;

	var eLine = oPanel._FCKToolbarPanelButtonLineDiv = eLineDiv.appendChild( oPanel.Document.createElement( 'IMG' ) ) ;
	eLine.className = 'TB_ConnectionLine' ;
	eLine.style.position = 'absolute' ;
//	eLine.style.backgroundColor = 'Red' ;
	eLine.src = FCK_SPACER_PATH ;

	oPanel.OnHide = FCKToolbarPanelButton_OnPanelHide ;
}

/*
	Events
*/

function FCKToolbarPanelButton_OnButtonClick( toolbarButton )
{
	var oButton = this._FCKToolbarPanelButton ;
	var e = oButton._UIButton.MainElement ;

	oButton._UIButton.ChangeState( FCK_TRISTATE_ON ) ;

	// oButton.LineImg.style.width = ( e.offsetWidth - 2 ) + 'px' ;

	var oCommand = FCK.ToolbarSet.CurrentInstance.Commands.GetCommand( oButton.CommandName ) ;
	var oPanel = oCommand._Panel ;
	oPanel._FCKToolbarPanelButtonLineDiv.style.width = ( e.offsetWidth - 2 ) + 'px' ;
	oCommand.Execute( 0, e.offsetHeight - 1, e ) ; // -1 to be over the border
}

function FCKToolbarPanelButton_OnPanelHide()
{
	var oMenuButton = this._FCKToolbarPanelButton ;
	oMenuButton._UIButton.ChangeState( FCK_TRISTATE_OFF ) ;
}

// The Panel Button works like a normal button so the refresh state functions
// defined for the normal button can be reused here.
FCKToolbarPanelButton.prototype.RefreshState	= FCKToolbarButton.prototype.RefreshState ;
FCKToolbarPanelButton.prototype.Enable			= FCKToolbarButton.prototype.Enable ;
FCKToolbarPanelButton.prototype.Disable			= FCKToolbarButton.prototype.Disable ;
