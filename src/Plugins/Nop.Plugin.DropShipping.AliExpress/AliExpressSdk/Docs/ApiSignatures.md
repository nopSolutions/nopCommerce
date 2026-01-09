## Signature algorithm

Aliexpress Open Platform verifies the identity of each API request, and the server will also verify whether the call
parameters are valid. Therefore, each HTTP request must contain the signature information. The requests with invalid
signature will be rejected.

Aliexpress Open Platform verifies the identity of the requests by the App Key and Secret that are assigned to your
application. The App Secret is used to generate the signature string in the HTTP request URL and server-side signature
string. It must be kept strictly confidential.

If you compose HTTP request manually (instead of using the official SDK), you need to understand the following signature
algorithm.

The process of generating the signature is as follows:(Notice:The only difference between System Interfaces and Business
Interfaces is how to handle api_path.)

### 1.Sort all request parameters (including system and application parameters, but except the “sign” and parameters with byte array type.If the API you used is Business Interface, api_path should be used as the request parameter to participate in sorting) according to the parameter name in ASCII table. For example:

```text
// Example of api_path key value pair
// method : aliexpress.offer.product.post

Before sort: foo=1, bar=2, foo_bar=3, foobar=4
After sort: bar=2, foo=1, foo_bar=3, foobar=

```

### 2.Concatenate the sorted parameters and their values into a string. For example:

```text
bar2foo1foo_bar3foobar4
```

### 3.(If the API you used is System Interface, Please add the API name in front of the concatenated string. For example, adding the API name "/test/api")

```text
/test/apibar2foo1foo_bar3foobar4
```

### 4.Encode the concatenated string in UTF-8 format and make a digest by the signature algorithm (using HMAC_SHA256). For example:

```text
hmac_sha256(/test/apibar2foo1foo_bar3foobar4)
```

### 5.Convert the digest to hexadecimal format. For example:

```text
hex("helloworld".getBytes("utf-8")) = "68656C6C6F776F726C64"
```

```csharp

public static final String CHARSET_UTF8 = "UTF-8";
public static final String SIGN_METHOD_SHA256 = "sha256";
public static final String SIGN_METHOD_HMAC_SHA256 = "HmacSHA256";

public static String signApiRequest(Map<String, String> params, String appSecret, String signMethod, String apiName) throws IOException {
    // If you are using Business Interface, please do as step 1,add api_path into params.
    // params.put("method",apiName);
    
    // sort all text parameters
    String[] keys = params.keySet().toArray(new String[0]);
    Arrays.sort(keys);
    
    // connect all text parameters with key and value
    StringBuilder query = new StringBuilder();
    // If you are using System Interface, please do as step 3
    // append API name
    query.append(apiName);
    for (String key : keys) {
        String value = params.get(key);
        if (areNotEmpty(key, value)) {
            query.append(key).append(value);
        }
    }
    
    // sign the whole request
    byte[] bytes = null;
    
    
    if (signMethod.equals(SIGN_METHOD_SHA256)) {
        bytes = encryptHMACSHA256(query.toString(), appSecret);
    }
    
    // finally : transfer sign result from binary to upper hex string
    return byte2hex(bytes);
}


private static byte[] encryptHMACSHA256(String data, String secret) throws IOException {
    byte[] bytes = null;
    try {
        SecretKey secretKey = new SecretKeySpec(secret.getBytes(CHARSET_UTF8), SIGN_METHOD_HMAC_SHA256);
        Mac mac = Mac.getInstance(secretKey.getAlgorithm());
        mac.init(secretKey);
        bytes = mac.doFinal(data.getBytes(CHARSET_UTF8));
    } catch (GeneralSecurityException gse) {
        throw new IOException(gse.toString());
    }
    return bytes;
}



/**
* Transfer binary array to HEX string.
  */
  public static String byte2hex(byte[] bytes) {
  StringBuilder sign = new StringBuilder();
  for (int i = 0; i < bytes.length; i++) {
  String hex = Integer.toHexString(bytes[i] & 0xFF);
  if (hex.length() == 1) {
  sign.append("0");
  }
  sign.append(hex.toUpperCase());
  }
  return
```
