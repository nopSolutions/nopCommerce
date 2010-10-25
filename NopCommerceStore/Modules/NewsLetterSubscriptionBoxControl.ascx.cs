using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class NewsLetterSubscriptionBoxControl : BaseNopUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            this.Visible = !IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Display.HideNewsletterBox");
            base.OnInit(e);
        }

        protected void BtnSubscribe_OnClick(object sender, EventArgs e)
        {
            try
            {
                var subscription = IoCFactory.Resolve<IMessageManager>().GetNewsLetterSubscriptionByEmail(txtEmail.Text);
                if(subscription != null)
                {
                    if(rbSubscribe.Checked)
                    {
                        if (!subscription.Active)
                        {
                            IoCFactory.Resolve<IMessageManager>().SendNewsLetterSubscriptionActivationMessage(subscription.NewsLetterSubscriptionId, NopContext.Current.WorkingLanguage.LanguageId);
                        }
                        lblResult.Text = GetLocaleResourceString("NewsLetterSubscriptionBox.SubscriptionCreated");
                    }
                    else if(rbUnsubscribe.Checked)
                    {
                        if (subscription.Active)
                        {
                            IoCFactory.Resolve<IMessageManager>().SendNewsLetterSubscriptionDeactivationMessage(subscription.NewsLetterSubscriptionId, NopContext.Current.WorkingLanguage.LanguageId);
                        }
                        lblResult.Text = GetLocaleResourceString("NewsLetterSubscriptionBox.SubscriptionDeactivated");
                    }
                }
                else if(rbSubscribe.Checked)
                {
                    subscription = new NewsLetterSubscription()
                    {
                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                        Email = txtEmail.Text,
                        Active = false,
                        CreatedOn = DateTime.UtcNow
                    };
                    IoCFactory.Resolve<IMessageManager>().InsertNewsLetterSubscription(subscription);
                    IoCFactory.Resolve<IMessageManager>().SendNewsLetterSubscriptionActivationMessage(subscription.NewsLetterSubscriptionId, NopContext.Current.WorkingLanguage.LanguageId);
                    lblResult.Text = GetLocaleResourceString("NewsLetterSubscriptionBox.SubscriptionCreated");
                }
                else
                {
                    lblResult.Text = GetLocaleResourceString("NewsLetterSubscriptionBox.SubscriptionDeactivated");
                }
            }
            catch(Exception ex)
            {
                lblResult.Text = ex.Message;
            }

            pnlResult.Visible = true;
            pnlSubscribe.Visible = false;
        }
    }
}