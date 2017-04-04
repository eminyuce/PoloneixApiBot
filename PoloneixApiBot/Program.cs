using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jojatekok.PoloniexAPI;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Reflection;
using NLog;

namespace PoloneixApiBot
{
    class Program
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                Logger.Info("PoloneixApiBot is started.");
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { new TradingService() };
                ServiceBase.Run(ServicesToRun);
            }


        }

    }
}
