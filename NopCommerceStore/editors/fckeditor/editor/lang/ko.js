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
 * Korean language file.
 */

var FCKLang =
{
// Language direction : "ltr" (left to right) or "rtl" (right to left).
Dir					: "ltr",

ToolbarCollapse		: "툴바 감추기",
ToolbarExpand		: "툴바 보이기",

// Toolbar Items and Context Menu
Save				: "저장하기",
NewPage				: "새 문서",
Preview				: "미리보기",
Cut					: "잘라내기",
Copy				: "복사하기",
Paste				: "붙여넣기",
PasteText			: "텍스트로 붙여넣기",
PasteWord			: "MS Word 형식에서 붙여넣기",
Print				: "인쇄하기",
SelectAll			: "전체선택",
RemoveFormat		: "포맷 지우기",
InsertLinkLbl		: "링크",
InsertLink			: "링크 삽입/변경",
RemoveLink			: "링크 삭제",
VisitLink			: "Open Link",	//MISSING
Anchor				: "책갈피 삽입/변경",
AnchorDelete		: "Remove Anchor",	//MISSING
InsertImageLbl		: "이미지",
InsertImage			: "이미지 삽입/변경",
InsertFlashLbl		: "플래쉬",
InsertFlash			: "플래쉬 삽입/변경",
InsertTableLbl		: "표",
InsertTable			: "표 삽입/변경",
InsertLineLbl		: "수평선",
InsertLine			: "수평선 삽입",
InsertSpecialCharLbl: "특수문자 삽입",
InsertSpecialChar	: "특수문자 삽입",
InsertSmileyLbl		: "아이콘",
InsertSmiley		: "아이콘 삽입",
About				: "FCKeditor에 대하여",
Bold				: "진하게",
Italic				: "이텔릭",
Underline			: "밑줄",
StrikeThrough		: "취소선",
Subscript			: "아래 첨자",
Superscript			: "위 첨자",
LeftJustify			: "왼쪽 정렬",
CenterJustify		: "가운데 정렬",
RightJustify		: "오른쪽 정렬",
BlockJustify		: "양쪽 맞춤",
DecreaseIndent		: "내어쓰기",
IncreaseIndent		: "들여쓰기",
Blockquote			: "Blockquote",	//MISSING
CreateDiv			: "Create Div Container",	//MISSING
EditDiv				: "Edit Div Container",	//MISSING
DeleteDiv			: "Remove Div Container",	//MISSING
Undo				: "취소",
Redo				: "재실행",
NumberedListLbl		: "순서있는 목록",
NumberedList		: "순서있는 목록",
BulletedListLbl		: "순서없는 목록",
BulletedList		: "순서없는 목록",
ShowTableBorders	: "표 테두리 보기",
ShowDetails			: "문서기호 보기",
Style				: "스타일",
FontFormat			: "포맷",
Font				: "폰트",
FontSize			: "글자 크기",
TextColor			: "글자 색상",
BGColor				: "배경 색상",
Source				: "소스",
Find				: "찾기",
Replace				: "바꾸기",
SpellCheck			: "철자검사",
UniversalKeyboard	: "다국어 입력기",
PageBreakLbl		: "Page Break",	//MISSING
PageBreak			: "Insert Page Break",	//MISSING

Form			: "폼",
Checkbox		: "체크박스",
RadioButton		: "라디오버튼",
TextField		: "입력필드",
Textarea		: "입력영역",
HiddenField		: "숨김필드",
Button			: "버튼",
SelectionField	: "펼침목록",
ImageButton		: "이미지버튼",

FitWindow		: "에디터 최대화",
ShowBlocks		: "Show Blocks",	//MISSING

// Context Menu
EditLink			: "링크 수정",
CellCM				: "셀/칸(Cell)",
RowCM				: "행(Row)",
ColumnCM			: "열(Column)",
InsertRowAfter		: "뒤에 행 삽입",
InsertRowBefore		: "앞에 행 삽입",
DeleteRows			: "가로줄 삭제",
InsertColumnAfter	: "뒤에 열 삽입",
InsertColumnBefore	: "앞에 열 삽입",
DeleteColumns		: "세로줄 삭제",
InsertCellAfter		: "뒤에 셀/칸 삽입",
InsertCellBefore	: "앞에 셀/칸 삽입",
DeleteCells			: "셀 삭제",
MergeCells			: "셀 합치기",
MergeRight			: "오른쪽 뭉치기",
MergeDown			: "왼쪽 뭉치기",
HorizontalSplitCell	: "수평 나누기",
VerticalSplitCell	: "수직 나누기",
TableDelete			: "표 삭제",
CellProperties		: "셀 속성",
TableProperties		: "표 속성",
ImageProperties		: "이미지 속성",
FlashProperties		: "플래쉬 속성",

AnchorProp			: "책갈피 속성",
ButtonProp			: "버튼 속성",
CheckboxProp		: "체크박스 속성",
HiddenFieldProp		: "숨김필드 속성",
RadioButtonProp		: "라디오버튼 속성",
ImageButtonProp		: "이미지버튼 속성",
TextFieldProp		: "입력필드 속성",
SelectionFieldProp	: "펼침목록 속성",
TextareaProp		: "입력영역 속성",
FormProp			: "폼 속성",

FontFormats			: "Normal;Formatted;Address;Heading 1;Heading 2;Heading 3;Heading 4;Heading 5;Heading 6",

// Alerts and Messages
ProcessingXHTML		: "XHTML 처리중. 잠시만 기다려주십시요.",
Done				: "완료",
PasteWordConfirm	: "붙여넣기 할 텍스트는 MS Word에서 복사한 것입니다. 붙여넣기 전에 MS Word 포멧을 삭제하시겠습니까?",
NotCompatiblePaste	: "이 명령은 인터넷익스플로러 5.5 버전 이상에서만 작동합니다. 포멧을 삭제하지 않고 붙여넣기 하시겠습니까?",
UnknownToolbarItem	: "알수없는 툴바입니다. : \"%1\"",
UnknownCommand		: "알수없는 기능입니다. : \"%1\"",
NotImplemented		: "기능이 실행되지 않았습니다.",
UnknownToolbarSet	: "툴바 설정이 없습니다. : \"%1\"",
NoActiveX			: "브러우저의 보안 설정으로 인해 몇몇 기능의 작동에 장애가 있을 수 있습니다. \"액티브-액스 기능과 플러그 인\" 옵션을 허용하여 주시지 않으면 오류가 발생할 수 있습니다.",
BrowseServerBlocked : "브러우저 요소가 열리지 않습니다. 팝업차단 설정이 꺼져있는지 확인하여 주십시오.",
DialogBlocked		: "윈도우 대화창을 열 수 없습니다. 팝업차단 설정이 꺼져있는지 확인하여 주십시오.",
VisitLinkBlocked	: "It was not possible to open a new window. Make sure all popup blockers are disabled.",	//MISSING

// Dialogs
DlgBtnOK			: "예",
DlgBtnCancel		: "아니오",
DlgBtnClose			: "닫기",
DlgBtnBrowseServer	: "서버 보기",
DlgAdvancedTag		: "자세히",
DlgOpOther			: "<기타>",
DlgInfoTab			: "정보",
DlgAlertUrl			: "URL을 입력하십시요",

// General Dialogs Labels
DlgGenNotSet		: "<설정되지 않음>",
DlgGenId			: "ID",
DlgGenLangDir		: "쓰기 방향",
DlgGenLangDirLtr	: "왼쪽에서 오른쪽 (LTR)",
DlgGenLangDirRtl	: "오른쪽에서 왼쪽 (RTL)",
DlgGenLangCode		: "언어 코드",
DlgGenAccessKey		: "엑세스 키",
DlgGenName			: "Name",
DlgGenTabIndex		: "탭 순서",
DlgGenLongDescr		: "URL 설명",
DlgGenClass			: "Stylesheet Classes",
DlgGenTitle			: "Advisory Title",
DlgGenContType		: "Advisory Content Type",
DlgGenLinkCharset	: "Linked Resource Charset",
DlgGenStyle			: "Style",

// Image Dialog
DlgImgTitle			: "이미지 설정",
DlgImgInfoTab		: "이미지 정보",
DlgImgBtnUpload		: "서버로 전송",
DlgImgURL			: "URL",
DlgImgUpload		: "업로드",
DlgImgAlt			: "이미지 설명",
DlgImgWidth			: "너비",
DlgImgHeight		: "높이",
DlgImgLockRatio		: "비율 유지",
DlgBtnResetSize		: "원래 크기로",
DlgImgBorder		: "테두리",
DlgImgHSpace		: "수평여백",
DlgImgVSpace		: "수직여백",
DlgImgAlign			: "정렬",
DlgImgAlignLeft		: "왼쪽",
DlgImgAlignAbsBottom: "줄아래(Abs Bottom)",
DlgImgAlignAbsMiddle: "줄중간(Abs Middle)",
DlgImgAlignBaseline	: "기준선",
DlgImgAlignBottom	: "아래",
DlgImgAlignMiddle	: "중간",
DlgImgAlignRight	: "오른쪽",
DlgImgAlignTextTop	: "글자상단",
DlgImgAlignTop		: "위",
DlgImgPreview		: "미리보기",
DlgImgAlertUrl		: "이미지 URL을 입력하십시요",
DlgImgLinkTab		: "링크",

// Flash Dialog
DlgFlashTitle		: "플래쉬 등록정보",
DlgFlashChkPlay		: "자동재생",
DlgFlashChkLoop		: "반복",
DlgFlashChkMenu		: "플래쉬메뉴 가능",
DlgFlashScale		: "영역",
DlgFlashScaleAll	: "모두보기",
DlgFlashScaleNoBorder	: "경계선없음",
DlgFlashScaleFit	: "영역자동조절",

// Link Dialog
DlgLnkWindowTitle	: "링크",
DlgLnkInfoTab		: "링크 정보",
DlgLnkTargetTab		: "타겟",

DlgLnkType			: "링크 종류",
DlgLnkTypeURL		: "URL",
DlgLnkTypeAnchor	: "책갈피",
DlgLnkTypeEMail		: "이메일",
DlgLnkProto			: "프로토콜",
DlgLnkProtoOther	: "<기타>",
DlgLnkURL			: "URL",
DlgLnkAnchorSel		: "책갈피 선택",
DlgLnkAnchorByName	: "책갈피 이름",
DlgLnkAnchorById	: "책갈피 ID",
DlgLnkNoAnchors		: "(문서에 책갈피가 없습니다.)",
DlgLnkEMail			: "이메일 주소",
DlgLnkEMailSubject	: "제목",
DlgLnkEMailBody		: "내용",
DlgLnkUpload		: "업로드",
DlgLnkBtnUpload		: "서버로 전송",

DlgLnkTarget		: "타겟",
DlgLnkTargetFrame	: "<프레임>",
DlgLnkTargetPopup	: "<팝업창>",
DlgLnkTargetBlank	: "새 창 (_blank)",
DlgLnkTargetParent	: "부모 창 (_parent)",
DlgLnkTargetSelf	: "현재 창 (_self)",
DlgLnkTargetTop		: "최 상위 창 (_top)",
DlgLnkTargetFrameName	: "타겟 프레임 이름",
DlgLnkPopWinName	: "팝업창 이름",
DlgLnkPopWinFeat	: "팝업창 설정",
DlgLnkPopResize		: "크기조정",
DlgLnkPopLocation	: "주소표시줄",
DlgLnkPopMenu		: "메뉴바",
DlgLnkPopScroll		: "스크롤바",
DlgLnkPopStatus		: "상태바",
DlgLnkPopToolbar	: "툴바",
DlgLnkPopFullScrn	: "전체화면 (IE)",
DlgLnkPopDependent	: "Dependent (Netscape)",
DlgLnkPopWidth		: "너비",
DlgLnkPopHeight		: "높이",
DlgLnkPopLeft		: "왼쪽 위치",
DlgLnkPopTop		: "윗쪽 위치",

DlnLnkMsgNoUrl		: "링크 URL을 입력하십시요.",
DlnLnkMsgNoEMail	: "이메일주소를 입력하십시요.",
DlnLnkMsgNoAnchor	: "책갈피명을 입력하십시요.",
DlnLnkMsgInvPopName	: "팝업창의 타이틀은 공백을 허용하지 않습니다.",

// Color Dialog
DlgColorTitle		: "색상 선택",
DlgColorBtnClear	: "지우기",
DlgColorHighlight	: "현재",
DlgColorSelected	: "선택됨",

// Smiley Dialog
DlgSmileyTitle		: "아이콘 삽입",

// Special Character Dialog
DlgSpecialCharTitle	: "특수문자 선택",

// Table Dialog
DlgTableTitle		: "표 설정",
DlgTableRows		: "가로줄",
DlgTableColumns		: "세로줄",
DlgTableBorder		: "테두리 크기",
DlgTableAlign		: "정렬",
DlgTableAlignNotSet	: "<설정되지 않음>",
DlgTableAlignLeft	: "왼쪽",
DlgTableAlignCenter	: "가운데",
DlgTableAlignRight	: "오른쪽",
DlgTableWidth		: "너비",
DlgTableWidthPx		: "픽셀",
DlgTableWidthPc		: "퍼센트",
DlgTableHeight		: "높이",
DlgTableCellSpace	: "셀 간격",
DlgTableCellPad		: "셀 여백",
DlgTableCaption		: "캡션",
DlgTableSummary		: "요약",
DlgTableHeaders		: "Headers",	//MISSING
DlgTableHeadersNone		: "None",	//MISSING
DlgTableHeadersColumn	: "First column",	//MISSING
DlgTableHeadersRow		: "First Row",	//MISSING
DlgTableHeadersBoth		: "Both",	//MISSING

// Table Cell Dialog
DlgCellTitle		: "셀 설정",
DlgCellWidth		: "너비",
DlgCellWidthPx		: "픽셀",
DlgCellWidthPc		: "퍼센트",
DlgCellHeight		: "높이",
DlgCellWordWrap		: "워드랩",
DlgCellWordWrapNotSet	: "<설정되지 않음>",
DlgCellWordWrapYes	: "예",
DlgCellWordWrapNo	: "아니오",
DlgCellHorAlign		: "수평 정렬",
DlgCellHorAlignNotSet	: "<설정되지 않음>",
DlgCellHorAlignLeft	: "왼쪽",
DlgCellHorAlignCenter	: "가운데",
DlgCellHorAlignRight: "오른쪽",
DlgCellVerAlign		: "수직 정렬",
DlgCellVerAlignNotSet	: "<설정되지 않음>",
DlgCellVerAlignTop	: "위",
DlgCellVerAlignMiddle	: "중간",
DlgCellVerAlignBottom	: "아래",
DlgCellVerAlignBaseline	: "기준선",
DlgCellType		: "Cell Type",	//MISSING
DlgCellTypeData		: "Data",	//MISSING
DlgCellTypeHeader	: "Header",	//MISSING
DlgCellRowSpan		: "세로 합치기",
DlgCellCollSpan		: "가로 합치기",
DlgCellBackColor	: "배경 색상",
DlgCellBorderColor	: "테두리 색상",
DlgCellBtnSelect	: "선택",

// Find and Replace Dialog
DlgFindAndReplaceTitle	: "찾기 & 바꾸기",

// Find Dialog
DlgFindTitle		: "찾기",
DlgFindFindBtn		: "찾기",
DlgFindNotFoundMsg	: "문자열을 찾을 수 없습니다.",

// Replace Dialog
DlgReplaceTitle			: "바꾸기",
DlgReplaceFindLbl		: "찾을 문자열:",
DlgReplaceReplaceLbl	: "바꿀 문자열:",
DlgReplaceCaseChk		: "대소문자 구분",
DlgReplaceReplaceBtn	: "바꾸기",
DlgReplaceReplAllBtn	: "모두 바꾸기",
DlgReplaceWordChk		: "온전한 단어",

// Paste Operations / Dialog
PasteErrorCut	: "브라우저의 보안설정때문에 잘라내기 기능을 실행할 수 없습니다. 키보드 명령을 사용하십시요. (Ctrl+X).",
PasteErrorCopy	: "브라우저의 보안설정때문에 복사하기 기능을 실행할 수 없습니다. 키보드 명령을 사용하십시요.  (Ctrl+C).",

PasteAsText		: "텍스트로 붙여넣기",
PasteFromWord	: "MS Word 형식에서 붙여넣기",

DlgPasteMsg2	: "키보드의 (<STRONG>Ctrl+V</STRONG>) 를 이용해서 상자안에 붙여넣고 <STRONG>OK</STRONG> 를 누르세요.",
DlgPasteSec		: "브러우저 보안 설정으로 인해, 클립보드의 자료를 직접 접근할 수 없습니다. 이 창에 다시 붙여넣기 하십시오.",
DlgPasteIgnoreFont		: "폰트 설정 무시",
DlgPasteRemoveStyles	: "스타일 정의 제거",

// Color Picker
ColorAutomatic	: "기본색상",
ColorMoreColors	: "색상선택...",

// Document Properties
DocProps		: "문서 속성",

// Anchor Dialog
DlgAnchorTitle		: "책갈피 속성",
DlgAnchorName		: "책갈피 이름",
DlgAnchorErrorName	: "책갈피 이름을 입력하십시요.",

// Speller Pages Dialog
DlgSpellNotInDic		: "사전에 없는 단어",
DlgSpellChangeTo		: "변경할 단어",
DlgSpellBtnIgnore		: "건너뜀",
DlgSpellBtnIgnoreAll	: "모두 건너뜀",
DlgSpellBtnReplace		: "변경",
DlgSpellBtnReplaceAll	: "모두 변경",
DlgSpellBtnUndo			: "취소",
DlgSpellNoSuggestions	: "- 추천단어 없음 -",
DlgSpellProgress		: "철자검사를 진행중입니다...",
DlgSpellNoMispell		: "철자검사 완료: 잘못된 철자가 없습니다.",
DlgSpellNoChanges		: "철자검사 완료: 변경된 단어가 없습니다.",
DlgSpellOneChange		: "철자검사 완료: 단어가 변경되었습니다.",
DlgSpellManyChanges		: "철자검사 완료: %1 단어가 변경되었습니다.",

IeSpellDownload			: "철자 검사기가 철치되지 않았습니다. 지금 다운로드하시겠습니까?",

// Button Dialog
DlgButtonText		: "버튼글자(값)",
DlgButtonType		: "버튼종류",
DlgButtonTypeBtn	: "Button",	//MISSING
DlgButtonTypeSbm	: "Submit",	//MISSING
DlgButtonTypeRst	: "Reset",	//MISSING

// Checkbox and Radio Button Dialogs
DlgCheckboxName		: "이름",
DlgCheckboxValue	: "값",
DlgCheckboxSelected	: "선택됨",

// Form Dialog
DlgFormName		: "폼이름",
DlgFormAction	: "실행경로(Action)",
DlgFormMethod	: "방법(Method)",

// Select Field Dialog
DlgSelectName		: "이름",
DlgSelectValue		: "값",
DlgSelectSize		: "세로크기",
DlgSelectLines		: "줄",
DlgSelectChkMulti	: "여러항목 선택 허용",
DlgSelectOpAvail	: "선택옵션",
DlgSelectOpText		: "이름",
DlgSelectOpValue	: "값",
DlgSelectBtnAdd		: "추가",
DlgSelectBtnModify	: "변경",
DlgSelectBtnUp		: "위로",
DlgSelectBtnDown	: "아래로",
DlgSelectBtnSetValue : "선택된것으로 설정",
DlgSelectBtnDelete	: "삭제",

// Textarea Dialog
DlgTextareaName	: "이름",
DlgTextareaCols	: "칸수",
DlgTextareaRows	: "줄수",

// Text Field Dialog
DlgTextName			: "이름",
DlgTextValue		: "값",
DlgTextCharWidth	: "글자 너비",
DlgTextMaxChars		: "최대 글자수",
DlgTextType			: "종류",
DlgTextTypeText		: "문자열",
DlgTextTypePass		: "비밀번호",

// Hidden Field Dialog
DlgHiddenName	: "이름",
DlgHiddenValue	: "값",

// Bulleted List Dialog
BulletedListProp	: "순서없는 목록 속성",
NumberedListProp	: "순서있는 목록 속성",
DlgLstStart			: "Start",	//MISSING
DlgLstType			: "종류",
DlgLstTypeCircle	: "원(Circle)",
DlgLstTypeDisc		: "Disc",	//MISSING
DlgLstTypeSquare	: "네모점(Square)",
DlgLstTypeNumbers	: "번호 (1, 2, 3)",
DlgLstTypeLCase		: "소문자 (a, b, c)",
DlgLstTypeUCase		: "대문자 (A, B, C)",
DlgLstTypeSRoman	: "로마자 수문자 (i, ii, iii)",
DlgLstTypeLRoman	: "로마자 대문자 (I, II, III)",

// Document Properties Dialog
DlgDocGeneralTab	: "일반",
DlgDocBackTab		: "배경",
DlgDocColorsTab		: "색상 및 여백",
DlgDocMetaTab		: "메타데이터",

DlgDocPageTitle		: "페이지명",
DlgDocLangDir		: "문자 쓰기방향",
DlgDocLangDirLTR	: "왼쪽에서 오른쪽 (LTR)",
DlgDocLangDirRTL	: "오른쪽에서 왼쪽 (RTL)",
DlgDocLangCode		: "언어코드",
DlgDocCharSet		: "캐릭터셋 인코딩",
DlgDocCharSetCE		: "Central European",	//MISSING
DlgDocCharSetCT		: "Chinese Traditional (Big5)",	//MISSING
DlgDocCharSetCR		: "Cyrillic",	//MISSING
DlgDocCharSetGR		: "Greek",	//MISSING
DlgDocCharSetJP		: "Japanese",	//MISSING
DlgDocCharSetKR		: "Korean",	//MISSING
DlgDocCharSetTR		: "Turkish",	//MISSING
DlgDocCharSetUN		: "Unicode (UTF-8)",	//MISSING
DlgDocCharSetWE		: "Western European",	//MISSING
DlgDocCharSetOther	: "다른 캐릭터셋 인코딩",

DlgDocDocType		: "문서 헤드",
DlgDocDocTypeOther	: "다른 문서헤드",
DlgDocIncXHTML		: "XHTML 문서정의 포함",
DlgDocBgColor		: "배경색상",
DlgDocBgImage		: "배경이미지 URL",
DlgDocBgNoScroll	: "스크롤되지않는 배경",
DlgDocCText			: "텍스트",
DlgDocCLink			: "링크",
DlgDocCVisited		: "방문한 링크(Visited)",
DlgDocCActive		: "활성화된 링크(Active)",
DlgDocMargins		: "페이지 여백",
DlgDocMaTop			: "위",
DlgDocMaLeft		: "왼쪽",
DlgDocMaRight		: "오른쪽",
DlgDocMaBottom		: "아래",
DlgDocMeIndex		: "문서 키워드 (콤마로 구분)",
DlgDocMeDescr		: "문서 설명",
DlgDocMeAuthor		: "작성자",
DlgDocMeCopy		: "저작권",
DlgDocPreview		: "미리보기",

// Templates Dialog
Templates			: "템플릿",
DlgTemplatesTitle	: "내용 템플릿",
DlgTemplatesSelMsg	: "에디터에서 사용할 템플릿을 선택하십시요.<br>(지금까지 작성된 내용은 사라집니다.):",
DlgTemplatesLoading	: "템플릿 목록을 불러오는중입니다. 잠시만 기다려주십시요.",
DlgTemplatesNoTpl	: "(템플릿이 없습니다.)",
DlgTemplatesReplace	: "현재 내용 바꾸기",

// About Dialog
DlgAboutAboutTab	: "About",
DlgAboutBrowserInfoTab	: "브라우저 정보",
DlgAboutLicenseTab	: "License",	//MISSING
DlgAboutVersion		: "버전",
DlgAboutInfo		: "더 많은 정보를 보시려면 다음 사이트로 가십시오.",

// Div Dialog
DlgDivGeneralTab	: "General",	//MISSING
DlgDivAdvancedTab	: "Advanced",	//MISSING
DlgDivStyle		: "Style",	//MISSING
DlgDivInlineStyle	: "Inline Style",	//MISSING

ScaytTitle			: "SCAYT",	//MISSING
ScaytTitleOptions	: "Options",	//MISSING
ScaytTitleLangs		: "Languages",	//MISSING
ScaytTitleAbout		: "About"	//MISSING
};
