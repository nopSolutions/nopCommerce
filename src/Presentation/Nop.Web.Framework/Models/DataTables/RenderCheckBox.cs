namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents checkbox render for DataTables column
    /// </summary>
    public partial class RenderCheckBox : IRender
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the RenderCheckBox class 
        /// </summary>
        /// <param name="name">Checkbox name</param>
        public RenderCheckBox(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets name checkbox
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}