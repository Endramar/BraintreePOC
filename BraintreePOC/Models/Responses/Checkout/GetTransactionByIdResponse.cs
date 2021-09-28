using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Models.Responses.Checkout
{
    public class GetTransactionByIdResponse
    {
        public bool Succeeded { get; set; }

        public string ErrorMessage { get; set; }

        public Transaction Transaction { get; set; }


    }
}
