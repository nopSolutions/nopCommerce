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
 * Chinese Traditional language file.
 */

var FCKLang =
{
// Language direction : "ltr" (left to right) or "rtl" (right to left).
Dir					: "ltr",

ToolbarCollapse		: "隱藏面板",
ToolbarExpand		: "顯示面板",

// Toolbar Items and Context Menu
Save				: "儲存",
NewPage				: "開新檔案",
Preview				: "預覽",
Cut					: "剪下",
Copy				: "複製",
Paste				: "貼上",
PasteText			: "貼為純文字格式",
PasteWord			: "自 Word 貼上",
Print				: "列印",
SelectAll			: "全選",
RemoveFormat		: "清除格式",
InsertLinkLbl		: "超連結",
InsertLink			: "插入/編輯超連結",
RemoveLink			: "移除超連結",
VisitLink			: "開啟超連結",
Anchor				: "插入/編輯錨點",
AnchorDelete		: "移除錨點",
InsertImageLbl		: "影像",
InsertImage			: "插入/編輯影像",
InsertFlashLbl		: "Flash",
InsertFlash			: "插入/編輯 Flash",
InsertTableLbl		: "表格",
InsertTable			: "插入/編輯表格",
InsertLineLbl		: "水平線",
InsertLine			: "插入水平線",
InsertSpecialCharLbl: "特殊符號",
InsertSpecialChar	: "插入特殊符號",
InsertSmileyLbl		: "表情符號",
InsertSmiley		: "插入表情符號",
About				: "關於 FCKeditor",
Bold				: "粗體",
Italic				: "斜體",
Underline			: "底線",
StrikeThrough		: "刪除線",
Subscript			: "下標",
Superscript			: "上標",
LeftJustify			: "靠左對齊",
CenterJustify		: "置中",
RightJustify		: "靠右對齊",
BlockJustify		: "左右對齊",
DecreaseIndent		: "減少縮排",
IncreaseIndent		: "增加縮排",
Blockquote			: "引用文字",
CreateDiv			: "新增 Div 標籤",
EditDiv				: "變更 Div 標籤",
DeleteDiv			: "移除 Div 標籤",
Undo				: "復原",
Redo				: "重複",
NumberedListLbl		: "編號清單",
NumberedList		: "插入/移除編號清單",
BulletedListLbl		: "項目清單",
BulletedList		: "插入/移除項目清單",
ShowTableBorders	: "顯示表格邊框",
ShowDetails			: "顯示詳細資料",
Style				: "樣式",
FontFormat			: "格式",
Font				: "字體",
FontSize			: "大小",
TextColor			: "文字顏色",
BGColor				: "背景顏色",
Source				: "原始碼",
Find				: "尋找",
Replace				: "取代",
SpellCheck			: "拼字檢查",
UniversalKeyboard	: "萬國鍵盤",
PageBreakLbl		: "分頁符號",
PageBreak			: "插入分頁符號",

Form			: "表單",
Checkbox		: "核取方塊",
RadioButton		: "選項按鈕",
TextField		: "文字方塊",
Textarea		: "文字區域",
HiddenField		: "隱藏欄位",
Button			: "按鈕",
SelectionField	: "清單/選單",
ImageButton		: "影像按鈕",

FitWindow		: "編輯器最大化",
ShowBlocks		: "顯示區塊",

// Context Menu
EditLink			: "編輯超連結",
CellCM				: "儲存格",
RowCM				: "列",
ColumnCM			: "欄",
InsertRowAfter		: "向下插入列",
InsertRowBefore		: "向上插入列",
DeleteRows			: "刪除列",
InsertColumnAfter	: "向右插入欄",
InsertColumnBefore	: "向左插入欄",
DeleteColumns		: "刪除欄",
InsertCellAfter		: "向右插入儲存格",
InsertCellBefore	: "向左插入儲存格",
DeleteCells			: "刪除儲存格",
MergeCells			: "合併儲存格",
MergeRight			: "向右合併儲存格",
MergeDown			: "向下合併儲存格",
HorizontalSplitCell	: "橫向分割儲存格",
VerticalSplitCell	: "縱向分割儲存格",
TableDelete			: "刪除表格",
CellProperties		: "儲存格屬性",
TableProperties		: "表格屬性",
ImageProperties		: "影像屬性",
FlashProperties		: "Flash 屬性",

AnchorProp			: "錨點屬性",
ButtonProp			: "按鈕屬性",
CheckboxProp		: "核取方塊屬性",
HiddenFieldProp		: "隱藏欄位屬性",
RadioButtonProp		: "選項按鈕屬性",
ImageButtonProp		: "影像按鈕屬性",
TextFieldProp		: "文字方塊屬性",
SelectionFieldProp	: "清單/選單屬性",
TextareaProp		: "文字區域屬性",
FormProp			: "表單屬性",

FontFormats			: "一般;已格式化;位址;標題 1;標題 2;標題 3;標題 4;標題 5;標題 6;一般 (DIV)",

// Alerts and Messages
ProcessingXHTML		: "處理 XHTML 中，請稍候…",
Done				: "完成",
PasteWordConfirm	: "您想貼上的文字似乎是自 Word 複製而來，請問您是否要先清除 Word 的格式後再行貼上？",
NotCompatiblePaste	: "此指令僅在 Internet Explorer 5.5 或以上的版本有效。請問您是否同意不清除格式即貼上？",
UnknownToolbarItem	: "未知工具列項目 \"%1\"",
UnknownCommand		: "未知指令名稱 \"%1\"",
NotImplemented		: "尚未安裝此指令",
UnknownToolbarSet	: "工具列設定 \"%1\" 不存在",
NoActiveX			: "瀏覽器的安全性設定限制了本編輯器的某些功能。您必須啟用安全性設定中的「執行ActiveX控制項與外掛程式」項目，否則本編輯器將會出現錯誤並缺少某些功能",
BrowseServerBlocked : "無法開啟資源瀏覽器，請確定所有快顯視窗封鎖程式是否關閉",
DialogBlocked		: "無法開啟對話視窗，請確定所有快顯視窗封鎖程式是否關閉",
VisitLinkBlocked	: "無法開啟新視窗，請確定所有快顯視窗封鎖程式是否關閉",

// Dialogs
DlgBtnOK			: "確定",
DlgBtnCancel		: "取消",
DlgBtnClose			: "關閉",
DlgBtnBrowseServer	: "瀏覽伺服器端",
DlgAdvancedTag		: "進階",
DlgOpOther			: "<其他>",
DlgInfoTab			: "資訊",
DlgAlertUrl			: "請插入 URL",

// General Dialogs Labels
DlgGenNotSet		: "<尚未設定>",
DlgGenId			: "ID",
DlgGenLangDir		: "語言方向",
DlgGenLangDirLtr	: "由左而右 (LTR)",
DlgGenLangDirRtl	: "由右而左 (RTL)",
DlgGenLangCode		: "語言代碼",
DlgGenAccessKey		: "存取鍵",
DlgGenName			: "名稱",
DlgGenTabIndex		: "定位順序",
DlgGenLongDescr		: "詳細 URL",
DlgGenClass			: "樣式表類別",
DlgGenTitle			: "標題",
DlgGenContType		: "內容類型",
DlgGenLinkCharset	: "連結資源之編碼",
DlgGenStyle			: "樣式",

// Image Dialog
DlgImgTitle			: "影像屬性",
DlgImgInfoTab		: "影像資訊",
DlgImgBtnUpload		: "上傳至伺服器",
DlgImgURL			: "URL",
DlgImgUpload		: "上傳",
DlgImgAlt			: "替代文字",
DlgImgWidth			: "寬度",
DlgImgHeight		: "高度",
DlgImgLockRatio		: "等比例",
DlgBtnResetSize		: "重設為原大小",
DlgImgBorder		: "邊框",
DlgImgHSpace		: "水平距離",
DlgImgVSpace		: "垂直距離",
DlgImgAlign			: "對齊",
DlgImgAlignLeft		: "靠左對齊",
DlgImgAlignAbsBottom: "絕對下方",
DlgImgAlignAbsMiddle: "絕對中間",
DlgImgAlignBaseline	: "基準線",
DlgImgAlignBottom	: "靠下對齊",
DlgImgAlignMiddle	: "置中對齊",
DlgImgAlignRight	: "靠右對齊",
DlgImgAlignTextTop	: "文字上方",
DlgImgAlignTop		: "靠上對齊",
DlgImgPreview		: "預覽",
DlgImgAlertUrl		: "請輸入影像 URL",
DlgImgLinkTab		: "超連結",

// Flash Dialog
DlgFlashTitle		: "Flash 屬性",
DlgFlashChkPlay		: "自動播放",
DlgFlashChkLoop		: "重複",
DlgFlashChkMenu		: "開啟選單",
DlgFlashScale		: "縮放",
DlgFlashScaleAll	: "全部顯示",
DlgFlashScaleNoBorder	: "無邊框",
DlgFlashScaleFit	: "精確符合",

// Link Dialog
DlgLnkWindowTitle	: "超連結",
DlgLnkInfoTab		: "超連結資訊",
DlgLnkTargetTab		: "目標",

DlgLnkType			: "超連接類型",
DlgLnkTypeURL		: "URL",
DlgLnkTypeAnchor	: "本頁錨點",
DlgLnkTypeEMail		: "電子郵件",
DlgLnkProto			: "通訊協定",
DlgLnkProtoOther	: "<其他>",
DlgLnkURL			: "URL",
DlgLnkAnchorSel		: "請選擇錨點",
DlgLnkAnchorByName	: "依錨點名稱",
DlgLnkAnchorById	: "依元件 ID",
DlgLnkNoAnchors		: "(本文件尚無可用之錨點)",
DlgLnkEMail			: "電子郵件",
DlgLnkEMailSubject	: "郵件主旨",
DlgLnkEMailBody		: "郵件內容",
DlgLnkUpload		: "上傳",
DlgLnkBtnUpload		: "傳送至伺服器",

DlgLnkTarget		: "目標",
DlgLnkTargetFrame	: "<框架>",
DlgLnkTargetPopup	: "<快顯視窗>",
DlgLnkTargetBlank	: "新視窗 (_blank)",
DlgLnkTargetParent	: "父視窗 (_parent)",
DlgLnkTargetSelf	: "本視窗 (_self)",
DlgLnkTargetTop		: "最上層視窗 (_top)",
DlgLnkTargetFrameName	: "目標框架名稱",
DlgLnkPopWinName	: "快顯視窗名稱",
DlgLnkPopWinFeat	: "快顯視窗屬性",
DlgLnkPopResize		: "可調整大小",
DlgLnkPopLocation	: "網址列",
DlgLnkPopMenu		: "選單列",
DlgLnkPopScroll		: "捲軸",
DlgLnkPopStatus		: "狀態列",
DlgLnkPopToolbar	: "工具列",
DlgLnkPopFullScrn	: "全螢幕 (IE)",
DlgLnkPopDependent	: "從屬 (NS)",
DlgLnkPopWidth		: "寬",
DlgLnkPopHeight		: "高",
DlgLnkPopLeft		: "左",
DlgLnkPopTop		: "右",

DlnLnkMsgNoUrl		: "請輸入欲連結的 URL",
DlnLnkMsgNoEMail	: "請輸入電子郵件位址",
DlnLnkMsgNoAnchor	: "請選擇錨點",
DlnLnkMsgInvPopName	: "快顯名稱必須以「英文字母」為開頭，且不得含有空白",

// Color Dialog
DlgColorTitle		: "請選擇顏色",
DlgColorBtnClear	: "清除",
DlgColorHighlight	: "預覽",
DlgColorSelected	: "選擇",

// Smiley Dialog
DlgSmileyTitle		: "插入表情符號",

// Special Character Dialog
DlgSpecialCharTitle	: "請選擇特殊符號",

// Table Dialog
DlgTableTitle		: "表格屬性",
DlgTableRows		: "列數",
DlgTableColumns		: "欄數",
DlgTableBorder		: "邊框",
DlgTableAlign		: "對齊",
DlgTableAlignNotSet	: "<未設定>",
DlgTableAlignLeft	: "靠左對齊",
DlgTableAlignCenter	: "置中",
DlgTableAlignRight	: "靠右對齊",
DlgTableWidth		: "寬度",
DlgTableWidthPx		: "像素",
DlgTableWidthPc		: "百分比",
DlgTableHeight		: "高度",
DlgTableCellSpace	: "間距",
DlgTableCellPad		: "內距",
DlgTableCaption		: "標題",
DlgTableSummary		: "摘要",
DlgTableHeaders		: "Headers",	//MISSING
DlgTableHeadersNone		: "None",	//MISSING
DlgTableHeadersColumn	: "First column",	//MISSING
DlgTableHeadersRow		: "First Row",	//MISSING
DlgTableHeadersBoth		: "Both",	//MISSING

// Table Cell Dialog
DlgCellTitle		: "儲存格屬性",
DlgCellWidth		: "寬度",
DlgCellWidthPx		: "像素",
DlgCellWidthPc		: "百分比",
DlgCellHeight		: "高度",
DlgCellWordWrap		: "自動換行",
DlgCellWordWrapNotSet	: "<尚未設定>",
DlgCellWordWrapYes	: "是",
DlgCellWordWrapNo	: "否",
DlgCellHorAlign		: "水平對齊",
DlgCellHorAlignNotSet	: "<尚未設定>",
DlgCellHorAlignLeft	: "靠左對齊",
DlgCellHorAlignCenter	: "置中",
DlgCellHorAlignRight: "靠右對齊",
DlgCellVerAlign		: "垂直對齊",
DlgCellVerAlignNotSet	: "<尚未設定>",
DlgCellVerAlignTop	: "靠上對齊",
DlgCellVerAlignMiddle	: "置中",
DlgCellVerAlignBottom	: "靠下對齊",
DlgCellVerAlignBaseline	: "基準線",
DlgCellType		: "儲存格類型",
DlgCellTypeData		: "資料",
DlgCellTypeHeader	: "標題",
DlgCellRowSpan		: "合併列數",
DlgCellCollSpan		: "合併欄数",
DlgCellBackColor	: "背景顏色",
DlgCellBorderColor	: "邊框顏色",
DlgCellBtnSelect	: "請選擇…",

// Find and Replace Dialog
DlgFindAndReplaceTitle	: "尋找與取代",

// Find Dialog
DlgFindTitle		: "尋找",
DlgFindFindBtn		: "尋找",
DlgFindNotFoundMsg	: "未找到指定的文字。",

// Replace Dialog
DlgReplaceTitle			: "取代",
DlgReplaceFindLbl		: "尋找:",
DlgReplaceReplaceLbl	: "取代:",
DlgReplaceCaseChk		: "大小寫須相符",
DlgReplaceReplaceBtn	: "取代",
DlgReplaceReplAllBtn	: "全部取代",
DlgReplaceWordChk		: "全字相符",

// Paste Operations / Dialog
PasteErrorCut	: "瀏覽器的安全性設定不允許編輯器自動執行剪下動作。請使用快捷鍵 (Ctrl+X) 剪下。",
PasteErrorCopy	: "瀏覽器的安全性設定不允許編輯器自動執行複製動作。請使用快捷鍵 (Ctrl+C) 複製。",

PasteAsText		: "貼為純文字格式",
PasteFromWord	: "自 Word 貼上",

DlgPasteMsg2	: "請使用快捷鍵 (<strong>Ctrl+V</strong>) 貼到下方區域中並按下 <strong>確定</strong>",
DlgPasteSec		: "因為瀏覽器的安全性設定，本編輯器無法直接存取您的剪貼簿資料，請您自行在本視窗進行貼上動作。",
DlgPasteIgnoreFont		: "移除字型設定",
DlgPasteRemoveStyles	: "移除樣式設定",

// Color Picker
ColorAutomatic	: "自動",
ColorMoreColors	: "更多顏色…",

// Document Properties
DocProps		: "文件屬性",

// Anchor Dialog
DlgAnchorTitle		: "命名錨點",
DlgAnchorName		: "錨點名稱",
DlgAnchorErrorName	: "請輸入錨點名稱",

// Speller Pages Dialog
DlgSpellNotInDic		: "不在字典中",
DlgSpellChangeTo		: "更改為",
DlgSpellBtnIgnore		: "忽略",
DlgSpellBtnIgnoreAll	: "全部忽略",
DlgSpellBtnReplace		: "取代",
DlgSpellBtnReplaceAll	: "全部取代",
DlgSpellBtnUndo			: "復原",
DlgSpellNoSuggestions	: "- 無建議值 -",
DlgSpellProgress		: "進行拼字檢查中…",
DlgSpellNoMispell		: "拼字檢查完成：未發現拼字錯誤",
DlgSpellNoChanges		: "拼字檢查完成：未更改任何單字",
DlgSpellOneChange		: "拼字檢查完成：更改了 1 個單字",
DlgSpellManyChanges		: "拼字檢查完成：更改了 %1 個單字",

IeSpellDownload			: "尚未安裝拼字檢查元件。您是否想要現在下載？",

// Button Dialog
DlgButtonText		: "顯示文字 (值)",
DlgButtonType		: "類型",
DlgButtonTypeBtn	: "按鈕 (Button)",
DlgButtonTypeSbm	: "送出 (Submit)",
DlgButtonTypeRst	: "重設 (Reset)",

// Checkbox and Radio Button Dialogs
DlgCheckboxName		: "名稱",
DlgCheckboxValue	: "選取值",
DlgCheckboxSelected	: "已選取",

// Form Dialog
DlgFormName		: "名稱",
DlgFormAction	: "動作",
DlgFormMethod	: "方法",

// Select Field Dialog
DlgSelectName		: "名稱",
DlgSelectValue		: "選取值",
DlgSelectSize		: "大小",
DlgSelectLines		: "行",
DlgSelectChkMulti	: "可多選",
DlgSelectOpAvail	: "可用選項",
DlgSelectOpText		: "顯示文字",
DlgSelectOpValue	: "值",
DlgSelectBtnAdd		: "新增",
DlgSelectBtnModify	: "修改",
DlgSelectBtnUp		: "上移",
DlgSelectBtnDown	: "下移",
DlgSelectBtnSetValue : "設為預設值",
DlgSelectBtnDelete	: "刪除",

// Textarea Dialog
DlgTextareaName	: "名稱",
DlgTextareaCols	: "字元寬度",
DlgTextareaRows	: "列數",

// Text Field Dialog
DlgTextName			: "名稱",
DlgTextValue		: "值",
DlgTextCharWidth	: "字元寬度",
DlgTextMaxChars		: "最多字元數",
DlgTextType			: "類型",
DlgTextTypeText		: "文字",
DlgTextTypePass		: "密碼",

// Hidden Field Dialog
DlgHiddenName	: "名稱",
DlgHiddenValue	: "值",

// Bulleted List Dialog
BulletedListProp	: "項目清單屬性",
NumberedListProp	: "編號清單屬性",
DlgLstStart			: "起始編號",
DlgLstType			: "清單類型",
DlgLstTypeCircle	: "圓圈",
DlgLstTypeDisc		: "圓點",
DlgLstTypeSquare	: "方塊",
DlgLstTypeNumbers	: "數字 (1, 2, 3)",
DlgLstTypeLCase		: "小寫字母 (a, b, c)",
DlgLstTypeUCase		: "大寫字母 (A, B, C)",
DlgLstTypeSRoman	: "小寫羅馬數字 (i, ii, iii)",
DlgLstTypeLRoman	: "大寫羅馬數字 (I, II, III)",

// Document Properties Dialog
DlgDocGeneralTab	: "一般",
DlgDocBackTab		: "背景",
DlgDocColorsTab		: "顯色與邊界",
DlgDocMetaTab		: "Meta 資料",

DlgDocPageTitle		: "頁面標題",
DlgDocLangDir		: "語言方向",
DlgDocLangDirLTR	: "由左而右 (LTR)",
DlgDocLangDirRTL	: "由右而左 (RTL)",
DlgDocLangCode		: "語言代碼",
DlgDocCharSet		: "字元編碼",
DlgDocCharSetCE		: "中歐語系",
DlgDocCharSetCT		: "正體中文 (Big5)",
DlgDocCharSetCR		: "斯拉夫文",
DlgDocCharSetGR		: "希臘文",
DlgDocCharSetJP		: "日文",
DlgDocCharSetKR		: "韓文",
DlgDocCharSetTR		: "土耳其文",
DlgDocCharSetUN		: "Unicode (UTF-8)",
DlgDocCharSetWE		: "西歐語系",
DlgDocCharSetOther	: "其他字元編碼",

DlgDocDocType		: "文件類型",
DlgDocDocTypeOther	: "其他文件類型",
DlgDocIncXHTML		: "包含 XHTML 定義",
DlgDocBgColor		: "背景顏色",
DlgDocBgImage		: "背景影像",
DlgDocBgNoScroll	: "浮水印",
DlgDocCText			: "文字",
DlgDocCLink			: "超連結",
DlgDocCVisited		: "已瀏覽過的超連結",
DlgDocCActive		: "作用中的超連結",
DlgDocMargins		: "頁面邊界",
DlgDocMaTop			: "上",
DlgDocMaLeft		: "左",
DlgDocMaRight		: "右",
DlgDocMaBottom		: "下",
DlgDocMeIndex		: "文件索引關鍵字 (用半形逗號[,]分隔)",
DlgDocMeDescr		: "文件說明",
DlgDocMeAuthor		: "作者",
DlgDocMeCopy		: "版權所有",
DlgDocPreview		: "預覽",

// Templates Dialog
Templates			: "樣版",
DlgTemplatesTitle	: "內容樣版",
DlgTemplatesSelMsg	: "請選擇欲開啟的樣版<br> (原有的內容將會被清除):",
DlgTemplatesLoading	: "讀取樣版清單中，請稍候…",
DlgTemplatesNoTpl	: "(無樣版)",
DlgTemplatesReplace	: "取代原有內容",

// About Dialog
DlgAboutAboutTab	: "關於",
DlgAboutBrowserInfoTab	: "瀏覽器資訊",
DlgAboutLicenseTab	: "許可證",
DlgAboutVersion		: "版本",
DlgAboutInfo		: "想獲得更多資訊請至 ",

// Div Dialog
DlgDivGeneralTab	: "一般",
DlgDivAdvancedTab	: "進階",
DlgDivStyle		: "樣式",
DlgDivInlineStyle	: "CSS 樣式",

ScaytTitle			: "SCAYT",	//MISSING
ScaytTitleOptions	: "Options",	//MISSING
ScaytTitleLangs		: "Languages",	//MISSING
ScaytTitleAbout		: "About"	//MISSING
};
