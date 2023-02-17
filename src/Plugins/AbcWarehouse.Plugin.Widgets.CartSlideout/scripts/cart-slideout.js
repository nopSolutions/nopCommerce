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

const deliveryOptionsInformation = document.querySelector('.delivery-options-information');
const warrantyInformation = document.querySelector('.warranty-information');

var cartSlideoutShoppingCartItemId = 0;
var productId = 0;
var editMode = false;
var selectedShop = "";

// Event listeners for updating the check delivery options button:
zipCodeInput.addEventListener('keyup', updateCheckDeliveryAvailabilityButton);

async function checkDeliveryShippingAvailabilityAsync() {
    zipCodeInput.disabled = true;
    checkButton.disabled = true;

    const zip = zipCodeInput.value;
    document.cookie = `customerZipCode=${zip}`;
    const response = await fetch(`/AddToCart/GetDeliveryOptions?zip=${zip}&productId=${productId}`);
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
    // if (editMode) {
    //     location.reload();
    // } else {
        CartSlideoutOverlay.style.display = "none";
        CartSlideout.style.display = "none";
        deliveryOptions.style.display = "none";
        deliveryNotAvailable.style.display = "none";
        cartSlideoutBackButton.style.display = "none";

        deliveryOptionsInformation.style.display = "none";
        warrantyInformation.style.display = "none";

        document.body.classList.remove("scrollYRemove");
    // }
}

// Checks currently open sub-screen and goes to the appropriate screen
function back() {
    if (deliveryOptions.style.display === "block" || deliveryNotAvailable.style.display === "block") {
        deliveryNotAvailable.style.display = "none";
        deliveryOptions.style.display = "none";
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

    // Should only check zip code if product has delivery options
    if (response.slideoutInfo.DeliveryOptionsHtml !== "") {
        deliveryInput.style.display = "block";
        zipCodeInput.value = getCookie('customerZipCode');
        updateCheckDeliveryAvailabilityButton();
        $('.cart-slideout__delivery-options').html(response.slideoutInfo.DeliveryOptionsHtml);
        setAttributeListeners(response.slideoutInfo.ShoppingCartItemId);
        productId = response.slideoutInfo.ProductId;
        cartSlideoutShoppingCartItemId = response.slideoutInfo.ShoppingCartItemId;
    }
}

async function selectStoreAsync(shopId, message)
{
    var elements = document.querySelectorAll("button[id^='select_store_'");
    elements.forEach(e => e.classList.remove("selected"));

    var selectedElement = document.querySelector(`#select_store_${shopId}`);
    selectedElement.classList.add("selected");

    selectedShop = `${shopId};${message}`

    return;
}

function setAttributeListeners() {
    setInformationalIconListeners();

    var deliveryOptions = document.querySelectorAll('.cart-slideout__delivery-options [name^=product_attribute_]');
    if (deliveryOptions.length < 2) { return; }
    for (option in deliveryOptions) {
        deliveryOptions[option].onclick = function() {
            fetch(`/slideout_attributechange?productId=${productId}`, {
                method: 'POST',
                headers: {
                    'content-type': 'application/x-www-form-urlencoded; charset=UTF-8'
                },
                body: $('#delivery-options').serialize()
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

                const pickupInStoreOptions = document.querySelector('.cart-slideout__pickup-in-store');
                pickupInStoreOptions.style.display = responseJson.IsPickup ?
                    "block" :
                    "none";

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

    deliveryOptions.style.display = "block";

    document.querySelector('.cart-slideout__buttons__cart').style.display = "none";
}

async function addCartItemAsync(productId) {
    editMode = true;

    AjaxCart.setLoadWaiting(true);
    const response = await fetch(`/AddToCart/GetAddCartItemInfo?productId=${productId}`, {
        method: 'POST',
        headers: {
            'content-type': 'application/x-www-form-urlencoded; charset=UTF-8'
        },
        body: $('#product-details-form').serialize()
    });
    if (response.status != 200) {
        alert('Error occurred when getting delivery information.');
        AjaxCart.setLoadWaiting(false);
        return;
    }
    AjaxCart.setLoadWaiting(false);

    const responseJson = await response.json();
    showCartSlideout(responseJson);
}

function getCookie(cookieName) {
    let cookie = {};
    document.cookie.split(';').forEach(function(el) {
      let [key,value] = el.split('=');
      cookie[key.trim()] = value;
    })
    return cookie[cookieName] ?? '';
}

function AddToCart()
{
    var payload = $('#delivery-options, #product-details-form').serialize();
    if (selectedShop != "")
    {
        payload += `&selectedShopId=${selectedShop}`;
    }

    $.ajax({
        cache: false,
        url: `/addproducttocart/details/${productId}/1`,
        data: payload,
        type: "POST",
        success: function(response) {
            if (response.redirect) {
                location.href = response.redirect;
                return true;
            }
            return false;
        },
        error: function() {
            alert('Error when adding item to cart.');
        }
    });
    return false;
}