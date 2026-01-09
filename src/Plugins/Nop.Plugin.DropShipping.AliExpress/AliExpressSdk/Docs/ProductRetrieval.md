# Feature: Product Retrieval

## aliexpress.ds.text.search | Search Products by Text

example

```csharp

IIopClient client = new IopClient(url, appkey, appSecret);
IopRequest request = new IopRequest();
request.SetApiName("aliexpress.ds.text.search");
request.AddApiParameter("keyWord", "\u88D9\u5B50");
request.AddApiParameter("local", "zh_CN");
request.AddApiParameter("countryCode", "US");
request.AddApiParameter("categoryId", "349");
request.AddApiParameter("sortBy", "min_price,asc");
request.AddApiParameter("pageSize", "20");
request.AddApiParameter("pageIndex", "1");
request.AddApiParameter("currency", "USD");
request.AddApiParameter("searchExtend", "[{\"min\":\"min\",\"max\":\"max\",\"searchKey\":\"searchKey\",\"searchValue\":\"searchValue\"}]");
request.AddApiParameter("selectionName", "selection name");
IopResponse response = client.Execute(request);
Console.WriteLine(response.IsError());
Console.WriteLine(response.Body);

```

| Name                     | Type     | Required or not | Description                                                                                  |
|--------------------------|----------|-----------------|----------------------------------------------------------------------------------------------|
| keyWord                  | String   | No              | query word                                                                                   |
| local                    | String   | Yes             | lang: en_US                                                                                  |
| countryCode              | String   | Yes             | ship to country                                                                              |
| categoryId               | Number   | No              | categoryId                                                                                   |
| sortBy                   | String   | No              | accept value: min_price,asc min_price,desc orders,asc orders,desc comments,asc comments,desc |
| pageSize                 | Number   | No              | page size                                                                                    |
| pageIndex                | Number   | No              | page index                                                                                   |
| currency                 | String   | Yes             | currency                                                                                     |
| searchExtend             | Object[] | No              | search extend                                                                                |
| searchExtend.searchKey   | String   | No              | search key                                                                                   |
| searchExtend.searchValue | String   | No              | search value                                                                                 |
| searchExtend.max         | String   | No              | max                                                                                          |
| searchExtend.min         | String   | No              | min                                                                                          |
| selectionName            | String   | No              | text search within specific selection                                                        |

```json
{
  "msg": "message",
  "code": "0",
  "data": {
    "pageIndex": "pageIndex",
    "pageSize": "pageSize",
    "totalCount": "totalCount",
    "products": [
      {
        "productVideoUrl": "productVideoUrl",
        "originalPrice": "12.73",
        "originalPriceCurrency": "USD",
        "salePrice": "6.62",
        "discount": "48%",
        "itemMainPic": "itemMainPic",
        "title": "title",
        "type": "search",
        "originalPriceFormat": "originalPriceFormat",
        "score": "score",
        "itemId": "itemId",
        "targetSalePrice": "6.62",
        "cateId": "cateId",
        "targetOriginalPriceCurrency": "USD",
        "originMinPrice": "originMinPrice",
        "evaluateRate": "97.8%",
        "salePriceFormat": "salePriceFormat",
        "orders": "orders",
        "targetOriginalPrice": "12.73",
        "itemUrl": "itemUrl",
        "salePriceCurrency": "USD"
      }
    ]
  },
  "request_id": "0ba2887315178178017221014"
}
```

## aliexpress.ds.product.get

example

```csharp

IIopClient client = new IopClient(url, appkey, appSecret);
IopRequest request = new IopRequest();
request.SetApiName("aliexpress.ds.product.get");
request.AddApiParameter("ship_to_country", "US");
request.AddApiParameter("product_id", "1005003784285827");
request.AddApiParameter("target_currency", "USD");
request.AddApiParameter("target_language", "en");
request.AddApiParameter("remove_personal_benefit", "false");
request.AddApiParameter("biz_model", "biz_model");
request.AddApiParameter("province_code", "provice");
request.AddApiParameter("city_code", "city");
IopResponse response = client.Execute(request, accessToken);
Console.WriteLine(response.IsError());
Console.WriteLine(response.Body);
```

| Name                    | 	Type   | 	Required or not | 	Description                                                                                        |
|-------------------------|---------|------------------|-----------------------------------------------------------------------------------------------------|
| ship_to_country         | String  | 	Yes             | Country                                                                                             |                                                                                            
| product_id              | Number  | 	Yes             | Item ID                                                                                             |                                                                           
| target_currency         | String  | 	No              | Target currency                                                                                     |                                                                                     
| target_language         | String  | 	No              | hi de ru pt ko in en it fr zh es iw ar vi th uk ja id pl he nl tr (lowercase) or "en_US"、"ko_KR"... | 
| remove_personal_benefit | Boolean | 	No              | if true, you will not get any crowd type promotion                                                  |
| biz_model               | String  | 	No              | BETA version for open api business model                                                            |
| province_code           | String  | 	No              | province                                                                                            |
| city_code               | String  | 	No              | city                                                                                                |

```json
{
  "result": {
    "ae_item_sku_info_dtos": [
      {
        "tax_amount": "652",
        "sku_attr": "73:175#Black Green;71:193#Polarized",
        "offer_sale_price": "3.94",
        "estimated_import_charges": "24.85",
        "sku_id": "12000027158136202",
        "wholesale_price_tiers": [
          {
            "min_quantity": "1",
            "wholesale_price": "\"5.78\"",
            "discount": "0.52"
          }
        ],
        "ean_code": "eanCode",
        "limit_strategy": "create_order_fail",
        "price_include_tax": "true",
        "currency_code": "USD",
        "sku_price": "3.94",
        "buy_amount_limit_set_by_promotion": "3",
        "offer_bulk_sale_price": "3.74",
        "sku_available_stock": "57",
        "id": "73:175#Black Green;71:193#Polarized",
        "sku_bulk_order": "10",
        "tax_currency_code": "USD",
        "barcode": "320325455",
        "ae_sku_property_dtos": [
          {
            "sku_property_value": "green",
            "sku_image": "https://ae04.alicdn.com/kf/Hba46f8222fdf4440a271134f8ce2ca2aB.jpg",
            "sku_property_name": "Lenses Color",
            "property_value_definition_name": "Black Green",
            "property_value_id": "175",
            "sku_property_id": "73"
          }
        ]
      }
    ],
    "ae_multimedia_info_dto": {
      "ae_video_dtos": [
        {
          "media_status": "approved",
          "media_type": "MAIN_IMAGE_VIDEO",
          "poster_url": "https://img.alicdn.com/imgextra/i4/6000000004252/O1CN01R1TCD21hHSjEOLpKT_!!6000000004252-0-tbvideo.jpg",
          "ali_member_id": "227887939",
          "media_id": "296195919792",
          "media_url": "media url"
        }
      ],
      "image_urls": "https://ae04.alicdn.com/kf/H551494696f2342edbeee5d2f288f4d1cv.jpg;https://ae04.alicdn.com/kf/H847c9c1b04614adfa20f042cc4490df6S.jpg;https://ae04.alicdn.com/kf/H1936d341e4bf4773ace153b527070a07C.jpg;https://ae04.alicdn.com/kf/Hf0cc14d4cc5741dc91b6dcf90bf5553cu.jpg;https://ae04.alicdn.com/kf/H80cb5bc712984795a850e9fc3a156d9fV.jpg;https://ae04.alicdn.com/kf/H71d81f9898af4d72b3bd18f7fffeb557j.jpg"
    },
    "package_info_dto": {
      "package_width": "8",
      "package_height": "6",
      "package_length": "18",
      "gross_weight": "0.050",
      "package_type": "false",
      "product_unit": "100000015",
      "base_unit": "2"
    },
    "logistics_info_dto": {
      "delivery_time": "5",
      "ship_to_country": "CA"
    },
    "product_id_converter_result": {
      "main_product_id": "32982857990",
      "sub_product_id": {
        "US": 2251832796543238
      }
    },
    "ae_item_base_info_dto": {
      "gmt_create": "0",
      "mobile_detail": "\u003cdiv\u003eThis is a product\u003c/div\u003e",
      "subject": "FUQIAN 2022 Fashion Square Polarized Sunglasses Men Vintage Plastic Male Sun Glasses Women Stylish Black Sport Shades UV400",
      "category_sequence": "(Beta Release) Only open to whitelist appkey",
      "evaluation_count": "1004",
      "sales_count": "1000+",
      "product_status_type": "onSelling",
      "avg_evaluation_rating": "4.8",
      "gmt_modified": "0",
      "separated_listing": "false",
      "currency_code": "CNY",
      "owner_member_seq_long": "227887939",
      "category_id": "33902",
      "product_id": "4000903675543",
      "detail": "\u003cdiv\u003eThis is a product\u003c/div\u003e"
    },
    "manufacturer_info": {
      "address": "address",
      "phone": "phone",
      "name": "name",
      "country_name": "China",
      "email": "email",
      "phone_prefix": "86"
    },
    "has_whole_sale": "true",
    "ae_item_properties": [
      {
        "attr_name_id": "2",
        "attr_value_id": "110567917",
        "attr_value_unit": "piece",
        "attr_name": "Brand Name",
        "attr_value_start": "1",
        "attr_value_end": "10",
        "attr_value": "FUQIAN"
      }
    ],
    "ae_store_info": {
      "store_id": "4874072",
      "shipping_speed_rating": "4.7",
      "communication_rating": "4.7",
      "store_name": "FUQIAN Eyewear Store",
      "store_country_code": "CN",
      "item_as_described_rating": "4.7"
    }
  },
  "code": "0",
  "rsp_code": "200",
  "rsp_msg": "Call succeeds",
  "request_id": "0ba2887315178178017221014"
}
```


