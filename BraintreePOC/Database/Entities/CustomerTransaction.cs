using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Database.Entities
{
    public class CustomerTransaction
    {
        [Key]
        public long Id { get; set; }

        public string BraintreeTransactionId { get; set; }

        public decimal Amount { get; set; }

        public bool WasSuccessfull { get; set; }

        public string ProcessorResponseCode { get; set; }

        public string ProcessorResponseText { get; set; }

        public string Status { get; set; }

        public long CustomerId { get; set; }

        public Customer Customer { get; set; }
    }
}
