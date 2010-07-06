<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Tax.GeneralTaxConfigure.ConfigureTax"
    CodeBehind="ConfigureTax.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="TaxRates" Src="TaxRates.ascx" %>
<div>
    <table class="adminContent">
        <tr>
            <td>
                <asp:UpdatePanel ID="upTaxRates" runat="server">
                    <ContentTemplate>
                        <nopCommerce:TaxRates runat="server" ID="ctrlTaxRates"></nopCommerce:TaxRates>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upTaxRates">
                    <ProgressTemplate>
                        <div class="progress">
                            <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/UpdateProgress.gif" AlternateText="update" />
                            <%=GetLocaleResourceString("Admin.Common.Wait...")%>
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
        </tr>
    </table>
</div>
