<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CountriesControl"
    CodeBehind="Countries.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.Countries.Title")%>" />
        <%=GetLocaleResourceString("Admin.Countries.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='CountryAdd.aspx'" value="<%=GetLocaleResourceString("Admin.Countries.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.Countries.AddNewButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvCountries" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Countries.Name %>" ItemStyle-Width="20%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="AllowsRegistration" HeaderText="<% $NopResources:Admin.Countries.AllowsRegistration %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="AllowsBilling" HeaderText="<% $NopResources:Admin.Countries.AllowsBilling %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="AllowsShipping" HeaderText="<% $NopResources:Admin.Countries.AllowsShipping %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="TwoLetterIsoCode" HeaderText="<% $NopResources:Admin.Countries.TwoLetterISOCode %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="ThreeLetterIsoCode" HeaderText="<% $NopResources:Admin.Countries.ThreeLetterISOCode %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="NumericIsoCode" HeaderText="<% $NopResources:Admin.Countries.NumericISOCode %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="SubjectToVAT" HeaderText="<% $NopResources:Admin.Countries.SubjectToVAT %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="DisplayOrder" HeaderText="<% $NopResources:Admin.Countries.DisplayOrder %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Countries.Published %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:ImageCheckBox runat="server" ID="cbPublished" Checked='<%# Eval("Published") %>'>
                </nopCommerce:ImageCheckBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Countries.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="CountryDetails.aspx?CountryID=<%#Eval("CountryId")%>" title="<%#GetLocaleResourceString("Admin.Countries.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Countries.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
