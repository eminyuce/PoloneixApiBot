using PoloneixApiBot.DB;
using PoloneixApiBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace PoloneixApiBot.Repositories
{
    public class SellBuyOrderQuoteRepository
    {

        public static List<SellBuyOrderQuote> GetSellBuyOrderQuotesFromCache()
        {
            string cacheKey = "SellBuyOrderQuoteCache";
            var items = (List<SellBuyOrderQuote>)MemoryCache.Default.Get(cacheKey);
            if (items == null)
            {
                items = GetSellBuyOrderQuotes();
                CacheItemPolicy policy = null;
                policy = new CacheItemPolicy();
                policy.Priority = CacheItemPriority.Default;
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(Settings.CacheMediumSeconds);
                MemoryCache.Default.Set(cacheKey, items, policy);
            }
            return items;
        }

        public static List<SellBuyOrderQuote> GetSellBuyOrderQuotes()
        {
            return DBDirectory.GetSellBuyOrderQuotes();
        }
        public static int SaveOrUpdateSellBuyOrderQuote(SellBuyOrderQuote item)
        {
            return DBDirectory.SaveOrUpdateSellBuyOrderQuote(item);
        }
        public static SellBuyOrderQuote GetSellBuyOrderQuote(int id)
        {
            return DBDirectory.GetSellBuyOrderQuote(id);
        }
        public static SellBuyOrderQuote GetLatestTransactionCurrency(string QuoteCurrency, string type)
        {
            return DBDirectory.GetLatestTransactionCurrency(QuoteCurrency, type);
        }
        public static void DeleteSellBuyOrderQuote(int id)
        {
            DBDirectory.DeleteSellBuyOrderQuote(id);
        }
    }

}
