using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Security {
    public enum HashFormat {
        Sha1 = 0,
        Sha256 = 1,
        Sha512 = 2
    }
}
