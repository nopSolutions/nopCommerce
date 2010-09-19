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
 * This class is a menu block that behaves like a panel. It's a mix of the
 * FCKMenuBlock and FCKPanel classes.
 */

var FCKMenuBlockPanel = function()
{
	// Call the "base" constructor.
	FCKMenuBlock.call( this ) ;
}

FCKMenuBlockPanel.prototype = new FCKMenuBlock() ;


// Override the create method.
FCKMenuBlockPanel.prototype.Create = function()
{
	var oPanel = this.Panel = ( this.Parent && this.Parent.Panel ? this.Parent.Panel.CreateChildPanel() : new FCKPanel() ) ;
	oPanel.AppendStyleSheet( FCKConfig.SkinEditorCSS ) ;

	// Call the "base" implementation.
	FCKMenuBlock.prototype.Create.call( this, oPanel.MainNode ) ;
}

FCKMenuBlockPanel.prototype.Show = function( x, y, relElement )
{
	if ( !this.Panel.CheckIsOpened() )
		this.Panel.Show( x, y, relElement ) ;
}

FCKMenuBlockPanel.prototype.Hide = function()
{
	if ( this.Panel.CheckIsOpened() )
		this.Panel.Hide() ;
}
