using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Models.Requests.Checkout
{
    public class IndexRequest
    {
        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public long? CustomerId { get; set; }

    }
}
