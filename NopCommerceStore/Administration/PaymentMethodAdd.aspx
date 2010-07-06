<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_PaymentMethodAdd"
    CodeBehind="PaymentMethodAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="Modules/ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="Modules/NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="Modules/SimpleTextBox.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="section-header">
        <div class="title">
            <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.PaymentMethodAdd.Title")%>" />
            <%=GetLocaleResourceString("Admin.PaymentMethodAdd.Title")%>
            <a href="PaymentMethods.aspx" title="<%=GetLocaleResourceString("Admin.PaymentMethodAdd.BackToMethodList")%>">
                (<%=GetLocaleResourceString("Admin.PaymentMethodAdd.BackToMethodList")%>)</a>
        </div>
        <div class="options">
            <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.PaymentMethodAdd.SaveButton.Text %>"
                CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.PaymentMethodAdd.SaveButton.Tooltip %>" />
        </div>
    </div>
    <table class="adminContent">
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.PaymentMethodInfo.Name %>"
                    ToolTip="<% $NopResources:Admin.PaymentMethodInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.PaymentMethodInfo.Name.ErrorMessage %>">
                </nopCommerce:SimpleTextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblVisibleName" Text="<% $NopResources:Admin.PaymentMethodInfo.VisibleName %>"
                    ToolTip="<% $NopResources:Admin.PaymentMethodInfo.VisibleName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:SimpleTextBox runat="server" ID="txtVisibleName" CssClass="adminInput"
                    ErrorMessage="<% $NopResources:Admin.PaymentMethodInfo.VisibleName.ErrorMessage %>">
                </nopCommerce:SimpleTextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblDescription" Text="<% $NopResources:Admin.PaymentMethodInfo.Description %>"
                    ToolTip="<% $NopResources:Admin.PaymentMethodInfo.Description.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:TextBox ID="txtDescription" runat="server" CssClass="adminInput" TextMode="MultiLine"
                    Height="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblConfigureTemplatePath" Text="<% $NopResources:Admin.PaymentMethodInfo.ConfigureTemplatePath %>"
                    ToolTip="<% $NopResources:Admin.PaymentMethodInfo.ConfigureTemplatePath.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:TextBox ID="txtConfigureTemplatePath" CssClass="adminInput" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblUserTemplatePath" Text="<% $NopResources:Admin.PaymentMethodInfo.UserTemplatePath %>"
                    ToolTip="<% $NopResources:Admin.PaymentMethodInfo.UserTemplatePath.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:TextBox ID="txtUserTemplatePath" CssClass="adminInput" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblClassName" Text="<% $NopResources:Admin.PaymentMethodInfo.ClassName %>"
                    ToolTip="<% $NopResources:Admin.PaymentMethodInfo.ClassName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:SimpleTextBox runat="server" ID="txtClassName" CssClass="adminInput"
                    ErrorMessage="<% $NopResources:Admin.PaymentMethodInfo.ClassName.ErrorMessage %>">
                </nopCommerce:SimpleTextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblSystemKeyword" Text="<% $NopResources:Admin.PaymentMethodInfo.SystemKeyword %>"
                    ToolTip="<% $NopResources:Admin.PaymentMethodInfo.SystemKeyword.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:TextBox ID="txtSystemKeyword" CssClass="adminInput" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblHidePaymentInfoForZeroOrders" Text="<% $NopResources:Admin.PaymentMethodInfo.HidePaymentInfoForZeroOrders %>"
                    ToolTip="<% $NopResources:Admin.PaymentMethodInfo.HidePaymentInfoForZeroOrders.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:CheckBox ID="cbHidePaymentInfoForZeroOrders" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblActive" Text="<% $NopResources:Admin.PaymentMethodInfo.Active %>"
                    ToolTip="<% $NopResources:Admin.PaymentMethodInfo.Active.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:CheckBox ID="cbActive" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.PaymentMethodInfo.DisplayOrder %>"
                    ToolTip="<% $NopResources:Admin.PaymentMethodInfo.DisplayOrder.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtDisplayOrder"
                    Value="1" RequiredErrorMessage="<% $NopResources:Admin.PaymentMethodInfo.DisplayOrder.RequiredErrorMessage %>"
                    RangeErrorMessage="<% $NopResources:Admin.PaymentMethodInfo.DisplayOrder.RangeErrorMessage %>"
                    MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
            </td>
        </tr>
    </table>
</asp:Content>
