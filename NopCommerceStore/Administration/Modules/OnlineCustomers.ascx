<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.OnlineCustomersControl"
    CodeBehind="OnlineCustomers.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-customers.png" alt="<%=GetLocaleResourceString("Admin.OnlineCustomers.Title")%>" />
        <%=GetLocaleResourceString("Admin.OnlineCustomers.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.OnlineCustomers.SaveButton.Text %>"
            CssClass="adminButtonBlue" ID="btnSave" OnClick="btnSave_Click" />
    </div>
</div>
<table width="100%">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblForumsEnabled" Text="<% $NopResources:Admin.OnlineCustomers.Enabled %>"
                ToolTip="<% $NopResources:Admin.OnlineCustomers.Enabled.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbEnabled"></asp:CheckBox>
        </td>
    </tr>
</table>
<asp:PlaceHolder runat="server" ID="phOnlineStats">
    <br />
    <hr />
    <asp:GridView ID="gvCustomers" runat="server" AutoGenerateColumns="False" Width="100%"
        OnPageIndexChanging="gvCustomers_PageIndexChanging" AllowPaging="true" PageSize="15">
        <Columns>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.OnlineCustomers.CustomerInfoColumn %>"
                ItemStyle-Width="70%">
                <ItemTemplate>
                    <%#GetCustomerInfo((OnlineUserInfo)Container.DataItem)%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.Customers.LastVisitColumn %>"
                HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("LastVisit"), DateTimeKind.Utc).ToString()%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
    </asp:GridView>
    <p>
    </p>
    <table width="100%">
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblGuestsTitle" Text="<% $NopResources:Admin.OnlineCustomers.Guests %>"
                    ToolTip="<% $NopResources:Admin.OnlineCustomers.Guests.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:Label runat="server" ID="lblGuests" />
            </td>
        </tr>
    </table>
</asp:PlaceHolder>
