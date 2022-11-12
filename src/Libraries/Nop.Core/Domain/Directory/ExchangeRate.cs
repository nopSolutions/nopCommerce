<<<<<<< HEAD
<<<<<<< HEAD
using System;

namespace Nop.Core.Domain.Directory
{
    /// <summary>
    /// Represents an exchange rate
    /// </summary>
    public partial class ExchangeRate
    {
        /// <summary>
        /// Creates a new instance of the ExchangeRate class
        /// </summary>
        public ExchangeRate()
        {
            CurrencyCode = string.Empty;
            Rate = 1.0m;
        }
        
        /// <summary>
        /// The three letter ISO code for the Exchange Rate, e.g. USD
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// The conversion rate of this currency from the base currency
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// When was this exchange rate updated from the data source (the data XML feed)
        /// </summary>
        public DateTime UpdatedOn { get; set; }
        
        /// <summary>
        /// Format the rate into a string with the currency code, e.g. "USD 0.72543"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{CurrencyCode} {Rate}";
        }
    }
}
=======
=======
=======
<<<<<<< HEAD
>>>>>>> 974287325803649b246516d81982b95e372d09b9
using System;

namespace Nop.Core.Domain.Directory
{
    /// <summary>
    /// Represents an exchange rate
    /// </summary>
    public partial class ExchangeRate
    {
        /// <summary>
        /// Creates a new instance of the ExchangeRate class
        /// </summary>
        public ExchangeRate()
        {
            CurrencyCode = string.Empty;
            Rate = 1.0m;
        }
        
        /// <summary>
        /// The three letter ISO code for the Exchange Rate, e.g. USD
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// The conversion rate of this currency from the base currency
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// When was this exchange rate updated from the data source (the data XML feed)
        /// </summary>
        public DateTime UpdatedOn { get; set; }
        
        /// <summary>
        /// Format the rate into a string with the currency code, e.g. "USD 0.72543"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{CurrencyCode} {Rate}";
        }
    }
}
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
using System;

namespace Nop.Core.Domain.Directory
{
    /// <summary>
    /// Represents an exchange rate
    /// </summary>
    public partial class ExchangeRate
    {
        /// <summary>
        /// Creates a new instance of the ExchangeRate class
        /// </summary>
        public ExchangeRate()
        {
            CurrencyCode = string.Empty;
            Rate = 1.0m;
        }
        
        /// <summary>
        /// The three letter ISO code for the Exchange Rate, e.g. USD
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// The conversion rate of this currency from the base currency
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// When was this exchange rate updated from the data source (the data XML feed)
        /// </summary>
        public DateTime UpdatedOn { get; set; }
        
        /// <summary>
        /// Format the rate into a string with the currency code, e.g. "USD 0.72543"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{CurrencyCode} {Rate}";
        }
    }
}
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
