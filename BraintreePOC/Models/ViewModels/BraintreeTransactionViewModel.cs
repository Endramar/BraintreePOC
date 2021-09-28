using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Models.ViewModels
{
    public class BraintreeTransactionViewModel
    {
        public string Id { get; set; }

        public TransactionType Type  { get; set; }

        public decimal? Amount { get; set; }

        public TransactionStatus Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool Succeeded { get; set; }

        public string ErrorMessage { get; set; }

        public BraintreeTransactionCreditCardViewModel CreditCardDetails { get; set; }

        public BraintreeTransactionCustomerViewModel CustomerDetails { get; set; }

    }
}
