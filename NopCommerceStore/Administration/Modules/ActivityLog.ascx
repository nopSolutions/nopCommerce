<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ActivityLogControl" CodeBehind="ActivityLog.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DatePicker" Src="DatePicker.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ActivityLog.Title")%>" />
        <%=GetLocaleResourceString("Admin.ActivityLog.Title")%>
    </div>
    <div class="options">
        <asp:Button ID="SearchButton" runat="server" Text="<% $NopResources:Admin.ActivityLog.SearchButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SearchButton_Click" ToolTip="<% $NopResources:Admin.ActivityLog.SearchButton.Tooltip %>" />
        <asp:Button ID="ClearButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ActivityLog.ClearButton.Text %>"
            OnClick="ClearButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.ActivityLog.ClearButton.Tooltip %>" />
    </div>
</div>
<table width="100%">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnFrom" Text="<% $NopResources:Admin.ActivityLog.CreatedOnFrom %>"
                ToolTip="<% $NopResources:Admin.ActivityLog.CreatedOnFrom.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlCreatedOnFromDatePicker" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTo" Text="<% $NopResources:Admin.ActivityLog.CreatedOnTo %>"
                ToolTip="<% $NopResources:Admin.ActivityLog.CreatedOnTo.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlCreatedOnToDatePicker" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblEmail" Text="<% $NopResources:Admin.ActivityLog.CustomerEmail %>"
                ToolTip="<% $NopResources:Admin.ActivityLog.CustomerEmail.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtCustomerEmail" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <asp:PlaceHolder runat="server" ID="phCustomerName">
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblUsername" Text="<% $NopResources:Admin.ActivityLog.CustomerName %>"
                    ToolTip="<% $NopResources:Admin.ActivityLog.CustomerName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:TextBox ID="txtCustomerName" CssClass="adminInput" runat="server"></asp:TextBox>
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblActivityLogType" Text="<% $NopResources:Admin.ActivityLog.ActivityLogType %>"
                ToolTip="<% $NopResources:Admin.ActivityLog.ActivityLogType.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlActivityLogType" runat="server" CssClass="adminInput" />
        </td>
    </tr>
</table>
<p>
</p>
<asp:GridView ID="gvActivityLog" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvActivityLog_PageIndexChanging" AllowPaging="true" PageSize="15">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ActivityLog.ActivityLogTypeColumn %>" ItemStyle-Width="20%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("ActivityLogType").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ActivityLog.CustomerColumn %>" ItemStyle-Width="15%">
            <ItemTemplate>
                <%#GetCustomerInfo ((Customer)(Eval("Customer")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ActivityLog.MessageColumn %>" ItemStyle-Width="45%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Message").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ActivityLog.CreateOnColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%">
            <ItemTemplate>
                <%#((DateTime)Eval("CreateOn")).ToString()%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ActivityLog.DeleteColumn %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Button ID="DeleteActivityLogButton" runat="server" CssClass="adminButton" CommandName="DeleteActivityLog"
                    Text="<% $NopResources:Admin.ActivityLog.DeleteButton.Text %>" CommandArgument='<%#Eval("ActivityLogId")%>'
                    OnCommand="DeleteActivityLogButton_OnCommand" CausesValidation="false" ToolTip="<% $NopResources:Admin.ActivityLog.DeleteButton.Tooltip %>" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
</asp:GridView>
