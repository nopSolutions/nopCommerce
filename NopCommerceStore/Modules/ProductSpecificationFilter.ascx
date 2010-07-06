<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductSpecificationFilterControl"
    CodeBehind="ProductSpecificationFilter.ascx.cs" %>
<div class="product-spec-filter">
    <div class="title">
        <%=GetLocaleResourceString("Common.SpecificationFilter")%>
    </div>
    <div class="clear">
    </div>
    <asp:Panel runat="server" ID="pnlPSOSelector">
        <table class="filter">
            <asp:Repeater ID="rptFilterByPSO" runat="server" OnItemDataBound="rptFilterByPSO_OnItemDataBound">
                <ItemTemplate>
                    <%#addSpecificationAttribute()%>
                    <tr class="item">
                        <td>
                            <asp:HyperLink ID="lnkFilter" runat="server">
                                <%#Server.HtmlEncode(Eval("SpecificationAttributeOptionName").ToString())%>
                            </asp:HyperLink>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlAlreadyFilteredPSO">
        <div class="title">
            <%=GetLocaleResourceString("Common.CurrentlyFilteredBy")%>
        </div>
        <table class="filter">
            <asp:Repeater ID="rptAlreadyFilteredPSO" runat="server" OnItemDataBound="rptAlreadyFilteredPSO_OnItemDataBound">
                <ItemTemplate>
                    <tr class="filtereditem">
                        <td>
                            <b>
                                <%#Server.HtmlEncode(Eval("SpecificationAttributeName").ToString())%>:</b>
                            <%#Server.HtmlEncode(Eval("SpecificationAttributeOptionName").ToString())%>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlRemoveFilter" CssClass="remove-filter">
        <asp:HyperLink ID="hlRemoveFilter" runat="server" CssClass="remove-product-spec-filter">
            <%=GetLocaleResourceString("Common.SpecificationFilterRemove")%>
        </asp:HyperLink>
    </asp:Panel>
</div>
