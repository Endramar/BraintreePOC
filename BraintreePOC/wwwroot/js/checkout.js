$(document).ready(function () {
    let clientToken = $("#clientToken").val();
    createBrainTreeUI(clientToken);
});

function createBrainTreeUI(clientToken) {

    var form = document.querySelector('#my-sample-form');
    var submit = document.querySelector('input[type="submit"]');
    let deviceData = "";

    braintree.client.create({
        authorization: clientToken
    }, function (clientErr, clientInstance) {
        if (clientErr) {
            console.error(clientErr);
            return;
        }

        braintree.dataCollector.create({
            client: clientInstance
        }, function (err, dataCollectorInstance) {
            if (err) {
                // Handle error in creation of data collector
                return;
            }
            // At this point, you should access the dataCollectorInstance.deviceData value and provide it
            // to your server, e.g. by injecting it into your form as a hidden input.
            deviceData = dataCollectorInstance.deviceData;
        });

        // This example shows Hosted Fields, but you can also use this
        // client instance to create additional components here, such as
        // PayPal or Data Collector.

        braintree.hostedFields.create({
            client: clientInstance,
            styles: {
                'input': {
                    'font-size': '14px'
                },
                'input.invalid': {
                    'color': 'red'
                },
                'input.valid': {
                    'color': 'green'
                }
            },
            fields: {
                number: {
                    container: '#card-number',
                    placeholder: '4111 1111 1111 1111'
                },
                cvv: {
                    container: '#cvv',
                    placeholder: '123'
                },
                expirationDate: {
                    container: '#expiration-date',
                    placeholder: '10/2022'
                }
            }
        }, function (hostedFieldsErr, hostedFieldsInstance) {
            if (hostedFieldsErr) {
                console.error(hostedFieldsErr);
                return;
            }

            submit.removeAttribute('disabled');

            form.addEventListener('submit', function (event) {
                event.preventDefault();

                hostedFieldsInstance.tokenize(function (tokenizeErr, payload) {
                    if (tokenizeErr) {
                        console.error(tokenizeErr);
                        return;
                    }

                    // If this was a real integration, this is where you would
                    // send the nonce to your server.
                    console.log('Got a nonce: ' + payload.nonce);

                    createBrainTreePurchase(payload.nonce, deviceData);

                });
            }, false);
        });
    });
}


function createBrainTreePurchase(clientNonce, deviceData) {
    let amount = +$("#amount").val();
    let currency = $("#currency").val();

    let request = {
        ClientNonce: clientNonce,
        DeviceData: deviceData,
        Amount: amount,
        Currency: currency
    };

    $.ajax({
        contentType: "application/json",
        method: "POST",
        url: "Checkout/CreatePurchase",
        data: JSON.stringify(request),
        success: function (purchaseResult) {
            console.log("PURCHASE WAS SUCCESSFUL : ", purchaseResult);

            if (purchaseResult) {
                location.href = `Checkout/Result?id=${purchaseResult.transactionId}`;
            }
            
        },
        error: function (error) {
            console.error("PURCHASE ERROR", error);
        }
    });
}


