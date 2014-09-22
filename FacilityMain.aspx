<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="FacilityMain.aspx.cs" Inherits="FacilityMain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="container">

        <h1><%= Facility.Name %></h1>
        <div class="spacer"></div>

        <!-- <a href="/Schedule.aspx?facilityId=<%= FacilityId %>" class="no-decoration"> -->
        <% if (HasService) { %>
        <a href="<%= ScheduleAddress %>" class="no-decoration">
            <div class="button button-red button-top">
                <%= Resources.LocalizedText.Schedule %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>
        <% } %>

        <!-- FacilityInfo -->
        <a href="FacilityInfo.aspx?<%= (FriskisService.IsLoggedIn && FriskisService.IsApp) ? "authenticated=true&" : "" %>facilityId=<%= FacilityId %>" class="no-decoration">
            <div class="button button-red button-<%= HasService ? "middle" : "top" %>">
                <%= Resources.LocalizedText.ContactInfo %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>

        <%-- Cache page for android  --%>
        <% if (FriskisService.Client == "android") { %>
            <!-- <iframe src="FacilityInfo.aspx?<%= (FriskisService.IsLoggedIn && FriskisService.IsApp) ? "authenticated=true&" : "" %>facilityId=<%= FacilityId %>" style="display:none"> </iframe> -->
        <% } %>
        
        <!-- FacilityNews -->
        <a href="/FacilityNews.aspx?<%= (FriskisService.IsLoggedIn && FriskisService.IsApp) ? "authenticated=true&" : "" %>facilityId=<%= FacilityId %>" class="no-decoration">
            <div class="button button-red button-middle">
                <%= Resources.LocalizedText.News %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>
        
        <%-- Cache page for android  --%>
        <% if (FriskisService.Client == "android") { %>
            <!-- <iframe src="/FacilityNews.aspx?<%= (FriskisService.IsLoggedIn && FriskisService.IsApp) ? "authenticated=true&" : "" %>facilityId=<%= FacilityId %>" style="display:none"> </iframe> -->
        <% } %>
        
        
        <!-- FacilityClasses -->
        <a href="/FacilityClasses.aspx?<%= (FriskisService.IsLoggedIn && FriskisService.IsApp) ? "authenticated=true&" : "" %>facilityId=<%= FacilityId %>" class="no-decoration">
            <div class="button button-red button-middle">
                <%= Resources.LocalizedText.OurClasses %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>

        <%-- Cache page for android  --%>
        <% if (FriskisService.Client == "android") { %>
            <!-- <iframe src="/FacilityClasses.aspx?<%= (FriskisService.IsLoggedIn && FriskisService.IsApp) ? "authenticated=true&" : "" %>facilityId=<%= FacilityId %>" style="display:none"> </iframe> -->
        <% } %>

        <%-- 
        <% if (FriskisService.IsLoggedIn) { %>
            <a href="/Logout.aspx" class="no-decoration">
                <div class="button button-red button-middle">
                    <%= Resources.LocalizedText.Logout %>
                    <img src="/images/button/button-red-arrow.png" />
                </div>
            </a>
        <% } else { %>
            <a href="/Login.aspx?facilityId=<%= FacilityId %>" class="no-decoration">
                <div class="button button-red button-middle">
                    <%= Resources.LocalizedText.Login %>
                    <img src="/images/button/button-red-arrow.png" />
                </div>
            </a>
        <% } %>
        --%>

        <a href="<%= MapAddress %>" class="no-decoration">
            <div class="button button-red button-bottom">
                <%= Resources.LocalizedText.Map %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>
        
        <%-- 
        <a href="/BecomeMember.aspx?facilityId=<%= FacilityId %>" class="no-decoration">
            <div class="button button-red button-bottom">
                <%= Resources.LocalizedText.BecomeMember %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>
        --%>

    </div>

</asp:Content>

