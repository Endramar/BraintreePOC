using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Models.ViewModels
{
    public class BraintreeTransactionCustomerViewModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Company { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

    }
}
