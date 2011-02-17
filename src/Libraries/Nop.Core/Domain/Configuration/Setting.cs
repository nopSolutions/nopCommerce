
namespace Nop.Core.Domain.Configuration
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    public partial class Setting : BaseEntity
    {
        public Setting() { }
        
        public Setting(string name, string value, string description = "") {
            this.Name = name;
            this.Value = value;
            this.Description = description;
        }
        
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns the setting value as the specified type
        /// </summary>
        public T As<T>() {
            return CommonHelper.To<T>(this.Value);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
