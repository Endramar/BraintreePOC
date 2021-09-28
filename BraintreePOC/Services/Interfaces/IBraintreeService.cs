using Braintree;
using BraintreePOC.Models.Requests.Checkout;
using BraintreePOC.Models.Requests.Customer;
using BraintreePOC.Models.Responses.Checkout;
using System.Threading.Tasks;

namespace BraintreePOC.Services.Interfaces
{
    public interface IBraintreeService
    {
        Task<string> GenerateClientToken(string currency);

        Task<string> GenerateClientTokenWithoutCurrency();

        Task<CreatePurchaseResponse> CreatePurchase(CreatePurchaseRequest request);

        Task<GetTransactionByIdResponse> GetTransactionById(string transactionId);

        Task<GetAllTransactionsResponse> GetAllTransactions();

        Task<string> CreateCustomer(CreateCustomerRequest request);

        Task<string> CreateCustomerCreditCard(CreateCustomerCreditCardRequest request);

        Task<CreditCard> GetPaymentMethodByToken(string token);

    }
}
