using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Quartz;

namespace PoloneixApiBot.Quartz.QuartzJob
{
    public class TradingJob : IJob
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static string PublicKey;
        public static string PrivateKey;


        public void Execute(IJobExecutionContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Logger.Info("TradingJob is started.");
          
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            var p = new PatienceBot(PublicKey, PrivateKey);
            p.StartTrading();
            Logger.Info(String.Format("TradingJob is finished. Elapsed Seconds={0}",elapsedTime));
           
        }
    }
}
