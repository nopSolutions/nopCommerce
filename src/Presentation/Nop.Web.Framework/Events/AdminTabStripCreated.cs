<<<<<<< HEAD
<<<<<<< HEAD
﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Admin tabstrip created event
    /// </summary>
    public partial class AdminTabStripCreated
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="helper">HTML Helper</param>
        /// <param name="tabStripName">Tabstrip name</param>
        public AdminTabStripCreated(IHtmlHelper helper, string tabStripName)
        {
            Helper = helper;
            TabStripName = tabStripName;
            BlocksToRender = new List<IHtmlContent>();
        }

        /// <summary>
        /// HTML Helper
        /// </summary>
        public IHtmlHelper Helper { get; private set; }
        /// <summary>
        /// TabStripName
        /// </summary>
        public string TabStripName { get; private set; }
        /// <summary>
        /// Blocks to render
        /// </summary>
        public IList<IHtmlContent> BlocksToRender { get; set; }
    }
}
=======
=======
=======
<<<<<<< HEAD
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Admin tabstrip created event
    /// </summary>
    public partial class AdminTabStripCreated
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="helper">HTML Helper</param>
        /// <param name="tabStripName">Tabstrip name</param>
        public AdminTabStripCreated(IHtmlHelper helper, string tabStripName)
        {
            Helper = helper;
            TabStripName = tabStripName;
            BlocksToRender = new List<IHtmlContent>();
        }

        /// <summary>
        /// HTML Helper
        /// </summary>
        public IHtmlHelper Helper { get; private set; }
        /// <summary>
        /// TabStripName
        /// </summary>
        public string TabStripName { get; private set; }
        /// <summary>
        /// Blocks to render
        /// </summary>
        public IList<IHtmlContent> BlocksToRender { get; set; }
    }
}
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Admin tabstrip created event
    /// </summary>
    public partial class AdminTabStripCreated
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="helper">HTML Helper</param>
        /// <param name="tabStripName">Tabstrip name</param>
        public AdminTabStripCreated(IHtmlHelper helper, string tabStripName)
        {
            Helper = helper;
            TabStripName = tabStripName;
            BlocksToRender = new List<IHtmlContent>();
        }

        /// <summary>
        /// HTML Helper
        /// </summary>
        public IHtmlHelper Helper { get; private set; }
        /// <summary>
        /// TabStripName
        /// </summary>
        public string TabStripName { get; private set; }
        /// <summary>
        /// Blocks to render
        /// </summary>
        public IList<IHtmlContent> BlocksToRender { get; set; }
    }
}
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
