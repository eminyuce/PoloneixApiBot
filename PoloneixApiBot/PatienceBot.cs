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

namespace PoloneixApiBot
{
    public class PatienceBot
    {
        private PoloniexClient PoloniexClient { get; set; }
        private Dictionary<String, int> QuoteCurrency { get; set; }

        private static double BitcoinForEachCurreny = 0.0025;

        public PatienceBot(string publicKey, string privateKey)
        {
            PoloniexClient = new PoloniexClient(publicKey, privateKey);
            QuoteCurrency = new Dictionary<String, int>();

            QuoteCurrency.Add("	AMP	", 20);
            QuoteCurrency.Add("	ARDR	", 20);
            QuoteCurrency.Add("	BBR	", 20);
            QuoteCurrency.Add("	BCN	", 20);
            QuoteCurrency.Add("	BCY	", 20);
            QuoteCurrency.Add("	BELA	", 20);
            QuoteCurrency.Add("	BITS	", 20);
            QuoteCurrency.Add("	BLK	", 20);
            QuoteCurrency.Add("	BTCD	", 20);
            QuoteCurrency.Add("	BTM	", 20);
            QuoteCurrency.Add("	BTS	", 20);
            QuoteCurrency.Add("	BURST	", 20);
            QuoteCurrency.Add("	C2	", 20);
            QuoteCurrency.Add("	CLAM	", 20);
            QuoteCurrency.Add("	CURE	", 20);
            QuoteCurrency.Add("	DASH	", 20);
            QuoteCurrency.Add("	DCR	", 20);
            QuoteCurrency.Add("	DGB	", 20);
            QuoteCurrency.Add("	DOGE	", 20);
            QuoteCurrency.Add("	EMC2	", 20);
            QuoteCurrency.Add("	ETC	", 20);
            QuoteCurrency.Add("	ETH	", 20);
            QuoteCurrency.Add("	EXP	", 20);
            QuoteCurrency.Add("	FCT	", 20);
            QuoteCurrency.Add("	FLDC	", 20);
            QuoteCurrency.Add("	FLO	", 20);
            QuoteCurrency.Add("	GAME	", 20);
            QuoteCurrency.Add("	GNT	", 20);
            QuoteCurrency.Add("	GRC	", 20);
            QuoteCurrency.Add("	HUC	", 20);
            QuoteCurrency.Add("	HZ	", 20);
            QuoteCurrency.Add("	IOC	", 20);
            QuoteCurrency.Add("	LBC	", 20);
            QuoteCurrency.Add("	LSK	", 20);
            QuoteCurrency.Add("	LTC	", 20);
            QuoteCurrency.Add("	MAID	", 20);
            QuoteCurrency.Add("	MYR	", 20);
            QuoteCurrency.Add("	NAUT	", 20);
            QuoteCurrency.Add("	NAV	", 20);
            QuoteCurrency.Add("	NEOS	", 20);
            QuoteCurrency.Add("	NMC	", 20);
            QuoteCurrency.Add("	NOBL	", 20);
            QuoteCurrency.Add("	NOTE	", 20);
            QuoteCurrency.Add("	NSR	", 20);
            QuoteCurrency.Add("	NXC	", 20);
            QuoteCurrency.Add("	NXT	", 20);
            QuoteCurrency.Add("	OMNI	", 20);
            QuoteCurrency.Add("	PASC	", 20);
            QuoteCurrency.Add("	PINK	", 20);
            QuoteCurrency.Add("	POT	", 20);
            QuoteCurrency.Add("	PPC	", 20);
            QuoteCurrency.Add("	QBK	", 20);
            QuoteCurrency.Add("	QORA	", 20);
            QuoteCurrency.Add("	QTL	", 20);
            QuoteCurrency.Add("	RADS	", 20);
            QuoteCurrency.Add("	RBY	", 20);
            QuoteCurrency.Add("	REP	", 20);
            QuoteCurrency.Add("	RIC	", 20);
            QuoteCurrency.Add("	SBD	", 20);
            QuoteCurrency.Add("	SC	", 20);
            QuoteCurrency.Add("	SDC	", 20);
            QuoteCurrency.Add("	SJCX	", 20);
            QuoteCurrency.Add("	STEEM	", 20);
            QuoteCurrency.Add("	STR	", 20);
            QuoteCurrency.Add("	STRAT	", 20);
            QuoteCurrency.Add("	SYS	", 20);
            QuoteCurrency.Add("	UNITY	", 20);
            QuoteCurrency.Add("	USDT	", 20);
            QuoteCurrency.Add("	VIA	", 20);
            QuoteCurrency.Add("	VOX	", 20);
            QuoteCurrency.Add("	VRC	", 20);
            QuoteCurrency.Add("	VTC	", 20);
            QuoteCurrency.Add("	XBC	", 20);
            QuoteCurrency.Add("	XCP	", 20);
            QuoteCurrency.Add("	XEM	", 20);
            QuoteCurrency.Add("	XMG	", 20);
            QuoteCurrency.Add("	XMR	", 20);
            QuoteCurrency.Add("	XPM	", 20);
            QuoteCurrency.Add("	XRP	", 20);
            QuoteCurrency.Add("	XVC	", 20);
            QuoteCurrency.Add("	ZEC	", 20);
            QuoteCurrency = QuoteCurrency.ToDictionary(x => x.Key.Trim(), x => x.Value);
        }
        public async void LoadMarketSummaryAsync()
        {
            var markets = await PoloniexClient.Markets.GetSummaryAsync();
            foreach (var item in markets.Keys)
            {
                Console.WriteLine(item.BaseCurrency);
            }
        }
        public async void StartTrading()
        {
            var cnt = QuoteCurrency.Keys.Count;
            var p = PoloniexClient.Wallet.GetBalances2Async();
            p.Wait();
            BTC bitcoinObj = p.Result.BTC;
            double totalBitcoinValue = bitcoinObj.available;

            foreach (var quoteCurrency in QuoteCurrency.Keys)
            {
                if (QuoteCurrency[quoteCurrency] > 0)
                {
                    BTC bitcoinObjFldc = new BTC();
                    bitcoinObjFldc.btcValue = bitcoinObj.btcValue;
                    bitcoinObjFldc.onOrders = bitcoinObj.onOrders;

                    if (totalBitcoinValue > BitcoinForEachCurreny)
                    {

                        bitcoinObjFldc.available = BitcoinForEachCurreny;
                        totalBitcoinValue = totalBitcoinValue - BitcoinForEachCurreny;
                        StartTradingQuoteCurrency(GetQuoteCurrency(p.Result, quoteCurrency), bitcoinObj, quoteCurrency);
                    }

                }
            }



        }


        private QuoteCurrency GetQuoteCurrency(Balance2 p, String quoteCurrency = "FLDC")
        {
            switch (quoteCurrency)
            {
                case "FLDC": return p.FLDC;
                case "AMP": return p.AMP;
                default:
                    return p.FLDC;
            }
        }
        public async void StartTradingQuoteCurrency(QuoteCurrency quoteCurrencyObj, BTC bitcoinObj, String quoteCurrency = "FLDC")
        {
            try
            {
                var markets = PoloniexClient.Markets.GetOpenOrdersAsync(new CurrencyPair("BTC", quoteCurrency));
                markets.Wait();

                BuyCurrency(quoteCurrencyObj, bitcoinObj, quoteCurrency, markets);
                SellCurrency(quoteCurrencyObj, quoteCurrency, markets);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // PoloniexClient.Trading.GetOpenOrdersAsync(new CurrencyPair("));

            Console.WriteLine("Type anything to continue.");
            Console.ReadKey();
        }

        private void SellCurrency(QuoteCurrency quoteCurrencyObj, string quoteCurrency, Task<Jojatekok.PoloniexAPI.MarketTools.IOrderBook> markets)
        {
            if (quoteCurrencyObj.available > 0)
            {

                Console.WriteLine(quoteCurrencyObj);
                var aFdlc = quoteCurrencyObj.available;
                var latestQuote = SellBuyOrderQuoteRepository.GetLatestTransactionCurrency(quoteCurrency,
                    OrderType.Buy.ToString());


                var firstBuyOrder = markets.Result.BuyOrders.FirstOrDefault();
                var currencyBuyPrice = latestQuote.PricePerCoin;
                if (latestQuote.PricePerCoin < firstBuyOrder.PricePerCoin)
                {
                    currencyBuyPrice = firstBuyOrder.PricePerCoin;
                }

                Console.WriteLine("BuyOrders:" + quoteCurrency + "  pricePerCoin:"
                    + String.Format("{0:F20}", firstBuyOrder.PricePerCoin)
                    + "  amountQuote:" + firstBuyOrder.AmountQuote + " ");


                var newAmountBase = currencyBuyPrice + currencyBuyPrice * 0.1;
                double pricePerCoin = newAmountBase;
                double amountQuote = aFdlc;

                if (quoteCurrencyObj.btcValue > newAmountBase)
                {

                    var www = PoloniexClient.Trading.PostOrderAsync(new CurrencyPair("BTC", quoteCurrency),
                        OrderType.Sell,
                        pricePerCoin,
                        amountQuote);
                    www.Wait();
                    Console.WriteLine(www.Result);


                    Console.WriteLine("Selling:" + quoteCurrency + "  pricePerCoin:" +
                        String.Format("{0:F20}", pricePerCoin) +
                        "  amountQuote:" + amountQuote + " currencyBuyPrice:"
                        + String.Format("{0:F20}", currencyBuyPrice) + " newAmountBase:"
                        + String.Format("{0:F20}", newAmountBase));
                    SyncTrades(quoteCurrency);
                    Thread.Sleep(500);
                }

            }
        }

        private void BuyCurrency(QuoteCurrency quoteCurrencyObj, BTC bitcoinObj, string quoteCurrency, Task<Jojatekok.PoloniexAPI.MarketTools.IOrderBook> markets)
        {
            var fldcValue = quoteCurrencyObj.btcValue;
            try
            {
                double altLimitForBTC = 0.0001;
                if (bitcoinObj.available > altLimitForBTC)
                {
                    Console.WriteLine(bitcoinObj);

                    var currencyBuyPrice = markets.Result.SellOrders.FirstOrDefault().PricePerCoin;

                    double pricePerCoin = currencyBuyPrice - currencyBuyPrice * 0.1;
                    double amountQuote = bitcoinObj.available / pricePerCoin;

                    var www = PoloniexClient.Trading.PostOrderAsync(new CurrencyPair("BTC", quoteCurrency),
                       OrderType.Buy,
                       pricePerCoin,
                       amountQuote);
                    www.Wait();
                    Console.WriteLine(www.Result);

                    Console.WriteLine("Buying:" + quoteCurrency + "  pricePerCoin:" + String.Format("{0:F20}", pricePerCoin) + "  amountQuote:" + amountQuote + " ");
                    Thread.Sleep(500);
                    SyncTrades(quoteCurrency);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SyncTrades(string quoteCurrency)
        {
            Thread.Sleep(100);
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
    }
}
