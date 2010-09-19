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
 * Croatian language file.
 */

var FCKLang =
{
// Language direction : "ltr" (left to right) or "rtl" (right to left).
Dir					: "ltr",

ToolbarCollapse		: "Smanji trake s alatima",
ToolbarExpand		: "Proširi trake s alatima",

// Toolbar Items and Context Menu
Save				: "Snimi",
NewPage				: "Nova stranica",
Preview				: "Pregledaj",
Cut					: "Izreži",
Copy				: "Kopiraj",
Paste				: "Zalijepi",
PasteText			: "Zalijepi kao čisti tekst",
PasteWord			: "Zalijepi iz Worda",
Print				: "Ispiši",
SelectAll			: "Odaberi sve",
RemoveFormat		: "Ukloni formatiranje",
InsertLinkLbl		: "Link",
InsertLink			: "Ubaci/promijeni link",
RemoveLink			: "Ukloni link",
VisitLink			: "Otvori link",
Anchor				: "Ubaci/promijeni sidro",
AnchorDelete		: "Ukloni sidro",
InsertImageLbl		: "Slika",
InsertImage			: "Ubaci/promijeni sliku",
InsertFlashLbl		: "Flash",
InsertFlash			: "Ubaci/promijeni Flash",
InsertTableLbl		: "Tablica",
InsertTable			: "Ubaci/promijeni tablicu",
InsertLineLbl		: "Linija",
InsertLine			: "Ubaci vodoravnu liniju",
InsertSpecialCharLbl: "Posebni karakteri",
InsertSpecialChar	: "Ubaci posebne znakove",
InsertSmileyLbl		: "Smješko",
InsertSmiley		: "Ubaci smješka",
About				: "O FCKeditoru",
Bold				: "Podebljaj",
Italic				: "Ukosi",
Underline			: "Potcrtano",
StrikeThrough		: "Precrtano",
Subscript			: "Subscript",
Superscript			: "Superscript",
LeftJustify			: "Lijevo poravnanje",
CenterJustify		: "Središnje poravnanje",
RightJustify		: "Desno poravnanje",
BlockJustify		: "Blok poravnanje",
DecreaseIndent		: "Pomakni ulijevo",
IncreaseIndent		: "Pomakni udesno",
Blockquote			: "Blockquote",
CreateDiv			: "Napravi Div kontejner",
EditDiv				: "Uredi Div kontejner",
DeleteDiv			: "Ukloni Div kontejner",
Undo				: "Poništi",
Redo				: "Ponovi",
NumberedListLbl		: "Brojčana lista",
NumberedList		: "Ubaci/ukloni brojčanu listu",
BulletedListLbl		: "Obična lista",
BulletedList		: "Ubaci/ukloni običnu listu",
ShowTableBorders	: "Prikaži okvir tablice",
ShowDetails			: "Prikaži detalje",
Style				: "Stil",
FontFormat			: "Format",
Font				: "Font",
FontSize			: "Veličina",
TextColor			: "Boja teksta",
BGColor				: "Boja pozadine",
Source				: "Kôd",
Find				: "Pronađi",
Replace				: "Zamijeni",
SpellCheck			: "Provjeri pravopis",
UniversalKeyboard	: "Univerzalna tipkovnica",
PageBreakLbl		: "Prijelom stranice",
PageBreak			: "Ubaci prijelom stranice",

Form			: "Form",
Checkbox		: "Checkbox",
RadioButton		: "Radio Button",
TextField		: "Text Field",
Textarea		: "Textarea",
HiddenField		: "Hidden Field",
Button			: "Button",
SelectionField	: "Selection Field",
ImageButton		: "Image Button",

FitWindow		: "Povećaj veličinu editora",
ShowBlocks		: "Prikaži blokove",

// Context Menu
EditLink			: "Promijeni link",
CellCM				: "Ćelija",
RowCM				: "Red",
ColumnCM			: "Kolona",
InsertRowAfter		: "Ubaci red poslije",
InsertRowBefore		: "Ubaci red prije",
DeleteRows			: "Izbriši redove",
InsertColumnAfter	: "Ubaci kolonu poslije",
InsertColumnBefore	: "Ubaci kolonu prije",
DeleteColumns		: "Izbriši kolone",
InsertCellAfter		: "Ubaci ćeliju poslije",
InsertCellBefore	: "Ubaci ćeliju prije",
DeleteCells			: "Izbriši ćelije",
MergeCells			: "Spoji ćelije",
MergeRight			: "Spoji desno",
MergeDown			: "Spoji dolje",
HorizontalSplitCell	: "Podijeli ćeliju vodoravno",
VerticalSplitCell	: "Podijeli ćeliju okomito",
TableDelete			: "Izbriši tablicu",
CellProperties		: "Svojstva ćelije",
TableProperties		: "Svojstva tablice",
ImageProperties		: "Svojstva slike",
FlashProperties		: "Flash svojstva",

AnchorProp			: "Svojstva sidra",
ButtonProp			: "Image Button svojstva",
CheckboxProp		: "Checkbox svojstva",
HiddenFieldProp		: "Hidden Field svojstva",
RadioButtonProp		: "Radio Button svojstva",
ImageButtonProp		: "Image Button svojstva",
TextFieldProp		: "Text Field svojstva",
SelectionFieldProp	: "Selection svojstva",
TextareaProp		: "Textarea svojstva",
FormProp			: "Form svojstva",

FontFormats			: "Normal;Formatted;Address;Heading 1;Heading 2;Heading 3;Heading 4;Heading 5;Heading 6;Normal (DIV)",

// Alerts and Messages
ProcessingXHTML		: "Obrađujem XHTML. Molimo pričekajte...",
Done				: "Završio",
PasteWordConfirm	: "Tekst koji želite zalijepiti čini se da je kopiran iz Worda. Želite li prije očistiti tekst?",
NotCompatiblePaste	: "Ova naredba je dostupna samo u Internet Exploreru 5.5 ili novijem. Želite li nastaviti bez čišćenja?",
UnknownToolbarItem	: "Nepoznati član trake s alatima \"%1\"",
UnknownCommand		: "Nepoznata naredba \"%1\"",
NotImplemented		: "Naredba nije implementirana",
UnknownToolbarSet	: "Traka s alatima \"%1\" ne postoji",
NoActiveX			: "Vaše postavke pretraživača mogle bi ograničiti neke od mogućnosti editora. Morate uključiti opciju \"Run ActiveX controls and plug-ins\" u postavkama. Ukoliko to ne učinite, moguće su razliite greške tijekom rada.",
BrowseServerBlocked : "Pretraivač nije moguće otvoriti. Provjerite da li je uključeno blokiranje pop-up prozora.",
DialogBlocked		: "Nije moguće otvoriti novi prozor. Provjerite da li je uključeno blokiranje pop-up prozora.",
VisitLinkBlocked	: "Nije moguće otvoriti novi prozor. Provjerite da li je uključeno blokiranje pop-up prozora.",

// Dialogs
DlgBtnOK			: "OK",
DlgBtnCancel		: "Poništi",
DlgBtnClose			: "Zatvori",
DlgBtnBrowseServer	: "Pretraži server",
DlgAdvancedTag		: "Napredno",
DlgOpOther			: "<Drugo>",
DlgInfoTab			: "Info",
DlgAlertUrl			: "Molimo unesite URL",

// General Dialogs Labels
DlgGenNotSet		: "<nije postavljeno>",
DlgGenId			: "Id",
DlgGenLangDir		: "Smjer jezika",
DlgGenLangDirLtr	: "S lijeva na desno (LTR)",
DlgGenLangDirRtl	: "S desna na lijevo (RTL)",
DlgGenLangCode		: "Kôd jezika",
DlgGenAccessKey		: "Pristupna tipka",
DlgGenName			: "Naziv",
DlgGenTabIndex		: "Tab Indeks",
DlgGenLongDescr		: "Dugački opis URL",
DlgGenClass			: "Stylesheet klase",
DlgGenTitle			: "Advisory naslov",
DlgGenContType		: "Advisory vrsta sadržaja",
DlgGenLinkCharset	: "Kodna stranica povezanih resursa",
DlgGenStyle			: "Stil",

// Image Dialog
DlgImgTitle			: "Svojstva slika",
DlgImgInfoTab		: "Info slike",
DlgImgBtnUpload		: "Pošalji na server",
DlgImgURL			: "URL",
DlgImgUpload		: "Pošalji",
DlgImgAlt			: "Alternativni tekst",
DlgImgWidth			: "Širina",
DlgImgHeight		: "Visina",
DlgImgLockRatio		: "Zaključaj odnos",
DlgBtnResetSize		: "Obriši veličinu",
DlgImgBorder		: "Okvir",
DlgImgHSpace		: "HSpace",
DlgImgVSpace		: "VSpace",
DlgImgAlign			: "Poravnaj",
DlgImgAlignLeft		: "Lijevo",
DlgImgAlignAbsBottom: "Abs dolje",
DlgImgAlignAbsMiddle: "Abs sredina",
DlgImgAlignBaseline	: "Bazno",
DlgImgAlignBottom	: "Dolje",
DlgImgAlignMiddle	: "Sredina",
DlgImgAlignRight	: "Desno",
DlgImgAlignTextTop	: "Vrh teksta",
DlgImgAlignTop		: "Vrh",
DlgImgPreview		: "Pregledaj",
DlgImgAlertUrl		: "Unesite URL slike",
DlgImgLinkTab		: "Link",

// Flash Dialog
DlgFlashTitle		: "Flash svojstva",
DlgFlashChkPlay		: "Auto Play",
DlgFlashChkLoop		: "Ponavljaj",
DlgFlashChkMenu		: "Omogući Flash izbornik",
DlgFlashScale		: "Omjer",
DlgFlashScaleAll	: "Prikaži sve",
DlgFlashScaleNoBorder	: "Bez okvira",
DlgFlashScaleFit	: "Točna veličina",

// Link Dialog
DlgLnkWindowTitle	: "Link",
DlgLnkInfoTab		: "Link Info",
DlgLnkTargetTab		: "Meta",

DlgLnkType			: "Link vrsta",
DlgLnkTypeURL		: "URL",
DlgLnkTypeAnchor	: "Sidro na ovoj stranici",
DlgLnkTypeEMail		: "E-Mail",
DlgLnkProto			: "Protokol",
DlgLnkProtoOther	: "<drugo>",
DlgLnkURL			: "URL",
DlgLnkAnchorSel		: "Odaberi sidro",
DlgLnkAnchorByName	: "Po nazivu sidra",
DlgLnkAnchorById	: "Po Id elementa",
DlgLnkNoAnchors		: "(Nema dostupnih sidra)",
DlgLnkEMail			: "E-Mail adresa",
DlgLnkEMailSubject	: "Naslov",
DlgLnkEMailBody		: "Sadržaj poruke",
DlgLnkUpload		: "Pošalji",
DlgLnkBtnUpload		: "Pošalji na server",

DlgLnkTarget		: "Meta",
DlgLnkTargetFrame	: "<okvir>",
DlgLnkTargetPopup	: "<popup prozor>",
DlgLnkTargetBlank	: "Novi prozor (_blank)",
DlgLnkTargetParent	: "Roditeljski prozor (_parent)",
DlgLnkTargetSelf	: "Isti prozor (_self)",
DlgLnkTargetTop		: "Vršni prozor (_top)",
DlgLnkTargetFrameName	: "Ime ciljnog okvira",
DlgLnkPopWinName	: "Naziv popup prozora",
DlgLnkPopWinFeat	: "Mogućnosti popup prozora",
DlgLnkPopResize		: "Promjenljive veličine",
DlgLnkPopLocation	: "Traka za lokaciju",
DlgLnkPopMenu		: "Izborna traka",
DlgLnkPopScroll		: "Scroll traka",
DlgLnkPopStatus		: "Statusna traka",
DlgLnkPopToolbar	: "Traka s alatima",
DlgLnkPopFullScrn	: "Cijeli ekran (IE)",
DlgLnkPopDependent	: "Ovisno (Netscape)",
DlgLnkPopWidth		: "Širina",
DlgLnkPopHeight		: "Visina",
DlgLnkPopLeft		: "Lijeva pozicija",
DlgLnkPopTop		: "Gornja pozicija",

DlnLnkMsgNoUrl		: "Molimo upišite URL link",
DlnLnkMsgNoEMail	: "Molimo upišite e-mail adresu",
DlnLnkMsgNoAnchor	: "Molimo odaberite sidro",
DlnLnkMsgInvPopName	: "Ime popup prozora mora početi sa slovom i ne smije sadržavati razmake",

// Color Dialog
DlgColorTitle		: "Odaberite boju",
DlgColorBtnClear	: "Obriši",
DlgColorHighlight	: "Osvijetli",
DlgColorSelected	: "Odaberi",

// Smiley Dialog
DlgSmileyTitle		: "Ubaci smješka",

// Special Character Dialog
DlgSpecialCharTitle	: "Odaberite posebni karakter",

// Table Dialog
DlgTableTitle		: "Svojstva tablice",
DlgTableRows		: "Redova",
DlgTableColumns		: "Kolona",
DlgTableBorder		: "Veličina okvira",
DlgTableAlign		: "Poravnanje",
DlgTableAlignNotSet	: "<nije postavljeno>",
DlgTableAlignLeft	: "Lijevo",
DlgTableAlignCenter	: "Središnje",
DlgTableAlignRight	: "Desno",
DlgTableWidth		: "Širina",
DlgTableWidthPx		: "piksela",
DlgTableWidthPc		: "postotaka",
DlgTableHeight		: "Visina",
DlgTableCellSpace	: "Prostornost ćelija",
DlgTableCellPad		: "Razmak ćelija",
DlgTableCaption		: "Naslov",
DlgTableSummary		: "Sažetak",
DlgTableHeaders		: "Headers",	//MISSING
DlgTableHeadersNone		: "None",	//MISSING
DlgTableHeadersColumn	: "First column",	//MISSING
DlgTableHeadersRow		: "First Row",	//MISSING
DlgTableHeadersBoth		: "Both",	//MISSING

// Table Cell Dialog
DlgCellTitle		: "Svojstva ćelije",
DlgCellWidth		: "Širina",
DlgCellWidthPx		: "piksela",
DlgCellWidthPc		: "postotaka",
DlgCellHeight		: "Visina",
DlgCellWordWrap		: "Word Wrap",
DlgCellWordWrapNotSet	: "<nije postavljeno>",
DlgCellWordWrapYes	: "Da",
DlgCellWordWrapNo	: "Ne",
DlgCellHorAlign		: "Vodoravno poravnanje",
DlgCellHorAlignNotSet	: "<nije postavljeno>",
DlgCellHorAlignLeft	: "Lijevo",
DlgCellHorAlignCenter	: "Središnje",
DlgCellHorAlignRight: "Desno",
DlgCellVerAlign		: "Okomito poravnanje",
DlgCellVerAlignNotSet	: "<nije postavljeno>",
DlgCellVerAlignTop	: "Gornje",
DlgCellVerAlignMiddle	: "Srednišnje",
DlgCellVerAlignBottom	: "Donje",
DlgCellVerAlignBaseline	: "Bazno",
DlgCellType		: "Cell Type",	//MISSING
DlgCellTypeData		: "Data",	//MISSING
DlgCellTypeHeader	: "Header",	//MISSING
DlgCellRowSpan		: "Spajanje redova",
DlgCellCollSpan		: "Spajanje kolona",
DlgCellBackColor	: "Boja pozadine",
DlgCellBorderColor	: "Boja okvira",
DlgCellBtnSelect	: "Odaberi...",

// Find and Replace Dialog
DlgFindAndReplaceTitle	: "Pronađi i zamijeni",

// Find Dialog
DlgFindTitle		: "Pronađi",
DlgFindFindBtn		: "Pronađi",
DlgFindNotFoundMsg	: "Traženi tekst nije pronađen.",

// Replace Dialog
DlgReplaceTitle			: "Zamijeni",
DlgReplaceFindLbl		: "Pronađi:",
DlgReplaceReplaceLbl	: "Zamijeni s:",
DlgReplaceCaseChk		: "Usporedi mala/velika slova",
DlgReplaceReplaceBtn	: "Zamijeni",
DlgReplaceReplAllBtn	: "Zamijeni sve",
DlgReplaceWordChk		: "Usporedi cijele riječi",

// Paste Operations / Dialog
PasteErrorCut	: "Sigurnosne postavke Vašeg pretraživača ne dozvoljavaju operacije automatskog izrezivanja. Molimo koristite kraticu na tipkovnici (Ctrl+X).",
PasteErrorCopy	: "Sigurnosne postavke Vašeg pretraživača ne dozvoljavaju operacije automatskog kopiranja. Molimo koristite kraticu na tipkovnici (Ctrl+C).",

PasteAsText		: "Zalijepi kao čisti tekst",
PasteFromWord	: "Zalijepi iz Worda",

DlgPasteMsg2	: "Molimo zaljepite unutar doljnjeg okvira koristeći tipkovnicu (<STRONG>Ctrl+V</STRONG>) i kliknite <STRONG>OK</STRONG>.",
DlgPasteSec		: "Zbog sigurnosnih postavki Vašeg pretraživača, editor nema direktan pristup Vašem međuspremniku. Potrebno je ponovno zalijepiti tekst u ovaj prozor.",
DlgPasteIgnoreFont		: "Zanemari definiciju vrste fonta",
DlgPasteRemoveStyles	: "Ukloni definicije stilova",

// Color Picker
ColorAutomatic	: "Automatski",
ColorMoreColors	: "Više boja...",

// Document Properties
DocProps		: "Svojstva dokumenta",

// Anchor Dialog
DlgAnchorTitle		: "Svojstva sidra",
DlgAnchorName		: "Ime sidra",
DlgAnchorErrorName	: "Molimo unesite ime sidra",

// Speller Pages Dialog
DlgSpellNotInDic		: "Nije u rječniku",
DlgSpellChangeTo		: "Promijeni u",
DlgSpellBtnIgnore		: "Zanemari",
DlgSpellBtnIgnoreAll	: "Zanemari sve",
DlgSpellBtnReplace		: "Zamijeni",
DlgSpellBtnReplaceAll	: "Zamijeni sve",
DlgSpellBtnUndo			: "Vrati",
DlgSpellNoSuggestions	: "-Nema preporuke-",
DlgSpellProgress		: "Provjera u tijeku...",
DlgSpellNoMispell		: "Provjera završena: Nema grešaka",
DlgSpellNoChanges		: "Provjera završena: Nije napravljena promjena",
DlgSpellOneChange		: "Provjera završena: Jedna riječ promjenjena",
DlgSpellManyChanges		: "Provjera završena: Promijenjeno %1 riječi",

IeSpellDownload			: "Provjera pravopisa nije instalirana. Želite li skinuti provjeru pravopisa?",

// Button Dialog
DlgButtonText		: "Tekst (vrijednost)",
DlgButtonType		: "Vrsta",
DlgButtonTypeBtn	: "Gumb",
DlgButtonTypeSbm	: "Pošalji",
DlgButtonTypeRst	: "Poništi",

// Checkbox and Radio Button Dialogs
DlgCheckboxName		: "Ime",
DlgCheckboxValue	: "Vrijednost",
DlgCheckboxSelected	: "Odabrano",

// Form Dialog
DlgFormName		: "Ime",
DlgFormAction	: "Akcija",
DlgFormMethod	: "Metoda",

// Select Field Dialog
DlgSelectName		: "Ime",
DlgSelectValue		: "Vrijednost",
DlgSelectSize		: "Veličina",
DlgSelectLines		: "linija",
DlgSelectChkMulti	: "Dozvoli višestruki odabir",
DlgSelectOpAvail	: "Dostupne opcije",
DlgSelectOpText		: "Tekst",
DlgSelectOpValue	: "Vrijednost",
DlgSelectBtnAdd		: "Dodaj",
DlgSelectBtnModify	: "Promijeni",
DlgSelectBtnUp		: "Gore",
DlgSelectBtnDown	: "Dolje",
DlgSelectBtnSetValue : "Postavi kao odabranu vrijednost",
DlgSelectBtnDelete	: "Obriši",

// Textarea Dialog
DlgTextareaName	: "Ime",
DlgTextareaCols	: "Kolona",
DlgTextareaRows	: "Redova",

// Text Field Dialog
DlgTextName			: "Ime",
DlgTextValue		: "Vrijednost",
DlgTextCharWidth	: "Širina",
DlgTextMaxChars		: "Najviše karaktera",
DlgTextType			: "Vrsta",
DlgTextTypeText		: "Tekst",
DlgTextTypePass		: "Šifra",

// Hidden Field Dialog
DlgHiddenName	: "Ime",
DlgHiddenValue	: "Vrijednost",

// Bulleted List Dialog
BulletedListProp	: "Svojstva liste",
NumberedListProp	: "Svojstva brojčane liste",
DlgLstStart			: "Početak",
DlgLstType			: "Vrsta",
DlgLstTypeCircle	: "Krug",
DlgLstTypeDisc		: "Disk",
DlgLstTypeSquare	: "Kvadrat",
DlgLstTypeNumbers	: "Brojevi (1, 2, 3)",
DlgLstTypeLCase		: "Mala slova (a, b, c)",
DlgLstTypeUCase		: "Velika slova (A, B, C)",
DlgLstTypeSRoman	: "Male rimske brojke (i, ii, iii)",
DlgLstTypeLRoman	: "Velike rimske brojke (I, II, III)",

// Document Properties Dialog
DlgDocGeneralTab	: "Općenito",
DlgDocBackTab		: "Pozadina",
DlgDocColorsTab		: "Boje i margine",
DlgDocMetaTab		: "Meta Data",

DlgDocPageTitle		: "Naslov stranice",
DlgDocLangDir		: "Smjer jezika",
DlgDocLangDirLTR	: "S lijeva na desno",
DlgDocLangDirRTL	: "S desna na lijevo",
DlgDocLangCode		: "Kôd jezika",
DlgDocCharSet		: "Enkodiranje znakova",
DlgDocCharSetCE		: "Središnja Europa",
DlgDocCharSetCT		: "Tradicionalna kineska (Big5)",
DlgDocCharSetCR		: "Ćirilica",
DlgDocCharSetGR		: "Grčka",
DlgDocCharSetJP		: "Japanska",
DlgDocCharSetKR		: "Koreanska",
DlgDocCharSetTR		: "Turska",
DlgDocCharSetUN		: "Unicode (UTF-8)",
DlgDocCharSetWE		: "Zapadna Europa",
DlgDocCharSetOther	: "Ostalo enkodiranje znakova",

DlgDocDocType		: "Zaglavlje vrste dokumenta",
DlgDocDocTypeOther	: "Ostalo zaglavlje vrste dokumenta",
DlgDocIncXHTML		: "Ubaci XHTML deklaracije",
DlgDocBgColor		: "Boja pozadine",
DlgDocBgImage		: "URL slike pozadine",
DlgDocBgNoScroll	: "Pozadine se ne pomiče",
DlgDocCText			: "Tekst",
DlgDocCLink			: "Link",
DlgDocCVisited		: "Posjećeni link",
DlgDocCActive		: "Aktivni link",
DlgDocMargins		: "Margine stranice",
DlgDocMaTop			: "Vrh",
DlgDocMaLeft		: "Lijevo",
DlgDocMaRight		: "Desno",
DlgDocMaBottom		: "Dolje",
DlgDocMeIndex		: "Ključne riječi dokumenta (odvojene zarezom)",
DlgDocMeDescr		: "Opis dokumenta",
DlgDocMeAuthor		: "Autor",
DlgDocMeCopy		: "Autorska prava",
DlgDocPreview		: "Pregledaj",

// Templates Dialog
Templates			: "Predlošci",
DlgTemplatesTitle	: "Predlošci sadržaja",
DlgTemplatesSelMsg	: "Molimo odaberite predložak koji želite otvoriti<br>(stvarni sadržaj će biti izgubljen):",
DlgTemplatesLoading	: "Učitavam listu predložaka. Molimo pričekajte...",
DlgTemplatesNoTpl	: "(Nema definiranih predložaka)",
DlgTemplatesReplace	: "Zamijeni trenutne sadržaje",

// About Dialog
DlgAboutAboutTab	: "O FCKEditoru",
DlgAboutBrowserInfoTab	: "Podaci o pretraživaču",
DlgAboutLicenseTab	: "Licenca",
DlgAboutVersion		: "inačica",
DlgAboutInfo		: "Za više informacija posjetite",

// Div Dialog
DlgDivGeneralTab	: "Općenito",
DlgDivAdvancedTab	: "Napredno",
DlgDivStyle		: "Stil",
DlgDivInlineStyle	: "Stil u redu",

ScaytTitle			: "SCAYT",	//MISSING
ScaytTitleOptions	: "Options",	//MISSING
ScaytTitleLangs		: "Languages",	//MISSING
ScaytTitleAbout		: "About"	//MISSING
};
