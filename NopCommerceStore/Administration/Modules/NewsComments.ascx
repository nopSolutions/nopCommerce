<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.NewsCommentsControl"
    CodeBehind="NewsComments.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.NewsComments.Title")%>" />
    <%=GetLocaleResourceString("Admin.NewsComments.Title")%>
</div>
<asp:DataPager ID="pagerNewsComments" runat="server" PagedControlID="lvNewsComments"
    PageSize="10">
    <Fields>
        <asp:NextPreviousPagerField ButtonType="Button" ShowFirstPageButton="True" ShowLastPageButton="True" />
    </Fields>
</asp:DataPager>
<asp:ListView ID="lvNewsComments" runat="server" DataKeyNames="NewsCommentId" OnPagePropertiesChanging="lvNewsComments_OnPagePropertiesChanging">
    <LayoutTemplate>
        <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
    </LayoutTemplate>
    <ItemTemplate>
        <p>
            <strong>
                <%#Server.HtmlEncode((string)Eval("Title"))%>
            </strong>
        </p>
        <p>
            <%#NewsManager.FormatCommentText((string)Eval("Comment"))%>
        </p>
        <p>
            <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
            -
            <%#GetCustomerInfo(Convert.ToInt32(Eval("CustomerId")))%>
            <%# string.Format(GetLocaleResourceString("Admin.NewsComments.IPAddress"), Eval("IPAddress"))%>
        </p>
        <p>
            <a href="NewsDetails.aspx?NewsID=<%#Eval("NewsId")%>">
                <%#Server.HtmlEncode(((News)Eval("News")).Title)%></a>
        </p>
        <asp:Button runat="server" ID="btnEditNewsComment" CssClass="adminButton" Text="<% $NopResources:Admin.NewsComments.EditButton.Text %>"
            ToolTip="<% $NopResources:Admin.NewsComments.EditButton.Tooltip %>" CommandName="Edit"
            OnCommand="btnEditNewsComment_Click" CommandArgument='<%#Eval("NewsCommentId")%>' />
        <asp:Button runat="server" ID="btnDeleteNewsComment" CssClass="adminButton" Text="<% $NopResources:Admin.NewsComments.Delete.Text %>"
            ToolTip="<% $NopResources:Admin.NewsComments.Delete.Tooltip %>" CommandName="Delete"
            OnCommand="btnDeleteNewsComment_Click" CommandArgument='<%#Eval("NewsCommentId")%>' />
    </ItemTemplate>
    <ItemSeparatorTemplate>
        <hr />
    </ItemSeparatorTemplate>
</asp:ListView>
