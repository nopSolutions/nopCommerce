//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): planetcloud (http://www.planetcloud.co.uk). 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security {
    public class UserRegistrationResult {
        public User User { get; set; }
        public IList<string> Errors { get; set; }

        public UserRegistrationResult() {
            this.Errors = new List<string>();
        }

        public bool Success {
            get { return (this.User != null) && (this.Errors.Count == 0); }
        }

        public void AddError(string error) {
            this.Errors.Add(error);
        }
    }
}
