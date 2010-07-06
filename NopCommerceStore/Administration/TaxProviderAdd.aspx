<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_TaxProviderAdd"
    CodeBehind="TaxProviderAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="Modules/ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="Modules/NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="Modules/SimpleTextBox.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="section-header">
        <div class="title">
            <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.TaxProviderAdd.Title")%>" />
            <%=GetLocaleResourceString("Admin.TaxProviderAdd.Title")%>
            <a href="TaxProviders.aspx" title="<%=GetLocaleResourceString("Admin.TaxProviderAdd.BackToProviders")%>">
                (<%=GetLocaleResourceString("Admin.TaxProviderAdd.BackToProviders")%>)</a>
        </div>
        <div class="options">
            <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.TaxProviderAdd.SaveButton.Text %>"
                CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.TaxProviderAdd.SaveButton.Tooltip %>" />
        </div>
    </div>
    <table class="adminContent">
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.TaxProviderInfo.Name %>"
                    ToolTip="<% $NopResources:Admin.TaxProviderInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.TaxProviderInfo.Name.ErrorMessage %>">
                </nopCommerce:SimpleTextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblDescription" Text="<% $NopResources:Admin.TaxProviderInfo.Description %>"
                    ToolTip="<% $NopResources:Admin.TaxProviderInfo.Description.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:TextBox ID="txtDescription" runat="server" CssClass="adminInput" TextMode="MultiLine"
                    Height="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblConfigureTemplatePath" Text="<% $NopResources:Admin.TaxProviderInfo.ConfigureTemplatePath %>"
                    ToolTip="<% $NopResources:Admin.TaxProviderInfo.ConfigureTemplatePath.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:TextBox ID="txtConfigureTemplatePath" runat="server" CssClass="adminInput"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblClassName" Text="<% $NopResources:Admin.TaxProviderInfo.ClassName %>"
                    ToolTip="<% $NopResources:Admin.TaxProviderInfo.ClassName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:SimpleTextBox runat="server" ID="txtClassName" CssClass="adminInput"
                    ErrorMessage="<% $NopResources:Admin.TaxProviderInfo.ClassName.ErrorMessage %>">
                </nopCommerce:SimpleTextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.TaxProviderInfo.DisplayOrder %>"
                    ToolTip="<% $NopResources:Admin.TaxProviderInfo.DisplayOrder.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:NumericTextBox runat="server" ID="txtDisplayOrder" CssClass="adminInput"
                    Value="1" RequiredErrorMessage="<% $NopResources:Admin.TaxProviderInfo.DisplayOrder.RequiredErrorMessage %>"
                    MinimumValue="-99999" MaximumValue="99999" RangeErrorMessage="<% $NopResources:Admin.TaxProviderInfo.DisplayOrder.RangeErrorMessage %>">
                </nopCommerce:NumericTextBox>
            </td>
        </tr>
    </table>
</asp:Content>
