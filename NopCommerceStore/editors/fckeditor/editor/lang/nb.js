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
 * Norwegian Bokmål language file.
 */

var FCKLang =
{
// Language direction : "ltr" (left to right) or "rtl" (right to left).
Dir					: "ltr",

ToolbarCollapse		: "Skjul verktøylinje",
ToolbarExpand		: "Vis verktøylinje",

// Toolbar Items and Context Menu
Save				: "Lagre",
NewPage				: "Ny Side",
Preview				: "Forhåndsvis",
Cut					: "Klipp ut",
Copy				: "Kopier",
Paste				: "Lim inn",
PasteText			: "Lim inn som ren tekst",
PasteWord			: "Lim inn fra Word",
Print				: "Skriv ut",
SelectAll			: "Merk alt",
RemoveFormat		: "Fjern format",
InsertLinkLbl		: "Lenke",
InsertLink			: "Sett inn/Rediger lenke",
RemoveLink			: "Fjern lenke",
VisitLink			: "Åpne lenke",
Anchor				: "Sett inn/Rediger anker",
AnchorDelete		: "Fjern anker",
InsertImageLbl		: "Bilde",
InsertImage			: "Sett inn/Rediger bilde",
InsertFlashLbl		: "Flash",
InsertFlash			: "Sett inn/Rediger Flash",
InsertTableLbl		: "Tabell",
InsertTable			: "Sett inn/Rediger tabell",
InsertLineLbl		: "Linje",
InsertLine			: "Sett inn horisontal linje",
InsertSpecialCharLbl: "Spesielt tegn",
InsertSpecialChar	: "Sett inn spesielt tegn",
InsertSmileyLbl		: "Smil",
InsertSmiley		: "Sett inn smil",
About				: "Om FCKeditor",
Bold				: "Fet",
Italic				: "Kursiv",
Underline			: "Understrek",
StrikeThrough		: "Gjennomstrek",
Subscript			: "Senket skrift",
Superscript			: "Hevet skrift",
LeftJustify			: "Venstrejuster",
CenterJustify		: "Midtjuster",
RightJustify		: "Høyrejuster",
BlockJustify		: "Blokkjuster",
DecreaseIndent		: "Senk nivå",
IncreaseIndent		: "Øk nivå",
Blockquote			: "Blockquote",	//MISSING
CreateDiv			: "Create Div Container",	//MISSING
EditDiv				: "Edit Div Container",	//MISSING
DeleteDiv			: "Remove Div Container",	//MISSING
Undo				: "Angre",
Redo				: "Gjør om",
NumberedListLbl		: "Nummerert liste",
NumberedList		: "Sett inn/Fjern nummerert liste",
BulletedListLbl		: "Uordnet liste",
BulletedList		: "Sett inn/Fjern uordnet liste",
ShowTableBorders	: "Vis tabellrammer",
ShowDetails			: "Vis detaljer",
Style				: "Stil",
FontFormat			: "Format",
Font				: "Skrift",
FontSize			: "Størrelse",
TextColor			: "Tekstfarge",
BGColor				: "Bakgrunnsfarge",
Source				: "Kilde",
Find				: "Søk",
Replace				: "Erstatt",
SpellCheck			: "Stavekontroll",
UniversalKeyboard	: "Universelt tastatur",
PageBreakLbl		: "Sideskift",
PageBreak			: "Sett inn sideskift",

Form			: "Skjema",
Checkbox		: "Avmerkingsboks",
RadioButton		: "Alternativknapp",
TextField		: "Tekstboks",
Textarea		: "Tekstområde",
HiddenField		: "Skjult felt",
Button			: "Knapp",
SelectionField	: "Rullegardinliste",
ImageButton		: "Bildeknapp",

FitWindow		: "Maksimer størrelsen på redigeringsverktøyet",
ShowBlocks		: "Show Blocks",	//MISSING

// Context Menu
EditLink			: "Rediger lenke",
CellCM				: "Celle",
RowCM				: "Rader",
ColumnCM			: "Kolonne",
InsertRowAfter		: "Sett inn rad etter",
InsertRowBefore		: "Sett inn rad før",
DeleteRows			: "Slett rader",
InsertColumnAfter	: "Sett inn kolonne etter",
InsertColumnBefore	: "Sett inn kolonne før",
DeleteColumns		: "Slett kolonner",
InsertCellAfter		: "Sett inn celle etter",
InsertCellBefore	: "Sett inn celle før",
DeleteCells			: "Slett celler",
MergeCells			: "Slå sammen celler",
MergeRight			: "Slå sammen høyre",
MergeDown			: "Slå sammen ned",
HorizontalSplitCell	: "Del celle horisontalt",
VerticalSplitCell	: "Del celle vertikalt",
TableDelete			: "Slett tabell",
CellProperties		: "Egenskaper for celle",
TableProperties		: "Egenskaper for tabell",
ImageProperties		: "Egenskaper for bilde",
FlashProperties		: "Egenskaper for Flash-objekt",

AnchorProp			: "Egenskaper for anker",
ButtonProp			: "Egenskaper for knapp",
CheckboxProp		: "Egenskaper for avmerkingsboks",
HiddenFieldProp		: "Egenskaper for skjult felt",
RadioButtonProp		: "Egenskaper for alternativknapp",
ImageButtonProp		: "Egenskaper for bildeknapp",
TextFieldProp		: "Egenskaper for tekstfelt",
SelectionFieldProp	: "Egenskaper for rullegardinliste",
TextareaProp		: "Egenskaper for tekstområde",
FormProp			: "Egenskaper for skjema",

FontFormats			: "Normal;Formatert;Adresse;Tittel 1;Tittel 2;Tittel 3;Tittel 4;Tittel 5;Tittel 6;Normal (DIV)",

// Alerts and Messages
ProcessingXHTML		: "Lager XHTML. Vennligst vent...",
Done				: "Ferdig",
PasteWordConfirm	: "Teksten du prøver å lime inn ser ut som om den kommer fra Word. Vil du rense den for unødvendig kode før du limer inn?",
NotCompatiblePaste	: "Denne kommandoen er kun tilgjenglig for Internet Explorer versjon 5.5 eller bedre. Vil du fortsette uten å rense? (Du kan lime inn som ren tekst)",
UnknownToolbarItem	: "Ukjent menyvalg \"%1\"",
UnknownCommand		: "Ukjent kommando \"%1\"",
NotImplemented		: "Kommando ikke implimentert",
UnknownToolbarSet	: "Verktøylinjesett \"%1\" finnes ikke",
NoActiveX			: "Din nettlesers sikkerhetsinstillinger kan begrense noen av funksjonene i redigeringsverktøyet. Du må aktivere \"Kjør ActiveX-kontroller og plugin-modeller\". Du kan oppleve feil og advarsler om manglende funksjoner",
BrowseServerBlocked : "Kunne ikke åpne dialogboksen for filarkiv. Sjekk at popup-blokkering er deaktivert.",
DialogBlocked		: "Kunne ikke åpne dialogboksen. Sjekk at popup-blokkering er deaktivert.",
VisitLinkBlocked	: "Kunne ikke åpne et nytt vindu. Sjekk at popup-blokkering er deaktivert.",

// Dialogs
DlgBtnOK			: "OK",
DlgBtnCancel		: "Avbryt",
DlgBtnClose			: "Lukk",
DlgBtnBrowseServer	: "Bla igjennom server",
DlgAdvancedTag		: "Avansert",
DlgOpOther			: "<Annet>",
DlgInfoTab			: "Info",
DlgAlertUrl			: "Vennligst skriv inn URL-en",

// General Dialogs Labels
DlgGenNotSet		: "<ikke satt>",
DlgGenId			: "Id",
DlgGenLangDir		: "Språkretning",
DlgGenLangDirLtr	: "Venstre til høyre (VTH)",
DlgGenLangDirRtl	: "Høyre til venstre (HTV)",
DlgGenLangCode		: "Språkkode",
DlgGenAccessKey		: "Aksessknapp",
DlgGenName			: "Navn",
DlgGenTabIndex		: "Tab Indeks",
DlgGenLongDescr		: "Utvidet beskrivelse",
DlgGenClass			: "Stilarkklasser",
DlgGenTitle			: "Tittel",
DlgGenContType		: "Type",
DlgGenLinkCharset	: "Lenket språkkart",
DlgGenStyle			: "Stil",

// Image Dialog
DlgImgTitle			: "Bildeegenskaper",
DlgImgInfoTab		: "Bildeinformasjon",
DlgImgBtnUpload		: "Send det til serveren",
DlgImgURL			: "URL",
DlgImgUpload		: "Last opp",
DlgImgAlt			: "Alternativ tekst",
DlgImgWidth			: "Bredde",
DlgImgHeight		: "Høyde",
DlgImgLockRatio		: "Lås forhold",
DlgBtnResetSize		: "Tilbakestill størrelse",
DlgImgBorder		: "Ramme",
DlgImgHSpace		: "HMarg",
DlgImgVSpace		: "VMarg",
DlgImgAlign			: "Juster",
DlgImgAlignLeft		: "Venstre",
DlgImgAlignAbsBottom: "Abs bunn",
DlgImgAlignAbsMiddle: "Abs midten",
DlgImgAlignBaseline	: "Bunnlinje",
DlgImgAlignBottom	: "Bunn",
DlgImgAlignMiddle	: "Midten",
DlgImgAlignRight	: "Høyre",
DlgImgAlignTextTop	: "Tekst topp",
DlgImgAlignTop		: "Topp",
DlgImgPreview		: "Forhåndsvis",
DlgImgAlertUrl		: "Vennligst skriv bilde-urlen",
DlgImgLinkTab		: "Lenke",

// Flash Dialog
DlgFlashTitle		: "Flash-egenskaper",
DlgFlashChkPlay		: "Autospill",
DlgFlashChkLoop		: "Loop",
DlgFlashChkMenu		: "Slå på Flash-meny",
DlgFlashScale		: "Skaler",
DlgFlashScaleAll	: "Vis alt",
DlgFlashScaleNoBorder	: "Ingen ramme",
DlgFlashScaleFit	: "Skaler til å passe",

// Link Dialog
DlgLnkWindowTitle	: "Lenke",
DlgLnkInfoTab		: "Lenkeinfo",
DlgLnkTargetTab		: "Mål",

DlgLnkType			: "Lenketype",
DlgLnkTypeURL		: "URL",
DlgLnkTypeAnchor	: "Lenke til anker i teksten",
DlgLnkTypeEMail		: "E-post",
DlgLnkProto			: "Protokoll",
DlgLnkProtoOther	: "<annet>",
DlgLnkURL			: "URL",
DlgLnkAnchorSel		: "Velg et anker",
DlgLnkAnchorByName	: "Anker etter navn",
DlgLnkAnchorById	: "Element etter ID",
DlgLnkNoAnchors		: "(Ingen anker i dokumentet)",
DlgLnkEMail			: "E-postadresse",
DlgLnkEMailSubject	: "Meldingsemne",
DlgLnkEMailBody		: "Melding",
DlgLnkUpload		: "Last opp",
DlgLnkBtnUpload		: "Send til server",

DlgLnkTarget		: "Mål",
DlgLnkTargetFrame	: "<ramme>",
DlgLnkTargetPopup	: "<popup vindu>",
DlgLnkTargetBlank	: "Nytt vindu (_blank)",
DlgLnkTargetParent	: "Foreldrevindu (_parent)",
DlgLnkTargetSelf	: "Samme vindu (_self)",
DlgLnkTargetTop		: "Hele vindu (_top)",
DlgLnkTargetFrameName	: "Målramme",
DlgLnkPopWinName	: "Navn på popup-vindus",
DlgLnkPopWinFeat	: "Egenskaper for popup-vindu",
DlgLnkPopResize		: "Endre størrelse",
DlgLnkPopLocation	: "Adresselinje",
DlgLnkPopMenu		: "Menylinje",
DlgLnkPopScroll		: "Scrollbar",
DlgLnkPopStatus		: "Statuslinje",
DlgLnkPopToolbar	: "Verktøylinje",
DlgLnkPopFullScrn	: "Full skjerm (IE)",
DlgLnkPopDependent	: "Avhenging (Netscape)",
DlgLnkPopWidth		: "Bredde",
DlgLnkPopHeight		: "Høyde",
DlgLnkPopLeft		: "Venstre posisjon",
DlgLnkPopTop		: "Topp-posisjon",

DlnLnkMsgNoUrl		: "Vennligst skriv inn lenkens url",
DlnLnkMsgNoEMail	: "Vennligst skriv inn e-postadressen",
DlnLnkMsgNoAnchor	: "Vennligst velg et anker",
DlnLnkMsgInvPopName	: "Popup-vinduets navn må begynne med en bokstav, og kan ikke inneholde mellomrom",

// Color Dialog
DlgColorTitle		: "Velg farge",
DlgColorBtnClear	: "Tøm",
DlgColorHighlight	: "Marker",
DlgColorSelected	: "Valgt",

// Smiley Dialog
DlgSmileyTitle		: "Sett inn smil",

// Special Character Dialog
DlgSpecialCharTitle	: "Velg spesielt tegn",

// Table Dialog
DlgTableTitle		: "Egenskaper for tabell",
DlgTableRows		: "Rader",
DlgTableColumns		: "Kolonner",
DlgTableBorder		: "Rammestørrelse",
DlgTableAlign		: "Justering",
DlgTableAlignNotSet	: "<Ikke satt>",
DlgTableAlignLeft	: "Venstre",
DlgTableAlignCenter	: "Midtjuster",
DlgTableAlignRight	: "Høyre",
DlgTableWidth		: "Bredde",
DlgTableWidthPx		: "piksler",
DlgTableWidthPc		: "prosent",
DlgTableHeight		: "Høyde",
DlgTableCellSpace	: "Cellemarg",
DlgTableCellPad		: "Cellepolstring",
DlgTableCaption		: "Tittel",
DlgTableSummary		: "Sammendrag",
DlgTableHeaders		: "Headers",	//MISSING
DlgTableHeadersNone		: "None",	//MISSING
DlgTableHeadersColumn	: "First column",	//MISSING
DlgTableHeadersRow		: "First Row",	//MISSING
DlgTableHeadersBoth		: "Both",	//MISSING

// Table Cell Dialog
DlgCellTitle		: "Celleegenskaper",
DlgCellWidth		: "Bredde",
DlgCellWidthPx		: "piksler",
DlgCellWidthPc		: "prosent",
DlgCellHeight		: "Høyde",
DlgCellWordWrap		: "Tekstbrytning",
DlgCellWordWrapNotSet	: "<Ikke satt>",
DlgCellWordWrapYes	: "Ja",
DlgCellWordWrapNo	: "Nei",
DlgCellHorAlign		: "Horisontal justering",
DlgCellHorAlignNotSet	: "<Ikke satt>",
DlgCellHorAlignLeft	: "Venstre",
DlgCellHorAlignCenter	: "Midtjuster",
DlgCellHorAlignRight: "Høyre",
DlgCellVerAlign		: "Vertikal justering",
DlgCellVerAlignNotSet	: "<Ikke satt>",
DlgCellVerAlignTop	: "Topp",
DlgCellVerAlignMiddle	: "Midten",
DlgCellVerAlignBottom	: "Bunn",
DlgCellVerAlignBaseline	: "Bunnlinje",
DlgCellType		: "Cell Type",	//MISSING
DlgCellTypeData		: "Data",	//MISSING
DlgCellTypeHeader	: "Header",	//MISSING
DlgCellRowSpan		: "Radspenn",
DlgCellCollSpan		: "Kolonnespenn",
DlgCellBackColor	: "Bakgrunnsfarge",
DlgCellBorderColor	: "Rammefarge",
DlgCellBtnSelect	: "Velg...",

// Find and Replace Dialog
DlgFindAndReplaceTitle	: "Søk og erstatt",

// Find Dialog
DlgFindTitle		: "Søk",
DlgFindFindBtn		: "Søk",
DlgFindNotFoundMsg	: "Fant ikke søketeksten.",

// Replace Dialog
DlgReplaceTitle			: "Erstatt",
DlgReplaceFindLbl		: "Søk etter:",
DlgReplaceReplaceLbl	: "Erstatt med:",
DlgReplaceCaseChk		: "Skill mellom store og små bokstaver",
DlgReplaceReplaceBtn	: "Erstatt",
DlgReplaceReplAllBtn	: "Erstatt alle",
DlgReplaceWordChk		: "Bare hele ord",

// Paste Operations / Dialog
PasteErrorCut	: "Din nettlesers sikkerhetsinstillinger tillater ikke automatisk klipping av tekst. Vennligst bruk snareveien (Ctrl+X).",
PasteErrorCopy	: "Din nettlesers sikkerhetsinstillinger tillater ikke automatisk kopiering av tekst. Vennligst bruk snareveien (Ctrl+C).",

PasteAsText		: "Lim inn som ren tekst",
PasteFromWord	: "Lim inn fra Word",

DlgPasteMsg2	: "Vennligst lim inn i den følgende boksen med tastaturet (<STRONG>Ctrl+V</STRONG>) og trykk <STRONG>OK</STRONG>.",
DlgPasteSec		: "Din nettlesers sikkerhetsinstillinger gir ikke redigeringsverktøyet direkte tilgang til utklippstavlen. Du må lime det igjen i dette vinduet.",
DlgPasteIgnoreFont		: "Fjern skrifttyper",
DlgPasteRemoveStyles	: "Fjern stildefinisjoner",

// Color Picker
ColorAutomatic	: "Automatisk",
ColorMoreColors	: "Flere farger...",

// Document Properties
DocProps		: "Dokumentegenskaper",

// Anchor Dialog
DlgAnchorTitle		: "Ankeregenskaper",
DlgAnchorName		: "Ankernavn",
DlgAnchorErrorName	: "Vennligst skriv inn ankernavnet",

// Speller Pages Dialog
DlgSpellNotInDic		: "Ikke i ordboken",
DlgSpellChangeTo		: "Endre til",
DlgSpellBtnIgnore		: "Ignorer",
DlgSpellBtnIgnoreAll	: "Ignorer alle",
DlgSpellBtnReplace		: "Erstatt",
DlgSpellBtnReplaceAll	: "Erstatt alle",
DlgSpellBtnUndo			: "Angre",
DlgSpellNoSuggestions	: "- Ingen forslag -",
DlgSpellProgress		: "Stavekontroll pågår...",
DlgSpellNoMispell		: "Stavekontroll fullført: ingen feilstavinger funnet",
DlgSpellNoChanges		: "Stavekontroll fullført: ingen ord endret",
DlgSpellOneChange		: "Stavekontroll fullført: Ett ord endret",
DlgSpellManyChanges		: "Stavekontroll fullført: %1 ord endret",

IeSpellDownload			: "Stavekontroll er ikke installert. Vil du laste den ned nå?",

// Button Dialog
DlgButtonText		: "Tekst (verdi)",
DlgButtonType		: "Type",
DlgButtonTypeBtn	: "Knapp",
DlgButtonTypeSbm	: "Send",
DlgButtonTypeRst	: "Nullstill",

// Checkbox and Radio Button Dialogs
DlgCheckboxName		: "Navn",
DlgCheckboxValue	: "Verdi",
DlgCheckboxSelected	: "Valgt",

// Form Dialog
DlgFormName		: "Navn",
DlgFormAction	: "Handling",
DlgFormMethod	: "Metode",

// Select Field Dialog
DlgSelectName		: "Navn",
DlgSelectValue		: "Verdi",
DlgSelectSize		: "Størrelse",
DlgSelectLines		: "Linjer",
DlgSelectChkMulti	: "Tillat flervalg",
DlgSelectOpAvail	: "Tilgjenglige alternativer",
DlgSelectOpText		: "Tekst",
DlgSelectOpValue	: "Verdi",
DlgSelectBtnAdd		: "Legg til",
DlgSelectBtnModify	: "Endre",
DlgSelectBtnUp		: "Opp",
DlgSelectBtnDown	: "Ned",
DlgSelectBtnSetValue : "Sett som valgt",
DlgSelectBtnDelete	: "Slett",

// Textarea Dialog
DlgTextareaName	: "Navn",
DlgTextareaCols	: "Kolonner",
DlgTextareaRows	: "Rader",

// Text Field Dialog
DlgTextName			: "Navn",
DlgTextValue		: "Verdi",
DlgTextCharWidth	: "Tegnbredde",
DlgTextMaxChars		: "Maks antall tegn",
DlgTextType			: "Type",
DlgTextTypeText		: "Tekst",
DlgTextTypePass		: "Passord",

// Hidden Field Dialog
DlgHiddenName	: "Navn",
DlgHiddenValue	: "Verdi",

// Bulleted List Dialog
BulletedListProp	: "Egenskaper for uordnet liste",
NumberedListProp	: "Egenskaper for ordnet liste",
DlgLstStart			: "Start",
DlgLstType			: "Type",
DlgLstTypeCircle	: "Sirkel",
DlgLstTypeDisc		: "Hel sirkel",
DlgLstTypeSquare	: "Firkant",
DlgLstTypeNumbers	: "Numre (1, 2, 3)",
DlgLstTypeLCase		: "Små bokstaver (a, b, c)",
DlgLstTypeUCase		: "Store bokstaver (A, B, C)",
DlgLstTypeSRoman	: "Små romerske tall (i, ii, iii)",
DlgLstTypeLRoman	: "Store romerske tall (I, II, III)",

// Document Properties Dialog
DlgDocGeneralTab	: "Generelt",
DlgDocBackTab		: "Bakgrunn",
DlgDocColorsTab		: "Farger og marginer",
DlgDocMetaTab		: "Meta-data",

DlgDocPageTitle		: "Sidetittel",
DlgDocLangDir		: "Språkretning",
DlgDocLangDirLTR	: "Venstre til høyre (LTR)",
DlgDocLangDirRTL	: "Høyre til venstre (RTL)",
DlgDocLangCode		: "Språkkode",
DlgDocCharSet		: "Tegnsett",
DlgDocCharSetCE		: "Sentraleuropeisk",
DlgDocCharSetCT		: "Tradisonell kinesisk(Big5)",
DlgDocCharSetCR		: "Cyrillic",
DlgDocCharSetGR		: "Gresk",
DlgDocCharSetJP		: "Japansk",
DlgDocCharSetKR		: "Koreansk",
DlgDocCharSetTR		: "Tyrkisk",
DlgDocCharSetUN		: "Unicode (UTF-8)",
DlgDocCharSetWE		: "Vesteuropeisk",
DlgDocCharSetOther	: "Annet tegnsett",

DlgDocDocType		: "Dokumenttype header",
DlgDocDocTypeOther	: "Annet dokumenttype header",
DlgDocIncXHTML		: "Inkluder XHTML-deklarasjon",
DlgDocBgColor		: "Bakgrunnsfarge",
DlgDocBgImage		: "URL for bakgrunnsbilde",
DlgDocBgNoScroll	: "Lås bakgrunnsbilde",
DlgDocCText			: "Tekst",
DlgDocCLink			: "Link",
DlgDocCVisited		: "Besøkt lenke",
DlgDocCActive		: "Aktiv lenke",
DlgDocMargins		: "Sidemargin",
DlgDocMaTop			: "Topp",
DlgDocMaLeft		: "Venstre",
DlgDocMaRight		: "Høyre",
DlgDocMaBottom		: "Bunn",
DlgDocMeIndex		: "Dokument nøkkelord (kommaseparert)",
DlgDocMeDescr		: "Dokumentbeskrivelse",
DlgDocMeAuthor		: "Forfatter",
DlgDocMeCopy		: "Kopirett",
DlgDocPreview		: "Forhåndsvising",

// Templates Dialog
Templates			: "Maler",
DlgTemplatesTitle	: "Innholdsmaler",
DlgTemplatesSelMsg	: "Velg malen du vil åpne<br>(innholdet du har skrevet blir tapt!):",
DlgTemplatesLoading	: "Laster malliste. Vennligst vent...",
DlgTemplatesNoTpl	: "(Ingen maler definert)",
DlgTemplatesReplace	: "Erstatt faktisk innold",

// About Dialog
DlgAboutAboutTab	: "Om",
DlgAboutBrowserInfoTab	: "Nettleserinfo",
DlgAboutLicenseTab	: "Lisens",
DlgAboutVersion		: "versjon",
DlgAboutInfo		: "For mer informasjon, se",

// Div Dialog
DlgDivGeneralTab	: "Generelt",
DlgDivAdvancedTab	: "Avansert",
DlgDivStyle		: "Stil",
DlgDivInlineStyle	: "Inline Style",	//MISSING

ScaytTitle			: "SCAYT",	//MISSING
ScaytTitleOptions	: "Options",	//MISSING
ScaytTitleLangs		: "Languages",	//MISSING
ScaytTitleAbout		: "About"	//MISSING
};
