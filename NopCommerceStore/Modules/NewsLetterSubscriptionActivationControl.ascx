<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewsLetterSubscriptionActivationControl.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Modules.NewsLetterSubscriptionActivationControl" %>
<div class="newsletter-page">
    <div class="page-title">
        <h1><%=GetLocaleResourceString("NewsLetterSubscriptionActivation.Info")%></h1>
    </div>
    <div class="clear">
    </div>
    <div class="body">
        <div>
            <strong>
                <asp:Literal runat="server" ID="lblActivationResult" EnableViewState="false" />
            </strong>
        </div>
    </div>
</div>
