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
 * Library of keys collections.
 *
 * Test have shown that check for the existence of a key in an object is the
 * most efficient list entry check (10x faster that regex). Example:
 *		if ( FCKListsLib.<ListName>[key] != null )
 */

var FCKListsLib =
{
	// We are not handling <ins> and <del> as block elements, for now.
	BlockElements : { address:1,blockquote:1,center:1,div:1,dl:1,fieldset:1,form:1,h1:1,h2:1,h3:1,h4:1,h5:1,h6:1,hr:1,marquee:1,noscript:1,ol:1,p:1,pre:1,script:1,table:1,ul:1 },

	// Block elements that may be filled with &nbsp; if empty.
	NonEmptyBlockElements : { p:1,div:1,form:1,h1:1,h2:1,h3:1,h4:1,h5:1,h6:1,address:1,pre:1,ol:1,ul:1,li:1,td:1,th:1 },

	// Inline elements which MUST have child nodes.
	InlineChildReqElements : { abbr:1,acronym:1,b:1,bdo:1,big:1,cite:1,code:1,del:1,dfn:1,em:1,font:1,i:1,ins:1,label:1,kbd:1,q:1,samp:1,small:1,span:1,strike:1,strong:1,sub:1,sup:1,tt:1,u:1,'var':1 },

	// Inline elements which are not marked as empty "Empty" in the XHTML DTD.
	InlineNonEmptyElements : { a:1,abbr:1,acronym:1,b:1,bdo:1,big:1,cite:1,code:1,del:1,dfn:1,em:1,font:1,i:1,ins:1,label:1,kbd:1,q:1,samp:1,small:1,span:1,strike:1,strong:1,sub:1,sup:1,tt:1,u:1,'var':1 },

	// Elements marked as empty "Empty" in the XHTML DTD.
	EmptyElements : { base:1,col:1,meta:1,link:1,hr:1,br:1,param:1,img:1,area:1,input:1 },

	// Elements that may be considered the "Block boundary" in an element path.
	PathBlockElements : { address:1,blockquote:1,dl:1,h1:1,h2:1,h3:1,h4:1,h5:1,h6:1,p:1,pre:1,li:1,dt:1,de:1 },

	// Elements that may be considered the "Block limit" in an element path.
	PathBlockLimitElements : { body:1,div:1,td:1,th:1,caption:1,form:1 },

	// Block elements for the Styles System.
	StyleBlockElements : { address:1,div:1,h1:1,h2:1,h3:1,h4:1,h5:1,h6:1,p:1,pre:1 },

	// Object elements for the Styles System.
	StyleObjectElements : { img:1,hr:1,li:1,table:1,tr:1,td:1,embed:1,object:1,ol:1,ul:1 },

	// Elements that accept text nodes, but are not possible to edit in the browser.
	NonEditableElements : { button:1,option:1,script:1,iframe:1,textarea:1,object:1,embed:1,map:1,applet:1 },

	// Elements used to separate block contents.
	BlockBoundaries : { p:1,div:1,h1:1,h2:1,h3:1,h4:1,h5:1,h6:1,hr:1,address:1,pre:1,ol:1,ul:1,li:1,dt:1,de:1,table:1,thead:1,tbody:1,tfoot:1,tr:1,th:1,td:1,caption:1,col:1,colgroup:1,blockquote:1,body:1 },
	ListBoundaries  : { p:1,div:1,h1:1,h2:1,h3:1,h4:1,h5:1,h6:1,hr:1,address:1,pre:1,ol:1,ul:1,li:1,dt:1,de:1,table:1,thead:1,tbody:1,tfoot:1,tr:1,th:1,td:1,caption:1,col:1,colgroup:1,blockquote:1,body:1,br:1 }
} ;
