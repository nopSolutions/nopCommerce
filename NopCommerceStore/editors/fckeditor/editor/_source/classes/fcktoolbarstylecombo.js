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

var FCKToolbarStyleCombo = function( tooltip, style )
{
	if ( tooltip === false )
		return ;

	this.CommandName = 'Style' ;
	this.Label		= this.GetLabel() ;
	this.Tooltip	= tooltip ? tooltip : this.Label ;
	this.Style		= style ? style : FCK_TOOLBARITEM_ICONTEXT ;

	this.DefaultLabel = FCKConfig.DefaultStyleLabel || '' ;
}

// Inherit from FCKToolbarSpecialCombo.
FCKToolbarStyleCombo.prototype = new FCKToolbarSpecialCombo ;

FCKToolbarStyleCombo.prototype.GetLabel = function()
{
	return FCKLang.Style ;
}

FCKToolbarStyleCombo.prototype.GetStyles = function()
{
	var styles = {} ;
	var allStyles = FCK.ToolbarSet.CurrentInstance.Styles.GetStyles() ;

	for ( var styleName in allStyles )
	{
		var style = allStyles[ styleName ] ;
		if ( !style.IsCore )
			styles[ styleName ] = style ;
	}
	return styles ;
}

FCKToolbarStyleCombo.prototype.CreateItems = function( targetSpecialCombo )
{
	var targetDoc = targetSpecialCombo._Panel.Document ;

	// Add the Editor Area CSS to the panel so the style classes are previewed correctly.
	FCKTools.AppendStyleSheet( targetDoc, FCKConfig.ToolbarComboPreviewCSS ) ;
	FCKTools.AppendStyleString( targetDoc, FCKConfig.EditorAreaStyles ) ;
	targetDoc.body.className += ' ForceBaseFont' ;

	// Add ID and Class to the body.
	FCKConfig.ApplyBodyAttributes( targetDoc.body ) ;

	// Get the styles list.
	var styles = this.GetStyles() ;

	for ( var styleName in styles )
	{
		var style = styles[ styleName ] ;

		// Object type styles have no preview.
		var caption = style.GetType() == FCK_STYLE_OBJECT ?
			styleName :
			FCKToolbarStyleCombo_BuildPreview( style, style.Label || styleName ) ;

		var item = targetSpecialCombo.AddItem( styleName, caption ) ;

		item.Style = style ;
	}

	// We must prepare the list before showing it.
	targetSpecialCombo.OnBeforeClick = this.StyleCombo_OnBeforeClick ;
}

FCKToolbarStyleCombo.prototype.RefreshActiveItems = function( targetSpecialCombo )
{
	var startElement = FCK.ToolbarSet.CurrentInstance.Selection.GetBoundaryParentElement( true ) ;

	if ( startElement )
	{
		var path = new FCKElementPath( startElement ) ;
		var elements = path.Elements ;

		for ( var e = 0 ; e < elements.length ; e++ )
		{
			for ( var i in targetSpecialCombo.Items )
			{
				var item = targetSpecialCombo.Items[i] ;
				var style = item.Style ;

				if ( style.CheckElementRemovable( elements[ e ], true ) )
				{
					targetSpecialCombo.SetLabel( style.Label || style.Name ) ;
					return ;
				}
			}
		}
	}

	targetSpecialCombo.SetLabel( this.DefaultLabel ) ;
}

FCKToolbarStyleCombo.prototype.StyleCombo_OnBeforeClick = function( targetSpecialCombo )
{
	// Two things are done here:
	//	- In a control selection, get the element name, so we'll display styles
	//	  for that element only.
	//	- Select the styles that are active for the current selection.

	// Clear the current selection.
	targetSpecialCombo.DeselectAll() ;

	var startElement ;
	var path ;
	var tagName ;

	var selection = FCK.ToolbarSet.CurrentInstance.Selection ;

	if ( selection.GetType() == 'Control' )
	{
		startElement = selection.GetSelectedElement() ;
		tagName = startElement.nodeName.toLowerCase() ;
	}
	else
	{
		startElement = selection.GetBoundaryParentElement( true ) ;
		path = new FCKElementPath( startElement ) ;
	}

	for ( var i in targetSpecialCombo.Items )
	{
		var item = targetSpecialCombo.Items[i] ;
		var style = item.Style ;

		if ( ( tagName && style.Element == tagName ) || ( !tagName && style.GetType() != FCK_STYLE_OBJECT ) )
		{
			item.style.display = '' ;

			if ( ( path && style.CheckActive( path ) ) || ( !path && style.CheckElementRemovable( startElement, true ) ) )
				targetSpecialCombo.SelectItem( style.Name ) ;
		}
		else
			item.style.display = 'none' ;
	}
}

function FCKToolbarStyleCombo_BuildPreview( style, caption )
{
	var styleType = style.GetType() ;
	var html = [] ;

	if ( styleType == FCK_STYLE_BLOCK )
		html.push( '<div class="BaseFont">' ) ;

	var elementName = style.Element ;

	// Avoid <bdo> in the preview.
	if ( elementName == 'bdo' )
		elementName = 'span' ;

	html = [ '<', elementName ] ;

	// Assign all defined attributes.
	var attribs	= style._StyleDesc.Attributes ;
	if ( attribs )
	{
		for ( var att in attribs )
		{
			html.push( ' ', att, '="', style.GetFinalAttributeValue( att ), '"' ) ;
		}
	}

	// Assign the style attribute.
	if ( style._GetStyleText().length > 0 )
		html.push( ' style="', style.GetFinalStyleValue(), '"' ) ;

	html.push( '>', caption, '</', elementName, '>' ) ;

	if ( styleType == FCK_STYLE_BLOCK )
		html.push( '</div>' ) ;

	return html.join( '' ) ;
}
