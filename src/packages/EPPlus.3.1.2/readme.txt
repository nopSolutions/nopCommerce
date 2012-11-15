EPPlus 3.1.2

Visit epplus.codeplex.com for the latest information

This version contain version contains two new major features - VBA and conditional formatting

New features
 
VBA
 Create,read and write a VBA project from scratch or in a template.
 Add and modify modules and classes.
 Read and write code to documents(workbook and worksheets), modules and classes.
 VBA protection
 Code signing
 And more...
 
Conditional Formatting
 Apply conditional formatting on any range of cells.
 Choose between more than 40 different types ("beginsWith", "between", "containsText", "endsWith", "equal", "greaterThan", "greaterThanOrEqual", "iconSet3", "iconSet4", "iconSet5", "last7Days", "lastMonth" and many more)

Minor new features
Picture Hyperlinks
Chart Axis Titles
Remove method added to Drawings collection

An a lot of bug fixes

3.1.2 changes
* Fixes problem with messed up Normal style if the Normal style is not the first saved Style in the NamedStyles collection.
* Fixed problem with CellIs in templates (14750, 14749).Turned off dtd on XmlDocument.Load / LoadXml to avoid Xml-bombs. Fixed pivottable constructor that set default values in templates(14723).
* Fixes problem with indexed colors in conditional formatting.
* Fixes full row / column ranges with defined names (throwing exception when accessing Start and End properties).
