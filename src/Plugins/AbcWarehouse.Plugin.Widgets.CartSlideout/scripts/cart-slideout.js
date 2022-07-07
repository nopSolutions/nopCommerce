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

const warrantyOptions = document.querySelector('.cart-slideout__warranty');

const deliveryOptionsInformation = document.querySelector('.delivery-options-information');
const warrantyInformation = document.querySelector('.warranty-information');

// Event listeners for updating the check delivery options button:
zipCodeInput.addEventListener('keyup', updateCheckDeliveryAvailabilityButton);

async function checkDeliveryShippingAvailabilityAsync() {
    zipCodeInput.disabled = true;
    checkButton.disabled = true;
    checkButton.innerText = "Checking...";

    const zip = zipCodeInput.value;
    document.cookie = `customerZipCode=${zip}`;
    const response = await fetch(`/AddToCart/GetDeliveryOptions?zip=${zip}`);
    if (response.status != 200) {
        alert('Error occurred when checking delivery options.');
        updateCheckDeliveryAvailabilityButton();
        return;
    }

    const responseJson = await response.json();
    openDeliveryOptions(responseJson);
    updateCheckDeliveryAvailabilityButton();
}

function openDeliveryOptions(response) {
    deliveryInput.style.display = "none";
    deliveryNotAvailable.style.display = "none";
    deliveryOptions.style.display = "none";

    if (response.isDeliveryAvailable) {
        deliveryOptions.style.display = "block";
        deliveryOptionsContinue.style.display = "block";
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
    CartSlideout.style.display = "block";
    document.body.classList.add("scrollYRemove");
}

function hideCartSlideout() {
    CartSlideoutOverlay.style.display = "none";
    CartSlideout.style.display = "none";
    deliveryOptions.style.display = "none";
    deliveryOptionsContinue.style.display = "none";
    deliveryNotAvailable.style.display = "none";
    warrantyOptions.style.display = "none";
    cartSlideoutBackButton.style.display = "none";

    hideDeliveryOptionsInformation();
    hideWarrantyInformation();

    document.body.classList.remove("scrollYRemove");
}

function hideDeliveryOptionsInformation() {
    deliveryOptionsInformation.style.display = "none";
}

function hideWarrantyInformation() {
    warrantyInformation.style.display = "none";
}

// Checks currently open sub-screen and goes to the appropriate screen
function back() {
    if (warrantyOptions.style.display === "block") {
        warrantyOptions.style.display = "none";

        deliveryOptions.style.display = "block";
        deliveryOptionsContinue.style.display = "block";
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
    if (response.slideoutInfo.WarrantyHtml) {
        $('.cart-slideout__warranty').html(response.slideoutInfo.WarrantyHtml);
    }

    // Should only check zip code if product has delivery options
    if (response.slideoutInfo.DeliveryOptionsHtml &&
        response.slideoutInfo.DeliveryOptionsHtml !== "\n") {
        deliveryInput.style.display = "block";
        zipCodeInput.value = getCookie('customerZipCode');
        updateCheckDeliveryAvailabilityButton();
        $('.cart-slideout__delivery-options').html(response.slideoutInfo.DeliveryOptionsHtml);
        setAttributeListeners(response.slideoutInfo.ShoppingCartItemId);
    }
}

function toWarranty() {
    deliveryOptions.style.display = "none";
    deliveryOptionsContinue.style.display = "none";

    warrantyOptions.style.display = "block";
}

function setAttributeListeners(shoppingCartItemId) {
    setInformationalIconListeners();

    // TODO: Refactor this
    var deliveryOptions = document.querySelectorAll('.cart-slideout__delivery-options [name^=product_attribute_]');
    var warrantyOptions = document.querySelectorAll('.cart-slideout__warranty [name^=product_attribute_]');
    for (option in deliveryOptions) {
        deliveryOptions[option].onclick = function() {
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
    var deliveryOptionInformationIcon = document.querySelector('i.delivery-options-info');
    var warrantyInformationIcon = document.querySelector('i.warranty-info');
    deliveryOptionInformationIcon.onclick = function () {
        deliveryOptionsInformation.style.display = "block";
    };
    warrantyInformationIcon.onclick = function () {
        warrantyInformation.style.display = "block";
    };
}

function getCookie(cookieName) {
    let cookie = {};
    document.cookie.split(';').forEach(function(el) {
      let [key,value] = el.split('=');
      cookie[key.trim()] = value;
    })
    return cookie[cookieName] ?? '';
}