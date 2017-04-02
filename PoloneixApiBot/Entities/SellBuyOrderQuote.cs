using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoloneixApiBot.Entities
{
    public class SellBuyOrderQuote
    {
        public int Id { get; set; }
        public ulong IdOrder { get; set; }
        public string TypeInternal { get; set; }
        public string Type { get; set; }
        public double PricePerCoin { get; set; }
        public double AmountQuote { get; set; }
        public double AmountBase { get; set; }
        public DateTime TradeTime { get; set; }

    }
}
