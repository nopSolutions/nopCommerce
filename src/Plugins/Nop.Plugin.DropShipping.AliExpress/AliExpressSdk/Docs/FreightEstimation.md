# Feature: Shipping and Freight Estimation APIs

## GET/POST:aliexpress.ds.freight.query

| Name              | Type    | Required or not | Description                              |
|-------------------|---------|-----------------|------------------------------------------|
| ◉queryDeliveryReq | 	Object | Yes             | Delivery query request                   |
| quantity          | 	Number | Yes             | Quantity for your request                |
| shipToCountry     | 	String | Yes             | Country that ships to                    |
| productId         | 	String | Yes             | Product_id(From aliexpres.product.get)   |
| provinceCode      | 	String | No              | Provice                                  |
| cityCode          | 	String | No              | City                                     |
| language          | 	String | Yes             | Language                                 |
| locale            | 	String | Yes             | `Locale                                  |
| selectedSkuId     | 	String | Yes             | Selected sku(From aliexpres.product.get) |
| currency          | 	String | Yes             | Currency for calculate the freight fee   |

### Example 

```php
<?php
$url = "https://api-sg.aliexpress.com/sync";
//The request address for system-level APIs (such as php) is different, and you need to use the URL below.
$appkey = "Your-appkey";
$appSecret = "Your-appSecret";
$accessToken = "Your-accessToken";

$c = new IopClient(url,appkey,appSecret);
$request = new IopRequest('aliexpress.ds.freight.query');
$request->addApiParam('queryDeliveryReq','{\"quantity\":\"1\",\"shipToCountry\":\"US\",\"productId\":\"3256802900954148\",\"provinceCode\":\"California\",\"cityCode\":\"Mill Valley\",\"selectedSkuId\":\"12000023999200390\",\"language\":\"en_US\",\"currency\":\"USD\",\"locale\":\"zh_CN\"}');
var_dump($c->execute($request, $accessToken));
```

> **Common errors**
Q:How to determine whether the product is free shipping
A:If 'free_shipping' is 'true', shipping is free. If 'free_shipping' is 'false', check 'mayHavePFS'. If 'mayHavePFS' is 'true' and 'free_shipping_threshold' is 0, shipping is free. If 'mayHavePFS' is 'true' and 'free_shipping_threshold' is not 0, shipping is free when the order total meets or exceeds 'free_shipping_threshold'. Otherwise, shipping is not free. 


```json
{
  "aliexpress_ds_freight_query_response": {
    "result": {
      "msg": "Call succeeds",
      "code": 200,
      "success": true,
      "delivery_options": {
        "delivery_option_d_t_o": [
          {
            "code": "CAINIAO_STANDARD",
            "shipping_fee_currency": "USD",
            "free_shipping": false,
            "mayHavePFS": false,
            "guaranteed_delivery_days": "75",
            "max_delivery_days": 34,
            "tracking": true,
            "shipping_fee_format": "US $172.71",
            "delivery_date_desc": "Mar 08 - 25",
            "company": "AliExpress standard shipping",
            "ship_from_country": "CN",
            "min_delivery_days": 17,
            "available_stock": "182",
            "ddpIncludeVATTax": "true",
            "shipping_fee_cent": "172.71"
          }
        ]
      }
    },
    "request_id": "2140d58917400193503907772"
  }
}
```



