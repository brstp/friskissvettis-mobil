<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AboutUs.aspx.cs" Inherits="AboutUs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="container">
    

        <% if (Request.QueryString["facilityId"] != null) { %>
           
           <%= AboutUsText %>

        <% } else { %>

            <h1><%= Resources.LocalizedText.IdeaWithoutDemand%></h1>
        
            <div class="spacer" />
            <div class="line" />
            <div class="spacer" />

            <div class="text-normal text-gray">
                <%= AboutUsText %>
            </div>

        <% } %>
    </div>

</asp:Content>

