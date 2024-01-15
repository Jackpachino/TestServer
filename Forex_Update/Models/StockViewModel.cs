using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Forex_Update.Models
{
    public class StockViewModel
    {
        public IEnumerable<stock> StocksList { get; set; }

        public IEnumerable<StockSecond> SecondStocksList { get; set; }

        public IEnumerable<Crypto> CryptoList { get; set; }

        public IEnumerable<Currency> CurrenciesList { get; set; }
    }
}