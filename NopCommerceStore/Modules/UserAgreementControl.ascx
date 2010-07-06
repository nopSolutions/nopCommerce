<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserAgreementControl.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Modules.UserAgreementControl" %>

<script type="text/javascript">
    function getE(name) {
        if (document.getElementById)
            var elem = document.getElementById(name);
        else if (document.all)
            var elem = document.all[name];
        else if (document.layers)
            var elem = document.layers[name];
        return elem;
    }

    function toggleContinueButton() {
        getE('<%=btnContinue.ClientID %>').disabled = !getE('<%=cbIsAgree.ClientID %>').checked
    }
</script>

<div class="user-agreement-page">
    <div class="page-title">
        <h1><%=GetLocaleResourceString("Download.UserAgreement")%></h1>
    </div>
    <div class="clear">
    </div>
    <div class="page-body">
        <asp:Literal runat="server" ID="lblUserAgreementText" />
        <br />
        <br />
        <asp:CheckBox runat="server" ID="cbIsAgree" Text="<% $NopResources:UserAgreementControl.CbIsAgree.Text %>" />
        <br />
        <br />
        <asp:Button runat="server" ID="btnContinue" Enabled="false" OnClick="BtnContinue_OnClick"
            Text="<% $NopResources:UserAgreementControl.BtnContinue.Text %>" CssClass="useragreementbutton" />
    </div>
</div>
