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
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Objects;
using System.Diagnostics;

namespace NopSolutions.NopCommerce.BusinessLogic.Data
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Determines whether record is attached
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="entity">Entity</param>
        /// <returns>Result</returns>
        public static bool IsAttached(this ObjectContext context, object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            ObjectStateEntry entry;
            try
            {
                entry = context.ObjectStateManager.GetObjectStateEntry(entity);
                return (entry.State != EntityState.Detached);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.ToString());
            }
            return false;
        }
    }
}
