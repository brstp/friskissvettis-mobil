<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="UnbookingDone.aspx.cs" Inherits="loggedin_UnbookingDone" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../css/common-schedule-item.css" rel="stylesheet" type="text/css" momasdk="true" />
    <link href="../css/schedule.css" rel="stylesheet" type="text/css" momasdk="true" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="container">
    
        <h1><%= Resources.LocalizedText.UnbookingDone %></h1>
        
        <div class="spacer" />
        <div class="line" />
        <div class="spacer" />

        <div class="text-normal text-gray">
            <%= Item.From.ToString("dd")%>/<%= Item.From.ToString("MM yyyy")%> <br />
            <%= Item.From.ToString("H:mm")%> - <%= Item.To.ToString("H:mm")%> <br />
            <%= GetName(Item) %> 
            <%= (string.IsNullOrEmpty(Item.Room) ? "" : "<br />" + Resources.LocalizedText.Room + ":") %> <%= Item.Room %>
        </div>

        <div class="spacer" />

        <a class="no-decoration" href="MyBookings.aspx">
            <div class="button button-red">
                <%= Resources.LocalizedText.MyPages %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>

    </div>

</asp:Content>

