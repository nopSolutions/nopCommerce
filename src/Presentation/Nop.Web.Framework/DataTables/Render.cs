namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represents render (process) the data for use in the DataTables.
    /// </summary>
    public abstract class Render
    {
        public RenderType Type { get; set; }
    };

    public enum RenderType
    {
        Checkbox,
        Date,
        Link,
        Button,
        ButtonEdit,
        Picture,
        Boolean,
        Custom
    }
}
