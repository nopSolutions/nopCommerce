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
using System.Globalization;
using System.Web.UI.WebControls;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class DecimalTextBox : BaseNopUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            string validChars = NumberFormatInfo.CurrentInfo.NegativeSign;
            validChars += NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;            
            ftbeValue.ValidChars = validChars;
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public decimal Value
        {
            get
            {
                return decimal.Parse(txtValue.Text);
            }
            set
            {
                txtValue.Text = value.ToString("G29");
            }
        }

        public string RequiredErrorMessage
        {
            get
            {
                return rfvValue.ErrorMessage;
            }
            set
            {
                rfvValue.ErrorMessage = value;
            }
        }

        public string RangeErrorMessage
        {
            get
            {
                return rvValue.ErrorMessage;
            }
            set
            {
                rvValue.ErrorMessage = value;
            }
        }

        public string MinimumValue
        {
            get
            {
                return rvValue.MinimumValue;
            }
            set
            {
                rvValue.MinimumValue = value;
            }
        }

        public string MaximumValue
        {
            get
            {
                return rvValue.MaximumValue;
            }
            set
            {
                rvValue.MaximumValue = value;
            }
        }

        public Unit Width
        {
            get
            {
                return txtValue.Width;
            }
            set
            {
                txtValue.Width = value;
            }
        }

        public string ValidationGroup
        {
            get
            {
                return rfvValue.ValidationGroup;
            }
            set
            {
                txtValue.ValidationGroup = value;
                rfvValue.ValidationGroup = value;
                rvValue.ValidationGroup = value;
            }
        }

        public string CssClass
        {
            get
            {
                return txtValue.CssClass;
            }
            set
            {
                txtValue.CssClass = value;
            }
        }
    }
}
