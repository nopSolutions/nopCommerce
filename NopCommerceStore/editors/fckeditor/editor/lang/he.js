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
 * Hebrew language file.
 */

var FCKLang =
{
// Language direction : "ltr" (left to right) or "rtl" (right to left).
Dir					: "rtl",

ToolbarCollapse		: "כיווץ סרגל הכלים",
ToolbarExpand		: "פתיחת סרגל הכלים",

// Toolbar Items and Context Menu
Save				: "שמירה",
NewPage				: "דף חדש",
Preview				: "תצוגה מקדימה",
Cut					: "גזירה",
Copy				: "העתקה",
Paste				: "הדבקה",
PasteText			: "הדבקה כטקסט פשוט",
PasteWord			: "הדבקה מ-וורד",
Print				: "הדפסה",
SelectAll			: "בחירת הכל",
RemoveFormat		: "הסרת העיצוב",
InsertLinkLbl		: "קישור",
InsertLink			: "הוספת/עריכת קישור",
RemoveLink			: "הסרת הקישור",
VisitLink			: "פתח קישור",
Anchor				: "הוספת/עריכת נקודת עיגון",
AnchorDelete		: "הסר נקודת עיגון",
InsertImageLbl		: "תמונה",
InsertImage			: "הוספת/עריכת תמונה",
InsertFlashLbl		: "פלאש",
InsertFlash			: "הוסף/ערוך פלאש",
InsertTableLbl		: "טבלה",
InsertTable			: "הוספת/עריכת טבלה",
InsertLineLbl		: "קו",
InsertLine			: "הוספת קו אופקי",
InsertSpecialCharLbl: "תו מיוחד",
InsertSpecialChar	: "הוספת תו מיוחד",
InsertSmileyLbl		: "סמיילי",
InsertSmiley		: "הוספת סמיילי",
About				: "אודות FCKeditor",
Bold				: "מודגש",
Italic				: "נטוי",
Underline			: "קו תחתון",
StrikeThrough		: "כתיב מחוק",
Subscript			: "כתיב תחתון",
Superscript			: "כתיב עליון",
LeftJustify			: "יישור לשמאל",
CenterJustify		: "מרכוז",
RightJustify		: "יישור לימין",
BlockJustify		: "יישור לשוליים",
DecreaseIndent		: "הקטנת אינדנטציה",
IncreaseIndent		: "הגדלת אינדנטציה",
Blockquote			: "בלוק ציטוט",
CreateDiv			: "צור מיכל(תג)DIV",
EditDiv				: "ערוך מיכל (תג)DIV",
DeleteDiv			: "הסר מיכל(תג) DIV",
Undo				: "ביטול צעד אחרון",
Redo				: "חזרה על צעד אחרון",
NumberedListLbl		: "רשימה ממוספרת",
NumberedList		: "הוספת/הסרת רשימה ממוספרת",
BulletedListLbl		: "רשימת נקודות",
BulletedList		: "הוספת/הסרת רשימת נקודות",
ShowTableBorders	: "הצגת מסגרת הטבלה",
ShowDetails			: "הצגת פרטים",
Style				: "סגנון",
FontFormat			: "עיצוב",
Font				: "גופן",
FontSize			: "גודל",
TextColor			: "צבע טקסט",
BGColor				: "צבע רקע",
Source				: "מקור",
Find				: "חיפוש",
Replace				: "החלפה",
SpellCheck			: "בדיקת איות",
UniversalKeyboard	: "מקלדת אוניברסלית",
PageBreakLbl		: "שבירת דף",
PageBreak			: "הוסף שבירת דף",

Form			: "טופס",
Checkbox		: "תיבת סימון",
RadioButton		: "לחצן אפשרויות",
TextField		: "שדה טקסט",
Textarea		: "איזור טקסט",
HiddenField		: "שדה חבוי",
Button			: "כפתור",
SelectionField	: "שדה בחירה",
ImageButton		: "כפתור תמונה",

FitWindow		: "הגדל את גודל העורך",
ShowBlocks		: "הצג בלוקים",

// Context Menu
EditLink			: "עריכת קישור",
CellCM				: "תא",
RowCM				: "שורה",
ColumnCM			: "עמודה",
InsertRowAfter		: "הוסף שורה אחרי",
InsertRowBefore		: "הוסף שורה לפני",
DeleteRows			: "מחיקת שורות",
InsertColumnAfter	: "הוסף עמודה אחרי",
InsertColumnBefore	: "הוסף עמודה לפני",
DeleteColumns		: "מחיקת עמודות",
InsertCellAfter		: "הוסף תא אחרי",
InsertCellBefore	: "הוסף תא אחרי",
DeleteCells			: "מחיקת תאים",
MergeCells			: "מיזוג תאים",
MergeRight			: "מזג ימינה",
MergeDown			: "מזג למטה",
HorizontalSplitCell	: "פצל תא אופקית",
VerticalSplitCell	: "פצל תא אנכית",
TableDelete			: "מחק טבלה",
CellProperties		: "תכונות התא",
TableProperties		: "תכונות הטבלה",
ImageProperties		: "תכונות התמונה",
FlashProperties		: "מאפייני פלאש",

AnchorProp			: "מאפייני נקודת עיגון",
ButtonProp			: "מאפייני כפתור",
CheckboxProp		: "מאפייני תיבת סימון",
HiddenFieldProp		: "מאפיני שדה חבוי",
RadioButtonProp		: "מאפייני לחצן אפשרויות",
ImageButtonProp		: "מאפיני כפתור תמונה",
TextFieldProp		: "מאפייני שדה טקסט",
SelectionFieldProp	: "מאפייני שדה בחירה",
TextareaProp		: "מאפיני איזור טקסט",
FormProp			: "מאפיני טופס",

FontFormats			: "נורמלי;קוד;כתובת;כותרת;כותרת 2;כותרת 3;כותרת 4;כותרת 5;כותרת 6",

// Alerts and Messages
ProcessingXHTML		: "מעבד XHTML, נא להמתין...",
Done				: "המשימה הושלמה",
PasteWordConfirm	: "נראה הטקסט שבכוונתך להדביק מקורו בקובץ וורד. האם ברצונך לנקות אותו טרם ההדבקה?",
NotCompatiblePaste	: "פעולה זו זמינה לדפדפן אינטרנט אקספלורר מגירסא 5.5 ומעלה. האם להמשיך בהדבקה ללא הניקוי?",
UnknownToolbarItem	: "פריט לא ידוע בסרגל הכלים \"%1\"",
UnknownCommand		: "שם פעולה לא ידוע \"%1\"",
NotImplemented		: "הפקודה לא מיושמת",
UnknownToolbarSet	: "ערכת סרגל הכלים \"%1\" לא קיימת",
NoActiveX			: "הגדרות אבטחה של הדפדפן עלולות לגביל את אפשרויות העריכה.יש לאפשר את האופציה \"הרץ פקדים פעילים ותוספות\". תוכל לחוות טעויות וחיווים של אפשרויות שחסרים.",
BrowseServerBlocked : "לא ניתן לגשת לדפדפן משאבים.אנא וודא שחוסם חלונות הקופצים לא פעיל.",
DialogBlocked		: "לא היה ניתן לפתוח חלון דיאלוג. אנא וודא שחוסם חלונות קופצים לא פעיל.",
VisitLinkBlocked	: "לא ניתן לפתוח חלון חדש.נא לוודא שחוסמי החלונות הקופצים לא פעילים.",

// Dialogs
DlgBtnOK			: "אישור",
DlgBtnCancel		: "ביטול",
DlgBtnClose			: "סגירה",
DlgBtnBrowseServer	: "סייר השרת",
DlgAdvancedTag		: "אפשרויות מתקדמות",
DlgOpOther			: "<אחר>",
DlgInfoTab			: "מידע",
DlgAlertUrl			: "אנא הזן URL",

// General Dialogs Labels
DlgGenNotSet		: "<לא נקבע>",
DlgGenId			: "זיהוי (Id)",
DlgGenLangDir		: "כיוון שפה",
DlgGenLangDirLtr	: "שמאל לימין (LTR)",
DlgGenLangDirRtl	: "ימין לשמאל (RTL)",
DlgGenLangCode		: "קוד שפה",
DlgGenAccessKey		: "מקש גישה",
DlgGenName			: "שם",
DlgGenTabIndex		: "מספר טאב",
DlgGenLongDescr		: "קישור לתיאור מפורט",
DlgGenClass			: "גיליונות עיצוב קבוצות",
DlgGenTitle			: "כותרת מוצעת",
DlgGenContType		: "Content Type מוצע",
DlgGenLinkCharset	: "קידוד המשאב המקושר",
DlgGenStyle			: "סגנון",

// Image Dialog
DlgImgTitle			: "תכונות התמונה",
DlgImgInfoTab		: "מידע על התמונה",
DlgImgBtnUpload		: "שליחה לשרת",
DlgImgURL			: "כתובת (URL)",
DlgImgUpload		: "העלאה",
DlgImgAlt			: "טקסט חלופי",
DlgImgWidth			: "רוחב",
DlgImgHeight		: "גובה",
DlgImgLockRatio		: "נעילת היחס",
DlgBtnResetSize		: "איפוס הגודל",
DlgImgBorder		: "מסגרת",
DlgImgHSpace		: "מרווח אופקי",
DlgImgVSpace		: "מרווח אנכי",
DlgImgAlign			: "יישור",
DlgImgAlignLeft		: "לשמאל",
DlgImgAlignAbsBottom: "לתחתית האבסולוטית",
DlgImgAlignAbsMiddle: "מרכוז אבסולוטי",
DlgImgAlignBaseline	: "לקו התחתית",
DlgImgAlignBottom	: "לתחתית",
DlgImgAlignMiddle	: "לאמצע",
DlgImgAlignRight	: "לימין",
DlgImgAlignTextTop	: "לראש הטקסט",
DlgImgAlignTop		: "למעלה",
DlgImgPreview		: "תצוגה מקדימה",
DlgImgAlertUrl		: "נא להקליד את כתובת התמונה",
DlgImgLinkTab		: "קישור",

// Flash Dialog
DlgFlashTitle		: "מאפיני פלאש",
DlgFlashChkPlay		: "נגן אוטומטי",
DlgFlashChkLoop		: "לולאה",
DlgFlashChkMenu		: "אפשר תפריט פלאש",
DlgFlashScale		: "גודל",
DlgFlashScaleAll	: "הצג הכל",
DlgFlashScaleNoBorder	: "ללא גבולות",
DlgFlashScaleFit	: "התאמה מושלמת",

// Link Dialog
DlgLnkWindowTitle	: "קישור",
DlgLnkInfoTab		: "מידע על הקישור",
DlgLnkTargetTab		: "מטרה",

DlgLnkType			: "סוג קישור",
DlgLnkTypeURL		: "כתובת (URL)",
DlgLnkTypeAnchor	: "עוגן בעמוד זה",
DlgLnkTypeEMail		: "דוא''ל",
DlgLnkProto			: "פרוטוקול",
DlgLnkProtoOther	: "<אחר>",
DlgLnkURL			: "כתובת (URL)",
DlgLnkAnchorSel		: "בחירת עוגן",
DlgLnkAnchorByName	: "עפ''י שם העוגן",
DlgLnkAnchorById	: "עפ''י זיהוי (Id) הרכיב",
DlgLnkNoAnchors		: "(אין עוגנים זמינים בדף)",
DlgLnkEMail			: "כתובת הדוא''ל",
DlgLnkEMailSubject	: "נושא ההודעה",
DlgLnkEMailBody		: "גוף ההודעה",
DlgLnkUpload		: "העלאה",
DlgLnkBtnUpload		: "שליחה לשרת",

DlgLnkTarget		: "מטרה",
DlgLnkTargetFrame	: "<מסגרת>",
DlgLnkTargetPopup	: "<חלון קופץ>",
DlgLnkTargetBlank	: "חלון חדש (_blank)",
DlgLnkTargetParent	: "חלון האב (_parent)",
DlgLnkTargetSelf	: "באותו החלון (_self)",
DlgLnkTargetTop		: "חלון ראשי (_top)",
DlgLnkTargetFrameName	: "שם מסגרת היעד",
DlgLnkPopWinName	: "שם החלון הקופץ",
DlgLnkPopWinFeat	: "תכונות החלון הקופץ",
DlgLnkPopResize		: "בעל גודל ניתן לשינוי",
DlgLnkPopLocation	: "סרגל כתובת",
DlgLnkPopMenu		: "סרגל תפריט",
DlgLnkPopScroll		: "ניתן לגלילה",
DlgLnkPopStatus		: "סרגל חיווי",
DlgLnkPopToolbar	: "סרגל הכלים",
DlgLnkPopFullScrn	: "מסך מלא (IE)",
DlgLnkPopDependent	: "תלוי (Netscape)",
DlgLnkPopWidth		: "רוחב",
DlgLnkPopHeight		: "גובה",
DlgLnkPopLeft		: "מיקום צד שמאל",
DlgLnkPopTop		: "מיקום צד עליון",

DlnLnkMsgNoUrl		: "נא להקליד את כתובת הקישור (URL)",
DlnLnkMsgNoEMail	: "נא להקליד את כתובת הדוא''ל",
DlnLnkMsgNoAnchor	: "נא לבחור עוגן במסמך",
DlnLnkMsgInvPopName	: "שם החלון הקופץ חייב להתחיל באותיות ואסור לכלול רווחים",

// Color Dialog
DlgColorTitle		: "בחירת צבע",
DlgColorBtnClear	: "איפוס",
DlgColorHighlight	: "נוכחי",
DlgColorSelected	: "נבחר",

// Smiley Dialog
DlgSmileyTitle		: "הוספת סמיילי",

// Special Character Dialog
DlgSpecialCharTitle	: "בחירת תו מיוחד",

// Table Dialog
DlgTableTitle		: "תכונות טבלה",
DlgTableRows		: "שורות",
DlgTableColumns		: "עמודות",
DlgTableBorder		: "גודל מסגרת",
DlgTableAlign		: "יישור",
DlgTableAlignNotSet	: "<לא נקבע>",
DlgTableAlignLeft	: "שמאל",
DlgTableAlignCenter	: "מרכז",
DlgTableAlignRight	: "ימין",
DlgTableWidth		: "רוחב",
DlgTableWidthPx		: "פיקסלים",
DlgTableWidthPc		: "אחוז",
DlgTableHeight		: "גובה",
DlgTableCellSpace	: "מרווח תא",
DlgTableCellPad		: "ריפוד תא",
DlgTableCaption		: "כיתוב",
DlgTableSummary		: "סיכום",
DlgTableHeaders		: "כותרות",
DlgTableHeadersNone		: "אין",
DlgTableHeadersColumn	: "עמודה ראשונה",
DlgTableHeadersRow		: "שורה ראשונה",
DlgTableHeadersBoth		: "שניהם",

// Table Cell Dialog
DlgCellTitle		: "תכונות תא",
DlgCellWidth		: "רוחב",
DlgCellWidthPx		: "פיקסלים",
DlgCellWidthPc		: "אחוז",
DlgCellHeight		: "גובה",
DlgCellWordWrap		: "גלילת שורות",
DlgCellWordWrapNotSet	: "<לא נקבע>",
DlgCellWordWrapYes	: "כן",
DlgCellWordWrapNo	: "לא",
DlgCellHorAlign		: "יישור אופקי",
DlgCellHorAlignNotSet	: "<לא נקבע>",
DlgCellHorAlignLeft	: "שמאל",
DlgCellHorAlignCenter	: "מרכז",
DlgCellHorAlignRight: "ימין",
DlgCellVerAlign		: "יישור אנכי",
DlgCellVerAlignNotSet	: "<לא נקבע>",
DlgCellVerAlignTop	: "למעלה",
DlgCellVerAlignMiddle	: "לאמצע",
DlgCellVerAlignBottom	: "לתחתית",
DlgCellVerAlignBaseline	: "קו תחתית",
DlgCellType		: "סוג תא",
DlgCellTypeData		: "סוג",
DlgCellTypeHeader	: "כותרת",
DlgCellRowSpan		: "טווח שורות",
DlgCellCollSpan		: "טווח עמודות",
DlgCellBackColor	: "צבע רקע",
DlgCellBorderColor	: "צבע מסגרת",
DlgCellBtnSelect	: "בחירה...",

// Find and Replace Dialog
DlgFindAndReplaceTitle	: "חפש והחלף",

// Find Dialog
DlgFindTitle		: "חיפוש",
DlgFindFindBtn		: "חיפוש",
DlgFindNotFoundMsg	: "הטקסט המבוקש לא נמצא.",

// Replace Dialog
DlgReplaceTitle			: "החלפה",
DlgReplaceFindLbl		: "חיפוש מחרוזת:",
DlgReplaceReplaceLbl	: "החלפה במחרוזת:",
DlgReplaceCaseChk		: "התאמת סוג אותיות (Case)",
DlgReplaceReplaceBtn	: "החלפה",
DlgReplaceReplAllBtn	: "החלפה בכל העמוד",
DlgReplaceWordChk		: "התאמה למילה המלאה",

// Paste Operations / Dialog
PasteErrorCut	: "הגדרות האבטחה בדפדפן שלך לא מאפשרות לעורך לבצע פעולות גזירה  אוטומטיות. יש להשתמש במקלדת לשם כך (Ctrl+X).",
PasteErrorCopy	: "הגדרות האבטחה בדפדפן שלך לא מאפשרות לעורך לבצע פעולות העתקה אוטומטיות. יש להשתמש במקלדת לשם כך (Ctrl+C).",

PasteAsText		: "הדבקה כטקסט פשוט",
PasteFromWord	: "הדבקה מ-וורד",

DlgPasteMsg2	: "אנא הדבק בתוך הקופסה באמצעות  (<STRONG>Ctrl+V</STRONG>) ולחץ על  <STRONG>אישור</STRONG>.",
DlgPasteSec		: "עקב הגדרות אבטחה בדפדפן, לא ניתן לגשת אל לוח הגזירים (clipboard) בצורה ישירה.אנא בצע הדבק שוב בחלון זה.",
DlgPasteIgnoreFont		: "התעלם מהגדרות סוג פונט",
DlgPasteRemoveStyles	: "הסר הגדרות סגנון",

// Color Picker
ColorAutomatic	: "אוטומטי",
ColorMoreColors	: "צבעים נוספים...",

// Document Properties
DocProps		: "מאפיני מסמך",

// Anchor Dialog
DlgAnchorTitle		: "מאפיני נקודת עיגון",
DlgAnchorName		: "שם לנקודת עיגון",
DlgAnchorErrorName	: "אנא הזן שם לנקודת עיגון",

// Speller Pages Dialog
DlgSpellNotInDic		: "לא נמצא במילון",
DlgSpellChangeTo		: "שנה ל",
DlgSpellBtnIgnore		: "התעלם",
DlgSpellBtnIgnoreAll	: "התעלם מהכל",
DlgSpellBtnReplace		: "החלף",
DlgSpellBtnReplaceAll	: "החלף הכל",
DlgSpellBtnUndo			: "החזר",
DlgSpellNoSuggestions	: "- אין הצעות -",
DlgSpellProgress		: "בדיקות איות בתהליך ....",
DlgSpellNoMispell		: "בדיקות איות הסתיימה: לא נמצאו שגיעות כתיב",
DlgSpellNoChanges		: "בדיקות איות הסתיימה: לא שונתה אף מילה",
DlgSpellOneChange		: "בדיקות איות הסתיימה: שונתה מילה אחת",
DlgSpellManyChanges		: "בדיקות איות הסתיימה: %1 מילים שונו",

IeSpellDownload			: "בודק האיות לא מותקן, האם אתה מעוניין להוריד?",

// Button Dialog
DlgButtonText		: "טקסט (ערך)",
DlgButtonType		: "סוג",
DlgButtonTypeBtn	: "כפתור",
DlgButtonTypeSbm	: "שלח",
DlgButtonTypeRst	: "אפס",

// Checkbox and Radio Button Dialogs
DlgCheckboxName		: "שם",
DlgCheckboxValue	: "ערך",
DlgCheckboxSelected	: "בחור",

// Form Dialog
DlgFormName		: "שם",
DlgFormAction	: "שלח אל",
DlgFormMethod	: "סוג שליחה",

// Select Field Dialog
DlgSelectName		: "שם",
DlgSelectValue		: "ערך",
DlgSelectSize		: "גודל",
DlgSelectLines		: "שורות",
DlgSelectChkMulti	: "אפשר בחירות מרובות",
DlgSelectOpAvail	: "אפשרויות זמינות",
DlgSelectOpText		: "טקסט",
DlgSelectOpValue	: "ערך",
DlgSelectBtnAdd		: "הוסף",
DlgSelectBtnModify	: "שנה",
DlgSelectBtnUp		: "למעלה",
DlgSelectBtnDown	: "למטה",
DlgSelectBtnSetValue : "קבע כברירת מחדל",
DlgSelectBtnDelete	: "מחק",

// Textarea Dialog
DlgTextareaName	: "שם",
DlgTextareaCols	: "עמודות",
DlgTextareaRows	: "שורות",

// Text Field Dialog
DlgTextName			: "שם",
DlgTextValue		: "ערך",
DlgTextCharWidth	: "רוחב באותיות",
DlgTextMaxChars		: "מקסימות אותיות",
DlgTextType			: "סוג",
DlgTextTypeText		: "טקסט",
DlgTextTypePass		: "סיסמה",

// Hidden Field Dialog
DlgHiddenName	: "שם",
DlgHiddenValue	: "ערך",

// Bulleted List Dialog
BulletedListProp	: "מאפייני רשימה",
NumberedListProp	: "מאפייני רשימה ממוספרת",
DlgLstStart			: "התחלה",
DlgLstType			: "סוג",
DlgLstTypeCircle	: "עיגול",
DlgLstTypeDisc		: "דיסק",
DlgLstTypeSquare	: "מרובע",
DlgLstTypeNumbers	: "מספרים (1, 2, 3)",
DlgLstTypeLCase		: "אותיות קטנות (a, b, c)",
DlgLstTypeUCase		: "אותיות גדולות (A, B, C)",
DlgLstTypeSRoman	: "ספרות רומאיות קטנות (i, ii, iii)",
DlgLstTypeLRoman	: "ספרות רומאיות גדולות (I, II, III)",

// Document Properties Dialog
DlgDocGeneralTab	: "כללי",
DlgDocBackTab		: "רקע",
DlgDocColorsTab		: "צבעים וגבולות",
DlgDocMetaTab		: "נתוני META",

DlgDocPageTitle		: "כותרת דף",
DlgDocLangDir		: "כיוון שפה",
DlgDocLangDirLTR	: "שמאל לימין (LTR)",
DlgDocLangDirRTL	: "ימין לשמאל (RTL)",
DlgDocLangCode		: "קוד שפה",
DlgDocCharSet		: "קידוד אותיות",
DlgDocCharSetCE		: "מרכז אירופה",
DlgDocCharSetCT		: "סיני מסורתי (Big5)",
DlgDocCharSetCR		: "קירילי",
DlgDocCharSetGR		: "יוונית",
DlgDocCharSetJP		: "יפנית",
DlgDocCharSetKR		: "קוראנית",
DlgDocCharSetTR		: "טורקית",
DlgDocCharSetUN		: "יוני קוד (UTF-8)",
DlgDocCharSetWE		: "מערב אירופה",
DlgDocCharSetOther	: "קידוד אותיות אחר",

DlgDocDocType		: "הגדרות סוג מסמך",
DlgDocDocTypeOther	: "הגדרות סוג מסמך אחרות",
DlgDocIncXHTML		: "כלול הגדרות XHTML",
DlgDocBgColor		: "צבע רקע",
DlgDocBgImage		: "URL לתמונת רקע",
DlgDocBgNoScroll	: "רגע ללא גלילה",
DlgDocCText			: "טקסט",
DlgDocCLink			: "קישור",
DlgDocCVisited		: "קישור שבוקר",
DlgDocCActive		: " קישור פעיל",
DlgDocMargins		: "גבולות דף",
DlgDocMaTop			: "למעלה",
DlgDocMaLeft		: "שמאלה",
DlgDocMaRight		: "ימינה",
DlgDocMaBottom		: "למטה",
DlgDocMeIndex		: "מפתח עניינים של המסמך )מופרד בפסיק(",
DlgDocMeDescr		: "תאור מסמך",
DlgDocMeAuthor		: "מחבר",
DlgDocMeCopy		: "זכויות יוצרים",
DlgDocPreview		: "תצוגה מקדימה",

// Templates Dialog
Templates			: "תבניות",
DlgTemplatesTitle	: "תביות תוכן",
DlgTemplatesSelMsg	: "אנא בחר תבנית לפתיחה בעורך <BR>התוכן המקורי ימחק:",
DlgTemplatesLoading	: "מעלה רשימת תבניות אנא המתן",
DlgTemplatesNoTpl	: "(לא הוגדרו תבניות)",
DlgTemplatesReplace	: "החלפת תוכן ממשי",

// About Dialog
DlgAboutAboutTab	: "אודות",
DlgAboutBrowserInfoTab	: "גירסת דפדפן",
DlgAboutLicenseTab	: "רשיון",
DlgAboutVersion		: "גירסא",
DlgAboutInfo		: "מידע נוסף ניתן למצוא כאן:",

// Div Dialog
DlgDivGeneralTab	: "כללי",
DlgDivAdvancedTab	: "מתקדם",
DlgDivStyle		: "סגנון",
DlgDivInlineStyle	: "סגנון בתוך השורה",

ScaytTitle			: "SCAYT",	//MISSING
ScaytTitleOptions	: "Options",	//MISSING
ScaytTitleLangs		: "Languages",	//MISSING
ScaytTitleAbout		: "About"	//MISSING
};
