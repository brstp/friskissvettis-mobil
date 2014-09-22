<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="FacilityMap.aspx.cs" Inherits="FacilityMap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0;" /> 
    <script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=true&amp;language=sv"></script>
    <link href="css/gmaps.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <iframe src="Map.aspx" width="100%" height="400px";></iframe>

</asp:Content>

