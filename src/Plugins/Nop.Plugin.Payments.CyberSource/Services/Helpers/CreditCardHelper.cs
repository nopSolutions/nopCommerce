using System.Text.RegularExpressions;

namespace Nop.Plugin.Payments.CyberSource.Services.Helpers
{
    /// <summary>
    /// Credit card helper
    /// </summary>
    public class CreditCardHelper
    {
        #region Fields

        protected static readonly Regex _specialCharRemoveRegex;
        protected static readonly Regex _americanExpressRegx;
        protected static readonly Regex _discoverRegx;
        protected static readonly Regex _mastercardRegx;
        protected static readonly Regex _visaRegx;
        protected static readonly Regex _dinersClubRegx;
        protected static readonly Regex _jcbRegx;

        #endregion

        #region Ctor

        static CreditCardHelper()
        {
            _specialCharRemoveRegex = new Regex("[*'\",_&#^@ ]");
            _americanExpressRegx = new Regex("^3[47][0-9]{13}$");
            _discoverRegx = new Regex("^6(?:011|5[0-9]{2})[0-9]{12}$");
            _mastercardRegx = new Regex("^(?:5[1-5][0-9]{2}|222[1-9]|22[3-9][0-9]|2[3-6][0-9]{2}|27[01][0-9]|2720)[0-9]{12}$");
            _visaRegx = new Regex("^4[0-9]{12}(?:[0-9]{3})?$");
            _dinersClubRegx = new Regex("^3(?:0[0-5]|[68][0-9])[0-9]{11}$");
            _jcbRegx = new Regex(@"^(?:2131|1800|35\d{3})\d{11}$");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Remove special characters from card number
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Card number without special characters</returns>
        public static string RemoveSpecialCharacters(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return string.Empty;

            return _specialCharRemoveRegex.Replace(cardNumber, string.Empty);
        }

        /// <summary>
        /// Get card type by a card number
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Card type</returns>
        public static string GetCardTypeByNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return string.Empty;

            cardNumber = RemoveSpecialCharacters(cardNumber);

            if (_mastercardRegx.IsMatch(cardNumber))
                return CardType.Mastercard;
            if (_visaRegx.IsMatch(cardNumber))
                return CardType.Visa;
            if (_americanExpressRegx.IsMatch(cardNumber))
                return CardType.AmericanExpress;
            if (_discoverRegx.IsMatch(cardNumber))
                return CardType.Discover;
            if (_dinersClubRegx.IsMatch(cardNumber))
                return CardType.DinersClub;
            if (_jcbRegx.IsMatch(cardNumber))
                return CardType.Jcb;

            return string.Empty;
        }

        /// <summary>
        /// Get three digit card type by a card number
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Three digit card type</returns>
        public static string GetThreeDigitCardTypeByNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return string.Empty;

            cardNumber = RemoveSpecialCharacters(cardNumber);

            if (_mastercardRegx.IsMatch(cardNumber))
                return ThreeDigitCardType.Mastercard;
            if (_visaRegx.IsMatch(cardNumber))
                return ThreeDigitCardType.Visa;
            if (_americanExpressRegx.IsMatch(cardNumber))
                return ThreeDigitCardType.AmericanExpress;
            if (_discoverRegx.IsMatch(cardNumber))
                return ThreeDigitCardType.Discover;
            if (_dinersClubRegx.IsMatch(cardNumber))
                return ThreeDigitCardType.DinersClub;
            if (_jcbRegx.IsMatch(cardNumber))
                return ThreeDigitCardType.Jcb;

            return string.Empty;
        }

        /// <summary>
        /// Get first six digits of card
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>First six digits of card</returns>
        public static string GetFirstSixDigitsOfCard(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return string.Empty;

            cardNumber = RemoveSpecialCharacters(cardNumber);

            return cardNumber.Length >= 6 ? cardNumber.Substring(0, 6) : cardNumber;
        }

        /// <summary>
        /// Get last four digits of card
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Last four digits of card</returns>
        public static string GetLastFourDigitsOfCard(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return string.Empty;

            cardNumber = RemoveSpecialCharacters(cardNumber);

            return cardNumber.Length >= 4 ? cardNumber.Substring(cardNumber.Length - 4) : cardNumber;
        }

        #endregion

        #region Card types

        public class CardType
        {
            /// <summary>
            /// Gets the visa card type
            /// </summary>
            public static string Visa => "visa";

            /// <summary>
            /// Gets the mastercard card type
            /// </summary>
            public static string Mastercard => "mastercard";

            /// <summary>
            /// Gets the american express card type
            /// </summary>
            public static string AmericanExpress => "american express";

            /// <summary>
            /// Gets the discover card type
            /// </summary>
            public static string Discover => "discover";

            /// <summary>
            /// Gets the diners club card type
            /// </summary>
            public static string DinersClub => "diners club";

            /// <summary>
            /// Gets the jcb card type
            /// </summary>
            public static string Jcb => "jcb";
        }

        #endregion

        #region Three digit card types

        public class ThreeDigitCardType
        {
            /// <summary>
            /// Gets the visa card type
            /// </summary>
            public static string Visa => "001";

            /// <summary>
            /// Gets the mastercard card type
            /// </summary>
            public static string Mastercard => "002";

            /// <summary>
            /// Gets the american express card type
            /// </summary>
            public static string AmericanExpress => "003";

            /// <summary>
            /// Gets the discover card type
            /// </summary>
            public static string Discover => "004";

            /// <summary>
            /// Gets the diners club card type
            /// </summary>
            public static string DinersClub => "005";

            /// <summary>
            /// Gets the jcb card type
            /// </summary>
            public static string Jcb => "007";
        }

        #endregion
    }
}