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
 * FCKToolbarSpecialCombo Class: This is a "abstract" base class to be used
 * by the special combo toolbar elements like font name, font size, paragraph format, etc...
 *
 * The following properties and methods must be implemented when inheriting from
 * this class:
 * 	- Property:	CommandName							[ The command name to be executed ]
 * 	- Method:	GetLabel()							[ Returns the label ]
 * 	-			CreateItems( targetSpecialCombo )	[ Add all items in the special combo ]
 */

var FCKToolbarSpecialCombo = function()
{
	this.SourceView			= false ;
	this.ContextSensitive	= true ;
	this.FieldWidth			= null ;
	this.PanelWidth			= null ;
	this.PanelMaxHeight		= null ;
	//this._LastValue			= null ;
}


FCKToolbarSpecialCombo.prototype.DefaultLabel = '' ;

function FCKToolbarSpecialCombo_OnSelect( itemId, item )
{
	FCK.ToolbarSet.CurrentInstance.Commands.GetCommand( this.CommandName ).Execute( itemId, item ) ;
}

FCKToolbarSpecialCombo.prototype.Create = function( targetElement )
{
	this._Combo = new FCKSpecialCombo( this.GetLabel(), this.FieldWidth, this.PanelWidth, this.PanelMaxHeight, FCKBrowserInfo.IsIE ? window : FCKTools.GetElementWindow( targetElement ).parent ) ;

	/*
	this._Combo.FieldWidth		= this.FieldWidth		!= null ? this.FieldWidth		: 100 ;
	this._Combo.PanelWidth		= this.PanelWidth		!= null ? this.PanelWidth		: 150 ;
	this._Combo.PanelMaxHeight	= this.PanelMaxHeight	!= null ? this.PanelMaxHeight	: 150 ;
	*/

	//this._Combo.Command.Name = this.Command.Name;
//	this._Combo.Label	= this.Label ;
	this._Combo.Tooltip	= this.Tooltip ;
	this._Combo.Style	= this.Style ;

	this.CreateItems( this._Combo ) ;

	this._Combo.Create( targetElement ) ;

	this._Combo.CommandName = this.CommandName ;

	this._Combo.OnSelect = FCKToolbarSpecialCombo_OnSelect ;
}

function FCKToolbarSpecialCombo_RefreshActiveItems( combo, value )
{
	combo.DeselectAll() ;
	combo.SelectItem( value ) ;
	combo.SetLabelById( value ) ;
}

FCKToolbarSpecialCombo.prototype.RefreshState = function()
{
	// Gets the actual state.
	var eState ;

//	if ( FCK.EditMode == FCK_EDITMODE_SOURCE && ! this.SourceView )
//		eState = FCK_TRISTATE_DISABLED ;
//	else
//	{
		var sValue = FCK.ToolbarSet.CurrentInstance.Commands.GetCommand( this.CommandName ).GetState() ;

//		FCKDebug.Output( 'RefreshState of Special Combo "' + this.TypeOf + '" - State: ' + sValue ) ;

		if ( sValue != FCK_TRISTATE_DISABLED )
		{
			eState = FCK_TRISTATE_ON ;

			if ( this.RefreshActiveItems )
				this.RefreshActiveItems( this._Combo, sValue ) ;
			else
			{
				if ( this._LastValue !== sValue)
				{
					this._LastValue = sValue ;

					if ( !sValue || sValue.length == 0 )
					{
						this._Combo.DeselectAll() ;
						this._Combo.SetLabel( this.DefaultLabel ) ;
					}
					else
						FCKToolbarSpecialCombo_RefreshActiveItems( this._Combo, sValue ) ;
				}
			}
		}
		else
			eState = FCK_TRISTATE_DISABLED ;
//	}

	// If there are no state changes then do nothing and return.
	if ( eState == this.State ) return ;

	if ( eState == FCK_TRISTATE_DISABLED )
	{
		this._Combo.DeselectAll() ;
		this._Combo.SetLabel( '' ) ;
	}

	// Sets the actual state.
	this.State = eState ;

	// Updates the graphical state.
	this._Combo.SetEnabled( eState != FCK_TRISTATE_DISABLED ) ;
}

FCKToolbarSpecialCombo.prototype.Enable = function()
{
	this.RefreshState() ;
}

FCKToolbarSpecialCombo.prototype.Disable = function()
{
	this.State = FCK_TRISTATE_DISABLED ;
	this._Combo.DeselectAll() ;
	this._Combo.SetLabel( '' ) ;
	this._Combo.SetEnabled( false ) ;
}
