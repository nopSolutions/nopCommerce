<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CustomerReturnRequestsControl"
    CodeBehind="CustomerReturnRequests.ascx.cs" %>
<div class="customer-return-request-list">
    <div class="section-title">
        <%=GetLocaleResourceString("CustomerReturnRequests.Title")%></div>
    <div class="clear">
    </div>
    <div class="request-list">
        <asp:Repeater ID="rptrRequests" runat="server" OnItemDataBound="rptrRequests_ItemDataBound"
            EnableViewState="false">
            <ItemTemplate>
                <div class="request-item">
                    <b>
                        <asp:Literal runat="server" ID="lRequestTitle" /></b>
                    <br />
                    <asp:Literal runat="server" ID="lItem" />
                    <br />
                    <asp:Literal runat="server" ID="lReason" />
                    <br />
                    <asp:Literal runat="server" ID="lAction" />
                    <br />
                    <asp:Literal runat="server" ID="lDate" />
                    <asp:PlaceHolder runat="server" ID="phComments">
                        <br />
                        <%=GetLocaleResourceString("CustomerReturnRequests.Comments")%>
                        <br />
                        <asp:Literal runat="server" ID="lComments" />
                    </asp:PlaceHolder>
                </div>
            </ItemTemplate>
            <SeparatorTemplate>
                <div class="clear">
                </div>
            </SeparatorTemplate>
        </asp:Repeater>
    </div>
</div>
