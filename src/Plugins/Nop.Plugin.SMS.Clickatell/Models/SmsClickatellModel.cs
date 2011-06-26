using Nop.Web.Framework;

namespace Nop.Plugin.Sms.Clickatell.Models
{
    public class SmsClickatellModel
    {
        [NopResourceDisplayName("Plugins.Sms.Clickatell.Fields.PhoneNumber")]
        public string PhoneNumber { get; set; }

        [NopResourceDisplayName("Plugins.Sms.Clickatell.Fields.ApiId")]
        public string ApiId { get; set; }

        [NopResourceDisplayName("Plugins.Sms.Clickatell.Fields.Username")]
        public string Username { get; set; }

        [NopResourceDisplayName("Plugins.Sms.Clickatell.Fields.Password")]
        public string Password { get; set; }


        [NopResourceDisplayName("Plugins.Sms.Clickatell.Fields.TestMessage")]
        public string TestMessage { get; set; }
        public string TestSmsResult { get; set; }
    }
}