using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Configuration
{
    public interface IConfiguration<TSettings> where TSettings : ISettings, new() {
        TSettings Settings { get; }
    }
}
