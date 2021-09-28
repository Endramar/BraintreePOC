using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Models.Requests.Customer
{
    public class CreateCustomerCreditCardRequest
    {
        public long CustomerId { get; set; }

        public string ClientNonce { get; set; }

    }
}
