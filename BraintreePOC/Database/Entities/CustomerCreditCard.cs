using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Database.Entities
{
    public class CustomerCreditCard
    {
        [Key]
        public long Id { get; set; }

        public string Token { get; set; }

        public string Last4 { get; set; }

        public string Brand { get; set; }

        public long CustomerId { get; set; }

        [DefaultValue(false)]
        public bool IsDefault { get; set; }

        public Customer Customer { get; set; }
    }
}
