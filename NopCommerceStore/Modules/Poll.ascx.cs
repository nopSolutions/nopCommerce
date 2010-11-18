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
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Polls;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class PollControl: BaseNopFrontendUserControl
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
            var poll = GetPoll();
            if (poll != null && poll.Published)
            {
                //bind data
                rfvPollAnswers.ValidationGroup = string.Format("Poll.{0}.{1}", poll.PollId, this.ClientID);
                btnSubmitVoteRecord.ValidationGroup = string.Format("Poll.{0}.{1}", poll.PollId, this.ClientID);

                //info
                lblPollName.Text = Server.HtmlEncode(poll.Name);
                lblTotalVotes.Text = string.Format(GetLocaleResourceString("Polls.TotalVotes"), poll.TotalVotes);

                //has customer already voted?
                var customer = NopContext.Current.User;
                bool showResults = false;
                if (customer != null)
                {
                    showResults = this.PollService.PollVotingRecordExists(poll.PollId, customer.CustomerId);
                }

                //bind info (answers or results)
                var pollAnswers = poll.PollAnswers;
                pnlTakePoll.Visible = !showResults;
                pnlPollResults.Visible = showResults;
                if (showResults)
                {
                    //bind results
                    dlResults.DataSource = pollAnswers;
                    dlResults.DataBind();
                }
                else
                {
                    //bind answers
                    rblPollAnswers.DataSource = pollAnswers;
                    rblPollAnswers.DataBind();
                }
            }
            else
            {
                //poll is not loaded
                pnlTakePoll.Visible = false;
                pnlPollResults.Visible = false;
            }
        }

        protected void btnSubmitVoteRecord_Click(object sender, EventArgs e)
        {
            if (rblPollAnswers.SelectedItem == null)
                return;

            //create anonymous user if required
            if (NopContext.Current.User == null && this.SettingManager.GetSettingValueBoolean("Common.AllowAnonymousUsersToVotePolls"))
            {
                this.CustomerService.CreateAnonymousUser();
            }

            if (NopContext.Current.User == null || (NopContext.Current.User.IsGuest && !this.SettingManager.GetSettingValueBoolean("Common.AllowAnonymousUsersToVotePolls")))
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            var poll = GetPoll();
            if (poll != null && poll.Published)
            {
                if (!this.PollService.PollVotingRecordExists(poll.PollId, NopContext.Current.User.CustomerId))
                {
                    int pollAnswerId = Convert.ToInt32(rblPollAnswers.SelectedItem.Value);
                    this.PollService.CreatePollVotingRecord(pollAnswerId, NopContext.Current.User.CustomerId);
                }
            }
            BindData();
        }

        protected void dlResults_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            //var imgPercentage = (Image)e.Item.FindControl("imgPercentage");
            var lPercentage = (Literal)e.Item.FindControl("lPercentage");

            var poll = GetPoll();
            if (poll != null)
            {
                int pollAnswerVoteCount = (int)DataBinder.Eval(e.Item.DataItem, "Count");
                int pollTotalVoteCount = poll.TotalVotes;
                if (pollTotalVoteCount > 0)
                {
                    double pct = (Convert.ToDouble(pollAnswerVoteCount) / Convert.ToDouble(pollTotalVoteCount)) * Convert.ToDouble(100);
                    lPercentage.Text = pct.ToString("0.0") + "%";
                    //imgPercentage.Width = Unit.Percentage(pct);
                }
                else
                {
                    lPercentage.Text = "0%";
                    //imgPercentage.Visible = false;
                }
            }
        }

        Poll _pollCached = null;
        public Poll GetPoll()
        {
            if (_pollCached == null)
            {
                _pollCached = this.PollService.GetPollById(this.PollId);
                if (_pollCached == null && !String.IsNullOrEmpty(this.SystemKeyword))
                    _pollCached = this.PollService.GetPollBySystemKeyword(this.SystemKeyword);
            }
            return _pollCached;
        }

        public int PollId
        {
            get
            {
                if (ViewState["PollId"] == null)
                    return -1;
                else
                    return (int)ViewState["PollId"];
            }
            set { ViewState["PollId"] = value; }
        }

        public string SystemKeyword
        {
            get
            {
                object obj2 = this.ViewState["SystemKeyword"];
                if (obj2 != null)
                    return (string)obj2;
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["SystemKeyword"] = value;
            }
        }
    }
}
