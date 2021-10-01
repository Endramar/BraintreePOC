using Braintree;
using BraintreePOC.Database.Entities;
using BraintreePOC.Entities;
using BraintreePOC.Models.Requests.Checkout;
using BraintreePOC.Models.Requests.Customer;
using BraintreePOC.Models.Responses.Checkout;
using BraintreePOC.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BraintreePOC.Services
{
    public class BraintreeService : IBraintreeService
    {
        private readonly IBraintreeFactory braintreeFactory;
        private readonly BraintreeDbContext dbContext;

        public BraintreeService(IBraintreeFactory braintreeFactory, BraintreeDbContext dbContext)
        {
            this.braintreeFactory = braintreeFactory;
            this.dbContext = dbContext;
        }

        public async Task<string> GenerateClientToken(string currency)
        {
            var merchantId = "torpedotvgbp";
            switch (currency)
            {
                case "GBP":
                    merchantId = "torpedotvgbp";
                    break;
                case "USD":
                    merchantId = "torpedotvusd";
                    break;
                case "EUR":
                    merchantId = "torpedotvlimited";
                    break;
                default:
                    merchantId = "torpedotvgbp";
                    break;
            }

            var clientTokenRequest = new ClientTokenRequest
            {
                MerchantAccountId = merchantId
            };

            return await this.braintreeFactory.Gateway.ClientToken.GenerateAsync(clientTokenRequest);
        }


        public async Task<string> GenerateClientTokenWithoutCurrency()
        {
            var clientTokenRequest = new ClientTokenRequest();
            return await this.braintreeFactory.Gateway.ClientToken.GenerateAsync(clientTokenRequest);
        }

        public async Task<CreatePurchaseResponse> CreatePurchase(CreatePurchaseRequest request)
        {
            var merchantId = "torpedotvgbp";
            switch (request.Currency)
            {
                case "GBP":
                    merchantId = "torpedotvgbp";
                    break;
                case "USD":
                    merchantId = "torpedotvusd";
                    break;
                case "EUR":
                    merchantId = "torpedotvlimited";
                    break;
                default:
                    merchantId = "torpedotvgbp";
                    break;
            }

            var transactionRequest = new TransactionRequest
            {
                Amount = request.Amount,
                PaymentMethodNonce = request.ClientNonce,
                DeviceData = request.DeviceData,
                MerchantAccountId = merchantId,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                },
                BillingAddress = new AddressRequest
                {
                    PostalCode = "SW6 5NN",
                    CountryName = "United Kingdom"
                }
            };

            Result<Transaction> result = await this.braintreeFactory.Gateway.Transaction.SaleAsync(transactionRequest);

            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;

                await SaveTransactionToDatabase(transaction, request.CustomerId);
                return new CreatePurchaseResponse { TransactionId = transaction.Id, Succeeded = true };
            }
            else if (result.Transaction != null)
            {
                await SaveTransactionToDatabase(result.Transaction, request.CustomerId);
                return new CreatePurchaseResponse { TransactionId = result.Transaction.Id, Succeeded = true };
            }
            else
            {
                string errorMessages = "";
                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                }

                return new CreatePurchaseResponse { Succeeded = false, ErrorMessage = errorMessages };
            }

        }

        public async Task<GetTransactionByIdResponse> GetTransactionById(string transactionId)
        {
            var transaction = await this.braintreeFactory.Gateway.Transaction.FindAsync(transactionId);

            if (transaction == null)
            {
                return new GetTransactionByIdResponse
                {
                    Succeeded = false,
                    ErrorMessage = "No transaction has been found by this id"
                };
            }

            if (this.braintreeFactory.TransactionSuccessStatuses.Contains(transaction.Status))
            {
                return new GetTransactionByIdResponse
                {
                    Succeeded = true,
                    Transaction = transaction
                };
            }
            else
            {
                return new GetTransactionByIdResponse
                {
                    Succeeded = false,
                    Transaction = transaction
                };
            }
        }

        public async Task<GetAllTransactionsResponse> GetAllTransactions()
        {
            var searchRequest = new TransactionSearchRequest();
            var transactions = await this.braintreeFactory.Gateway.Transaction.SearchAsync(searchRequest);

            var resultList = new List<GetTransactionByIdResponse>();
            foreach (Transaction transaction in transactions)
            {
                var succeeded = this.braintreeFactory.TransactionSuccessStatuses.Contains(transaction.Status);
                var item = new GetTransactionByIdResponse
                {
                    Transaction = transaction,
                    Succeeded = succeeded
                };

                resultList.Add(item);
            }

            return new GetAllTransactionsResponse { AllTransactions = resultList };
        }

        public async Task<string> CreateCustomer(CreateCustomerRequest request)
        {
            var customerRequest = new CustomerRequest
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
            };
            Result<Braintree.Customer> result = await this.braintreeFactory.Gateway.Customer.CreateAsync(customerRequest);

            if (result.IsSuccess())
            {
                string customerId = result.Target.Id;

                var dbCustomer = new BraintreePOC.Database.Entities.Customer
                {
                    BraintreeId = customerId,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Phone = request.Phone,
                };

                await dbContext.Customers.AddAsync(dbCustomer);
                await dbContext.SaveChangesAsync();

                return customerId;
            }
            else
            {
                return null;
            }

        }

        public async Task<string> CreateCustomerCreditCard(CreateCustomerCreditCardRequest request)
        {
            var customer = await dbContext.FindAsync<Database.Entities.Customer>(request.CustomerId);

            var paymentMethodRequest = new PaymentMethodRequest
            {
                CustomerId = customer.BraintreeId,
                PaymentMethodNonce = request.ClientNonce
            };


            Result<PaymentMethod> result = await this.braintreeFactory.Gateway.PaymentMethod.CreateAsync(paymentMethodRequest);

            if (result.IsSuccess())
            {
                var last4 = (result.Target as CreditCard)?.LastFour;
                var brand = (result.Target as CreditCard)?.CardType.ToString();
                var isDefault = (result.Target as CreditCard)?.IsDefault;

                string creditCardToken = result.Target.Token;

                var dbCustomerCreditCard = new Database.Entities.CustomerCreditCard
                {
                    CustomerId = request.CustomerId,
                    Last4 = last4,
                    Token = creditCardToken,
                    Brand = brand,
                    IsDefault = isDefault.Value
                };

                await dbContext.CreditCards.AddAsync(dbCustomerCreditCard);
                await dbContext.SaveChangesAsync();

                return creditCardToken;
            }
            else
            {
                return null;
            }

        }

        public async Task<CreditCard> GetPaymentMethodByToken(string token)
        {
            var paymentMethod = await this.braintreeFactory.Gateway.PaymentMethod.FindAsync(token);
            return paymentMethod as CreditCard;
        }

        public async Task<CreatePurchaseResponse> CreatePurchaseWithToken(CreatePurchaseWithTokenRequest request)
        {
            var merchantId = "torpedotvgbp";
            switch (request.Currency)
            {
                case "GBP":
                    merchantId = "torpedotvgbp";
                    break;
                case "USD":
                    merchantId = "torpedotvusd";
                    break;
                case "EUR":
                    merchantId = "torpedotvlimited";
                    break;
                default:
                    merchantId = "torpedotvgbp";
                    break;
            }

            var dbCreditCard = await dbContext.FindAsync<CustomerCreditCard>(request.SelectedCardId);

            if (dbCreditCard == null)
            {
                return new CreatePurchaseResponse { TransactionId = null, Succeeded = false, ErrorMessage = "Credit card could not be found by this id." };
            }

            var transactionRequest = new TransactionRequest
            {
                Amount = request.Amount,
                MerchantAccountId = merchantId,
                PaymentMethodToken = dbCreditCard.Token,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = await this.braintreeFactory.Gateway.Transaction.SaleAsync(transactionRequest);

            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;
                await SaveTransactionToDatabase(transaction, dbCreditCard.CustomerId);

                return new CreatePurchaseResponse { TransactionId = transaction.Id, Succeeded = true };
            }
            else if (result.Transaction != null)
            {
                await SaveTransactionToDatabase(result.Transaction, dbCreditCard.CustomerId);
                return new CreatePurchaseResponse { TransactionId = result.Transaction.Id, Succeeded = true };
            }
            else
            {
                string errorMessages = "";
                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                }

                return new CreatePurchaseResponse { Succeeded = false, ErrorMessage = errorMessages };
            }

        }


        public async Task<Database.Entities.CustomerAddress> CreateCustomerAddress(CreateAddressRequest request)
        {
            var customer = await dbContext.FindAsync<Database.Entities.Customer>(request.CustomerId);

            var addressRequest = new AddressRequest
            {
                StreetAddress = request.StreetAddress,
                ExtendedAddress = request.ExtendedAddress,
                Region = request.Region,
                PostalCode = request.PostalCode,
                PhoneNumber = request.PhoneNumber,
                CountryName = request.CountryName
            };


            Result<Address> result = await this.braintreeFactory.Gateway.Address.CreateAsync(customer.BraintreeId, addressRequest);

            if (result.IsSuccess())
            {

                var dbCustomerAddress = new Database.Entities.CustomerAddress
                {
                    PhoneNumber = result.Target.PhoneNumber,
                    CustomerId = request.CustomerId,
                    CountryName = request.CountryName,
                    ExtendedAddress = request.ExtendedAddress,
                    PostalCode = request.PostalCode,
                    Region = request.Region,
                    StreetAddress = request.StreetAddress,
                    BraintreeId = result.Target.Id
                };

                await dbContext.Addresses.AddAsync(dbCustomerAddress);
                await dbContext.SaveChangesAsync();

                return dbCustomerAddress;
            }
            else
            {
                return null;
            }

        }


        public async Task<CustomerTransactionRefund> RefundTransaction(RefundTransactionRequest request)
        {
            var transaction = await dbContext.FindAsync<Database.Entities.CustomerTransaction>(request.TransactionId);

            if (transaction == null)
            {
                throw new System.Exception("Transaction not found");
            }

            //if (request.Amount > transaction.Amount)
            //{
            //    throw new System.Exception("Cannot be grater than the original amount");
            //}

            var brainTreeTransaction = await GetTransactionById(transaction.BraintreeTransactionId);

            if (brainTreeTransaction.Transaction == null)
            {
                throw new System.Exception("There is no corresponding transaction in Braintree system");
            }

            Result<Transaction> result;

            // According to article on https://developer.paypal.com/braintree/docs/reference/request/transaction/void#arg.transaction_id
            // the transaction should be voided instead of refund if it has the following statuses.
            if (brainTreeTransaction.Transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT
                || brainTreeTransaction.Transaction.Status == TransactionStatus.AUTHORIZED
                || brainTreeTransaction.Transaction.Status == TransactionStatus.SETTLEMENT_PENDING)
            {
                result = await this.braintreeFactory.Gateway.Transaction.VoidAsync(transaction.BraintreeTransactionId);
            }
            else
            {
                result = await this.braintreeFactory.Gateway.Transaction.RefundAsync(transaction.BraintreeTransactionId, request.Amount);
            }

            if (result.IsSuccess())
            {
                Transaction refund = result.Target;
                var dbTransactionRefund = new Database.Entities.CustomerTransactionRefund
                {
                    Amount = refund.Amount.Value,
                    CustomerTransactionId = request.TransactionId,
                };

                await dbContext.TransactionRefunds.AddAsync(dbTransactionRefund);
                await dbContext.SaveChangesAsync();

                await UpdateTransactionStatus(transaction.Id, refund.Status.ToString());

                return dbTransactionRefund;
            }
            else
            {
                return null;
            }

        }

        private async Task<long> SaveTransactionToDatabase(Transaction transaction, long customerId)
        {
            bool wasSuccessful = this.braintreeFactory.TransactionSuccessStatuses.Contains(transaction.Status);

            var status = transaction.Status.ToString();

            if (transaction.Status == TransactionStatus.GATEWAY_REJECTED)
            {
                status = status + " " + transaction.GatewayRejectionReason.ToString();
            }

            var dbTransaction = new CustomerTransaction
            {
                Amount = transaction.Amount.HasValue ? transaction.Amount.Value : 0,
                BraintreeTransactionId = transaction.Id,
                CustomerId = customerId,
                WasSuccessfull = wasSuccessful,
                ProcessorResponseCode = transaction.ProcessorResponseCode,
                ProcessorResponseText = transaction.ProcessorResponseText,
                Status = status
            };

            dbContext.Transactions.Add(dbTransaction);
            await dbContext.SaveChangesAsync();

            return dbTransaction.Id;
        }

        private async Task<long> UpdateTransactionStatus(long transactionId, string newStatus)
        {
            var transaction = await dbContext.FindAsync<CustomerTransaction>(transactionId);
            if (transaction != null)
            {
                transaction.Status = newStatus;
                await dbContext.SaveChangesAsync();

                return transaction.Id;
            }

            return 0;
        }
    }
}
