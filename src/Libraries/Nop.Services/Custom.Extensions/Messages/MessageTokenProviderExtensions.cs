using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Vendors;

namespace Nop.Services.Messages
{

    public partial interface IMessageTokenProvider
    {
        Task AddCustomProductTokensAsync(IList<Token> tokens, Product product, Customer customer, int languageId, IList<SpecificationAttributeOption> specOptions);
    }

    public partial class MessageTokenProvider
    {
        #region Methods

        public virtual async Task AddCustomProductTokensAsync(IList<Token> tokens, Product product, Customer customer, int languageId, IList<SpecificationAttributeOption> specOptions)
        {
            tokens.Add(new Token("Product.ID", product.Id));
            tokens.Add(new Token("Product.Name", await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId)));
            tokens.Add(new Token("Product.ShortDescription", await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription, languageId), true));
            tokens.Add(new Token("Product.SKU", product.Sku));
            tokens.Add(new Token("Product.StockQuantity", await _productService.GetTotalStockQuantityAsync(product)));

            tokens.Add(new Token("Order.Product(s)", await TotalEmailTemplate(), true));

            var productUrl = await CustomRouteUrlAsync(routeName: "Product", routeValues: new { SeName = await _urlRecordService.GetSeNameAsync(product) });
            tokens.Add(new Token("Product.ProductURLForCustomer", productUrl, true));

            //add customer skillset
            var skills = string.Join(", ", specOptions.Select(x => x.Name).ToArray());
            //tokens.Add(new Token("Customer.RegisteredCustomerSkillSet", skills));

            tokens.Add(new Token("Customer.FullName", customer.FirstName + " " + customer.LastName));

            await _eventPublisher.EntityTokensAddedAsync(product, tokens);
        }

        #endregion

        #region Utilities

        protected virtual async Task<string> CustomProductListToHtmlTableAsync(Product product, int languageId)
        {
            var language = await _languageService.GetLanguageByIdAsync(languageId);

            var sb = new StringBuilder();
            sb.AppendLine("<table border=\"0\" style=\"width:100%;\">");

            sb.AppendLine($"<tr style=\"background-color:{_templatesSettings.Color1};text-align:center;\">");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Name", languageId)}</th>");
            sb.AppendLine("</tr>");

            if (product == null)
                return string.Empty;

            sb.AppendLine($"<tr style=\"background-color: {_templatesSettings.Color2};text-align: center;\">");

            //product name
            var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId);
            sb.AppendLine("<td style=\"padding: 0.6em 0.4em;text-align: left;\">" + WebUtility.HtmlEncode(productName));

            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("</table>");
            var result = sb.ToString();
            return result;
        }

        protected virtual async Task<string> TotalEmailTemplate()
        {

            var sb = new StringBuilder();

            //start opening tags
            sb.AppendLine(@"<table id=""emailtemplate"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" align=""center"">");

            //first tr
            sb.AppendLine(MailTemplateBeforeHeaderHtml());

            //second tr opening tags
            sb.AppendLine(@"
                    <tr>
                        <td>
                            <table style=""border:solid 1px #dddddd;max-width:580px"" bgcolor=""ffffff"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                             ");

            //second tr start 
            sb.AppendLine(MailTemplateHeaderHtml());

            sb.AppendLine(await MailTemplateBodyAsync(new Product()));

            sb.AppendLine(MailTemplateFooterHtml());

            //second tr end 

            //second tr closing tags
            sb.AppendLine(@"
                            </table>
                        </td>
                    </tr>");

            // thrid tr
            sb.AppendLine(MailTemplateAfterFooterDisclaimerHtml());

            //end closing tags
            sb.AppendLine(@"</table>");

            var result = sb.ToString();
            return result;
        }

        protected virtual async Task<string> CustomProductListToHtmlTableAsync(IList<Product> products, int languageId, int vendorId)
        {
            var language = await _languageService.GetLanguageByIdAsync(languageId);

            var sb = new StringBuilder();
            sb.AppendLine("<table border=\"0\" style=\"width:100%;\">");

            sb.AppendLine($"<tr style=\"background-color:{_templatesSettings.Color1};text-align:center;\">");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Name", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Price", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Quantity", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Total", languageId)}</th>");
            sb.AppendLine("</tr>");

            for (var i = 0; i <= products.Count - 1; i++)
            {
                var orderItem = products[i];

                var product = await _productService.GetProductByIdAsync(orderItem.Id);

                if (product == null)
                    continue;

                sb.AppendLine($"<tr style=\"background-color: {_templatesSettings.Color2};text-align: center;\">");
                //product name
                var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId);

                sb.AppendLine("<td style=\"padding: 0.6em 0.4em;text-align: left;\">" + WebUtility.HtmlEncode(productName));
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            var result = sb.ToString();
            return result;
        }

        protected virtual string MailTemplateBeforeHeaderHtml()
        {
            var sb = new StringBuilder();

            var strHeader = @"
                                <tr>
                                    <td valign=""top"">
                                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" align=""center"">
                                            <tbody>
                                                <tr>
                                                    <td align=""center"" valign=""top"">
                                                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""600"" align=""left"">
                                                            <tbody>
                                                                <tr>
                                                                    <td style=""padding-left:4px;padding-top:5px;padding-bottom:0"">
                                                                        <font style=""font-family:Arial,MS Sans serif,Arial,Verdana,Helvetica;font-size:9px;font-style:normal;text-align:justify;text-transform:none;color:rgb(166,166,166)"">
                                                                            You are receiving this mail as a registered member of onjobsupport.in Please add
                                                                            <a href=""mailto:no-reply@onjobsupport.in"" target=""_blank"">no-reply@onjobsupport.in</a>
                                                                            to your address book to ensure delivery to your inbox.
                                                                        </font>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>";

            sb.AppendLine(strHeader);

            var result = sb.ToString();
            return result;
        }

        protected virtual string MailTemplateHeaderLogoHtml()
        {
            var sb = new StringBuilder();

            var strHeaderLogo = $@"
                                    <tr>
                                        <td valign=""top"">
                                            <table align=""left"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                                                <tbody>
                                                    <tr>
                                                        <td height=""15""></td>
                                                    </tr>
                                                    <tr>
                                                        <td height=""25"">
                                                            <img src=""http://imgs.padmasalimatrimony.com/cbsimages/trans.gif"" alt="""" border=""0"" height=""1"" width=""20"" class=""CToWUd"">
                                                        </td>
                                                        <td valign=""bottom"">
                                                            <a href=""https://onjobsupport.in"" target=""_blank"">
                                                               <img src=""https://onjobsupport.in/images/thumbs/0000041_Logo_Final.png"" id="" alt="""" title="""" width=""200"" height=""20"" border=""0"">
                                                            </a>
                                                        </td>
                                                        <td>
                                                            <img src=""http://imgs.padmasalimatrimony.com/cbsimages/trans.gif"" alt="""" border=""0"" height=""1"" width=""20"" class=""CToWUd"">
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan=""3"" height=""15""></td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            <table align=""right"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                                                <tbody>
                                                    <tr>
                                                        <td style=""font:normal 1em/28px arial;color:#4f4f4f;padding:10px 15px 0px 10px"" valign=""top"">{DateTime.Now.ToString("MMMM dd, yyyy")}</td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                               ";

            sb.AppendLine(strHeaderLogo);

            var result = sb.ToString();
            return result;
        }

        protected virtual string MailTemplateAfterHeaderSubjectHtml()
        {
            var sb = new StringBuilder();

            var strAfterHeaderText = @"
                                        <tr>
                                            <td bgcolor=""ff6c0d"" valign=""top"">
                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                    <tbody>
                                                        <tr>
                                                            <td height=""5""></td>
                                                        </tr>
                                                        <tr>
                                                            <td style=""width:3%;max-width:20px"">
                                                                <img src=""https://ci6.googleusercontent.com/proxy/58z8S94yrwRpt6mRMtBLAVGXgvUolady4boC_sOZlJWkhzTVQxCNY6bTZ2Oc6qTHctyAzrE8d8Ri4mxuKDo1ABWhL1hAHbsAGA=s0-d-e1-ft#http://imgs.padmasalimatrimony.com/cbsimages/trans.gif"" alt="""" border=""0"" height=""1"" width=""1"" class=""CToWUd"">
                                                            </td>
                                                            <td style=""font:bold 1.3em/34px arial;color:#ffffff"" align=""left"" valign=""top"">
                                                                <span>Get direct reply from premium member(s)</span>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td height=""5""></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
                                    ";

            sb.AppendLine(strAfterHeaderText);

            var result = sb.ToString();
            return result;
        }

        protected virtual string MailTemplateHeaderHtml()
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" align=""center""><tbody>");

            sb.AppendLine(MailTemplateHeaderLogoHtml());
            sb.AppendLine(MailTemplateAfterHeaderSubjectHtml());

            var result = sb.ToString();
            return result;
        }

        protected virtual string MailTemplateAfterFooterDisclaimerHtml()
        {
            var sb = new StringBuilder();

            var strFooter = @"
                                <tr>
                                    <td>
                                        <table style=""max-width:580px"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                                            <tbody>
                                                <tr>
                                                    <td align=""center"" valign=""top"">
                                                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" align=""center"">
                                                            <tbody>
                                                                <tr>
                                                                    <td style=""padding-left:4px;padding-top:3px""><font style=""font-family:Arial,MS Sans serif,Arial,Verdana,Helvetica;font-size:9px;font-style:normal;text-align:justify;text-transform:none;color:rgb(166,166,166)"">You are a onjobsupport.in member. This e-mail comes to you in accordance with onjobsupport.in's Privacy Policy. <a rel=""nofollow"" href=""https://onjobsupport.in/privacy-notice"" style=""color:#336699"" target=""_blank""> <u>Click here</u></a> to unsubscribe. onjobsupport.in is not responsible for content other than its own and makes no warranties or guarantees about the products or services that are advertised.</font></td>
                                                                    <td></td>
                                                                </tr>
                                                                <tr>
                                                                    <td height=""10""></td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>";

            sb.AppendLine(strFooter);

            var result = sb.ToString();
            return result;
        }

        protected virtual string MailTemplateFooterHtml()
        {
            var sb = new StringBuilder();

            sb.AppendLine(MailTemplatePromotionalHtml());
            sb.AppendLine(MailTemplatePromotionalUpgradeHtml());
            sb.AppendLine(MailTemplateSignatureHtml());

            var result = sb.ToString();
            return result;
        }

        protected virtual string MailTemplatePromotionalHtml()
        {
            var sb = new StringBuilder();

            var strPromotional = @"
                           <tr>
                                <td align=""center"" bgcolor=""f2f2f2"" valign=""top"">
                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                        <tbody>
                                            <tr>
                                                <td height=""10""></td>
                                            </tr>
                                            <tr>
                                                <td style=""font:bold 1.4em arial;color:#2f2f2f"" align=""center"" valign=""top"">They do not match your technology preferences? </td>
                                            </tr>
                                            <tr>
                                                <td height=""5""></td>
                                            </tr>
                                            <tr>
                                                <td style=""font:normal 1em arial;color:#2e2e2e"" align=""center"" valign=""top""><a href=""https://onjobsupport.in/customer/info"" style=""color:#1d97ff;text-decoration:none"" target=""_blank"">Edit your technology Preferences </a> here to get better prospects.</td>
                                            </tr>
                                            <tr>
                                                <td height=""10""></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                          ";

            sb.AppendLine(strPromotional);

            var result = sb.ToString();
            return result;
        }

        protected virtual string MailTemplatePromotionalUpgradeHtml()
        {
            var sb = new StringBuilder();

            var strPromotinal = @"
                                    <tr>
                                        <td align=""center"" bgcolor=""ffffff"" valign=""top"">
                                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                <tbody>
                                                    <tr>
                                                        <td height=""10""></td>
                                                    </tr>
                                                    <tr>
                                                        <td align=""center"" valign=""top"">
                                                            <a href=""https://onjobsupport.in/pricing"" target=""_blank"">
                                                                 <img src=""https://ci5.googleusercontent.com/proxy/icb8DrxXChgGZr-3P4RqVy-yFgS0TIpylyi5JPAxtse02-hkowk-vAqNUvn-KQ2zWIxCIFT5CqMu9idrLoGImbu8LRySHCcL6bpZ4iVRQQ=s0-d-e1-ft#http://imgs.padmasalimatrimony.com/cbsimages/upgrad_text.jpg"" style=""display:block"" class=""CToWUd"">
                                                            </a>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td height=""10""></td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                               ";

            sb.AppendLine(strPromotinal);

            var result = sb.ToString();
            return result;
        }

        protected virtual string MailTemplateSignatureHtml()
        {
            var sb = new StringBuilder();

            var strSignature = @"
                        <tr>
                            <td valign=""top"">
                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                    <tbody>
                                        <tr>
                                            <td valign=""top"">
                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                    <tbody>
                                                        <tr>
                                                            <td><img src=""https://ci6.googleusercontent.com/proxy/58z8S94yrwRpt6mRMtBLAVGXgvUolady4boC_sOZlJWkhzTVQxCNY6bTZ2Oc6qTHctyAzrE8d8Ri4mxuKDo1ABWhL1hAHbsAGA=s0-d-e1-ft#http://imgs.padmasalimatrimony.com/cbsimages/trans.gif"" alt="""" border=""0"" height=""1"" width=""8"" class=""CToWUd""></td>
                                                            <td valign=""top"">
                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style=""font:normal .9em/18px arial;color:#606060"" valign=""top"">
                                                                                Together with you in the search for your prospects.
                                                                                <br><span>Team onjobsupport</span>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td height=""10""></td>
                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </td>
                                                            <td><img src=""https://ci6.googleusercontent.com/proxy/58z8S94yrwRpt6mRMtBLAVGXgvUolady4boC_sOZlJWkhzTVQxCNY6bTZ2Oc6qTHctyAzrE8d8Ri4mxuKDo1ABWhL1hAHbsAGA=s0-d-e1-ft#http://imgs.padmasalimatrimony.com/cbsimages/trans.gif"" alt="""" border=""0"" height=""1"" width=""15"" class=""CToWUd""></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                      ";

            sb.AppendLine(strSignature);

            var result = sb.ToString();
            return result;
        }

        protected virtual async Task<string> CustomRouteUrlAsync(int storeId = 0, string routeName = null, object routeValues = null)
        {
            //try to get a store by the passed identifier
            var store = await _storeService.GetStoreByIdAsync(storeId) ?? await _storeContext.GetCurrentStoreAsync()
                ?? throw new Exception("No store could be loaded");

            //ensure that the store URL is specified
            if (string.IsNullOrEmpty(store.Url))
                throw new Exception("URL cannot be null");

            var url = "/" + routeValues.GetType().GetProperty("SeName").GetValue(routeValues, null);

            //compose the result
            return Uri.EscapeUriString(WebUtility.UrlDecode($"{store.Url.TrimEnd('/')}{url}"));
        }

        protected virtual async Task<string> MailTemplateBodyAsync(Product product)
        {
            var sb = new StringBuilder();
            product.ShortDescription = "Looking for Citrix support";
            product.Name = "Lcuky";
            product.FullDescription = "FullDescription FullDescription FullDescription FullDescription";

            var str1 = $@"
                        <tr>
                            <td valign=""top"">
                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                    <tbody>
                                        <tr>
                                            <td style=""width:3%;max-width:20px""> </td>
                                            <td align=""left"" valign=""top"">
                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                    <tbody>
                                                        <tr>
                                                            <td valign=""top"">
                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                                    <tr>
                                                                        <td height=""15""></td>
                                                                    </tr>
                                                                </table>
                                                                <table style=""border:solid 1px #d1d1d1"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td height=""9""></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td valign=""top"">
                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td style=""width:2%;max-width:9px"">
                                                                                                <img src=""https://ci6.googleusercontent.com/proxy/58z8S94yrwRpt6mRMtBLAVGXgvUolady4boC_sOZlJWkhzTVQxCNY6bTZ2Oc6qTHctyAzrE8d8Ri4mxuKDo1ABWhL1hAHbsAGA=s0-d-e1-ft#http://imgs.padmasalimatrimony.com/cbsimages/trans.gif"" alt="""" border=""0"" height=""1"" width=""1"" class=""CToWUd"">
                                                                                            </td>
                                                                                            <td style=""width:156px"" valign=""top"">
                                                                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                                                                    <tbody>
                                                                                                        <tr>
                                                                                                            <td align=""center"" valign=""middle"">
                                                                                                                <a href=""https://onjobsupport.in/roja-venigalla"" target=""_blank"">
                                                                                                                    <img src=""https://onjobsupport.in/images/thumbs/0000046_415.png"" alt=""PDM242220"" style=""border:solid 1px #d8d8d8;display:block;overflow-x:auto;max-width:148px;width:100%;padding:3px"" id=""m_-8589411776884389367"" border=""0"" class=""CToWUd"">
                                                                                                                </a>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                        <tr>
                                                                                                            <td height=""10""></td>
                                                                                                        </tr>
                                                                                                    </tbody>
                                                                                                </table>
                                                                                            </td>
                                                                                            <td style=""max-width:310px"" valign=""top"">
                                                                                                <a href=""https://onjobsupport.in/roja-venigalla"" style=""text-decoration:none"" target=""_blank"">
                                                                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                                                                        <tbody>
                                                                                                            <tr>
                                                                                                                <td>
                                                                                                                    <img src=""https://ci6.googleusercontent.com/proxy/58z8S94yrwRpt6mRMtBLAVGXgvUolady4boC_sOZlJWkhzTVQxCNY6bTZ2Oc6qTHctyAzrE8d8Ri4mxuKDo1ABWhL1hAHbsAGA=s0-d-e1-ft#http://imgs.padmasalimatrimony.com/cbsimages/trans.gif"" alt="""" border=""0"" height=""1"" width=""10"" class=""CToWUd"">
                                                                                                                </td>
                                                                                                                <td style=""padding-top:5px"" valign=""top"">
                                                                                                                    <a href=""https://onjobsupport.in/roja-venigalla"" style=""font:bold 1em arial;line-height:20px;color:#363636;text-decoration:none"" target=""_blank""> {product.Name}  </a>
                                                                                                                </td>
                             ";

            var str2 = @" <tr><td colspan=""2"" height=""9""></td></tr>";

            var str3 = $@"
                        <tr>
                            <td></td>
                            <td height=""90"" valign=""top"">
                                <a href=""https://onjobsupport.in/roja-venigalla"" style=""font:normal .9em/20px arial;color:#656565;text-decoration:none"" target=""_blank"">
                                    {product.ShortDescription}<br>
                                    Location: Hyderabad <br>
                                    Primary Technology: <br>
                                    Mother Tounge : English <br>
                                    Exp : 1-3 Years <br>
                                    Occupation : {product.ShortDescription}
                                </a>
                            </td>
                        </tr> ";

            var str4 = @"
                            <tr>
                                <td></td>
                                <td valign=""top"">
                                    <table align=""left"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                                        <tbody>
                                            <tr>
                                                <td valign=""middle"">
                                                    <a href=""https://onjobsupport.in/roja-venigalla"" target=""_blank"">
                                                        View full profile
                                                        <img src=""https://ci5.googleusercontent.com/proxy/W2Rn2G4dVBXAUUWYxA1Fffg404n3ckQWQjEe7xGx8rAq-xk3T8Ngi0rNy6QuqVycuWgwyCkrWtAr50gyWkRUeF0nMHa4MmAx2_AKpLU_zTvgU3k=s0-d-e1-ft#http://imgs.padmasalimatrimony.com/cbsimages/mwatch-vfp-bull.gif"" class=""CToWUd"">
                                                    </a>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                    <table align=""right"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                                        <tbody>
                                            <tr>
                                                <td height=""15""></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr> ";

            var str5 = @"
                        <tr>
                                                                                                <td colspan=""2"" height=""10""></td>
                                                                                            </tr>
                                                                                        </tbody>
                                                                                    </table>
                                                                                </a>
                                                                            </td>
                                                                            <td style=""width:2%;max-width:10px""><img src=""https://ci6.googleusercontent.com/proxy/58z8S94yrwRpt6mRMtBLAVGXgvUolady4boC_sOZlJWkhzTVQxCNY6bTZ2Oc6qTHctyAzrE8d8Ri4mxuKDo1ABWhL1hAHbsAGA=s0-d-e1-ft#http://imgs.padmasalimatrimony.com/cbsimages/trans.gif"" alt="""" border=""0"" height=""1"" width=""1"" class=""CToWUd""></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan=""4"" style=""border-top:solid 2px #f4f4f4;font-size:5px;line-height:5px"">&nbsp;</td>
                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                    <tbody>
                                                        <tr>
                                                            <td height=""10""></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height=""17""></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                            <td style=""width:3%;max-width:20px"">
                                <img src=""https://ci6.googleusercontent.com/proxy/58z8S94yrwRpt6mRMtBLAVGXgvUolady4boC_sOZlJWkhzTVQxCNY6bTZ2Oc6qTHctyAzrE8d8Ri4mxuKDo1ABWhL1hAHbsAGA=s0-d-e1-ft#http://imgs.padmasalimatrimony.com/cbsimages/trans.gif"" alt="""" border=""0"" height=""1"" width=""1"" class=""CToWUd"">
                            </td>
                        </tr>
                    </tbody>
                                </table>
                            </td>
                        </tr>

                ";

            sb.AppendLine(str1);
            sb.AppendLine(str2);
            sb.AppendLine(str3);
            sb.AppendLine(str4);
            sb.AppendLine(str5);

            var result = sb.ToString();
            return result;
        }

        #endregion
    }
}