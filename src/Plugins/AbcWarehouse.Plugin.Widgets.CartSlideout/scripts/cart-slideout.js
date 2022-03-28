// Globals
const CartSlideoutProductInfo = document.getElementById("cart-slideout__product-info");

const CartSlideoutOverlay = document.getElementById("cart-slideout-overlay");
const CartSlideout = document.getElementById("cart-slideout");
const CartSlideoutBackButton = document.getElementById("cart-slideout__back");

const input = document.getElementById("cart-slideout__delivery-input");
const zipCodeInput = document.getElementById('cart-slideout__delivery-zip-code-input');
const checkButton = document.getElementById("cart-slideout__check-delivery-options");

const deliveryNotAvailable = document.getElementById("cart-slideout__delivery-not-available");


// Set up enable/disable for zip code input/button
zipCodeInput.addEventListener('keyup', updateCheckDeliveryAvailabilityButton);

function updateCheckDeliveryAvailabilityButton() {
  if (zipCodeInput === undefined) { return; }

  const isNumber = /^\d+$/.test(zipCodeInput.value);

  zipCodeInput.disabled = false;
  checkButton.disabled = !isNumber || zipCodeInput.value.length !== 5;
  checkButton.innerText = "Check Delivery/Pickup Options";
}


function displayCartSlideout(response) {
    document.getElementById("cart-slideout__delivery-input").style.display = response.IsAbcDeliveryItem ? "block" : "none";
    
    showCartSlideout();
}

function showCartSlideout() {
    deliveryNotAvailable.style.display = "none";
    CartSlideoutBackButton.style.display = "none";

    CartSlideout.style.width = "320px";
    CartSlideout.style.padding = "2.5rem 1rem 0 1rem";
    CartSlideoutOverlay.style.display = "block";
    document.body.classList.add("scrollYRemove");

    updateCheckDeliveryAvailabilityButton();
}

function hideCartSlideout() {
    CartSlideout.style.width = "0";
    CartSlideout.style.padding = "0";
    CartSlideoutOverlay.style.display = "none";
    document.body.classList.remove("scrollYRemove");
}

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
    //openDeliveryOptions(responseJson);
    updateCheckDeliveryAvailabilityButton();
}

// function openDeliveryOptions(response) {
//     document.getElementById("cart-slideout__delivery-input").style.display = "none";
    
//     deliveryNotAvailable.style.display = "none";
//     deliveryOptions.style.display = "none";

//     if (response.isDeliveryAvailable) {
//         deliveryOptions.style.display = "block";
//     } else {
//         deliveryNotAvailable.style.display = "block";
//     }

//     CartSlideoutBackButton.style.display = "block";
// }

function back() {
    deliveryNotAvailable.style.display = "none";
    deliveryOptions.style.display = "none";
    CartSlideoutBackButton.style.display = "none";

    input.style.display = "block";
}