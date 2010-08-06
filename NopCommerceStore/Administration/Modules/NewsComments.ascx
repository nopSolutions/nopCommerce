<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.NewsCommentsControl"
    CodeBehind="NewsComments.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.NewsComments.Title")%>" />
    <%=GetLocaleResourceString("Admin.NewsComments.Title")%>
</div>
<asp:GridView ID="gvNewsComments" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvNewsComments_PageIndexChanging" AllowPaging="true" PageSize="10">
    <Columns>
        <asp:TemplateField ItemStyle-Width="100%" ItemStyle-HorizontalAlign="Left">
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
                    ToolTip="<% $NopResources:Admin.NewsComments.EditButton.Tooltip %>" CommandName="EditItem"
                    OnCommand="btnEditNewsComment_Click" CommandArgument='<%#Eval("NewsCommentId")%>' />
                <asp:Button runat="server" ID="btnDeleteNewsComment" CssClass="adminButton" Text="<% $NopResources:Admin.NewsComments.Delete.Text %>"
                    ToolTip="<% $NopResources:Admin.NewsComments.Delete.Tooltip %>" CommandName="DeleteItem"
                    OnCommand="btnDeleteNewsComment_Click" CommandArgument='<%#Eval("NewsCommentId")%>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
</asp:GridView>