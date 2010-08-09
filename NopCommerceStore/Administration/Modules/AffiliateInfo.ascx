<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.AffiliateInfoControl"
    CodeBehind="AffiliateInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="EmailTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<table class="adminContent">
    <tr runat="server" id="pnlAffiliateId">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAffiliateIdTitle" Text="<% $NopResources:Admin.AffiliateInfo.AffiliateID %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.AffiliateID.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblAffiliateId" runat="server"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="pnlAffiliateUrl">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAffiliateUrl" Text="<% $NopResources:Admin.AffiliateInfo.AffiliateURL %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.AffiliateURL.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:HyperLink Target="_blank" ID="hlAffiliateUrl" runat="server"></asp:HyperLink>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFirstName" Text="<% $NopResources:Admin.AffiliateInfo.FirstName %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.FirstName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtFirstName" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLastName" Text="<% $NopResources:Admin.AffiliateInfo.LastName %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.LastName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtLastName" CssClass="adminInput"
                ErrorMessage="<% $NopResources:Admin.AffiliateInfo.LastName.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMiddleName" Text="<% $NopResources:Admin.AffiliateInfo.MiddleName %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.MiddleName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtMiddleName" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPhoneNumber" Text="<% $NopResources:Admin.AffiliateInfo.PhoneNumber %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.PhoneNumber.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblEmail" Text="<% $NopResources:Admin.AffiliateInfo.Email %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.Email.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:EmailTextBox runat="server" ID="txtEmail" CssClass="adminInput"></nopCommerce:EmailTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFaxNumber" Text="<% $NopResources:Admin.AffiliateInfo.FaxNumber %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.FaxNumber.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtFaxNumber" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCompany" Text="<% $NopResources:Admin.AffiliateInfo.Company %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.Company.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtCompany" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAddress1" Text="<% $NopResources:Admin.AffiliateInfo.Address1 %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.Address1.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtAddress1" CssClass="adminInput"
                ErrorMessage="<% $NopResources:Admin.AffiliateInfo.Address1.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAddress2" Text="<% $NopResources:Admin.AffiliateInfo.Address2 %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.Address2.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtAddress2" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCity" Text="<% $NopResources:Admin.AffiliateInfo.City %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.City.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtCity" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.AffiliateInfo.City.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblStateProvince" Text="<% $NopResources:Admin.AffiliateInfo.StateProvince %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.StateProvince.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtStateProvince" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblZipPostalCode" Text="<% $NopResources:Admin.AffiliateInfo.Zip %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.Zip.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtZipPostalCode" CssClass="adminInput"
                ErrorMessage="<% $NopResources:Admin.AffiliateInfo.Zip.ErrorMessage %>"></nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCountry" Text="<% $NopResources:Admin.AffiliateInfo.Country %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.Country.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlCountry" AutoPostBack="False" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblActive" Text="<% $NopResources:Admin.AffiliateInfo.Active %>"
                ToolTip="<% $NopResources:Admin.AffiliateInfo.Active.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbActive" runat="server"></asp:CheckBox>
        </td>
    </tr>
</table>
