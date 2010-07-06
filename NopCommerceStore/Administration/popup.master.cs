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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration
{
    public partial class popupMaster : BaseNopAdministrationMasterPage
    {
        public override void ShowMessage(string message)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = "messageBox messageBoxSuccess";
            lMessage.Text = message;
        }

        public override void ShowError(string message, string completeMessage)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = "messageBox messageBoxError";
            lMessage.Text = message;
            lMessageComplete.Text = completeMessage;
        }
    }
}