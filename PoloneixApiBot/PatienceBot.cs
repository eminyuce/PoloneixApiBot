using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jojatekok.PoloniexAPI;
using Jojatekok.PoloniexAPI.TradingTools;
using System.Reflection;
using PoloneixApiBot.Entities;
using PoloneixApiBot.Repositories;
using System.Threading;
using Jojatekok.PoloniexAPI.WalletTools;
using NLog;

namespace PoloneixApiBot
{
    public class PatienceBot
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private PoloniexClient PoloniexClient { get; set; }
        private Dictionary<String, PQuoteCurrency> QuoteCurrencyDic { get; set; }

        private static double BitcoinForEachCurreny = 0.0025;

        public PatienceBot(string publicKey, string privateKey)
        {
            PoloniexClient = new PoloniexClient(publicKey, privateKey);
            QuoteCurrencyDic = new Dictionary<String, PQuoteCurrency>();

            QuoteCurrencyDic.Add("AMP", new PQuoteCurrency(5, 4, 4, "AMP"));
            QuoteCurrencyDic.Add("FLDC", new PQuoteCurrency(5, 3, 3, "FLDC"));
            QuoteCurrencyDic.Add("SDC", new PQuoteCurrency(5, 3, 3, "SDC"));
            QuoteCurrencyDic.Add("BURST", new PQuoteCurrency(5, 3, 3, "BURST"));
            QuoteCurrencyDic.Add("STEEM ", new PQuoteCurrency(5, 3, 3, "STEEM "));
            QuoteCurrencyDic.Add("XMR", new PQuoteCurrency(5, 3, 3, "XMR"));
            QuoteCurrencyDic.Add("PASC", new PQuoteCurrency(5, 3, 3, "PASC"));
            QuoteCurrencyDic.Add("XRP", new PQuoteCurrency(5, 3, 3, "XRP"));
            QuoteCurrencyDic.Add("ETC", new PQuoteCurrency(5, 3, 3, "ETC"));
            QuoteCurrencyDic.Add("ETH", new PQuoteCurrency(5, 3, 3, "ETH"));
        }
        public async void LoadMarketSummaryAsync()
        {
            var markets = await PoloniexClient.Markets.GetSummaryAsync();
            foreach (var item in markets.Keys)
            {
                Logger.Info(item.BaseCurrency);
            }
        }
        public async void StartTrading()
        {
            var cnt = QuoteCurrencyDic.Keys.Count;
            var p = PoloniexClient.Wallet.GetBalances2Async();
            p.Wait();
            BTC bitcoinObj = p.Result.BTC;
            double totalBitcoinValue = bitcoinObj.available;

            foreach (var quoteCurrencySymbol in QuoteCurrencyDic.Keys)
            {
                var QuoteCurrency = QuoteCurrencyDic[quoteCurrencySymbol];

                // Play only selected currency.
                if (QuoteCurrency.TotalBitcoinPercantege > 0)
                {

                    try
                    {
                        var percentage = QuoteCurrency.TotalBitcoinPercantege;
                        var quoteCurrencyObj = GetQuoteCurrency(p.Result, quoteCurrencySymbol);
                        var markets = PoloniexClient.Markets.GetOpenOrdersAsync(new CurrencyPair("BTC", QuoteCurrency.QuoteCurrencySymbol));
                        markets.Wait();
                        var openOrdersForCurrency = PoloniexClient.Trading.GetOpenOrdersAsync(new CurrencyPair("BTC", QuoteCurrency.QuoteCurrencySymbol));
                        openOrdersForCurrency.Wait();

                        if (!openOrdersForCurrency.Result.Any() && totalBitcoinValue > 0)
                        {
                            BTC bitcoinObjFldc = new BTC();
                            bitcoinObjFldc.btcValue = bitcoinObj.btcValue;
                            bitcoinObjFldc.onOrders = bitcoinObj.onOrders;
                            bitcoinObjFldc.available = totalBitcoinValue * percentage / 100.0;
                            totalBitcoinValue = totalBitcoinValue - bitcoinObjFldc.available;
                            BuyCurrency(quoteCurrencyObj, bitcoinObjFldc, QuoteCurrency, markets.Result);
                        }


                        if (!openOrdersForCurrency.Result.Any(r => r.Type == OrderType.Sell))
                        {
                            SellCurrency(quoteCurrencyObj, QuoteCurrency, markets.Result);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, ex.Message);

                    }

                }

            }

            //Logger.Info("Type anything to continue.");
            //Console.ReadKey();
        }


        private QuoteCurrency GetQuoteCurrency(Balance2 p, String quoteCurrency = "FLDC")
        {
            switch (quoteCurrency)
            {

                case "FLDC": return p.FLDC;
                case "AMP": return p.AMP;
                case "SDC": return p.SDC;
                case "BURST": return p.BURST;
                case "STEEM": return p.STEEM;
                case "XMR": return p.XMR;
                case "PASC": return p.PASC;
                case "ETC": return p.ETC;
                case "ETH": return p.ETH;
                case "XRP": return p.XRP;

                default:
                    return p.FLDC;
            }
        }


        private void SellCurrency(QuoteCurrency quoteCurrencyObj,
            PQuoteCurrency quoteCurrencyObj2,
            Jojatekok.PoloniexAPI.MarketTools.IOrderBook markets)
        {
            try
            {


                if (quoteCurrencyObj.available > 0)
                {
                    string quoteCurrency = quoteCurrencyObj2.QuoteCurrencySymbol;
                    Logger.Info(quoteCurrencyObj);
                    var aFdlc = quoteCurrencyObj.available;
                    var latestQuote = SellBuyOrderQuoteRepository.GetLatestTransactionCurrency(quoteCurrency,
                        OrderType.Buy.ToString());


                    var firstBuyOrder = markets.BuyOrders.FirstOrDefault();
                    var currencyBuyPrice = latestQuote.PricePerCoin;
                    if (latestQuote.PricePerCoin < firstBuyOrder.PricePerCoin)
                    {
                        currencyBuyPrice = firstBuyOrder.PricePerCoin;
                    }

                    Logger.Info("BuyOrders:" + quoteCurrency + "  pricePerCoin:"
                        + String.Format("{0:F20}", firstBuyOrder.PricePerCoin)
                        + "  amountQuote:" + firstBuyOrder.AmountQuote + " ");


                    var newAmountBase = currencyBuyPrice + currencyBuyPrice * quoteCurrencyObj2.SellOrderPricePercantege / 100.0;
                    double pricePerCoin = newAmountBase;
                    double amountQuote = aFdlc;

                    if (quoteCurrencyObj.btcValue > newAmountBase)
                    {

                        var www = PoloniexClient.Trading.PostOrderAsync(new CurrencyPair("BTC", quoteCurrency),
                            OrderType.Sell,
                            pricePerCoin,
                            amountQuote);
                        www.Wait();
                        Logger.Info(www.Result);


                        Logger.Info("Selling:" + quoteCurrency + "  pricePerCoin:" +
                            String.Format("{0:F20}", pricePerCoin) +
                            "  amountQuote:" + amountQuote + " currencyBuyPrice:"
                            + String.Format("{0:F20}", currencyBuyPrice) + " newAmountBase:"
                            + String.Format("{0:F20}", newAmountBase));
                        SyncTrades(quoteCurrency);
                    }

                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
        }

        private void BuyCurrency(QuoteCurrency quoteCurrencyObj,
            BTC bitcoinObj,
          PQuoteCurrency quoteCurrencyObj2,
            Jojatekok.PoloniexAPI.MarketTools.IOrderBook markets)
        {


            var fldcValue = quoteCurrencyObj.btcValue;
            string quoteCurrency = quoteCurrencyObj2.QuoteCurrencySymbol;
            try
            {
                double altLimitForBTC = 0.0001;
                if (bitcoinObj.available > altLimitForBTC)
                {
                    Logger.Info(bitcoinObj);

                    var currencyBuyPrice = markets.SellOrders.FirstOrDefault().PricePerCoin;

                    double pricePerCoin = currencyBuyPrice - currencyBuyPrice * quoteCurrencyObj2.BuyOrderPricePercantege / 100.0;
                    double amountQuote = bitcoinObj.available / pricePerCoin;

                    var www = PoloniexClient.Trading.PostOrderAsync(new CurrencyPair("BTC", quoteCurrency),
                       OrderType.Buy,
                       pricePerCoin,
                       amountQuote);
                    www.Wait();
                    Logger.Info(www.Result);

                    Logger.Info("Buying:" + quoteCurrency + "  pricePerCoin:" + String.Format("{0:F20}", pricePerCoin) + "  amountQuote:" + amountQuote + " ");

                    SyncTrades(quoteCurrency);

                }

            }
            catch (Exception ex)
            {
                Logger.Info(ex.Message);
            }
        }

        private void SyncTrades(string quoteCurrency)
        {
            try
            {
                Thread.Sleep(500);
                var t2 = PoloniexClient.Trading.GetTradesAsync(new CurrencyPair("BTC", quoteCurrency));
                t2.Wait();
                foreach (Trade item2 in t2.Result)
                {
                    var item = new SellBuyOrderQuote();
                    item.IdOrder = item2.IdOrder;
                    item.TypeInternal = quoteCurrency;
                    item.Type = item2.Type.ToString();
                    item.PricePerCoin = item2.PricePerCoin;
                    item.AmountQuote = item2.AmountQuote;
                    item.AmountBase = item2.AmountBase;
                    item.TradeTime = item2.Time;
                    SellBuyOrderQuoteRepository.SaveOrUpdateSellBuyOrderQuote(item);

                }
            }
            catch (Exception ex)
            {

                Logger.Error(ex, ex.Message);
            }


        }
    }
}
