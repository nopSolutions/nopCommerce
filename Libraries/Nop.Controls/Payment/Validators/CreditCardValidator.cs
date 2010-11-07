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
// Contributor(s): Bruce Leggett. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace NopSolutions.NopCommerce.Controls.Payment.Validators
{
    /// <summary>
    /// Represents a credit card validator
    /// </summary>
    public partial class CreditCardValidator : BaseValidator
    {
        #region Methods
        /// <summary>
        /// Determines whether the value in the credit card control is valid
        /// </summary>
        /// <returns>true if the value in the input control is valid; otherwise, false.</returns>
        protected override bool EvaluateIsValid()
        {
            string valueToValidate = base.GetControlValidationValue(base.ControlToValidate).Trim();
            int indicator = 1;
            int firstNumToAdd = 0;
            int secondNumToAdd = 0;
            char[] ccArr = valueToValidate.ToCharArray();
            for (int i = ccArr.Length - 1; i >= 0; i--)
            {
                char ccNoAdd = ccArr[i];
                int ccAdd = 0;
                if (!int.TryParse(ccNoAdd.ToString(), out ccAdd))
                {
                    return false;
                }
                if (indicator == 1)
                {
                    firstNumToAdd += ccAdd;
                    indicator = 0;
                }
                else
                {
                    if ((ccAdd + ccAdd) >= 10)
                    {
                        int temporary = ccAdd + ccAdd;
                        string num1 = temporary.ToString().Substring(0, 1);
                        string num2 = temporary.ToString().Substring(1, 1);
                        secondNumToAdd += Convert.ToInt32(num1) + Convert.ToInt32(num2);
                    }
                    else
                    {
                        secondNumToAdd += ccAdd + ccAdd;
                    }
                    indicator = 1;
                }
            }
            return (((firstNumToAdd + secondNumToAdd) % 10) == 0);
        }
        #endregion
    }
}
