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
 * Defines and renders a menu items in a menu block.
 */

var FCKMenuItem = function( parentMenuBlock, name, label, iconPathOrStripInfoArray, isDisabled, customData )
{
	this.Name		= name ;
	this.Label		= label || name ;
	this.IsDisabled	= isDisabled ;

	this.Icon = new FCKIcon( iconPathOrStripInfoArray ) ;

	this.SubMenu			= new FCKMenuBlockPanel() ;
	this.SubMenu.Parent		= parentMenuBlock ;
	this.SubMenu.OnClick	= FCKTools.CreateEventListener( FCKMenuItem_SubMenu_OnClick, this ) ;
	this.CustomData = customData ;

	if ( FCK.IECleanup )
		FCK.IECleanup.AddItem( this, FCKMenuItem_Cleanup ) ;
}


FCKMenuItem.prototype.AddItem = function( name, label, iconPathOrStripInfoArrayOrIndex, isDisabled, customData )
{
	this.HasSubMenu = true ;
	return this.SubMenu.AddItem( name, label, iconPathOrStripInfoArrayOrIndex, isDisabled, customData ) ;
}

FCKMenuItem.prototype.AddSeparator = function()
{
	this.SubMenu.AddSeparator() ;
}

FCKMenuItem.prototype.Create = function( parentTable )
{
	var bHasSubMenu = this.HasSubMenu ;

	var oDoc = FCKTools.GetElementDocument( parentTable ) ;

	// Add a row in the table to hold the menu item.
	var r = this.MainElement = parentTable.insertRow(-1) ;
	r.className = this.IsDisabled ? 'MN_Item_Disabled' : 'MN_Item' ;

	// Set the row behavior.
	if ( !this.IsDisabled )
	{
		FCKTools.AddEventListenerEx( r, 'mouseover', FCKMenuItem_OnMouseOver, [ this ] ) ;
		FCKTools.AddEventListenerEx( r, 'click', FCKMenuItem_OnClick, [ this ] ) ;

		if ( !bHasSubMenu )
			FCKTools.AddEventListenerEx( r, 'mouseout', FCKMenuItem_OnMouseOut, [ this ] ) ;
	}

	// Create the icon cell.
	var eCell = r.insertCell(-1) ;
	eCell.className = 'MN_Icon' ;
	eCell.appendChild( this.Icon.CreateIconElement( oDoc ) ) ;

	// Create the label cell.
	eCell = r.insertCell(-1) ;
	eCell.className = 'MN_Label' ;
	eCell.noWrap = true ;
	eCell.appendChild( oDoc.createTextNode( this.Label ) ) ;

	// Create the arrow cell and setup the sub menu panel (if needed).
	eCell = r.insertCell(-1) ;
	if ( bHasSubMenu )
	{
		eCell.className = 'MN_Arrow' ;

		// The arrow is a fixed size image.
		var eArrowImg = eCell.appendChild( oDoc.createElement( 'IMG' ) ) ;
		eArrowImg.src = FCK_IMAGES_PATH + 'arrow_' + FCKLang.Dir + '.gif' ;
		eArrowImg.width	 = 4 ;
		eArrowImg.height = 7 ;

		this.SubMenu.Create() ;
		this.SubMenu.Panel.OnHide = FCKTools.CreateEventListener( FCKMenuItem_SubMenu_OnHide, this ) ;
	}
}

FCKMenuItem.prototype.Activate = function()
{
	this.MainElement.className = 'MN_Item_Over' ;

	if ( this.HasSubMenu )
	{
		// Show the child menu block. The ( +2, -2 ) correction is done because
		// of the padding in the skin. It is not a good solution because one
		// could change the skin and so the final result would not be accurate.
		// For now it is ok because we are controlling the skin.
		this.SubMenu.Show( this.MainElement.offsetWidth + 2, -2, this.MainElement ) ;
	}

	FCKTools.RunFunction( this.OnActivate, this ) ;
}

FCKMenuItem.prototype.Deactivate = function()
{
	this.MainElement.className = 'MN_Item' ;

	if ( this.HasSubMenu )
		this.SubMenu.Hide() ;
}

/* Events */

function FCKMenuItem_SubMenu_OnClick( clickedItem, listeningItem )
{
	FCKTools.RunFunction( listeningItem.OnClick, listeningItem, [ clickedItem ] ) ;
}

function FCKMenuItem_SubMenu_OnHide( menuItem )
{
	menuItem.Deactivate() ;
}

function FCKMenuItem_OnClick( ev, menuItem )
{
	if ( menuItem.HasSubMenu )
		menuItem.Activate() ;
	else
	{
		menuItem.Deactivate() ;
		FCKTools.RunFunction( menuItem.OnClick, menuItem, [ menuItem ] ) ;
	}
}

function FCKMenuItem_OnMouseOver( ev, menuItem )
{
	menuItem.Activate() ;
}

function FCKMenuItem_OnMouseOut( ev, menuItem )
{
	menuItem.Deactivate() ;
}

function FCKMenuItem_Cleanup()
{
	this.MainElement = null ;
}
