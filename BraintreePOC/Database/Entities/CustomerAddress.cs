using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Database.Entities
{
    public class CustomerAddress
    {
        [Key]
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryName { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string ExtendedAddress { get; set; }
        public string StreetAddress { get; set; }
        public long CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
