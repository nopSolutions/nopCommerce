using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Country lookup helper
    /// </summary>
    public partial class GeoCountryLookup : IGeoCountryLookup
    {
        #region Fields
        private long COUNTRY_BEGIN = 16776960;
        private readonly string[] _countryCode = 
								{ "--","AP","EU","AD","AE","AF","AG","AI","AL","AM","AN","AO","AQ","AR","AS","AT","AU","AW","AZ","BA","BB","BD","BE","BF","BG","BH","BI","BJ","BM","BN","BO","BR","BS","BT","BV","BW","BY","BZ","CA","CC","CD","CF","CG","CH","CI","CK","CL","CM","CN","CO","CR","CU","CV","CX","CY","CZ","DE","DJ","DK","DM","DO","DZ",
									"EC","EE","EG","EH","ER","ES","ET","FI","FJ","FK","FM","FO","FR","FX","GA","GB","GD","GE","GF","GH","GI","GL","GM","GN","GP","GQ","GR","GS","GT","GU","GW","GY","HK","HM","HN","HR","HT","HU","ID","IE","IL","IN","IO","IQ","IR","IS","IT","JM","JO","JP","KE","KG","KH","KI","KM","KN","KP","KR","KW","KY","KZ",
									"LA","LB","LC","LI","LK","LR","LS","LT","LU","LV","LY","MA","MC","MD","MG","MH","MK","ML","MM","MN","MO","MP","MQ","MR","MS","MT","MU","MV","MW","MX","MY","MZ","NA","NC","NE","NF","NG","NI","NL","NO","NP","NR","NU","NZ","OM","PA","PE","PF","PG","PH","PK","PL","PM","PN","PR","PS","PT","PW","PY","QA",
									"RE","RO","RU","RW","SA","SB","SC","SD","SE","SG","SH","SI","SJ","SK","SL","SM","SN","SO","SR","ST","SV","SY","SZ","TC","TD","TF","TG","TH","TJ","TK","TM","TN","TO","TL","TR","TT","TV","TW","TZ","UA","UG","UM","US","UY","UZ","VA","VC","VE","VG","VI","VN","VU","WF","WS","YE","YT","RS","ZA","ZM","ME","ZW","A1","A2",
									"O1","AX","GG","IM","JE","BL","MF"
									};
        private readonly string[] _countryName = 
								{"N/A","Asia/Pacific Region","Europe","Andorra","United Arab Emirates","Afghanistan","Antigua and Barbuda","Anguilla","Albania","Armenia","Netherlands Antilles","Angola","Antarctica","Argentina","American Samoa","Austria","Australia","Aruba","Azerbaijan","Bosnia and Herzegovina","Barbados","Bangladesh","Belgium",
									"Burkina Faso","Bulgaria","Bahrain","Burundi","Benin","Bermuda","Brunei Darussalam","Bolivia","Brazil","Bahamas","Bhutan","Bouvet Island","Botswana","Belarus","Belize","Canada","Cocos (Keeling) Islands","Congo, The Democratic Republic of the","Central African Republic","Congo","Switzerland","Cote D'Ivoire",
									"Cook Islands","Chile","Cameroon","China","Colombia","Costa Rica","Cuba","Cape Verde","Christmas Island","Cyprus","Czech Republic","Germany","Djibouti","Denmark","Dominica","Dominican Republic","Algeria","Ecuador","Estonia","Egypt","Western Sahara","Eritrea","Spain","Ethiopia","Finland","Fiji","Falkland Islands (Malvinas)",
									"Micronesia, Federated States of","Faroe Islands","France","France, Metropolitan","Gabon","United Kingdom","Grenada","Georgia","French Guiana","Ghana","Gibraltar","Greenland","Gambia","Guinea","Guadeloupe","Equatorial Guinea","Greece","South Georgia and the South Sandwich Islands","Guatemala","Guam","Guinea-Bissau","Guyana",
									"Hong Kong","Heard Island and McDonald Islands","Honduras","Croatia","Haiti","Hungary","Indonesia","Ireland","Israel","India","British Indian Ocean Territory","Iraq","Iran, Islamic Republic of","Iceland","Italy","Jamaica","Jordan","Japan","Kenya","Kyrgyzstan","Cambodia","Kiribati","Comoros","Saint Kitts and Nevis",
									"Korea, Democratic People's Republic of","Korea, Republic of","Kuwait","Cayman Islands","Kazakstan","Lao People's Democratic Republic","Lebanon","Saint Lucia","Liechtenstein","Sri Lanka","Liberia","Lesotho","Lithuania","Luxembourg","Latvia","Libyan Arab Jamahiriya","Morocco","Monaco","Moldova, Republic of","Madagascar",
									"Marshall Islands","Macedonia","Mali","Myanmar","Mongolia","Macau","Northern Mariana Islands","Martinique","Mauritania","Montserrat","Malta","Mauritius","Maldives","Malawi","Mexico","Malaysia","Mozambique","Namibia","New Caledonia","Niger","Norfolk Island","Nigeria","Nicaragua","Netherlands",
									"Norway","Nepal","Nauru","Niue","New Zealand","Oman","Panama","Peru","French Polynesia","Papua New Guinea","Philippines","Pakistan","Poland","Saint Pierre and Miquelon","Pitcairn Islands","Puerto Rico","Palestinian Territory","Portugal","Palau","Paraguay","Qatar","Reunion","Romania","Russian Federation","Rwanda","Saudi Arabia",
									"Solomon Islands","Seychelles","Sudan","Sweden","Singapore","Saint Helena","Slovenia","Svalbard and Jan Mayen","Slovakia","Sierra Leone","San Marino","Senegal","Somalia","Suriname","Sao Tome and Principe","El Salvador","Syrian Arab Republic","Swaziland","Turks and Caicos Islands","Chad","French Southern Territories","Togo",
									"Thailand","Tajikistan","Tokelau","Turkmenistan","Tunisia","Tonga","Timor-Leste","Turkey","Trinidad and Tobago","Tuvalu","Taiwan","Tanzania, United Republic of","Ukraine","Uganda","United States Minor Outlying Islands","United States","Uruguay","Uzbekistan","Holy See (Vatican City State)","Saint Vincent and the Grenadines",
									"Venezuela","Virgin Islands, British","Virgin Islands, U.S.","Vietnam","Vanuatu","Wallis and Futuna","Samoa","Yemen","Mayotte","Serbia","South Africa","Zambia","Montenegro","Zimbabwe","Anonymous Proxy","Satellite Provider",
									"Other","Aland Islands","Guernsey","Isle of Man","Jersey","Saint Barthelemy","Saint Martin"};

        #endregion

        #region Utilities

        protected virtual long AddrToNum(IPAddress addr)
        {
            long ipnum = 0;
            byte[] b = addr.GetAddressBytes();
            for (int i = 0; i < 4; ++i)
            {
                long y = b[i];
                if (y < 0)
                {
                    y += 256;
                }
                ipnum += y << ((3 - i) * 8);
            }
            Debug.WriteLine(ipnum);
            return ipnum;
        }

        protected virtual long SeekCountry(long offset, long ipnum, int depth)
        {
            byte[] buf = new byte[6];
            long[] x = new long[2];
            if (depth < 0)
            {
                Debug.WriteLine("Error seeking country.");
                return 0; // N/A
            }
            try
            {
                //you can download the latest GeoIP database here http://www.maxmind.com/app/free
                string fileName = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    fileName = String.Format("{0}App_Data\\GeoIP.dat", HttpContext.Current.Request.PhysicalApplicationPath);
                }
                using (var fileInput = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    fileInput.Seek(6 * offset, 0);
                    fileInput.Read(buf, 0, 6);
                }
            }
            catch (IOException)
            {
                Debug.WriteLine("IO Exception");
                return 0; // N/A
            }
            for (int i = 0; i < 2; i++)
            {
                x[i] = 0;
                for (int j = 0; j < 3; j++)
                {
                    int y = buf[i * 3 + j];
                    if (y < 0)
                    {
                        y += 256;
                    }
                    x[i] += (y << (j * 8));
                }
            }

            if ((ipnum & (1 << depth)) > 0)
            {
                if (x[1] >= COUNTRY_BEGIN)
                {
                    return x[1] - COUNTRY_BEGIN;
                }
                return SeekCountry(x[1], ipnum, depth - 1);
            }
            else
            {
                if (x[0] >= COUNTRY_BEGIN)
                {
                    return x[0] - COUNTRY_BEGIN;
                }
                return SeekCountry(x[0], ipnum, depth - 1);
            }
        }

        #endregion

        #region Methods

        public virtual string LookupCountryCode(string str)
        {
            if (String.IsNullOrEmpty(str))
                return string.Empty;

            IPAddress addr;
            try
            {
                addr = IPAddress.Parse(str);
            }
            catch
            {
                return string.Empty;
            }
            return LookupCountryCode(addr);
        }

        public virtual string LookupCountryCode(IPAddress addr)
        {
            string code = (_countryCode[(int)SeekCountry(0, AddrToNum(addr), 31)]);
            if (code == "--")
                code = string.Empty;
            return string.Empty;
        }

        public virtual string LookupCountryName(string str)
        {
            if (String.IsNullOrEmpty(str))
                return string.Empty;

            IPAddress addr;
            try
            {
                addr = IPAddress.Parse(str);
            }
            catch
            {
                return string.Empty;
            }
            return LookupCountryName(addr);
        }

        public virtual string LookupCountryName(IPAddress addr)
        {
            return (_countryName[(int)SeekCountry(0, AddrToNum(addr), 31)]);
        }

        #endregion
    }
}