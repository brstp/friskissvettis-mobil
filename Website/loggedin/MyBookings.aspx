<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MyBookings.aspx.cs" Inherits="loggedin_MyBookings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../css/common-schedule-item.css" rel="stylesheet" type="text/css" momasdk="true" />
    <link href="../css/mybookings.css" rel="stylesheet" type="text/css" momasdk="true" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <div runat="server" id="divSuceessMessage" visible="false" class="container container-success">
        <asp:Label runat="server" ID="lblSuccessMessage"></asp:Label>
    </div>

    <div class="container">

        <h1 class="booking"><%= Resources.LocalizedText.MyBookings %></h1>

        <div class="line" />
        <a href="/Schedule.aspx" class="no-decoration">
            <div class="button button-red booking">
                <%= Resources.LocalizedText.NewBooking %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>
        <a href="#" class="no-decoration" onclick="javascript: document.location.reload();">
            <div class="button button-red booking">
                <%= Resources.LocalizedText.UpdateSchedule %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>

    </div>

    <div class="container container-dark">

        <div runat="server" class="text-normal text-white" id="divNoBookings" visible="false">
            <%= Resources.LocalizedText.NoBookingsCouldBeFound %>
        </div>

        <asp:Repeater runat="server" ID="rptrBookings">
            <ItemTemplate>
                <div class="container-schedule">
                    <h1><%# GetName((ScheduleItem)Container.DataItem).ToString().Replace("/", "/&#8203;") %></h1>

                    <div class="description">
                        <%# GetDescription((ScheduleItem)Container.DataItem) %>
                    </div>

                    <%-- Only show if it's a app  --%>
                    <% if (FriskisService.IsApp) { %>
                    <br />
                    <a class="no-decoration button-book" href="<%# GetAddToPhoneCalendar((ScheduleItem)Container.DataItem) %>">
                        <div class="button button-red">
                            <span class="text"><%= Resources.LocalizedText.AddToPhoneCalendar %></span>
                            <img class="arrow" src="/images/button/button-red-arrow.png" />
                        </div>
                    </a>
                    <% } %>

                    <br />
                    <a class="no-decoration button-book" href="<%# GetUnbookLink((ScheduleItem)Container.DataItem) %>">
                        <div class="button button-red">
                            <span class="text"><%= Resources.LocalizedText.Unbook %></span>
                            <img class="arrow" alt="icon" src="/images/button/button-red-arrow.png" />
                        </div>
                    </a>
                </div>
            </ItemTemplate>
        </asp:Repeater>

    </div>
</asp:Content>

