
$(document).ready(function () {
    let clientToken = $("#clientToken").val();

    createBraintreeClientWithout3dSecure(clientToken)
        .then(function (instances) {
            let clientInstance = instances[0];
            let dataCollectorInstance = instances[1];
            createBraintreeHostedfields(clientInstance)
                .then(function (hostedFieldsInstance) {
                    finalizeHostedFieldsCreation(hostedFieldsInstance, dataCollectorInstance);
                }).catch(function (err) {
                    console.error(err);
                });

        }).catch(function (err) {
            console.error(err);
        });

});

$('#cc-name').on('change', function () {
    validateInput(ccName);
});

function validateInput(element) {
    if (!element.val().trim()) {
        setValidityClasses(element, false);
        return false;
    }
    setValidityClasses(element, true);
    return true;
}

function setValidityClasses(element, validity) {
    if (validity) {
        element.removeClass('is-invalid');
        element.addClass('is-valid');
    } else {
        element.addClass('is-invalid');
        element.removeClass('is-valid');
    }
}

 
function createBraintreeClientWithout3dSecure(clientToken) {
    return Promise.all([
        braintree.client.create({
            authorization: clientToken
        }),
        braintree.dataCollector.create({
            authorization: clientToken
        })
    ])
}


function createBraintreeHostedfields(clientInstance) {
    return braintree.hostedFields.create({
        client: clientInstance,
        styles: {
            input: {
                'font-size': '1rem',
                color: '#495057'
            }
        },
        fields: {
            cardholderName: {
                selector: '#cc-name',
                placeholder: 'Name as it appears on your card'
            },
            number: {
                selector: '#cc-number',
                placeholder: '4111 1111 1111 1111'
            },
            cvv: {
                selector: '#cc-cvv',
                placeholder: '123'
            },
            expirationDate: {
                selector: '#cc-expiration',
                placeholder: 'MM / YY'
            }
        }
    });
}



function finalizeHostedFieldsCreation(hostedFieldsInstance, dataCollectorInstance) {
    createHostedFieldsCardTypeEvent(hostedFieldsInstance);
    createHostedFieldsValidationChangeEvent(hostedFieldsInstance);
    createPaymentFormSubmitEvent(hostedFieldsInstance, dataCollectorInstance);
}

function createHostedFieldsCardTypeEvent(hostedFieldsInstance) {
    hostedFieldsInstance.on('cardTypeChange', function (event) {
        var cardBrand = $('#card-brand');
        var cvvLabel = $('[for="cc-cvv"]');

        if (event.cards.length === 1) {
            var card = event.cards[0];

            // change pay button to specify the type of card
            // being used
            cardBrand.text(card.niceType);
            // update the security code label
            cvvLabel.text(card.code.name);
        } else {
            // reset to defaults
            cardBrand.text('Card');
            cvvLabel.text('CVV');
        }
    });
}

function createHostedFieldsValidationChangeEvent(hostedFieldsInstance) {
    hostedFieldsInstance.on('validityChange', function (event) {
        var field = event.fields[event.emittedBy];

        // Remove any previously applied error or warning classes
        $(field.container).removeClass('is-valid');
        $(field.container).removeClass('is-invalid');

        if (field.isValid) {
            $(field.container).addClass('is-valid');
        } else if (field.isPotentiallyValid) {
            // skip adding classes if the field is
            // not valid, but is potentially valid
        } else {
            $(field.container).addClass('is-invalid');
        }
    });
}


function createPaymentFormSubmitEvent(hostedFieldsInstance, dataCollectorInstance) {
    $("#creditCardForm").submit(function (event) {
        event.preventDefault();

        var formIsInvalid = false;
        var state = hostedFieldsInstance.getState();

        // Loop through the Hosted Fields and check
        // for validity, apply the is-invalid class
        // to the field container if invalid
        Object.keys(state.fields).forEach(function (field) {
            if (!state.fields[field].isValid) {
                $(state.fields[field].container).addClass('is-invalid');
                formIsInvalid = true;
            }
        });

        if (formIsInvalid) {
            // skip tokenization request if any fields are invalid
            return;
        }

        tokenizeHostedFieldsInstance(hostedFieldsInstance, dataCollectorInstance);
    });
}

function tokenizeHostedFieldsInstance(hostedFieldsInstance, dataCollectorInstance) {
    hostedFieldsInstance.tokenize().then(function (payload) {
        let clientNonce = payload.nonce;
        saveCustomerCreditCard(clientNonce, dataCollectorInstance.deviceData);
    }).catch(function (err) {
        console.log(err);
    });
}


function saveCustomerCreditCard(clientNonce, deviceData) {
   
    let customerId = +$("#customerId").val();

    let request = {
        ClientNonce: clientNonce,
        DeviceData: deviceData,
        CustomerId: customerId
    };

    $.ajax({
        contentType: "application/json",
        method: "POST",
        url: "/Customer/CreateCustomerCreditCard",
        data: JSON.stringify(request),
        success: function (customerId) {
            location.reload();
        },
        error: function (error) {
            console.error("CREDIT CARD CREATION FAILED", error);
        }
    });

}

 


