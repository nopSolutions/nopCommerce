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

var FCKToolbarFontSizeCombo = function( tooltip, style )
{
	this.CommandName	= 'FontSize' ;
	this.Label		= this.GetLabel() ;
	this.Tooltip	= tooltip ? tooltip : this.Label ;
	this.Style		= style ? style : FCK_TOOLBARITEM_ICONTEXT ;

	this.DefaultLabel = FCKConfig.DefaultFontSizeLabel || '' ;

	this.FieldWidth = 70 ;
}

// Inherit from FCKToolbarSpecialCombo.
FCKToolbarFontSizeCombo.prototype = new FCKToolbarFontFormatCombo( false ) ;

FCKToolbarFontSizeCombo.prototype.GetLabel = function()
{
	return FCKLang.FontSize ;
}

FCKToolbarFontSizeCombo.prototype.GetStyles = function()
{
	var baseStyle = FCKStyles.GetStyle( '_FCK_Size' ) ;

	if ( !baseStyle )
	{
		alert( "The FCKConfig.CoreStyles['FontFace'] setting was not found. Please check the fckconfig.js file" ) ;
		return {} ;
	}

	var styles = {} ;

	var fonts = FCKConfig.FontSizes.split(';') ;

	for ( var i = 0 ; i < fonts.length ; i++ )
	{
		var fontParts = fonts[i].split('/') ;
		var font = fontParts[0] ;
		var caption = fontParts[1] || font ;

		var style = FCKTools.CloneObject( baseStyle ) ;
		style.SetVariable( 'Size', font ) ;
		style.Label = caption ;

		styles[ caption ] = style ;
	}

	return styles ;
}

FCKToolbarFontSizeCombo.prototype.RefreshActiveItems = FCKToolbarStyleCombo.prototype.RefreshActiveItems ;

FCKToolbarFontSizeCombo.prototype.StyleCombo_OnBeforeClick = FCKToolbarFontsCombo.prototype.StyleCombo_OnBeforeClick ;
