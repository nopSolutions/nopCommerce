EPPlus 4.0.3

Visit epplus.codeplex.com for the latest information

EPPlus-Create Advanced Excel spreadsheet.

New features

Replaced Packaging API with DotNetZip
* This will remove any problems with Isolated Storage and enable multi threading


New Cell store
* Less memory consumtion
* Insert columns (not on the range level)
* Faster row inserts,

Formula Parser
* Calculates all formulas in a workbook, a worksheet or in a specified range
* 100+ functions implemented
* Access via Calculate methods on Workbook, Worksheet and Range objects.
* Add custom/missing Excel functions via Workbook.FormulaParserManager.
* Samples added to the EPPlusSamples project.

The formula parser does not support Array Formulas
* Intersect operator (Space)
* References to external workbooks
* And probably a whole lot of other stuff as well :)

Perfomance
*Of course the performance of the formula parser is nowhere near Excels.Our focus has been functionality.

Agile Encryption (Office 2012-)
* Support for newer type of encryption.

Minor new features
* Chart worksheets
* New Chart Types Bubblecharts
* Radar Charts
* Area Charts
* And lots of bugfixes...

Beta 2 Changes
* Fixed bug when using RepeatColumns & RepeatRows at the same time.
* VBA project will be left untouched if its not accessed.
* Fixed problem with strings on save.
* Added locks to the cellstore for access by mulitple threads.
* Implemented Indirect function
* Used DisplayNameAttribute to generate column headers from LoadFromCollection
* Rewrote ExcelRangeBase.Copy function. 
* Added caching to Save ZipStream for Cells and shared strings to speed up the Save method.
* Added Missing InsertColumn and DeleteColumn
* Added pull request to support Date1904 
* Added pull request ExcelWorksheet.LoadFromDataReader

Release Candidare changes
* Fixed some problems with Range.Copy Function
* InsertColumn and Delete column didn't work in some cases
* Chart.DisplayBlankAs had the wrong default type in Excel 2010+
* Datavalidation list overflow cauesed corruption of the package
* Fixed a few Calculation when refering ranges (for example If)  function and some 
* Added ChartAxis.DisplayUnit
* Fixed a bug related to shared formulas
* Named styles faild in some cases.
* Style.Indent got an invalid value in some cases.
* Fixed a problem with AutofitColumns method.
* Performance fix.
* An a whole lot of other small fixes.

4.0.1 Fixes
* VBA unreadable content
* Fixed a few issues with InsertRow and DeleteRow
* Fixed bug in Average and AverageA 
* Handling of Div/0 in functions
* Fixed VBA CodeModule error when copying a worksheet.
* Value decoding when reading str element for cell value.
* Better exception when accessing a worksheet out of range in the Excelworksheets indexer.
* Added Small and Large function to formula parser. Performance fix when encountering an unknown function.
* Fixed handling strings in formulas
* Calculate hanges if formula start with a parenthes.
* Worksheet.Dimension returned an invalid range in some cases.
* Rowheight was wrong in some cases.
* ExcelSeries.Header had an incorrect validation check.

4.0.2 Fixes
* Fixes a whole bunch of bugs related to the cell store (Worksheet.InsertColumn, Worksheet.InsertRow, Worksheet.DeleteColumn, Worksheet.DeleteRow, Range.Copy, Range.Clear)
* Added functions Acos, Acosh, Asinh, Atanh, Atan, CountBlank, CountIfs, Mina, Offset, Median, Hyperlink, Rept
* Fix for reading Excel comment content from the t-element.
* Fix to make Range.LoadFromCollection work better with inheritence
* And alot of other smal fixes

4.0.3 Fixes
* Added compilation directive for MONO (Thanks Danny)
* Added functions IfError, Char, Error.Type, Degrees, Fixed, IsNonText, IfNa and SumIfs
* And fixed a lot of issues. See http://epplus.codeplex.com/SourceControl/list/changesets for more details