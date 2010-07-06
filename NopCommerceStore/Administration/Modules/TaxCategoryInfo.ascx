<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.TaxCategoryInfoControl"
    CodeBehind="TaxCategoryInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.TaxCategoryInfo.Name %>"
                ToolTip="<% $NopResources:Admin.TaxCategoryInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.TaxCategoryInfo.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.TaxCategoryInfo.DisplayOrder %>"
                ToolTip="<% $NopResources:Admin.TaxCategoryInfo.DisplayOrder.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtDisplayOrder"
                Value="1" RequiredErrorMessage="<% $NopResources:Admin.TaxCategoryInfo.DisplayOrder.RequiredErrorMessage %>"
                MinimumValue="-99999" MaximumValue="99999" RangeErrorMessage="<% $NopResources:Admin.TaxCategoryInfo.DisplayOrder.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr runat="server" id="pnlCreatedOn">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTitle" Text="<% $NopResources:Admin.TaxCategoryInfo.CreatedOn %>"
                ToolTip="<% $NopResources:Admin.TaxCategoryInfo.CreatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCreatedOn" runat="server"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="pnlUpdatedOn">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblUpdatedOnTitle" Text="<% $NopResources:Admin.TaxCategoryInfo.UpdatedOn %>"
                ToolTip="<% $NopResources:Admin.TaxCategoryInfo.UpdatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblUpdatedOn" runat="server"></asp:Label>
        </td>
    </tr>
</table>
