namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represets checkbox render for DataTables column
    /// </summary>
    public partial class RenderCheckBox : Render
    {
        public RenderCheckBox(string name)
        {
            this.Type = RenderType.Checkbox;
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets name checkbox
        /// </summary>
        public string Name { get; set; }
    }
}
