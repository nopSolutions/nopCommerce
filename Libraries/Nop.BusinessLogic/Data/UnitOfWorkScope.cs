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
// Contributor(s): Jordan Van Gogh_______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Text;
using System.Threading;


namespace NopSolutions.NopCommerce.BusinessLogic.Data
{
    /// <summary>
    /// Defines a scope for a business transaction. At the end of the scope all object changes
    /// can be persisted to the underlying datastore. Instances of this class are supposed to be 
    /// used in a using() statement.
    /// </summary>
    public sealed class UnitOfWorkScope : ObjectContextScope<NopObjectContext>
    {
        /// <summary>
        /// Default constructor. Object changes are not automatically saved at the 
        /// end of the scope.
        /// </summary>
        public UnitOfWorkScope()
            : base()
        { }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="saveAllChangesAtEndOfScope">
        /// A boolean value that indicates whether to automatically save 
        /// all object changes at end of the scope.
        /// </param>
        public UnitOfWorkScope(bool saveAllChangesAtEndOfScope)
            : base(saveAllChangesAtEndOfScope)
        { }
    }
}
