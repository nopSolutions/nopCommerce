<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerInfoControl"
    CodeBehind="CustomerInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="EmailTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DatePicker" Src="DatePicker.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerEmailTitle" Text="<% $NopResources:Admin.CustomerInfo.Email %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Email.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:EmailTextBox runat="server" ID="txtEmail" CssClass="adminInput" />
        </td>
    </tr>    
    <tr runat="server" id="pnlUsername">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerUsernameTitle" Text="<% $NopResources:Admin.CustomerInfo.Username %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Username.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtUsername" CssClass="adminInput"
                ErrorMessage="<% $NopResources:Admin.CustomerInfo.Username.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
            <asp:Label runat="server" ID="lblUsername" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPasswordTitle" Text="<% $NopResources:Admin.CustomerInfo.Password %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Password.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtPassword" CssClass="adminInput" />
            <asp:Button runat="server" ID="btnChangePassword" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CustomerInfo.BtnChangePassword.Text %>"
                OnClick="BtnChangePassword_OnClick" />
        </td>
    </tr>
    <% if (CustomerManager.FormFieldGenderEnabled)
       { %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblGenderTitle" Text="<% $NopResources:Admin.CustomerInfo.Gender %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Gender.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:RadioButton runat="server" ID="rbGenderM" GroupName="Gender" Text="<% $NopResources:Admin.CustomerInfo.Gender.Male %>"
                Checked="true" />
            <asp:RadioButton runat="server" ID="rbGenderF" GroupName="Gender" Text="<% $NopResources:Admin.CustomerInfo.Gender.Female %>" />
        </td>
    </tr>
    <% } %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFirstNameTitle" Text="<% $NopResources:Admin.CustomerInfo.FirstName %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.FirstName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtFirstName" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLastNameTitle" Text="<% $NopResources:Admin.CustomerInfo.LastName %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.LastName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtLastName" CssClass="adminInput" />
        </td>
    </tr>
    <% if (CustomerManager.FormFieldDateOfBirthEnabled)
       { %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDateOfBirthTitle" Text="<% $NopResources:Admin.CustomerInfo.DateOfBirth %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.DateOfBirth.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlDateOfBirthDatePicker" />
        </td>
    </tr>
    <% } %>
    <% if (CustomerManager.FormFieldCompanyEnabled)
       { %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCompanyTitle" Text="<% $NopResources:Admin.CustomerInfo.Company %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Company.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtCompany" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <% } %>    
    <% if (TaxManager.EUVatEnabled)
       { %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblVatNumberTitle" Text="<% $NopResources:Admin.CustomerInfo.VatNumber %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.VatNumber.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtVatNumber" runat="server" CssClass="adminInput" />&nbsp;&nbsp;&nbsp;<asp:Label
                ID="lblVatNumberStatus" runat="server" />
            &nbsp;&nbsp;&nbsp;<asp:Button runat="server" ID="btnMarkVatNumberAsValid" CssClass="adminButton"
                CausesValidation="false" Text="<% $NopResources:Admin.CustomerInfo.BtnMarkVatNumberAsValid.Text %>"
                OnClick="BtnMarkVatNumberAsValid_OnClick" />&nbsp;&nbsp;&nbsp;
            <asp:Button runat="server" ID="btnMarkVatNumberAsInvalid" CssClass="adminButton"
                CausesValidation="false" Text="<% $NopResources:Admin.CustomerInfo.BtnMarkVatNumberAsInvalid.Text %>"
                OnClick="BtnMarkVatNumberAsInvalid_OnClick" />
        </td>
    </tr>
    <% } %>
    <% if (CustomerManager.FormFieldStreetAddressEnabled)
       { %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblStreetAddressTitle" Text="<% $NopResources:Admin.CustomerInfo.Address %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Address.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtStreetAddress" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <% } %>
    <% if (CustomerManager.FormFieldStreetAddress2Enabled)
       { %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblStreetAddress2Title" Text="<% $NopResources:Admin.CustomerInfo.Address2 %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Address2.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtStreetAddress2" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <% } %>
    <% if (CustomerManager.FormFieldPostCodeEnabled)
       { %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblZipPostalCodeTitle" Text="<% $NopResources:Admin.CustomerInfo.Zip %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Zip.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtZipPostalCode" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <% } %>
    <% if (CustomerManager.FormFieldCityEnabled)
       { %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCityTitle" Text="<% $NopResources:Admin.CustomerInfo.City %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.City.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtCity" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <% } %>
    <% if (CustomerManager.FormFieldCountryEnabled)
       { %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCountryTitle" Text="<% $NopResources:Admin.CustomerInfo.Country %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Country.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlCountry" AutoPostBack="True" runat="server" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged">
            </asp:DropDownList>
        </td>
    </tr>
    <% } %>
    <% if (CustomerManager.FormFieldCountryEnabled && CustomerManager.FormFieldStateEnabled)
       { %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblStateProvinceTitle" Text="<% $NopResources:Admin.CustomerInfo.State %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.State.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlStateProvince" AutoPostBack="False" runat="server" Width="137px">
            </asp:DropDownList>
        </td>
    </tr>
    <% } %>
    <% if (CustomerManager.FormFieldPhoneEnabled)
       { %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTelephoneNumberTitle" Text="<% $NopResources:Admin.CustomerInfo.Phone %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Phone.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <% } %>
    <% if (CustomerManager.FormFieldFaxEnabled)
       { %>
    <tr runat="server" id="pnlFaxNumber">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFaxNumberTitle" Text="<% $NopResources:Admin.CustomerInfo.Fax %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Fax.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtFaxNumber" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <% } %>
    <% if (CustomerManager.FormFieldNewsletterEnabled)
       { %>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblNewsletterTitle" Text="<% $NopResources:Admin.CustomerInfo.Newsletter %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Newsletter.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbNewsletter" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <% } %>
    <tr runat="server" id="pnlTimeZone">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTimeZoneTitle" Text="<% $NopResources:Admin.CustomerInfo.TimeZone %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.TimeZone.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlTimeZone" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAffiliateTitle" Text="<% $NopResources:Admin.CustomerInfo.Affiliate %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Affiliate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlAffiliate" AutoPostBack="False" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblIsTaxExempt" Text="<% $NopResources:Admin.CustomerInfo.TaxExempt %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.TaxExempt.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsTaxExempt" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblIsAdminTitle" Text="<% $NopResources:Admin.CustomerInfo.Admin %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Admin.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsAdmin" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblIsForumModerator" Text="<% $NopResources:Admin.CustomerInfo.ForumModerator %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.ForumModerator.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsForumModerator" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAdminComment" Text="<% $NopResources:Admin.CustomerInfo.AdminComment %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.AdminComment.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtAdminComment" runat="server" TextMode="MultiLine" CssClass="adminInput"
                Height="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblActiveTitle" Text="<% $NopResources:Admin.CustomerInfo.Active %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.Active.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbActive" runat="server" Checked="true"></asp:CheckBox>
        </td>
    </tr>
    <tr runat="server" id="pnlRegistrationDate">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblRegistrationDateTitle" Text="<% $NopResources:Admin.CustomerInfo.RegistrationDate %>"
                ToolTip="<% $NopResources:Admin.CustomerInfo.RegistrationDate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblRegistrationDate" runat="server"></asp:Label>
        </td>
    </tr>
</table>
