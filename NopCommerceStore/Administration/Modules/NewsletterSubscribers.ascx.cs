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
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class NewsletterSubscribersControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected List<NewsLetterSubscription> GetNewsletterSubscribers()
        {
            string email = txtEmail.Text.Trim();
            var newsLetterSubscriptions = MessageManager.GetAllNewsLetterSubscriptions(email, true);
            return newsLetterSubscriptions;
        }

        protected void BindGrid()
        {
            var newsLetterSubscriptions = GetNewsletterSubscribers();
            gvNewsletterSubscribers.DataSource = newsLetterSubscriptions;
            gvNewsletterSubscribers.DataBind();
        }

        protected void btnExportCVS_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string fileName = String.Format("newsletter_emails_{0}_{1}.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));

                    StringBuilder sb = new StringBuilder();
                    var newsLetterSubscriptions = GetNewsletterSubscribers();
                    if (newsLetterSubscriptions.Count == 0)
                    {
                        throw new NopException("No emails to export");
                    }
                    for (int i = 0; i < newsLetterSubscriptions.Count; i++)
                    {
                        var subscription = newsLetterSubscriptions[i];
                        sb.Append(subscription.Email);
                        if (i != newsLetterSubscriptions.Count - 1)
                            sb.AppendLine(",");
                    }
                    string result = sb.ToString();
                    CommonHelper.WriteResponseTxt(result, fileName);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnImportCSV_Click(object sender, EventArgs e)
        {
            if (fuCsvFile.PostedFile != null && !String.IsNullOrEmpty(fuCsvFile.FileName))
            {
                try
                {
                    int count = 0;

                    using (StreamReader reader = new StreamReader(fuCsvFile.FileContent))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] tmp = line.Split('\t');

                            if (tmp.Length == 2)
                            {
                                string email = tmp[0].Trim();
                                bool isActive = Boolean.Parse(tmp[1]);

                                NewsLetterSubscription subscription = MessageManager.GetNewsLetterSubscriptionByEmail(email);
                                if (subscription != null)
                                {
                                    subscription = MessageManager.UpdateNewsLetterSubscription(subscription.NewsLetterSubscriptionId, email, isActive);
                                }
                                else
                                {
                                    subscription = MessageManager.InsertNewsLetterSubscription(email, isActive);
                                }
                                count++;
                            }
                        }
                        ShowMessage(String.Format(GetLocaleResourceString("Admin.NewsletterSubscribers.ImportEmailsButton.Success"), count));
                    }
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }    

        protected void gvNewsletterSubscribers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvNewsletterSubscribers.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    BindGrid();
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow row in gvNewsletterSubscribers.Rows)
                {
                    var cbNewsLetterSubscription = row.FindControl("cbNewsLetterSubscription") as CheckBox;
                    var hfNewsLetterSubscriptionId = row.FindControl("hfNewsLetterSubscriptionId") as HiddenField;

                    bool isChecked = cbNewsLetterSubscription.Checked;
                    int newsLetterSubscriptionId = int.Parse(hfNewsLetterSubscriptionId.Value);
                    if (isChecked)
                    {
                        MessageManager.DeleteNewsLetterSubscription(newsLetterSubscriptionId);
                    }
                }

                BindGrid();
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();

            base.OnPreRender(e);
        }
    }
}