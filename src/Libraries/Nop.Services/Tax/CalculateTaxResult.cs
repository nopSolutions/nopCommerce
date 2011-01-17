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


using System.Collections.Generic;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Represents a result of tax calculation
    /// </summary>
    public partial class CalculateTaxResult
    {
        public CalculateTaxResult()
        {
            this.Errors = new List<string>();
        }

        /// <summary>
        /// Gets or sets a tax rate
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// Gets or sets an address
        /// </summary>
        public IList<string> Errors { get; set; }

        public bool Success
        {
            get 
            { 
                return this.Errors.Count == 0; 
            }
        }

        public void AddError(string error)
        {
            this.Errors.Add(error);
        }
    }
}
