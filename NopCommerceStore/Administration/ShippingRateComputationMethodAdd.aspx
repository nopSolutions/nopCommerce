<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ShippingRateComputationMethodAdd"
    CodeBehind="ShippingRateComputationMethodAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="Modules/ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="Modules/NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="Modules/SimpleTextBox.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="section-header">
        <div class="title">
            <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ShippingRateComputationMethodAdd.Title")%>" />
            <%=GetLocaleResourceString("Admin.ShippingRateComputationMethodAdd.Title")%>
            <a href="ShippingRateComputationMethods.aspx" title="<%=GetLocaleResourceString("Admin.ShippingRateComputationMethodAdd.BackToMethods")%>">
                (<%=GetLocaleResourceString("Admin.ShippingRateComputationMethodAdd.BackToMethods")%>)</a>
        </div>
        <div class="options">
            <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.ShippingRateComputationMethodAdd.SaveButton.Text %>"
                CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodAdd.SaveButton.Tooltip %>" />
        </div>
    </div>
    <table class="adminContent">
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Name %>"
                    ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Name.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Name.ErrorMessage %>">
                </nopCommerce:SimpleTextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblDescription" Text="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Description %>"
                    ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Description.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:TextBox ID="txtDescription" runat="server" CssClass="adminInput" TextMode="MultiLine"
                    Height="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblConfigureTemplatePath" Text="<% $NopResources:Admin.ShippingRateComputationMethodInfo.ConfigureTemplatePath %>"
                    ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodInfo.ConfigureTemplatePath.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:TextBox ID="txtConfigureTemplatePath" runat="server" CssClass="adminInput"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblClassName" Text="<% $NopResources:Admin.ShippingRateComputationMethodInfo.ClassName %>"
                    ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodInfo.ClassName.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:SimpleTextBox runat="server" ID="txtClassName" CssClass="adminInput"
                    ErrorMessage="<% $NopResources:Admin.ShippingRateComputationMethodInfo.ClassName.ErrorMessage %>">
                </nopCommerce:SimpleTextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblActive" Text="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Active %>"
                    ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Active.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:CheckBox ID="cbActive" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.ShippingRateComputationMethodInfo.DisplayOrder %>"
                    ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodInfo.DisplayOrder.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:NumericTextBox runat="server" ID="txtDisplayOrder" CssClass="adminInput"
                    Value="1" RequiredErrorMessage="<% $NopResources:Admin.ShippingRateComputationMethodInfo.DisplayOrder.RequiredErrorMessage %>"
                    MinimumValue="-99999" MaximumValue="99999" RangeErrorMessage="<% $NopResources:Admin.ShippingRateComputationMethodInfo.DisplayOrder.RangeErrorMessage %>">
                </nopCommerce:NumericTextBox>
            </td>
        </tr>
    </table>
</asp:Content>
