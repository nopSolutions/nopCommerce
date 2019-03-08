namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents type of Render
    /// </summary>
    public enum RenderType
    {
        /// <summary>
        /// Render column as check box
        /// </summary>
        Checkbox,
        /// <summary>
        /// Render column as date
        /// </summary>
        Date,
        /// <summary>
        /// Render column as link
        /// </summary>
        Link,
        /// <summary>
        /// Render column as custom button in each cells
        /// </summary>
        Button,
        /// <summary>
        /// Render column as edit button in each cells
        /// </summary>
        ButtonEdit,
        /// <summary>
        /// Render column as picture
        /// </summary>
        Picture,
        /// <summary>
        /// Render column as boolean
        /// </summary>
        Boolean,
        /// <summary>
        /// Custom render column
        /// </summary>
        Custom
    }
}
