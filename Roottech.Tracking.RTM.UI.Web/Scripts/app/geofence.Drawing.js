function geofenceDrawing() {

    var distanceUnit = "Kms"; // Mi
    var distanceQty = 1000; //1609.34

    $("#miles").click(function () {
        $("#tdMiles").css('backgroundColor', '#A5CFF7');
        $("#tdKilo").css('backgroundColor', 'transparent');
        distanceQty = 1609.34;
        distanceUnit = "Mi";
    });
    $("#kilometer").click(function () {
        $("#tdMiles").css('backgroundColor', 'transparent');
        $("#tdKilo").css('backgroundColor', '#A5CFF7');
        distanceQty = 1000;
        distanceUnit = "Kms";
    });
    //    var distanceUnit = "Kms"; // Mi
    //    var distanceQty = 1000; //1609.34
    var options = {
        fillColor: "#e2ffd1",
        fillOpacity: 0.5,
        strokeWeight: 2,
        strokeColor: "#9d9d9d",
        strokeOpacity: 1,
        clickable: true,
        zIndex: 1,
        editable: true,
        strokePosition: google.maps.StrokePosition.INSIDE
    };

    var polylineOptions = options;
    polylineOptions.geodesic = true;

    var lineSymbol = {
        path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW, //FORWARD_OPEN_ARROW
        strokeColor: "#000000",
        strokeOpacity: 1
    }; /*
    var arrow = {
        icon: lineSymbol,
        offset: "50%",
        repeat: "50%",
        fixedRotation: true
    };
    flightPath = new google.maps.Polyline({
        path: somePath,
        strokeColor: "#ff0000",
        strokeOpacity: 1,
        icons: [arrow]
    });*/
    var arrowArray = [];
    for (var i = 0; i < 10; i++) {
        arrowArray.push({
            icon: lineSymbol,
            offset: ((i + 1) * 10) + "%",
            repeat: ((i + 1) * 10) + "%",
            fixedRotation: false
        });
    }
    polylineOptions.icons = arrowArray; //[arrow];

    drawingManager = new google.maps.drawing.DrawingManager({
        //drawingMode: google.maps.drawing.OverlayType.MARKER,
        drawingControl: false,
        drawingControlOptions: {
            position: google.maps.ControlPosition.TOP_CENTER,
            drawingModes: [
                google.maps.drawing.OverlayType.MARKER,
                google.maps.drawing.OverlayType.CIRCLE,
                google.maps.drawing.OverlayType.POLYGON,
                google.maps.drawing.OverlayType.POLYLINE,
                google.maps.drawing.OverlayType.RECTANGLE
            ]
        },
        markerOptions: options,
        //icon: new google.maps.MarkerImage("http://www.example.com/icon.png")
        circleOptions: options,
        polygonOptions: polylineOptions,
        //icon: new google.maps.CircleImage("~/Images/geoFence/hand_24.png"),
        //polygonOptions: options,
        polylineOptions: polylineOptions,
        rectangleOptions: options
    });
    drawingManager.setMap(map);

    google.maps.event.addListener(drawingManager, "markercomplete", function (marker) {
        registerOverlay(null, null, null, null, marker, null, null, google.maps.drawing.OverlayType.CIRCLE);
        marker.setTitle('Latitude: ' + marker.getPosition().lat() + '  Longitude :' + marker.getPosition().lng());

        marker.setIcon(new google.maps.MarkerImage(
            "http://www.google.com/intl/en_us/mapfiles/ms/micons/blue-pushpin.png",
            new google.maps.Size(32, 32), new google.maps.Point(0, 0), new google.maps.Point(16, 32)));

        $("#latitude").val(marker.getPosition().lat());
        $("#longitude").val(marker.getPosition().lng());
        google.maps.event.addListener(marker, "rightclick", function (mouseEvent) {
            unRegisterOverlay(overlaysArray.marker.indexOf(marker));
        });
    });

    google.maps.event.addListener(drawingManager, "overlaycomplete", function (event) {
        var marker;
        if (event.type == google.maps.drawing.OverlayType.CIRCLE) {
            marker = makeMarker(event.overlay.getCenter());
            registerOverlay(null, event.overlay, null, null, marker, null,
                showLabel(marker, "Radius: " + (event.overlay.getRadius() / distanceQty).toFixed(2) + distanceUnit),
                google.maps.drawing.OverlayType.CIRCLE);

            /* Add By Irfan*/
            $("#lblTotlaDistance").text("Radius: " + (event.overlay.getRadius() / distanceQty).toFixed(2) + distanceUnit);
            /* Add By Irfan*/

            google.maps.event.addListener(event.overlay, "rightclick", function (mouseEvent) {
                unRegisterOverlay(overlaysArray.circle.indexOf(event.overlay));
            });
            //event for when circle move label and marker move as well;
        }
        else if (event.type == google.maps.drawing.OverlayType.POLYLINE
            || event.type == google.maps.drawing.OverlayType.POLYGON) {
            var linePathArray = event.overlay.getPath().getArray();

            var prevPath = linePathArray[0];
            var totalLength = 0;
            // For total Lenth
            var markers = [];
            marker = makeMarker(linePathArray[0]);
            markers.push(marker);
            var labels = [];
            //labels.push(showLabel(marker, "Total: " + calcDistance(linePathArray[0], linePathArray[linePathArray.length - 1], distanceQty) + distanceUnit));

            // For sub lengths
            $(linePathArray).each(function (index, element) {
                if (index == 0) return true; //continue;
                marker = makeMarker(element);
                markers.push(marker);
                labels.push(showLabel(marker, calcDistance(prevPath, element, distanceQty) + " " + distanceUnit));
                totalLength = totalLength + calcDistance(prevPath, element, distanceQty);
                console.log(totalLength);
                prevPath = element;
            });
            if (event.type == google.maps.drawing.OverlayType.POLYGON) {
                totalLength = totalLength + calcDistance(linePathArray[0], linePathArray[linePathArray.length - 1], distanceQty);
            }

            labels.push(showLabel(markers[0], "Total: " + parseFloat(totalLength).toFixed(2) + ' ' + distanceUnit));

            /* Add By Irfan*/
            $("#lblTotlaDistance").text("Total: " + parseFloat(totalLength).toFixed(2) + ' ' + distanceUnit);
            /* Add By Irfan*/

            registerOverlay(event.type == google.maps.drawing.OverlayType.POLYGON ? event.overlay : null, null,
                            event.type == google.maps.drawing.OverlayType.POLYLINE ? event.overlay : null,
                            null, markers, null, labels, event.type);

            google.maps.event.addListener(event.overlay, "rightclick", function (mouseEvent) {
                if (event.type == google.maps.drawing.OverlayType.POLYGON)
                    unRegisterOverlay(overlaysArray.polygon.indexOf(event.overlay));
                else
                    unRegisterOverlay(overlaysArray.polyline.indexOf(event.overlay));
            });
        }
        else if (event.type == google.maps.drawing.OverlayType.RECTANGLE) {
            /*mapRectangle.setOptions({fillColor: "#"+("00000"+(Math.random()*(16777216)|0).toString(16)).slice(-6)});*/
            var mapRectangle = event.overlay;
            var rectangleBounds = mapRectangle.getBounds();
            var rectangleNorthEastBounds = rectangleBounds.getNorthEast();
            var rectangleNorthEastBoundsLatBounds = rectangleNorthEastBounds.lat();
            var rectangleNorthEastBoundsLngBounds = rectangleNorthEastBounds.lng();
            var rectangleSouthWestBounds = rectangleBounds.getSouthWest();
            var rectangleSouthWestBoundsLatBounds = rectangleSouthWestBounds.lat();
            var rectangleSouthWestBoundsLngBounds = rectangleSouthWestBounds.lng();
            var rectangleNorthWestBounds = new google.maps.LatLng(rectangleNorthEastBoundsLatBounds, rectangleSouthWestBoundsLngBounds);
            var rectangleSouthEastBounds = new google.maps.LatLng(rectangleSouthWestBoundsLatBounds, rectangleNorthEastBoundsLngBounds);
            var rectanglePath = [rectangleNorthEastBounds, rectangleSouthEastBounds, rectangleSouthWestBounds, rectangleNorthWestBounds, rectangleNorthEastBounds];
            var rectangleArea = google.maps.geometry.spherical.computeArea(rectanglePath);
            //polygonArea = .000000386102 * polygonArea;
            //alert(polygonArea.toFixed(4) + " miles sq");
            //document.getElementById("rectangleStatus").innerHTML = "RECTANGLE AREA: " + rectangleArea.toFixed(2) + " meters square";
            google.maps.event.addListener(mapRectangle, "bounds_changed", function () {
                rectangleBounds = mapRectangle.getBounds();
                rectangleNorthEastBounds = rectangleBounds.getNorthEast();
                rectangleNorthEastBoundsLatBounds = rectangleNorthEastBounds.lat();
                rectangleNorthEastBoundsLngBounds = rectangleNorthEastBounds.lng();
                rectangleSouthWestBounds = rectangleBounds.getSouthWest();
                rectangleSouthWestBoundsLatBounds = rectangleSouthWestBounds.lat();
                rectangleSouthWestBoundsLngBounds = rectangleSouthWestBounds.lng();
                rectangleNorthWestBounds = new google.maps.LatLng(rectangleNorthEastBoundsLatBounds, rectangleSouthWestBoundsLngBounds);
                rectangleSouthEastBounds = new google.maps.LatLng(rectangleSouthWestBoundsLatBounds, rectangleNorthEastBoundsLngBounds);
                rectanglePath = [rectangleNorthEastBounds, rectangleSouthEastBounds, rectangleSouthWestBounds, rectangleNorthWestBounds, rectangleNorthEastBounds];
                rectangleArea = google.maps.geometry.spherical.computeArea(rectanglePath);
                //polygonArea = .000000386102 * polygonArea;
                //alert(polygonArea.toFixed(4) + " miles sq");
                var index = overlaysArray.rectangle.indexOf(mapRectangle);
                var label = overlaysArray.rectangle[index];
                label.set("text", (rectangleArea / distanceQty).toFixed(2) + " Sq " + distanceUnit);



                document.getElementById("rectangleStatus").innerHTML = "RECTANGLE AREA: " + rectangleArea.toFixed(2) + " meters square";
            });
            google.maps.event.addListener(event.overlay, "rightclick", function (mouseEvent) {
                unRegisterOverlay(overlaysArray.rectangle.indexOf(mapRectangle));
            });
            marker = makeMarker(rectangleBounds.getCenter());
            registerOverlay(null, null, null, event.overlay, marker, null,
                showLabel(marker, (rectangleArea / distanceQty).toFixed(2) + " Sq " + distanceUnit),
                google.maps.drawing.OverlayType.CIRCLE);

            /* Add By Irfan*/
            $("#lblTotlaDistance").text("Total: " + (rectangleArea / distanceQty).toFixed(2) + " Sq " + distanceUnit);
            /* Add By Irfan*/

            //event for when circle move label and marker move as well;
        }


        /*google.maps.event.addListener(myPolyline, "capturing_changed", function(e) { //event.overlay
        var path = this.getPath();
        // Path... blah blah
        });*/


    });
    /*
    google.maps.event.addListener(drawingManager, "click", function (event) {
    console.log(event.type);
    console.log(event.type);
    //console.log(event.overlay);
    //overlaysArray.push(event.overlay);
    });
    */
}

function makeMarker(pointToShow) {
    return addMarkerForResource(pointToShow, undefined, true, false, -1, undefined,
        new google.maps.MarkerImage("/Images/marker-panel.png",
            new google.maps.Size(100, 39),
            new google.maps.Point(0, 0),
            new google.maps.Point(50, 39)));
}

function showLabel(markerToBind, textToShow) {
    var label = new Label({
        map: map
    });
    label.set("zIndex", 1234);
    label.bindTo("position", markerToBind);
    label.set("text", textToShow);
    label.set("visible", true);
    /*
    label.bindTo("clickable", marker);
    
    google.maps.event.addListener(marker, "click", function() { alert("Marker has been clicked"); })
    google.maps.event.addListener(label, "click", function() { alert("Label has been clicked"); })
    */
    return label;
}

/*
computeAngle
google.maps.geometry.spherical.computeDistanceBetween(
overlaysArray[1].getPath().getAt(0),
overlaysArray[1].getPath().getAt(1)) / 1000*/
//calculates distance between two points in km"s
function calcDistance(p1, p2, distanceQty) {
    return parseFloat((google.maps.geometry.spherical.computeDistanceBetween(p1, p2) / distanceQty).toFixed(2));
}