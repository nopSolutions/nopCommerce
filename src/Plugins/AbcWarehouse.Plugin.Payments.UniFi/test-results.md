# Initial call after modal close

Payments.UniFi: Transaction lookup response: {"inquiryType":"BUY","creditAuthorizationInfo":{"transactionCode":"033","transactionMessage":"Customer Approval Success","merchantInfo":{"merchantNumber":"5348121490000021"}}}

# Transact API call

Payments.UniFi: Transact API request: {"transactionToken":"PI200014189151118b24492cbd","addressInfo":{"cipher.addressLine1":"950 Forrer Blvd","cipher.city":"Dayton","cipher.state":"OH","cipher.zipCode":"45420"},"transactionInfo":[{"amount":"919.07"}]}

Payments.UniFi: Transact API response: {"dbuyTokenId":"PI200014186978118b243d2293","transactionToken":"PI200014186978118b243d2293","transactionInfo":{"authorizationCode":"012716","transactionType":"AUTHORIZATION","status":"Auth Approved","statusCode":"000","transactionDate":"10/12/2023 14:15:50","amount":"919.07","promotionCode":"106"},"cardInfo":{"cardHolderName":"TEST WAREHOUSE","accountId":"XXXXXXXXXXXX1071","expiry":{"month":"12","year":"49"},"last4AccountNumber":"1071"}}

# 2nd Transaction Lookup (to get account number information)

Payments.UniFi: Transaction lookup response: {"inquiryType":"BUY","creditAuthorizationInfo":{"customerInfo":{"cipher.firstName":"TEST","cipher.lastName":"WAREHOUSE","address":{"cipher.addressLine1":"950 FORRER BLVD","cipher.city":"DAYTON","cipher.state":"OH","cipher.zipCode":"454201469"}},"accountInfo":{"cipher.accountNumber":"6019170127801071"},"transactionCode":"000","transactionMessage":"Auth Approved","transactionInfo":{"amount":"919.07","dateTime":"Thu Oct 12 14:14:37 UTC 2023","description":"AUTHORIZATION","authorizationCode":"012716","promoCode":"106","promoType":"REGULAR"},"merchantInfo":{"merchantNumber":"5348121490000021","verificationId":"a8622e6d-b7eb-4417-8cd4-c1e4465924d6","correlationId":"PI200014186978118b243d2293"}}}