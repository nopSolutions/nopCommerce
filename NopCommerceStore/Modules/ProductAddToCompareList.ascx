<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductAddToCompareList"
    CodeBehind="ProductAddToCompareList.ascx.cs" %>
<asp:Button runat="server" ID="btnAddToCompareList" Text="<% $NopResources:Products.AddToCompareList %>"
    OnClick="btnAddToCompareList_Click" CausesValidation="false" CssClass="productaddtocomparelistbutton" />
