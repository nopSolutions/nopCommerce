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
 * This file define the HTML entities handled by the editor.
 */

var FCKXHtmlEntities = new Object() ;

FCKXHtmlEntities.Initialize = function()
{
	if ( FCKXHtmlEntities.Entities )
		return ;

	var sChars = '' ;
	var oEntities, e ;

	if ( FCKConfig.ProcessHTMLEntities )
	{
		FCKXHtmlEntities.Entities = {
			// Latin-1 Entities
			' ':'nbsp',
			'¡':'iexcl',
			'¢':'cent',
			'£':'pound',
			'¤':'curren',
			'¥':'yen',
			'¦':'brvbar',
			'§':'sect',
			'¨':'uml',
			'©':'copy',
			'ª':'ordf',
			'«':'laquo',
			'¬':'not',
			'­':'shy',
			'®':'reg',
			'¯':'macr',
			'°':'deg',
			'±':'plusmn',
			'²':'sup2',
			'³':'sup3',
			'´':'acute',
			'µ':'micro',
			'¶':'para',
			'·':'middot',
			'¸':'cedil',
			'¹':'sup1',
			'º':'ordm',
			'»':'raquo',
			'¼':'frac14',
			'½':'frac12',
			'¾':'frac34',
			'¿':'iquest',
			'×':'times',
			'÷':'divide',

			// Symbols

			'ƒ':'fnof',
			'•':'bull',
			'…':'hellip',
			'′':'prime',
			'″':'Prime',
			'‾':'oline',
			'⁄':'frasl',
			'℘':'weierp',
			'ℑ':'image',
			'ℜ':'real',
			'™':'trade',
			'ℵ':'alefsym',
			'←':'larr',
			'↑':'uarr',
			'→':'rarr',
			'↓':'darr',
			'↔':'harr',
			'↵':'crarr',
			'⇐':'lArr',
			'⇑':'uArr',
			'⇒':'rArr',
			'⇓':'dArr',
			'⇔':'hArr',
			'∀':'forall',
			'∂':'part',
			'∃':'exist',
			'∅':'empty',
			'∇':'nabla',
			'∈':'isin',
			'∉':'notin',
			'∋':'ni',
			'∏':'prod',
			'∑':'sum',
			'−':'minus',
			'∗':'lowast',
			'√':'radic',
			'∝':'prop',
			'∞':'infin',
			'∠':'ang',
			'∧':'and',
			'∨':'or',
			'∩':'cap',
			'∪':'cup',
			'∫':'int',
			'∴':'there4',
			'∼':'sim',
			'≅':'cong',
			'≈':'asymp',
			'≠':'ne',
			'≡':'equiv',
			'≤':'le',
			'≥':'ge',
			'⊂':'sub',
			'⊃':'sup',
			'⊄':'nsub',
			'⊆':'sube',
			'⊇':'supe',
			'⊕':'oplus',
			'⊗':'otimes',
			'⊥':'perp',
			'⋅':'sdot',
			'\u2308':'lceil',
			'\u2309':'rceil',
			'\u230a':'lfloor',
			'\u230b':'rfloor',
			'\u2329':'lang',
			'\u232a':'rang',
			'◊':'loz',
			'♠':'spades',
			'♣':'clubs',
			'♥':'hearts',
			'♦':'diams',

			// Other Special Characters

			'"':'quot',
		//	'&':'amp',		// This entity is automatically handled by the XHTML parser.
		//	'<':'lt',		// This entity is automatically handled by the XHTML parser.
			'>':'gt',			// Opera and Safari don't encode it in their implementation
			'ˆ':'circ',
			'˜':'tilde',
			' ':'ensp',
			' ':'emsp',
			' ':'thinsp',
			'‌':'zwnj',
			'‍':'zwj',
			'‎':'lrm',
			'‏':'rlm',
			'–':'ndash',
			'—':'mdash',
			'‘':'lsquo',
			'’':'rsquo',
			'‚':'sbquo',
			'“':'ldquo',
			'”':'rdquo',
			'„':'bdquo',
			'†':'dagger',
			'‡':'Dagger',
			'‰':'permil',
			'‹':'lsaquo',
			'›':'rsaquo',
			'€':'euro'
		} ;

		// Process Base Entities.
		for ( e in FCKXHtmlEntities.Entities )
			sChars += e ;

		// Include Latin Letters Entities.
		if ( FCKConfig.IncludeLatinEntities )
		{
			oEntities = {
				'À':'Agrave',
				'Á':'Aacute',
				'Â':'Acirc',
				'Ã':'Atilde',
				'Ä':'Auml',
				'Å':'Aring',
				'Æ':'AElig',
				'Ç':'Ccedil',
				'È':'Egrave',
				'É':'Eacute',
				'Ê':'Ecirc',
				'Ë':'Euml',
				'Ì':'Igrave',
				'Í':'Iacute',
				'Î':'Icirc',
				'Ï':'Iuml',
				'Ð':'ETH',
				'Ñ':'Ntilde',
				'Ò':'Ograve',
				'Ó':'Oacute',
				'Ô':'Ocirc',
				'Õ':'Otilde',
				'Ö':'Ouml',
				'Ø':'Oslash',
				'Ù':'Ugrave',
				'Ú':'Uacute',
				'Û':'Ucirc',
				'Ü':'Uuml',
				'Ý':'Yacute',
				'Þ':'THORN',
				'ß':'szlig',
				'à':'agrave',
				'á':'aacute',
				'â':'acirc',
				'ã':'atilde',
				'ä':'auml',
				'å':'aring',
				'æ':'aelig',
				'ç':'ccedil',
				'è':'egrave',
				'é':'eacute',
				'ê':'ecirc',
				'ë':'euml',
				'ì':'igrave',
				'í':'iacute',
				'î':'icirc',
				'ï':'iuml',
				'ð':'eth',
				'ñ':'ntilde',
				'ò':'ograve',
				'ó':'oacute',
				'ô':'ocirc',
				'õ':'otilde',
				'ö':'ouml',
				'ø':'oslash',
				'ù':'ugrave',
				'ú':'uacute',
				'û':'ucirc',
				'ü':'uuml',
				'ý':'yacute',
				'þ':'thorn',
				'ÿ':'yuml',
				'Œ':'OElig',
				'œ':'oelig',
				'Š':'Scaron',
				'š':'scaron',
				'Ÿ':'Yuml'
			} ;

			for ( e in oEntities )
			{
				FCKXHtmlEntities.Entities[ e ] = oEntities[ e ] ;
				sChars += e ;
			}

			oEntities = null ;
		}

		// Include Greek Letters Entities.
		if ( FCKConfig.IncludeGreekEntities )
		{
			oEntities = {
				'Α':'Alpha',
				'Β':'Beta',
				'Γ':'Gamma',
				'Δ':'Delta',
				'Ε':'Epsilon',
				'Ζ':'Zeta',
				'Η':'Eta',
				'Θ':'Theta',
				'Ι':'Iota',
				'Κ':'Kappa',
				'Λ':'Lambda',
				'Μ':'Mu',
				'Ν':'Nu',
				'Ξ':'Xi',
				'Ο':'Omicron',
				'Π':'Pi',
				'Ρ':'Rho',
				'Σ':'Sigma',
				'Τ':'Tau',
				'Υ':'Upsilon',
				'Φ':'Phi',
				'Χ':'Chi',
				'Ψ':'Psi',
				'Ω':'Omega',
				'α':'alpha',
				'β':'beta',
				'γ':'gamma',
				'δ':'delta',
				'ε':'epsilon',
				'ζ':'zeta',
				'η':'eta',
				'θ':'theta',
				'ι':'iota',
				'κ':'kappa',
				'λ':'lambda',
				'μ':'mu',
				'ν':'nu',
				'ξ':'xi',
				'ο':'omicron',
				'π':'pi',
				'ρ':'rho',
				'ς':'sigmaf',
				'σ':'sigma',
				'τ':'tau',
				'υ':'upsilon',
				'φ':'phi',
				'χ':'chi',
				'ψ':'psi',
				'ω':'omega',
				'\u03d1':'thetasym',
				'\u03d2':'upsih',
				'\u03d6':'piv'
			} ;

			for ( e in oEntities )
			{
				FCKXHtmlEntities.Entities[ e ] = oEntities[ e ] ;
				sChars += e ;
			}

			oEntities = null ;
		}
	}
	else
	{
		FCKXHtmlEntities.Entities = {
			'>':'gt' // Opera and Safari don't encode it in their implementation
		} ;
		sChars = '>';

		// Even if we are not processing the entities, we must render the &nbsp;
		// correctly. As we don't want HTML entities, let's use its numeric
		// representation (&#160).
		sChars += ' ' ;
	}

	// Create the Regex used to find entities in the text.
	var sRegexPattern = '[' + sChars + ']' ;

	if ( FCKConfig.ProcessNumericEntities )
		sRegexPattern = '[^ -~]|' + sRegexPattern ;

	var sAdditional = FCKConfig.AdditionalNumericEntities ;

	if ( sAdditional && sAdditional.length > 0 )
		sRegexPattern += '|' + FCKConfig.AdditionalNumericEntities ;

	FCKXHtmlEntities.EntitiesRegex = new RegExp( sRegexPattern, 'g' ) ;
}
