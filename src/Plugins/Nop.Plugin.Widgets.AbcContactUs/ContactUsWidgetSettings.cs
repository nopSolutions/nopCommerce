
using Nop.Core.Configuration;
using Nop.Plugin.Widgets.AbcContactUs.Models;
using System;

namespace Nop.Plugin.Widgets.AbcContactUs
{
    public class ContactUsWidgetSettings : ISettings
    {
        public string ContactUsEmail { get; private set; }

        public string AdditionalEmails { get; private set; }
        public bool IsEmailSubmissionSkipped { get; private set; }

        internal static ContactUsWidgetSettings FromModel(ContactUsConfigModel model)
        {
            return new ContactUsWidgetSettings()
            {
                ContactUsEmail = model.ContactUsEmail,
                AdditionalEmails = model.AdditionalEmails,
                IsEmailSubmissionSkipped = model.IsEmailSubmissionSkipped
            };
        }

        public ContactUsConfigModel ToModel()
        {
            return new ContactUsConfigModel()
            {
                ContactUsEmail = ContactUsEmail,
                AdditionalEmails = AdditionalEmails,
                IsEmailSubmissionSkipped = IsEmailSubmissionSkipped
            };
        }

        internal static ContactUsWidgetSettings Default()
        {
            return new ContactUsWidgetSettings()
            {
                AdditionalEmails = "support@abcwarehouse.com,mshelby@abcwarehouse.com,ddirven@abcwarehouse.com,martyh@abcwarehouse.com,johnjh@abcwarehouse.com"
            };
        }
    }
}