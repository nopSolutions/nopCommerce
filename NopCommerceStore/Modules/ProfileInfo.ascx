<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProfileInfoControl"
    CodeBehind="ProfileInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="PrivateMessageSendButton" Src="~/Modules/PrivateMessageSendButton.ascx" %>
<ajaxToolkit:ToolkitScriptManager runat="Server" EnableScriptGlobalization="true"
    EnableScriptLocalization="true" ID="sm1" ScriptMode="Release" CompositeScript-ScriptMode="Release" />
<ajaxToolkit:TabContainer runat="server" ID="ProfileTabs" ActiveTabIndex="0" CssClass="grey">
    <ajaxToolkit:TabPanel runat="server" ID="pnlInfo" HeaderText="<% $NopResources:Profile.PersonalInfo %>">
        <ContentTemplate>
            <div class="profileInfoBox">
                <div class="userDetails">
                    <asp:PlaceHolder runat="server" ID="phAvatar">
                        <div class="avatar">
                            <asp:Image ID="imgAvatar" runat="server" AlternateText="Avatar" CssClass="avatar-img" />
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="phFullName">
                        <div class="profileStat">
                            <%=GetLocaleResourceString("Profile.FullName")%>:
                            <asp:Label runat="server" ID="lblFullName" CssClass="profileStatValue" />
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="phLocation">
                        <div class="profileStat">
                            <%=GetLocaleResourceString("Profile.Country")%>:
                            <asp:Label runat="server" ID="lblCountry" CssClass="profileStatValue" />
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="phPM">
                        <div class="profileStat">
                            <nopCommerce:PrivateMessageSendButton runat="server" ID="btnSendPM" />
                        </div>
                    </asp:PlaceHolder>
                </div>
                <div class="userStats">
                    <div class="section-title">
                        <%=GetLocaleResourceString("Profile.Statistics")%>
                    </div>
                    <asp:PlaceHolder runat="server" ID="phTotalPosts">
                        <div class="profileStat">
                            <%=GetLocaleResourceString("Profile.TotalPosts")%>:
                            <asp:Label runat="server" ID="lblTotalPosts" CssClass="profileStatValue" />
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="phJoinDate">
                        <div class="profileStat">
                            <%=GetLocaleResourceString("Profile.JoinDate")%>:
                            <asp:Label runat="server" ID="lblJoinDate" CssClass="profileStatValue" />
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="phDateOfBirth">
                        <div class="profileStat">
                            <%=GetLocaleResourceString("Profile.DateOfBirth")%>:
                            <asp:Label runat="server" ID="lblDateOfBirth" CssClass="profileStatValue" />
                        </div>
                    </asp:PlaceHolder>
                </div>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlLatestPosts" HeaderText="<% $NopResources:Profile.LatestPosts %>">
        <ContentTemplate>
            <div class="userLastPosts">
                <div class="section-title">
                    <asp:GridView ID="gvLP" DataKeyNames="ForumPostId" runat="server" AllowPaging="True"
                        AutoGenerateColumns="False" CellPadding="4" DataSourceID="odsLP" GridLines="None"
                        PageSize="10" OnRowDataBound="gvLP_RowDataBound">
                        <Columns>
                            <asp:TemplateField ItemStyle-Width="25%">
                                <ItemTemplate>
                                    <div class="topic">
                                        <div class="topicTitle">
                                            <asp:Label runat="server" ID="lblTopicTitle" Text="<% $NopResources:Profile.Topic %>"></asp:Label>:
                                            <asp:HyperLink runat="server" ID="hlTopic"></asp:HyperLink>
                                        </div>
                                        <div class="topicBody">
                                            <asp:Label runat="server" ID="lblPost"></asp:Label>
                                        </div>
                                        <div class="topicData">
                                            <asp:Label runat="server" ID="lblPostedTitle" Text="<% $NopResources:Profile.PostedOn %>"></asp:Label>:
                                            <asp:Label runat="server" ID="lblPosted"></asp:Label></div>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                           <%#GetLocaleResourceString("Profile.LatestPosts.NoPosts")%>
                        </EmptyDataTemplate>
                        <PagerSettings PageButtonCount="20" Position="Bottom" FirstPageText="<% $NopResources:Pager.First %>"
                            LastPageText="<% $NopResources:Pager.Last %>" NextPageText="<% $NopResources:Pager.Next %>"
                            PreviousPageText="<% $NopResources:Pager.Previous %>" Mode="NumericFirstLast" />
                        <PagerStyle CssClass="latestpostsgridpagerstyle" />
                    </asp:GridView>
                </div>
            </div>
            <div>
                <asp:ObjectDataSource ID="odsLP" runat="server" SelectMethod="GetUserLatestPosts"
                    EnablePaging="true" TypeName="NopSolutions.NopCommerce.Web.ForumHelper" StartRowIndexParameterName="StartIndex"
                    MaximumRowsParameterName="PageSize" SelectCountMethod="GetUserLatestPostCount">
                </asp:ObjectDataSource>
            </div>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
