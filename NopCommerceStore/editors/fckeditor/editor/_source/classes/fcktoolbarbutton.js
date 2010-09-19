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
 * FCKToolbarButton Class: represents a button in the toolbar.
 */

var FCKToolbarButton = function( commandName, label, tooltip, style, sourceView, contextSensitive, icon )
{
	this.CommandName		= commandName ;
	this.Label				= label ;
	this.Tooltip			= tooltip ;
	this.Style				= style ;
	this.SourceView			= sourceView ? true : false ;
	this.ContextSensitive	= contextSensitive ? true : false ;

	if ( icon == null )
		this.IconPath = FCKConfig.SkinPath + 'toolbar/' + commandName.toLowerCase() + '.gif' ;
	else if ( typeof( icon ) == 'number' )
		this.IconPath = [ FCKConfig.SkinPath + 'fck_strip.gif', 16, icon ] ;
	else
		this.IconPath = icon ;
}

FCKToolbarButton.prototype.Create = function( targetElement )
{
	this._UIButton = new FCKToolbarButtonUI( this.CommandName, this.Label, this.Tooltip, this.IconPath, this.Style ) ;
	this._UIButton.OnClick = this.Click ;
	this._UIButton._ToolbarButton = this ;
	this._UIButton.Create( targetElement ) ;
}

FCKToolbarButton.prototype.RefreshState = function()
{
	var uiButton = this._UIButton ;

	if ( !uiButton )
		return ;

	// Gets the actual state.
	var eState = FCK.ToolbarSet.CurrentInstance.Commands.GetCommand( this.CommandName ).GetState() ;

	// If there are no state changes than do nothing and return.
	if ( eState == uiButton.State ) return ;

	// Sets the actual state.
	uiButton.ChangeState( eState ) ;
}

FCKToolbarButton.prototype.Click = function()
{
	var oToolbarButton = this._ToolbarButton || this ;
	FCK.ToolbarSet.CurrentInstance.Commands.GetCommand( oToolbarButton.CommandName ).Execute() ;
}

FCKToolbarButton.prototype.Enable = function()
{
	this.RefreshState() ;
}

FCKToolbarButton.prototype.Disable = function()
{
	// Sets the actual state.
	this._UIButton.ChangeState( FCK_TRISTATE_DISABLED ) ;
}
