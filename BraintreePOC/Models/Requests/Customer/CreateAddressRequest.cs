using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Models.Requests.Customer
{
    public class CreateAddressRequest
    {
        public string PhoneNumber { get; set; }
        public string CountryName { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string ExtendedAddress { get; set; }
        public string StreetAddress { get; set; }
        public long CustomerId { get; set; }
    }
}
