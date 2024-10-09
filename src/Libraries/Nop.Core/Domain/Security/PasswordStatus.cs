using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Security;

public enum PasswordStatus
{
    Valid = 1,
    Expired,
    NeedToBeChanged,
    MustToBeChanged
}
