using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoloneixApi.Domain.Entities
{
    public class PQuoteCurrency
    {
        public int TotalBitcoinPercantege { get; set; }

        public int BuyOrderPricePercantege { get; set; }

        public int SellOrderPricePercantege { get; set; }

        public string QuoteCurrencySymbol { get; set; }

        public PQuoteCurrency(int TotalBitcoinPercantege, 
            int BuyOrderPricePercantege, 
            int SellOrderPricePercantege, 
            string QuoteCurrencySymbol)
        {
            this.TotalBitcoinPercantege = TotalBitcoinPercantege;
            this.BuyOrderPricePercantege = BuyOrderPricePercantege;
            this.SellOrderPricePercantege = SellOrderPricePercantege;
            this.QuoteCurrencySymbol = QuoteCurrencySymbol;
        }
    }
}
