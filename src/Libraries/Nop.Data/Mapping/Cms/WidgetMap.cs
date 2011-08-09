using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Cms;

namespace Nop.Data.Mapping.Cms
{
    public partial class WidgetMap : EntityTypeConfiguration<Widget>
    {
        public WidgetMap()
        {
            this.ToTable("Widget");
            this.HasKey(pv => pv.Id);
            this.Property(pv => pv.PluginSystemName).IsRequired().IsMaxLength();

            this.Ignore(pv => pv.WidgetZone);
        }
    }
}