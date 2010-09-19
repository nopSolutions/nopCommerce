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
 * Useful functions used by almost all dialog window pages.
 * Dialogs should link to this file as the very first script on the page.
 */

// Automatically detect the correct document.domain (#123).
(function()
{
	var d = document.domain ;

	while ( true )
	{
		// Test if we can access a parent property.
		try
		{
			var test = window.parent.document.domain ;
			break ;
		}
		catch( e ) {}

		// Remove a domain part: www.mytest.example.com => mytest.example.com => example.com ...
		d = d.replace( /.*?(?:\.|$)/, '' ) ;

		if ( d.length == 0 )
			break ;		// It was not able to detect the domain.

		try
		{
			document.domain = d ;
		}
		catch (e)
		{
			break ;
		}
	}
})() ;

// Attention: FCKConfig must be available in the page.
function GetCommonDialogCss( prefix )
{
	// CSS minified by http://iceyboard.no-ip.org/projects/css_compressor (see _dev/css_compression.txt).
	return FCKConfig.BasePath + 'dialog/common/' + '|.ImagePreviewArea{border:#000 1px solid;overflow:auto;width:100%;height:170px;background-color:#fff}.FlashPreviewArea{border:#000 1px solid;padding:5px;overflow:auto;width:100%;height:170px;background-color:#fff}.BtnReset{float:left;background-position:center center;background-image:url(images/reset.gif);width:16px;height:16px;background-repeat:no-repeat;border:1px none;font-size:1px}.BtnLocked,.BtnUnlocked{float:left;background-position:center center;background-image:url(images/locked.gif);width:16px;height:16px;background-repeat:no-repeat;border:none 1px;font-size:1px}.BtnUnlocked{background-image:url(images/unlocked.gif)}.BtnOver{border:outset 1px;cursor:pointer;cursor:hand}' ;
}

// Gets a element by its Id. Used for shorter coding.
function GetE( elementId )
{
	return document.getElementById( elementId )  ;
}

function ShowE( element, isVisible )
{
	if ( typeof( element ) == 'string' )
		element = GetE( element ) ;
	element.style.display = isVisible ? '' : 'none' ;
}

function SetAttribute( element, attName, attValue )
{
	if ( attValue == null || attValue.length == 0 )
		element.removeAttribute( attName, 0 ) ;			// 0 : Case Insensitive
	else
		element.setAttribute( attName, attValue, 0 ) ;	// 0 : Case Insensitive
}

function GetAttribute( element, attName, valueIfNull )
{
	var oAtt = element.attributes[attName] ;

	if ( oAtt == null || !oAtt.specified )
		return valueIfNull ? valueIfNull : '' ;

	var oValue = element.getAttribute( attName, 2 ) ;

	if ( oValue == null )
		oValue = oAtt.nodeValue ;

	return ( oValue == null ? valueIfNull : oValue ) ;
}

function SelectField( elementId )
{
	var element = GetE( elementId ) ;
	element.focus() ;

	// element.select may not be available for some fields (like <select>).
	if ( element.select )
		element.select() ;
}

// Functions used by text fields to accept numbers only.
var IsDigit = ( function()
	{
		var KeyIdentifierMap =
		{
			End			: 35,
			Home		: 36,
			Left		: 37,
			Right		: 39,
			'U+00007F'	: 46		// Delete
		} ;

		return function ( e )
			{
				if ( !e )
					e = event ;

				var iCode = ( e.keyCode || e.charCode ) ;

				if ( !iCode && e.keyIdentifier && ( e.keyIdentifier in KeyIdentifierMap ) )
						iCode = KeyIdentifierMap[ e.keyIdentifier ] ;

				return (
						( iCode >= 48 && iCode <= 57 )		// Numbers
						|| (iCode >= 35 && iCode <= 40)		// Arrows, Home, End
						|| iCode == 8						// Backspace
						|| iCode == 46						// Delete
						|| iCode == 9						// Tab
				) ;
			}
	} )() ;

String.prototype.Trim = function()
{
	return this.replace( /(^\s*)|(\s*$)/g, '' ) ;
}

String.prototype.StartsWith = function( value )
{
	return ( this.substr( 0, value.length ) == value ) ;
}

String.prototype.Remove = function( start, length )
{
	var s = '' ;

	if ( start > 0 )
		s = this.substring( 0, start ) ;

	if ( start + length < this.length )
		s += this.substring( start + length , this.length ) ;

	return s ;
}

String.prototype.ReplaceAll = function( searchArray, replaceArray )
{
	var replaced = this ;

	for ( var i = 0 ; i < searchArray.length ; i++ )
	{
		replaced = replaced.replace( searchArray[i], replaceArray[i] ) ;
	}

	return replaced ;
}

function OpenFileBrowser( url, width, height )
{
	// oEditor must be defined.

	var iLeft = ( oEditor.FCKConfig.ScreenWidth  - width ) / 2 ;
	var iTop  = ( oEditor.FCKConfig.ScreenHeight - height ) / 2 ;

	var sOptions = "toolbar=no,status=no,resizable=yes,dependent=yes,scrollbars=yes" ;
	sOptions += ",width=" + width ;
	sOptions += ",height=" + height ;
	sOptions += ",left=" + iLeft ;
	sOptions += ",top=" + iTop ;

	window.open( url, 'FCKBrowseWindow', sOptions ) ;
}

/**
 Utility function to create/update an element with a name attribute in IE, so it behaves properly when moved around
 It also allows to change the name or other special attributes in an existing node
	oEditor : instance of FCKeditor where the element will be created
	oOriginal : current element being edited or null if it has to be created
	nodeName : string with the name of the element to create
	oAttributes : Hash object with the attributes that must be set at creation time in IE
								Those attributes will be set also after the element has been
								created for any other browser to avoid redudant code
*/
function CreateNamedElement( oEditor, oOriginal, nodeName, oAttributes )
{
	var oNewNode ;

	// IE doesn't allow easily to change properties of an existing object,
	// so remove the old and force the creation of a new one.
	var oldNode = null ;
	if ( oOriginal && oEditor.FCKBrowserInfo.IsIE )
	{
		// Force the creation only if some of the special attributes have changed:
		var bChanged = false;
		for( var attName in oAttributes )
			bChanged |= ( oOriginal.getAttribute( attName, 2) != oAttributes[attName] ) ;

		if ( bChanged )
		{
			oldNode = oOriginal ;
			oOriginal = null ;
		}
	}

	// If the node existed (and it's not IE), then we just have to update its attributes
	if ( oOriginal )
	{
		oNewNode = oOriginal ;
	}
	else
	{
		// #676, IE doesn't play nice with the name or type attribute
		if ( oEditor.FCKBrowserInfo.IsIE )
		{
			var sbHTML = [] ;
			sbHTML.push( '<' + nodeName ) ;
			for( var prop in oAttributes )
			{
				sbHTML.push( ' ' + prop + '="' + oAttributes[prop] + '"' ) ;
			}
			sbHTML.push( '>' ) ;
			if ( !oEditor.FCKListsLib.EmptyElements[nodeName.toLowerCase()] )
				sbHTML.push( '</' + nodeName + '>' ) ;

			oNewNode = oEditor.FCK.EditorDocument.createElement( sbHTML.join('') ) ;
			// Check if we are just changing the properties of an existing node: copy its properties
			if ( oldNode )
			{
				CopyAttributes( oldNode, oNewNode, oAttributes ) ;
				oEditor.FCKDomTools.MoveChildren( oldNode, oNewNode ) ;
				oldNode.parentNode.removeChild( oldNode ) ;
				oldNode = null ;

				if ( oEditor.FCK.Selection.SelectionData )
				{
					// Trick to refresh the selection object and avoid error in
					// fckdialog.html Selection.EnsureSelection
					var oSel = oEditor.FCK.EditorDocument.selection ;
					oEditor.FCK.Selection.SelectionData = oSel.createRange() ; // Now oSel.type will be 'None' reflecting the real situation
				}
			}
			oNewNode = oEditor.FCK.InsertElement( oNewNode ) ;

			// FCK.Selection.SelectionData is broken by now since we've
			// deleted the previously selected element. So we need to reassign it.
			if ( oEditor.FCK.Selection.SelectionData )
			{
				var range = oEditor.FCK.EditorDocument.body.createControlRange() ;
				range.add( oNewNode ) ;
				oEditor.FCK.Selection.SelectionData = range ;
			}
		}
		else
		{
			oNewNode = oEditor.FCK.InsertElement( nodeName ) ;
		}
	}

	// Set the basic attributes
	for( var attName in oAttributes )
		oNewNode.setAttribute( attName, oAttributes[attName], 0 ) ;	// 0 : Case Insensitive

	return oNewNode ;
}

// Copy all the attributes from one node to the other, kinda like a clone
// But oSkipAttributes is an object with the attributes that must NOT be copied
function CopyAttributes( oSource, oDest, oSkipAttributes )
{
	var aAttributes = oSource.attributes ;

	for ( var n = 0 ; n < aAttributes.length ; n++ )
	{
		var oAttribute = aAttributes[n] ;

		if ( oAttribute.specified )
		{
			var sAttName = oAttribute.nodeName ;
			// We can set the type only once, so do it with the proper value, not copying it.
			if ( sAttName in oSkipAttributes )
				continue ;

			var sAttValue = oSource.getAttribute( sAttName, 2 ) ;
			if ( sAttValue == null )
				sAttValue = oAttribute.nodeValue ;

			oDest.setAttribute( sAttName, sAttValue, 0 ) ;	// 0 : Case Insensitive
		}
	}
	// The style:
	if ( oSource.style.cssText !== '' )
		oDest.style.cssText = oSource.style.cssText ;
}

/**
* Replaces a tag with another one, keeping its contents:
* for example TD --> TH, and TH --> TD.
* input: the original node, and the new tag name
* http://www.w3.org/TR/DOM-Level-3-Core/core.html#Document3-renameNode
*/
function RenameNode( oNode , newTag )
{
	// TODO: if the browser natively supports document.renameNode call it.
	// does any browser currently support it in order to test?

	// Only rename element nodes.
	if ( oNode.nodeType != 1 )
		return null ;

	// If it's already correct exit here.
	if ( oNode.nodeName == newTag )
		return oNode ;

	var oDoc = oNode.ownerDocument ;
	// Create the new node
	var newNode = oDoc.createElement( newTag ) ;

	// Copy all attributes
	CopyAttributes( oNode, newNode, {} ) ;

	// Move children to the new node
	FCKDomTools.MoveChildren( oNode, newNode ) ;

	// Finally replace the node and return the new one
	oNode.parentNode.replaceChild( newNode, oNode ) ;

	return newNode ;
}
