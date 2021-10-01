using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Models.Requests.Checkout
{
    public class RefundTransactionRequest
    {
        public long TransactionId { get; set; }

        public long CustomerId { get; set; }

        public decimal Amount { get; set; }
    }
}
