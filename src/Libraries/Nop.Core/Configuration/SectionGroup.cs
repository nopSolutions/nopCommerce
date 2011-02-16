using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Nop.Core.Configuration
{
    public class SectionGroup : ConfigurationSectionGroup
    {
        public EngineSection Engine
        {
            get { return (EngineSection)Sections["engine"]; }
        }
    }
}
