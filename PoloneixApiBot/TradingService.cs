using NLog;
using PoloneixApiBot.Quartz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PoloneixApiBot
{
    partial class TradingService : ServiceBase
    {
        public TradingService()
        {
            InitializeComponent();
        }
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            var s =new PoloniexQuartzService();
            s.StartService();
            Logger.Info("PoloniexQuartzService is started.");
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
