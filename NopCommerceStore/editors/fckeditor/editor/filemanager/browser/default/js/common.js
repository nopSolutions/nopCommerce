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
 * Common objects and functions shared by all pages that compose the
 * File Browser dialog window.
 */

// Automatically detect the correct document.domain (#1919).
(function()
{
	var d = document.domain ;

	while ( true )
	{
		// Test if we can access a parent property.
		try
		{
			var test = window.top.opener.document.domain ;
			break ;
		}
		catch( e )
		{}

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

function AddSelectOption( selectElement, optionText, optionValue )
{
	var oOption = document.createElement("OPTION") ;

	oOption.text	= optionText ;
	oOption.value	= optionValue ;

	selectElement.options.add(oOption) ;

	return oOption ;
}

var oConnector	= window.parent.oConnector ;
var oIcons		= window.parent.oIcons ;


function StringBuilder( value )
{
    this._Strings = new Array( value || '' ) ;
}

StringBuilder.prototype.Append = function( value )
{
    if ( value )
        this._Strings.push( value ) ;
}

StringBuilder.prototype.ToString = function()
{
    return this._Strings.join( '' ) ;
}
