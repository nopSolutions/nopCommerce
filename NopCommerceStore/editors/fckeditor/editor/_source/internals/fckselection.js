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
 * Active selection functions.
 */

var FCKSelection = FCK.Selection =
{
	GetParentBlock : function()
	{
		var retval = this.GetParentElement() ;
		while ( retval )
		{
			if ( FCKListsLib.BlockBoundaries[retval.nodeName.toLowerCase()] )
				break ;
			retval = retval.parentNode ;
		}
		return retval ;
	},

	ApplyStyle : function( styleDefinition )
	{
		FCKStyles.ApplyStyle( new FCKStyle( styleDefinition ) ) ;
	}
} ;
