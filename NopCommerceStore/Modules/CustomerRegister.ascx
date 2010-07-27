<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CustomerRegisterControl" CodeBehind="CustomerRegister.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="Captcha" Src="~/Modules/Captcha.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="Topic" Src="~/Modules/Topic.ascx" %>

<div class="registration-page">
    <div class="page-title">
        <h1><%=GetLocaleResourceString("Account.Registration")%></h1>
    </div>
    <div class="clear">
    </div>
    <div class="body">
        <asp:CreateUserWizard ID="CreateUserForm" EmailRegularExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+"
            RequireEmail="False" runat="server" OnCreatedUser="CreatedUser" OnCreatingUser="CreatingUser"
            OnCreateUserError="CreateUserError" FinishDestinationPageUrl="~/default.aspx"
            ContinueDestinationPageUrl="~/default.aspx" Width="100%" LoginCreatedUser="true">
            <WizardSteps> 
                <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server" Title="">
                    <ContentTemplate>
                        <div class="message-error">
                            <div>
                                <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
                            </div>
                            <div class="clear">
                            </div>
                            <div>
                                <asp:ValidationSummary ID="valSum" runat="server" ShowSummary="true" DisplayMode="BulletList"
                                    ValidationGroup="CreateUserForm" />
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                        <div class="section-title">
                            <%=GetLocaleResourceString("Account.YourPersonalDetails")%>
                        </div>
                        <div class="clear">
                        </div>
                        <div class="section-body">
                            <table class="table-container">
                                <tbody>
                                <asp:PlaceHolder runat="server" ID="phGender">
                                    <tr class="row">
                                        <td class="item-name">
                                            <%=GetLocaleResourceString("Account.Gender")%>:
                                        </td>
                                        <td class="item-value">
                                            <asp:RadioButton runat="server" ID="rbGenderM" GroupName="Gender" Text="<% $NopResources:Account.GenderMale %>"
                                                Checked="true" />
                                            <asp:RadioButton runat="server" ID="rbGenderF" GroupName="Gender" Text="<% $NopResources:Account.GenderFemale %>" />
                                        </td>
                                    </tr>
                                     </asp:PlaceHolder>
                                    <tr class="row">
                                        <td class="item-name">
                                            <%=GetLocaleResourceString("Account.FirstName")%>:
                                        </td>
                                        <td class="item-value">
                                            <asp:TextBox ID="txtFirstName" runat="server" MaxLength="40"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName"
                                                ErrorMessage="<% $NopResources:Account.FirstNameIsRequired %>" ToolTip="<% $NopResources:Account.FirstNameIsRequired %>"
                                                ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr class="row">
                                        <td class="item-name">
                                            <%=GetLocaleResourceString("Account.LastName")%>:
                                        </td>
                                        <td class="item-value">
                                            <asp:TextBox ID="txtLastName" runat="server" MaxLength="40"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName"
                                                ErrorMessage="<% $NopResources:Account.LastNameIsRequired %>" ToolTip="<% $NopResources:Account.LastNameIsRequired %>"
                                                ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <asp:PlaceHolder runat="server" ID="phDateOfBirth">
                                    <tr class="row">
                                        <td class="item-name">
                                            <%=GetLocaleResourceString("Account.DateOfBirth")%>:
                                        </td>
                                        <td class="item-value">
                                            <nopCommerce:NopDatePicker runat="server" ID="dtDateOfBirth" DayText="<% $NopResources:DatePicker2.Day %>"
                                                MonthText="<% $NopResources:DatePicker2.Month %>" YearText="<% $NopResources:DatePicker2.Year %>" />
                                        </td>
                                    </tr>
                                     </asp:PlaceHolder>
                                    <%--pnlEmail is visible only when customers are authenticated by usernames and is used to get an email--%>
                                    <tr class="row" runat="server" id="pnlEmail">
                                        <td class="item-name">
                                            <%=GetLocaleResourceString("Account.E-Mail")%>:
                                        </td>
                                        <td class="item-value">
                                            <asp:TextBox ID="Email" runat="server" MaxLength="40"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                                ErrorMessage="Email is required" ToolTip="Email is required" Display="Dynamic"
                                                ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator runat="server" ID="revEmail" Display="Dynamic" ControlToValidate="Email"
                                                ErrorMessage="Invalid email" ToolTip="Invalid email" ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+"
                                                ValidationGroup="CreateUserForm">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <%--this table row is used to get an username when customers are authenticated by usernames--%>
                                    <%--this table row is used to get an email when customers are authenticated by emails--%>
                                    <tr class="row">
                                        <td class="item-name">
                                            <asp:Literal runat="server" ID="lUsernameOrEmail" Text="E-Mail" />:
                                        </td>
                                        <td class="item-value">
                                            <asp:TextBox ID="UserName" runat="server" MaxLength="40"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="UserNameOrEmailRequired" runat="server" ControlToValidate="UserName"
                                                ErrorMessage="Username is required" ToolTip="Username is required" Display="Dynamic"
                                                ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator runat="server" ID="refUserNameOrEmail" Display="Dynamic"
                                                ControlToValidate="UserName" ErrorMessage="Invalid email" ToolTip="Invalid email"
                                                ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+" ValidationGroup="CreateUserForm">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="clear">
                        </div>
                        <asp:PlaceHolder runat="server" ID="phCompanyDetails">
                            <div class="section-title">
                                <%=GetLocaleResourceString("Account.CompanyDetails")%>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="section-body">
                                <table class="table-container">
                                    <tbody>
                                        <asp:PlaceHolder runat="server" ID="phCompanyName">
                                            <tr class="row">
                                                <td class="item-name">
                                                    <%=GetLocaleResourceString("Account.CompanyName")%>:
                                                </td>
                                                <td class="item-value">
                                                    <asp:TextBox ID="txtCompany" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvCompany" runat="server" ControlToValidate="txtCompany"
                                                        ErrorMessage="<% $NopResources:Account.CompanyIsRequired %>" ToolTip="<% $NopResources:Account.CompanyIsRequired %>"
                                                        ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" ID="phVatNumber">
                                            <tr class="row">
                                                <td class="item-name">
                                                    <%=GetLocaleResourceString("Account.VATNumber")%>:
                                                </td>
                                                <td class="item-value">
                                                    <asp:TextBox ID="txtVatNumber" runat="server" />
                                                </td>
                                            </tr>
                                        </asp:PlaceHolder>
                                    </tbody>
                                </table>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="phYourAddress">
                            <div class="section-title">
                                <%=GetLocaleResourceString("Account.YourAddress")%>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="section-body">
                                <table class="table-container">
                                    <tbody>
                                        <asp:PlaceHolder runat="server" ID="phStreetAddress">
                                            <tr class="row">
                                                <td class="item-name">
                                                    <%=GetLocaleResourceString("Account.StreetAddress")%>:
                                                </td>
                                                <td class="item-value">
                                                    <asp:TextBox ID="txtStreetAddress" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvStreetAddress" runat="server" ControlToValidate="txtStreetAddress"
                                                        ErrorMessage="<% $NopResources:Account.StreetAddressIsRequired %>" ToolTip="<% $NopResources:Account.StreetAddressIsRequired %>"
                                                        ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" ID="phStreetAddress2">
                                            <tr class="row">
                                                <td class="item-name">
                                                    <%=GetLocaleResourceString("Account.StreetAddress2")%>:
                                                </td>
                                                <td class="item-value">
                                                    <asp:TextBox ID="txtStreetAddress2" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvStreetAddress2" runat="server" ControlToValidate="txtStreetAddress2"
                                                        ErrorMessage="<% $NopResources:Account.StreetAddress2IsRequired %>" ToolTip="<% $NopResources:Account.StreetAddress2IsRequired %>"
                                                        ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" ID="phPostCode">
                                            <tr class="row">
                                                <td class="item-name">
                                                    <%=GetLocaleResourceString("Account.PostCode")%>:
                                                </td>
                                                <td class="item-value">
                                                    <asp:TextBox ID="txtZipPostalCode" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvZipPostalCode" runat="server" ControlToValidate="txtZipPostalCode"
                                                        ErrorMessage="<% $NopResources:Account.ZipPostalCodeIsRequired %>" ToolTip="<% $NopResources:Account.ZipPostalCodeIsRequired %>"
                                                        ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" ID="phCity">
                                            <tr class="row">
                                                <td class="item-name">
                                                    <%=GetLocaleResourceString("Account.City")%>:
                                                </td>
                                                <td class="item-value">
                                                    <asp:TextBox ID="txtCity" runat="server" MaxLength="40"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="txtCity"
                                                        ErrorMessage="<% $NopResources:Account.CityIsRequired %>" ToolTip="<% $NopResources:Account.CityIsRequired %>"
                                                        ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" ID="phCountry">
                                            <tr class="row">
                                                <td class="item-name">
                                                    <%=GetLocaleResourceString("Account.Country")%>:
                                                </td>
                                                <td class="item-value">
                                                    <asp:DropDownList ID="ddlCountry" AutoPostBack="True" runat="server" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged"
                                                        Width="137px">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" ID="phStateProvince">
                                            <tr class="row">
                                                <td class="item-name">
                                                    <%=GetLocaleResourceString("Account.StateProvince")%>:
                                                </td>
                                                <td class="item-value">
                                                    <asp:DropDownList ID="ddlStateProvince" AutoPostBack="False" runat="server" Width="137px">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </asp:PlaceHolder>
                                    </tbody>
                                </table>
                            </div>
                            <div class="clear">
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="phYourContactInformation">
                            <div class="section-title">
                                <%=GetLocaleResourceString("Account.YourContactInformation")%>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="section-body">
                                <table class="table-container">
                                    <tbody>
                                        <asp:PlaceHolder runat="server" ID="phTelephoneNumber">
                                            <tr class="row">
                                                <td class="item-name">
                                                    <%=GetLocaleResourceString("Account.TelephoneNumber")%>:
                                                </td>
                                                <td class="item-value">
                                                    <asp:TextBox ID="txtPhoneNumber" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvPhoneNumber" runat="server" ControlToValidate="txtPhoneNumber"
                                                        ErrorMessage="<% $NopResources:Account.PhoneNumberIsRequired %>" ToolTip="<% $NopResources:Account.PhoneNumberIsRequired %>"
                                                        ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" ID="phFaxNumber">
                                            <tr class="row">
                                                <td class="item-name">
                                                    <%=GetLocaleResourceString("Account.FaxNumber")%>:
                                                </td>
                                                <td class="item-value">
                                                    <asp:TextBox ID="txtFaxNumber" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvFaxNumber" runat="server" ControlToValidate="txtFaxNumber"
                                                        ErrorMessage="<% $NopResources:Account.FaxIsRequired %>" ToolTip="<% $NopResources:Account.FaxIsRequired %>"
                                                        ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                        </asp:PlaceHolder>
                                    </tbody>
                                </table>
                            </div>
                            <div class="clear">
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="phNewsletter">
                            <div class="section-title">
                                <%=GetLocaleResourceString("Account.Options")%>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="section-body">
                                <table class="table-container">
                                    <tbody>
                                        <tr class="row">
                                            <td class="item-name">
                                                <%=GetLocaleResourceString("Account.Newsletter")%>:
                                            </td>
                                            <td class="item-value">
                                                <asp:CheckBox ID="cbNewsletter" runat="server" Checked="true"></asp:CheckBox>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="clear">
                            </div>
                        </asp:PlaceHolder>
                        <div class="section-title">
                            <%=GetLocaleResourceString("Account.YourPassword")%>
                        </div>
                        <div class="clear">
                        </div>
                        <div class="section-body">
                            <table class="table-container">
                                <tbody>
                                    <tr class="row">
                                        <td class="item-name">
                                            <%=GetLocaleResourceString("Account.Password")%>:
                                        </td>
                                        <td class="item-value">
                                            <asp:TextBox ID="Password" runat="server" MaxLength="50" TextMode="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                                ErrorMessage="<% $NopResources:Account.PasswordIsRequired %>" ToolTip="<% $NopResources:Account.PasswordIsRequired %>"
                                                ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr class="row">
                                        <td class="item-name">
                                            <%=GetLocaleResourceString("Account.PasswordConfirmation")%>:
                                        </td>
                                        <td class="item-value">
                                            <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword"
                                                ErrorMessage="<% $NopResources:Account.ConfirmPasswordIsRequired %>" ToolTip="<% $NopResources:Account.ConfirmPasswordIsRequired %>"
                                                ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                                                ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="<% $NopResources:Account.EnteredPasswordsDoNotMatch %>"
                                                ToolTip="<% $NopResources:Account.EnteredPasswordsDoNotMatch %>" ValidationGroup="CreateUserForm">*</asp:CompareValidator>
                                        </td>
                                    </tr>
                                    <tr class="row">
                                        <td colspan="2">
                                            <nopCommerce:Captcha ID="CaptchaCtrl" runat="server" />
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="clear">
                        </div>
                    </ContentTemplate>
                    <CustomNavigationTemplate>
                        <div class="button">
                            <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Text="<% $NopResources:Account.RegisterNextStepButton %>"
                                ValidationGroup="CreateUserForm" CssClass="registernextstepbutton" />
                        </div>
                    </CustomNavigationTemplate>
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                    <ContentTemplate>
                        <div class="section-body">
                            <p>
                                <asp:Label runat="server" ID="lblCompleteStep"></asp:Label>
                            </p>
                            <asp:Button ID="ContinueButton" runat="server" CausesValidation="False" CommandName="Continue"
                                Text="<% $NopResources:Account.RegisterContinueButton %>" ValidationGroup="CreateUserForm"
                                CssClass="completeregistrationbutton" />
                        </div>
                        <div class="clear">
                        </div>
                    </ContentTemplate>
                </asp:CompleteWizardStep>
            </WizardSteps> 
        </asp:CreateUserWizard>
        <nopCommerce:Topic ID="topicRegistrationNotAllowed" runat="server" TopicName="RegistrationNotAllowed"
            OverrideSEO="false" Visible="false"></nopCommerce:Topic>
    </div>
</div>
