<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CustomerRewardPointsControl"
    CodeBehind="CustomerRewardPoints.ascx.cs" %>
<div class="customer-rewardpoints">
    <div class="section-title">
        <%=GetLocaleResourceString("Customer.RewardPoints.Overview")%>
    </div>
    <div class="clear">
    </div>
    <div class="reward-points-overview">
        <asp:Label runat="server" ID="lblBalance" EnableViewState="false"></asp:Label>
        <br />
        <asp:Label runat="server" ID="lblRate" EnableViewState="false"></asp:Label>
    </div>
    <div class="clear">
    </div>
    <div class="section-title">
        <%=GetLocaleResourceString("Customer.RewardPoints.History")%>
    </div>
    <div class="clear">
    </div>
    <div class="reward-points-history">
        <asp:GridView ID="gvRewardPoints" runat="server" AutoGenerateColumns="False" Width="100%"
            OnPageIndexChanging="gvRewardPoints_PageIndexChanging" AllowPaging="true"
            PageSize="15" EnableViewState="false">
            <Columns>
                <asp:TemplateField HeaderText="<% $NopResources:Customer.RewardPoints.Grid.Date %>"
                    ItemStyle-Width="20%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:Customer.RewardPoints.Grid.Points %>"
                    ItemStyle-Width="20%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%#Server.HtmlEncode(Eval("Points").ToString())%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:Customer.RewardPoints.Grid.Balance %>"
                    ItemStyle-Width="20%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%#Server.HtmlEncode(Eval("PointsBalance").ToString())%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:Customer.RewardPoints.Grid.Message %>"
                    ItemStyle-Width="40%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%#Server.HtmlEncode(Eval("Message").ToString())%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Label runat="server" ID="lblHistoryMessage" EnableViewState="false" />
    </div>
</div>
