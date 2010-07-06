<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Measures.ascx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.MeasuresControl" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.Measures.Title")%>" />
        <%=GetLocaleResourceString("Admin.Measures.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.Measures.SaveButton.Text %>"
            CssClass="adminButtonBlue" ID="btnSave" OnClick="btnSave_Click" />
        <asp:Button ID="btnAddDimension" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Measures.AddDimension.Text %>"
            OnClick="btnAddDimension_Click" ToolTip="<% $NopResources:Admin.Measures.AddDimension.Tooltip %>" />
        <asp:Button ID="btnAddWeight" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Measures.AddWeight.Text %>"
            OnClick="btnAddWeight_Click" ToolTip="<% $NopResources:Admin.Measures.AddWeight.Tooltip %>" />
    </div>
</div>
<p>
</p>
<ajaxToolkit:TabContainer runat="server" ID="MeasuresTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlDimensions" HeaderText="<% $NopResources:Admin.Measures.Dimensions.Title %>">
        <ContentTemplate>
            <asp:GridView ID="gvDimensions" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.Measures.Dimensions.Name %>"
                        ItemStyle-Width="55%">
                        <ItemTemplate>
                            <a href="MeasureDimensionDetails.aspx?MeasureDimensionID=<%#Eval("MeasureDimensionId")%>"
                                title="<%#GetLocaleResourceString("Admin.Measures.Dimensions.Name.Edit")%>">
                                <%#Server.HtmlEncode(Eval("Name").ToString())%>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Ratio" HeaderText="<% $NopResources:Admin.Measures.Dimensions.Ratio %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                    </asp:BoundField>
                    <asp:BoundField DataField="DisplayOrder" HeaderText="<% $NopResources:Admin.Measures.Dimensions.DisplayOrder %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.Measures.Dimensions.PrimaryDimension %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <nopCommerce:GlobalRadioButton runat="server" ID="rdbIsPrimaryDimension" Checked='<%#Eval("IsPrimaryDimension")%>'
                                GroupName="PrimaryDimension" ToolTip="<% $NopResources:Admin.Measures.Dimensions.PrimaryDimension.Tooltip %>" />
                            <asp:HiddenField runat="server" ID="hfMeasureDimensionId" Value='<%#Eval("MeasureDimensionId")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlWeights" HeaderText="<% $NopResources:Admin.Measures.Weights.Title %>">
        <ContentTemplate>
            <asp:GridView ID="gvWeights" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.Measures.Weights.Name %>" ItemStyle-Width="55%">
                        <ItemTemplate>
                            <a href="MeasureWeightDetails.aspx?MeasureWeightID=<%#Eval("MeasureWeightId")%>"
                                title="<%#GetLocaleResourceString("Admin.Measures.Weights.Name.Edit")%>">
                                <%#Server.HtmlEncode(Eval("Name").ToString())%>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Ratio" HeaderText="<% $NopResources:Admin.Measures.Weights.Ratio %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                    </asp:BoundField>
                    <asp:BoundField DataField="DisplayOrder" HeaderText="<% $NopResources:Admin.Measures.Weights.DisplayOrder %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.Measures.Weights.PrimaryWeight %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <nopCommerce:GlobalRadioButton runat="server" ID="rdbIsPrimaryWeight" Checked='<%#Eval("IsPrimaryWeight")%>'
                                GroupName="PrimaryWeight" ToolTip="<% $NopResources:Admin.Measures.Weights.PrimaryWeight.Tooltip %>" />
                            <asp:HiddenField runat="server" ID="hfMeasureWeightId" Value='<%#Eval("MeasureWeightId")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>