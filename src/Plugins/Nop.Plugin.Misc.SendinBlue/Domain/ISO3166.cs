using System.Collections.Generic;
using System.Linq;
using Nop.Core;

namespace Nop.Plugin.Misc.SendinBlue.Domain
{
    /// <summary>
    /// Represents ISO3166 country info
    /// </summary>
    public static class ISO3166
    {
        #region Fields

        private static readonly IList<ISO3166Country> _countriesInfo = new List<ISO3166Country>
        {
            // This collection built from Wikipedia entry on ISO3166-1 on 9th Feb 2016
            new ISO3166Country("Afghanistan", "AF", "AFG", 4, new[] { "93" }),
            new ISO3166Country("Åland Islands", "AX", "ALA", 248, new[] { "358" }),
            new ISO3166Country("Albania", "AL", "ALB", 8, new[] { "355" }),
            new ISO3166Country("Algeria", "DZ", "DZA", 12, new[] { "213" }),
            new ISO3166Country("American Samoa", "AS", "ASM", 16, new[] { "1 684" }),
            new ISO3166Country("Andorra", "AD", "AND", 20, new[] { "376" }),
            new ISO3166Country("Angola", "AO", "AGO", 24, new[] { "244" }),
            new ISO3166Country("Anguilla", "AI", "AIA", 660, new[] { "1 264" }),
            new ISO3166Country("Antarctica", "AQ", "ATA", 10, new[] { "672" }),
            new ISO3166Country("Antigua and Barbuda", "AG", "ATG", 28, new[] { "1 268" }),
            new ISO3166Country("Argentina", "AR", "ARG", 32, new[] { "54" }),
            new ISO3166Country("Armenia", "AM", "ARM", 51, new[] { "374" }),
            new ISO3166Country("Aruba", "AW", "ABW", 533, new[] { "297" }),
            new ISO3166Country("Australia", "AU", "AUS", 36, new[] { "61" }),
            new ISO3166Country("Austria", "AT", "AUT", 40, new[] { "43" }),
            new ISO3166Country("Azerbaijan", "AZ", "AZE", 31, new[] { "994" }),
            new ISO3166Country("Bahamas", "BS", "BHS", 44, new[] { "1 242" }),
            new ISO3166Country("Bahrain", "BH", "BHR", 48, new[] { "973" }),
            new ISO3166Country("Bangladesh", "BD", "BGD", 50, new[] { "880" }),
            new ISO3166Country("Barbados", "BB", "BRB", 52, new[] { "1 246" }),
            new ISO3166Country("Belarus", "BY", "BLR", 112, new[] { "375" }),
            new ISO3166Country("Belgium", "BE", "BEL", 56, new[] { "32" }),
            new ISO3166Country("Belize", "BZ", "BLZ", 84, new[] { "501" }),
            new ISO3166Country("Benin", "BJ", "BEN", 204, new[] { "229" }),
            new ISO3166Country("Bermuda", "BM", "BMU", 60, new[] { "1 441" }),
            new ISO3166Country("Bhutan", "BT", "BTN", 64, new[] { "975" }),
            new ISO3166Country("Bolivia (Plurinational State of)", "BO", "BOL", 68, new[] { "591" }),
            new ISO3166Country("Bonaire, Sint Eustatius and Saba", "BQ", "BES", 535, new[] { "599" }),
            new ISO3166Country("Bosnia and Herzegovina", "BA", "BIH", 70, new[] { "387" }),
            new ISO3166Country("Botswana", "BW", "BWA", 72, new[] { "267" }),
            new ISO3166Country("Bouvet Island", "BV", "BVT", 74),
            new ISO3166Country("Brazil", "BR", "BRA", 76, new[] { "55" }),
            new ISO3166Country("British Indian Ocean Territory", "IO", "IOT", 86, new[] { "246" }),
            new ISO3166Country("Brunei Darussalam", "BN", "BRN", 96, new[] { "673" }),
            new ISO3166Country("Bulgaria", "BG", "BGR", 100, new[] { "359" }),
            new ISO3166Country("Burkina Faso", "BF", "BFA", 854, new[] { "226" }),
            new ISO3166Country("Burundi", "BI", "BDI", 108, new[] { "257" }),
            new ISO3166Country("Cabo Verde", "CV", "CPV", 132, new[] { "238" }),
            new ISO3166Country("Cambodia", "KH", "KHM", 116, new[] { "855" }),
            new ISO3166Country("Cameroon", "CM", "CMR", 120, new[] { "237" }),
            new ISO3166Country("Canada", "CA", "CAN", 124, new[] { "1" }),
            new ISO3166Country("Cayman Islands", "KY", "CYM", 136, new[] { "1 345" }),
            new ISO3166Country("Central African Republic", "CF", "CAF", 140, new[] { "236" }),
            new ISO3166Country("Chad", "TD", "TCD", 148, new[] { "235" }),
            new ISO3166Country("Chile", "CL", "CHL", 152, new[] { "56" }),
            new ISO3166Country("China", "CN", "CHN", 156, new[] { "86" }),
            new ISO3166Country("Christmas Island", "CX", "CXR", 162, new[] { "61" }),
            new ISO3166Country("Cocos (Keeling) Islands", "CC", "CCK", 166, new[] { "61" }),
            new ISO3166Country("Colombia", "CO", "COL", 170, new[] { "57" }),
            new ISO3166Country("Comoros", "KM", "COM", 174, new[] { "269" }),
            new ISO3166Country("Congo", "CG", "COG", 178, new[] { "242" }),
            new ISO3166Country("Congo (Democratic Republic of the)", "CD", "COD", 180, new[] { "243" }),
            new ISO3166Country("Cook Islands", "CK", "COK", 184, new[] { "682" }),
            new ISO3166Country("Costa Rica", "CR", "CRI", 188, new[] { "506" }),
            new ISO3166Country("Côte d'Ivoire", "CI", "CIV", 384, new[] { "225" }),
            new ISO3166Country("Croatia", "HR", "HRV", 191, new[] { "385" }),
            new ISO3166Country("Cuba", "CU", "CUB", 192, new[] { "53" }),
            new ISO3166Country("Curaçao", "CW", "CUW", 531, new[] { "599" }),
            new ISO3166Country("Cyprus", "CY", "CYP", 196, new[] { "357" }),
            new ISO3166Country("Czech Republic", "CZ", "CZE", 203, new[] { "420" }),
            new ISO3166Country("Denmark", "DK", "DNK", 208, new[] { "45" }),
            new ISO3166Country("Djibouti", "DJ", "DJI", 262, new[] { "253" }),
            new ISO3166Country("Dominica", "DM", "DMA", 212, new[] { "1 767" }),
            new ISO3166Country("Dominican Republic", "DO", "DOM", 214, new[] { "1 809", "1 829", "1 849" }),
            new ISO3166Country("Ecuador", "EC", "ECU", 218, new[] { "593" }),
            new ISO3166Country("Egypt", "EG", "EGY", 818, new[] { "20" }),
            new ISO3166Country("El Salvador", "SV", "SLV", 222, new[] { "503" }),
            new ISO3166Country("Equatorial Guinea", "GQ", "GNQ", 226, new[] { "240" }),
            new ISO3166Country("Eritrea", "ER", "ERI", 232, new[] { "291" }),
            new ISO3166Country("Estonia", "EE", "EST", 233, new[] { "372" }),
            new ISO3166Country("Ethiopia", "ET", "ETH", 231, new[] { "251" }),
            new ISO3166Country("Falkland Islands (Malvinas)", "FK", "FLK", 238, new[] { "500" }),
            new ISO3166Country("Faroe Islands", "FO", "FRO", 234, new[] { "298" }),
            new ISO3166Country("Fiji", "FJ", "FJI", 242, new[] { "679" }),
            new ISO3166Country("Finland", "FI", "FIN", 246, new[] { "358" }),
            new ISO3166Country("France", "FR", "FRA", 250, new[] { "33" }),
            new ISO3166Country("French Guiana", "GF", "GUF", 254, new[] { "594" }),
            new ISO3166Country("French Polynesia", "PF", "PYF", 258, new[] { "689" }),
            new ISO3166Country("French Southern Territories", "TF", "ATF", 260, new[] { "262" }),
            new ISO3166Country("Gabon", "GA", "GAB", 266, new[] { "241" }),
            new ISO3166Country("Gambia", "GM", "GMB", 270, new[] { "220" }),
            new ISO3166Country("Georgia", "GE", "GEO", 268, new[] { "995" }),
            new ISO3166Country("Germany", "DE", "DEU", 276, new[] { "49" }),
            new ISO3166Country("Ghana", "GH", "GHA", 288, new[] { "233" }),
            new ISO3166Country("Gibraltar", "GI", "GIB", 292, new[] { "350" }),
            new ISO3166Country("Greece", "GR", "GRC", 300, new[] { "30" }),
            new ISO3166Country("Greenland", "GL", "GRL", 304, new[] { "299" }),
            new ISO3166Country("Grenada", "GD", "GRD", 308, new[] { "1 473" }),
            new ISO3166Country("Guadeloupe", "GP", "GLP", 312, new[] { "590" }),
            new ISO3166Country("Guam", "GU", "GUM", 316, new[] { "1 671" }),
            new ISO3166Country("Guatemala", "GT", "GTM", 320, new[] { "502" }),
            new ISO3166Country("Guernsey", "GG", "GGY", 831, new[] { "44 1481" }),
            new ISO3166Country("Guinea", "GN", "GIN", 324, new[] { "224" }),
            new ISO3166Country("Guinea-Bissau", "GW", "GNB", 624, new[] { "245" }),
            new ISO3166Country("Guyana", "GY", "GUY", 328, new[] { "592" }),
            new ISO3166Country("Haiti", "HT", "HTI", 332, new[] { "509" }),
            new ISO3166Country("Heard Island and McDonald Islands", "HM", "HMD", 334),
            new ISO3166Country("Holy See", "VA", "VAT", 336, new[] { "379" }),
            new ISO3166Country("Honduras", "HN", "HND", 340, new[] { "504" }),
            new ISO3166Country("Hong Kong", "HK", "HKG", 344, new[] { "852" }),
            new ISO3166Country("Hungary", "HU", "HUN", 348, new[] { "36" }),
            new ISO3166Country("Iceland", "IS", "ISL", 352, new[] { "354" }),
            new ISO3166Country("India", "IN", "IND", 356, new[] { "91" }),
            new ISO3166Country("Indonesia", "ID", "IDN", 360, new[] { "62" }),
            new ISO3166Country("Iran (Islamic Republic of)", "IR", "IRN", 364, new[] { "98" }),
            new ISO3166Country("Iraq", "IQ", "IRQ", 368, new[] { "964" }),
            new ISO3166Country("Ireland", "IE", "IRL", 372, new[] { "353" }),
            new ISO3166Country("Isle of Man", "IM", "IMN", 833, new[] { "44 1624" }),
            new ISO3166Country("Israel", "IL", "ISR", 376, new[] { "972" }),
            new ISO3166Country("Italy", "IT", "ITA", 380, new[] { "39" }),
            new ISO3166Country("Jamaica", "JM", "JAM", 388, new[] { "1 876" }),
            new ISO3166Country("Japan", "JP", "JPN", 392, new[] { "81" }),
            new ISO3166Country("Jersey", "JE", "JEY", 832, new[] { "44 1534" }),
            new ISO3166Country("Jordan", "JO", "JOR", 400, new[] { "962" }),
            new ISO3166Country("Kazakhstan", "KZ", "KAZ", 398, new[] { "7" }),
            new ISO3166Country("Kenya", "KE", "KEN", 404, new[] { "254" }),
            new ISO3166Country("Kiribati", "KI", "KIR", 296, new[] { "686" }),
            new ISO3166Country("Korea (Democratic People's Republic of)", "KP", "PRK", 408, new[] { "850" }),
            new ISO3166Country("Korea (Republic of)", "KR", "KOR", 410, new[] { "82" }),
            new ISO3166Country("Kuwait", "KW", "KWT", 414, new[] { "965" }),
            new ISO3166Country("Kyrgyzstan", "KG", "KGZ", 417, new[] { "996" }),
            new ISO3166Country("Lao People's Democratic Republic", "LA", "LAO", 418, new[] { "856" }),
            new ISO3166Country("Latvia", "LV", "LVA", 428, new[] { "371" }),
            new ISO3166Country("Lebanon", "LB", "LBN", 422, new[] { "961" }),
            new ISO3166Country("Lesotho", "LS", "LSO", 426, new[] { "266" }),
            new ISO3166Country("Liberia", "LR", "LBR", 430, new[] { "231" }),
            new ISO3166Country("Libya", "LY", "LBY", 434, new[] { "218" }),
            new ISO3166Country("Liechtenstein", "LI", "LIE", 438, new[] { "423" }),
            new ISO3166Country("Lithuania", "LT", "LTU", 440, new[] { "370" }),
            new ISO3166Country("Luxembourg", "LU", "LUX", 442, new[] { "352" }),
            new ISO3166Country("Macao", "MO", "MAC", 446, new[] { "853" }),
            new ISO3166Country("Macedonia (the former Yugoslav Republic of)", "MK", "MKD", 807, new[] { "389" }),
            new ISO3166Country("Madagascar", "MG", "MDG", 450, new[] { "261" }),
            new ISO3166Country("Malawi", "MW", "MWI", 454, new[] { "265" }),
            new ISO3166Country("Malaysia", "MY", "MYS", 458, new[] { "60" }),
            new ISO3166Country("Maldives", "MV", "MDV", 462, new[] { "960" }),
            new ISO3166Country("Mali", "ML", "MLI", 466, new[] { "223" }),
            new ISO3166Country("Malta", "MT", "MLT", 470, new[] { "356" }),
            new ISO3166Country("Marshall Islands", "MH", "MHL", 584, new[] { "692" }),
            new ISO3166Country("Martinique", "MQ", "MTQ", 474, new[] { "596" }),
            new ISO3166Country("Mauritania", "MR", "MRT", 478, new[] { "222" }),
            new ISO3166Country("Mauritius", "MU", "MUS", 480, new[] { "230" }),
            new ISO3166Country("Mayotte", "YT", "MYT", 175, new[] { "262" }),
            new ISO3166Country("Mexico", "MX", "MEX", 484, new[] { "52" }),
            new ISO3166Country("Micronesia (Federated States of)", "FM", "FSM", 583, new[] { "691" }),
            new ISO3166Country("Moldova (Republic of)", "MD", "MDA", 498, new[] { "373" }),
            new ISO3166Country("Monaco", "MC", "MCO", 492, new[] { "377" }),
            new ISO3166Country("Mongolia", "MN", "MNG", 496, new[] { "976" }),
            new ISO3166Country("Montenegro", "ME", "MNE", 499, new[] { "382" }),
            new ISO3166Country("Montserrat", "MS", "MSR", 500, new[] { "1 664" }),
            new ISO3166Country("Morocco", "MA", "MAR", 504, new[] { "212" }),
            new ISO3166Country("Mozambique", "MZ", "MOZ", 508, new[] { "258" }),
            new ISO3166Country("Myanmar", "MM", "MMR", 104, new[] { "95" }),
            new ISO3166Country("Namibia", "NA", "NAM", 516, new[] { "264" }),
            new ISO3166Country("Nauru", "NR", "NRU", 520, new[] { "674" }),
            new ISO3166Country("Nepal", "NP", "NPL", 524, new[] { "977" }),
            new ISO3166Country("Netherlands", "NL", "NLD", 528, new[] { "31" }),
            new ISO3166Country("New Caledonia", "NC", "NCL", 540, new[] { "687" }),
            new ISO3166Country("New Zealand", "NZ", "NZL", 554, new[] { "64" }),
            new ISO3166Country("Nicaragua", "NI", "NIC", 558, new[] { "505" }),
            new ISO3166Country("Niger", "NE", "NER", 562, new[] { "227" }),
            new ISO3166Country("Nigeria", "NG", "NGA", 566, new[] { "234" }),
            new ISO3166Country("Niue", "NU", "NIU", 570, new[] { "683" }),
            new ISO3166Country("Norfolk Island", "NF", "NFK", 574, new[] { "672" }),
            new ISO3166Country("Northern Mariana Islands", "MP", "MNP", 580, new[] { "1 670" }),
            new ISO3166Country("Norway", "NO", "NOR", 578, new[] { "47" }),
            new ISO3166Country("Oman", "OM", "OMN", 512, new[] { "968" }),
            new ISO3166Country("Pakistan", "PK", "PAK", 586, new[] { "92" }),
            new ISO3166Country("Palau", "PW", "PLW", 585, new[] { "680" }),
            new ISO3166Country("Palestine, State of", "PS", "PSE", 275, new[] { "970" }),
            new ISO3166Country("Panama", "PA", "PAN", 591, new[] { "507" }),
            new ISO3166Country("Papua New Guinea", "PG", "PNG", 598, new[] { "675" }),
            new ISO3166Country("Paraguay", "PY", "PRY", 600, new[] { "595" }),
            new ISO3166Country("Peru", "PE", "PER", 604, new[] { "51" }),
            new ISO3166Country("Philippines", "PH", "PHL", 608, new[] { "63" }),
            new ISO3166Country("Pitcairn", "PN", "PCN", 612, new[] { "64" }),
            new ISO3166Country("Poland", "PL", "POL", 616, new[] { "48" }),
            new ISO3166Country("Portugal", "PT", "PRT", 620, new[] { "351" }),
            new ISO3166Country("Puerto Rico", "PR", "PRI", 630, new[] { "1 787", "1 939" }),
            new ISO3166Country("Qatar", "QA", "QAT", 634, new[] { "974" }),
            new ISO3166Country("Réunion", "RE", "REU", 638, new[] { "262" }),
            new ISO3166Country("Romania", "RO", "ROU", 642, new[] { "40" }),
            new ISO3166Country("Russian Federation", "RU", "RUS", 643, new[] { "7" }),
            new ISO3166Country("Rwanda", "RW", "RWA", 646, new[] { "250" }),
            new ISO3166Country("Saint Barthélemy", "BL", "BLM", 652, new[] { "590" }),
            new ISO3166Country("Saint Helena, Ascension and Tristan da Cunha", "SH", "SHN", 654, new[] { "290" }),
            new ISO3166Country("Saint Kitts and Nevis", "KN", "KNA", 659, new[] { "1 869" }),
            new ISO3166Country("Saint Lucia", "LC", "LCA", 662, new[] { "1 758" }),
            new ISO3166Country("Saint Martin (French part)", "MF", "MAF", 663, new[] { "590" }),
            new ISO3166Country("Saint Pierre and Miquelon", "PM", "SPM", 666, new[] { "508" }),
            new ISO3166Country("Saint Vincent and the Grenadines", "VC", "VCT", 670, new[] { "1 784" }),
            new ISO3166Country("Samoa", "WS", "WSM", 882, new[] { "685" }),
            new ISO3166Country("San Marino", "SP", "SMR", 674),
            new ISO3166Country("Sao Tome and Principe", "ST", "STP", 678, new[] { "239" }),
            new ISO3166Country("Saudi Arabia", "SA", "SAU", 682, new[] { "966" }),
            new ISO3166Country("Senegal", "SN", "SEN", 686, new[] { "221" }),
            new ISO3166Country("Serbia", "RS", "SRB", 688, new[] { "381" }),
            new ISO3166Country("Seychelles", "SC", "SYC", 690, new[] { "248" }),
            new ISO3166Country("Sierra Leone", "SL", "SLE", 694, new[] { "232" }),
            new ISO3166Country("Singapore", "SG", "SGP", 702, new[] { "65" }),
            new ISO3166Country("Sint Maarten (Dutch part)", "SX", "SXM", 534, new[] { "1 721" }),
            new ISO3166Country("Slovakia", "SK", "SVK", 703, new[] { "421" }),
            new ISO3166Country("Slovenia", "SI", "SVN", 705, new[] { "386" }),
            new ISO3166Country("Solomon Islands", "SB", "SLB", 90, new[] { "677" }),
            new ISO3166Country("Somalia", "SO", "SOM", 706, new[] { "252" }),
            new ISO3166Country("South Africa", "ZA", "ZAF", 710, new[] { "27" }),
            new ISO3166Country("South Georgia and the South Sandwich Islands", "GS", "SGS", 239, new[] { "500" }),
            new ISO3166Country("South Sudan", "SS", "SSD", 728, new[] { "211" }),
            new ISO3166Country("Spain", "ES", "ESP", 724, new[] { "34" }),
            new ISO3166Country("Sri Lanka", "LK", "LKA", 144, new[] { "94" }),
            new ISO3166Country("Sudan", "SD", "SDN", 729, new[] { "249" }),
            new ISO3166Country("Suriname", "SR", "SUR", 740, new[] { "597" }),
            new ISO3166Country("Svalbard and Jan Mayen", "SJ", "SJM", 744, new[] { "47" }),
            new ISO3166Country("Swaziland", "SZ", "SWZ", 748, new[] { "268" }),
            new ISO3166Country("Sweden", "SE", "SWE", 752, new[] { "46" }),
            new ISO3166Country("Switzerland", "CH", "CHE", 756, new[] { "41" }),
            new ISO3166Country("Syrian Arab Republic", "SY", "SYR", 760, new[] { "963" }),
            new ISO3166Country("Taiwan, Province of China[a]", "TW", "TWN", 158, new[] { "886" }),
            new ISO3166Country("Tajikistan", "TJ", "TJK", 762, new[] { "992" }),
            new ISO3166Country("Tanzania, United Republic of", "TZ", "TZA", 834, new[] { "255" }),
            new ISO3166Country("Thailand", "TH", "THA", 764, new[] { "66" }),
            new ISO3166Country("Timor-Leste", "TL", "TLS", 626, new[] { "670" }),
            new ISO3166Country("Togo", "TG", "TGO", 768, new[] { "228" }),
            new ISO3166Country("Tokelau", "TK", "TKL", 772, new[] { "690" }),
            new ISO3166Country("Tonga", "TO", "TON", 776, new[] { "676" }),
            new ISO3166Country("Trinidad and Tobago", "TT", "TTO", 780, new[] { "1 868" }),
            new ISO3166Country("Tunisia", "TN", "TUN", 788, new[] { "216" }),
            new ISO3166Country("Turkey", "TR", "TUR", 792, new[] { "90" }),
            new ISO3166Country("Turkmenistan", "TM", "TKM", 795, new[] { "993" }),
            new ISO3166Country("Turks and Caicos Islands", "TC", "TCA", 796, new[] { "1 649" }),
            new ISO3166Country("Tuvalu", "TV", "TUV", 798, new[] { "688" }),
            new ISO3166Country("Uganda", "UG", "UGA", 800, new[] { "256" }),
            new ISO3166Country("Ukraine", "UA", "UKR", 804, new[] { "380" }),
            new ISO3166Country("United Arab Emirates", "AE", "ARE", 784, new[] { "971" }),
            new ISO3166Country("United Kingdom of Great Britain and Northern Ireland", "GB", "GBR", 826, new[] { "44" }),
            new ISO3166Country("United States of America", "US", "USA", 840, new[] { "1" }),
            new ISO3166Country("United States Minor Outlying Islands", "UM", "UMI", 581),
            new ISO3166Country("Uruguay", "UY", "URY", 858, new[] { "598" }),
            new ISO3166Country("Uzbekistan", "UZ", "UZB", 860, new[] { "998" }),
            new ISO3166Country("Vanuatu", "VU", "VUT", 548, new[] { "678" }),
            new ISO3166Country("Venezuela (Bolivarian Republic of)", "VE", "VEN", 862, new[] { "58" }),
            new ISO3166Country("Viet Nam", "VN", "VNM", 704, new[] { "84" }),
            new ISO3166Country("Virgin Islands (British)", "VG", "VGB", 92, new[] { "1 284" }),
            new ISO3166Country("Virgin Islands (U.S.)", "VI", "VIR", 850, new[] { "1 340" }),
            new ISO3166Country("Wallis and Futuna", "WF", "WLF", 876, new[] { "681" }),
            new ISO3166Country("Western Sahara", "EH", "ESH", 732, new[] { "212" }),
            new ISO3166Country("Yemen", "YE", "YEM", 887, new[] { "967" }),
            new ISO3166Country("Zambia", "ZM", "ZMB", 894, new[] { "260" }),
            new ISO3166Country("Zimbabwe", "ZW", "ZWE", 716, new[] { "263" })
        };

        #endregion

        #region Methods

        /// <summary>
        /// Obtain ISO3166-1 Country based on its ISO code
        /// </summary>
        /// <param name="code">ISO code</param>
        /// <returns>Country info</returns>
        public static ISO3166Country GetCountryInfoFromIsoCode(int code)
        {
            return _countriesInfo.FirstOrDefault(country => country.NumericCode == code)
                ?? throw new NopException($"There is no country with ISO code '{code}'");
        }

        #endregion

        #region Nested class

        public class ISO3166Country
        {
            public ISO3166Country(string name, string alpha2, string alpha3, int numericCode, string[] dialCodes = null)
            {
                Name = name;
                Alpha2 = alpha2;
                Alpha3 = alpha3;
                NumericCode = numericCode;
                DialCodes = dialCodes;
            }

            public string Name { get; private set; }

            public string Alpha2 { get; private set; }

            public string Alpha3 { get; private set; }

            public int NumericCode { get; private set; }

            public IEnumerable<string> DialCodes { get; private set; }
        }

        #endregion
    }
}