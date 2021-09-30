using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Database.Entities
{
    public class CustomerTransactionRefund
    {
        [Key]
        public long Id { get; set; }

        public decimal Amount { get; set; }

        public long CustomerTransactionId { get; set; }

        public CustomerTransaction CustomerTransaction { get; set; }
    }
}
