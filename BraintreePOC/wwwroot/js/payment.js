﻿
$(document).ready(function () {
    let clientToken = $("#clientToken").val();

    createBraintreeClientWith3dSecure(clientToken)
        .then(function (instances) {
            let clientInstance = instances[0];
            let threeDInstance = instances[1];
            let dataCollectorInstance = instances[2];
            createBraintreeHostedfields(clientInstance)
                .then(function (hostedFieldsInstance) {
                    finalizeHostedFieldsCreation(hostedFieldsInstance, threeDInstance, dataCollectorInstance);
                }).catch(function (err) {
                    console.error(err);
                });

        }).catch(function (err) {
            console.error(err);
        });

    $("#paymentWithSavedForm").submit(function (event) {
        event.preventDefault();
        createBrainTreePurchaseWithToken();
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

function createBraintreeClientWith3dSecure(clientToken) {
    return Promise.all([
        braintree.client.create({
            authorization: clientToken
        }),
        braintree.threeDSecure.create({
            authorization: clientToken,
            version: 2
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



function finalizeHostedFieldsCreation(hostedFieldsInstance, threeDInstance, dataCollectorInstance) {
    createHostedFieldsCardTypeEvent(hostedFieldsInstance);
    createHostedFieldsValidationChangeEvent(hostedFieldsInstance);
    createPaymentFormSubmitEvent(hostedFieldsInstance, threeDInstance, dataCollectorInstance);
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


function createPaymentFormSubmitEvent(hostedFieldsInstance, threeDInstance, dataCollectorInstance) {
    $("#paymentForm").submit(function (event) {
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

        tokenizeHostedFieldsInstance(hostedFieldsInstance, threeDInstance, dataCollectorInstance);
    });
}


function tokenizeHostedFieldsInstance(hostedFieldsInstance, threeDInstance, dataCollectorInstance) {
    let amount = $("#amount").val();

    hostedFieldsInstance.tokenize().then(function (payload) {
        return threeDInstance.verifyCard({
            onLookupComplete: function (data, next) {
                next();
            },
            amount: amount,
            nonce: payload.nonce,
            bin: payload.details.bin,
            //email: billingFields.email.input.value,
            billingAddress: {
                postalCode: "SW6 5NN",
                countryName : "United Kingdom"
            }
        })
    }).then(function (payload) {
        if (!payload.liabilityShifted) {
            console.log('Liability did not shift', payload);
            return;
        }

        console.log('verification success:', payload);
        createBrainTreePurchase(payload.nonce, dataCollectorInstance.deviceData);

    }).catch(function (err) {
        console.log(err);
    });
}


function createBrainTreePurchase(clientNonce, deviceData) {
    let amount = +$("#amount").val();
    let currency = $("#currency").val();
    let customerId = +$("#customerId").val();

    let request = {
        ClientNonce: clientNonce,
        DeviceData: deviceData,
        Amount: amount,
        Currency: currency,
        CustomerId : customerId
    };

    $("#payWithoutTokenButton").attr("disabled", true);

    $.ajax({
        contentType: "application/json",
        method: "POST",
        url: "/Checkout/CreatePurchase",
        data: JSON.stringify(request),
        success: function (purchaseResult) {
            console.log("PURCHASE WAS SUCCESSFUL : ", purchaseResult);
            if (purchaseResult) {
                location.href = `/Checkout/Result?id=${purchaseResult.transactionId}`;
            }
        },
        error: function (error) {
            $("#payWithoutTokenButton").attr("disabled", false);
            console.error("PURCHASE ERROR", error);
        }
    });
}


function verify3DSecurity(hostedFieldsInstance, threeDInstance, dataCollectorInstance) {
    let amount = $("#amount").val();

    hostedFieldsInstance.tokenize().then(function (payload) {
        return threeDInstance.verifyCard({
            onLookupComplete: function (data, next) {
                next();
            },
            amount: amount,
            
        })
    }).then(function (payload) {
        if (!payload.liabilityShifted) {
            console.log('Liability did not shift', payload);
            return;
        }

        console.log('verification success:', payload);
        createBrainTreePurchase(payload.nonce, dataCollectorInstance.deviceData);

    }).catch(function (err) {
        console.log(err);
    });
}


function createBrainTreePurchaseWithToken() {
    let amount = +$("#amount").val();
    let currency = $("#currency").val();
    let selectedCardId = +$("#selectedCreditCardId").val();


    let request = {
        Amount: amount,
        Currency: currency,
        SelectedCardId: selectedCardId
    };

    $("#payWithTokenButton").attr("disabled", true);

    $.ajax({
        contentType: "application/json",
        method: "POST",
        url: "/Checkout/CreatePurchaseWithToken",
        data: JSON.stringify(request),
        success: function (purchaseResult) {
            console.log("PURCHASE WAS SUCCESSFUL : ", purchaseResult);

            if (purchaseResult) {
                location.href = `/Checkout/Result?id=${purchaseResult.transactionId}`;
            }

        },
        error: function (error) {
            $("#payWithTokenButton").attr("disabled", false);
            console.error("PURCHASE ERROR", error);
        }
    });
}

