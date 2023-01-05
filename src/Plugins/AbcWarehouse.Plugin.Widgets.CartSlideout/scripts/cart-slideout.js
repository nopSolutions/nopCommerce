// Globals
const cartSlideoutBackButton = document.querySelector('.cart-slideout__back');

const CartSlideoutOverlay = document.getElementsByClassName("cart-slideout-overlay")[0];
const CartSlideout = document.getElementsByClassName("cart-slideout")[0];

const CartSlideoutProductInfo = document.getElementsByClassName("cart-slideout__product-info")[0];

const deliveryInput = document.getElementsByClassName("cart-slideout__delivery-input")[0];
const zipCodeInput = document.getElementsByClassName('cart-slideout__delivery-zip-code-input')[0];
const checkButton = document.querySelector(".cart-slideout__check-delivery-options__btn");

const deliveryOptions = document.querySelector('.cart-slideout__delivery-options');
const deliveryNotAvailable = document.querySelector('.cart-slideout__delivery-not-available');
const deliveryOptionsContinue = document.querySelector('.cart-slideout__delivery-options-continue');

const pickupInStoreOptions = document.querySelector('.cart-slideout__pickup-in-store');

const warrantyOptions = document.querySelector('.cart-slideout__warranty');

const deliveryOptionsInformation = document.querySelector('.delivery-options-information');
const warrantyInformation = document.querySelector('.warranty-information');

var cartSlideoutShoppingCartItemId = 0;
var cartSlideoutProductId = 0;
var isPickup = false;
var hasWarranties = false;
var editMode = false;

// Event listeners for updating the check delivery options button:
zipCodeInput.addEventListener('keyup', updateCheckDeliveryAvailabilityButton);

async function checkDeliveryShippingAvailabilityAsync() {
    zipCodeInput.disabled = true;
    checkButton.disabled = true;

    const zip = zipCodeInput.value;
    document.cookie = `customerZipCode=${zip}`;
    const response = await fetch(`/AddToCart/GetDeliveryOptions?zip=${zip}&productId=${cartSlideoutProductId}`);
    if (response.status != 200) {
        alert('Error occurred when checking delivery options.');
        updateCheckDeliveryAvailabilityButton();
        return;
    }

    const responseJson = await response.json();
    openDeliveryOptions(responseJson);
    if (responseJson.pickupInStoreHtml) {
        $('.cart-slideout__pickup-in-store').html(responseJson.pickupInStoreHtml);
    }
    setInformationalIconListeners();
    updateCheckDeliveryAvailabilityButton();
}

function openDeliveryOptions(response) {
    deliveryInput.style.display = "none";
    deliveryNotAvailable.style.display = "none";
    deliveryOptions.style.display = "none";

    if (response.isDeliveryAvailable) {
        deliveryOptions.style.display = "block";
        updateContinueButton();
    } else {
        deliveryNotAvailable.style.display = "block";
    }

    cartSlideoutBackButton.style.display = "block";
}

function updateCheckDeliveryAvailabilityButton() {
    if (zipCodeInput === undefined) { return; }
  
    const isNumber = /^\d+$/.test(zipCodeInput.value);
  
    zipCodeInput.disabled = false;
    checkButton.disabled = !isNumber || zipCodeInput.value.length !== 5;
    checkButton.innerText = "Check Delivery/Shipping Availability";
}

function showCartSlideout(response) {
    updateCartSlideoutHtml(response);

    CartSlideoutOverlay.style.display = "block";
    CartSlideout.style.display = "flex";
    document.body.classList.add("scrollYRemove");
}

function hideCartSlideout() {
    if (editMode) {
        location.reload();
    } else {
        CartSlideoutOverlay.style.display = "none";
        CartSlideout.style.display = "none";
        deliveryOptions.style.display = "none";
        deliveryOptionsContinue.style.display = "none";
        deliveryNotAvailable.style.display = "none";
        warrantyOptions.style.display = "none";
        cartSlideoutBackButton.style.display = "none";

        isPickup = false;
        hasWarranties = false;

        hideDeliveryOptionsInformation();
        hideWarrantyInformation();

        document.body.classList.remove("scrollYRemove");
    }
}

function hideDeliveryOptionsInformation() {
    deliveryOptionsInformation.style.display = "none";
}

function hideWarrantyInformation() {
    warrantyInformation.style.display = "none";
}

// Checks currently open sub-screen and goes to the appropriate screen
function back() {
    if (pickupInStoreOptions.style.display === "block") {
        pickupInStoreOptions.style.display = "none";

        deliveryOptions.style.display = "block";
        updateContinueButton();
    } else if (warrantyOptions.style.display === "block") {
        warrantyOptions.style.display = "none";

        if (isPickup) {
            pickupInStoreOptions.style.display = "block";
        } else {
            deliveryOptions.style.display = "block";
            updateContinueButton();
        }
    } else if (deliveryOptions.style.display === "block" || deliveryNotAvailable.style.display === "block") {
        deliveryNotAvailable.style.display = "none";
        deliveryOptions.style.display = "none";
        deliveryOptionsContinue.style.display = "none";
        cartSlideoutBackButton.style.display = "none";

        deliveryInput.style.display = "block";
    } else {
        console.log('Unable to find existing state for back button.');
    }
}

function updateCartSlideoutHtml(response) {
    if (response.slideoutInfo.ProductInfoHtml) {
        $('.cart-slideout__product-info').html(response.slideoutInfo.ProductInfoHtml);
    }
    if (response.slideoutInfo.SubtotalHtml) {
        $('.cart-slideout__subtotal').html(response.slideoutInfo.SubtotalHtml);
    }
    hasWarranties = response.slideoutInfo.WarrantyHtml &&
                    response.slideoutInfo.WarrantyHtml !== "\n" &&
                    response.slideoutInfo.WarrantyHtml !== "\r\n";
    if (hasWarranties) {
        $('.cart-slideout__warranty').html(response.slideoutInfo.WarrantyHtml);
    }

    // Should only check zip code if product has delivery options
    if (response.slideoutInfo.DeliveryOptionsHtml &&
        response.slideoutInfo.DeliveryOptionsHtml !== "\n" &&
        response.slideoutInfo.DeliveryOptionsHtml !== "\r\n") {
        deliveryInput.style.display = "block";
        zipCodeInput.value = getCookie('customerZipCode');
        updateCheckDeliveryAvailabilityButton();
        $('.cart-slideout__delivery-options').html(response.slideoutInfo.DeliveryOptionsHtml);
        setAttributeListeners(response.slideoutInfo.ShoppingCartItemId);
        cartSlideoutProductId = response.slideoutInfo.ProductId;
        cartSlideoutShoppingCartItemId = response.slideoutInfo.ShoppingCartItemId;
    }
}

function updateContinueButton() {
    deliveryOptionsContinue.style.display = hasWarranties || isPickup ? "block" : "none";
}

function clickContinueButton() {
    deliveryOptions.style.display = "none";
    deliveryOptionsContinue.style.display = "none";

    // determine which option was selected
    if (isPickup) {
        pickupInStoreOptions.style.display = "block";
    } else {
        warrantyOptions.style.display = "block";
    }
}

async function selectStoreAsync(shopId)
{
    const payload = {
        ShoppingCartItemId: cartSlideoutShoppingCartItemId,
        ShopId: shopId
    }
    const response = await fetch('/AddToCart/SelectPickupStore', {
        method: 'POST',
        headers: {
            'content-type': 'application/json'
        },
        body: JSON.stringify(payload)
    })
    if (response.status != 200) {
        alert('Error occurred when selecting pickup store.');
        return;
    }

    pickupInStoreOptions.style.display = "none";
    warrantyOptions.style.display = "block";
}

function setAttributeListeners(shoppingCartItemId) {
    setInformationalIconListeners();

    // TODO: Refactor this
    var deliveryOptions = document.querySelectorAll('.cart-slideout__delivery-options [name^=product_attribute_]');
    var warrantyOptions = document.querySelectorAll('.cart-slideout__warranty [name^=product_attribute_]');
    for (option in deliveryOptions) {
        deliveryOptions[option].onclick = function() {
            var continueButton = document.querySelector('.cart-slideout__continue__btn');
            continueButton.disabled = true;

            const [attributeMappingId] = this.name.split('_').slice(-1);
            const payload = {
                shoppingCartItemId: shoppingCartItemId,
                productAttributeMappingId: attributeMappingId,
                productAttributeValueId: this.value,
                isChecked: this.checked
            };
            fetch('/CartSlideout/UpdateShoppingCartItem', {
                method: 'POST',
                headers: {
                    'content-type': 'application/json'
                },
                body: JSON.stringify(payload)
            })
            .then(response => response.json())
            .then(responseJson => {
                // update possible conditional options
                responseJson.DisabledAttributeMappingIds.forEach(id => {
                    var options = document.querySelectorAll(`[id$='_${id}']`);
                    options.forEach(option => {
                        option.style.display = "none";
                        // Uncheck the hidden option
                        var checkbox = option.querySelector(`input[id^='product_attribute_${id}_']`);
                        if (checkbox !== undefined && checkbox !== null) {
                            checkbox.checked = false;
                        }
                    })
                });

                responseJson.EnabledAttributeMappingIds.forEach(id => {
                    var options = document.querySelectorAll(`[id$='_${id}']`);
                    options.forEach(option => {
                        option.style.display = "block";
                    })
                });

                isPickup = responseJson.IsPickup;
                updateContinueButton();

                $('.cart-slideout__subtotal').html(responseJson.SubtotalHtml);

                continueButton.disabled = false;
            })
            .catch(err => {
                console.log(err)
            })
        }
    }

    for (option in warrantyOptions) {
        warrantyOptions[option].onclick = function() {
            const [attributeMappingId] = this.name.split('_').slice(-1);
            const payload = {
                shoppingCartItemId: shoppingCartItemId,
                productAttributeMappingId: attributeMappingId,
                productAttributeValueId: this.value,
                isChecked: this.checked
            };
            fetch('/CartSlideout/UpdateShoppingCartItem', {
                method: 'POST',
                headers: {
                    'content-type': 'application/json'
                },
                body: JSON.stringify(payload)
            })
            .then(response => response.json())
            .then(responseJson => {
                $('.cart-slideout__subtotal').html(responseJson.SubtotalHtml);
            })
            .catch(err => {
                console.log(err)
            })
        }
    }
}

function setInformationalIconListeners() {
    var deliveryOptionInformationIcons = document.querySelectorAll('i.delivery-options-info');
    var warrantyInformationIcon = document.querySelector('i.warranty-info');
    for (icon in deliveryOptionInformationIcons) {
        deliveryOptionInformationIcons[icon].onclick = function() {
            deliveryOptionsInformation.style.display = "block";
        }
    }

    if (warrantyInformationIcon !== null) {
        warrantyInformationIcon.onclick = function () {
            warrantyInformation.style.display = "block";
        };
    }
}

async function editCartItemAsync(shoppingCartItemId) {
    editMode = true;
    var zip = getCookie('customerZipCode');

    AjaxCart.setLoadWaiting(true);
    const response = await fetch(`/AddToCart/GetEditCartItemInfo?shoppingCartItemId=${shoppingCartItemId}&zip=${zip}`);
    if (response.status != 200) {
        alert('Error occurred when editing cart item.');
        AjaxCart.setLoadWaiting(false);
        return;
    }
    AjaxCart.setLoadWaiting(false);

    const responseJson = await response.json();
    showCartSlideout(responseJson);

    // now we'll need to show warranty options, or pickup options, or delivery options
    cartSlideoutBackButton.style.display = "block";
    deliveryInput.style.display = "none";

    if (hasWarranties) {
        warrantyOptions.style.display = "block";
    } else {
        deliveryOptions.style.display = "block";
    }

    document.querySelector('.cart-slideout__buttons__cart').style.display = "none";
}

function getCookie(cookieName) {
    let cookie = {};
    document.cookie.split(';').forEach(function(el) {
      let [key,value] = el.split('=');
      cookie[key.trim()] = value;
    })
    return cookie[cookieName] ?? '';
}