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
 * Define all commands available in the editor.
 */

var FCKCommands = FCK.Commands = new Object() ;
FCKCommands.LoadedCommands = new Object() ;

FCKCommands.RegisterCommand = function( commandName, command )
{
	this.LoadedCommands[ commandName ] = command ;
}

FCKCommands.GetCommand = function( commandName )
{
	var oCommand = FCKCommands.LoadedCommands[ commandName ] ;

	if ( oCommand )
		return oCommand ;

	switch ( commandName )
	{
		case 'Bold'			:
		case 'Italic'		:
		case 'Underline'	:
		case 'StrikeThrough':
		case 'Subscript'	:
		case 'Superscript'	: oCommand = new FCKCoreStyleCommand( commandName ) ; break ;

		case 'RemoveFormat'	: oCommand = new FCKRemoveFormatCommand() ; break ;

		case 'DocProps'		: oCommand = new FCKDialogCommand( 'DocProps'	, FCKLang.DocProps				, 'dialog/fck_docprops.html'	, 400, 380, FCKCommands.GetFullPageState ) ; break ;
		case 'Templates'	: oCommand = new FCKDialogCommand( 'Templates'	, FCKLang.DlgTemplatesTitle		, 'dialog/fck_template.html'	, 380, 450 ) ; break ;
		case 'Link'			: oCommand = new FCKDialogCommand( 'Link'		, FCKLang.DlgLnkWindowTitle		, 'dialog/fck_link.html'		, 400, 300 ) ; break ;
		case 'Unlink'		: oCommand = new FCKUnlinkCommand() ; break ;
		case 'VisitLink'	: oCommand = new FCKVisitLinkCommand() ; break ;
		case 'Anchor'		: oCommand = new FCKDialogCommand( 'Anchor'		, FCKLang.DlgAnchorTitle		, 'dialog/fck_anchor.html'		, 370, 160 ) ; break ;
		case 'AnchorDelete'	: oCommand = new FCKAnchorDeleteCommand() ; break ;
		case 'BulletedList'	: oCommand = new FCKDialogCommand( 'BulletedList', FCKLang.BulletedListProp		, 'dialog/fck_listprop.html?UL'	, 370, 160 ) ; break ;
		case 'NumberedList'	: oCommand = new FCKDialogCommand( 'NumberedList', FCKLang.NumberedListProp		, 'dialog/fck_listprop.html?OL'	, 370, 160 ) ; break ;
		case 'About'		: oCommand = new FCKDialogCommand( 'About'		, FCKLang.About					, 'dialog/fck_about.html'		, 420, 330, function(){ return FCK_TRISTATE_OFF ; } ) ; break ;
		case 'Find'			: oCommand = new FCKDialogCommand( 'Find'		, FCKLang.DlgFindAndReplaceTitle, 'dialog/fck_replace.html'		, 340, 230, null, null, 'Find' ) ; break ;
		case 'Replace'		: oCommand = new FCKDialogCommand( 'Replace'	, FCKLang.DlgFindAndReplaceTitle, 'dialog/fck_replace.html'		, 340, 230, null, null, 'Replace' ) ; break ;

		case 'Image'		: oCommand = new FCKDialogCommand( 'Image'		, FCKLang.DlgImgTitle			, 'dialog/fck_image.html'		, 450, 390 ) ; break ;
		case 'Flash'		: oCommand = new FCKDialogCommand( 'Flash'		, FCKLang.DlgFlashTitle			, 'dialog/fck_flash.html'		, 450, 390 ) ; break ;
		case 'SpecialChar'	: oCommand = new FCKDialogCommand( 'SpecialChar', FCKLang.DlgSpecialCharTitle	, 'dialog/fck_specialchar.html'	, 400, 290 ) ; break ;
		case 'Smiley'		: oCommand = new FCKDialogCommand( 'Smiley'		, FCKLang.DlgSmileyTitle		, 'dialog/fck_smiley.html'		, FCKConfig.SmileyWindowWidth, FCKConfig.SmileyWindowHeight ) ; break ;
		case 'Table'		: oCommand = new FCKDialogCommand( 'Table'		, FCKLang.DlgTableTitle			, 'dialog/fck_table.html'		, 480, 250 ) ; break ;
		case 'TableProp'	: oCommand = new FCKDialogCommand( 'Table'		, FCKLang.DlgTableTitle			, 'dialog/fck_table.html?Parent', 480, 250 ) ; break ;
		case 'TableCellProp': oCommand = new FCKDialogCommand( 'TableCell'	, FCKLang.DlgCellTitle			, 'dialog/fck_tablecell.html'	, 550, 240 ) ; break ;

		case 'Style'		: oCommand = new FCKStyleCommand() ; break ;

		case 'FontName'		: oCommand = new FCKFontNameCommand() ; break ;
		case 'FontSize'		: oCommand = new FCKFontSizeCommand() ; break ;
		case 'FontFormat'	: oCommand = new FCKFormatBlockCommand() ; break ;

		case 'Source'		: oCommand = new FCKSourceCommand() ; break ;
		case 'Preview'		: oCommand = new FCKPreviewCommand() ; break ;
		case 'Save'			: oCommand = new FCKSaveCommand() ; break ;
		case 'NewPage'		: oCommand = new FCKNewPageCommand() ; break ;
		case 'PageBreak'	: oCommand = new FCKPageBreakCommand() ; break ;
		case 'Rule'			: oCommand = new FCKRuleCommand() ; break ;
		case 'Nbsp'			: oCommand = new FCKNbsp() ; break ;

		case 'TextColor'	: oCommand = new FCKTextColorCommand('ForeColor') ; break ;
		case 'BGColor'		: oCommand = new FCKTextColorCommand('BackColor') ; break ;

		case 'Paste'		: oCommand = new FCKPasteCommand() ; break ;
		case 'PasteText'	: oCommand = new FCKPastePlainTextCommand() ; break ;
		case 'PasteWord'	: oCommand = new FCKPasteWordCommand() ; break ;

		case 'JustifyLeft'	: oCommand = new FCKJustifyCommand( 'left' ) ; break ;
		case 'JustifyCenter'	: oCommand = new FCKJustifyCommand( 'center' ) ; break ;
		case 'JustifyRight'	: oCommand = new FCKJustifyCommand( 'right' ) ; break ;
		case 'JustifyFull'	: oCommand = new FCKJustifyCommand( 'justify' ) ; break ;
		case 'Indent'	: oCommand = new FCKIndentCommand( 'indent', FCKConfig.IndentLength ) ; break ;
		case 'Outdent'	: oCommand = new FCKIndentCommand( 'outdent', FCKConfig.IndentLength * -1 ) ; break ;
		case 'Blockquote'	: oCommand = new FCKBlockQuoteCommand() ; break ;
		case 'CreateDiv'	: oCommand = new FCKDialogCommand( 'CreateDiv', FCKLang.CreateDiv, 'dialog/fck_div.html', 380, 210, null, null, true ) ; break ;
		case 'EditDiv'		: oCommand = new FCKDialogCommand( 'EditDiv', FCKLang.EditDiv, 'dialog/fck_div.html', 380, 210, null, null, false ) ; break ;
		case 'DeleteDiv'	: oCommand = new FCKDeleteDivCommand() ; break ;

		case 'TableInsertRowAfter'		: oCommand = new FCKTableCommand('TableInsertRowAfter') ; break ;
		case 'TableInsertRowBefore'		: oCommand = new FCKTableCommand('TableInsertRowBefore') ; break ;
		case 'TableDeleteRows'			: oCommand = new FCKTableCommand('TableDeleteRows') ; break ;
		case 'TableInsertColumnAfter'	: oCommand = new FCKTableCommand('TableInsertColumnAfter') ; break ;
		case 'TableInsertColumnBefore'	: oCommand = new FCKTableCommand('TableInsertColumnBefore') ; break ;
		case 'TableDeleteColumns'		: oCommand = new FCKTableCommand('TableDeleteColumns') ; break ;
		case 'TableInsertCellAfter'		: oCommand = new FCKTableCommand('TableInsertCellAfter') ; break ;
		case 'TableInsertCellBefore'	: oCommand = new FCKTableCommand('TableInsertCellBefore') ; break ;
		case 'TableDeleteCells'			: oCommand = new FCKTableCommand('TableDeleteCells') ; break ;
		case 'TableMergeCells'			: oCommand = new FCKTableCommand('TableMergeCells') ; break ;
		case 'TableMergeRight'			: oCommand = new FCKTableCommand('TableMergeRight') ; break ;
		case 'TableMergeDown'			: oCommand = new FCKTableCommand('TableMergeDown') ; break ;
		case 'TableHorizontalSplitCell'	: oCommand = new FCKTableCommand('TableHorizontalSplitCell') ; break ;
		case 'TableVerticalSplitCell'	: oCommand = new FCKTableCommand('TableVerticalSplitCell') ; break ;
		case 'TableDelete'				: oCommand = new FCKTableCommand('TableDelete') ; break ;

		case 'Form'			: oCommand = new FCKDialogCommand( 'Form'		, FCKLang.Form			, 'dialog/fck_form.html'		, 380, 210 ) ; break ;
		case 'Checkbox'		: oCommand = new FCKDialogCommand( 'Checkbox'	, FCKLang.Checkbox		, 'dialog/fck_checkbox.html'	, 380, 200 ) ; break ;
		case 'Radio'		: oCommand = new FCKDialogCommand( 'Radio'		, FCKLang.RadioButton	, 'dialog/fck_radiobutton.html'	, 380, 200 ) ; break ;
		case 'TextField'	: oCommand = new FCKDialogCommand( 'TextField'	, FCKLang.TextField		, 'dialog/fck_textfield.html'	, 380, 210 ) ; break ;
		case 'Textarea'		: oCommand = new FCKDialogCommand( 'Textarea'	, FCKLang.Textarea		, 'dialog/fck_textarea.html'	, 380, 210 ) ; break ;
		case 'HiddenField'	: oCommand = new FCKDialogCommand( 'HiddenField', FCKLang.HiddenField	, 'dialog/fck_hiddenfield.html'	, 380, 190 ) ; break ;
		case 'Button'		: oCommand = new FCKDialogCommand( 'Button'		, FCKLang.Button		, 'dialog/fck_button.html'		, 380, 210 ) ; break ;
		case 'Select'		: oCommand = new FCKDialogCommand( 'Select'		, FCKLang.SelectionField, 'dialog/fck_select.html'		, 400, 340 ) ; break ;
		case 'ImageButton'	: oCommand = new FCKDialogCommand( 'ImageButton', FCKLang.ImageButton	, 'dialog/fck_image.html?ImageButton', 450, 390 ) ; break ;

		case 'SpellCheck'	: oCommand = new FCKSpellCheckCommand() ; break ;
		case 'FitWindow'	: oCommand = new FCKFitWindow() ; break ;

		case 'Undo'	: oCommand = new FCKUndoCommand() ; break ;
		case 'Redo'	: oCommand = new FCKRedoCommand() ; break ;
		case 'Copy'	: oCommand = new FCKCutCopyCommand( false ) ; break ;
		case 'Cut'	: oCommand = new FCKCutCopyCommand( true ) ; break ;

		case 'SelectAll'			: oCommand = new FCKSelectAllCommand() ; break ;
		case 'InsertOrderedList'	: oCommand = new FCKListCommand( 'insertorderedlist', 'ol' ) ; break ;
		case 'InsertUnorderedList'	: oCommand = new FCKListCommand( 'insertunorderedlist', 'ul' ) ; break ;
		case 'ShowBlocks' : oCommand = new FCKShowBlockCommand( 'ShowBlocks', FCKConfig.StartupShowBlocks ? FCK_TRISTATE_ON : FCK_TRISTATE_OFF ) ; break ;

		// Generic Undefined command (usually used when a command is under development).
		case 'Undefined'	: oCommand = new FCKUndefinedCommand() ; break ;

		case 'Scayt' : oCommand = FCKScayt.CreateCommand() ; break ;
		case 'ScaytContext' : oCommand = FCKScayt.CreateContextCommand() ; break ;

		// By default we assume that it is a named command.
		default:
			if ( FCKRegexLib.NamedCommands.test( commandName ) )
				oCommand = new FCKNamedCommand( commandName ) ;
			else
			{
				alert( FCKLang.UnknownCommand.replace( /%1/g, commandName ) ) ;
				return null ;
			}
	}

	FCKCommands.LoadedCommands[ commandName ] = oCommand ;

	return oCommand ;
}

// Gets the state of the "Document Properties" button. It must be enabled only
// when "Full Page" editing is available.
FCKCommands.GetFullPageState = function()
{
	return FCKConfig.FullPage ? FCK_TRISTATE_OFF : FCK_TRISTATE_DISABLED ;
}


FCKCommands.GetBooleanState = function( isDisabled )
{
	return isDisabled ? FCK_TRISTATE_DISABLED : FCK_TRISTATE_OFF ;
}
