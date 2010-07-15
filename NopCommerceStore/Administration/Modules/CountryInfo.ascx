<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CountryInfoControl"
    CodeBehind="CountryInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.CountryInfo.Name %>"
                ToolTip="<% $NopResources:Admin.CountryInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.CountryInfo.Name.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAllowsRegistration" Text="<% $NopResources:Admin.CountryInfo.AllowsRegistration %>"
                ToolTip="<% $NopResources:Admin.CountryInfo.AllowsRegistration.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbAllowsRegistration" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAllowsBilling" Text="<% $NopResources:Admin.CountryInfo.AllowsBilling %>"
                ToolTip="<% $NopResources:Admin.CountryInfo.AllowsBilling.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbAllowsBilling" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAllowsShipping" Text="<% $NopResources:Admin.CountryInfo.AllowsShipping %>"
                ToolTip="<% $NopResources:Admin.CountryInfo.AllowsShipping.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbAllowsShipping" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTwoLetterISOCode" Text="<% $NopResources:Admin.CountryInfo.TwoLetterISOCode %>"
                ToolTip="<% $NopResources:Admin.CountryInfo.TwoLetterISOCode.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtTwoLetterISOCode" CssClass="adminInput"
                ErrorMessage="<% $NopResources:Admin.CountryInfo.TwoLetterISOCode.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblThreeLetterISOCode" Text="<% $NopResources:Admin.CountryInfo.ThreeLetterISOCode %>"
                ToolTip="<% $NopResources:Admin.CountryInfo.ThreeLetterISOCode.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtThreeLetterISOCode" CssClass="adminInput"
                ErrorMessage="<% $NopResources:Admin.CountryInfo.ThreeLetterISOCode.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblNumbericISOCode" Text="<% $NopResources:Admin.CountryInfo.NumbericISOCode %>"
                ToolTip="<% $NopResources:Admin.CountryInfo.NumbericISOCode.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" ID="txtNumericISOCode" Value="1" CssClass="adminInput"
                RequiredErrorMessage="<% $NopResources:Admin.CountryInfo.NumbericISOCode.RequiredErrorMessage %>"
                RangeErrorMessage="<% $NopResources:Admin.CountryInfo.NumbericISOCode.RangeErrorMessage %>"
                MinimumValue="1" MaximumValue="9999"></nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSubjectToVAT" Text="<% $NopResources:Admin.CountryInfo.SubjectToVAT %>"
                ToolTip="<% $NopResources:Admin.CountryInfo.SubjectToVAT.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbSubjectToVAT" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPublished" Text="<% $NopResources:Admin.CountryInfo.Published %>"
                ToolTip="<% $NopResources:Admin.CountryInfo.Published.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbPublished" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.CountryInfo.DisplayOrder %>"
                ToolTip="<% $NopResources:Admin.CountryInfo.DisplayOrder.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" ID="txtDisplayOrder" CssClass="adminInput"
                Value="1" RequiredErrorMessage="<% $NopResources:Admin.CountryInfo.DisplayOrder.RequiredErrorMessage %>"
                RangeErrorMessage="<% $NopResources:Admin.CountryInfo.DisplayOrder.RangeErrorMessage %>"
                MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
        </td>
    </tr>
</table>
