using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Security {
    public enum EncryptionFormat {
        TripleDes = 0,
        Aes128 = 1,
        Aes256 = 2
    }
}
