using Braintree;
using BraintreePOC.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreePOC.Services
{
    public class BraintreeFactory : IBraintreeFactory
    {
        public BraintreeGateway Gateway => GetBraintreeGateway();

        public HashSet<TransactionStatus> TransactionSuccessStatuses => GetTransactionSuccessStatuses();


        private BraintreeGateway GetBraintreeGateway()
        {
            return new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = "snr98rtt2b5kcyft",
                PublicKey = "tjwbhc42jgqg839q",
                PrivateKey = "dcd764bac6e9df580d5286bf401a0ddc"
            };
        }

        private HashSet<TransactionStatus> GetTransactionSuccessStatuses()
        {
            return new HashSet<TransactionStatus>    {
                TransactionStatus.AUTHORIZED,
                TransactionStatus.AUTHORIZING,
                TransactionStatus.SETTLED,
                TransactionStatus.SETTLING,
                TransactionStatus.SETTLEMENT_CONFIRMED,
                TransactionStatus.SETTLEMENT_PENDING,
                TransactionStatus.SUBMITTED_FOR_SETTLEMENT
            };
        }
    }
}
