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
 * This is a generic Document Fragment object. It is not intended to provide
 * the W3C implementation, but is a way to fix the missing of a real Document
 * Fragment in IE (where document.createDocumentFragment() returns a normal
 * document instead), giving a standard interface for it.
 * (IE Implementation)
 */

var FCKDocumentFragment = function( parentDocument, baseDocFrag )
{
	this.RootNode = baseDocFrag || parentDocument.createDocumentFragment() ;
}

FCKDocumentFragment.prototype =
{

	// Append the contents of this Document Fragment to another element.
	AppendTo : function( targetNode )
	{
		targetNode.appendChild( this.RootNode ) ;
	},

	AppendHtml : function( html )
	{
		var eTmpDiv = this.RootNode.ownerDocument.createElement( 'div' ) ;
		eTmpDiv.innerHTML = html ;
		FCKDomTools.MoveChildren( eTmpDiv, this.RootNode ) ;
	},

	InsertAfterNode : function( existingNode )
	{
		FCKDomTools.InsertAfterNode( existingNode, this.RootNode ) ;
	}
}
