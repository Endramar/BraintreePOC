using Braintree;
using BraintreePOC.Entities;
using BraintreePOC.Models;
using BraintreePOC.Models.Requests.Checkout;
using BraintreePOC.Models.ViewModels;
using BraintreePOC.Models.ViewModels.Checkout;
using BraintreePOC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IBraintreeService braintreeService;
        private readonly BraintreeDbContext dbContext;

        public CheckoutController(IBraintreeService braintreeService, BraintreeDbContext dbContext)
        {
            this.braintreeService = braintreeService;
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index([FromForm] IndexRequest request)
        {
            ViewBag.ClientToken = await this.braintreeService.GenerateClientToken(request.Currency);
            return View(request);
        }


        public IActionResult Refund(long transactionId, long customerId)
        {
            ViewBag.TransactionId = transactionId;
            ViewBag.CustomerId = customerId;
            return View();
        }

        public async Task<IActionResult> Payment([FromForm] IndexRequest request)
        {
            ViewBag.ClientToken = await this.braintreeService.GenerateClientToken(request.Currency);

            var paymentViewModel = new PaymentViewModel
            {
                Amount = request.Amount,
                Currency = request.Currency,
                CustomerId = request.CustomerId
            };

            if (request.CustomerId.HasValue)
            {
                paymentViewModel.CustomerCreditCards = dbContext.CreditCards.Where(x => x.CustomerId == request.CustomerId.Value).ToList();
            }

            return View(paymentViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseRequest request)
        {
            var result = await this.braintreeService.CreatePurchase(request);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseWithToken([FromBody] CreatePurchaseWithTokenRequest request)
        {
            var result = await this.braintreeService.CreatePurchaseWithToken(request);
            return new JsonResult(result);
        }

        public async Task<IActionResult> Result(string id)
        {
            var transactionResponse = await this.braintreeService.GetTransactionById(id);

            var transaction = transactionResponse.Transaction;

            var resultViewModel = new BraintreeTransactionViewModel
            {
                Succeeded = transactionResponse.Succeeded,
                ErrorMessage = transactionResponse.ErrorMessage,
                Amount = transaction.Amount,
                CreatedAt = transaction.CreatedAt,
                Id = transaction.Id,
                Status = transaction.Status,
                Type = transaction.Type,
                UpdatedAt = transaction.UpdatedAt,
                CreditCardDetails = new BraintreeTransactionCreditCardViewModel
                {
                    Bin = transaction.CreditCard?.Bin,
                    CardholderName = transaction.CreditCard?.CardholderName,
                    ExpirationDate = transaction.CreditCard?.ExpirationDate,
                    Last4 = transaction.CreditCard?.LastFour,
                    Token = transaction.CreditCard?.Token,
                    Type = transaction.CreditCard.CardType
                },
                CustomerDetails = new BraintreeTransactionCustomerViewModel
                {
                    Company = transaction.CustomerDetails?.Company,
                    Email = transaction.CustomerDetails?.Email,
                    Fax = transaction.CustomerDetails?.Fax,
                    FirstName = transaction.CustomerDetails?.FirstName,
                    Id = transaction.CustomerDetails?.Id,
                    LastName = transaction.CustomerDetails?.LastName,
                    Phone = transaction.CustomerDetails?.Phone,
                    Website = transaction.CustomerDetails?.Website
                }
            };

            return View(resultViewModel);
        }

        public async Task<IActionResult> Transactions()
        {
            var result = await this.braintreeService.GetAllTransactions();
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> RefundTransaction([FromForm] RefundTransactionRequest request)
        {
            var result = await this.braintreeService.RefundTransaction(request);
            return RedirectToAction("Transactions", "Home", new { customerId = request.CustomerId });
        }
    }
}
