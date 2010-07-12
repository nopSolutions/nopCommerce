<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CustomerForumSubscriptionsControl"
    CodeBehind="CustomerForumSubscriptions.ascx.cs" %>
<div class="forum-subscriptions-box">
    <div class="forum-subscriptions">
        <asp:GridView runat="server" ID="gvForumSubscriptions" OnPageIndexChanging="gvForumSubscriptions_PageIndexChanging" AllowPaging="True" AutoGenerateColumns="False"
            CellPadding="4" PageSize="10" Width="100%">
            <Columns>
                <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerForumSubscriptions.DeleteColumn %>" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:CheckBox ID="cbSelect" runat="server" />
                        <asp:HiddenField ID="hfForumSubscriptionId" runat="server" Value='<%# Eval("ForumSubscriptionId") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerForumSubscriptions.InfoColumn %>"
                    ItemStyle-Width="90%" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%#GetInfo(Container.DataItem as ForumSubscription)%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>
