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
 * FCKStyleCommand Class: represents the "Spell Check" command.
 * (Gecko specific implementation)
 */

var FCKSpellCheckCommand = function()
{
	this.Name = 'SpellCheck' ;
	this.IsEnabled = ( FCKConfig.SpellChecker != 'ieSpell' ) ;
}

FCKSpellCheckCommand.prototype.Execute = function()
{
	switch ( FCKConfig.SpellChecker )
	{
		case 'SpellerPages' :
			FCKDialog.OpenDialog( 'FCKDialog_SpellCheck', 'Spell Check', 'dialog/fck_spellerpages.html', 440, 480 ) ;
			break;

		case 'WSC' :
			FCKDialog.OpenDialog( 'FCKDialog_SpellCheck', 'Spell Check', 'wsc/w.html', 530, 480 ) ;
	}
}

FCKSpellCheckCommand.prototype.GetState = function()
{
	if ( FCK.EditMode != FCK_EDITMODE_WYSIWYG )
		return FCK_TRISTATE_DISABLED ;
	return this.IsEnabled ? FCK_TRISTATE_OFF : FCK_TRISTATE_DISABLED ;
}
