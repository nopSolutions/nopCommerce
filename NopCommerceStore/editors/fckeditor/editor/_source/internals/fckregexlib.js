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
 * These are some Regular Expressions used by the editor.
 */

var FCKRegexLib =
{
// This is the Regular expression used by the SetData method for the "&apos;" entity.
AposEntity		: /&apos;/gi ,

// Used by the Styles combo to identify styles that can't be applied to text.
ObjectElements	: /^(?:IMG|TABLE|TR|TD|TH|INPUT|SELECT|TEXTAREA|HR|OBJECT|A|UL|OL|LI)$/i ,

// List all named commands (commands that can be interpreted by the browser "execCommand" method.
NamedCommands	: /^(?:Cut|Copy|Paste|Print|SelectAll|RemoveFormat|Unlink|Undo|Redo|Bold|Italic|Underline|StrikeThrough|Subscript|Superscript|JustifyLeft|JustifyCenter|JustifyRight|JustifyFull|Outdent|Indent|InsertOrderedList|InsertUnorderedList|InsertHorizontalRule)$/i ,

BeforeBody	: /(^[\s\S]*\<body[^\>]*\>)/i,
AfterBody	: /(\<\/body\>[\s\S]*$)/i,

// Temporary text used to solve some browser specific limitations.
ToReplace		: /___fcktoreplace:([\w]+)/ig ,

// Get the META http-equiv attribute from the tag.
MetaHttpEquiv	: /http-equiv\s*=\s*["']?([^"' ]+)/i ,

HasBaseTag		: /<base /i ,
HasBodyTag		: /<body[\s|>]/i ,

HtmlOpener		: /<html\s?[^>]*>/i ,
HeadOpener		: /<head\s?[^>]*>/i ,
HeadCloser		: /<\/head\s*>/i ,

// Temporary classes (Tables without border, Anchors with content) used in IE
FCK_Class		: /\s*FCK__[^ ]*(?=\s+|$)/ ,

// Validate element names (it must be in lowercase).
ElementName		: /(^[a-z_:][\w.\-:]*\w$)|(^[a-z_]$)/ ,

// Used in conjunction with the FCKConfig.ForceSimpleAmpersand configuration option.
ForceSimpleAmpersand : /___FCKAmp___/g ,

// Get the closing parts of the tags with no closing tags, like <br/>... gets the "/>" part.
SpaceNoClose	: /\/>/g ,

// Empty elements may be <p></p> or even a simple opening <p> (see #211).
EmptyParagraph	: /^<(p|div|address|h\d|center)(?=[ >])[^>]*>\s*(<\/\1>)?$/ ,

EmptyOutParagraph : /^<(p|div|address|h\d|center)(?=[ >])[^>]*>(?:\s*|&nbsp;|&#160;)(<\/\1>)?$/ ,

TagBody			: /></ ,

GeckoEntitiesMarker : /#\?-\:/g ,

// We look for the "src" and href attribute with the " or ' or without one of
// them. We have to do all in one, otherwise we will have problems with URLs
// like "thumbnail.php?src=someimage.jpg" (SF-BUG 1554141).
ProtectUrlsImg	: /<img(?=\s).*?\ssrc=((?:(?:\s*)("|').*?\2)|(?:[^"'][^ >]+))/gi ,
ProtectUrlsA	: /<a(?=\s).*?\shref=((?:(?:\s*)("|').*?\2)|(?:[^"'][^ >]+))/gi ,
ProtectUrlsArea	: /<area(?=\s).*?\shref=((?:(?:\s*)("|').*?\2)|(?:[^"'][^ >]+))/gi ,

Html4DocType	: /HTML 4\.0 Transitional/i ,
DocTypeTag		: /<!DOCTYPE[^>]*>/i ,
HtmlDocType		: /DTD HTML/ ,

// These regex are used to save the original event attributes in the HTML.
TagsWithEvent	: /<[^\>]+ on\w+[\s\r\n]*=[\s\r\n]*?('|")[\s\S]+?\>/g ,
EventAttributes	: /\s(on\w+)[\s\r\n]*=[\s\r\n]*?('|")([\s\S]*?)\2/g,
ProtectedEvents : /\s\w+_fckprotectedatt="([^"]+)"/g,

StyleProperties : /\S+\s*:/g,

// [a-zA-Z0-9:]+ seams to be more efficient than [\w:]+
InvalidSelfCloseTags : /(<(?!base|meta|link|hr|br|param|img|area|input)([a-zA-Z0-9:]+)[^>]*)\/>/gi,

// All variables defined in a style attribute or style definition. The variable
// name is returned with $2.
StyleVariableAttName : /#\(\s*("|')(.+?)\1[^\)]*\s*\)/g,

RegExp : /^\/(.*)\/([gim]*)$/,

HtmlTag : /<[^\s<>](?:"[^"]*"|'[^']*'|[^<])*>/
} ;
