<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CustomerAccountActivationControl"
    CodeBehind="CustomerAccountActivation.ascx.cs" %>
<div class="account-activation-page">
    <div class="page-title">
        <h1><%=GetLocaleResourceString("Account.AccountActivation")%></h1>
    </div>
    <div class="clear">
    </div>
    <div class="body">
        <div runat="server">
            <strong>
                <asp:Literal runat="server" ID="lResult"></asp:Literal>
            </strong>
        </div>
    </div>
</div>
