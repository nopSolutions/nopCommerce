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
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class SimpleTextBox : BaseNopUserControl
    {
        public string Text
        {
            get
            {
                return txtValue.Text;
            }
            set
            {
                txtValue.Text = value;
            }
        }

        public string ErrorMessage
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

        public bool Enabled
        {
            get
            {
                return txtValue.Enabled;
            }
            set
            {
                txtValue.Enabled = value;
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

        public Unit Height
        {
            get
            {
                return txtValue.Height;
            }
            set
            {
                txtValue.Height = value;
            }
        }

        public TextBoxMode TextMode
        {
            get
            {
                return txtValue.TextMode;
            }
            set
            {
                txtValue.TextMode = value;
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
            }
        }

        /// <summary>
        ///  Gets or sets the skin to apply to the control.
        /// </summary>
        public string SkinID
        {
            get
            {
                return txtValue.SkinID;
            }
            set
            {
                txtValue.SkinID = value;
            }
        }

        public AutoCompleteType AutoCompleteType
        {
            get
            {
                return txtValue.AutoCompleteType;
            }
            set
            {
                txtValue.AutoCompleteType = value;
            }
        }
    }
}
