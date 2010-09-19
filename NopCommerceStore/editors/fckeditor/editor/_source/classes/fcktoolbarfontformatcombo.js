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

var FCKToolbarFontFormatCombo = function( tooltip, style )
{
	if ( tooltip === false )
		return ;

	this.CommandName = 'FontFormat' ;
	this.Label		= this.GetLabel() ;
	this.Tooltip	= tooltip ? tooltip : this.Label ;
	this.Style		= style ? style : FCK_TOOLBARITEM_ICONTEXT ;

	this.NormalLabel = 'Normal' ;

	this.PanelWidth = 190 ;

	this.DefaultLabel = FCKConfig.DefaultFontFormatLabel || '' ;
}

// Inherit from FCKToolbarSpecialCombo.
FCKToolbarFontFormatCombo.prototype = new FCKToolbarStyleCombo( false ) ;

FCKToolbarFontFormatCombo.prototype.GetLabel = function()
{
	return FCKLang.FontFormat ;
}

FCKToolbarFontFormatCombo.prototype.GetStyles = function()
{
	var styles = {} ;

	// Get the format names from the language file.
	var aNames = FCKLang['FontFormats'].split(';') ;
	var oNames = {
		p		: aNames[0],
		pre		: aNames[1],
		address	: aNames[2],
		h1		: aNames[3],
		h2		: aNames[4],
		h3		: aNames[5],
		h4		: aNames[6],
		h5		: aNames[7],
		h6		: aNames[8],
		div		: aNames[9] || ( aNames[0] + ' (DIV)')
	} ;

	// Get the available formats from the configuration file.
	var elements = FCKConfig.FontFormats.split(';') ;

	for ( var i = 0 ; i < elements.length ; i++ )
	{
		var elementName = elements[ i ] ;
		var style = FCKStyles.GetStyle( '_FCK_' + elementName ) ;
		if ( style )
		{
			style.Label = oNames[ elementName ] ;
			styles[ '_FCK_' + elementName ] = style ;
		}
		else
			alert( "The FCKConfig.CoreStyles['" + elementName + "'] setting was not found. Please check the fckconfig.js file" ) ;
	}

	return styles ;
}

FCKToolbarFontFormatCombo.prototype.RefreshActiveItems = function( targetSpecialCombo )
{
	var startElement = FCK.ToolbarSet.CurrentInstance.Selection.GetBoundaryParentElement( true ) ;

	if ( startElement )
	{
		var path = new FCKElementPath( startElement ) ;
		var blockElement = path.Block ;

		if ( blockElement )
		{
			for ( var i in targetSpecialCombo.Items )
			{
				var item = targetSpecialCombo.Items[i] ;
				var style = item.Style ;

				if ( style.CheckElementRemovable( blockElement ) )
				{
					targetSpecialCombo.SetLabel( style.Label ) ;
					return ;
				}
			}
		}
	}

	targetSpecialCombo.SetLabel( this.DefaultLabel ) ;
}

FCKToolbarFontFormatCombo.prototype.StyleCombo_OnBeforeClick = function( targetSpecialCombo )
{
	// Clear the current selection.
	targetSpecialCombo.DeselectAll() ;

	var startElement = FCK.ToolbarSet.CurrentInstance.Selection.GetBoundaryParentElement( true ) ;

	if ( startElement )
	{
		var path = new FCKElementPath( startElement ) ;
		var blockElement = path.Block ;

		for ( var i in targetSpecialCombo.Items )
		{
			var item = targetSpecialCombo.Items[i] ;
			var style = item.Style ;

			if ( style.CheckElementRemovable( blockElement ) )
			{
				targetSpecialCombo.SelectItem( item ) ;
				return ;
			}
		}
	}
}
