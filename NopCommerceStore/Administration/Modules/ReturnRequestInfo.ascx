<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ReturnRequestInfoControl"
    CodeBehind="ReturnRequestInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblReturnRequestIdTitle" Text="<% $NopResources:Admin.ReturnRequestInfo.RequestId %>"
                ToolTip="<% $NopResources:Admin.ReturnRequestInfo.RequestId.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblReturnRequestId" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblNameTitle" Text="<% $NopResources:Admin.ReturnRequestInfo.Name %>"
                ToolTip="<% $NopResources:Admin.ReturnRequestInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblName" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblOrderTitle" Text="<% $NopResources:Admin.ReturnRequestInfo.Order %>"
                ToolTip="<% $NopResources:Admin.ReturnRequestInfo.Order.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblOrder" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerTitle" Text="<% $NopResources:Admin.ReturnRequestInfo.Customer %>"
                ToolTip="<% $NopResources:Admin.ReturnRequestInfo.Customer.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCustomer" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblStatus" Text="<% $NopResources:Admin.ReturnRequestInfo.Status %>"
                ToolTip="<% $NopResources:Admin.ReturnRequestInfo.Status.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlStatus" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblReasonForReturn" Text="<% $NopResources:Admin.ReturnRequestInfo.ReasonForReturn %>"
                ToolTip="<% $NopResources:Admin.ReturnRequestInfo.ReasonForReturn.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtReasonForReturn" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblRequestedAction" Text="<% $NopResources:Admin.ReturnRequestInfo.RequestedAction %>"
                ToolTip="<% $NopResources:Admin.ReturnRequestInfo.RequestedAction.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtRequestedAction" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerComments" Text="<% $NopResources:Admin.ReturnRequestInfo.CustomerComments %>"
                ToolTip="<% $NopResources:Admin.ReturnRequestInfo.CustomerComments.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtCustomerComments" TextMode="MultiLine" Height="150px"
                Width="500px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblStaffNotes" Text="<% $NopResources:Admin.ReturnRequestInfo.StaffNotes %>"
                ToolTip="<% $NopResources:Admin.ReturnRequestInfo.StaffNotes.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtStaffNotes" TextMode="MultiLine" Height="150px"
                Width="500px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDateTooltip" Text="<% $NopResources:Admin.ReturnRequestInfo.Date %>"
                ToolTip="<% $NopResources:Admin.ReturnRequestInfo.Date.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label runat="server" ID="lblDate"></asp:Label>
        </td>
    </tr>
</table>
