using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BraintreePOC.Models;
using Braintree;
using Microsoft.AspNetCore.Http;
using BraintreePOC.Entities;
using BraintreePOC.Services.Interfaces;

namespace BraintreePOC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BraintreeDbContext dbContext;
        private readonly IBraintreeService braintreeService;

        public HomeController(ILogger<HomeController> logger, BraintreeDbContext dbContext, IBraintreeService braintreeService)
        {
            _logger = logger;
            this.dbContext = dbContext;
            this.braintreeService = braintreeService;
        }

        public IActionResult Index()
        {
            var customers = dbContext.Customers.ToList();
            return View(customers);
        }

        public IActionResult Customers()
        {
            var customers = dbContext.Customers.ToList();
            return View(customers);
        }

        public async Task<IActionResult> CreditCards(long customerId)
        {
            var customerCreditCards = dbContext.CreditCards.Where(x => x.CustomerId == customerId).ToList();

            ViewBag.CustomerId = customerId;
            ViewBag.ClientToken = await this.braintreeService.GenerateClientTokenWithoutCurrency();

            return View(customerCreditCards);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
