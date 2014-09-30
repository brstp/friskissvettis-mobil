<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="BecomeMember.aspx.cs" Inherits="BecomeMember" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="container">
    
        <h1><%= Facility.City %> - <%= Facility.Name %></h1>
        
        <div class="spacer" />
        <div class="line" />
        <div class="spacer" />

        <div class="text-normal text-gray become_member_text">
            <%= Resources.LocalizedText.BecomeMemberBody %>
        </div>

        <div class="spacer" />

        <%--  
            <a class="no-decoration" href="FacilityInfo.aspx?facilityId=<%= Facility.LocalId.ToString() %>">
                <div class="button button-red">
                    <%= Resources.LocalizedText.Contact %>
                    <img src="/images/button/button-red-arrow.png" />
                </div>
            </a>
        --%>

    </div>

</asp:Content>

