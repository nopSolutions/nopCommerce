<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ContactUsControl"
    CodeBehind="ContactUs.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="~/Modules/SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="~/Modules/EmailTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<div class="contact-form">
    <div>
        <nopCommerce:Topic ID="topicContactUs" runat="server" TopicName="ContactUs" OverrideSEO="false">
        </nopCommerce:Topic>
    </div>
    <div class="clear">
    </div>
    <asp:Panel runat="server" ID="pnlResult" Visible="false" CssClass="result">
        <strong>
            <%=GetLocaleResourceString("ContactUs.YourEnquiryHasBeenSent")%></strong>
    </asp:Panel>
    <div class="clear">
    </div>
    <div class="send-email" runat="server" id="pnlContactUs">
        <table class="table-container">
            <tr class="row">
                <td class="item-name">
                    <%=GetLocaleResourceString("ContactUs.FullName")%>:
                </td>
                <td class="item-value">
                    <nopCommerce:SimpleTextBox runat="server" ID="txtFullName" ValidationGroup="ContactUs"
                        Width="250px"></nopCommerce:SimpleTextBox>
                </td>
            </tr>
            <tr class="row">
                <td class="item-name">
                    <%=GetLocaleResourceString("ContactUs.E-MailAddress")%>:
                </td>
                <td class="item-value">
                    <nopCommerce:EmailTextBox runat="server" ID="txtEmail" ValidationGroup="ContactUs"
                        Width="250px"></nopCommerce:EmailTextBox>
                </td>
            </tr>
            <tr class="row">
                <td class="item-name">
                    <%=GetLocaleResourceString("ContactUs.Enquiry")%>:
                </td>
                <td class="item-value">
                    <asp:TextBox runat="server" ID="txtEnquiry" TextMode="MultiLine" SkinID="ContactUsEnquiryText"></asp:TextBox>
                </td>
            </tr>
            <tr class="row">
                <td class="item-name">
                </td>
                <td class="button">
                    <asp:Button runat="server" ID="btnContactUs" Text="<% $NopResources:ContactUs.ContactUsButton %>"
                        ValidationGroup="ContactUs" OnClick="btnContactUs_Click" CssClass="contactusbutton">
                    </asp:Button>
                </td>
            </tr>
        </table>
    </div>
</div>
