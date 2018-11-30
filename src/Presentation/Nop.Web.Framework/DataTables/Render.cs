namespace Nop.Web.Framework.DataTables
{
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
