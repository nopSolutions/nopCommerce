<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ForumLastPostControl"
    CodeBehind="ForumLastPost.ascx.cs" %>
<asp:Panel class="LastPost" runat="server" ID="pnlPostInfo">
    <asp:Label ID="lblLastPostDate" runat="server" />
    <br />
    <%if (this.ShowTopic)
      { %>
    <%=GetLocaleResourceString("Forum.In")%>
    <asp:HyperLink ID="hlTopic" runat="server"></asp:HyperLink>
    <br />
    <%} %>
    <%=GetLocaleResourceString("Forum.By")%>
    <asp:HyperLink ID="hlUser" runat="server"></asp:HyperLink>
    <asp:Label ID="lblUser" runat="server" />
</asp:Panel>
<asp:Panel class="nopost" runat="server" ID="pnlNoPost">
    <%=GetLocaleResourceString("Forum.NoPosts")%>
</asp:Panel>
