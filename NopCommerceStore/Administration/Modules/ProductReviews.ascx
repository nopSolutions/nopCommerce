<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductReviewsControl"
    CodeBehind="ProductReviews.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.ProductReviews.ProductReviews")%>" />
    <%=GetLocaleResourceString("Admin.ProductReviews.ProductReviews")%>
</div>
<asp:DataPager ID="pagerProductReviews" runat="server" PagedControlID="lvProductReviews"
    PageSize="10">
    <Fields>
        <asp:NextPreviousPagerField ButtonType="Button" ShowFirstPageButton="True" ShowLastPageButton="True" />
    </Fields>
</asp:DataPager>
<asp:ListView ID="lvProductReviews" runat="server" DataKeyNames="ProductReviewId"
    OnPagePropertiesChanging="lvProductReviews_OnPagePropertiesChanging">
    <LayoutTemplate>
        <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
    </LayoutTemplate>
    <ItemTemplate>
        <p>
            <strong>
                <%#Server.HtmlEncode(Eval("Title").ToString())%></strong>
        </p>
        <p>
            <%#ProductManager.FormatProductReviewText((string)Eval("ReviewText"))%>
        </p>
        <p>
            <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
            -
            <%#GetCustomerInfo(Convert.ToInt32(Eval("CustomerId")))%>
            <%# string.Format(GetLocaleResourceString("Admin.ProductReviews.IPAddress"), Eval("IPAddress"))%>
        </p>
        <p>
            <b><a href="ProductDetails.aspx?ProductID=<%#Eval("ProductId")%>" title="<%=GetLocaleResourceString("Admin.ProductReviews.ViewProductDetails")%>">
                <%#Server.HtmlEncode(((Product)Eval("Product")).Name)%></a></b>
        </p>
        <p>
        </p>
        <asp:Button runat="server" ID="btnUpdateProductReview" CssClass="adminButton" Text='<%# Convert.ToBoolean(Eval("IsApproved"))?GetLocaleResourceString("Admin.ProductReviews.Disapprove") :GetLocaleResourceString("Admin.ProductReviews.Approve")%>'
            ToolTip='<%# Convert.ToBoolean(Eval("IsApproved"))?GetLocaleResourceString("Admin.ProductReviews.Disapprove.Tooltip") :GetLocaleResourceString("Admin.ProductReviews.Approve.Tooltip")%>'
            CommandName="Update" OnCommand="btnUpdateProductReview_Click" CommandArgument='<%#Eval("ProductReviewId")%>' />
        <p>
        </p>
        <asp:Button runat="server" ID="btnEditProductReview" CssClass="adminButton" Text="<% $NopResources:Admin.ProductReviews.Edit %>"
            ToolTip="<% $NopResources:Admin.ProductReviews.Edit.Tooltip %>" CommandName="Edit"
            OnCommand="btnEditProductReview_Click" CommandArgument='<%#Eval("ProductReviewId")%>' />
        <asp:Button runat="server" ID="btnDeleteProductReview" CssClass="adminButton" Text="<% $NopResources:Admin.ProductReviews.Delete %>"
            ToolTip="<% $NopResources:Admin.ProductReviews.Delete.Tooltip %>" CommandName="Delete"
            OnCommand="btnDeleteProductReview_Click" CommandArgument='<%#Eval("ProductReviewId")%>' />
    </ItemTemplate>
    <ItemSeparatorTemplate>
        <hr />
    </ItemSeparatorTemplate>
</asp:ListView>
