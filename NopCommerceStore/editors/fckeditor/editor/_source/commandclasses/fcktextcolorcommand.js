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
 * FCKTextColorCommand Class: represents the text color comand. It shows the
 * color selection panel.
 */

// FCKTextColorCommand Constructor
//		type: can be 'ForeColor' or 'BackColor'.
var FCKTextColorCommand = function( type )
{
	this.Name = type == 'ForeColor' ? 'TextColor' : 'BGColor' ;
	this.Type = type ;

	var oWindow ;

	if ( FCKBrowserInfo.IsIE )
		oWindow = window ;
	else if ( FCK.ToolbarSet._IFrame )
		oWindow = FCKTools.GetElementWindow( FCK.ToolbarSet._IFrame ) ;
	else
		oWindow = window.parent ;

	this._Panel = new FCKPanel( oWindow ) ;
	this._Panel.AppendStyleSheet( FCKConfig.SkinEditorCSS ) ;
	this._Panel.MainNode.className = 'FCK_Panel' ;
	this._CreatePanelBody( this._Panel.Document, this._Panel.MainNode ) ;
	FCK.ToolbarSet.ToolbarItems.GetItem( this.Name ).RegisterPanel( this._Panel ) ;

	FCKTools.DisableSelection( this._Panel.Document.body ) ;
}

FCKTextColorCommand.prototype.Execute = function( panelX, panelY, relElement )
{
	// Show the Color Panel at the desired position.
	this._Panel.Show( panelX, panelY, relElement ) ;
}

FCKTextColorCommand.prototype.SetColor = function( color )
{
	FCKUndo.SaveUndoStep() ;

	var style = FCKStyles.GetStyle( '_FCK_' +
		( this.Type == 'ForeColor' ? 'Color' : 'BackColor' ) ) ;

	if ( !color || color.length == 0 )
		FCK.Styles.RemoveStyle( style ) ;
	else
	{
		style.SetVariable( 'Color', color ) ;
		FCKStyles.ApplyStyle( style ) ;
	}

	FCKUndo.SaveUndoStep() ;

	FCK.Focus() ;
	FCK.Events.FireEvent( 'OnSelectionChange' ) ;
}

FCKTextColorCommand.prototype.GetState = function()
{
	if ( FCK.EditMode != FCK_EDITMODE_WYSIWYG )
		return FCK_TRISTATE_DISABLED ;
	return FCK_TRISTATE_OFF ;
}

function FCKTextColorCommand_OnMouseOver()
{
	this.className = 'ColorSelected' ;
}

function FCKTextColorCommand_OnMouseOut()
{
	this.className = 'ColorDeselected' ;
}

function FCKTextColorCommand_OnClick( ev, command, color )
{
	this.className = 'ColorDeselected' ;
	command.SetColor( color ) ;
	command._Panel.Hide() ;
}

function FCKTextColorCommand_AutoOnClick( ev, command )
{
	this.className = 'ColorDeselected' ;
	command.SetColor( '' ) ;
	command._Panel.Hide() ;
}

function FCKTextColorCommand_MoreOnClick( ev, command )
{
	this.className = 'ColorDeselected' ;
	command._Panel.Hide() ;
	FCKDialog.OpenDialog( 'FCKDialog_Color', FCKLang.DlgColorTitle, 'dialog/fck_colorselector.html', 410, 320,
			FCKTools.Bind( command, command.SetColor ) ) ;
}

FCKTextColorCommand.prototype._CreatePanelBody = function( targetDocument, targetDiv )
{
	function CreateSelectionDiv()
	{
		var oDiv = targetDocument.createElement( "DIV" ) ;
		oDiv.className = 'ColorDeselected' ;
		FCKTools.AddEventListenerEx( oDiv, 'mouseover', FCKTextColorCommand_OnMouseOver ) ;
		FCKTools.AddEventListenerEx( oDiv, 'mouseout', FCKTextColorCommand_OnMouseOut ) ;

		return oDiv ;
	}

	// Create the Table that will hold all colors.
	var oTable = targetDiv.appendChild( targetDocument.createElement( "TABLE" ) ) ;
	oTable.className = 'ForceBaseFont' ;		// Firefox 1.5 Bug.
	oTable.style.tableLayout = 'fixed' ;
	oTable.cellPadding = 0 ;
	oTable.cellSpacing = 0 ;
	oTable.border = 0 ;
	oTable.width = 150 ;

	var oCell = oTable.insertRow(-1).insertCell(-1) ;
	oCell.colSpan = 8 ;

	// Create the Button for the "Automatic" color selection.
	var oDiv = oCell.appendChild( CreateSelectionDiv() ) ;
	oDiv.innerHTML =
		'<table cellspacing="0" cellpadding="0" width="100%" border="0">\
			<tr>\
				<td><div class="ColorBoxBorder"><div class="ColorBox" style="background-color: #000000"></div></div></td>\
				<td nowrap width="100%" align="center">' + FCKLang.ColorAutomatic + '</td>\
			</tr>\
		</table>' ;

	FCKTools.AddEventListenerEx( oDiv, 'click', FCKTextColorCommand_AutoOnClick, this ) ;

	// Dirty hack for Opera, Safari and Firefox 3.
	if ( !FCKBrowserInfo.IsIE )
		oDiv.style.width = '96%' ;

	// Create an array of colors based on the configuration file.
	var aColors = FCKConfig.FontColors.toString().split(',') ;

	// Create the colors table based on the array.
	var iCounter = 0 ;
	while ( iCounter < aColors.length )
	{
		var oRow = oTable.insertRow(-1) ;

		for ( var i = 0 ; i < 8 ; i++, iCounter++ )
		{
			// The div will be created even if no more colors are available.
			// Extra divs will be hidden later in the code. (#1597)
			if ( iCounter < aColors.length )
			{
				var colorParts = aColors[iCounter].split('/') ;
				var colorValue = '#' + colorParts[0] ;
				var colorName = colorParts[1] || colorValue ;
			}

			oDiv = oRow.insertCell(-1).appendChild( CreateSelectionDiv() ) ;
			oDiv.innerHTML = '<div class="ColorBoxBorder"><div class="ColorBox" style="background-color: ' + colorValue + '"></div></div>' ;

			if ( iCounter >= aColors.length )
				oDiv.style.visibility = 'hidden' ;
			else
				FCKTools.AddEventListenerEx( oDiv, 'click', FCKTextColorCommand_OnClick, [ this, colorName ] ) ;
		}
	}

	// Create the Row and the Cell for the "More Colors..." button.
	if ( FCKConfig.EnableMoreFontColors )
	{
		oCell = oTable.insertRow(-1).insertCell(-1) ;
		oCell.colSpan = 8 ;

		oDiv = oCell.appendChild( CreateSelectionDiv() ) ;
		oDiv.innerHTML = '<table width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td nowrap align="center">' + FCKLang.ColorMoreColors + '</td></tr></table>' ;

		FCKTools.AddEventListenerEx( oDiv, 'click', FCKTextColorCommand_MoreOnClick, this ) ;

		// Dirty hack for Opera, Safari and Firefox 3.
		if ( !FCKBrowserInfo.IsIE )
			oDiv.style.width = '96%' ;
	}
}
