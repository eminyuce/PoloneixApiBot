using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jojatekok.PoloniexAPI;

namespace PoloneixApiBot
{
    class Program
    {
        public static string PublicKey;
        public static string PrivateKey; 

        static void Main(string[] args)
        {
            var p = new PatienceBot(PublicKey, PrivateKey);
            p.StartTrading();

        }
        
    }
}
