<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ConfirmUnbooking.aspx.cs" Inherits="loggedin_ConfirmUnbooking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../css/common-schedule-item.css" rel="stylesheet" type="text/css" momasdk="true" />
    <link href="../css/schedule.css" rel="stylesheet" type="text/css" momasdk="true" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="container">
        <h1 class="booking"><%= Resources.LocalizedText.DoYouWantToUnbook %></h1>
    </div>

    <div runat="server" visible="false" id="error" class="container container-error">
        
    </div>

    <div class="container container-dark">
        <div class="container-schedule container-workout-schedule">

            <table>
                <tr>
                    <td valign="middle"><img class="difficulty" src="/images/workout/dot-<%= Item.Level.ToString().ToLower() %>.png" /></td>
                    <td valign="middle"><h1><%= GetName(Item) %></h1></td>
                    <td valign="middle"></td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <div class="description">
                            <%= Item.From.ToString("dd")%>/<%= Item.From.ToString("MM yyyy, H:mm")%> - <%= Item.To.ToString("H:mm")%> <br />
                            <%= Item.Where.Name %>
                        </div>
                    </td>
                </tr>
            </table>

            <div class="spots">
                <%= SpotsText() %>
            </div>

        </div>
    </div>

    <div class="container">
<%--
        <a class="no-decoration" href="?m=unbook&id=<%= Item.Id %>&bookid=<%= BookId %>&date=<%= Item.From.ToShortDateString() %>">
            <div class="button button-red button-login">
                <%= Resources.LocalizedText.YesCancelBooking %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>--%>
    
        <asp:LinkButton runat="server" ID="btnUnbook" OnClick="btnUnbook_OnClick">
            <div class="button button-red button-login">
                <%= Resources.LocalizedText.YesCancelBooking%>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </asp:LinkButton>

        <div class="spacer" />

        <a class="no-decoration" href="/loggedin/MyBookings.aspx">
            <div class="button button-red">
                <%= Resources.LocalizedText.NoCancelBooking %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>

    </div>

</asp:Content>

