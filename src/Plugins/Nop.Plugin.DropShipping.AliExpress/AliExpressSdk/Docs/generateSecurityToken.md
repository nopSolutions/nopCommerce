

Start here

https://openservice.aliexpress.com/doc/doc.htm?spm=a2o9m.11193531.0.0.7dc63b53muBffH&nodeId=27493&docId=118729#/?docId=1370

[Endpoints](https://openservice.aliexpress.com/doc/doc.htm?spm=a2o9m.11193531.0.0.74453b53sJ0M9s&nodeId=27493&docId=118729#/?docId=1369)

[Parameters](https://openservice.aliexpress.com/doc/doc.htm?spm=a2o9m.11193531.0.0.74453b53sJ0M9s&nodeId=27493&docId=118729#/?docId=1368)

[Signature Algorithm](https://openservice.aliexpress.com/doc/doc.htm?spm=a2o9m.11193531.0.0.74453b53sJ0M9s&nodeId=27493&docId=118729#/?docId=1367)


Aliexpress Open Platform provides an online production environment. The data under the production environment are all true online data, providing limited times and authority of interface calling. The production environment shares data with the online system, and the true data of an online shop are directly influenced by the interface for writing class, so you must operate with caution.
Currently, all interfaces are divided into two categories: System interfaces and Business interfaces. And you need to choose the correct endponint to use for each type API.
System interfaces: Authorization relative APIs under 'System Tool' in the API documentation.
Business interfaces: All other APIs except system APIs which mentioned above.
1.For those business APIs: https://api-sg.aliexpress.com/sync?method={api_path}&{query}
2.For those system APIs: https://api-sg.aliexpress.com/rest{api_path}?{query}




```http request

## declare variables
@authBaseUrl = https://api-sg.aliexpress.com/rest
@tokenApiPath = /auth/token/security/create

### Generate Security Token
POST {{authBaseUrl}}/{{tokenApiPath}}


```


## System Interfaces

Taking /auth/token/create(generate access_token) API call as example, the steps of assembling the HTTP request is as follows:

### Step 1. Populate parameters and values

Common parameters:(access_token is not needed in this API)

app_key = “12345678”
timestamp = “1517820392000”
sign_method = “sha256”
Business parameters:

code = “3_500102_JxZ05Ux3cnnSSUm6dCxYg6Q26”

### Step 2. Sort all parameters and values according to the parameter name in ASCII table

app_key = “12345678”
code = “3_500102_JxZ05Ux3cnnSSUm6dCxYg6Q26”
sign_method = “sha256”
timestamp = “1517820392000”

### Step 3. Concatenate the sorted parameters and their values into a string

app_key12345678code3_500102_JxZ05Ux3cnnSSUm6dCxYg6Q26sign_methodsha256timestamp1517820392000
Step 4. Add the API name in front of the concatenated string

/auth/token/createapp_key12345678code3_500102_JxZ05Ux3cnnSSUm6dCxYg6Q26sign_methodsha256timestamp1517820392000

### Step 5. Generate signature

Assuming that the App Secret is “helloworld”, the signature is:

hex(sha256(/auth/token/createapp_key12345678code3_500102_JxZ05Ux3cnnSSUm6dCxYg6Q26sign_methodsha256timestamp1517820392000))=35607762342831B6A417A0DED84B79C05FEFBF116969C48AD6DC00279A9F4D81

### Step 6. Assemble HTTP request

Encode all the parameters and values (with the “sign” parameter) using UTF-8 format (the order of parameters can be arbitrary).The splicing method is:https://api-sg.aliexpress.com/rest{api_path}?{query}.

https://api-sg.aliexpress.com/rest/auth/token/create?app_key=12345678&code=3_500102_JxZ05Ux3cnnSSUm6dCxYg6Q26&timestamp=1517820392000&sign_method=sha256&sign=35607762342831B6A417A0DED84B79C05FEFBF116969C48AD6DC00279A9F4D81


### Step 7 java example

public static void main(String[] args) throws Exception {
final String SIGN_METHOD = "sha256";
final String APP_KEY = "YOUR_APP_KEY";
final String APP_SECRET = "YOUR_APP_SECRET";
final String CODE = "YOUR_CODE";
final String API_NAME = "/auth/token/create";


long time = System.currentTimeMillis();
String timeStamp = Long.toString(time);


Map<String, String> params = new HashMap<String, String>();
params.put("app_key", APP_KEY);
params.put("timestamp", timeStamp);
params.put("sign_method", "sha256");
params.put("CODE", CODE);
params.put("simplify", "true");
String requestBody = null;


String sign = IopUtils.signApiRequest(API_NAME,params, requestBody, APP_SECRET, SIGN_METHOD);
params.put("sign",sign);



String url = "https://api-sg.aliexpress.com/rest" + API_NAME + "?";
for (String key : params.keySet()) {
url += "&" + key + "=" + URLEncoder.encode(params.get(key), "utf-8");
}
System.out.println(url);
String sendGet = HttpUtils.sendGet(url);
System.out.println("sendGet = " + s


Notes:

All request and response data codes are in UTF-8 format.Please encode all parameter names and parameter values in the URL. And all parameter values in the HTTP body should also be encoded If the requested content type is application / x-www-form-urlencoded.If it is in multipart / form data format, the parameter value of each form field does not need to be encoded, but the charset part of each form field needs to be specified as UTF-8.
When the length of the URL assembled by the parameter name and parameter value is less than 1024 characters, you can use method "GET" to initiate the request; If the parameter type contains byte [] type or the assembled request URL is too long, the request must be initiated by method "POST". All APIs can initiate requests using "POST".
Only those who do not use the official AE SDK need to generate the signature for API calls.If the official AE SDK is used, the SDK will complete this step automatically.
