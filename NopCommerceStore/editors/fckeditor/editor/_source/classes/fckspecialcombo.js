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
 * FCKSpecialCombo Class: represents a special combo.
 */

var FCKSpecialCombo = function( caption, fieldWidth, panelWidth, panelMaxHeight, parentWindow )
{
	// Default properties values.
	this.FieldWidth		= fieldWidth || 100 ;
	this.PanelWidth		= panelWidth || 150 ;
	this.PanelMaxHeight	= panelMaxHeight || 150 ;
	this.Label			= '&nbsp;' ;
	this.Caption		= caption ;
	this.Tooltip		= caption ;
	this.Style			= FCK_TOOLBARITEM_ICONTEXT ;

	this.Enabled = true ;

	this.Items = new Object() ;

	this._Panel = new FCKPanel( parentWindow || window ) ;
	this._Panel.AppendStyleSheet( FCKConfig.SkinEditorCSS ) ;
	this._PanelBox = this._Panel.MainNode.appendChild( this._Panel.Document.createElement( 'DIV' ) ) ;
	this._PanelBox.className = 'SC_Panel' ;
	this._PanelBox.style.width = this.PanelWidth + 'px' ;

	this._PanelBox.innerHTML = '<table cellpadding="0" cellspacing="0" width="100%" style="TABLE-LAYOUT: fixed"><tr><td nowrap></td></tr></table>' ;

	this._ItemsHolderEl = this._PanelBox.getElementsByTagName('TD')[0] ;

	if ( FCK.IECleanup )
		FCK.IECleanup.AddItem( this, FCKSpecialCombo_Cleanup ) ;

//	this._Panel.StyleSheet = FCKConfig.SkinPath + 'fck_contextmenu.css' ;
//	this._Panel.Create() ;
//	this._Panel.PanelDiv.className += ' SC_Panel' ;
//	this._Panel.PanelDiv.innerHTML = '<table cellpadding="0" cellspacing="0" width="100%" style="TABLE-LAYOUT: fixed"><tr><td nowrap></td></tr></table>' ;
//	this._ItemsHolderEl = this._Panel.PanelDiv.getElementsByTagName('TD')[0] ;
}

function FCKSpecialCombo_ItemOnMouseOver()
{
	this.className += ' SC_ItemOver' ;
}

function FCKSpecialCombo_ItemOnMouseOut()
{
	this.className = this.originalClass ;
}

function FCKSpecialCombo_ItemOnClick( ev, specialCombo, itemId )
{
	this.className = this.originalClass ;

	specialCombo._Panel.Hide() ;

	specialCombo.SetLabel( this.FCKItemLabel ) ;

	if ( typeof( specialCombo.OnSelect ) == 'function' )
		specialCombo.OnSelect( itemId, this ) ;
}

FCKSpecialCombo.prototype.ClearItems = function ()
{
	if ( this.Items )
		this.Items = {} ;

	var itemsholder = this._ItemsHolderEl ;
	while ( itemsholder.firstChild )
		itemsholder.removeChild( itemsholder.firstChild ) ;
}

FCKSpecialCombo.prototype.AddItem = function( id, html, label, bgColor )
{
	// <div class="SC_Item" onmouseover="this.className='SC_Item SC_ItemOver';" onmouseout="this.className='SC_Item';"><b>Bold 1</b></div>
	var oDiv = this._ItemsHolderEl.appendChild( this._Panel.Document.createElement( 'DIV' ) ) ;
	oDiv.className = oDiv.originalClass = 'SC_Item' ;
	oDiv.innerHTML = html ;
	oDiv.FCKItemLabel = label || id ;
	oDiv.Selected = false ;

	// In IE, the width must be set so the borders are shown correctly when the content overflows.
	if ( FCKBrowserInfo.IsIE )
		oDiv.style.width = '100%' ;

	if ( bgColor )
		oDiv.style.backgroundColor = bgColor ;

	FCKTools.AddEventListenerEx( oDiv, 'mouseover', FCKSpecialCombo_ItemOnMouseOver ) ;
	FCKTools.AddEventListenerEx( oDiv, 'mouseout', FCKSpecialCombo_ItemOnMouseOut ) ;
	FCKTools.AddEventListenerEx( oDiv, 'click', FCKSpecialCombo_ItemOnClick, [ this, id ] ) ;

	this.Items[ id.toString().toLowerCase() ] = oDiv ;

	return oDiv ;
}

FCKSpecialCombo.prototype.SelectItem = function( item )
{
	if ( typeof item == 'string' )
		item = this.Items[ item.toString().toLowerCase() ] ;

	if ( item )
	{
		item.className = item.originalClass = 'SC_ItemSelected' ;
		item.Selected = true ;
	}
}

FCKSpecialCombo.prototype.SelectItemByLabel = function( itemLabel, setLabel )
{
	for ( var id in this.Items )
	{
		var oDiv = this.Items[id] ;

		if ( oDiv.FCKItemLabel == itemLabel )
		{
			oDiv.className = oDiv.originalClass = 'SC_ItemSelected' ;
			oDiv.Selected = true ;

			if ( setLabel )
				this.SetLabel( itemLabel ) ;
		}
	}
}

FCKSpecialCombo.prototype.DeselectAll = function( clearLabel )
{
	for ( var i in this.Items )
	{
		if ( !this.Items[i] ) continue;
		this.Items[i].className = this.Items[i].originalClass = 'SC_Item' ;
		this.Items[i].Selected = false ;
	}

	if ( clearLabel )
		this.SetLabel( '' ) ;
}

FCKSpecialCombo.prototype.SetLabelById = function( id )
{
	id = id ? id.toString().toLowerCase() : '' ;

	var oDiv = this.Items[ id ] ;
	this.SetLabel( oDiv ? oDiv.FCKItemLabel : '' ) ;
}

FCKSpecialCombo.prototype.SetLabel = function( text )
{
	text = ( !text || text.length == 0 ) ? '&nbsp;' : text ;

	if ( text == this.Label )
		return ;

	this.Label = text ;

	var labelEl = this._LabelEl ;
	if ( labelEl )
	{
		labelEl.innerHTML = text ;

		// It may happen that the label is some HTML, including tags. This
		// would be a problem because when the user click on those tags, the
		// combo will get the selection from the editing area. So we must
		// disable any kind of selection here.
		FCKTools.DisableSelection( labelEl ) ;
	}
}

FCKSpecialCombo.prototype.SetEnabled = function( isEnabled )
{
	this.Enabled = isEnabled ;

	// In IE it can happen when the page is reloaded that _OuterTable is null, so check its existence
	if ( this._OuterTable )
		this._OuterTable.className = isEnabled ? '' : 'SC_FieldDisabled' ;
}

FCKSpecialCombo.prototype.Create = function( targetElement )
{
	var oDoc = FCKTools.GetElementDocument( targetElement ) ;
	var eOuterTable = this._OuterTable = targetElement.appendChild( oDoc.createElement( 'TABLE' ) ) ;
	eOuterTable.cellPadding = 0 ;
	eOuterTable.cellSpacing = 0 ;

	eOuterTable.insertRow(-1) ;

	var sClass ;
	var bShowLabel ;

	switch ( this.Style )
	{
		case FCK_TOOLBARITEM_ONLYICON :
			sClass = 'TB_ButtonType_Icon' ;
			bShowLabel = false;
			break ;
		case FCK_TOOLBARITEM_ONLYTEXT :
			sClass = 'TB_ButtonType_Text' ;
			bShowLabel = false;
			break ;
		case FCK_TOOLBARITEM_ICONTEXT :
			bShowLabel = true;
			break ;
	}

	if ( this.Caption && this.Caption.length > 0 && bShowLabel )
	{
		var oCaptionCell = eOuterTable.rows[0].insertCell(-1) ;
		oCaptionCell.innerHTML = this.Caption ;
		oCaptionCell.className = 'SC_FieldCaption' ;
	}

	// Create the main DIV element.
	var oField = FCKTools.AppendElement( eOuterTable.rows[0].insertCell(-1), 'div' ) ;
	if ( bShowLabel )
	{
		oField.className = 'SC_Field' ;
		oField.style.width = this.FieldWidth + 'px' ;
		oField.innerHTML = '<table width="100%" cellpadding="0" cellspacing="0" style="TABLE-LAYOUT: fixed;"><tbody><tr><td class="SC_FieldLabel"><label>&nbsp;</label></td><td class="SC_FieldButton">&nbsp;</td></tr></tbody></table>' ;

		this._LabelEl = oField.getElementsByTagName('label')[0] ;		// Memory Leak
		this._LabelEl.innerHTML = this.Label ;
	}
	else
	{
		oField.className = 'TB_Button_Off' ;
		//oField.innerHTML = '<span className="SC_FieldCaption">' + this.Caption + '<table cellpadding="0" cellspacing="0" style="TABLE-LAYOUT: fixed;"><tbody><tr><td class="SC_FieldButton" style="border-left: none;">&nbsp;</td></tr></tbody></table>' ;
		//oField.innerHTML = '<table cellpadding="0" cellspacing="0" style="TABLE-LAYOUT: fixed;"><tbody><tr><td class="SC_FieldButton" style="border-left: none;">&nbsp;</td></tr></tbody></table>' ;

		// Gets the correct CSS class to use for the specified style (param).
		oField.innerHTML = '<table title="' + this.Tooltip + '" class="' + sClass + '" cellspacing="0" cellpadding="0" border="0">' +
				'<tr>' +
					//'<td class="TB_Icon"><img src="' + FCKConfig.SkinPath + 'toolbar/' + this.Command.Name.toLowerCase() + '.gif" width="21" height="21"></td>' +
					'<td><img class="TB_Button_Padding" src="' + FCK_SPACER_PATH + '" /></td>' +
					'<td class="TB_Text">' + this.Caption + '</td>' +
					'<td><img class="TB_Button_Padding" src="' + FCK_SPACER_PATH + '" /></td>' +
					'<td class="TB_ButtonArrow"><img src="' + FCKConfig.SkinPath + 'images/toolbar.buttonarrow.gif" width="5" height="3"></td>' +
					'<td><img class="TB_Button_Padding" src="' + FCK_SPACER_PATH + '" /></td>' +
				'</tr>' +
			'</table>' ;
	}


	// Events Handlers

	FCKTools.AddEventListenerEx( oField, 'mouseover', FCKSpecialCombo_OnMouseOver, this ) ;
	FCKTools.AddEventListenerEx( oField, 'mouseout', FCKSpecialCombo_OnMouseOut, this ) ;
	FCKTools.AddEventListenerEx( oField, 'click', FCKSpecialCombo_OnClick, this ) ;

	FCKTools.DisableSelection( this._Panel.Document.body ) ;
}

function FCKSpecialCombo_Cleanup()
{
	this._LabelEl = null ;
	this._OuterTable = null ;
	this._ItemsHolderEl = null ;
	this._PanelBox = null ;

	if ( this.Items )
	{
		for ( var key in this.Items )
			this.Items[key] = null ;
	}
}

function FCKSpecialCombo_OnMouseOver( ev, specialCombo )
{
	if ( specialCombo.Enabled )
	{
		switch ( specialCombo.Style )
		{
			case FCK_TOOLBARITEM_ONLYICON :
				this.className = 'TB_Button_On_Over';
				break ;
			case FCK_TOOLBARITEM_ONLYTEXT :
				this.className = 'TB_Button_On_Over';
				break ;
			case FCK_TOOLBARITEM_ICONTEXT :
				this.className = 'SC_Field SC_FieldOver' ;
				break ;
		}
	}
}

function FCKSpecialCombo_OnMouseOut( ev, specialCombo )
{
	switch ( specialCombo.Style )
	{
		case FCK_TOOLBARITEM_ONLYICON :
			this.className = 'TB_Button_Off';
			break ;
		case FCK_TOOLBARITEM_ONLYTEXT :
			this.className = 'TB_Button_Off';
			break ;
		case FCK_TOOLBARITEM_ICONTEXT :
			this.className='SC_Field' ;
			break ;
	}
}

function FCKSpecialCombo_OnClick( e, specialCombo )
{
	// For Mozilla we must stop the event propagation to avoid it hiding
	// the panel because of a click outside of it.
//	if ( e )
//	{
//		e.stopPropagation() ;
//		FCKPanelEventHandlers.OnDocumentClick( e ) ;
//	}

	if ( specialCombo.Enabled )
	{
		var oPanel			= specialCombo._Panel ;
		var oPanelBox		= specialCombo._PanelBox ;
		var oItemsHolder	= specialCombo._ItemsHolderEl ;
		var iMaxHeight		= specialCombo.PanelMaxHeight ;

		if ( specialCombo.OnBeforeClick )
			specialCombo.OnBeforeClick( specialCombo ) ;

		// This is a tricky thing. We must call the "Load" function, otherwise
		// it will not be possible to retrieve "oItemsHolder.offsetHeight" (IE only).
		if ( FCKBrowserInfo.IsIE )
			oPanel.Preload( 0, this.offsetHeight, this ) ;

		if ( oItemsHolder.offsetHeight > iMaxHeight )
//		{
			oPanelBox.style.height = iMaxHeight + 'px' ;

//			if ( FCKBrowserInfo.IsGecko )
//				oPanelBox.style.overflow = '-moz-scrollbars-vertical' ;
//		}
		else
			oPanelBox.style.height = '' ;

//		oPanel.PanelDiv.style.width = specialCombo.PanelWidth + 'px' ;

		oPanel.Show( 0, this.offsetHeight, this ) ;
	}

//	return false ;
}

/*
Sample Combo Field HTML output:

<div class="SC_Field" style="width: 80px;">
	<table width="100%" cellpadding="0" cellspacing="0" style="table-layout: fixed;">
		<tbody>
			<tr>
				<td class="SC_FieldLabel"><label>&nbsp;</label></td>
				<td class="SC_FieldButton">&nbsp;</td>
			</tr>
		</tbody>
	</table>
</div>
*/
