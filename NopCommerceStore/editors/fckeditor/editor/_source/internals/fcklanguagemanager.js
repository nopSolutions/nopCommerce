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
 * Defines the FCKLanguageManager object that is used for language
 * operations.
 */

var FCKLanguageManager = FCK.Language =
{
	AvailableLanguages :
	{
		af		: 'Afrikaans',
		ar		: 'Arabic',
		bg		: 'Bulgarian',
		bn		: 'Bengali/Bangla',
		bs		: 'Bosnian',
		ca		: 'Catalan',
		cs		: 'Czech',
		da		: 'Danish',
		de		: 'German',
		el		: 'Greek',
		en		: 'English',
		'en-au'	: 'English (Australia)',
		'en-ca'	: 'English (Canadian)',
		'en-uk'	: 'English (United Kingdom)',
		eo		: 'Esperanto',
		es		: 'Spanish',
		et		: 'Estonian',
		eu		: 'Basque',
		fa		: 'Persian',
		fi		: 'Finnish',
		fo		: 'Faroese',
		fr		: 'French',
		'fr-ca'	: 'French (Canada)',
		gl		: 'Galician',
		gu		: 'Gujarati',
		he		: 'Hebrew',
		hi		: 'Hindi',
		hr		: 'Croatian',
		hu		: 'Hungarian',
		is		: 'Icelandic',
		it		: 'Italian',
		ja		: 'Japanese',
		km		: 'Khmer',
		ko		: 'Korean',
		lt		: 'Lithuanian',
		lv		: 'Latvian',
		mn		: 'Mongolian',
		ms		: 'Malay',
		nb		: 'Norwegian Bokmal',
		nl		: 'Dutch',
		no		: 'Norwegian',
		pl		: 'Polish',
		pt		: 'Portuguese (Portugal)',
		'pt-br'	: 'Portuguese (Brazil)',
		ro		: 'Romanian',
		ru		: 'Russian',
		sk		: 'Slovak',
		sl		: 'Slovenian',
		sr		: 'Serbian (Cyrillic)',
		'sr-latn'	: 'Serbian (Latin)',
		sv		: 'Swedish',
		th		: 'Thai',
		tr		: 'Turkish',
		uk		: 'Ukrainian',
		vi		: 'Vietnamese',
		zh		: 'Chinese Traditional',
		'zh-cn'	: 'Chinese Simplified'
	},

	GetActiveLanguage : function()
	{
		if ( FCKConfig.AutoDetectLanguage )
		{
			var sUserLang ;

			// IE accepts "navigator.userLanguage" while Gecko "navigator.language".
			if ( navigator.userLanguage )
				sUserLang = navigator.userLanguage.toLowerCase() ;
			else if ( navigator.language )
				sUserLang = navigator.language.toLowerCase() ;
			else
			{
				// Firefox 1.0 PR has a bug: it doens't support the "language" property.
				return FCKConfig.DefaultLanguage ;
			}

			// Some language codes are set in 5 characters,
			// like "pt-br" for Brazilian Portuguese.
			if ( sUserLang.length >= 5 )
			{
				sUserLang = sUserLang.substr(0,5) ;
				if ( this.AvailableLanguages[sUserLang] ) return sUserLang ;
			}

			// If the user's browser is set to, for example, "pt-br" but only the
			// "pt" language file is available then get that file.
			if ( sUserLang.length >= 2 )
			{
				sUserLang = sUserLang.substr(0,2) ;
				if ( this.AvailableLanguages[sUserLang] ) return sUserLang ;
			}
		}

		return this.DefaultLanguage ;
	},

	TranslateElements : function( targetDocument, tag, propertyToSet, encode )
	{
		var e = targetDocument.getElementsByTagName(tag) ;
		var sKey, s ;
		for ( var i = 0 ; i < e.length ; i++ )
		{
			// The extra () is to avoid a warning with strict error checking. This is ok.
			if ( (sKey = e[i].getAttribute( 'fckLang' )) )
			{
				// The extra () is to avoid a warning with strict error checking. This is ok.
				if ( (s = FCKLang[ sKey ]) )
				{
					if ( encode )
						s = FCKTools.HTMLEncode( s ) ;
					e[i][ propertyToSet ] = s ;
				}
			}
		}
	},

	TranslatePage : function( targetDocument )
	{
		this.TranslateElements( targetDocument, 'INPUT', 'value' ) ;
		this.TranslateElements( targetDocument, 'SPAN', 'innerHTML' ) ;
		this.TranslateElements( targetDocument, 'LABEL', 'innerHTML' ) ;
		this.TranslateElements( targetDocument, 'OPTION', 'innerHTML', true ) ;
		this.TranslateElements( targetDocument, 'LEGEND', 'innerHTML' ) ;
	},

	Initialize : function()
	{
		if ( this.AvailableLanguages[ FCKConfig.DefaultLanguage ] )
			this.DefaultLanguage = FCKConfig.DefaultLanguage ;
		else
			this.DefaultLanguage = 'en' ;

		this.ActiveLanguage = new Object() ;
		this.ActiveLanguage.Code = this.GetActiveLanguage() ;
		this.ActiveLanguage.Name = this.AvailableLanguages[ this.ActiveLanguage.Code ] ;
	}
} ;
