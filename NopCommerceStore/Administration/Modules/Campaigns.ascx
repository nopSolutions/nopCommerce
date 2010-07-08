<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CampaignsControl"
    CodeBehind="Campaigns.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.Campaigns.Title")%>" />
        <%=GetLocaleResourceString("Admin.Campaigns.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='CampaignAdd.aspx'" value="<%=GetLocaleResourceString("Admin.Campaigns.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.Campaigns.AddNewButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvCampaigns" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Campaigns.Name %>" ItemStyle-Width="50%">
            <ItemTemplate>
                <%#Eval("Name")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Campaigns.CreatedOn %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Campaigns.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="40%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="CampaignDetails.aspx?CampaignID=<%#Eval("CampaignId")%>" title="<%#GetLocaleResourceString("Admin.Campaigns.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Campaigns.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
