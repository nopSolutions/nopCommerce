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
 * FCKStyleCommand Class: represents the "Style" command.
 */

var FCKStyleCommand = function()
{}

FCKStyleCommand.prototype =
{
	Name : 'Style',

	Execute : function( styleName, styleComboItem )
	{
		FCKUndo.SaveUndoStep() ;

		if ( styleComboItem.Selected )
			FCK.Styles.RemoveStyle( styleComboItem.Style ) ;
		else
			FCK.Styles.ApplyStyle( styleComboItem.Style ) ;

		FCKUndo.SaveUndoStep() ;

		FCK.Focus() ;
		FCK.Events.FireEvent( 'OnSelectionChange' ) ;
	},

	GetState : function()
	{
		if ( FCK.EditMode != FCK_EDITMODE_WYSIWYG || !FCK.EditorDocument )
			return FCK_TRISTATE_DISABLED ;

		if ( FCKSelection.GetType() == 'Control' )
		{
			var el = FCKSelection.GetSelectedElement() ;
			if ( !el || !FCKStyles.CheckHasObjectStyle( el.nodeName.toLowerCase() ) )
				return FCK_TRISTATE_DISABLED ;
		}

		return FCK_TRISTATE_OFF ;
	}
};
