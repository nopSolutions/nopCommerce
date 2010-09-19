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
 * Format the HTML.
 */

var FCKCodeFormatter = new Object() ;

FCKCodeFormatter.Init = function()
{
	var oRegex = this.Regex = new Object() ;

	// Regex for line breaks.
	oRegex.BlocksOpener = /\<(P|DIV|H1|H2|H3|H4|H5|H6|ADDRESS|PRE|OL|UL|LI|DL|DT|DD|TITLE|META|LINK|BASE|SCRIPT|LINK|TD|TH|AREA|OPTION)[^\>]*\>/gi ;
	oRegex.BlocksCloser = /\<\/(P|DIV|H1|H2|H3|H4|H5|H6|ADDRESS|PRE|OL|UL|LI|DL|DT|DD|TITLE|META|LINK|BASE|SCRIPT|LINK|TD|TH|AREA|OPTION)[^\>]*\>/gi ;

	oRegex.NewLineTags	= /\<(BR|HR)[^\>]*\>/gi ;

	oRegex.MainTags = /\<\/?(HTML|HEAD|BODY|FORM|TABLE|TBODY|THEAD|TR)[^\>]*\>/gi ;

	oRegex.LineSplitter = /\s*\n+\s*/g ;

	// Regex for indentation.
	oRegex.IncreaseIndent = /^\<(HTML|HEAD|BODY|FORM|TABLE|TBODY|THEAD|TR|UL|OL|DL)[ \/\>]/i ;
	oRegex.DecreaseIndent = /^\<\/(HTML|HEAD|BODY|FORM|TABLE|TBODY|THEAD|TR|UL|OL|DL)[ \>]/i ;
	oRegex.FormatIndentatorRemove = new RegExp( '^' + FCKConfig.FormatIndentator ) ;

	oRegex.ProtectedTags = /(<PRE[^>]*>)([\s\S]*?)(<\/PRE>)/gi ;
}

FCKCodeFormatter._ProtectData = function( outer, opener, data, closer )
{
	return opener + '___FCKpd___' + ( FCKCodeFormatter.ProtectedData.push( data ) - 1 ) + closer ;
}

FCKCodeFormatter.Format = function( html )
{
	if ( !this.Regex )
		this.Init() ;

	// Protected content that remain untouched during the
	// process go in the following array.
	FCKCodeFormatter.ProtectedData = new Array() ;

	var sFormatted = html.replace( this.Regex.ProtectedTags, FCKCodeFormatter._ProtectData ) ;

	// Line breaks.
	sFormatted		= sFormatted.replace( this.Regex.BlocksOpener, '\n$&' ) ;
	sFormatted		= sFormatted.replace( this.Regex.BlocksCloser, '$&\n' ) ;
	sFormatted		= sFormatted.replace( this.Regex.NewLineTags, '$&\n' ) ;
	sFormatted		= sFormatted.replace( this.Regex.MainTags, '\n$&\n' ) ;

	// Indentation.
	var sIndentation = '' ;

	var asLines = sFormatted.split( this.Regex.LineSplitter ) ;
	sFormatted = '' ;

	for ( var i = 0 ; i < asLines.length ; i++ )
	{
		var sLine = asLines[i] ;

		if ( sLine.length == 0 )
			continue ;

		if ( this.Regex.DecreaseIndent.test( sLine ) )
			sIndentation = sIndentation.replace( this.Regex.FormatIndentatorRemove, '' ) ;

		sFormatted += sIndentation + sLine + '\n' ;

		if ( this.Regex.IncreaseIndent.test( sLine ) )
			sIndentation += FCKConfig.FormatIndentator ;
	}

	// Now we put back the protected data.
	for ( var j = 0 ; j < FCKCodeFormatter.ProtectedData.length ; j++ )
	{
		var oRegex = new RegExp( '___FCKpd___' + j ) ;
		sFormatted = sFormatted.replace( oRegex, FCKCodeFormatter.ProtectedData[j].replace( /\$/g, '$$$$' ) ) ;
	}

	return sFormatted.Trim() ;
}
