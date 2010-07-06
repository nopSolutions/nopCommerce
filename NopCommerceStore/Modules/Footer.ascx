<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.FooterControl"
    CodeBehind="Footer.ascx.cs" %>
<div class="footer">
    <div class="footer-disclaimer">
        <%=String.Format(GetLocaleResourceString("Content.CopyrightNotice"), 
                                    DateTime.Now.Year.ToString(), 
                                    SettingManager.StoreName)%>
    </div>
    <div class="footer-poweredby">
        Powered by <a href="http://www.nopcommerce.com/">nopCommerce</a>
        <%--Would you like to remove the "Powered by nopCommerce" link in the bottom of the footer? 
        Find more info here http://www.nopcommerce.com/copyrightremoval.aspx--%>
    </div>
</div>
