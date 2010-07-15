<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewsLetterSubscriptionBoxControl.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Modules.NewsLetterSubscriptionBoxControl" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="EmailTextBox.ascx" %>
<div class="block block-newsletter">
    <div class="title">
        <%=GetLocaleResourceString("NewsLetterSubscriptionBox.Info")%>
    </div>
    <div class="clear">
    </div>
    <div class="listbox">
        <div runat="server" id="pnlSubscribe">
            <asp:Label runat="server" Text="<% $NopResources:NewsLetterSubscriptionBox.EmailInput %>" />:
            <nopCommerce:EmailTextBox runat="server" ID="txtEmail" ValidationGroup="NewsLetterSubscriptionValidation" />
            <div class="options">
                <asp:RadioButton runat="server" ID="rbSubscribe" Text="<% $NopResources:NewsLetterSubscriptionBox.Subscribe %>"
                    Checked="true" ValidationGroup="NewsLetterSubscriptionValidation" GroupName="NewsLetterSubscription" />
                <asp:RadioButton runat="server" ID="rbUnsubscribe" Text="<% $NopResources:NewsLetterSubscriptionBox.Unsubscribe %>"
                    ValidationGroup="NewsLetterSubscriptionValidation" GroupName="NewsLetterSubscription" />
            </div>
            <div class="clear">
            </div>
            <div class="buttons">
                <asp:Button runat="server" ID="btnSubscribe" Text="<% $NopResources:NewsLetterSubscriptionBox.BtnSubscribe.Text %>"
                    ValidationGroup="NewsLetterSubscriptionValidation" OnClick="BtnSubscribe_OnClick" CssClass="newsletterbox-subscribebutton"  />
            </div>
        </div>
        <div runat="server" id="pnlResult" visible="false">
            <asp:Label runat="server" ID="lblResult" />
        </div>
    </div>
</div>
