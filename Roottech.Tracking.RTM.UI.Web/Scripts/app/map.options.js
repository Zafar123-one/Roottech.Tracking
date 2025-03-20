function initialize(mapCanvas, latLng, zoom) {
    mapOptions = {
        center: latLng,
        zoom: zoom,
        minZoom: minZoomLevel,
        //maxZoom: 9,
        mapTypeId: google.maps.MapTypeId.ROADMAP,//.TERRAIN,
        mapTypeControl: false,
        mapTypeControlOptions:
            {
                //style: google.maps.MapTypeControlStyle.HORIZONTAL_BAR, //HORIZONTAL_BAR
                //poistion: google.maps.ControlPosition.TOP_RIGHT,
                mapTypeIds: [google.maps.MapTypeId.ROADMAP,
                    google.maps.MapTypeId.TERRAIN,
                    google.maps.MapTypeId.HYBRID,
                    google.maps.MapTypeId.SATELLITE]
            },
        overviewMapControl: true,
        overviewMapControlOptions: {
            opened: true
        },
        scaleControl: false,
        scaleControlOptions:
            {
                poistion: google.maps.ControlPosition.TOP_LEFT
            },
        zoomControl: false,
        zoomControlOptions:
            {
                style: google.maps.NavigationControlStyle.LARGE //SMALL
                //,poistion: google.maps.ControlPosition.LEFT_BOTTOM
            },
        panControl: false,
        panControlOptions:
            {
                

                //poistion: google.maps.ControlPosition.LEFT_BOTTOM
            },
        rotateControl: false,
        rotateControlOptions:
            {
                

                //poistion: google.maps.ControlPosition.LEFT_BOTTOM
            },
        streetViewControl: false,
        streetViewControlOptions:
            {
                poistion: google.maps.ControlPosition.RIGHT_CENTER
            }/*,
        disableDoubleClickZoom: true,
        draggable: false,
        clickable: false*/
            //, editable: true
    };
    map = new google.maps.Map(document.getElementById(mapCanvas), mapOptions);

    map.enableKeyDragZoom({
        key: "none", 
        visualEnabled: true,
        visualSize: new google.maps.Size(0, 0),
        /*
        //visualPosition: google.maps.ControlPosition.LEFT,
        //visualPositionOffset: new google.maps.Size(35, 0),
        //visualPositionIndex: null,
        //visualSprite: "http://maps.gstatic.com/mapfiles/ftr/controls/dragzoom_btn.png",
        visualTips: {
            off: "Turn on",
            on: "Turn off"
        },*/
        boxStyle: {
            border: "1px dashed black",//"4px dashed black",
            backgroundColor: "transparent",
            opacity: 1.0
        },
        veilStyle: {
            //backgroundColor: "red",
            opacity: 0.35,
            cursor: "crosshair"
        }
    });

    g.event.addListener(map, 'zoom_changed', function () {
        //if (map.getZoom() < minZoomLevel) map.setZoom(minZoomLevel);
        $("#zoomLevel").html(map.getZoom());
    });

    /*addMarkerForResource(new google.maps.LatLng(24.893599, 67.027903),
        "<b>For checking purpose <br/> default marker</b>", true, true, undefined, undefined,
        new google.maps.MarkerImage(
            "http://www.google.com/intl/en_us/mapfiles/ms/micons/blue-pushpin.png", 
            new google.maps.Size(32, 32), new google.maps.Point(0, 0), new google.maps.Point(16, 32)));
            */
    /*
    geocoder = new google.maps.Geocoder();

    //Update postal address when the marker is dragged
    google.maps.event.addListener(marker, 'click', function () { //dragend
    geocoder.geocode({ latLng: marker.getPosition() }, function (responses) {
    if (responses && responses.length > 0) {
    infoWindow.setContent(
    "<div style=\"font-size:smaller;\">" + responses[0].formatted_address
    + "<br />"
    + "Latitude: " + marker.getPosition().lat() + "&nbsp"
    + "Longitude: " + marker.getPosition().lng() + "</div>"
    );
    infoWindow.open(map, marker);
    } else {
    alert('Error: Google Maps could not determine the address of this location.');
    }
    });
    map.panTo(marker.getPosition());
    });

    // Close the marker window when being dragged
    google.maps.event.addListener(marker, 'dragstart', function () {
    infoWindow.close(map, marker);
    });*/
    //mapDrawing();
}

function registerOverlay(polygon, circle, polyline, rectangle, marker, infoWindow, customOverlayLabel, baseType, markerPolyline, arrayIndex, markerArray, infoWindowArray) {
    if (arrayIndex == undefined) {
        overlaysArray.polygon.push(polygon);
        overlaysArray.circle.push(circle);
        overlaysArray.polyline.push(polyline);
        overlaysArray.rectangle.push(rectangle);
        overlaysArray.marker.push(marker);
        overlaysArray.infoWindow.push(infoWindow);
        overlaysArray.customOverlayLabel.push(customOverlayLabel);
        overlaysArray.markerPolyline.push(markerPolyline);
        overlaysArray.baseType.push(baseType);
        overlaysArray.markerArray.push(markerArray);
        overlaysArray.infoWindowArray.push(infoWindowArray);
    } else {
        unRegisterOverlay(arrayIndex);
        overlaysArray.polygon[arrayIndex] = polygon;
        overlaysArray.circle[arrayIndex] = circle;
        overlaysArray.polyline[arrayIndex] = polyline;
        overlaysArray.rectangle[arrayIndex] = rectangle;
        overlaysArray.marker[arrayIndex] = marker;
        overlaysArray.infoWindow[arrayIndex] = infoWindow;
        overlaysArray.customOverlayLabel[arrayIndex] = customOverlayLabel;
        overlaysArray.markerPolyline[arrayIndex] = markerPolyline;
        overlaysArray.baseType[arrayIndex] = baseType;
        overlaysArray.markerArray[arrayIndex] = markerArray;
        overlaysArray.infoWindowArray[arrayIndex] = infoWindowArray;
    }
}

// Deletes all markers in the array by removing references to them
function deleteOverlays() {
    if (overlaysArray.marker) {
        for (i in overlaysArray.marker) {
            unRegisterOverlay(i);
            //overlaysArray[i].setMap(null);
        }
        overlaysArray.marker.length = 0;
        overlaysArray.infoWindow.length = 0;
        overlaysArray.polygon.length = 0;
        overlaysArray.polyline.length = 0;
        overlaysArray.circle.length = 0;
        overlaysArray.rectangle.length = 0;
        overlaysArray.customOverlayLabel.length = 0;
        overlaysArray.markerPolyline.length = 0;
        overlaysArray.baseType.length = 0;
        overlaysArray.markerArray.length = 0;
        overlaysArray.infoWindowArray.length = 0;
    }
}

function unRegisterOverlay(i) {
    if (overlaysArray) {
        deleteOverlay(overlaysArray.marker, i);
        deleteOverlay(overlaysArray.infoWindow, i);
        deleteOverlay(overlaysArray.polygon, i);
        deleteOverlay(overlaysArray.polyline, i);
        deleteOverlay(overlaysArray.circle, i);
        deleteOverlay(overlaysArray.rectangle, i);
        deleteOverlay(overlaysArray.customOverlayLabel, i);
        deleteOverlay(overlaysArray.markerPolyline, i);
        overlaysArray.baseType[i] = null;
        deleteOverlay(overlaysArray.markerArray, 0);
        deleteOverlay(overlaysArray.infoWindowArray, 0);
        //overlaysArray.length = overlaysArray.length - 1;
    }
}

function deleteOverlay(arrayItem, i) {
    if (arrayItem[i] != null) {
        if (arrayItem[i].length > 0) {
            $(arrayItem[i]).each(function () {
                this.setMap(null);
            });
            arrayItem[i].length = 0;
        }
        else
            arrayItem[i].setMap(null);
        
        arrayItem[i] = null;
    }
}