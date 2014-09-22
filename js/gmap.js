/***************************************************************************************/
/*                       START OF PARAMETERS TO CUSTOMIZE THIS MAP                     */
/***************************************************************************************/

var custom_p_directions_color = "#ff0000";
var custom_p_handler_url = "/handler/MapPositionHandler.ashx";

/***************************************************************************************/
/*                         END OF PARAMETERS TO CUSTOMIZE THIS MAP                     */
/***************************************************************************************/

function getQueryString() {
    var result = {}, queryString = location.search.substring(1),
      re = /([^&=]+)=([^&]*)/g, m;

    while (m = re.exec(queryString)) {
        result[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
    }

    return result;
}

var map = null;
var bounds = new google.maps.LatLngBounds();
var boundItems = [];
var me = this;
me.stations = [];
var findMyLocation = document.createElement('DIV');


// maxWidth set to 200
var infoWindow = new google.maps.InfoWindow({
    content: "Gasstationer",
    maxWidth: 300
});

// current user location
var userLocation = null;
var orgUserLocation = null;

$(function () {

    // ADJUST SCREEN ACCORDING TO BROWSER
    var useragent = navigator.userAgent;
    var mapdiv = document.getElementById("map_canvas");
    var dirdiv = document.getElementById("outputDiv");

    if (useragent.indexOf('iPhone') != -1 || useragent.indexOf('Android') != -1) {
        mapdiv.style.width = '100%';
        mapdiv.style.height = '100%';
        dirdiv.style.width = '100%';
        dirdiv.style.height = '100%';
    } else {
        mapdiv.style.width = $(window).width() - 0, 5 + 'px';
        mapdiv.style.height = $(window).height() - 0, 5 + 'px';
        dirdiv.style.width = $(window).width() - 0, 5 + 'px';
        dirdiv.style.height = $(window).height() - 0, 5 + 'px';
    }


    // DIRECTIONS
    var rendererOptions = {
        draggable: true,
        hideRouteList: true,
        polylineOptions: {
            /* http://code.google.com/intl/sv-SE/apis/maps/documentation/javascript/reference.html#PolylineOptions */
            strokeColor: custom_p_directions_color  /* green line on directionpath */
        }
    };

    var directionsDisplay = new google.maps.DirectionsRenderer(rendererOptions);
    var directionsService = new google.maps.DirectionsService();

    var zoomDefault = 8;
    var markersArray = [];
    var lat1;
    var lon1;

    // USED FOR "MIN POSITION"
    var destinationLocation;
    var fullStationInfo;

    ///////////////////////////////////////////////////

    var outputDiv = document.getElementById('outputDiv');

    outputDiv.innerHTML = '<strong>Söker din position...<strong><br />';


    // FF-WORKAROUND - BUTTON ADDED TO SKIP FINDING LOCATION   
    var btnSkip = document.createElement("input");
    btnSkip.setAttribute("type", "submit");
    btnSkip.setAttribute("id", "btnSkip");
    btnSkip.setAttribute("value", "Avbryt sökning");
    btnSkip.setAttribute("class", "singleButton");
    btnSkip.onclick = function () { return false };
    outputDiv.appendChild(btnSkip);

    // CLOSE DIRECTIONS DIV
    $('#btnSkip').live('mouseup', function () {
        defaultSettings();
        document.getElementById("directions").hidden = true;
        document.getElementById("directions").style.visibility = 'hidden';

        return false; // PREVENT MAP FROM RELOADING ON SUBMISSION
    });

    function showDirectionLink() {
        me.directionsLink.hidden = false;
        me.directionsLink.style.visibility = 'visible';
    }

    function initialize() {
        geocoder = new google.maps.Geocoder();
        var latlng = new google.maps.LatLng(59.68993, 14.50195);
        var myOptions = {
            streetViewControl: false,
            mapTypeControl: false,
            scaleControl: false,
            overviewMapControl: false,
            panControl: false,
            zoom: zoomDefault,
            zoomControl: true,
            zoomControlOptions: {
                style: google.maps.ZoomControlStyle.ANDROID,
                position: google.maps.ControlPosition.LEFT_BOTTOM
            },
            center: latlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            mapTypeControlOptions: { mapTypeIds: [0] }
        }
        map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);

        directionsDisplay.setMap(map);
        directionsDisplay.setPanel(document.getElementById("outputDiv"));

        ////////////////////////////////////////////////////////////////////////////////
        // CREATE CUSTOM CONTROLS
        ////////////////////////////////////////////////////////////////////////////////

        // "SÖK"
        function SearchControl(searchDiv, map) {
            searchDiv.setAttribute('class', 'mapControls');

            var searchLocation = document.createElement('DIV');
            searchLocation.setAttribute('id', 'searchLocation');
            searchLocation.setAttribute('class', 'MapControlContainers');
            searchLocation.title = 'Klicka för att söka på en adress';
            searchDiv.appendChild(searchLocation);

            var searchBox = document.createElement('INPUT');
            searchBox.setAttribute('id', 'searchBox');
            searchBox.setAttribute('class', 'mapControlsPadding');
            searchBox.value = 'Sök adress';
            searchLocation.appendChild(searchBox);

            // PREVENT ENTER-KEY FROM RELOADING MAP
            $(document).ready(function () {
                $(searchBox).bind("keypress", function (e) {
                    if (e.keyCode == 13) {
                        if (document.getElementById("searchBox").value != "") {
                            codeAddess();
                        }
                        return false;
                    }
                });
            });

            google.maps.event.addDomListener(searchBox, 'click', function () {
                // CLEAR BOX OF STANDARD VALUE UPON CLICK
                if (document.getElementById("searchBox").value == "Sök adress") {
                    document.getElementById("searchBox").value = "";
                }
            });
        }

        // "DIN POSITION"
        function HomeControl(controlDiv, map) {
            controlDiv.setAttribute('class', 'mapControls');

            // SET CSS FOR THE CONTROL BORDER
            var myLocation = document.createElement('DIV');
            myLocation.setAttribute('class', 'MapControlContainers');
            myLocation.setAttribute('id', 'myLocation');
            myLocation.title = 'Klicka för att visa din position på kartan';
            // myLocation.hidden = true;
            // myLocation.style.visibility = 'hidden';
            controlDiv.appendChild(myLocation);

            // SET CSS FOR THE CONTROL INTERIOR
            var controlText = document.createElement('DIV');
            controlText.setAttribute('class', 'mapControlsPadding');
            controlText.innerHTML = 'Din position';
            myLocation.appendChild(controlText);

            // SETUP THE CLICK EVENT LISTENERS
            google.maps.event.addDomListener(myLocation, 'mouseup', function () {
                userLocation = orgUserLocation;
                map.setCenter(userLocation)
                map.setZoom(16);
            });
        }

        // "SÖK POSITION"
        function FindPositionControl(controlDiv, map) {
            controlDiv.setAttribute('class', 'mapControls');

            // SET CSS FOR THE CONTROL BORDER
            // var myLocation = document.createElement('DIV');
            findMyLocation.setAttribute('class', 'MapControlContainers');
            findMyLocation.setAttribute('id', 'findMyLocation');
            findMyLocation.title = 'Klicka för att visa din position på kartan';
            findMyLocation.hidden = false;
            findMyLocation.style.visibility = 'visible';
            controlDiv.appendChild(findMyLocation);

            // SET CSS FOR THE CONTROL INTERIOR
            var controlText = document.createElement('DIV');
            controlText.setAttribute('class', 'mapControlsPadding');
            controlText.innerHTML = 'Sök position';
            findMyLocation.appendChild(controlText);

            // SETUP THE CLICK EVENT LISTENERS
            google.maps.event.addDomListener(findMyLocation, 'mouseup', function () {
                me.getLocation();
                /*map.setCenter(userLocation)
                map.setZoom(16);*/
            });
        }

        function hideDirectionLink() {
            me.directionsLink.hidden = true;
            me.directionsLink.style.visibility = 'hidden';
        }

        // "TILLBAKA TILL VÄGBESKRIVNING"
        function DirectionControl(directionDiv, map) {
            directionDiv.setAttribute('class', 'mapControlsSmall');

            // Set CSS for the control border
            me.directionsLink = document.createElement('DIV');
            me.directionsLink.setAttribute('id', 'directions');
            me.directionsLink.setAttribute('class', 'MapControlContainersSmall');
            // me.directionsLink.style.backgroundColor = "#018001";

            /* remove to show from start */
            hideDirectionLink();

            me.directionsLink.title = 'Visa vägbeskrivning';
            directionDiv.appendChild(me.directionsLink);

            // Set CSS for the control interior
            var directionText = document.createElement('DIV');
            directionText.setAttribute('class', 'mapControlsPaddingSmall');
            directionText.innerHTML = 'Vägbeskrivning';
            me.directionsLink.appendChild(directionText);

            // SHOW DRIVING DIRECTIONS
            google.maps.event.addDomListener(me.directionsLink, 'mouseup', function () {
                infoWindow.close();
                activeSettings();
            });

            return directionDiv;
        }

        /////////////////////////////////////////////////////////////////////////////////
        // ADD CUSTOM CONTROLS TO MAP

        var homeControlDiv = document.createElement('DIV');
        var homeControl = new HomeControl(homeControlDiv, map);

        var findPositionControlDiv = document.createElement('DIV');
        var findPositionControl = new FindPositionControl(findPositionControlDiv, map);

        var searchControlDiv = document.createElement('DIV');
        var searchControl = new SearchControl(searchControlDiv, map);

        me.setupButtons = function (showFindPosition) {

            map.controls[google.maps.ControlPosition.TOP_CENTER].pop();
            map.controls[google.maps.ControlPosition.TOP_CENTER].pop();

            findPositionControlDiv.index = 1;
            homeControlDiv.index = 1;
            if (showFindPosition) {
                map.controls[google.maps.ControlPosition.TOP_CENTER].push(findPositionControlDiv);
            } else {
                map.controls[google.maps.ControlPosition.TOP_CENTER].push(homeControlDiv);
            }

            searchControlDiv.index = 2;
            map.controls[google.maps.ControlPosition.TOP_CENTER].push(searchControlDiv);
        }
        me.setupButtons(true);

        var directionControlDiv = document.createElement('DIV');
        var directionControl = new DirectionControl(directionControlDiv, map);

        directionControlDiv.index = 3;
        map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(directionControlDiv);

        /////////////////////////////////////////////////////////////////////////////////
        // MAP EVENTS
        google.maps.event.addListener(map, 'idle', function () {
            if (this.loaded == null) {
                loadPoints(map);
                this.loaded = true;
            }
        });

        google.maps.event.addListener(map, 'dragend', function () {
            // loadPoints(map);
            if (document.getElementById("searchBox").value == "") {
                document.getElementById("searchBox").value = "Sök adress";
            }
        });

        google.maps.event.addListener(map, 'click', function () {
            infoWindow.close();
            if (document.getElementById("searchBox").value == "") {
                document.getElementById("searchBox").value = "Sök adress";
            }
        });

        // TRY TO GET LOCATION OF USER
        //if (getQueryString()["id"] == "") {
        // if (1 == 1) {
        if (geo_position_js.init()) {

            me.getLocation = function () {
                geo_position_js.getCurrentPosition(currentPositionCallback, location_errors, { timeout: 10000, maximumAge: 5000, enableHighAccuracy: true });
            }

            me.getLocation();
        }
        else if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(currentPositionCallback, navigator_geolocation_errors, { timeout: 10000, maximumAge: 5000 });
        } else {
            alert('Kunde inte finna din position på kartan.');
            defaultSettings();
        }
        // } else {
        //     defaultSettings();
        //}

        function location_errors(error) {
            //PERMISSION_DENIED
            if (error.code == 1) {
                alert("Tillåtelse att söka position avbröts av användaren.");
                defaultSettings();
            }
            //POSITION_UNAVAILABLE
            else if (error.code == 2) {
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(currentPositionCallback, navigator_geolocation_errors, { timeout: 10000, maximumAge: 5000 });
                }
            }
            //TIMEOUT
            else if (error.code == 3) {
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(currentPositionCallback, navigator_geolocation_errors, { timeout: 10000, maximumAge: 5000 });
                }
            } else {
                defaultSettings();
            }
        }

        function navigator_geolocation_errors(error) {
            switch (error.code) {
                case error.PERMISSION_DENIED: alert("Tillåtelse att söka position avbröts av användaren.");
                    defaultSettings();
                    break;

                case error.POSITION_UNAVAILABLE:
                    alert("Kunde ej finna din position på kartan (Möjlig orsak: svag GPS-signal)");
                    defaultSettings();
                    break;

                case error.TIMEOUT:
                    alert("Kunde ej finna din position på kartan (Möjlig orsak: svag GPS-signal)");
                    defaultSettings();
                    break;
                default: //alert("Okänt fel.");
                    defaultSettings();
                    break;
            }
        }

        // CLICK-EVENT FOR LINKS IN INFOWINDOW
        $('a').live('mouseup', function () {
            var attributeIs = this.getAttribute("class");

            switch (attributeIs) {
                // DIRECTIONS FROM USERLOCATION TO STATION                                              
                case "directionLink":
                    outputDiv.innerHTML = '';
                    // CREATE BUTTON
                    var btnClose = document.createElement("input");
                    btnClose.setAttribute("type", "submit");
                    btnClose.setAttribute("id", "btnClose");
                    btnClose.setAttribute("value", "Visa karta");
                    btnClose.setAttribute("class", "singleButton");
                    btnClose.onclick = function () {
                        // show driving directions link
                        // todo: don't work?
                        //$(this.directionsLink).removeAttr("style").removeAttr("hidden");
                        return false
                    };

                    outputDiv.appendChild(btnClose);
                    // activeSettings(); /* show directions */ 
                    infoWindow.close();

                    calcRoute();

                    // show direction link
                    showDirectionLink();

                    return false;
                    break;

                // DIRECTIONS FROM AN ADDRESS                                              
                case "addressLink":

                    infoWindow.close();

                    outputDiv.innerHTML = '';
                    // ADD TEXTBOX TO SEARCH ADDRESS
                    var tbFrom = document.createElement("input");
                    tbFrom.setAttribute("type", "text");
                    tbFrom.setAttribute("id", "tbFrom");
                    tbFrom.setAttribute("value", "Ange gatuadress");
                    tbFrom.setAttribute("class", "inputBig");
                    tbFrom.onclick = function () { return false };
                    outputDiv.appendChild(tbFrom);

                    // SUBMIT BUTTON
                    var btnSubmit = document.createElement("input");
                    btnSubmit.setAttribute("type", "submit");
                    btnSubmit.setAttribute("id", "btnSubmit");
                    btnSubmit.setAttribute("value", "Sök");
                    btnSubmit.setAttribute("class", "Buttons");
                    btnSubmit.onclick = function () { return false };
                    outputDiv.appendChild(btnSubmit);


                    var btnClose = document.createElement("input");
                    btnClose.setAttribute("type", "submit");
                    btnClose.setAttribute("id", "btnClose");
                    btnClose.setAttribute("value", "Visa karta");
                    btnClose.setAttribute("class", "Buttons");
                    btnClose.onclick = function () { return false };
                    outputDiv.appendChild(btnClose);

                    activeSettings();
                    return false;
                    break;

                // MORE INFO ABOUT STATION                                              
                case "moreInfoLink":
                    // CREATE BUTTON
                    outputDiv.innerHTML = "";

                    outputDiv.innerHTML = fullStationInfo;

                    btnClose = document.createElement("input");
                    btnClose.setAttribute("type", "submit");
                    btnClose.setAttribute("id", "btnSkip");
                    btnClose.setAttribute("value", "Visa karta");
                    btnClose.setAttribute("class", "singleButton");
                    btnClose.onclick = function () { return false };
                    outputDiv.appendChild(btnClose);

                    activeSettings();

                    return false;
                    break;
            }
        });

        // CLOSE DIRECTIONS DIV
        $('#btnClose').live('mouseup', function () {
            defaultSettings();
            // document.getElementById("directions").hidden = false;
            // document.getElementById("directions").style.visibility = 'visible';
            return false; // PREVENT MAP FROM RELOADING
        });

        // SUBMIT SEARCH
        $('#btnSubmit').live('mouseup', function () {
            userLocation = document.getElementById("tbFrom").value;
            calcRoute();
            defaultSettings();
            return false; // PREVENT MAP FROM RELOADING
        });

        $('#tbFrom').live('mouseup', function () {
            // CLEAR BOX OF STANDARD VALUE
            if (document.getElementById("tbFrom").value == "Ange gatuadress") {
                document.getElementById("tbFrom").value = "";
            }
        });
    }

    // SHOW MAP / HIDE DIRECTIONS
    function defaultSettings() {
        /* set properties */
        $("#map_canvas").removeClass('out').addClass('in');
        $("#outputDiv").removeClass('in').addClass('out');
    }

    // SHOW DIRECTIONS / HIDE MAP
    function activeSettings() {
        /* set properties */
        $("#map_canvas").removeClass('in').addClass('out');
        $("#outputDiv").removeClass('out').addClass('in');
    }

    function calcRoute() {
        var start = userLocation;
        var end = destinationLocation;
        var request = {
            origin: start,
            destination: end,
            travelMode: google.maps.DirectionsTravelMode.DRIVING,
            provideRouteAlternatives: false
        };
        directionsService.route(request, function (response, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                directionsDisplay.draggable = false;
                directionsDisplay.setDirections(response);
                showDirectionLink();
            }
        });
    }

    // SEARCH FOR ADDRESS
    function codeAddess() {
        var address = document.getElementById("searchBox").value;
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {

                var Name = 'Fri sökning';
                var Markertext = results[0].formatted_address; ;
                var latlng = results[0].geometry.location;
                var image = 'Icons/black.png';

                map.setCenter(results[0].geometry.location);

                // ZOOM IN ON USER LOCATION
                var listener = google.maps.event.addListener(map, "idle", function () {
                    if (map.getZoom() < 16) map.setZoom(16);
                    google.maps.event.removeListener(listener);
                });

                // MARK ADDRESS ON MAP
                //addMarker(Name, Markertext, latlng, image);

            } else {
                alert("Adress ej funnen!");
            }
        });
    }

    function loadPoints(map) {
        var query = "?action=GetByBounds" +
                    "&NorthEastLat=" + (map.getBounds().getNorthEast().lat()).toString().replace(".", ",") +
                    "&NorthEastLng=" + (map.getBounds().getNorthEast().lng()).toString().replace(".", ",") +
                    "&SouthWestLat=" + (map.getBounds().getSouthWest().lat()).toString().replace(".", ",") +
                    "&SouthWestLng=" + (map.getBounds().getSouthWest().lng()).toString().replace(".", ",");

        $.ajax({
            "url": custom_p_handler_url + query,
            "dataType": "xml",
            "type": "GET",

            success: function (stations) {
                addStations(stations);
                if (boundItems.length > 1) {
                    map.fitBounds(bounds);
                } else if (boundItems.length == 1) {
                    map.setCenter(boundItems[0]);
                    map.setZoom(12);
                }
            },

            error: function (a, b, c) {
                // alert(b + "," + c);
            }
        });

        return false;
    }

    function addStations(stations) {

        var boundCounter = 0;
        var boundId = getQueryString()["id"];
        //var bounds = new google.maps.LatLngBounds();

        $(stations).find("marker").each(function (i) {

            var id = $(this).find("id").text();
            var name = $(this).find("owner").text();
            var info = $(this).find("mapInfo").text();
            var fInfo = $(this).find("fullInfo").text();
            var lat = $(this).find("lat").text();
            var lng = $(this).find("lng").text();

            var latlng = new google.maps.LatLng(parseFloat(lat), parseFloat(lng));
            var image = $(this).find("icon").text();

            // ONLY ADD DIRECTIONS FROM USERLOCATION IF LOCATION IS DEFINED
            /*if (userLocation != undefined) {
                info += "<br/><a href='#' class='directionLink'>» Vägbeskrivning</a>";
            } */
            info += "<br/><a href='#' class='addressLink'></a>";
            info += "<br/><a href='#' class='moreInfoLink'></a></div>";
            //            info += "<br/><a href='#' class='addressLink'>» Från adress</a>";
            //            info += "<br/><a href='#' class='moreInfoLink'>» Mer information</a></div>";

            var marker = addMarker(name, info, latlng, image, fInfo);

            if (boundId != "" && id == boundId) {
                bounds.extend(latlng);
                boundItems.push(latlng);
            }

            me.stations.push({
                text: name,
                marker: marker,
                lng: lng,
                lat: lat
            });
        });

        if (bounds.length > 0 && bounds != null) {
            // map.fitBounds(bounds);
        }

        // update closest station marker
        // var closestMarker = closestStationToCurrentPosition();
        // closestMarker.point.marker.setIcon('Icons/orange.png');
    }

    var currentMarker = null;
    function currentPositionCallback(position) {

        me.setupButtons(false);

        // hides "find my position" because it has been found
        // findMyLocation.hidden = true;
        // findMyLocation.style.visibility = 'hidden';

        var Name = "Användarens Position";
        lat1 = position.coords.latitude;
        lon1 = position.coords.longitude;
        Markertext = "Du är här";

        var latlng = new google.maps.LatLng(lat1, lon1)

        if (currentMarker == null) {
            // currentMarker = addMarker("currentPosition", "Current position", latlng, 'Icons/orange.png', null)
            currentMarker = addMarker("Din position", "Din position", latlng, 'images/map/black.png', null)

            // FUNCTION TO MARK USERS LOCATION ON MAP
            // CURRENTLY JUST ZOOMS IN ON USER LOCATION

            userLocation = latlng;
            orgUserLocation = userLocation;

            map.setCenter(latlng);
            if (getQueryString()["id"] != "") {
                if (boundItems.length > 1) {
                    map.fitBounds(bounds);
                } else if (boundItems.length == 1) {
                    map.setCenter(boundItems[0]);
                    map.setZoom(12);
                }
            } else {
            }

            // ZOOM IN ON USER LOCATION
            var listener = google.maps.event.addListener(map, "idle", function () {
                if (map.getZoom() < 12) map.setZoom(12);
                google.maps.event.removeListener(listener);
            });

            var image = 'Icons/black.png';

            if ($("#myLocation").length > 0) {
                $("#myLocation").get(0).hidden = false;
                $("#myLocation").get(0).style.visibility = 'visible';
            }

            defaultSettings();
        } else {
            // todo: update current marker
        }

        // get current position every five second 
        //setTimeout("me.getLocation();", 5000);
    }

    function addMarker(Name, Markertext, latlng, image, fInfo) {
        // only add new markers
        var add = true;
        var markersArrayLength = markersArray.length;
        if (markersArrayLength > 0) {
            for (var i = 0; i < markersArrayLength; i++) {
                if (markersArray[i].position.lat() == latlng.lat() && markersArray[i].position.lng() == latlng.lng()) {
                    add = false;
                    break;
                }
            }
        }

        var marker = null;
        if (add) {
            // POSSIBLE ISSUE WITH MOBILE SAFARI, REQUIRES -> Draggable: true
            marker = new google.maps.Marker({
                title: Name,
                map: map,
                icon: image,
                position: latlng,
                clickable: true
            });

            addInfoWindow(marker, Markertext, fInfo);
            markersArray.push(marker);
        }

        return marker;
    }

    function addInfoWindow(marker, text, fInfo) {
        marker.markerText = text;
        google.maps.event.addListener(marker, 'mouseup', function () {
            // SAVE CLICKED MARKER LOCATION FOR USE WITH DIRECTIONS

            outputDiv.innerHTML = '';
            fullStationInfo = fInfo; // ADD FULL INFO TO outputDIV
            destinationLocation = marker.getPosition();
            infoWindow.setContent(marker.markerText);
            infoWindow.open(map, marker);
        });
    }

    initialize();
});

    function closestStationToCurrentPosition() {
        return closestPoint(userLocation, me.stations);
    }

    function closestPoint(refLatLng /* google.maps.LatLng */ , points) {

        points: [{
            text: "",
            lng: 0,
            lat: 0
        }]

        //var refLatLng = new google.maps.LatLng(refPoint.lat, refPoint.lng);

        var closest = {
            point: null,
            distance: null /* meters */
        };

        for (var pointIndex in points) {
            var point = points[pointIndex];

            var latLng = new google.maps.LatLng(point.lat, point.lng);
            var dist = google.maps.geometry.spherical.computeDistanceBetween(refLatLng, latLng);

            if (closest.distance == null || closest.distance > dist) {
                closest.distance = dist;
                closest.point = point;
            }
        }

        return closest;
    };