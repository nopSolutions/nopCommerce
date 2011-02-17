
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Directory
{
    public class MeasureSettings : ISettings
    {
        public int BaseDimensionId { get; set; }
        public int BaseWeightId { get; set; }
    }
}