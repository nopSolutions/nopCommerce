<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.AffiliateCustomersControl"
    CodeBehind="AffiliateCustomers.ascx.cs" %>
<table class="adminContent">
    <tr>
        <td colspan="2" width="100%">
            <asp:GridView ID="gvAffiliateCustomers" runat="server" AutoGenerateColumns="False"
                Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.AffiliateCustomers.Email %>"
                        ItemStyle-Width="45%">
                        <ItemTemplate>
                            <%#Server.HtmlEncode(Eval("Email").ToString())%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.AffiliateCustomers.Name %>"
                        ItemStyle-Width="45%">
                        <ItemTemplate>
                            <%#Server.HtmlEncode(Eval("FullName").ToString())%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.AffiliateCustomers.View %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <a href="CustomerDetails.aspx?CustomerID=<%#Eval("CustomerId")%>" title="<%#GetLocaleResourceString("Admin.AffiliateCustomers.View.Tooltip")%>">
                                <%#GetLocaleResourceString("Admin.AffiliateCustomers.View")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
</table>
