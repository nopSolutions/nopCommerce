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
 * This plugin register Toolbar items for the combos modifying the style to
 * not show the box.
 */

FCKToolbarItems.RegisterItem( 'SourceSimple'	, new FCKToolbarButton( 'Source', FCKLang.Source, null, FCK_TOOLBARITEM_ONLYICON, true, true, 1 ) ) ;
FCKToolbarItems.RegisterItem( 'StyleSimple'		, new FCKToolbarStyleCombo( null, FCK_TOOLBARITEM_ONLYTEXT ) ) ;
FCKToolbarItems.RegisterItem( 'FontNameSimple'	, new FCKToolbarFontsCombo( null, FCK_TOOLBARITEM_ONLYTEXT ) ) ;
FCKToolbarItems.RegisterItem( 'FontSizeSimple'	, new FCKToolbarFontSizeCombo( null, FCK_TOOLBARITEM_ONLYTEXT ) ) ;
FCKToolbarItems.RegisterItem( 'FontFormatSimple', new FCKToolbarFontFormatCombo( null, FCK_TOOLBARITEM_ONLYTEXT ) ) ;
