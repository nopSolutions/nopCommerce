using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Nop.Core.ComponentModel;
using Nop.Core.Domain.Shipping;
using System.Linq;
using System.Web.Hosting;
using System.IO;
using System.Net;

namespace Nop.Core
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial class CommonHelper
    {
        /// <summary>
        /// Ensures the subscriber email or throw.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static string EnsureSubscriberEmailOrThrow(string email)
        {
            string output = EnsureNotNull(email);
            output = output.Trim();
            output = EnsureMaximumLength(output, 255);

            if (!IsValidEmail(output))
            {
                throw new NopException("Email is not valid.");
            }

            return output;
        }

        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public static bool IsValidEmail(string email)
        {
            var validator = new EmailValidator();
            return validator.IsValidEmail(email);
        }

        /// <summary>
        /// Verifies that string is an valid IP-Address
        /// </summary>
        /// <param name="ipAddress">IPAddress to verify</param>
        /// <returns>true if the string is a valid IpAddress and false if it's not</returns>
        public static bool IsValidIpAddress(string ipAddress)
        {
            IPAddress ip;
            return IPAddress.TryParse(ipAddress, out ip);
        }

        /// <summary>
        /// Generate random digit code
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString());
            return str;
        }

        /// <summary>
        /// Returns an random interger number within a specified rage
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="postfix">A string to add to the end if the original string was shorten</param>
        /// <returns>Input string if its lengh is OK; otherwise, truncated input string</returns>
        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (str.Length > maxLength)
            {
                var pLen = postfix == null ? 0 : postfix.Length;

                var result = str.Substring(0, maxLength - pLen);
                if (!String.IsNullOrEmpty(postfix))
                {
                    result += postfix;
                }
                return result;
            }

            return str;
        }

        /// <summary>
        /// Ensures that a string only contains numeric values
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Input string with only numeric values, empty string if input is null/empty</returns>
        public static string EnsureNumericOnly(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : new string(str.Where(p => char.IsDigit(p)).ToArray());
        }

        /// <summary>
        /// Ensure that a string is not null
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Result</returns>
        public static string EnsureNotNull(string str)
        {
            return str ?? string.Empty;
        }

        /// <summary>
        /// Indicates whether the specified strings are null or empty strings
        /// </summary>
        /// <param name="stringsToValidate">Array of strings to validate</param>
        /// <returns>Boolean</returns>
        public static bool AreNullOrEmpty(params string[] stringsToValidate)
        {
            return stringsToValidate.Any(p => string.IsNullOrEmpty(p));
        }

        /// <summary>
        /// Compare two arrasy
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="a1">Array 1</param>
        /// <param name="a2">Array 2</param>
        /// <returns>Result</returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            //also see Enumerable.SequenceEqual(a1, a2);
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        private static AspNetHostingPermissionLevel? _trustLevel;
        /// <summary>
        /// Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                //set minimum
                _trustLevel = AspNetHostingPermissionLevel.None;

                //determine maximum
                foreach (AspNetHostingPermissionLevel trustLevel in new[] {
                                AspNetHostingPermissionLevel.Unrestricted,
                                AspNetHostingPermissionLevel.High,
                                AspNetHostingPermissionLevel.Medium,
                                AspNetHostingPermissionLevel.Low,
                                AspNetHostingPermissionLevel.Minimal
                            })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        _trustLevel = trustLevel;
                        break; //we've set the highest permission we can
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }
                }
            }
            return _trustLevel.Value;
        }

        /// <summary>
        /// Sets a property on an object to a valuae.
        /// </summary>
        /// <param name="instance">The object whose property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            Type instanceType = instance.GetType();
            PropertyInfo pi = instanceType.GetProperty(propertyName);
            if (pi == null)
                throw new NopException("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);
            if (!pi.CanWrite)
                throw new NopException("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType);
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = To(value, pi.PropertyType);
            pi.SetValue(instance, value, new object[0]);
        }

        public static TypeConverter GetNopCustomTypeConverter(Type type)
        {
            //we can't use the following code in order to register our custom type descriptors
            //TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            //so we do it manually here

            if (type == typeof(List<int>))
                return new GenericListTypeConverter<int>();
            if (type == typeof(List<decimal>))
                return new GenericListTypeConverter<decimal>();
            if (type == typeof(List<string>))
                return new GenericListTypeConverter<string>();
            if (type == typeof(ShippingOption))
                return new ShippingOptionTypeConverter();
            if (type == typeof(List<ShippingOption>) || type == typeof(IList<ShippingOption>))
                return new ShippingOptionListTypeConverter();
            if (type == typeof(PickupPoint))
                return new PickupPointTypeConverter();
            if (type == typeof(Dictionary<int, int>))
                return new GenericDictionaryTypeConverter<int, int>();

            return TypeDescriptor.GetConverter(type);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();

                TypeConverter destinationConverter = GetNopCustomTypeConverter(destinationType);
                TypeConverter sourceConverter = GetNopCustomTypeConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);
                if (!destinationType.IsInstanceOfType(value))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }

        /// <summary>
        /// Convert enum for front-end
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Converted string</returns>
        public static string ConvertEnum(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            string result = string.Empty;
            foreach (var c in str)
                if (c.ToString() != c.ToString().ToLower())
                    result += " " + c.ToString();
                else
                    result += c.ToString();
            return result;
        }

        /// <summary>
        /// Set Telerik (Kendo UI) culture
        /// </summary>
        public static void SetTelerikCulture()
        {
            //little hack here
            //always set culture to 'en-US' (Kendo UI has a bug related to editing decimal values in other cultures). Like currently it's done for admin area in Global.asax.cs

            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Get difference in years
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            //source: http://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
            //this assumes you are looking for the western idea of age and not using East Asian reckoning.
            int age = endDate.Year - startDate.Year;
            if (startDate > endDate.AddYears(-age))
                age--;
            return age;
        }

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public static string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }

            //not hosted. For example, run in unit tests
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        // Email Validator's code has been taken from Microsoft examples
        // https://msdn.microsoft.com/en-us/library/01escwtf(v=vs.110).aspx
        private class EmailValidator
        {
            bool invalid = false;

            public bool IsValidEmail(string strIn)
            {
                invalid = false;
                if (String.IsNullOrEmpty(strIn))
                    return false;

                // Use IdnMapping class to convert Unicode domain names.
                try
                {
                    strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                          RegexOptions.None, TimeSpan.FromMilliseconds(200));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }

                if (invalid)
                    return false;

                // Return true if strIn is in valid e-mail format.
                try
                {
                    return Regex.IsMatch(strIn,
                          @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                          @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                          RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }
            }

            private string DomainMapper(Match match)
            {
                // IdnMapping class with default property values.
                IdnMapping idn = new IdnMapping();

                string domainName = match.Groups[2].Value;
                try
                {
                    domainName = idn.GetAscii(domainName);
                    var parts = domainName.Split('.');
                    if (parts.Length < 2 || !RootDomains.Contains(parts.Last()))
                    {
                        invalid = true;
                    }
                }
                catch (ArgumentException)
                {
                    invalid = true;
                }
                return match.Groups[1].Value + domainName;
            }

            #region Root domain names

            private static string[] RootDomains =
            {
                "aaa",
                "aarp",
                "abarth",
                "abb",
                "abbott",
                "abbvie",
                "abc",
                "able",
                "abogado",
                "abudhabi",
                "ac",
                "academy",
                "accenture",
                "accountant",
                "accountants",
                "aco",
                "active",
                "actor",
                "ad",
                "adac",
                "ads",
                "adult",
                "ae",
                "aeg",
                "aero",
                "aetna",
                "af",
                "afamilycompany",
                "afl",
                "ag",
                "agakhan",
                "agency",
                "ai",
                "aig",
                "airbus",
                "airforce",
                "airtel",
                "akdn",
                "al",
                "alfaromeo",
                "alibaba",
                "alipay",
                "allfinanz",
                "allstate",
                "ally",
                "alsace",
                "alstom",
                "am",
                "americanfamily",
                "amfam",
                "amica",
                "amsterdam",
                "an",
                "analytics",
                "android",
                "anquan",
                "anz",
                "ao",
                "apartments",
                "app",
                "apple",
                "aq",
                "aquarelle",
                "ar",
                "aramco",
                "archi",
                "army",
                "arpa",
                "art",
                "arte",
                "as",
                "asia",
                "associates",
                "at",
                "athleta",
                "attorney",
                "au",
                "auction",
                "audi",
                "audible",
                "audio",
                "author",
                "auto",
                "autos",
                "avianca",
                "aw",
                "aws",
                "ax",
                "axa",
                "az",
                "azure",
                "ba",
                "baby",
                "baidu",
                "banamex",
                "bananarepublic",
                "band",
                "bank",
                "bar",
                "barcelona",
                "barclaycard",
                "barclays",
                "barefoot",
                "bargains",
                "bauhaus",
                "bayern",
                "bb",
                "bbc",
                "bbt",
                "bbva",
                "bcg",
                "bcn",
                "bd",
                "be",
                "beats",
                "beauty",
                "beer",
                "bentley",
                "berlin",
                "best",
                "bestbuy",
                "bet",
                "bf",
                "bg",
                "bh",
                "bharti",
                "bi",
                "bible",
                "bid",
                "bike",
                "bing",
                "bingo",
                "bio",
                "biz",
                "bj",
                "bl",
                "black",
                "blackfriday",
                "blanco",
                "blockbuster",
                "blog",
                "bloomberg",
                "blue",
                "bm",
                "bms",
                "bmw",
                "bn",
                "bnl",
                "bnpparibas",
                "bo",
                "boats",
                "boehringer",
                "bofa",
                "bom",
                "bond",
                "boo",
                "book",
                "booking",
                "boots",
                "bosch",
                "bostik",
                "bot",
                "boutique",
                "bq",
                "br",
                "bradesco",
                "bridgestone",
                "broadway",
                "broker",
                "brother",
                "brussels",
                "bs",
                "bt",
                "budapest",
                "bugatti",
                "build",
                "builders",
                "business",
                "buy",
                "buzz",
                "bv",
                "bw",
                "by",
                "bz",
                "bzh",
                "ca",
                "cab",
                "cafe",
                "cal",
                "call",
                "calvinklein",
                "cam",
                "camera",
                "camp",
                "cancerresearch",
                "canon",
                "capetown",
                "capital",
                "car",
                "caravan",
                "cards",
                "care",
                "career",
                "careers",
                "cars",
                "cartier",
                "casa",
                "cash",
                "casino",
                "cat",
                "catering",
                "cba",
                "cbn",
                "cbre",
                "cbs",
                "cc",
                "cd",
                "ceb",
                "center",
                "ceo",
                "cern",
                "cf",
                "cfa",
                "cfd",
                "cg",
                "ch",
                "chanel",
                "channel",
                "chase",
                "chat",
                "cheap",
                "chintai",
                "chloe",
                "christmas",
                "chrome",
                "chrysler",
                "church",
                "ci",
                "cipriani",
                "circle",
                "cisco",
                "citadel",
                "citi",
                "citic",
                "city",
                "cityeats",
                "ck",
                "cl",
                "claims",
                "cleaning",
                "click",
                "clinic",
                "clinique",
                "clothing",
                "cloud",
                "club",
                "clubmed",
                "cm",
                "cn",
                "co",
                "coach",
                "codes",
                "coffee",
                "college",
                "cologne",
                "com",
                "comcast",
                "commbank",
                "community",
                "company",
                "compare",
                "computer",
                "comsec",
                "condos",
                "construction",
                "consulting",
                "contact",
                "contractors",
                "cooking",
                "cookingchannel",
                "cool",
                "coop",
                "corsica",
                "country",
                "coupon",
                "coupons",
                "courses",
                "cr",
                "credit",
                "creditcard",
                "creditunion",
                "cricket",
                "crown",
                "crs",
                "cruises",
                "csc",
                "cu",
                "cuisinella",
                "cv",
                "cw",
                "cx",
                "cy",
                "cymru",
                "cyou",
                "cz",
                "dabur",
                "dad",
                "dance",
                "date",
                "dating",
                "datsun",
                "day",
                "dclk",
                "dds",
                "de",
                "deal",
                "dealer",
                "deals",
                "degree",
                "delivery",
                "dell",
                "deloitte",
                "delta",
                "democrat",
                "dental",
                "dentist",
                "desi",
                "design",
                "dev",
                "dhl",
                "diamonds",
                "diet",
                "digital",
                "direct",
                "directory",
                "discount",
                "discover",
                "dj",
                "dk",
                "dm",
                "dnp",
                "do",
                "docs",
                "doctor",
                "dodge",
                "dog",
                "doha",
                "domains",
                "doosan",
                "dot",
                "download",
                "drive",
                "dtv",
                "dubai",
                "duck",
                "dunlop",
                "duns",
                "dupont",
                "durban",
                "dvag",
                "dz",
                "earth",
                "eat",
                "ec",
                "edeka",
                "edu",
                "education",
                "ee",
                "eg",
                "eh",
                "email",
                "emerck",
                "energy",
                "engineer",
                "engineering",
                "enterprises",
                "epost",
                "epson",
                "equipment",
                "er",
                "ericsson",
                "erni",
                "es",
                "esq",
                "estate",
                "esurance",
                "et",
                "eu",
                "eurovision",
                "eus",
                "events",
                "everbank",
                "exchange",
                "expert",
                "exposed",
                "express",
                "extraspace",
                "fage",
                "fail",
                "fairwinds",
                "faith",
                "family",
                "fan",
                "fans",
                "farm",
                "farmers",
                "fashion",
                "fast",
                "fedex",
                "feedback",
                "ferrari",
                "ferrero",
                "fi",
                "fiat",
                "fidelity",
                "film",
                "final",
                "finance",
                "financial",
                "fire",
                "firestone",
                "firmdale",
                "fish",
                "fishing",
                "fit",
                "fitness",
                "fj",
                "fk",
                "flickr",
                "flights",
                "flir",
                "florist",
                "flowers",
                "flsmidth",
                "fly",
                "fm",
                "fo",
                "foo",
                "foodnetwork",
                "football",
                "ford",
                "forex",
                "forsale",
                "forum",
                "foundation",
                "fox",
                "fr",
                "fresenius",
                "frl",
                "frogans",
                "frontdoor",
                "frontier",
                "ftr",
                "fujitsu",
                "fujixerox",
                "fund",
                "furniture",
                "futbol",
                "fyi",
                "ga",
                "gal",
                "gallery",
                "gallo",
                "gallup",
                "game",
                "games",
                "gap",
                "garden",
                "gb",
                "gbiz",
                "gd",
                "gdn",
                "ge",
                "gea",
                "gent",
                "genting",
                "gf",
                "gg",
                "ggee",
                "gh",
                "gi",
                "gift",
                "gifts",
                "gives",
                "giving",
                "gl",
                "glade",
                "glass",
                "gle",
                "global",
                "globo",
                "gm",
                "gmail",
                "gmbh",
                "gmo",
                "gmx",
                "gn",
                "godaddy",
                "gold",
                "goldpoint",
                "golf",
                "goo",
                "goodhands",
                "goodyear",
                "goog",
                "google",
                "gop",
                "got",
                "gov",
                "gp",
                "gq",
                "gr",
                "grainger",
                "graphics",
                "gratis",
                "green",
                "gripe",
                "group",
                "gs",
                "gt",
                "gu",
                "guardian",
                "gucci",
                "guge",
                "guide",
                "guitars",
                "guru",
                "gw",
                "gy",
                "hamburg",
                "hangout",
                "haus",
                "hdfcbank",
                "health",
                "healthcare",
                "help",
                "helsinki",
                "here",
                "hermes",
                "hgtv",
                "hiphop",
                "hisamitsu",
                "hitachi",
                "hiv",
                "hk",
                "hkt",
                "hm",
                "hn",
                "hockey",
                "holdings",
                "holiday",
                "homedepot",
                "homegoods",
                "homes",
                "homesense",
                "honda",
                "honeywell",
                "horse",
                "host",
                "hosting",
                "hoteles",
                "hotmail",
                "house",
                "how",
                "hr",
                "hsbc",
                "ht",
                "htc",
                "hu",
                "hyatt",
                "hyundai",
                "ibm",
                "icbc",
                "ice",
                "icu",
                "id",
                "ie",
                "ieee",
                "ifm",
                "iinet",
                "ikano",
                "il",
                "im",
                "imamat",
                "imdb",
                "immo",
                "immobilien",
                "in",
                "industries",
                "infiniti",
                "info",
                "ing",
                "ink",
                "institute",
                "insurance",
                "insure",
                "int",
                "intel",
                "international",
                "intuit",
                "investments",
                "io",
                "ipiranga",
                "iq",
                "ir",
                "irish",
                "is",
                "iselect",
                "ismaili",
                "ist",
                "istanbul",
                "it",
                "itau",
                "itv",
                "iwc",
                "jaguar",
                "java",
                "jcb",
                "jcp",
                "je",
                "jeep",
                "jetzt",
                "jewelry",
                "jlc",
                "jll",
                "jm",
                "jmp",
                "jnj",
                "jo",
                "jobs",
                "joburg",
                "jot",
                "joy",
                "jp",
                "jpmorgan",
                "jprs",
                "juegos",
                "juniper",
                "kaufen",
                "kddi",
                "ke",
                "kerryhotels",
                "kerrylogistics",
                "kerryproperties",
                "kfh",
                "kg",
                "kh",
                "ki",
                "kia",
                "kim",
                "kinder",
                "kindle",
                "kitchen",
                "kiwi",
                "km",
                "kn",
                "koeln",
                "komatsu",
                "kosher",
                "kp",
                "kpmg",
                "kpn",
                "kr",
                "krd",
                "kred",
                "kuokgroup",
                "kw",
                "ky",
                "kyoto",
                "kz",
                "la",
                "lacaixa",
                "ladbrokes",
                "lamborghini",
                "lamer",
                "lancaster",
                "lancia",
                "lancome",
                "land",
                "landrover",
                "lanxess",
                "lasalle",
                "lat",
                "latino",
                "latrobe",
                "law",
                "lawyer",
                "lb",
                "lc",
                "lds",
                "lease",
                "leclerc",
                "lefrak",
                "legal",
                "lego",
                "lexus",
                "lgbt",
                "li",
                "liaison",
                "lidl",
                "life",
                "lifeinsurance",
                "lifestyle",
                "lighting",
                "like",
                "lilly",
                "limited",
                "limo",
                "lincoln",
                "linde",
                "link",
                "lipsy",
                "live",
                "living",
                "lixil",
                "lk",
                "loan",
                "loans",
                "locker",
                "locus",
                "loft",
                "lol",
                "london",
                "lotte",
                "lotto",
                "love",
                "lpl",
                "lplfinancial",
                "lr",
                "ls",
                "lt",
                "ltd",
                "ltda",
                "lu",
                "lundbeck",
                "lupin",
                "luxe",
                "luxury",
                "lv",
                "ly",
                "ma",
                "macys",
                "madrid",
                "maif",
                "maison",
                "makeup",
                "man",
                "management",
                "mango",
                "market",
                "marketing",
                "markets",
                "marriott",
                "marshalls",
                "mattel",
                "mba",
                "mc",
                "mckinsey",
                "md",
                "me",
                "med",
                "media",
                "meet",
                "melbourne",
                "meme",
                "memorial",
                "men",
                "menu",
                "meo",
                "metlife",
                "mf",
                "mg",
                "mh",
                "miami",
                "microsoft",
                "mil",
                "mini",
                "mint",
                "mit",
                "mitsubishi",
                "mk",
                "ml",
                "mlb",
                "mls",
                "mm",
                "mma",
                "mn",
                "mo",
                "mobi",
                "mobily",
                "moda",
                "moe",
                "moi",
                "mom",
                "monash",
                "money",
                "montblanc",
                "mopar",
                "mormon",
                "mortgage",
                "moscow",
                "motorcycles",
                "mov",
                "movie",
                "movistar",
                "mp",
                "mq",
                "mr",
                "ms",
                "msd",
                "mt",
                "mtn",
                "mtpc",
                "mtr",
                "mu",
                "museum",
                "mutual",
                "mutuelle",
                "mv",
                "mw",
                "mx",
                "my",
                "mz",
                "na",
                "nadex",
                "nagoya",
                "name",
                "nationwide",
                "natura",
                "navy",
                "nba",
                "nc",
                "ne",
                "nec",
                "net",
                "netbank",
                "netflix",
                "network",
                "neustar",
                "new",
                "news",
                "next",
                "nextdirect",
                "nexus",
                "nf",
                "nfl",
                "ng",
                "ngo",
                "nhk",
                "ni",
                "nico",
                "nike",
                "nikon",
                "ninja",
                "nissan",
                "nissay",
                "nl",
                "no",
                "nokia",
                "northwesternmutual",
                "norton",
                "now",
                "nowruz",
                "nowtv",
                "np",
                "nr",
                "nra",
                "nrw",
                "ntt",
                "nu",
                "nyc",
                "nz",
                "obi",
                "off",
                "office",
                "okinawa",
                "olayan",
                "olayangroup",
                "oldnavy",
                "ollo",
                "om",
                "omega",
                "one",
                "ong",
                "onl",
                "online",
                "onyourside",
                "ooo",
                "oracle",
                "orange",
                "org",
                "organic",
                "orientexpress",
                "origins",
                "osaka",
                "otsuka",
                "ott",
                "ovh",
                "pa",
                "page",
                "pamperedchef",
                "panasonic",
                "panerai",
                "paris",
                "pars",
                "partners",
                "parts",
                "party",
                "passagens",
                "pccw",
                "pe",
                "pet",
                "pf",
                "pfizer",
                "pg",
                "ph",
                "pharmacy",
                "philips",
                "photo",
                "photography",
                "photos",
                "physio",
                "piaget",
                "pics",
                "pictet",
                "pictures",
                "pid",
                "pin",
                "ping",
                "pink",
                "pioneer",
                "pizza",
                "pk",
                "pl",
                "place",
                "play",
                "playstation",
                "plumbing",
                "plus",
                "pm",
                "pn",
                "pnc",
                "pohl",
                "poker",
                "politie",
                "porn",
                "post",
                "pr",
                "pramerica",
                "praxi",
                "press",
                "prime",
                "pro",
                "prod",
                "productions",
                "prof",
                "progressive",
                "promo",
                "properties",
                "property",
                "protection",
                "pru",
                "prudential",
                "ps",
                "pt",
                "pub",
                "pw",
                "pwc",
                "py",
                "qa",
                "qpon",
                "quebec",
                "quest",
                "qvc",
                "racing",
                "raid",
                "re",
                "read",
                "realestate",
                "realtor",
                "realty",
                "recipes",
                "red",
                "redstone",
                "redumbrella",
                "rehab",
                "reise",
                "reisen",
                "reit",
                "ren",
                "rent",
                "rentals",
                "repair",
                "report",
                "republican",
                "rest",
                "restaurant",
                "review",
                "reviews",
                "rexroth",
                "rich",
                "richardli",
                "ricoh",
                "rightathome",
                "rio",
                "rip",
                "ro",
                "rocher",
                "rocks",
                "rodeo",
                "room",
                "rs",
                "rsvp",
                "ru",
                "ruhr",
                "run",
                "rw",
                "rwe",
                "ryukyu",
                "sa",
                "saarland",
                "safe",
                "safety",
                "sakura",
                "sale",
                "salon",
                "samsung",
                "sandvik",
                "sandvikcoromant",
                "sanofi",
                "sap",
                "sapo",
                "sarl",
                "sas",
                "save",
                "saxo",
                "sb",
                "sbi",
                "sbs",
                "sc",
                "sca",
                "scb",
                "schaeffler",
                "schmidt",
                "scholarships",
                "school",
                "schule",
                "schwarz",
                "science",
                "scjohnson",
                "scor",
                "scot",
                "sd",
                "se",
                "seat",
                "security",
                "seek",
                "select",
                "sener",
                "services",
                "ses",
                "seven",
                "sew",
                "sex",
                "sexy",
                "sfr",
                "sg",
                "sh",
                "shangrila",
                "sharp",
                "shaw",
                "shell",
                "shia",
                "shiksha",
                "shoes",
                "shop",
                "shopping",
                "shouji",
                "show",
                "showtime",
                "shriram",
                "si",
                "silk",
                "sina",
                "singles",
                "site",
                "sj",
                "sk",
                "ski",
                "skin",
                "sky",
                "skype",
                "sl",
                "sm",
                "smart",
                "smile",
                "sn",
                "sncf",
                "so",
                "soccer",
                "social",
                "softbank",
                "software",
                "sohu",
                "solar",
                "solutions",
                "song",
                "sony",
                "soy",
                "space",
                "spiegel",
                "spot",
                "spreadbetting",
                "sr",
                "srl",
                "srt",
                "ss",
                "st",
                "stada",
                "staples",
                "star",
                "starhub",
                "statebank",
                "statefarm",
                "statoil",
                "stc",
                "stcgroup",
                "stockholm",
                "storage",
                "store",
                "stream",
                "studio",
                "study",
                "style",
                "su",
                "sucks",
                "supplies",
                "supply",
                "support",
                "surf",
                "surgery",
                "suzuki",
                "sv",
                "swatch",
                "swiftcover",
                "swiss",
                "sx",
                "sy",
                "sydney",
                "symantec",
                "systems",
                "sz",
                "tab",
                "taipei",
                "talk",
                "taobao",
                "tatamotors",
                "tatar",
                "tattoo",
                "tax",
                "taxi",
                "tc",
                "tci",
                "td",
                "tdk",
                "team",
                "tech",
                "technology",
                "tel",
                "telecity",
                "telefonica",
                "temasek",
                "tennis",
                "teva",
                "tf",
                "tg",
                "th",
                "thd",
                "theater",
                "theatre",
                "tiaa",
                "tickets",
                "tienda",
                "tiffany",
                "tips",
                "tires",
                "tirol",
                "tj",
                "tjmaxx",
                "tjx",
                "tk",
                "tkmaxx",
                "tl",
                "tm",
                "tmall",
                "tn",
                "to",
                "today",
                "tokyo",
                "tools",
                "top",
                "toray",
                "toshiba",
                "total",
                "tours",
                "town",
                "toyota",
                "toys",
                "tp",
                "tr",
                "trade",
                "trading",
                "training",
                "travel",
                "travelchannel",
                "travelers",
                "travelersinsurance",
                "trust",
                "trv",
                "tt",
                "tube",
                "tui",
                "tunes",
                "tushu",
                "tv",
                "tvs",
                "tw",
                "tz",
                "ua",
                "ubs",
                "uconnect",
                "ug",
                "uk",
                "um",
                "unicom",
                "university",
                "uno",
                "uol",
                "ups",
                "us",
                "uy",
                "uz",
                "va",
                "vacations",
                "vana",
                "vc",
                "ve",
                "vegas",
                "ventures",
                "verisign",
                "versicherung",
                "vet",
                "vg",
                "vi",
                "viajes",
                "video",
                "vig",
                "viking",
                "villas",
                "vin",
                "vip",
                "virgin",
                "visa",
                "vision",
                "vista",
                "vistaprint",
                "viva",
                "vivo",
                "vlaanderen",
                "vn",
                "vodka",
                "volkswagen",
                "vote",
                "voting",
                "voto",
                "voyage",
                "vu",
                "vuelos",
                "wales",
                "walter",
                "wang",
                "wanggou",
                "warman",
                "watch",
                "watches",
                "weather",
                "weatherchannel",
                "webcam",
                "weber",
                "website",
                "wed",
                "wedding",
                "weibo",
                "weir",
                "wf",
                "whoswho",
                "wien",
                "wiki",
                "williamhill",
                "win",
                "windows",
                "wine",
                "winners",
                "wme",
                "wolterskluwer",
                "woodside",
                "work",
                "works",
                "world",
                "ws",
                "wtc",
                "wtf",
                "xbox",
                "xerox",
                "xfinity",
                "xihuan",
                "xin",
                "测试",
                "कॉम",
                "परीक्षा",
                "セール",
                "佛山",
                "ಭಾರತ",
                "慈善",
                "集团",
                "在线",
                "한국",
                "ଭାରତ",
                "点看",
                "คอม",
                "ভাৰত",
                "ভারত",
                "八卦‏",
                "موقع‎",
                "বাংলা",
                "公益",
                "公司",
                "香格里拉",
                "网站",
                "移动",
                "我爱你",
                "москва",
                "испытание",
                "қаз",
                "онлайн",
                "сайт",
                "联通",
                "срб",
                "бг",
                "бел‏",
                "קום‎",
                "时尚",
                "微博",
                "테스트",
                "淡马锡",
                "ファッション",
                "орг",
                "नेट",
                "ストア",
                "삼성",
                "சிங்கப்பூர்",
                "商标",
                "商店",
                "商城",
                "дети",
                "мкд‏",
                "טעסט‎",
                "ею",
                "ポイント",
                "新闻",
                "工行",
                "家電‏",
                "كوم‎",
                "中文网",
                "中信",
                "中国",
                "中國",
                "娱乐",
                "谷歌",
                "భారత్",
                "ලංකා",
                "電訊盈科",
                "购物",
                "測試",
                "クラウド",
                "ભારત",
                "भारत‏",
                "آزمایشی‎",
                "பரிட்சை",
                "网店",
                "संगठन",
                "餐厅",
                "网络",
                "ком",
                "укр",
                "香港",
                "诺基亚",
                "食品",
                "δοκιμή",
                "飞利浦‏",
                "إختبار‎",
                "台湾",
                "台灣",
                "手表",
                "手机",
                "мон‏",
                "الجزائر‎‏",
                "عمان‎‏",
                "ارامكو‎‏",
                "ایران‎‏",
                "العليان‎‏",
                "امارات‎‏",
                "بازار‎‏",
                "پاکستان‎‏",
                "الاردن‎‏",
                "موبايلي‎‏",
                "بھارت‎‏",
                "المغرب‎‏",
                "ابوظبي‎‏",
                "السعودية‎‏",
                "سودان‎‏",
                "همراه‎‏",
                "عراق‎‏",
                "مليسيا‎",
                "澳門",
                "닷컴",
                "政府‏",
                "شبكة‎‏",
                "بيتك‎",
                "გე",
                "机构",
                "组织机构",
                "健康",
                "ไทย‏",
                "سورية‎",
                "рус",
                "рф",
                "珠宝‏",
                "تونس‎",
                "大拿",
                "みんな",
                "グーグル",
                "ελ",
                "世界",
                "書籍",
                "ഭാരതം",
                "ਭਾਰਤ",
                "网址",
                "닷넷",
                "コム",
                "游戏",
                "vermögensberater",
                "vermögensberatung",
                "企业",
                "信息",
                "嘉里大酒店",
                "嘉里‏",
                "مصر‎‏",
                "قطر‎",
                "广东",
                "இலங்கை",
                "இந்தியா",
                "հայ",
                "新加坡‏",
                "فلسطين‎",
                "テスト",
                "政务",
                "xperia",
                "xxx",
                "xyz",
                "yachts",
                "yahoo",
                "yamaxun",
                "yandex",
                "ye",
                "yodobashi",
                "yoga",
                "yokohama",
                "you",
                "youtube",
                "yt",
                "yun",
                "za",
                "zappos",
                "zara",
                "zero",
                "zip",
                "zippo",
                "zm",
                "zone",
                "zuerich",
                "zw"
            };

            #endregion
        }
    }
}
