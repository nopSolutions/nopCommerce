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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Content.Polls;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class PollBlockControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected void BindData()
        {
            //get poll
            var poll = IoCFactory.Resolve<IPollService>().GetPollById(this.PollId);
            if (poll == null && !String.IsNullOrEmpty(this.SystemKeyword))
                poll = IoCFactory.Resolve<IPollService>().GetPollBySystemKeyword(this.SystemKeyword);

            this.Visible = poll != null && poll.Published;
        }

        public int PollId
        {
            get
            {
                return this.ctrlPoll.PollId;
            }
            set
            {
                this.ctrlPoll.PollId = value;
            }
        }

        public string SystemKeyword
        {
            get
            {
                return this.ctrlPoll.SystemKeyword;
            }
            set
            {
                this.ctrlPoll.SystemKeyword = value;
            }
        }
    }
}