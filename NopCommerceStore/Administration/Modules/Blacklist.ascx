<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Blacklist.ascx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlacklistControl" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-blacklist.png" alt="<%=GetLocaleResourceString("Admin.Blacklist.Title")%>" />
        <%=GetLocaleResourceString("Admin.Blacklist.Title")%>
    </div>
    <div class="options">
        <asp:Button ID="btnAddBannedIP" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Blacklist.AddBannedIP.Text %>"
            OnClick="btnAddBannedIP_Click" ToolTip="<% $NopResources:Admin.Blacklist.AddBannedIP.Tooltip %>" />
        <asp:Button ID="btnAddBannedNetwork" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Blacklist.AddBannedNetwork.Text %>"
            OnClick="btnAddBannedNetwork_Click" ToolTip="<% $NopResources:Admin.Blacklist.AddBannedNetwork.Tooltip %>" />
    </div>
</div>
<p>
</p>
<ajaxToolkit:TabContainer runat="server" ID="BlacklistTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlIpAddress" HeaderText="<% $NopResources:Admin.Blacklist.IpAddress.Title %>">
        <ContentTemplate>
            <asp:GridView ID="gvBannedIpAddress" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.Blacklist.IpAddress.IPAddress %>"
                        ItemStyle-Width="20%">
                        <ItemTemplate>
                            <a href="BlacklistIPDetails.aspx?BannedIpAddressID=<%#Eval("BannedIpAddressId")%>"
                                title="<%#GetLocaleResourceString("Admin.Blacklist.IpAddress.IPAddress.Edit")%>">
                                <%#Server.HtmlEncode(Eval("Address").ToString())%>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Comment" HeaderText="<% $NopResources:Admin.Blacklist.IpAddress.Comment %>"
                        ItemStyle-Width="40%"></asp:BoundField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.Blacklist.IpAddress.CreatedOn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.Blacklist.IpAddress.UpdatedOn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("UpdatedOn"), DateTimeKind.Utc).ToString()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlIpNetwork" HeaderText="<% $NopResources:Admin.Blacklist.IpNetwork.Title %>">
        <ContentTemplate>
            <asp:GridView ID="gvBannedIpNetwork" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.Blacklist.IpNetwork.IPRange %>"
                        ItemStyle-Width="20%">
                        <ItemTemplate>
                            <a href="BlacklistNetworkDetails.aspx?BannedIpNetworkID=<%#Eval("BannedIpNetworkId")%>"
                                title="<%#GetLocaleResourceString("Admin.Blacklist.IpNetwork.IPRange.Edit")%>">
                                <%#Server.HtmlEncode(Eval("StartAddress").ToString())%>-<%#Server.HtmlEncode(Eval("EndAddress").ToString())%>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Comment" HeaderText="<% $NopResources:Admin.Blacklist.IpNetwork.Comment %>"
                        ItemStyle-Width="20%"></asp:BoundField>
                    <asp:BoundField DataField="IpException" HeaderText="<% $NopResources:Admin.Blacklist.IpNetwork.Exceptions %>"
                        ItemStyle-Width="20%"></asp:BoundField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.Blacklist.IpNetwork.CreatedOn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.Blacklist.IpNetwork.UpdatedOn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("UpdatedOn"), DateTimeKind.Utc).ToString()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>