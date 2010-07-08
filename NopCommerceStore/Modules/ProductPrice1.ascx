<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Modules.ProductPrice1Control" Codebehind="ProductPrice1.ascx.cs" %>
   
<asp:PlaceHolder runat="server" ID="phOldPrice">
    <%=GetLocaleResourceString("Products.OldPrice")%>&nbsp;
    <asp:Label ID="lblOldPrice" runat="server" CssClass="oldProductPrice" />
</asp:PlaceHolder>
<br />
<asp:Label ID="lblCustomerEnterPrice" runat="server" Visible="false" />
<asp:Label ID="lblPrice" runat="server" Visible="false" />
<asp:Label ID="lblPriceValue" runat="server" CssClass="productPrice" />
<asp:PlaceHolder runat="server" ID="phDiscount">
    <br />
    <%=GetLocaleResourceString("Products.FinalPriceWithDiscount")%>&nbsp;&nbsp;
    <asp:Label ID="lblFinalPriceWithDiscount" runat="server" CssClass="productPrice" />
</asp:PlaceHolder>
