<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="FacilityInfo.aspx.cs" Inherits="FacilityInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/facilityinfo.css" rel="stylesheet" type="text/css" momasdk="true" />
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?sensor=false"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="container">
    
        <h1><%= Facility.Name %></h1>
        
        <div class="spacer" />
        <div class="line" />
        <div class="spacer" />
    
        <% if (!string.IsNullOrWhiteSpace(Facility.Phone)) { %>
        <a class="no-decoration" href="tel:<%= Facility.Phone %>">
            <div class="button button-red">
                <%= Resources.LocalizedText.CallUs %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>
        
        <div class="spacer" />
        <% } %>

        <% if (!string.IsNullOrEmpty(Facility.Email))
           {%>
            <a class="no-decoration" href="mailto:<%= Facility.Email %>">
                <div class="button button-red">
                    <%= Resources.LocalizedText.SendEmail%>
                    <img src="/images/button/button-red-arrow.png" />
                </div>
            </a>
        <% } %>
        
        <div class="spacer" />

        <!-- <a href="/Map.aspx?id=<%= FacilityId %>" class="no-decoration"> -->
        <a href="<%= MapAddress %>" class="no-decoration">
            <div class="button button-red">
                <%= Resources.LocalizedText.Map %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>

        <div class="spacer" />
        <div class="line" />
        <div class="spacer" />

        <div class="facility-info">
            <%= Facility.HtmlInfo %>
        </div>

        <div class="spacer" />
        <div class="line" />
        <div class="spacer" />

        <!-- MAP 
        <div id="map_canvas" style="width:100%; height:80DW%;"></div> -->

        <script type="text/javascript">

//            function initialize() {
//                var latlng = new google.maps.LatLng(<%= Facility.Latitude %>, <%= Facility.Longitude %>);
//                
//                var myOptions = {
//                    zoom: 13,
//                    center: latlng,
//                    mapTypeId: google.maps.MapTypeId.ROADMAP
//                };

//                var map = new google.maps.Map(document.getElementById("map_canvas"),myOptions);

//                marker = new google.maps.Marker({
//                    title: '<%= Facility.Name %>',
//                    map: map,
//                    icon: null,
//                    position: new google.maps.LatLng(<%= Facility.Latitude %>, <%= Facility.Longitude %>),
//                    clickable: false
//                });
//            }

//            window.onload = initialize;

        </script>

        <!-- <img id="map" src="images/dummys/map.png" /> -->

    </div>

</asp:Content>

