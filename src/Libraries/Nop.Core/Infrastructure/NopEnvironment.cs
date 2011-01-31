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
// Contributor(s): dmitryr1 (http://blogs.msdn.com/b/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)_______. 
//------------------------------------------------------------------------------

using System.Web;

namespace Nop.Core.Infrastructure
{
    public class NopEnvironment
    {     

        private static AspNetHostingPermissionLevel? _trustLevel = null;
        /// <summary>
        /// Returns the trust level for the current environment
        /// </summary>
        public static AspNetHostingPermissionLevel ApplicationTrustLevel
        {
            get
            {
                if (!_trustLevel.HasValue)
                {
                    //set minimum
                    _trustLevel = AspNetHostingPermissionLevel.None;

                    //determine maximum
                    foreach (AspNetHostingPermissionLevel trustLevel in
                            new AspNetHostingPermissionLevel[] {
                                AspNetHostingPermissionLevel.Unrestricted,
                                AspNetHostingPermissionLevel.High,
                                AspNetHostingPermissionLevel.Medium,
                                AspNetHostingPermissionLevel.Low,
                                AspNetHostingPermissionLevel.Minimal 
                            })
                    {
                        try
                        {
                            new AspNetHostingPermission(trustLevel).Demand();
                            _trustLevel = trustLevel;
                            break; //we've set the highest permission we can
                        }
                        catch (System.Security.SecurityException)
                        {
                            continue;
                        }
                    }
                }
                return _trustLevel.Value;
            }
        }



    }
}
