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
        /// <param name="propertyKeyName">Property key name ("Id" by default). This property must be defined in the row dataset.</param>
        public RenderCheckBox(string name, string propertyKeyName = "Id")
        {
            Name = name;
            PropertyKeyName = propertyKeyName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets name checkbox
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets identificator for row 
        /// </summary>
        public string PropertyKeyName { get; set; }

        #endregion
    }
}