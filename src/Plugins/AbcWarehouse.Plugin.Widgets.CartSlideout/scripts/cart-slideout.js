// Globals
const cartSlideoutBackButton = document.querySelector('.cart-slideout__back');

const CartSlideoutProductInfo = document.getElementsByClassName("cart-slideout__product-info")[0];

const CartSlideoutOverlay = document.getElementsByClassName("cart-slideout-overlay")[0];
const CartSlideout = document.getElementsByClassName("cart-slideout")[0];

const input = document.getElementsByClassName("cart-slideout__delivery-input")[0];
const zipCodeInput = document.getElementsByClassName('cart-slideout__delivery-zip-code-input')[0];
const checkButton = document.querySelector(".cart-slideout__check-delivery-options__btn");

const deliveryOptions = document.querySelector('.cart-slideout__delivery-options');
const deliveryNotAvailable = document.querySelector('.cart-slideout__delivery-not-available');

// Event listeners for updating the check delivery options button:
zipCodeInput.addEventListener('keyup', updateCheckDeliveryAvailabilityButton);

async function checkDeliveryShippingAvailabilityAsync() {
    zipCodeInput.disabled = true;
    checkButton.disabled = true;
    checkButton.innerText = "Checking...";

    const zip = zipCodeInput.value;
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
    input.style.display = "none";
    deliveryNotAvailable.style.display = "none";
    deliveryOptions.style.display = "none";

    if (response.isDeliveryAvailable) {
        deliveryOptions.style.display = "block";
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

    CartSlideout.style.width = "320px";
    CartSlideout.style.padding = "2.5rem 1rem 0 1rem";
    CartSlideoutOverlay.style.display = "block";
    document.body.classList.add("scrollYRemove");
}

function hideCartSlideout() {
    CartSlideout.style.width = "0";
    CartSlideout.style.padding = "0";
    CartSlideoutOverlay.style.display = "none";
    document.body.classList.remove("scrollYRemove");
}

function back() {
    deliveryNotAvailable.style.display = "none";
    deliveryOptions.style.display = "none";
    cartSlideoutBackButton.style.display = "none";

    input.style.display = "block";
}

function updateCartSlideoutHtml(response) {
    if (response.slideoutInfo.ProductInfoHtml) {
        $('.cart-slideout__product-info').html(response.slideoutInfo.ProductInfoHtml);
    }
    if (response.slideoutInfo.SubtotalHtml) {
        $('.cart-slideout__subtotal').html(response.slideoutInfo.SubtotalHtml);
    }
    if (response.slideoutInfo.ProductAttributesHtml) {
        $('.cart-slideout__attributes').html(response.slideoutInfo.ProductAttributesHtml);
    }
    if (response.slideoutInfo.DeliveryOptionsHtml) {
        $('.cart-slideout__delivery-options').html(response.slideoutInfo.DeliveryOptionsHtml);
        setAttributeListeners(response.slideoutInfo.ShoppingCartItemId);
    }
}

function setAttributeListeners(shoppingCartItemId) {
    var options = document.querySelectorAll('.cart-slideout__delivery-options [name^=product_attribute_]');
    for (option in options) {
        options[option].onclick = function() {
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