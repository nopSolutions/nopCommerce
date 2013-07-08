EPPlus 3.1.3

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

3.1.3 changes
* Fixed VBA project corruption when adding a new worksheet. 
* Fixed Drawing resizing with hidden rows.
* Fixed problem with named styles and blank text property on range when it shold be "0"
* Fixes issue 14768-Missing Normal style causes corruption of xfs styles.
* Fixed issue 14799-Table-StyleName bug.
* Fixed issue 14799-Change of contenttype from xlsx to xlsm and pivottable caused corruption
* Fixed issue 14795-Scatterchart smooth lines.
* Fixed issue 14809-Namedstyles was invalid when loading from a template
* Fixed issue 14805-LoadFromText did not work with a null argument for parameter Text)
* Fixed issue 14819-Setting data validations default operator to Any.
* Fixed issue 14823-Invalid hyperlinks when uri was encoded.
* Fixed issue 14818-attribute createdVersion of the Pivottable was set to 1 and causing the pivottable ignore the enableDrill and showDrill attributes
* Fixed issue 14803-Duplicate column names when creating a table caused a corrupt workbook.
* Fixed issue 14820-Invalid sort order on header footer elements when creating from a template. Added reference to System.Web for Uri decoding
