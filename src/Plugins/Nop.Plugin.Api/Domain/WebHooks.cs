using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Api.Domain
{
    public class WebHooks : BaseEntity
    {
        public string User { get; set; }

        public string Id { get; set; }

        public string ProtectedData { get; set; }

        public Byte[] RowVer { get; set; }
    }
}
