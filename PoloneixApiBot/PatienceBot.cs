using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jojatekok.PoloniexAPI;
using Jojatekok.PoloniexAPI.TradingTools;
using System.Reflection;
using PoloneixApi.Domain.Entities;
using PoloneixApi.Domain.Repositories;
using System.Threading;
using Jojatekok.PoloniexAPI.WalletTools;
using NLog;
using PoloneixApi.Domain.Helpers;
using System.Text.RegularExpressions;

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
        }

        private void PopulateCurrencyDic()
        {
            QuoteCurrencyDic = new Dictionary<String, PQuoteCurrency>();

            string line = "";
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("D:\\Services\\currency.txt");
            while ((line = file.ReadLine()) != null)
            {
                var parts = Regex.Split(line, @",").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                QuoteCurrencyDic.Add(parts.LastOrDefault(),
                    new PQuoteCurrency(parts.FirstOrDefault().ToInt(),
                    parts.Skip(1).FirstOrDefault().ToInt(),
                    parts.Skip(2).FirstOrDefault().ToInt(),
                    parts.LastOrDefault()));
            }

            file.Close();
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
            //Populate our dictionary of currency to play.
            PopulateCurrencyDic(); 
            var cnt = QuoteCurrencyDic.Keys.Count;
            //Get the all our wallet current currency to trade, what we have and how much we have for each of them. 
            var p = PoloniexClient.Wallet.GetBalances2Async();
            p.Wait();

            //Total BTC in our wallet. It will increase or decrease based on our buying-selling other currency
            BTC bitcoinObj = p.Result.BTC;
            // Play only currency if we have enough BTC.
            double totalBitcoinValue = bitcoinObj.available;


            //Quote currencies dictionary, it stores currencies with its PQuoteCurrency object.
             
            foreach (var quoteCurrencySymbol in QuoteCurrencyDic.Keys)
            {
                // PQuoteCurrency class has percantage value properties of 
                // Total BTC will be used
                // buying order based on market latest selling transaction, ex. buy its less 4 percant of latest transcation  
                // selling order based on our buying price or the latest buying transaction, which ever bigger. 
                // Its symbol
                PQuoteCurrency QuoteCurrency = QuoteCurrencyDic[quoteCurrencySymbol];


                // Total bitcoin value will be shared for all other currency, will be decreased for each buy operation 
                // so that percantage calculation will be accurate for all buy operation.
                if (QuoteCurrency.TotalBitcoinPercantege > 0)
                {

                    try
                    {
                        var percentage = QuoteCurrency.TotalBitcoinPercantege;
                        //Get currency object how much we have available
                        var quoteCurrencyObj = QuoteCurrencyHelper.GetQuoteCurrency(p.Result, quoteCurrencySymbol);

                        // Get open orders of market for that currency so that we can make a decision of buying or selling.
                        var markets = PoloniexClient.Markets.GetOpenOrdersAsync(new CurrencyPair("BTC", QuoteCurrency.QuoteCurrencySymbol));
                        markets.Wait();

                        // Get open orders of ours for that currency
                        var openOrdersForCurrency = PoloniexClient.Trading.GetOpenOrdersAsync(new CurrencyPair("BTC", QuoteCurrency.QuoteCurrencySymbol));
                        openOrdersForCurrency.Wait();


                        // Make buy operation if we do not have any open orders for the currency
                        // Do not buy any currency TWICE.
                        if (!openOrdersForCurrency.Result.Any() && totalBitcoinValue > 0)
                        {
                            BTC bitcoinObjFldc = new BTC();
                            bitcoinObjFldc.btcValue = bitcoinObj.btcValue;
                            bitcoinObjFldc.onOrders = bitcoinObj.onOrders;
                            // Only buy enough for its BTC percantage, prevent to spend all available BTC for one currency
                            bitcoinObjFldc.available = totalBitcoinValue * percentage / 100.0;
                            totalBitcoinValue = totalBitcoinValue - bitcoinObjFldc.available;
                            BuyCurrency(quoteCurrencyObj, bitcoinObjFldc, QuoteCurrency, markets.Result);
                        }

                        // Make only sell operation if we do not have sell operation and enough available currency.
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
