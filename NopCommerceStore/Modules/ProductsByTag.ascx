<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductsByTagControl"
    CodeBehind="ProductsByTag.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductBox1" Src="~/Modules/ProductBox1.ascx" %>
<div class="producttag-page">
    <div class="page-title">
        <h1><asp:Literal runat="server" ID="lTitle"></asp:Literal></h1>
    </div>
    <div class="clear">
    </div>
    <div class="product-grid">
        <asp:DataList ID="dlProducts" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
            RepeatLayout="Table" ItemStyle-CssClass="item-box">
            <ItemTemplate>
                <nopCommerce:ProductBox1 ID="ctrlProductBox" Product='<%# Container.DataItem %>'
                    runat="server" />
            </ItemTemplate>
        </asp:DataList>
    </div>
    <div class="product-pager">
        <nopCommerce:Pager runat="server" ID="productsPager" FirstButtonText="<% $NopResources:Pager.First %>"
            LastButtonText="<% $NopResources:Pager.Last %>" NextButtonText="<% $NopResources:Pager.Next %>"
            PreviousButtonText="<% $NopResources:Pager.Previous %>" CurrentPageText="Pager.CurrentPage" />
    </div>
</div>
