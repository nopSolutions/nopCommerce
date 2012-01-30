
namespace Nop.Core.Domain.Cms
{
    /// <summary>
    /// Represents a widget
    /// </summary>
    public partial class Widget : BaseEntity
    {
        /// <summary>
        /// Gets or sets the widget zone identifier
        /// </summary>
        public virtual int WidgetZoneId { get; set; }

        /// <summary>
        /// Gets or sets the widget plugin system name
        /// </summary>
        public virtual string PluginSystemName { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }
        
        /// <summary>
        /// Gets or sets the widget zone
        /// </summary>
        public virtual WidgetZone WidgetZone
        {
            get
            {
                return (WidgetZone)this.WidgetZoneId;
            }
            set
            {
                this.WidgetZoneId = (int)value;
            }
        }
    }
}
