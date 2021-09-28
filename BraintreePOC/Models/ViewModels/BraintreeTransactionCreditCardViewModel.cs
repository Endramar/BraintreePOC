using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Models.ViewModels
{
    public class BraintreeTransactionCreditCardViewModel
    {
        public string Token{ get; set; }

        public string Bin { get; set; }

        public string Last4 { get; set; }

        public CreditCardCardType Type { get; set; }

        public string ExpirationDate { get; set; }

        public string CardholderName { get; set; }


    }
}
