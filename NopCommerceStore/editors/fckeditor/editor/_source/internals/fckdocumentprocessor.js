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
 * Advanced document processors.
 */

var FCKDocumentProcessor = new Object() ;
FCKDocumentProcessor._Items = new Array() ;

FCKDocumentProcessor.AppendNew = function()
{
	var oNewItem = new Object() ;
	this._Items.push( oNewItem ) ;
	return oNewItem ;
}

FCKDocumentProcessor.Process = function( document )
{
	var bIsDirty = FCK.IsDirty() ;
	var oProcessor, i = 0 ;
	while( ( oProcessor = this._Items[i++] ) )
		oProcessor.ProcessDocument( document ) ;
	if ( !bIsDirty )
		FCK.ResetIsDirty() ;
}

var FCKDocumentProcessor_CreateFakeImage = function( fakeClass, realElement )
{
	var oImg = FCKTools.GetElementDocument( realElement ).createElement( 'IMG' ) ;
	oImg.className = fakeClass ;
	oImg.src = FCKConfig.BasePath + 'images/spacer.gif' ;
	oImg.setAttribute( '_fckfakelement', 'true', 0 ) ;
	oImg.setAttribute( '_fckrealelement', FCKTempBin.AddElement( realElement ), 0 ) ;
	return oImg ;
}

// Link Anchors
if ( FCKBrowserInfo.IsIE || FCKBrowserInfo.IsOpera )
{
	var FCKAnchorsProcessor = FCKDocumentProcessor.AppendNew() ;
	FCKAnchorsProcessor.ProcessDocument = function( document )
	{
		var aLinks = document.getElementsByTagName( 'A' ) ;

		var oLink ;
		var i = aLinks.length - 1 ;
		while ( i >= 0 && ( oLink = aLinks[i--] ) )
		{
			// If it is anchor. Doesn't matter if it's also a link (even better: we show that it's both a link and an anchor)
			if ( oLink.name.length > 0 )
			{
				//if the anchor has some content then we just add a temporary class
				if ( oLink.innerHTML !== '' )
				{
					if ( FCKBrowserInfo.IsIE )
						oLink.className += ' FCK__AnchorC' ;
				}
				else
				{
					var oImg = FCKDocumentProcessor_CreateFakeImage( 'FCK__Anchor', oLink.cloneNode(true) ) ;
					oImg.setAttribute( '_fckanchor', 'true', 0 ) ;

					oLink.parentNode.insertBefore( oImg, oLink ) ;
					oLink.parentNode.removeChild( oLink ) ;
				}
			}
		}
	}
}

// Page Breaks
var FCKPageBreaksProcessor = FCKDocumentProcessor.AppendNew() ;
FCKPageBreaksProcessor.ProcessDocument = function( document )
{
	var aDIVs = document.getElementsByTagName( 'DIV' ) ;

	var eDIV ;
	var i = aDIVs.length - 1 ;
	while ( i >= 0 && ( eDIV = aDIVs[i--] ) )
	{
		if ( eDIV.style.pageBreakAfter == 'always' && eDIV.childNodes.length == 1 && eDIV.childNodes[0].style && eDIV.childNodes[0].style.display == 'none' )
		{
			var oFakeImage = FCKDocumentProcessor_CreateFakeImage( 'FCK__PageBreak', eDIV.cloneNode(true) ) ;

			eDIV.parentNode.insertBefore( oFakeImage, eDIV ) ;
			eDIV.parentNode.removeChild( eDIV ) ;
		}
	}
/*
	var aCenters = document.getElementsByTagName( 'CENTER' ) ;

	var oCenter ;
	var i = aCenters.length - 1 ;
	while ( i >= 0 && ( oCenter = aCenters[i--] ) )
	{
		if ( oCenter.style.pageBreakAfter == 'always' && oCenter.innerHTML.Trim().length == 0 )
		{
			var oFakeImage = FCKDocumentProcessor_CreateFakeImage( 'FCK__PageBreak', oCenter.cloneNode(true) ) ;

			oCenter.parentNode.insertBefore( oFakeImage, oCenter ) ;
			oCenter.parentNode.removeChild( oCenter ) ;
		}
	}
*/
}

// EMBED and OBJECT tags.
var FCKEmbedAndObjectProcessor = (function()
{
	var customProcessors = [] ;

	var processElement = function( el )
	{
		var clone = el.cloneNode( true ) ;
		var replaceElement ;
		var fakeImg = replaceElement = FCKDocumentProcessor_CreateFakeImage( 'FCK__UnknownObject', clone ) ;
		FCKEmbedAndObjectProcessor.RefreshView( fakeImg, el ) ;

		for ( var i = 0 ; i < customProcessors.length ; i++ )
			replaceElement = customProcessors[i]( el, replaceElement ) || replaceElement ;

		if ( replaceElement != fakeImg )
			FCKTempBin.RemoveElement( fakeImg.getAttribute( '_fckrealelement' ) ) ;

		el.parentNode.replaceChild( replaceElement, el ) ;
	}

	var processElementsByName = function( elementName, doc )
	{
		var aObjects = doc.getElementsByTagName( elementName );
		for ( var i = aObjects.length - 1 ; i >= 0 ; i-- )
			processElement( aObjects[i] ) ;
	}

	var processObjectAndEmbed = function( doc )
	{
		processElementsByName( 'object', doc );
		processElementsByName( 'embed', doc );
	}

	return FCKTools.Merge( FCKDocumentProcessor.AppendNew(),
		       {
				ProcessDocument : function( doc )
				{
					// Firefox 3 would sometimes throw an unknown exception while accessing EMBEDs and OBJECTs
					// without the setTimeout().
					if ( FCKBrowserInfo.IsGecko )
						FCKTools.RunFunction( processObjectAndEmbed, this, [ doc ] ) ;
					else
						processObjectAndEmbed( doc ) ;
				},

				RefreshView : function( placeHolder, original )
				{
					if ( original.getAttribute( 'width' ) > 0 )
						placeHolder.style.width = FCKTools.ConvertHtmlSizeToStyle( original.getAttribute( 'width' ) ) ;

					if ( original.getAttribute( 'height' ) > 0 )
						placeHolder.style.height = FCKTools.ConvertHtmlSizeToStyle( original.getAttribute( 'height' ) ) ;
				},

				AddCustomHandler : function( func )
				{
					customProcessors.push( func ) ;
				}
			} ) ;
} )() ;

FCK.GetRealElement = function( fakeElement )
{
	var e = FCKTempBin.Elements[ fakeElement.getAttribute('_fckrealelement') ] ;

	if ( fakeElement.getAttribute('_fckflash') )
	{
		if ( fakeElement.style.width.length > 0 )
				e.width = FCKTools.ConvertStyleSizeToHtml( fakeElement.style.width ) ;

		if ( fakeElement.style.height.length > 0 )
				e.height = FCKTools.ConvertStyleSizeToHtml( fakeElement.style.height ) ;
	}

	return e ;
}

// HR Processor.
// This is a IE only (tricky) thing. We protect all HR tags before loading them
// (see FCK.ProtectTags). Here we put the HRs back.
if ( FCKBrowserInfo.IsIE )
{
	FCKDocumentProcessor.AppendNew().ProcessDocument = function( document )
	{
		var aHRs = document.getElementsByTagName( 'HR' ) ;

		var eHR ;
		var i = aHRs.length - 1 ;
		while ( i >= 0 && ( eHR = aHRs[i--] ) )
		{
			// Create the replacement HR.
			var newHR = document.createElement( 'hr' ) ;
			newHR.mergeAttributes( eHR, true ) ;

			// We must insert the new one after it. insertBefore will not work in all cases.
			FCKDomTools.InsertAfterNode( eHR, newHR ) ;

			eHR.parentNode.removeChild( eHR ) ;
		}
	}
}

// INPUT:hidden Processor.
FCKDocumentProcessor.AppendNew().ProcessDocument = function( document )
{
	var aInputs = document.getElementsByTagName( 'INPUT' ) ;

	var oInput ;
	var i = aInputs.length - 1 ;
	while ( i >= 0 && ( oInput = aInputs[i--] ) )
	{
		if ( oInput.type == 'hidden' )
		{
			var oImg = FCKDocumentProcessor_CreateFakeImage( 'FCK__InputHidden', oInput.cloneNode(true) ) ;
			oImg.setAttribute( '_fckinputhidden', 'true', 0 ) ;

			oInput.parentNode.insertBefore( oImg, oInput ) ;
			oInput.parentNode.removeChild( oInput ) ;
		}
	}
}

// Flash handler.
FCKEmbedAndObjectProcessor.AddCustomHandler( function( el, fakeImg )
	{
		if ( ! ( el.nodeName.IEquals( 'embed' ) && ( el.type == 'application/x-shockwave-flash' || /\.swf($|#|\?)/i.test( el.src ) ) ) )
			return ;
		fakeImg.className = 'FCK__Flash' ;
		fakeImg.setAttribute( '_fckflash', 'true', 0 );
	} ) ;

// Buggy <span class="Apple-style-span"> tags added by Safari.
if ( FCKBrowserInfo.IsSafari )
{
	FCKDocumentProcessor.AppendNew().ProcessDocument = function( doc )
	{
		var spans = doc.getElementsByClassName ?
			doc.getElementsByClassName( 'Apple-style-span' ) :
			Array.prototype.filter.call(
					doc.getElementsByTagName( 'span' ),
					function( item ){ return item.className == 'Apple-style-span' ; }
					) ;
		for ( var i = spans.length - 1 ; i >= 0 ; i-- )
			FCKDomTools.RemoveNode( spans[i], true ) ;
	}
}
