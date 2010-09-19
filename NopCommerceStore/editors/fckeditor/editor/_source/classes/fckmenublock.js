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
 * Renders a list of menu items.
 */

var FCKMenuBlock = function()
{
	this._Items	= new Array() ;
}


FCKMenuBlock.prototype.Count = function()
{
	return this._Items.length ;
}

FCKMenuBlock.prototype.AddItem = function( name, label, iconPathOrStripInfoArrayOrIndex, isDisabled, customData )
{
	var oItem = new FCKMenuItem( this, name, label, iconPathOrStripInfoArrayOrIndex, isDisabled, customData ) ;

	oItem.OnClick		= FCKTools.CreateEventListener( FCKMenuBlock_Item_OnClick, this ) ;
	oItem.OnActivate	= FCKTools.CreateEventListener( FCKMenuBlock_Item_OnActivate, this ) ;

	this._Items.push( oItem ) ;

	return oItem ;
}

FCKMenuBlock.prototype.AddSeparator = function()
{
	this._Items.push( new FCKMenuSeparator() ) ;
}

FCKMenuBlock.prototype.RemoveAllItems = function()
{
	this._Items = new Array() ;

	var eItemsTable = this._ItemsTable ;
	if ( eItemsTable )
	{
		while ( eItemsTable.rows.length > 0 )
			eItemsTable.deleteRow( 0 ) ;
	}
}

FCKMenuBlock.prototype.Create = function( parentElement )
{
	if ( !this._ItemsTable )
	{
		if ( FCK.IECleanup )
			FCK.IECleanup.AddItem( this, FCKMenuBlock_Cleanup ) ;

		this._Window = FCKTools.GetElementWindow( parentElement ) ;

		var oDoc = FCKTools.GetElementDocument( parentElement ) ;

		var eTable = parentElement.appendChild( oDoc.createElement( 'table' ) ) ;
		eTable.cellPadding = 0 ;
		eTable.cellSpacing = 0 ;

		FCKTools.DisableSelection( eTable ) ;

		var oMainElement = eTable.insertRow(-1).insertCell(-1) ;
		oMainElement.className = 'MN_Menu' ;

		var eItemsTable = this._ItemsTable = oMainElement.appendChild( oDoc.createElement( 'table' ) ) ;
		eItemsTable.cellPadding = 0 ;
		eItemsTable.cellSpacing = 0 ;
	}

	for ( var i = 0 ; i < this._Items.length ; i++ )
		this._Items[i].Create( this._ItemsTable ) ;
}

/* Events */

function FCKMenuBlock_Item_OnClick( clickedItem, menuBlock )
{
	if ( menuBlock.Hide )
		menuBlock.Hide() ;

	FCKTools.RunFunction( menuBlock.OnClick, menuBlock, [ clickedItem ] ) ;
}

function FCKMenuBlock_Item_OnActivate( menuBlock )
{
	var oActiveItem = menuBlock._ActiveItem ;

	if ( oActiveItem && oActiveItem != this )
	{
		// Set the focus to this menu block window (to fire OnBlur on opened panels).
		if ( !FCKBrowserInfo.IsIE && oActiveItem.HasSubMenu && !this.HasSubMenu )
		{
			menuBlock._Window.focus() ;

			// Due to the event model provided by Opera, we need to set
			// HasFocus here as the above focus() call will not fire the focus
			// event in the panel immediately (#1200).
			menuBlock.Panel.HasFocus = true ;
		}

		oActiveItem.Deactivate() ;
	}

	menuBlock._ActiveItem = this ;
}

function FCKMenuBlock_Cleanup()
{
	this._Window = null ;
	this._ItemsTable = null ;
}

// ################# //

var FCKMenuSeparator = function()
{}

FCKMenuSeparator.prototype.Create = function( parentTable )
{
	var oDoc = FCKTools.GetElementDocument( parentTable ) ;

	var r = parentTable.insertRow(-1) ;

	var eCell = r.insertCell(-1) ;
	eCell.className = 'MN_Separator MN_Icon' ;

	eCell = r.insertCell(-1) ;
	eCell.className = 'MN_Separator' ;
	eCell.appendChild( oDoc.createElement( 'DIV' ) ).className = 'MN_Separator_Line' ;

	eCell = r.insertCell(-1) ;
	eCell.className = 'MN_Separator' ;
	eCell.appendChild( oDoc.createElement( 'DIV' ) ).className = 'MN_Separator_Line' ;
}
