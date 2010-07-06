<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductWriteReview"
    CodeBehind="ProductWriteReview.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="~/Modules/SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="~/Modules/EmailTextBox.ascx" %>
<div class="write-product-review-box">
    <table>
        <tr>
            <td style="text-align: left; vertical-align: middle;" colspan="2">
                <strong>
                    <asp:Label runat="server" ID="lblLeaveYourReview"></asp:Label></strong>
            </td>
        </tr>
        <tr>
            <td style="width: 100px; text-align: left; vertical-align: middle;">
                <%=GetLocaleResourceString("Products.ReviewTitle")%>:
            </td>
            <td>
                <nopCommerce:SimpleTextBox runat="server" ID="txtProductReviewTitle" ValidationGroup="ProductReview"
                    Width="250px"></nopCommerce:SimpleTextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 100px; text-align: left; vertical-align: middle;">
                <%=GetLocaleResourceString("Products.ReviewText")%>:
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtProductReviewText" TextMode="MultiLine" Height="150px"
                    Width="250px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvProductReviewText" runat="server" ControlToValidate="txtProductReviewText"
                    ErrorMessage="<% $NopResources:Products.PleaseEnterReviewText %>" ToolTip="<% $NopResources:Products.PleaseEnterReviewText %>"
                    ValidationGroup="ProductReview">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td style="width: 100px; text-align: left; vertical-align: middle;">
                <%=GetLocaleResourceString("Products.WriteReview.Rating")%>:
            </td>
            <td>
                <table>
                    <tr>
                        <td>
                            <%=GetLocaleResourceString("Products.WriteReview.RatingBad")%>
                        </td>
                        <td>
                            <asp:RadioButtonList runat="server" ID="rblRating" RepeatDirection="Horizontal" RepeatLayout="Table"
                                ValidationGroup="ProductReview">
                            </asp:RadioButtonList>
                        </td>
                        <td width="50px;">
                            <%=GetLocaleResourceString("Products.WriteReview.RatingExcellent")%>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr runat="server" id="pnlError">
            <td class="message-error" colspan="2">
                <asp:Literal ID="lErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td style="text-align: left; vertical-align: middle;">
                <asp:Button runat="server" ID="btnReview" Text="<% $NopResources:Products.ReviewButton %>"
                    ValidationGroup="ProductReview" OnClick="btnReview_Click" CssClass="productwritereviewbutton"></asp:Button>
            </td>
        </tr>
    </table>
</div>
