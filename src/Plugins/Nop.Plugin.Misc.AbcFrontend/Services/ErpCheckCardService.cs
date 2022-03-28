using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using Nop.Plugin.Misc.AbcFrontend.Models;
using Nop.Services.Payments;

namespace Nop.Plugin.Misc.AbcFrontend.Services
{
    public class ErpCheckCardService : IErpCheckCardService
    {
        public ErpCheckCardResponse CheckCreditCard(ProcessPaymentRequest processPaymentRequest)
        {
            return new ErpCheckCardResponse();
        }
    }
}

// the following CURL can be used for testing:
// curl --request POST \
//   --url http://160.160.160.20:63000/ \
//   --header 'Content-Length: 414' \
//   --header 'Content-Type: application/xml' \
//   --data '<Request>
// <Card_Check>
// <Card_Number>372739582391234</Card_Number>
// <Exp_Month>9</Exp_Month>
// <Exp_Year>2025</Exp_Year>
// <Cvv2>1234</Cvv2>
// <Bill_First_Name>Dave</Bill_First_Name>
// <Bill_Last_Name>Farinelli</Bill_Last_Name>
// <Bill_Address>2258 Holton Ln</Bill_Address>
// <Bill_Zip>48323</Bill_Zip>
// <Company>https://stage.abcwarehouse.com/</Company>
// <email>dfarinelli@xby2.com</email>
// </Card_Check>
// </Request>'

// for home delivery check:
// curl --request POST \
//   --url http://160.160.160.20:63000/ \
//   --header 'Content-Length: 123' \
//   --header 'Content-Type: text/xml; charset=utf-8' \
//   --data '<Request>
//   <Delivery_Lookup>
//     <INVOICE>1234</INVOICE>
//     <ZIPCODE>48323</ZIPCODE>
//   </Delivery_Lookup>
// </Request>'