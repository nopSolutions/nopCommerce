EPPlus 3.0

Visit epplus.codeplex.com for the latest information

What's new?
Ok, so this version actually don't contain any new major functionallity. The major change is the version that goes from GPL --> LGPL.
Theres also whole bunch of bugfixes since 2.9
Since I have rewritten a lot of stuff, I also took the opportunity to change a few things in the interface and behavior.

LGPL
This version changes the version from GPL to LGPL (I have rewritten a lot of old stuff from the Excelpackage project to make this possible)

* Backgroundimages
Backgroundimage property added to ExcelWorksheet class. 

* Header and footer images
 Pictures collection added to ExcelHeaderFooter class

* Gradiant Cell Backgrounds fills
Gradient property added to the ExcelFill class

Interface changes
Renamed properties ExcelWorksheet...
* defaultColWidth --> DefaultColWidth 
* defaultRowHeight --> DefaultRowHeight 

Renamed Properties ExcelHeaderFooter...
* oddHeader -->OddHeader 
* oddFooter --> OddFooter 
* evenHeader --> EvenHeader 
* evenFooter --> EvenFooter 
* firstHeader --> FirstHeader 
* firstFooter --> FirstFooter 

Behavior change

* Cells collection
	Using the cells collection without the indexer didn't work that well in the previous versions. 
	From version 3.0, using the Cells collection without the indexer will reference all cells.
	Setting values, formulas etc in this matter will throw an Exception
* Resizing of drawings when column width and row height are changed
	Drawings (charts, shapes and pictures) will now size when row height and column width are changed, depending on the EditAs property. This behavior can be turned of by setting the ExcelPackage.DoAdjustDrawings to false
* Cells.Value property return an array if more than one cell is specified in the indexer.

New Properties and Classes
* ToolTip property added to ExcelHyperlink
* LockText property added to ExcelShape
* ExcelLineChartSeries class added (to enable change of markers).
* MajorTickMark and MinorTickMark added to ExcelChartAxis (Thanks Mario)
* CalculatedColumnFormula added to ExcelTableColumn (Thanks Maverick)
* SubTotalFunctions property added to ExcelPivotTableField
* UnderLineStyle added to ExcelFont
* ShowFilter added to ExcelTable