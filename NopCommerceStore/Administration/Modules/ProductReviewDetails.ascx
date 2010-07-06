<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductReviewDetailsControl"
    CodeBehind="ProductReviewDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.ProductReviewDetails.EditProductReview")%>" />
        <%=GetLocaleResourceString("Admin.ProductReviewDetails.EditProductReview")%>
        <a href="ProductReviews.aspx" title="<%=GetLocaleResourceString("Admin.ProductReviewDetails.BackToProductReviews")%>">
            (<%=GetLocaleResourceString("Admin.ProductReviewDetails.BackToProductReviews")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ProductReviewDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ProductReviewDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ProductReviewDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.ProductReviewDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerName" Text="<% $NopResources:Admin.ProductReviewDetails.Customer %>"
                ToolTip="<% $NopResources:Admin.ProductReviewDetails.Customer.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCustomer" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblIPAddressTitle" Text="<% $NopResources:Admin.ProductReviewDetails.IPAddress %>"
                ToolTip="<% $NopResources:Admin.ProductReviewDetails.IPAddress.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblIPAddress" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblProductName" Text="<% $NopResources:Admin.ProductReviewDetails.Product %>"
                ToolTip="<% $NopResources:Admin.ProductReviewDetails.Product.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblProduct" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTitle" Text="<% $NopResources:Admin.ProductReviewDetails.Title %>"
                ToolTip="<% $NopResources:Admin.ProductReviewDetails.Title.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtTitle" ErrorMessage="<% $NopResources:Admin.ProductReviewDetails.Title.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblReviewText" Text="<% $NopResources:Admin.ProductReviewDetails.ReviewText %>"
                ToolTip="<% $NopResources:Admin.ProductReviewDetails.ReviewText.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtReviewText" TextMode="MultiLine" Height="150px"
                Width="500px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblRating" Text="<% $NopResources:Admin.ProductReviewDetails.Rating %>"
                ToolTip="<% $NopResources:Admin.ProductReviewDetails.Rating.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <ajaxToolkit:Rating ID="productRating" AutoPostBack="false" runat="server" MaxRating="5"
                StarCssClass="rating-star" WaitingStarCssClass="saved-rating-star" FilledStarCssClass="filled-rating-star"
                EmptyStarCssClass="empty-rating-star" ReadOnly="true" Style="float: left;" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblApproved" Text="<% $NopResources:Admin.ProductReviewDetails.Approved %>"
                ToolTip="<% $NopResources:Admin.ProductReviewDetails.Approved.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsApproved" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreated" Text="<% $NopResources:Admin.ProductReviewDetails.Created %>"
                ToolTip="<% $NopResources:Admin.ProductReviewDetails.Created.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCreatedOn" runat="server"></asp:Label>
        </td>
    </tr>
</table>
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
