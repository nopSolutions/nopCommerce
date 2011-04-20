using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Localization;

namespace Nop.Web.Framework.Localization
{
    public interface ILocalizedModelLocal
    {
        int LanguageId { get; set; }
    }
}
