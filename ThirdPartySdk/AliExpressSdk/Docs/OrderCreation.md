# Feature: Order Creation APIs

## GET/POST：aliexpress.ds.order.create

| Request Parameter                           | Name                     | Type         | Required | Description                                                                                          |
|---------------------------------------------|--------------------------|--------------|----------|------------------------------------------------------------------------------------------------------|
| ◉ ds_extend_request                         |                          | Object       | No       | DS ExtendParam                                                                                       |
|                                             | ◎ promotion              | List[Object] | No       | PromotionCode                                                                                        |
|                                             | promotion_code           | String       | No       | PromotionCode                                                                                        |
|                                             | promotion_channel_info   | String       | Yes      | PromotionChannelInfo                                                                                 |
|                                             | ◎ payment                | List[Object] | No       | Payment                                                                                              |
|                                             | pay_currency             | String       | No       | Used to pass in the specified currency. Example: `{"pay_currency":"XXX"}`                            |
|                                             | try_to_pay               | String       | No       | Please authorize your account as DS before setting it to true (`true` or `false`)                    |
|                                             | ◎ trade_extra_param      | List[Object] | No       | Whether it is wholesale                                                                              |
|                                             | business_model           | String       | No       | Choose to place the order in wholesale or retail model                                               |
| ◉ param_place_order_request4_open_api_d_t_o |                          | Object       | Yes      | Specific order parameters                                                                            |
|                                             | out_order_id             | String       | No       | Recommended to be required. Used as an idempotent ID to prevent duplicate orders. Valid for 24 hours |
|                                             | ◎ logistics_address      | List[Object] | Yes      | Logistic address information                                                                         |
|                                             | address                  | String       | Yes      | Address information                                                                                  |
|                                             | address2                 | String       | No       | Address extension information                                                                        |
|                                             | city                     | String       | Yes      | City                                                                                                 |
|                                             | contact_person           | String       | No       | Contact person (Required for CPF validation in Brazil)                                               |
|                                             | country                  | String       | Yes      | Receiver country (two-letter code)                                                                   |
|                                             | cpf                      | String       | No       | Taxpayer ID (Required if country is BR)                                                              |
|                                             | full_name                | String       | No       | Receiver full name                                                                                   |
|                                             | locale                   | String       | No       | Internationalization locale                                                                          |
|                                             | mobile_no                | String       | No       | Mobile phone number                                                                                  |
|                                             | passport_no              | String       | No       | Passport number (Required for MX)                                                                    |
|                                             | passport_no_date         | String       | No       | Passport expiry date                                                                                 |
|                                             | passport_organization    | String       | No       | Passport issuing agency                                                                              |
|                                             | phone_country            | String       | No       | Phone country code                                                                                   |
|                                             | province                 | String       | Yes      | Province                                                                                             |
|                                             | tax_number               | String       | No       | TAX number                                                                                           |
|                                             | zip                      | String       | No       | ZIP code                                                                                             |
|                                             | rut_no                   | String       | No       | Chile tax number (not used)                                                                          |
|                                             | foreigner_passport_no    | String       | No       | Foreign tax number (Required for Korean foreigners)                                                  |
|                                             | is_foreigner             | String       | No       | Whether the receiver is a foreigner                                                                  |
|                                             | vat_no                   | String       | No       | VAT tax number                                                                                       |
|                                             | tax_company              | String       | No       | Company name                                                                                         |
|                                             | location_tree_address_id | String       | No       | Location tree address ID                                                                             |
|                                             | ◎ product_items          | List[Object] | Yes      | Product attributes                                                                                   |
|                                             | product_count            | Number       | Yes      | Product count                                                                                        |
|                                             | product_id               | Number       | Yes      | Product ID                                                                                           |
|                                             | sku_attr                 | String       | No       | Product SKU (From `aliexpress.ds.product.get`; empty if only one SKU exists)                         |
|                                             | logistics_service_name   | String       | No       | Logistics service name (From `aliexpress.ds.freight.query`)                                          |
|                                             | order_memo               | String       | No       | User comments                                                                                        |


## Examples

```java
String url = "https://api-sg.aliexpress.com";
String appkey = "your_appkey";
String appSecret = "your_appSecret";

IopClient client = new IopClient(url, appkey, appSecret);
IopRequest request = new IopRequest();

request.setApiName("aliexpress.ds.order.create");
request.addApiParameter('ds_extend_request','{"payment":{"pay_currency":"USD"},"promotion":{"promotion_code":""}}');
request.addApiParameter(
    'param_place_order_request4_open_api_d_t_o','
{
    "out_order_id": "",
    "logistics_address": {
        "address": "WWEPerformance Center,",
        "address2": "STE 100",
        "birthday": "",
        "city": "Orlando",
        "contact_person": "person name",
        "country": "US",
        "cpf": "",
        "fax_area": "",
        "fax_country": "",
        "fax_number": "",
        "full_name": "full_name",
        "locale": "local",
        "mobile_no": "11231231231",
        "passport_no": "",
        "passport_no_date": "",
        "passport_organization": "",
        "phone_area": "",
        "phone_country": "+",
        "phone_number": "11231231231",
        "province": "Florida",
        "tax_number": "",
        "zip": "32807",
        "rut_no": "",
        "foreigner_passport_no": "",
        "is_foreigner": false,
        "vat_no": "",
        "tax_company": "",
        "location_tree_address_id": ""
    },
    "product_items": [
        {
            "product_id": 1005002704149141,
            "product_count": 1,
            "sku_attr": "14:175;5:200003528#Length -26cm",
            "logistics_service_name": "CAINIAO_FULFILLMENT_STD"
        }
    ]
}
);

IopResponse response = client.execute(request, accessToken, Protocol.TOP);

System.out.println(response.getBody());
```



```json
{
  "aliexpress_ds_order_create_response": {
    "result": {
      "is_success": true,
      "order_list": {
        "number": [
          8198352049851315
        ]
      }
    },
    "request_id": "212a790f17395203524904964"
  }
}
```


## Multiple Products at once

```json
{
  "logistics_address": {
    "address": "Enter_your_address",
    "city": "Vineland",
    "contact_person": "Malik Jack",
    "country": "US",
    "mobile_no": "23450000",
    "phone_country": "+",
    "phone_number": "5593600000",
    "province": "New Jersey",
    "zip": "08360"
  },
  "product_items": [
    {
      "product_id": 1005004638293060,
      "product_count": 1,
      "sku_attr": "14:365458#Brown",
      "logistics_service_name": "",
      "order_memo": " "
    },
    {
      "product_id": 1005004638293060,
      "product_count": 1,
      "sku_attr": "14:365458#Brown",
      "logistics_service_name": "",
      "order_memo": " "
    }
  ]
}
```