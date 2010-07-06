<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PurchasedGiftCardsControl"
    CodeBehind="PurchasedGiftCards.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DatePicker" Src="DatePicker.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.PurchasedGiftCards.Title")%>" />
        <%=GetLocaleResourceString("Admin.PurchasedGiftCards.Title")%>
    </div>
    <div class="options">
        <asp:Button ID="SearchButton" runat="server" Text="<% $NopResources:Admin.PurchasedGiftCards.SearchButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SearchButton_Click" ToolTip="<% $NopResources:Admin.PurchasedGiftCards.SearchButton.Tooltip %>" />
    </div>
</div>
<table width="100%">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPurchasedFrom" Text="<% $NopResources:Admin.PurchasedGiftCards.PurchasedFrom %>"
                ToolTip="<% $NopResources:Admin.PurchasedGiftCards.PurchasedFrom.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlStartDatePicker" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPurchasedTo" Text="<% $NopResources:Admin.PurchasedGiftCards.PurchasedTo %>"
                ToolTip="<% $NopResources:Admin.PurchasedGiftCards.PurchasedTo.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlEndDatePicker" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblOrderStatus" Text="<% $NopResources:Admin.PurchasedGiftCards.OrderStatus %>"
                ToolTip="<% $NopResources:Admin.PurchasedGiftCards.OrderStatus.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlOrderStatus" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblActivated" Text="<% $NopResources:Admin.PurchasedGiftCards.Activated %>"
                ToolTip="<% $NopResources:Admin.PurchasedGiftCards.Activated.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlActivated" runat="server" CssClass="adminInput">
                <asp:ListItem Text="<% $NopResources:Admin.PurchasedGiftCards.Activated.All %>"
                    Value="0"></asp:ListItem>
                <asp:ListItem Text="<% $NopResources:Admin.PurchasedGiftCards.Activated.Activated %>"
                    Value="1"></asp:ListItem>
                <asp:ListItem Text="<% $NopResources:Admin.PurchasedGiftCards.Activated.Deactivated %>"
                    Value="2"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblGiftCardCouponCode" Text="<% $NopResources:Admin.PurchasedGiftCards.GiftCardCouponCode %>"
                ToolTip="<% $NopResources:Admin.PurchasedGiftCards.GiftCardCouponCode.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtGiftCardCouponCode" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
</table>
<p>
</p>
<asp:GridView ID="gvGiftCards" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvGiftCards_PageIndexChanging" AllowPaging="true" PageSize="15">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PurchasedGiftCards.CustomerColumn %>"
            ItemStyle-Width="15%">
            <ItemTemplate>
                <%#GetCustomerInfo((GiftCard)Container.DataItem)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PurchasedGiftCards.OrderStatusColumn %>"
            ItemStyle-Width="10%">
            <ItemTemplate>
                <%#GetOrderStatusInfo((GiftCard)Container.DataItem)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PurchasedGiftCards.InitialValueColumn %>"
            ItemStyle-Width="10%">
            <ItemTemplate>
                <%#GetInitialValueInfo((GiftCard)Container.DataItem)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PurchasedGiftCards.RemainingAmountColumn %>"
            ItemStyle-Width="10%">
            <ItemTemplate>
                <%#GetRemainingAmountInfo((GiftCard)Container.DataItem)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PurchasedGiftCards.CouponCodeColumn %>"
            ItemStyle-Width="15%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("GiftCardCouponCode").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PurchasedGiftCards.IsGiftCardActivatedColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:ImageCheckBox runat="server" ID="cbIsGiftCardActivated" Checked='<%# Eval("IsGiftCardActivated") %>'>
                </nopCommerce:ImageCheckBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PurchasedGiftCards.PurchasedOnColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#GetPurchasedOnInfo((GiftCard)Container.DataItem)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PurchasedGiftCards.EditColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="GiftCardDetails.aspx?GiftCardID=<%#Eval("GiftCardId")%>" title="<%#GetLocaleResourceString("Admin.PurchasedGiftCards.EditColumn.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.PurchasedGiftCards.EditColumn")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
