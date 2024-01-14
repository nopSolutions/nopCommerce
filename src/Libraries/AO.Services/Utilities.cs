using System;
using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace AO.Services
{
    public class Utilities
    {
        public static string CleanString(string value)
        {
            return value.Replace("\r", "").Trim();
        }

        public static string FirstUpperCase(string input)
        {
            input = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
            return input;
        }

        public static string GetTimeOutSettingText()
        {
            var settingsTimeout = Singleton<AppSettings>.Instance.Get<CommonConfig>().ScheduleTaskRunTimeout;
             
            int timout = settingsTimeout == null ? 5000000 : settingsTimeout.Value;
            string text = $"{Environment.NewLine}Timout setting: {string.Format("{0:hh\\:mm\\:ss}", TimeSpan.FromMilliseconds(timout))}";

            return text;
        }

        public static string PresentationPrice(decimal price, string currency = "")
        {
            if (string.IsNullOrEmpty(currency))
            {
                return price.ToString("N2");                
            }
            else 
            {
                return currency + " " + price.ToString("N2");
            }           
        }

        public enum ProductStatus
        {
            [Display(Name = "Crawled")]
            Crawled = 1,
            [Display(Name = "Prepared")]
            Prepared = 2,
            [Display(Name = "Controlled by sync")]
            ControlledBySync = 3,
            [Display(Name = "On hold")]
            OnHold = 4,
            [Display(Name = "Deactive")]
            Deactive = 5
        }

        public static string CleanupText(string stringToHaveReplaced, int languageId)
        {
            switch (languageId)
            {
                case 1: // English
                    stringToHaveReplaced = stringToHaveReplaced
                                .Replace("friliv.dk", "andersenoutdoor.com")
                                .Replace("Friliv.dk", "Andersenoutdoor.com")
                                .Replace("friliv", "andersenoutdoor")
                                .Replace("Friliv", "Andersenoutdoor");
                    break;
                case 3: // Swedish
                    stringToHaveReplaced = stringToHaveReplaced
                                .Replace("friliv.dk", "andersenoutdoor.se")
                                .Replace("Friliv.dk", "Andersenoutdoor.se")
                                .Replace("friliv", "andersenoutdoor")
                                .Replace("Friliv", "Andersenoutdoor")
                                .Replace("danska", "svenska")
                                .Replace("Danska", "Svenska")
                                .Replace("dansk", "svensk")
                                .Replace("Dansk", "Svensk");
                    break;
            }

            char[] lineBreakChars = { '\r', '\n', '.', '"', ':', ';' };
            stringToHaveReplaced = stringToHaveReplaced.TrimStart(lineBreakChars);

            return stringToHaveReplaced;
        }
    }
}
