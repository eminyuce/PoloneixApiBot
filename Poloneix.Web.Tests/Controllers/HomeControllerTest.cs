﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poloneix.Web;
using Poloneix.Web.Controllers;
using Jojatekok.PoloniexAPI;
using Jojatekok.PoloniexAPI.WalletTools;
using PoloneixApi.Domain;

namespace Poloneix.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
   

        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
         
        [TestMethod]
        public void GetTradesAsync()
        {
            var PoloniexClient = new PoloniexClient(Settings.PublicKey, Settings.PrivateKey);
            var t2 = PoloniexClient.Trading.GetTradesAsync(new CurrencyPair("BTC", "POT"));
            t2.Wait();
            var r = t2.Result;

        }
    }
}
