<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.LogsControl"
    CodeBehind="Logs.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DatePicker" Src="DatePicker.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-system.png" alt="<%=GetLocaleResourceString("Admin.Logs.Title")%>" />
        <%=GetLocaleResourceString("Admin.Logs.Title")%>
    </div>
    <div class="options">
        <asp:Button ID="btnSearch" runat="server" Text="<% $NopResources:Admin.Logs.SearchButton.Text %>"
            CssClass="adminButtonBlue" OnClick="btnSearch_Click" ToolTip="<% $NopResources:Admin.Logs.SearchButton.Tooltip %>" />
        <asp:Button ID="btnClear" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Logs.ClearButton.Text %>"
            OnClick="btnClear_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.Logs.ClearButton.Tooltip %>" />
    </div>
</div>

<table width="100%">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnFrom" Text="<% $NopResources:Admin.Log.CreatedOnFrom %>"
                ToolTip="<% $NopResources:Admin.Log.CreatedOnFrom.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlCreatedOnFromDatePicker" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTo" Text="<% $NopResources:Admin.Log.CreatedOnTo %>"
                ToolTip="<% $NopResources:Admin.Log.CreatedOnTo.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlCreatedOnToDatePicker" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMessage" Text="<% $NopResources:Admin.Log.Message %>"
                ToolTip="<% $NopResources:Admin.Log.Message.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtMessage" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLogType" Text="<% $NopResources:Admin.Log.LogType %>"
                ToolTip="<% $NopResources:Admin.Log.LogType.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlLogType" runat="server" CssClass="adminInput" />
        </td>
    </tr>
</table>
<p>
</p>
<asp:GridView ID="gvLogs" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvLogs_PageIndexChanging" AllowPaging="true" PageSize="15">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Logs.LogType %>" ItemStyle-Width="12%">
            <ItemTemplate>
                <%#CommonHelper.ConvertEnum(((LogTypeEnum)(Eval("LogType"))).ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Logs.Customer %>" ItemStyle-Width="15%">
            <ItemTemplate>
                <%#GetCustomerInfo(Convert.ToInt32(Eval("CustomerId")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Logs.CreatedOn %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="18%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Logs.Message %>" ItemStyle-Width="20%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Message").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Logs.Details %>" ItemStyle-Width="15%">
            <ItemTemplate>
                <a href="LogDetails.aspx?LogID=<%#Eval("LogId")%>" title="<%#GetLocaleResourceString("Admin.Logs.Details.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Logs.Details")%></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Logs.Delete %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Button ID="DeleteLogButton" runat="server" CssClass="adminButton" CommandName="DeleteLog"
                    Text="<% $NopResources:Admin.Logs.DeleteButton.Text %>" CommandArgument='<%#Eval("LogId")%>'
                    OnCommand="DeleteLogButton_OnCommand" CausesValidation="false" ToolTip="<% $NopResources:Admin.Logs.DeleteButton.Tooltip %>" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
</asp:GridView>
