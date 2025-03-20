function mapDrawing(wantToAddGeofence) {
    var distanceUnit = "Kms";
    var distanceQty = 1000;

    $("#miles").click(function () {
        $("#tdMiles").css("backgroundColor", "#A5CFF7");
        $("#tdKilo").css("backgroundColor", "transparent");
        distanceQty = 1609.34;
        distanceUnit = "Mi";
    });
    $("#kilometer").click(function () {
        $("#tdMiles").css("backgroundColor", "transparent");
        $("#tdKilo").css("backgroundColor", "#A5CFF7");
        distanceQty = 1000;
        distanceUnit = "Kms";
    });

    var options = {
        fillColor: "#e2ffd1",
        fillOpacity: 0.5,
        strokeWeight: 2,
        strokeColor: "#9d9d9d",
        strokeOpacity: 1,
        clickable: true,
        zIndex: 1,
        editable: true,
        strokePosition: g.StrokePosition.INSIDE
    };

    var polylineOptions = options;
    polylineOptions.geodesic = true;

    var lineSymbol = {
        path: g.SymbolPath.FORWARD_CLOSED_ARROW,
        strokeColor: "#000000",
        strokeOpacity: 1
    };
    var arrowArray = [];
    for (var i = 0; i < 10; i++) {
        arrowArray.push({
            icon: lineSymbol,
            offset: ((i+1) * 10) + "%",
            repeat: ((i + 1) * 10) + "%",
            fixedRotation: false
        });
    }
    polylineOptions.icons = arrowArray;

    drawingManager = new g.drawing.DrawingManager({
        drawingControl: false ,
        drawingControlOptions: {
            position: g.ControlPosition.TOP_CENTER,
            drawingModes: [
                g.drawing.OverlayType.MARKER,
                g.drawing.OverlayType.CIRCLE,
                g.drawing.OverlayType.POLYGON,
                g.drawing.OverlayType.POLYLINE,
                g.drawing.OverlayType.RECTANGLE
            ]
        },
        markerOptions: options,
        circleOptions: options,
        polygonOptions: polylineOptions,
        polylineOptions: polylineOptions,
        rectangleOptions: options
    });
    drawingManager.setMap(map);

    g.event.addListener(drawingManager, "markercomplete", function (marker) {
        registerOverlay(null, null, null, null, marker, null, null, g.drawing.OverlayType.CIRCLE);
        marker.setTitle("Latitude: " + marker.getPosition().lat() + "  Longitude :" + marker.getPosition().lng());

        marker.setIcon(new g.MarkerImage(
            "http://www.google.com/intl/en_us/mapfiles/ms/micons/blue-pushpin.png",
            new g.Size(32, 32), new g.Point(0, 0), new g.Point(16, 32)));
        
        $("#latitude").val(marker.getPosition().lat());
        $("#longitude").val(marker.getPosition().lng());
        g.event.addListener(marker, "rightclick", function (mouseEvent) {
            unRegisterOverlay(overlaysArray.marker.indexOf(marker));
        });
    });

    g.event.addListener(drawingManager, "overlaycomplete", function (event) {
        var marker,textToDisplay;
        if (event.type == g.drawing.OverlayType.CIRCLE) {
            textToDisplay = "Radius: " + (event.overlay.getRadius() / distanceQty).toFixed(2) + " " + distanceUnit;
            registerOverlay(null, event.overlay, null, null, makeMarker(event.overlay.getCenter(), textToDisplay, "bbT"), null, null, g.drawing.OverlayType.CIRCLE);
            $("#lblTotlaDistance").text(textToDisplay);

            g.event.addListener(event.overlay, "rightclick", function (mouseEvent) {
                unRegisterOverlay(overlaysArray.circle.indexOf(event.overlay));
            });
            if (wantToAddGeofence)
                saveGeofence("C", null, event.overlay.getRadius(), event.overlay.getCenter().lat(), event.overlay.getCenter().lng(), 0, 0, 0, 0, overlaysArray.circle.indexOf(event.overlay));
        }
        else if (event.type == g.drawing.OverlayType.POLYLINE || event.type == g.drawing.OverlayType.POLYGON) {
            var linePathArray = event.overlay.getPath().getArray();
            var prevPath = linePathArray[0], totalLength = 0, markers = [], typeToSave = "L";

            // For sub lengths
            $(linePathArray).each(function(index, element) {
                if (index == 0) return true; //continue;
                markers.push(makeMarker(element, calcDistance(prevPath, element, distanceQty) + " " + distanceUnit, "edge_bc"));
                totalLength = totalLength + calcDistance(prevPath, element, distanceQty);
                prevPath = element;
            });
            if (event.type == g.drawing.OverlayType.POLYGON) {
                totalLength = totalLength + calcDistance(linePathArray[0], linePathArray[linePathArray.length - 1], distanceQty);
                typeToSave = "P";
            }
            textToDisplay = "Total: " + parseFloat(totalLength).toFixed(2) + " " + distanceUnit;
            markers.push(makeMarker(linePathArray[0], textToDisplay, "edge_bc"));
            $("#lblTotlaDistance").text(textToDisplay);

            registerOverlay(event.type == g.drawing.OverlayType.POLYGON ? event.overlay : null, null,
                event.type == g.drawing.OverlayType.POLYLINE ? event.overlay : null,
                null, markers, null, null, event.type);

            g.event.addListener(event.overlay, "rightclick", function(mouseEvent) {
                if (event.type == g.drawing.OverlayType.POLYGON)
                    unRegisterOverlay(overlaysArray.polygon.indexOf(event.overlay));
                else
                    unRegisterOverlay(overlaysArray.polyline.indexOf(event.overlay));
            });
            if (wantToAddGeofence)
                saveGeofence(typeToSave, linePathArray, 0, 0, 0, 0, 0, 0, 0, 
                (event.type == g.drawing.OverlayType.POLYGON) 
                    ? overlaysArray.polygon.indexOf(event.overlay)
                    : overlaysArray.polyline.indexOf(event.overlay));
        } else if (event.type == g.drawing.OverlayType.RECTANGLE) {
            var mapRectangle = event.overlay;
            var rectangleBounds = mapRectangle.getBounds();
            var rectangleNorthEastBounds = rectangleBounds.getNorthEast();
            var rectangleNorthEastBoundsLatBounds = rectangleNorthEastBounds.lat();
            var rectangleNorthEastBoundsLngBounds = rectangleNorthEastBounds.lng();
            var rectangleSouthWestBounds = rectangleBounds.getSouthWest();
            var rectangleSouthWestBoundsLatBounds = rectangleSouthWestBounds.lat();
            var rectangleSouthWestBoundsLngBounds = rectangleSouthWestBounds.lng();
            var rectangleNorthWestBounds = new g.LatLng(rectangleNorthEastBoundsLatBounds, rectangleSouthWestBoundsLngBounds);
            var rectangleSouthEastBounds = new g.LatLng(rectangleSouthWestBoundsLatBounds, rectangleNorthEastBoundsLngBounds);
            var rectanglePath = [rectangleNorthEastBounds, rectangleSouthEastBounds, rectangleSouthWestBounds, rectangleNorthWestBounds, rectangleNorthEastBounds];
            var rectangleArea = g.geometry.spherical.computeArea(rectanglePath);
            g.event.addListener(mapRectangle, "bounds_changed", function () {
                rectangleBounds = mapRectangle.getBounds();
                rectangleNorthEastBounds = rectangleBounds.getNorthEast();
                rectangleNorthEastBoundsLatBounds = rectangleNorthEastBounds.lat();
                rectangleNorthEastBoundsLngBounds = rectangleNorthEastBounds.lng();
                rectangleSouthWestBounds = rectangleBounds.getSouthWest();
                rectangleSouthWestBoundsLatBounds = rectangleSouthWestBounds.lat();
                rectangleSouthWestBoundsLngBounds = rectangleSouthWestBounds.lng();
                rectangleNorthWestBounds = new g.LatLng(rectangleNorthEastBoundsLatBounds, rectangleSouthWestBoundsLngBounds);
                rectangleSouthEastBounds = new g.LatLng(rectangleSouthWestBoundsLatBounds, rectangleNorthEastBoundsLngBounds);
                rectanglePath = [rectangleNorthEastBounds, rectangleSouthEastBounds, rectangleSouthWestBounds, rectangleNorthWestBounds, rectangleNorthEastBounds];
                rectangleArea = g.geometry.spherical.computeArea(rectanglePath);
            });
            g.event.addListener(event.overlay, "rightclick", function (mouseEvent) {
                unRegisterOverlay(overlaysArray.rectangle.indexOf(mapRectangle));
            });
            textToDisplay = "Rectangle Area: " + (rectangleArea / distanceQty).toFixed(2) + " " + distanceUnit + " Sq ";
            marker = makeMarker(rectangleBounds.getCenter(), textToDisplay, "bbT");
            registerOverlay(null, null, null, event.overlay, marker, null, null, g.drawing.OverlayType.CIRCLE);
            $("#lblTotlaDistance").text(textToDisplay);
            if (wantToAddGeofence)
                saveGeofence("R", null, 0, 0, 0, rectangleNorthWestBounds.lat(), rectangleNorthWestBounds.lng(), rectangleSouthEastBounds.lat(), rectangleSouthEastBounds.lng()
                , overlaysArray.rectangle.indexOf(mapRectangle));
        }
    });

    $("#measureTool").dialog("option", "title", wantToAddGeofence ? "Add Geo Fence" : "Measure Tool");//there is a reference of this label please replace that as well.
    $("#measureTool").dialog("open");
}

function makeMarker(pointToShow, textToDisplay, frameStyle) {
    //frameStyle https://developers.google.com/chart/image/docs/gallery/dynamic_icons#frame_style_constants

    return addMarkerForResource(pointToShow, undefined, true, false, -1, undefined,
        new g.MarkerImage("https://chart.googleapis.com/chart?chst=d_bubble_text_small&chld=" + frameStyle + "|" + textToDisplay + "|C6EF8C|000", null, null, null, null));
}

//calculates distance between two points in km"s
function calcDistance(p1, p2, distanceQty) {
    return parseFloat((g.geometry.spherical.computeDistanceBetween(p1, p2) / distanceQty).toFixed(2));
}