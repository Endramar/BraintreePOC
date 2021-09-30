using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Models.Requests.Checkout
{
    public class CreatePurchaseRequest
    {
        public decimal Amount { get; set; }

        public string ClientNonce { get; set; }

        public string DeviceData { get; set; }

        public string Currency { get; set; }

        public long CustomerId { get; set; }

    }
}
