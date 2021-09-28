using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Models.Responses.Checkout
{
    public class GetAllTransactionsResponse
    {
        public List<GetTransactionByIdResponse> AllTransactions{ get; set; }
    }
}
