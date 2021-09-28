using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Models.Responses.Checkout
{
    public class CreatePurchaseResponse
    {
        public bool Succeeded { get; set; }

        public string TransactionId { get; set; }

        public string ErrorMessage { get; set; }

    }
}
