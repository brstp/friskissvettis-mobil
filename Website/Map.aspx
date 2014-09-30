<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Map.aspx.cs" Inherits="Map" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0;" /> 
    <title>Friskis & Svettis</title>
    <script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=true&amp;language=sv"></script>
    <link href="css/gmaps.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <noscript>
        <p><% = Resources.LocalizedText.PageRequiresJavascript%></p>
    </noscript>
    <form id="form1" runat="server">
        <div>
            <div id="map_canvas" class="out" style="width: 100%; height: 100%">
            </div>
            <div id="outputDiv" class="in" style="width: 100%; height: 100%">
            </div>
        </div>
    </form>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.6.4/jquery.min.js" type="text/javascript"></script>
    <script src="js/gmap.js" type="text/javascript" charset="UTF-8"></script>
    <script src="js/geo.js" type="text/javascript" charset="UTF-8"></script>    
</body>
</html>
