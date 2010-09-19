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
 * FCKToolbarPanelButton Class: Handles the Fonts combo selector.
 */

var FCKToolbarFontsCombo = function( tooltip, style )
{
	this.CommandName	= 'FontName' ;
	this.Label		= this.GetLabel() ;
	this.Tooltip	= tooltip ? tooltip : this.Label ;
	this.Style		= style ? style : FCK_TOOLBARITEM_ICONTEXT ;

	this.DefaultLabel = FCKConfig.DefaultFontLabel || '' ;
}

// Inherit from FCKToolbarSpecialCombo.
FCKToolbarFontsCombo.prototype = new FCKToolbarFontFormatCombo( false ) ;

FCKToolbarFontsCombo.prototype.GetLabel = function()
{
	return FCKLang.Font ;
}

FCKToolbarFontsCombo.prototype.GetStyles = function()
{
	var baseStyle = FCKStyles.GetStyle( '_FCK_FontFace' ) ;

	if ( !baseStyle )
	{
		alert( "The FCKConfig.CoreStyles['Size'] setting was not found. Please check the fckconfig.js file" ) ;
		return {} ;
	}

	var styles = {} ;

	var fonts = FCKConfig.FontNames.split(';') ;

	for ( var i = 0 ; i < fonts.length ; i++ )
	{
		var fontParts = fonts[i].split('/') ;
		var font = fontParts[0] ;
		var caption = fontParts[1] || font ;

		var style = FCKTools.CloneObject( baseStyle ) ;

		style.SetVariable( 'Font', font ) ;
		style.Label = caption ;

		styles[ caption ] = style ;
	}

	return styles ;
}

FCKToolbarFontsCombo.prototype.RefreshActiveItems = FCKToolbarStyleCombo.prototype.RefreshActiveItems ;

FCKToolbarFontsCombo.prototype.StyleCombo_OnBeforeClick = function( targetSpecialCombo )
{
	// Clear the current selection.
	targetSpecialCombo.DeselectAll() ;

	var startElement = FCKSelection.GetBoundaryParentElement( true ) ;

	if ( startElement )
	{
		var path = new FCKElementPath( startElement ) ;

		for ( var i in targetSpecialCombo.Items )
		{
			var item = targetSpecialCombo.Items[i] ;
			var style = item.Style ;

			if ( style.CheckActive( path ) )
			{
				targetSpecialCombo.SelectItem( item ) ;
				return ;
			}
		}
	}
}
