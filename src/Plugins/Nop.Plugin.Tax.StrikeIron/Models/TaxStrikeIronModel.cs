using System.ComponentModel;

namespace Nop.Plugin.Tax.StrikeIron.Models
{
    public class TaxStrikeIronModel
    {
        [DisplayName("StrikeIron User ID")]
        public string UserId { get; set; }

        [DisplayNameAttribute("StrikeIron Password")]
        public string Password { get; set; }

        [DisplayNameAttribute("Zip Code")]
        public string TestingUsaZip { get; set; }

        public string TestingUsaResult { get; set; }

        [DisplayNameAttribute("Two Letter Province Code")]
        public string TestingCanadaProvinceCode { get; set; }

        public string TestingCanadaResult { get; set; }
    }
}