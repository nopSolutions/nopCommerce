<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ForumSubscriptionsControl"
    CodeBehind="ForumSubscriptions.ascx.cs" %>
<div class="forum-subscriptions-box">
    <div class="forum-subscriptions">
        <asp:GridView runat="server" ID="gvForumSubscriptions" DataKeyNames="ForumSubscriptionId" AllowPaging="True" AutoGenerateColumns="False"
            CellPadding="4" PageSize="10" CssClass="forum-subscriptions-grid" DataSourceID="odsForumSubscriptions" Width="100%">
            <Columns>
                <asp:TemplateField ItemStyle-Width="5%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:CheckBox ID="cbSelect" runat="server" />
                        <asp:HiddenField ID="hfForumSubscriptionId" runat="server" Value='<%# Eval("ForumSubscriptionId") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:ForumSubscriptions.InfoColumn %>" HeaderStyle-HorizontalAlign="Center"
                    ItemStyle-Width="90%">
                    <ItemTemplate>
                        <a href='<%#GetForumTopicLink(Container.DataItem as ForumSubscription)%>'>
                            <%#GetForumTopicInfo(Container.DataItem as ForumSubscription)%>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="odsForumSubscriptions" runat="server" SelectMethod="GetCurrentUserForumSubscriptions"
            EnablePaging="true" TypeName="NopSolutions.NopCommerce.Web.ForumHelper" StartRowIndexParameterName="StartIndex"
            MaximumRowsParameterName="PageSize" SelectCountMethod="GetCurrentUserForumSubscriptionsCount">
        </asp:ObjectDataSource>
    </div>
    <div class="clear">
    </div>
    <div class="button">
        <asp:Button runat="server" ID="btnDeleteSelected" Text="<% $NopResources:ForumSubscriptions.DeleteSelected %>"
            ValidationGroup="ForumSubscriptions" OnClick="btnDeleteSelected_Click" CssClass="deleteselectedfsbutton">
        </asp:Button>
    </div>
</div>
