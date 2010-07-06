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
    public sealed class ScopedObjectContextManager<T> : ObjectContextManager<T> where T : ObjectContext, new()
    {
        private T _objectContext;

        /// <summary>
        /// Returns the ObjectContext instance that belongs to the current ObjectContextScope.
        /// If currently no ObjectContextScope exists, a local instance of an ObjectContext 
        /// class is returned.
        /// </summary>
        public override T ObjectContext
        {
            get
            {
                if (ObjectContextScope<T>.CurrentObjectContext != null)
                    return ObjectContextScope<T>.CurrentObjectContext;
                else
                {
                    if (_objectContext == null)
                        _objectContext = new T();

                    return _objectContext;
                }
            }
        }
    }
}
