using BraintreePOC.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Models.ViewModels.Checkout
{
    public class PaymentViewModel
    {
        public List<CustomerCreditCard> CustomerCreditCards { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public long? CustomerId { get; set; }

    }
}
