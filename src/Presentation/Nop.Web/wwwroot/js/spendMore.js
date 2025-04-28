$(document).ready(function(){ 
   var spendMore = 40 - Number($(".order-subtotal .value-summary")[0].innerText.replace("£",""));
   var spendMoreMessage = 'Free delivery on all orders over £40.00';
   if (spendMore <= 10) {
      spendMoreMessage = spendMore <= 0 ? 'Free delivery' : 'Spend another £' + spendMore.toFixed(2) + ' for free delivery';
   }
   $(".order-total").after('<tr><td colspan="2">' + spendMoreMessage + '</td></tr>');
});