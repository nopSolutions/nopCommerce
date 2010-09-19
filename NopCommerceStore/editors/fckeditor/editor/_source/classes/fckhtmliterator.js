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
 * This class can be used to interate through nodes inside a range.
 *
 * During interation, the provided range can become invalid, due to document
 * mutations, so CreateBookmark() used to restore it after processing, if
 * needed.
 */

var FCKHtmlIterator = function( source )
{
	this._sourceHtml = source ;
}
FCKHtmlIterator.prototype =
{
	Next : function()
	{
		var sourceHtml = this._sourceHtml ;
		if ( sourceHtml == null )
			return null ;

		var match = FCKRegexLib.HtmlTag.exec( sourceHtml ) ;
		var isTag = false ;
		var value = "" ;
		if ( match )
		{
			if ( match.index > 0 )
			{
				value = sourceHtml.substr( 0, match.index ) ;
				this._sourceHtml = sourceHtml.substr( match.index ) ;
			}
			else
			{
				isTag = true ;
				value = match[0] ;
				this._sourceHtml = sourceHtml.substr( match[0].length ) ;
			}
		}
		else
		{
			value = sourceHtml ;
			this._sourceHtml = null ;
		}
		return { 'isTag' : isTag, 'value' : value } ;
	},

	Each : function( func )
	{
		var chunk ;
		while ( ( chunk = this.Next() ) )
			func( chunk.isTag, chunk.value ) ;
	}
} ;
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
 * This class can be used to interate through nodes inside a range.
 *
 * During interation, the provided range can become invalid, due to document
 * mutations, so CreateBookmark() used to restore it after processing, if
 * needed.
 */

var FCKHtmlIterator = function( source )
{
	this._sourceHtml = source ;
}
FCKHtmlIterator.prototype =
{
	Next : function()
	{
		var sourceHtml = this._sourceHtml ;
		if ( sourceHtml == null )
			return null ;

		var match = FCKRegexLib.HtmlTag.exec( sourceHtml ) ;
		var isTag = false ;
		var value = "" ;
		if ( match )
		{
			if ( match.index > 0 )
			{
				value = sourceHtml.substr( 0, match.index ) ;
				this._sourceHtml = sourceHtml.substr( match.index ) ;
			}
			else
			{
				isTag = true ;
				value = match[0] ;
				this._sourceHtml = sourceHtml.substr( match[0].length ) ;
			}
		}
		else
		{
			value = sourceHtml ;
			this._sourceHtml = null ;
		}
		return { 'isTag' : isTag, 'value' : value } ;
	},

	Each : function( func )
	{
		var chunk ;
		while ( ( chunk = this.Next() ) )
			func( chunk.isTag, chunk.value ) ;
	}
} ;
