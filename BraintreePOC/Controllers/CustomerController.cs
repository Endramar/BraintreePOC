using BraintreePOC.Models;
using BraintreePOC.Models.Requests.Customer;
using BraintreePOC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IBraintreeService braintreeService;

        public CustomerController(IBraintreeService braintreeService)
        {
            this.braintreeService = braintreeService;
        }

        public async Task<IActionResult> CreateCustomer([FromForm] CreateCustomerRequest request)
        {
            var customerId = await this.braintreeService.CreateCustomer(request);

            if (customerId != null)
            {
                return RedirectToAction("Customers", "Home");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        public async Task<IActionResult> CreateAddress([FromForm] CreateAddressRequest request)
        {
            var address = await this.braintreeService.CreateCustomerAddress(request);

            if (address != null)
            {
                return RedirectToAction("Addresses", "Home", new { customerId = request.CustomerId });
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerCreditCard([FromBody] CreateCustomerCreditCardRequest request)
        {
            var cardToken = await this.braintreeService.CreateCustomerCreditCard(request);
            return new JsonResult(request.CustomerId);
        }
    }
}
