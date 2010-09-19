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
 * This is a sample implementation for a custom Data Processor for basic BBCode.
 */

FCK.DataProcessor =
{
	/*
	 * Returns a string representing the HTML format of "data". The returned
	 * value will be loaded in the editor.
	 * The HTML must be from <html> to </html>, eventually including
	 * the DOCTYPE.
	 *     @param {String} data The data to be converted in the
	 *            DataProcessor specific format.
	 */
	ConvertToHtml : function( data )
	{
		// Convert < and > to their HTML entities.
        data = data.replace( /</g, '&lt;' ) ;
        data = data.replace( />/g, '&gt;' ) ;

        // Convert line breaks to <br>.
        data = data.replace( /(?:\r\n|\n|\r)/g, '<br>' ) ;

        // [url]
        data = data.replace( /\[url\](.+?)\[\/url]/gi, '<a href="$1">$1</a>' ) ;
        data = data.replace( /\[url\=([^\]]+)](.+?)\[\/url]/gi, '<a href="$1">$2</a>' ) ;

        // [b]
        data = data.replace( /\[b\](.+?)\[\/b]/gi, '<b>$1</b>' ) ;

        // [i]
        data = data.replace( /\[i\](.+?)\[\/i]/gi, '<i>$1</i>' ) ;

        // [u]
        data = data.replace( /\[u\](.+?)\[\/u]/gi, '<u>$1</u>' ) ;

		return '<html><head><title></title></head><body>' + data + '</body></html>' ;
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
		var data = rootNode.innerHTML ;

		// Convert <br> to line breaks.
		data = data.replace( /<br(?=[ \/>]).*?>/gi, '\r\n') ;

		// [url]
		data = data.replace( /<a .*?href=(["'])(.+?)\1.*?>(.+?)<\/a>/gi, '[url=$2]$3[/url]') ;

		// [b]
		data = data.replace( /<(?:b|strong)>/gi, '[b]') ;
		data = data.replace( /<\/(?:b|strong)>/gi, '[/b]') ;

		// [i]
		data = data.replace( /<(?:i|em)>/gi, '[i]') ;
		data = data.replace( /<\/(?:i|em)>/gi, '[/i]') ;

		// [u]
		data = data.replace( /<u>/gi, '[u]') ;
		data = data.replace( /<\/u>/gi, '[/u]') ;

		// Remove remaining tags.
		data = data.replace( /<[^>]+>/g, '') ;

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

// This Data Processor doesn't support <p>, so let's use <br>.
FCKConfig.EnterMode = 'br' ;

// To avoid pasting invalid markup (which is discarded in any case), let's
// force pasting to plain text.
FCKConfig.ForcePasteAsPlainText	= true ;

// Rename the "Source" buttom to "BBCode".
FCKToolbarItems.RegisterItem( 'Source', new FCKToolbarButton( 'Source', 'BBCode', null, FCK_TOOLBARITEM_ICONTEXT, true, true, 1 ) ) ;

// Let's enforce the toolbar to the limits of this Data Processor. A custom
// toolbar set may be defined in the configuration file with more or less entries.
FCKConfig.ToolbarSets["Default"] = [
	['Source'],
	['Bold','Italic','Underline','-','Link'],
	['About']
] ;
