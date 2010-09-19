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
 * The Data Processor is responsible for transforming the input and output data
 * in the editor. For more info:
 * http://dev.fckeditor.net/wiki/Components/DataProcessor
 *
 * The default implementation offers the base XHTML compatibility features of
 * FCKeditor. Further Data Processors may be implemented for other purposes.
 *
 */

var FCKDataProcessor = function()
{}

FCKDataProcessor.prototype =
{
	/*
	 * Returns a string representing the HTML format of "data". The returned
	 * value will be loaded in the editor.
	 * The HTML must be from <html> to </html>, including <head>, <body> and
	 * eventually the DOCTYPE.
	 * Note: HTML comments may already be part of the data because of the
	 * pre-processing made with ProtectedSource.
	 *     @param {String} data The data to be converted in the
	 *            DataProcessor specific format.
	 */
	ConvertToHtml : function( data )
	{
		// The default data processor must handle two different cases depending
		// on the FullPage setting. Custom Data Processors will not be
		// compatible with FullPage, much probably.
		if ( FCKConfig.FullPage )
		{
			// Save the DOCTYPE.
			FCK.DocTypeDeclaration = data.match( FCKRegexLib.DocTypeTag ) ;

			// Check if the <body> tag is available.
			if ( !FCKRegexLib.HasBodyTag.test( data ) )
				data = '<body>' + data + '</body>' ;

			// Check if the <html> tag is available.
			if ( !FCKRegexLib.HtmlOpener.test( data ) )
				data = '<html dir="' + FCKConfig.ContentLangDirection + '">' + data + '</html>' ;

			// Check if the <head> tag is available.
			if ( !FCKRegexLib.HeadOpener.test( data ) )
				data = data.replace( FCKRegexLib.HtmlOpener, '$&<head><title></title></head>' ) ;

			return data ;
		}
		else
		{
			var html =
				FCKConfig.DocType +
				'<html dir="' + FCKConfig.ContentLangDirection + '"' ;

			// On IE, if you are using a DOCTYPE different of HTML 4 (like
			// XHTML), you must force the vertical scroll to show, otherwise
			// the horizontal one may appear when the page needs vertical scrolling.
			// TODO : Check it with IE7 and make it IE6- if it is the case.
			if ( FCKBrowserInfo.IsIE && FCKConfig.DocType.length > 0 && !FCKRegexLib.Html4DocType.test( FCKConfig.DocType ) )
				html += ' style="overflow-y: scroll"' ;

			html += '><head><title></title></head>' +
				'<body' + FCKConfig.GetBodyAttributes() + '>' +
				data +
				'</body></html>' ;

			return html ;
		}
	},

	/*
	 * Converts a DOM (sub-)tree to a string in the data format.
	 *     @param {Object} rootNode The node that contains the DOM tree to be
	 *            converted to the data format.
	 *     @param {Boolean} excludeRoot Indicates that the root node must not
	 *            be included in the conversion, only its children.
	 *     @param {Boolean} format Indicates that the data must be formatted
	 *            for human reading. Not all Data Processors may provide it.
	 */
	ConvertToDataFormat : function( rootNode, excludeRoot, ignoreIfEmptyParagraph, format )
	{
		var data = FCKXHtml.GetXHTML( rootNode, !excludeRoot, format ) ;

		if ( ignoreIfEmptyParagraph && FCKRegexLib.EmptyOutParagraph.test( data ) )
			return '' ;

		return data ;
	},

	/*
	 * Makes any necessary changes to a piece of HTML for insertion in the
	 * editor selection position.
	 *     @param {String} html The HTML to be fixed.
	 */
	FixHtml : function( html )
	{
		return html ;
	}
} ;
