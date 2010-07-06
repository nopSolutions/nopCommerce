<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProfileInfoControl"
    CodeBehind="ProfileInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="PrivateMessageSendButton" Src="~/Modules/PrivateMessageSendButton.ascx" %>
<div class="profileInfoBox">
    <div class="section-title">
        <%--        <%=GetLocaleResourceString("Profile.PersonalInfo")%>--%>
    </div>
    <div class="clear">
    </div>
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
<asp:PlaceHolder runat="server" ID="phLatestPosts">
    <div class="userLastPosts">
        <asp:Repeater runat="server" ID="rptrLatestPosts" OnItemDataBound="rptrLatestPosts_ItemDataBound">
            <HeaderTemplate>
                <div class="section-title">
                    <asp:Label runat="server" ID="lblLatestPostsTitle" Text="<% $NopResources:Profile.LatestPosts %>"></asp:Label>:
                </div>
            </HeaderTemplate>
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
        </asp:Repeater>
    </div>
</asp:PlaceHolder>
