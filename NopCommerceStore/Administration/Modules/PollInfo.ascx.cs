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
using NopSolutions.NopCommerce.BusinessLogic.Content.Polls;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class PollInfoControl : BaseNopAdministrationUserControl
    {
        private void FillDropDowns()
        {
            this.ddlLanguage.Items.Clear();
            var languages = LanguageManager.GetAllLanguages();
            foreach (Language language in languages)
            {
                ListItem item2 = new ListItem(language.Name, language.LanguageId.ToString());
                this.ddlLanguage.Items.Add(item2);
            }
        }

        private void BindData()
        {
            Poll poll = PollManager.GetPollById(this.PollId);
            if (poll != null)
            {
                CommonHelper.SelectListItem(this.ddlLanguage, poll.LanguageId);
                this.txtName.Text = poll.Name;
                this.txtSystemKeyword.Text = poll.SystemKeyword;
                this.cbPublished.Checked = poll.Published;
                this.cbShowOnHomePage.Checked = poll.ShowOnHomePage;
                if (poll.StartDate.HasValue)
                {
                    this.ctrlStartDate.SelectedDate = poll.StartDate;
                }
                if (poll.EndDate.HasValue)
                {
                    this.ctrlEndDate.SelectedDate = poll.EndDate;
                }
                this.txtDisplayOrder.Value = poll.DisplayOrder;

                pnlPollAnswers.Visible = true;
                var pollAnswers = poll.PollAnswers;
                gvPollAnswers.DataSource = pollAnswers;
                gvPollAnswers.DataBind();
            }
            else
            {
                pnlPollAnswers.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
            }
        }

        protected void btnAddPollAnswer_Click(object sender, EventArgs e)
        {
            try
            {
                PollAnswer pollAnswer = PollManager.InsertPollAnswer(this.PollId,
                    txtPollAnswerName.Text, 0, txtPollAnswerDisplayOrder.Value);

                BindData();
            }
            catch (Exception exc)
            {
                processAjaxError(exc);
            }
        }

        protected void gvPollAnswers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdatePollAnswer")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvPollAnswers.Rows[index];

                HiddenField hfPollAnswerId = row.FindControl("hfPollAnswerId") as HiddenField;
                SimpleTextBox txtName = row.FindControl("txtName") as SimpleTextBox;
                NumericTextBox txtDisplayOrder = row.FindControl("txtDisplayOrder") as NumericTextBox;

                int pollAnswerId = int.Parse(hfPollAnswerId.Value);
                PollAnswer pollAnswer = PollManager.GetPollAnswerById(pollAnswerId);

                if (pollAnswer != null)
                    pollAnswer = PollManager.UpdatePollAnswer(pollAnswer.PollAnswerId, pollAnswer.PollId,
                       txtName.Text, pollAnswer.Count, txtDisplayOrder.Value);

                BindData();
            }
        }

        protected void gvPollAnswers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                PollAnswer pollAnswer = (PollAnswer)e.Row.DataItem;

                Button btnUpdate = e.Row.FindControl("btnUpdate") as Button;
                if (btnUpdate != null)
                    btnUpdate.CommandArgument = e.Row.RowIndex.ToString();
            }
        }

        protected void gvPollAnswers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int pollAnswerId = (int)gvPollAnswers.DataKeys[e.RowIndex]["PollAnswerId"];
            PollAnswer pollAnswer = PollManager.GetPollAnswerById(pollAnswerId);
            if (pollAnswer != null)
            {
                PollManager.DeletePollAnswer(pollAnswer.PollAnswerId);
                BindData();
            }
        }

        protected void processAjaxError(Exception exc)
        {
            ProcessException(exc, false);
            pnlError.Visible = true;
            lErrorTitle.Text = exc.Message;
        }

        public Poll SaveInfo()
        {
            Poll poll = PollManager.GetPollById(this.PollId);
            DateTime? startDate = ctrlStartDate.SelectedDate;
            DateTime? endDate = ctrlEndDate.SelectedDate;
            if (startDate.HasValue)
            {
                startDate = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
            }
            if (endDate.HasValue)
            {
                endDate = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
            }

            if (poll != null)
            {
                poll = PollManager.UpdatePoll(poll.PollId, int.Parse(this.ddlLanguage.SelectedItem.Value),
                    txtName.Text, txtSystemKeyword.Text, cbPublished.Checked, cbShowOnHomePage.Checked, txtDisplayOrder.Value, startDate, endDate);
            }
            else
            {
                poll = PollManager.InsertPoll(int.Parse(this.ddlLanguage.SelectedItem.Value),
                    txtName.Text, txtSystemKeyword.Text, cbPublished.Checked, cbShowOnHomePage.Checked, txtDisplayOrder.Value, startDate, endDate);
            }
            return poll;
        }

        public int PollId
        {
            get
            {
                return CommonHelper.QueryStringInt("PollId");
            }
        }
    }
}