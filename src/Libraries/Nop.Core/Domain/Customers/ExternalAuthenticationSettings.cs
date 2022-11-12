<<<<<<< HEAD
<<<<<<< HEAD
﻿using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// External authentication settings
    /// </summary>
    public partial class ExternalAuthenticationSettings : ISettings
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalAuthenticationSettings()
        {
            ActiveAuthenticationMethodSystemNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether email validation is required.
        /// In most cases we can skip email validation for Facebook or any other third-party external authentication plugins. I guess we can trust  Facebook for the validation.
        /// </summary>
        public bool RequireEmailValidation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need to logging errors on authentication process 
        /// </summary>
        public bool LogErrors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to remove external authentication associations
        /// </summary>
        public bool AllowCustomersToRemoveAssociations { get; set; }

        /// <summary>
        /// Gets or sets system names of active authentication methods
        /// </summary>
        public List<string> ActiveAuthenticationMethodSystemNames { get; set; }
    }
}
=======
=======
=======
<<<<<<< HEAD
﻿using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// External authentication settings
    /// </summary>
    public partial class ExternalAuthenticationSettings : ISettings
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalAuthenticationSettings()
        {
            ActiveAuthenticationMethodSystemNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether email validation is required.
        /// In most cases we can skip email validation for Facebook or any other third-party external authentication plugins. I guess we can trust  Facebook for the validation.
        /// </summary>
        public bool RequireEmailValidation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need to logging errors on authentication process 
        /// </summary>
        public bool LogErrors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to remove external authentication associations
        /// </summary>
        public bool AllowCustomersToRemoveAssociations { get; set; }

        /// <summary>
        /// Gets or sets system names of active authentication methods
        /// </summary>
        public List<string> ActiveAuthenticationMethodSystemNames { get; set; }
    }
}
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// External authentication settings
    /// </summary>
    public partial class ExternalAuthenticationSettings : ISettings
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalAuthenticationSettings()
        {
            ActiveAuthenticationMethodSystemNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether email validation is required.
        /// In most cases we can skip email validation for Facebook or any other third-party external authentication plugins. I guess we can trust  Facebook for the validation.
        /// </summary>
        public bool RequireEmailValidation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need to logging errors on authentication process 
        /// </summary>
        public bool LogErrors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to remove external authentication associations
        /// </summary>
        public bool AllowCustomersToRemoveAssociations { get; set; }

        /// <summary>
        /// Gets or sets system names of active authentication methods
        /// </summary>
        public List<string> ActiveAuthenticationMethodSystemNames { get; set; }
    }
}
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
