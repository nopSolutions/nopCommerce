using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Installation
{
    /// <summary>
    /// Represents the implementation of ISO3166-1
    /// </summary>
    /// <remarks>https://en.wikipedia.org/wiki/List_of_ISO_3166_country_codes</remarks>
    public static partial class ISO3166
    {
        /// <summary>
        /// Obtain ISO3166-1 Country based on its ISO code.
        /// </summary>
        /// <param name="codeISO"></param>
        /// <returns>ISO3166Country</returns>
        public static ISO3166Country FromISOCode(int codeISO)
        {
            return GetCollection().FirstOrDefault(p => p.NumericCode == codeISO);
        }

        /// <summary>
        /// Obtain ISO3166-1 Country based on its alpha-2.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns>ISO3166Country</returns>
        public static ISO3166Country FromCountryCode(string countryCode)
        {
            return GetCollection().FirstOrDefault(p => p.Alpha2 == countryCode);
        }

        /// <summary>
        /// Collection localization info for country
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns>IEnumerable<LocalizationInfo></returns>
        public static IEnumerable<LocalizationInfo> GetLocalizationInfo(string countryCode)
        {
            return FromCountryCode(countryCode).LocalizationInfo;
        }

        #region Collection of counties
        /// <summary>
        /// Collection of standard defining codes for the names of countries by ISO 3166-1
        /// </summary>
        /// <returns>IEnumerable<ISO3166Country></returns>
        public static IEnumerable<ISO3166Country> GetCollection()
        {
            // This collection built from Wikipedia entry on ISO3166-1 on 8th Dec 2020
            return new[] {
                new ISO3166Country("Afghanistan", "AF", "AFG", 4, new[] { "93" }),
                new ISO3166Country("Åland Islands", "AX", "ALA", 248, new[] { "358" }),
                new ISO3166Country("Albania", "AL", "ALB", 8, new[] { "355" }, localizationInfo: new[] { new LocalizationInfo("sq-AL", "Albanian") }),
                new ISO3166Country("Algeria", "DZ", "DZA", 12, new[] { "213" }, localizationInfo: new[] { new LocalizationInfo("ar-DZ", "Arabic") }),
                new ISO3166Country("American Samoa", "AS", "ASM", 16, new[] { "1 684" }),
                new ISO3166Country("Andorra", "AD", "AND", 20, new[] { "376" }, localizationInfo: new[] { new LocalizationInfo("ca-ES", "Catalan") }),
                new ISO3166Country("Angola", "AO", "AGO", 24, new[] { "244" }, localizationInfo: new[] { new LocalizationInfo("pt-AO", "Portuguese") }),
                new ISO3166Country("Anguilla", "AI", "AIA", 660, new[] { "1 264" }),
                new ISO3166Country("Antarctica", "AQ", "ATA", 10, new[] { "672" }),
                new ISO3166Country("Antigua and Barbuda", "AG", "ATG", 28, new[] { "1 268" }),
                new ISO3166Country("Argentina", "AR", "ARG", 32, new[] { "54" }, localizationInfo: new[] { new LocalizationInfo("es-AR", "Spanish") }),
                new ISO3166Country("Armenia", "AM", "ARM", 51, new[] { "374" }, localizationInfo: new[] { new LocalizationInfo("hy-AM", "Armenian") }),
                new ISO3166Country("Aruba", "AW", "ABW", 533, new[] { "297" }),
                new ISO3166Country("Australia", "AU", "AUS", 36, new[] { "61" }),
                new ISO3166Country("Austria", "AT", "AUT", 40, new[] { "43" }, true, localizationInfo: new[] { new LocalizationInfo("de-AT", "German") }),
                new ISO3166Country("Azerbaijan", "AZ", "AZE", 31, new[] { "994" }, localizationInfo: new[] { new LocalizationInfo("az-Latn-AZ", "Azerbaijani") }),
                new ISO3166Country("Bahamas", "BS", "BHS", 44, new[] { "1 242" }),
                new ISO3166Country("Bahrain", "BH", "BHR", 48, new[] { "973" }, localizationInfo: new[] { new LocalizationInfo("ar-BH", "Arabic") }),
                new ISO3166Country("Bangladesh", "BD", "BGD", 50, new[] { "880" }, localizationInfo: new[] { new LocalizationInfo("bn-BD", "Bangla") }),
                new ISO3166Country("Barbados", "BB", "BRB", 52, new[] { "1 246" }),
                new ISO3166Country("Belarus", "BY", "BLR", 112, new[] { "375" }, localizationInfo: new[] { new LocalizationInfo("ru-RU", "Russian") }),
                new ISO3166Country("Belgium", "BE", "BEL", 56, new[] { "32" }, true, localizationInfo: new[] { new LocalizationInfo("fr-BE", "French"), new LocalizationInfo("nl-BE", "Dutch") }),
                new ISO3166Country("Belize", "BZ", "BLZ", 84, new[] { "501" }),
                new ISO3166Country("Benin", "BJ", "BEN", 204, new[] { "229" }),
                new ISO3166Country("Bermuda", "BM", "BMU", 60, new[] { "1 441" }),
                new ISO3166Country("Bhutan", "BT", "BTN", 64, new[] { "975" }),
                new ISO3166Country("Bolivia (Plurinational State of)", "BO", "BOL", 68, new[] { "591" }, localizationInfo: new[] { new LocalizationInfo("es-BO", "Spanish") }),
                new ISO3166Country("Bonaire, Sint Eustatius and Saba", "BQ", "BES", 535, new[] { "599" }),
                new ISO3166Country("Bosnia and Herzegovina", "BA", "BIH", 70, new[] { "387" }),
                new ISO3166Country("Botswana", "BW", "BWA", 72, new[] { "267" }),
                new ISO3166Country("Bouvet Island", "BV", "BVT", 74),
                new ISO3166Country("Brazil", "BR", "BRA", 76, new[] { "55" }, localizationInfo: new[] { new LocalizationInfo("pt-BR", "Portuguese") }),
                new ISO3166Country("British Indian Ocean Territory", "IO", "IOT", 86, new[] { "246" }),
                new ISO3166Country("Brunei Darussalam", "BN", "BRN", 96, new[] { "673" }),
                new ISO3166Country("Bulgaria", "BG", "BGR", 100, new[] { "359" }, true, localizationInfo: new[] { new LocalizationInfo("bg-BG", "Bulgarian") }),
                new ISO3166Country("Burkina Faso", "BF", "BFA", 854, new[] { "226" }),
                new ISO3166Country("Burundi", "BI", "BDI", 108, new[] { "257" }),
                new ISO3166Country("Cabo Verde", "CV", "CPV", 132, new[] { "238" }, localizationInfo: new[] { new LocalizationInfo("pt-CV", "Portuguese") }),
                new ISO3166Country("Cambodia", "KH", "KHM", 116, new[] { "855" }),
                new ISO3166Country("Cameroon", "CM", "CMR", 120, new[] { "237" }),
                new ISO3166Country("Canada", "CA", "CAN", 124, new[] { "1" }, localizationInfo: new[] { new LocalizationInfo("en-US", "English"), new LocalizationInfo("fr-FR", "French") }),
                new ISO3166Country("Cayman Islands", "KY", "CYM", 136, new[] { "1 345" }),
                new ISO3166Country("Central African Republic", "CF", "CAF", 140, new[] { "236" }),
                new ISO3166Country("Chad", "TD", "TCD", 148, new[] { "235" }),
                new ISO3166Country("Chile", "CL", "CHL", 152, new[] { "56" }, localizationInfo: new[] { new LocalizationInfo("es-CL", "Spanish") }),
                new ISO3166Country("China", "CN", "CHN", 156, new[] { "86" }, localizationInfo: new[] { new LocalizationInfo("zh-CN", "Chinese") }),
                new ISO3166Country("Christmas Island", "CX", "CXR", 162, new[] { "61" }),
                new ISO3166Country("Cocos (Keeling) Islands", "CC", "CCK", 166, new[] { "61" }),
                new ISO3166Country("Colombia", "CO", "COL", 170, new[] { "57" }, localizationInfo: new[] { new LocalizationInfo("es-CO", "Spanish") }),
                new ISO3166Country("Comoros", "KM", "COM", 174, new[] { "269" }),
                new ISO3166Country("Congo", "CG", "COG", 178, new[] { "242" }),
                new ISO3166Country("Congo (Democratic Republic of the)", "CD", "COD", 180, new[] { "243" }),
                new ISO3166Country("Cook Islands", "CK", "COK", 184, new[] { "682" }),
                new ISO3166Country("Costa Rica", "CR", "CRI", 188, new[] { "506" }, localizationInfo: new[] { new LocalizationInfo("es-CR", "Spanish") }),
                new ISO3166Country("Côte d'Ivoire", "CI", "CIV", 384, new[] { "225" }),
                new ISO3166Country("Croatia", "HR", "HRV", 191, new[] { "385" }, true, localizationInfo: new[] { new LocalizationInfo("hr-HR", "Croatian") }),
                new ISO3166Country("Cuba", "CU", "CUB", 192, new[] { "53" }, localizationInfo: new[] { new LocalizationInfo("es-CU", "Spanish") }),
                new ISO3166Country("Curaçao", "CW", "CUW", 531, new[] { "599" }),
                new ISO3166Country("Cyprus", "CY", "CYP", 196, new[] { "357" }, true, localizationInfo: new[] { new LocalizationInfo("el-CY", "Greek"), new LocalizationInfo("tr-CY", "Turkish") }),
                new ISO3166Country("Czechia", "CZ", "CZE", 203, new[] { "420" }, true, localizationInfo: new[] { new LocalizationInfo("cs-CZ", "Czech") }),
                new ISO3166Country("Denmark", "DK", "DNK", 208, new[] { "45" }, true, localizationInfo: new[] { new LocalizationInfo("da-DK", "Danish") }),
                new ISO3166Country("Djibouti", "DJ", "DJI", 262, new[] { "253" }),
                new ISO3166Country("Dominica", "DM", "DMA", 212, new[] { "1 767" }),
                new ISO3166Country("Dominican Republic", "DO", "DOM", 214, new[] { "1 809", "1 829", "1 849" }, localizationInfo: new[] { new LocalizationInfo("es-DO", "Spanish") }),
                new ISO3166Country("Ecuador", "EC", "ECU", 218, new[] { "593" }, localizationInfo: new[] { new LocalizationInfo("es-EC", "Spanish") }),
                new ISO3166Country("Egypt", "EG", "EGY", 818, new[] { "20" }, localizationInfo: new[] { new LocalizationInfo("ar-EG", "Arabic") }),
                new ISO3166Country("El Salvador", "SV", "SLV", 222, new[] { "503" }, localizationInfo: new[] { new LocalizationInfo("es-SV", "Spanish") }),
                new ISO3166Country("Equatorial Guinea", "GQ", "GNQ", 226, new[] { "240" }),
                new ISO3166Country("Eritrea", "ER", "ERI", 232, new[] { "291" }),
                new ISO3166Country("Estonia", "EE", "EST", 233, new[] { "372" }, true, localizationInfo: new[] { new LocalizationInfo("et-EE", "Estonian") }),
                new ISO3166Country("Eswatini", "SZ", "SWZ", 748, new[] { "268" }),
                new ISO3166Country("Ethiopia", "ET", "ETH", 231, new[] { "251" }),
                new ISO3166Country("Falkland Islands (Malvinas)", "FK", "FLK", 238, new[] { "500" }),
                new ISO3166Country("Faroe Islands", "FO", "FRO", 234, new[] { "298" }),
                new ISO3166Country("Fiji", "FJ", "FJI", 242, new[] { "679" }),
                new ISO3166Country("Finland", "FI", "FIN", 246, new[] { "358" }, true, localizationInfo: new[] { new LocalizationInfo("fi-FI", "Finnish") }),
                new ISO3166Country("France", "FR", "FRA", 250, new[] { "33" }, true, localizationInfo: new[] { new LocalizationInfo("fr-FR", "French") }),
                new ISO3166Country("French Guiana", "GF", "GUF", 254, new[] { "594" }),
                new ISO3166Country("French Polynesia", "PF", "PYF", 258, new[] { "689" }),
                new ISO3166Country("French Southern Territories", "TF", "ATF", 260, new[] { "262" }),
                new ISO3166Country("Gabon", "GA", "GAB", 266, new[] { "241" }),
                new ISO3166Country("Gambia", "GM", "GMB", 270, new[] { "220" }),
                new ISO3166Country("Georgia", "GE", "GEO", 268, new[] { "995" }, localizationInfo: new[] { new LocalizationInfo("ka-GE", "Georgian") }),
                new ISO3166Country("Germany", "DE", "DEU", 276, new[] { "49" }, true, localizationInfo: new[] { new LocalizationInfo("de-DE", "German") }),
                new ISO3166Country("Ghana", "GH", "GHA", 288, new[] { "233" }),
                new ISO3166Country("Gibraltar", "GI", "GIB", 292, new[] { "350" }),
                new ISO3166Country("Greece", "GR", "GRC", 300, new[] { "30" }, true, localizationInfo: new[] { new LocalizationInfo("el-GR", "Greek") }),
                new ISO3166Country("Greenland", "GL", "GRL", 304, new[] { "299" }),
                new ISO3166Country("Grenada", "GD", "GRD", 308, new[] { "1 473" }),
                new ISO3166Country("Guadeloupe", "GP", "GLP", 312, new[] { "590" }),
                new ISO3166Country("Guam", "GU", "GUM", 316, new[] { "1 671" }),
                new ISO3166Country("Guatemala", "GT", "GTM", 320, new[] { "502" }, localizationInfo: new[] { new LocalizationInfo("es-GT", "Spanish") }),
                new ISO3166Country("Guernsey", "GG", "GGY", 831, new[] { "44 1481" }),
                new ISO3166Country("Guinea", "GN", "GIN", 324, new[] { "224" }),
                new ISO3166Country("Guinea-Bissau", "GW", "GNB", 624, new[] { "245" }, localizationInfo: new[] { new LocalizationInfo("pt-GW", "Portuguese") }),
                new ISO3166Country("Guyana", "GY", "GUY", 328, new[] { "592" }),
                new ISO3166Country("Haiti", "HT", "HTI", 332, new[] { "509" }),
                new ISO3166Country("Heard Island and McDonald Islands", "HM", "HMD", 334),
                new ISO3166Country("Holy See", "VA", "VAT", 336, new[] { "379" }),
                new ISO3166Country("Honduras", "HN", "HND", 340, new[] { "504" }, localizationInfo: new[] { new LocalizationInfo("es-HN", "Spanish") }),
                new ISO3166Country("Hong Kong", "HK", "HKG", 344, new[] { "852" }, localizationInfo: new[] { new LocalizationInfo("zh-CN", "Chinese") }),
                new ISO3166Country("Hungary", "HU", "HUN", 348, new[] { "36" }, true, localizationInfo: new[] { new LocalizationInfo("hu-HU", "Hungarian") }),
                new ISO3166Country("Iceland", "IS", "ISL", 352, new[] { "354" }, localizationInfo: new[] { new LocalizationInfo("is-IS", "Icelandic") }),
                new ISO3166Country("India", "IN", "IND", 356, new[] { "91" }),
                new ISO3166Country("Indonesia", "ID", "IDN", 360, new[] { "62" }, localizationInfo: new[] { new LocalizationInfo("id-ID", "Indonesian") }),
                new ISO3166Country("Iran (Islamic Republic of)", "IR", "IRN", 364, new[] { "98" }, localizationInfo: new[] { new LocalizationInfo("fa-IR", "Persian") }),
                new ISO3166Country("Iraq", "IQ", "IRQ", 368, new[] { "964" }, localizationInfo: new[] { new LocalizationInfo("ar-IQ", "Arabic") }),
                new ISO3166Country("Ireland", "IE", "IRL", 372, new[] { "353" }, true),
                new ISO3166Country("Isle of Man", "IM", "IMN", 833, new[] { "44 1624" }),
                new ISO3166Country("Israel", "IL", "ISR", 376, new[] { "972" }, localizationInfo: new[] { new LocalizationInfo("he-IL", "Hebrew") }),
                new ISO3166Country("Italy", "IT", "ITA", 380, new[] { "39" }, true, localizationInfo: new[] { new LocalizationInfo("it-IT", "Italian") }),
                new ISO3166Country("Jamaica", "JM", "JAM", 388, new[] { "1 876" }),
                new ISO3166Country("Japan", "JP", "JPN", 392, new[] { "81" }, localizationInfo: new[] { new LocalizationInfo("ja-JP", "Japanese") }),
                new ISO3166Country("Jersey", "JE", "JEY", 832, new[] { "44 1534" }),
                new ISO3166Country("Jordan", "JO", "JOR", 400, new[] { "962" }, localizationInfo: new[] { new LocalizationInfo("ar-JO", "Arabic") }),
                new ISO3166Country("Kazakhstan", "KZ", "KAZ", 398, new[] { "7" }),
                new ISO3166Country("Kenya", "KE", "KEN", 404, new[] { "254" }),
                new ISO3166Country("Kiribati", "KI", "KIR", 296, new[] { "686" }),
                new ISO3166Country("Korea (Democratic People's Republic of)", "KP", "PRK", 408, new[] { "850" }),
                new ISO3166Country("Korea (Republic of)", "KR", "KOR", 410, new[] { "82" }),
                new ISO3166Country("Kuwait", "KW", "KWT", 414, new[] { "965" }, localizationInfo: new[] { new LocalizationInfo("ar-KW", "Arabic") }),
                new ISO3166Country("Kyrgyzstan", "KG", "KGZ", 417, new[] { "996" }, localizationInfo: new[] { new LocalizationInfo("ky-KG", "Kyrgyz") }),
                new ISO3166Country("Lao People's Democratic Republic", "LA", "LAO", 418, new[] { "856" }),
                new ISO3166Country("Latvia", "LV", "LVA", 428, new[] { "371" }, true, localizationInfo: new[] { new LocalizationInfo("lv-LV", "Latvian") }),
                new ISO3166Country("Lebanon", "LB", "LBN", 422, new[] { "961" }, localizationInfo: new[] { new LocalizationInfo("ar-LB", "Arabic") }),
                new ISO3166Country("Lesotho", "LS", "LSO", 426, new[] { "266" }),
                new ISO3166Country("Liberia", "LR", "LBR", 430, new[] { "231" }),
                new ISO3166Country("Libya", "LY", "LBY", 434, new[] { "218" }, localizationInfo: new[] { new LocalizationInfo("ar-LY", "Arabic") }),
                new ISO3166Country("Liechtenstein", "LI", "LIE", 438, new[] { "423" }, localizationInfo: new[] { new LocalizationInfo("de-LI", "German") }),
                new ISO3166Country("Lithuania", "LT", "LTU", 440, new[] { "370" }, true, localizationInfo: new[] { new LocalizationInfo("lt-LT", "Lithuanian") }),
                new ISO3166Country("Luxembourg", "LU", "LUX", 442, new[] { "352" }, true, localizationInfo: new[] { new LocalizationInfo("fr-FR", "French"), new LocalizationInfo("de-LU", "German") }),
                new ISO3166Country("Macao", "MO", "MAC", 446, new[] { "853" }, localizationInfo: new[] { new LocalizationInfo("zh-CN", "Chinese") }),
                new ISO3166Country("North Macedonia", "MK", "MKD", 807, new[] { "389" }, localizationInfo: new[] { new LocalizationInfo("mk-MK", "Macedonian") }),
                new ISO3166Country("Madagascar", "MG", "MDG", 450, new[] { "261" }),
                new ISO3166Country("Malawi", "MW", "MWI", 454, new[] { "265" }),
                new ISO3166Country("Malaysia", "MY", "MYS", 458, new[] { "60" }),
                new ISO3166Country("Maldives", "MV", "MDV", 462, new[] { "960" }),
                new ISO3166Country("Mali", "ML", "MLI", 466, new[] { "223" }),
                new ISO3166Country("Malta", "MT", "MLT", 470, new[] { "356" }, true),
                new ISO3166Country("Marshall Islands", "MH", "MHL", 584, new[] { "692" }),
                new ISO3166Country("Martinique", "MQ", "MTQ", 474, new[] { "596" }),
                new ISO3166Country("Mauritania", "MR", "MRT", 478, new[] { "222" }),
                new ISO3166Country("Mauritius", "MU", "MUS", 480, new[] { "230" }),
                new ISO3166Country("Mayotte", "YT", "MYT", 175, new[] { "262" }),
                new ISO3166Country("Mexico", "MX", "MEX", 484, new[] { "52" }, localizationInfo: new[] { new LocalizationInfo("es-MX", "Spanish") }),
                new ISO3166Country("Micronesia (Federated States of)", "FM", "FSM", 583, new[] { "691" }),
                new ISO3166Country("Moldova (Republic of)", "MD", "MDA", 498, new[] { "373" }),
                new ISO3166Country("Monaco", "MC", "MCO", 492, new[] { "377" }, localizationInfo: new[] { new LocalizationInfo("fr-FR", "French") }),
                new ISO3166Country("Mongolia", "MN", "MNG", 496, new[] { "976" }),
                new ISO3166Country("Montenegro", "ME", "MNE", 499, new[] { "382" }),
                new ISO3166Country("Montserrat", "MS", "MSR", 500, new[] { "1 664" }),
                new ISO3166Country("Morocco", "MA", "MAR", 504, new[] { "212" }, localizationInfo: new[] { new LocalizationInfo("ar-MA", "Arabic") }),
                new ISO3166Country("Mozambique", "MZ", "MOZ", 508, new[] { "258" }, localizationInfo: new[] { new LocalizationInfo("pt-MZ", "Portuguese") }),
                new ISO3166Country("Myanmar", "MM", "MMR", 104, new[] { "95" }),
                new ISO3166Country("Namibia", "NA", "NAM", 516, new[] { "264" }),
                new ISO3166Country("Nauru", "NR", "NRU", 520, new[] { "674" }),
                new ISO3166Country("Nepal", "NP", "NPL", 524, new[] { "977" }, localizationInfo: new[] { new LocalizationInfo("ne-NP", "Nepali") }),
                new ISO3166Country("Netherlands", "NL", "NLD", 528, new[] { "31" }, true, localizationInfo: new[] { new LocalizationInfo("nl-NL", "Dutch") }),
                new ISO3166Country("New Caledonia", "NC", "NCL", 540, new[] { "687" }),
                new ISO3166Country("New Zealand", "NZ", "NZL", 554, new[] { "64" }),
                new ISO3166Country("Nicaragua", "NI", "NIC", 558, new[] { "505" }, localizationInfo: new[] { new LocalizationInfo("es-NI", "Spanish") }),
                new ISO3166Country("Niger", "NE", "NER", 562, new[] { "227" }),
                new ISO3166Country("Nigeria", "NG", "NGA", 566, new[] { "234" }),
                new ISO3166Country("Niue", "NU", "NIU", 570, new[] { "683" }),
                new ISO3166Country("Norfolk Island", "NF", "NFK", 574, new[] { "672" }),
                new ISO3166Country("Northern Mariana Islands", "MP", "MNP", 580, new[] { "1 670" }),
                new ISO3166Country("Norway", "NO", "NOR", 578, new[] { "47" }, localizationInfo: new[] { new LocalizationInfo("nb-NO", "Norwegian") }),
                new ISO3166Country("Oman", "OM", "OMN", 512, new[] { "968" }, localizationInfo: new[] { new LocalizationInfo("ar-OM", "Arabic") }),
                new ISO3166Country("Pakistan", "PK", "PAK", 586, new[] { "92" }),
                new ISO3166Country("Palau", "PW", "PLW", 585, new[] { "680" }),
                new ISO3166Country("Palestine, State of", "PS", "PSE", 275, new[] { "970" }, localizationInfo: new[] { new LocalizationInfo("ar-PS", "Arabic") }),
                new ISO3166Country("Panama", "PA", "PAN", 591, new[] { "507" }, localizationInfo: new[] { new LocalizationInfo("es-PA", "Spanish") }),
                new ISO3166Country("Papua New Guinea", "PG", "PNG", 598, new[] { "675" }),
                new ISO3166Country("Paraguay", "PY", "PRY", 600, new[] { "595" }, localizationInfo: new[] { new LocalizationInfo("es-PY", "Spanish") }),
                new ISO3166Country("Peru", "PE", "PER", 604, new[] { "51" }, localizationInfo: new[] { new LocalizationInfo("es-PE", "Spanish") }),
                new ISO3166Country("Philippines", "PH", "PHL", 608, new[] { "63" }),
                new ISO3166Country("Pitcairn", "PN", "PCN", 612, new[] { "64" }),
                new ISO3166Country("Poland", "PL", "POL", 616, new[] { "48" }, true, localizationInfo: new[] { new LocalizationInfo("pl-PL", "Polish") }),
                new ISO3166Country("Portugal", "PT", "PRT", 620, new[] { "351" }, true),
                new ISO3166Country("Puerto Rico", "PR", "PRI", 630, new[] { "1 787", "1 939" }, localizationInfo: new[] { new LocalizationInfo("es-PR", "Spanish") }),
                new ISO3166Country("Qatar", "QA", "QAT", 634, new[] { "974" }, localizationInfo: new[] { new LocalizationInfo("ar-QA", "Arabic") }),
                new ISO3166Country("Réunion", "RE", "REU", 638, new[] { "262" }),
                new ISO3166Country("Romania", "RO", "ROU", 642, new[] { "40" }, true, localizationInfo: new[] { new LocalizationInfo("ro-RO", "Romanian") }),
                new ISO3166Country("Russian Federation", "RU", "RUS", 643, new[] { "7" }, localizationInfo: new[] { new LocalizationInfo("ru-RU", "Russian") }),
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
                new ISO3166Country("Saudi Arabia", "SA", "SAU", 682, new[] { "966" }, localizationInfo: new[] { new LocalizationInfo("ar-SA", "Arabic") }),
                new ISO3166Country("Senegal", "SN", "SEN", 686, new[] { "221" }),
                new ISO3166Country("Serbia", "RS", "SRB", 688, new[] { "381" }, localizationInfo: new[] { new LocalizationInfo("sr-Cyrl-RS", "Serbian (Cyrillic)"), new LocalizationInfo("sr-Latn-RS", "Serbian (Latin)") }),
                new ISO3166Country("Seychelles", "SC", "SYC", 690, new[] { "248" }),
                new ISO3166Country("Sierra Leone", "SL", "SLE", 694, new[] { "232" }),
                new ISO3166Country("Singapore", "SG", "SGP", 702, new[] { "65" }),
                new ISO3166Country("Sint Maarten (Dutch part)", "SX", "SXM", 534, new[] { "1 721" }),
                new ISO3166Country("Slovakia", "SK", "SVK", 703, new[] { "421" }, true, localizationInfo: new[] { new LocalizationInfo("sk-SK", "Slovak") }),
                new ISO3166Country("Slovenia", "SI", "SVN", 705, new[] { "386" }, true, localizationInfo: new[] { new LocalizationInfo("sl-SI", "Slovenian") }),
                new ISO3166Country("Solomon Islands", "SB", "SLB", 90, new[] { "677" }),
                new ISO3166Country("Somalia", "SO", "SOM", 706, new[] { "252" }),
                new ISO3166Country("South Africa", "ZA", "ZAF", 710, new[] { "27" }),
                new ISO3166Country("South Georgia and the South Sandwich Islands", "GS", "SGS", 239, new[] { "500" }),
                new ISO3166Country("South Sudan", "SS", "SSD", 728, new[] { "211" }),
                new ISO3166Country("Spain", "ES", "ESP", 724, new[] { "34" }, true, localizationInfo: new[] { new LocalizationInfo("ca-ES", "Valencian"), new LocalizationInfo("es-ES", "Spanish") }),
                new ISO3166Country("Sri Lanka", "LK", "LKA", 144, new[] { "94" }, localizationInfo: new[] { new LocalizationInfo("si-LK", "Sinhala") }),
                new ISO3166Country("Sudan", "SD", "SDN", 729, new[] { "249" }),
                new ISO3166Country("Suriname", "SR", "SUR", 740, new[] { "597" }),
                new ISO3166Country("Svalbard and Jan Mayen", "SJ", "SJM", 744, new[] { "47" }),
                new ISO3166Country("Sweden", "SE", "SWE", 752, new[] { "46" }, true, localizationInfo: new[] { new LocalizationInfo("sv-SE", "Swedish") }),
                new ISO3166Country("Switzerland", "CH", "CHE", 756, new[] { "41" }, localizationInfo: new[] { new LocalizationInfo("de-CH", "German"), new LocalizationInfo("fr-CH", "French") }),
                new ISO3166Country("Syrian Arab Republic", "SY", "SYR", 760, new[] { "963" }, localizationInfo: new[] { new LocalizationInfo("ar-SY", "Arabic") }),
                new ISO3166Country("Taiwan, Province of China", "TW", "TWN", 158, new[] { "886" }),
                new ISO3166Country("Tajikistan", "TJ", "TJK", 762, new[] { "992" }),
                new ISO3166Country("Tanzania, United Republic of", "TZ", "TZA", 834, new[] { "255" }),
                new ISO3166Country("Thailand", "TH", "THA", 764, new[] { "66" }, localizationInfo: new[] { new LocalizationInfo("th-TH", "Thai") }),
                new ISO3166Country("Timor-Leste", "TL", "TLS", 626, new[] { "670" }),
                new ISO3166Country("Togo", "TG", "TGO", 768, new[] { "228" }),
                new ISO3166Country("Tokelau", "TK", "TKL", 772, new[] { "690" }),
                new ISO3166Country("Tonga", "TO", "TON", 776, new[] { "676" }),
                new ISO3166Country("Trinidad and Tobago", "TT", "TTO", 780, new[] { "1 868" }),
                new ISO3166Country("Tunisia", "TN", "TUN", 788, new[] { "216" }, localizationInfo: new[] { new LocalizationInfo("ar-TN", "Arabic") }),
                new ISO3166Country("Turkey", "TR", "TUR", 792, new[] { "90" }, localizationInfo: new[] { new LocalizationInfo("tr-TR", "Turkish") }),
                new ISO3166Country("Turkmenistan", "TM", "TKM", 795, new[] { "993" }),
                new ISO3166Country("Turks and Caicos Islands", "TC", "TCA", 796, new[] { "1 649" }),
                new ISO3166Country("Tuvalu", "TV", "TUV", 798, new[] { "688" }),
                new ISO3166Country("Uganda", "UG", "UGA", 800, new[] { "256" }),
                new ISO3166Country("Ukraine", "UA", "UKR", 804, new[] { "380" }, localizationInfo: new[] { new LocalizationInfo("uk-UA", "Ukrainian"), new LocalizationInfo("ru-RU", "Russian") }),
                new ISO3166Country("United Arab Emirates", "AE", "ARE", 784, new[] { "971" }, localizationInfo: new[] { new LocalizationInfo("ar-AE", "Arabic") }),
                new ISO3166Country("United Kingdom of Great Britain and Northern Ireland", "GB", "GBR", 826, new[] { "44" }),
                new ISO3166Country("United States Minor Outlying Islands", "UM", "UMI", 581),
                new ISO3166Country("United States of America", "US", "USA", 840, new[] { "1" }),
                new ISO3166Country("Uruguay", "UY", "URY", 858, new[] { "598" }, localizationInfo: new[] { new LocalizationInfo("es-UY", "Spanish") }),
                new ISO3166Country("Uzbekistan", "UZ", "UZB", 860, new[] { "998" }),
                new ISO3166Country("Vanuatu", "VU", "VUT", 548, new[] { "678" }),
                new ISO3166Country("Venezuela (Bolivarian Republic of)", "VE", "VEN", 862, new[] { "58" }, localizationInfo: new[] { new LocalizationInfo("es-VE", "Spanish") }),
                new ISO3166Country("Vietnam", "VN", "VNM", 704, new[] { "84" }, localizationInfo: new[] { new LocalizationInfo("vi-VN", "Vietnamese") }),
                new ISO3166Country("Virgin Islands (British)", "VG", "VGB", 92, new[] { "1 284" }),
                new ISO3166Country("Virgin Islands (U.S.)", "VI", "VIR", 850, new[] { "1 340" }),
                new ISO3166Country("Wallis and Futuna", "WF", "WLF", 876, new[] { "681" }),
                new ISO3166Country("Western Sahara", "EH", "ESH", 732, new[] { "212" }),
                new ISO3166Country("Yemen", "YE", "YEM", 887, new[] { "967" }, localizationInfo: new[] { new LocalizationInfo("ar-YE", "Arabic") }),
                new ISO3166Country("Zambia", "ZM", "ZMB", 894, new[] { "260" }),
                new ISO3166Country("Zimbabwe", "ZW", "ZWE", 716, new[] { "263" })
             };
        }
        #endregion
    }

    /// <summary>
    /// Representation of an ISO3166-1 Country
    /// </summary>
    public partial class ISO3166Country
    {
        public ISO3166Country(string name, string alpha2, string alpha3, int numericCode, string[] dialCodes = null, bool subjectToVat = false, IEnumerable<LocalizationInfo> localizationInfo = null)
        {
            Name = name;
            Alpha2 = alpha2;
            Alpha3 = alpha3;
            NumericCode = numericCode;
            DialCodes = dialCodes;
            SubjectToVat = subjectToVat;
            LocalizationInfo = localizationInfo ?? (new[] { new LocalizationInfo("en-US", "English") });
        }

        /// <summary>
        ///English short name of country
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Two-letter country code
        /// </summary>
        public string Alpha2 { get; private set; }

        /// <summary>
        /// three-letter country code which allow a better visual association between the codes and the country names than the alpha-2 codes
        /// </summary>
        public string Alpha3 { get; private set; }

        /// <summary>
        /// Three-digit country code which are identical to those developed and maintained by the United Nations Statistics Division
        /// </summary>
        public int NumericCode { get; private set; }

        /// <summary>
        /// Phone codes
        /// </summary>
        public string[] DialCodes { get; private set; }

        /// <summary>
        /// Belonging to the European Union
        /// </summary>
        public bool SubjectToVat { get; private set; }

        public IEnumerable<LocalizationInfo> LocalizationInfo { get; private set; }
    }

    public partial class LocalizationInfo
    {
        public LocalizationInfo(string culture, string language)
        {
            Culture = culture;
            Language = language;
        }

        public string Culture { get; private set; }

        public string Language { get; private set; }
    }
}
