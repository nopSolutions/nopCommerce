<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CustomerForumSubscriptionsControl"
    CodeBehind="CustomerForumSubscriptions.ascx.cs" %>

<div>
    <table class="adminContent">
        <tr>
            <td>
                <asp:UpdatePanel ID="upForumSubscriptions" runat="server">
                    <ContentTemplate>
                        <asp:GridView runat="server" ID="gvForumSubscriptions" OnPageIndexChanging="gvForumSubscriptions_PageIndexChanging"
                            AllowPaging="true" AutoGenerateColumns="false" PageSize="10" Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerForumSubscriptions.DeleteColumn %>"
                                    ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbSelect" runat="server" />
                                        <asp:HiddenField ID="hfForumSubscriptionId" runat="server" Value='<%# Eval("ForumSubscriptionId") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerForumSubscriptions.InfoColumn %>"
                                    ItemStyle-Width="90%" HeaderStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <a href='<%#GetForumTopicLink(Container.DataItem as ForumSubscription)%>'>
                                            <%#GetForumTopicInfo(Container.DataItem as ForumSubscription)%>
                                        </a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <PagerSettings Position="Bottom" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upForumSubscriptions">
                    <ProgressTemplate>
                        <div class="progress">
                            <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/UpdateProgress.gif"
                                AlternateText="update" />
                            <%=GetLocaleResourceString("Admin.Common.Wait...")%>
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
        </tr>
    </table>
</div>
