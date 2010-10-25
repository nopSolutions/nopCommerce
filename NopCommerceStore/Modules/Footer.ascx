<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.FooterControl"
    CodeBehind="Footer.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="StoreThemeSelector" Src="~/Modules/StoreThemeSelector.ascx" %>
<div class="footer">
    <div class="footer-poweredby">
        Powered by <a href="http://www.nopcommerce.com/">nopCommerce</a>
        <%--Would you like to remove the "Powered by nopCommerce" link in the bottom of the footer? 
        Find more info here http://www.nopcommerce.com/copyrightremoval.aspx--%>
    </div>
    <div class="footer-disclaimer">
        <%=String.Format(GetLocaleResourceString("Content.CopyrightNotice"), 
                                    DateTime.Now.Year.ToString(), 
                                    IoCFactory.Resolve<ISettingManager>().StoreName)%>
    </div>
    <div class="footer-storetheme">
        <nopCommerce:StoreThemeSelector ID="ctrlStoreTheme" runat="server" />
    </div>
</div>
