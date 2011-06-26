namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Represents a read table mode for ExcelHelper.ReadTable method
    /// </summary>
    public enum ExcelHelperReadTableMode
    {
        /// <summary>
        /// Read rows from all filled excel worksheet cells
        /// </summary>
        ReadFromWorkSheet,
        /// <summary>
        /// Read rows only from named range
        /// </summary>
        ReadFromNamedRange
    }
}
