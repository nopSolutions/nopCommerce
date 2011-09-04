
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    public class AdminAreaSettings : ISettings
    {
        public int GridPageSize { get; set; }

        public bool DisplayProductPictures { get; set; }
    }
}