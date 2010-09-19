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
 */

var FCKScayt;

(function()
{
	var scaytOnLoad = [] ;
	var isEngineLoaded = ( FCK && FCK.EditorWindow && FCK.EditorWindow.parent.parent.scayt)
						? true : false ;
	var scaytEnable = false;
	var scaytReady  = false;

	function ScaytEngineLoad( callback )
	{
		if ( isEngineLoaded )
			return ;

		isEngineLoaded = true ;
		var top = FCK.EditorWindow.parent.parent;

		var init = function ()
		{
			window.scayt = top.scayt ;
			InitScayt() ;
			var ScaytCombobox =  FCKToolbarItems.LoadedItems[ 'ScaytCombobox' ] ;
			ScaytCombobox && ScaytCombobox.SetEnabled( scyt_control && scyt_control.disabled ) ;
			InitSetup() ;
		};

		if ( top.scayt )
		{
			init() ;
			return ;
		}

		// Compose the scayt url.
		if (FCK.Config.ScaytCustomUrl)
			FCK.Config.ScaytCustomUrl = new String(FCK.Config.ScaytCustomUrl).replace( new RegExp( "^http[s]*:\/\/"),"") ;

		var protocol	= document.location.protocol ;
		var baseUrl		= FCK.Config.ScaytCustomUrl ||'svc.spellchecker.net/spellcheck3/lf/scayt/scayt4.js' ;
		var scaytUrl	= protocol + '//' + baseUrl ;
		var scaytConfigBaseUrl =  ParseUrl( scaytUrl ).path +  '/' ;

		// SCAYT is targetted to CKEditor, so we need this trick to make it work here.
		var CKEDITOR = top.window.CKEDITOR || ( top.window.CKEDITOR = {} ) ;
		CKEDITOR._djScaytConfig =
		{
			baseUrl : scaytConfigBaseUrl,
			addOnLoad : function()
			{
				init();
			},
			isDebug : false
		};


		if ( callback )
			scaytOnLoad.push( callback ) ;

		DoLoadScript( scaytUrl ) ;
	}

	/**
	 * DoLoadScript - load scripts with dinamic tag script creating
	 * @param string url
	 */
	function DoLoadScript( url )
	{
		if (!url)
            return false ;
		var top = FCK.EditorWindow.parent.parent;
        var s = top.document.createElement('script') ;
        s.type = 'text/javascript' ;
        s.src = url ;
        top.document.getElementsByTagName('head')[0].appendChild(s) ;

        return true ;
	}

	function ParseUrl( data )
	{
		var m = data.match(/(.*)[\/\\]([^\/\\]+\.\w+)$/) ;
		return m ? { path: m[1], file: m[2] } : data ;
	}

	function createScaytControl ()
	{
		// Get public scayt params.
		var oParams = {} ;
		var top = FCK.EditorWindow.parent.parent;
		oParams.srcNodeRef				= FCK.EditingArea.IFrame; 		// Get the iframe.
		// syntax : AppName.AppVersion@AppRevision
		//oParams.assocApp  = "FCKEDITOR." + FCKeditorAPI.Varsion + "@" + FCKeditorAPI.VersionBuild;
		oParams.customerid 				= FCK.Config.ScaytCustomerid ;
		oParams.customDictionaryName 	= FCK.Config.ScaytCustomDictionaryName ;
		oParams.userDictionaryName 		= FCK.Config.ScaytUserDictionaryName ;
		oParams.defLang 				= FCK.Config.ScaytDefLang ;

		var scayt = top.scayt;
		var scayt_control = window.scayt_control = new scayt( oParams ) ;
	}

	function InitScayt()
	{
		createScaytControl();

		var scayt_control = window.scayt_control ;

		if ( scayt_control )
		{
			scayt_control.setDisabled( false ) ;
			scaytReady = true;
			scaytEnable = !scayt_control.disabled ;

			// set default scayt status
			var ScaytCombobox = FCKToolbarItems.LoadedItems[ 'ScaytCombobox' ] ;
			ScaytCombobox && ScaytCombobox.Enable() ;
			ShowScaytState() ;
		}

		for ( var i = 0 ; i < scaytOnLoad.length ; i++ )
		{
			try
			{
				scaytOnLoad[i].call( this ) ;
			}
			catch(err)
			{}
		}
	}

	// ###
	// SCAYT command class.
	var ScaytCommand  = function()
	{
		name = 'Scayt' ;
	}

	ScaytCommand.prototype.Execute = function( action )
	{
		switch ( action )
		{
			case 'Options' :
			case 'Langs' :
			case 'About' :
				if ( isEngineLoaded && scaytReady && !scaytEnable )
				{
					ScaytMessage( 'SCAYT is not enabled' );
					break;
				}

				if ( isEngineLoaded && scaytReady )
					FCKDialog.OpenDialog( 'Scayt', 'SCAYT Settings', 'dialog/fck_scayt.html?' + action.toLowerCase(), 343, 343 );
				break;

			default :
				if ( !isEngineLoaded )
				{
					var me = this;
					ScaytEngineLoad( function ()
						{
							me.SetEnabled( !window.scayt_control.disabled ) ;
						}) ;

					return true;
				}
				else if ( scaytReady )
				{
					// Switch the current scayt state.
					if ( scaytEnable )
						this.Disable() ;
					else
						this.Enable() ;

					ShowScaytState() ;
				}

		}

		if ( !isEngineLoaded )
			return ScaytMessage( 'SCAYT is not loaded' ) || false;

		if ( !scaytReady )
			return ScaytMessage( 'SCAYT is not ready' ) || false;


		return true;
	}

	ScaytCommand.prototype.Enable = function()
	{
		window.scayt_control.setDisabled( false ) ;
		scaytEnable = true;
	}

	ScaytCommand.prototype.Disable = function()
	{
		window.scayt_control.setDisabled( true ) ;
		scaytEnable = false;
	}

	ScaytCommand.prototype.SetEnabled = function( state )
	{
		if ( state )
			this.Enable() ;
		else
			this.Disable() ;

		ShowScaytState() ;
		return true;
	}

	ScaytCommand.prototype.GetState = function()
	{
		return FCK_TRISTATE_OFF;
	}

	function ShowScaytState()
	{
		var combo = FCKToolbarItems.GetItem( 'SpellCheck' ) ;

		if ( !combo || !combo._Combo || !combo._Combo._OuterTable )
			return;

		var bItem = combo._Combo._OuterTable.getElementsByTagName( 'img' )[1] ;
		var dNode = combo._Combo.Items['trigger'] ;

		if ( scaytEnable )
		{
			bItem.style.opacity = '1' ;
			dNode.innerHTML = GetStatusLabel() ;
		}
		else
		{
			bItem.style.opacity = '0.5' ;
			dNode.innerHTML = GetStatusLabel() ;
		}
	}

	function GetStatusLabel()
	{
		if ( !scaytReady )
			return  '<b>Enable SCAYT</b>' ;

		return scaytEnable ? '<b>Disable SCAYT</b>' : '<b>Enable SCAYT</b>' ;
	}

	// ###
	// Class for the toolbar item.
	var ToolbarScaytComboBox = function( tooltip, style )
	{
		this.Command = FCKCommands.GetCommand( 'Scayt' ) ;
		this.CommandName = 'Scayt' ;
		this.Label = this.GetLabel() ;
		this.Tooltip = FCKLang.ScaytTitle ;
		this.Style = FCK_TOOLBARITEM_ONLYTEXT ; //FCK_TOOLBARITEM_ICONTEXT OR FCK_TOOLBARITEM_ONLYTEXT
	}

	ToolbarScaytComboBox.prototype = new FCKToolbarSpecialCombo ;

	//Add the items to the combo list
	ToolbarScaytComboBox.prototype.CreateItems = function()
	{
		this._Combo.AddItem( 'Trigger', '<b>Enable SCAYT</b>' );
		this._Combo.AddItem( 'Options', FCKLang.ScaytTitleOptions || "Options"  );
		this._Combo.AddItem( 'Langs', FCKLang.ScaytTitleLangs || "Languages");
		this._Combo.AddItem( 'About', FCKLang.ScaytTitleAbout || "About");
	}

	// Label shown in the toolbar.
	ToolbarScaytComboBox.prototype.GetLabel = function()
	{
		var strip = FCKConfig.SkinPath + 'fck_strip.gif';

		return FCKBrowserInfo.IsIE ?
				'<div class="TB_Button_Image"><img src="' + strip + '" style="top:-192px"></div>'
			:
				'<img class="TB_Button_Image" src="' + FCK_SPACER_PATH + '" style="background-position: 0px -192px;background-image: url(' + strip + ');">';
	}

	function ScaytMessage( m )
	{
		m && alert( m ) ;
	}

	var ScaytContextCommand = function()
	{
		name = 'ScaytContext' ;
	}

	ScaytContextCommand.prototype.Execute = function( contextInfo )
	{
		var action = contextInfo && contextInfo.action,
			node = action && contextInfo.node,
			scayt_control = window.scayt_control;

		if ( node )
		{
			switch ( action )
			{
				case 'Suggestion' :
					scayt_control.replace( node, contextInfo.suggestion ) ;
					break ;
				case 'Ignore' :
					scayt_control.ignore( node ) ;
					break ;
				case 'Ignore All' :
					scayt_control.ignoreAll( node ) ;
					break ;
				case 'Add Word' :
					var top = FCK.EditorWindow.parent.parent ;
					top.scayt.addWordToUserDictionary( node ) ;
					break ;
			}
		}
	}

	// Register context menu listeners.
	function InitSetup()
	{
		FCK.ContextMenu.RegisterListener(
			{
				AddItems : function( menu )
				{
					var top = FCK.EditorWindow.parent.parent;

					var scayt_control = window.scayt_control,
						scayt = top.scayt;

					if ( !scayt_control )
						return;

					var node = scayt_control.getScaytNode() ;

					if ( !node )
						return;

					var suggestions = scayt.getSuggestion( scayt_control.getWord( node ), scayt_control.getLang() ) ;

					if ( !suggestions || !suggestions.length )
						return;

					menu.AddSeparator() ;

					var maxSuggestions = FCK.Config.ScaytMaxSuggestions || 5 ;
					var suggAveCount = ( maxSuggestions == -1 ) ? suggestions.length : maxSuggestions ;

					for ( var i = 0 ; i < suggAveCount ; i += 1 )
					{
						if ( suggestions[i] )
						{
							menu.AddItem( 'ScaytContext', suggestions[i], null, false, {
								'action' : 'Suggestion',
								'node' : node,
								'suggestion' : suggestions[i] } ) ;
						}
					}

					menu.AddSeparator() ;

					menu.AddItem( 'ScaytContext', 'Ignore', null, false, { 'action' : 'Ignore', 'node' : node } );
					menu.AddItem( 'ScaytContext', 'Ignore All', null, false, { 'action' : 'Ignore All', 'node' : node } );
					menu.AddItem( 'ScaytContext', 'Add Word', null, false, { 'action' : 'Add Word', 'node' : node } );
					try
					{
						if (scaytReady && scaytEnable)
							scayt_control.fireOnContextMenu( null, FCK.ContextMenu._InnerContextMenu);

					}
					catch( err ) {}
				}
			}) ;

		FCK.Events.AttachEvent( 'OnPaste', function()
			{
					window.scayt_control.refresh() ;
					return true;
			} ) ;
	}

	// ##
	// Register event listeners.

 	FCK.Events.AttachEvent( 'OnAfterSetHTML', function()
		{
			if ( FCKConfig.SpellChecker == 'SCAYT' )
			{
				if ( !isEngineLoaded && FCK.Config.ScaytAutoStartup )
					ScaytEngineLoad() ;

				if ( FCK.EditMode == FCK_EDITMODE_WYSIWYG && isEngineLoaded && scaytReady )
					createScaytControl();

				ShowScaytState() ;
			}
		} ) ;

	FCK.Events.AttachEvent( 'OnBeforeGetData', function()
		{
			scaytReady && window.scayt_control.reset();
		} ) ;

	FCK.Events.AttachEvent( 'OnAfterGetData', function()
		{
			scaytReady && window.scayt_control.refresh();
		} ) ;

	// ###
	// The main object that holds the SCAYT interaction in the code.
	FCKScayt =
	{
		CreateCommand : function()
		{
			return new ScaytCommand();
		},

		CreateContextCommand : function()
		{
			return new ScaytContextCommand();
		},

		CreateToolbarItem : function()
		{
			return new ToolbarScaytComboBox() ;
		}
	} ;
})() ;
