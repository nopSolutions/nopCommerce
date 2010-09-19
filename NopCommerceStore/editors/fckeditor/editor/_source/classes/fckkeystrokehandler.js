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
 * Control keyboard keystroke combinations.
 */

var FCKKeystrokeHandler = function( cancelCtrlDefaults )
{
	this.Keystrokes = new Object() ;
	this.CancelCtrlDefaults = ( cancelCtrlDefaults !== false ) ;
}

/*
 * Listen to keystroke events in an element or DOM document object.
 *		@target: The element or document to listen to keystroke events.
 */
FCKKeystrokeHandler.prototype.AttachToElement = function( target )
{
	// For newer browsers, it is enough to listen to the keydown event only.
	// Some browsers instead, don't cancel key events in the keydown, but in the
	// keypress. So we must do a longer trip in those cases.
	FCKTools.AddEventListenerEx( target, 'keydown', _FCKKeystrokeHandler_OnKeyDown, this ) ;
	if ( FCKBrowserInfo.IsGecko10 || FCKBrowserInfo.IsOpera || ( FCKBrowserInfo.IsGecko && FCKBrowserInfo.IsMac ) )
		FCKTools.AddEventListenerEx( target, 'keypress', _FCKKeystrokeHandler_OnKeyPress, this ) ;
}

/*
 * Sets a list of keystrokes. It can receive either a single array or "n"
 * arguments, each one being an array of 1 or 2 elemenst. The first element
 * is the keystroke combination, and the second is the value to assign to it.
 * If the second element is missing, the keystroke definition is removed.
 */
FCKKeystrokeHandler.prototype.SetKeystrokes = function()
{
	// Look through the arguments.
	for ( var i = 0 ; i < arguments.length ; i++ )
	{
		var keyDef = arguments[i] ;

		// If the configuration for the keystrokes is missing some element or has any extra comma
		// this item won't be valid, so skip it and keep on processing.
		if ( !keyDef )
			continue ;

		if ( typeof( keyDef[0] ) == 'object' )		// It is an array with arrays defining the keystrokes.
			this.SetKeystrokes.apply( this, keyDef ) ;
		else
		{
			if ( keyDef.length == 1 )		// If it has only one element, remove the keystroke.
				delete this.Keystrokes[ keyDef[0] ] ;
			else							// Otherwise add it.
				this.Keystrokes[ keyDef[0] ] = keyDef[1] === true ? true : keyDef ;
		}
	}
}

function _FCKKeystrokeHandler_OnKeyDown( ev, keystrokeHandler )
{
	// Get the key code.
	var keystroke = ev.keyCode || ev.which ;

	// Combine it with the CTRL, SHIFT and ALT states.
	var keyModifiers = 0 ;

	if ( ev.ctrlKey || ev.metaKey )
		keyModifiers += CTRL ;

	if ( ev.shiftKey )
		keyModifiers += SHIFT ;

	if ( ev.altKey )
		keyModifiers += ALT ;

	var keyCombination = keystroke + keyModifiers ;

	var cancelIt = keystrokeHandler._CancelIt = false ;

	// Look for its definition availability.
	var keystrokeValue = keystrokeHandler.Keystrokes[ keyCombination ] ;

//	FCKDebug.Output( 'KeyDown: ' + keyCombination + ' - Value: ' + keystrokeValue ) ;

	// If the keystroke is defined
	if ( keystrokeValue )
	{
		// If the keystroke has been explicitly set to "true" OR calling the
		// "OnKeystroke" event, it doesn't return "true", the default behavior
		// must be preserved.
		if ( keystrokeValue === true || !( keystrokeHandler.OnKeystroke && keystrokeHandler.OnKeystroke.apply( keystrokeHandler, keystrokeValue ) ) )
			return true ;

		cancelIt = true ;
	}

	// By default, it will cancel all combinations with the CTRL key only (except positioning keys).
	if ( cancelIt || ( keystrokeHandler.CancelCtrlDefaults && keyModifiers == CTRL && ( keystroke < 33 || keystroke > 40 ) ) )
	{
		keystrokeHandler._CancelIt = true ;

		if ( ev.preventDefault )
			return ev.preventDefault() ;

		ev.returnValue = false ;
		ev.cancelBubble = true ;
		return false ;
	}

	return true ;
}

function _FCKKeystrokeHandler_OnKeyPress( ev, keystrokeHandler )
{
	if ( keystrokeHandler._CancelIt )
	{
//		FCKDebug.Output( 'KeyPress Cancel', 'Red') ;

		if ( ev.preventDefault )
			return ev.preventDefault() ;

		return false ;
	}

	return true ;
}
