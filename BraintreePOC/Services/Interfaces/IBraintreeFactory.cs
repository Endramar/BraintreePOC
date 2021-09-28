using Braintree;
using System.Collections.Generic;

namespace BraintreePOC.Services.Interfaces
{
    public interface IBraintreeFactory
    {
        BraintreeGateway Gateway { get; }

        HashSet<TransactionStatus> TransactionSuccessStatuses { get; }
    }
}
