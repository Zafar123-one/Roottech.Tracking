function drawGeoFenceByType(data, bounds) {
    var latLng, infoWindow = new g.InfoWindow(), i, options = {
        strokeColor: "#FF0000",
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillColor: "#FF0000",
        fillOpacity: 0.35,
        map: map,
        zIndex: 100
    }, path = [];

    $(data).each(function (index) {
        if (this.GFType == "C") {
            latLng = new g.LatLng(this.CLati, this.CLongi);
            options.center = latLng;
            options.radius = this.CRadius;
            infoWindow = "<table border=0 class='msgpopuptable'><tbody>" +
                "<tr><th>Circle Title:</th><td>" + this.GFTitle + "</td></tr>" +
                "<tr><th>Name:</th><td>" + this.GFName + "</td></tr>" +
                "<tr><th>Comment:</th><td>" + this.Comment + "</td></tr>" +
                "<tr><th>Buffer:</th><td>" + this.GFMargin + "</td></tr>" +
                "<tr><th>Latitude:</th><td>" + this.CLati + "</td></tr>" +
                "<tr><th>Longitude:</th><td>" + this.CLongi + "</td></tr>" +
                "<tr><th>Radius:</th><td>" + this.CRadius + "</td></tr></tbody></table>";
            i = overlaysArray.marker.indexOf(addMarkerForResource(latLng, infoWindow, this.GeofenceNo, false, false)); // Add the circle for this city to the map.

            // Add the circle for this city to the map.
            var circle = new g.Circle(options);
            overlaysArray.baseType[i] = g.drawing.OverlayType.CIRCLE;
            overlaysArray.circle[i] = circle;

            g.event.addListener(circle, "rightclick", function (mouseEvent) {
                unRegisterOverlay(overlaysArray.circle.indexOf(circle));
            });
            if (bounds) {
                bounds.extend(overlaysArray.circle[i].getBounds().getNorthEast());
                bounds.extend(overlaysArray.circle[i].getBounds().getSouthWest());
            }

            // Draw buffer circle
            if (this.GFMargin > 0) {
                options.fillColor = "#FFFF00";
                options.zIndex = 50;
                options.radius = parseInt(this.CRadius) + parseInt(this.GFMargin);

                circle = new g.Circle(options);
                overlaysArray.polygon[i] = circle;
            }
        } else if (this.GFType == "R") {
            var latLngBounds = new g.LatLngBounds(new g.LatLng(this.SLeftTopLati, this.SLeftTopLongi), new g.LatLng(this.SRightBotLati, this.SRightBotLongi));
            options.bounds = latLngBounds;

            infoWindow = "<table border=0 class='msgpopuptable'><tbody>" +
                "<tr><th>Rectangle Title:</th><td>" + this.GFTitle + "</td></tr>" +
                "<tr><th>Name:</th><td>" + this.GFName + "</td></tr>" +
                "<tr><th>Comment:</th><td>" + this.Comment + "</td></tr>" +
                "<tr><th>Buffer:</th><td>" + this.GFMargin + "</td></tr>" +
                "<tr><th>Left Top Latitude:</th><td>" + this.SLeftTopLati + "</td></tr>" +
                "<tr><th>Left Top Longitude:</th><td>" + this.SLeftTopLongi + "</td></tr>" +
                "<tr><th>Right Bottom Latitude:</th><td>" + this.SRightBotLati + "</td></tr>" +
                "<tr><th>Right Bottom Longitude:</th><td>" + this.SRightBotLongi + "</td></tr></tbody></table>";
            i = overlaysArray.marker.indexOf(addMarkerForResource(new g.LatLng(this.SLeftTopLati, this.SLeftTopLongi), infoWindow, this.GeofenceNo, false, false));

            // Add the rectangle for this city to the map.
            var rectangle = new g.Rectangle(options);
            overlaysArray.baseType[i] = g.drawing.OverlayType.RECTANGLE;
            overlaysArray.rectangle[i] = rectangle;

            g.event.addListener(rectangle, "rightclick", function (mouseEvent) {
                unRegisterOverlay(overlaysArray.rectangle.indexOf(rectangle));
            });
            if (bounds) {
                bounds.extend(new g.LatLng(this.SLeftTopLati, this.SLeftTopLongi));
                bounds.extend(new g.LatLng(this.SRightBotLati, this.SRightBotLongi));
            }
            // Draw buffer rectangle
            if (this.GFMargin > 0) {
                var distance = parseInt(this.GFMargin) + parseInt(this.GFMargin * 0.3);
                options.bounds = new g.LatLngBounds(g.geometry.spherical.computeOffset(rectangle.getBounds().getSouthWest(), distance, -45),
                                g.geometry.spherical.computeOffset(rectangle.getBounds().getNorthEast(), distance, 135));
                options.fillColor = "#FFFF00";
                options.zIndex = 50;
                rectangle = new g.Rectangle(options);
                overlaysArray.polygon[i] = rectangle;
            }
        } else if (this.GFType == "P" || this.GFType == "L") {
            // for every new poly
            if (this.PSeq == 1) {
                path.length = 0;
                path = [];
            }
            i = this.PSeq - 1;
            path[i] = new g.LatLng(this.PLati, this.PLongi);

            if (bounds) bounds.extend(path[i]);
            if ((index == data.length - 1 || this.GeofenceNo != data[index + 1].GeofenceNo)) {
                infoWindow = "<table border=0 class='msgpopuptable'><tbody><tr><th>" + (this.GFType == "P" ? "Polygon" : "Polyline") + " Title:</th><td>" + this.GFTitle + "</td></tr>" +
                            "<tr><th>Name:</th><td>" + this.GFName + "</td></tr>" +
                            "<tr><th>Comment:</th><td>" + this.Comment + "</td></tr>" +
                            "<tr><th>Buffer:</th><td>" + this.GFMargin + "</td></tr>" +
                            "<tr><th>Latitude:</th><td>" + this.PLati + "</td></tr>" +
                            "<tr><th>Longitude:</th><td>" + this.PLongi + "</td></tr>" +
                            "<tr><th>Last Sequence:</th><td>" + this.PSeq + "</td></tr></tbody></table>";
                var arrowSymbol = {
                    path: g.SymbolPath.FORWARD_CLOSED_ARROW, //FORWARD_OPEN_ARROW
                    strokeColor: "#000",
                    strokeOpacity: 1
                };
                var poly = this.GFType == "P" ? new g.Polygon() : poly = new g.Polyline();

                poly.setOptions({
                    map: map,
                    path: path,
                    strokeColor: "#ff0000",
                    strokeWeight: 2,
                    //strokeOpacity: 1,
                    geodesic: true,
                    clickable: true,
                    icons: [
                        {
                            icon: arrowSymbol,
                            repeat: "1%",
                            fixedRotation: false
                        }
                    ],
                    zIndex: 3
                });

                i = overlaysArray.marker.indexOf(addMarkerForResource(path[0], infoWindow, this.GeofenceNo, false, false));

                if (this.GFType == "P") {
                    overlaysArray.polygon[i] = poly;
                    overlaysArray.baseType[i] = g.drawing.OverlayType.POLYGON;
                } else {
                    overlaysArray.polyline[i] = poly;
                    overlaysArray.baseType[i] = g.drawing.OverlayType.POLYLINE;
                }

                // Draw buffer polygon
                if (this.GFMargin > 0) {
                    var refVar = { routePolygon: null };
                    drawRoute(i, this.GFMargin, refVar);

                    // for buffer shape to save reverting polyline to polygon and vice versa
                    if (this.GFType == "P") overlaysArray.polyline[i] = refVar.routePolygon; else overlaysArray.polygon[i] = refVar.routePolygon;
                }
                // Initiating info window for animated asset will display on pause
                var infoWindowDynamic = new g.InfoWindow();
                g.event.addListener(poly, "mouseover", function (h) {
                    var needle = {
                        minDistance: 9999999999, //silly high
                        index: -1,
                        latlng: null
                    };
                    poly.getPath().forEach(function (routePoint, index) {
                        var dist = g.geometry.spherical.computeDistanceBetween(h.latLng, routePoint);
                        if (dist < needle.minDistance) {
                            needle.minDistance = dist;
                            needle.index = index;
                            needle.latlng = routePoint;
                        }
                    });
                    infoWindowDynamic.open(map);
                    infoWindowDynamic.setContent("<table border=0 ><tbody>" +
                                    "<tr><td>Latitude:" + h.latLng.lat() + "</td></tr>" +
                                    "<tr><td>Longitude:" + h.latLng.lng() + "</td></tr>" +
                                    "<tr><td colspan='3'>Location:<span id='googleMapPopLocId'>Loading.....</span></td></tr></tbody></table>");
                    infoWindowDynamic.setPosition(h.latLng);
                    new g.Geocoder().geocode({ "latLng": h.latLng },
                                    function (results, status) {
                                        if (status == g.GeocoderStatus.OK)
                                            if (results[0]) $("#googleMapPopLocId").html(results[0].formatted_address);
                                    });
                });
                g.event.addListener(poly, "mouseout", function () {
                    infoWindowDynamic.close();
                });
            }
        }
    });
}

function addMarkerForResource(latlng, toolTipHtml, uniqueNo, isOpen, isDraggable, arrayIndex, optimized, markerImage) {
    // create a marker
    var marker = new g.Marker({
        position: latlng,
        map: map,
        title: "Latitude: " + latlng.lat() + "  Longitude :" + latlng.lng(),
        draggable: isDraggable
    });

    if (optimized != undefined) marker.setOptimized = optimized;
    if (markerImage != undefined) marker.setIcon(markerImage);
    var infoWindow = null;
    if (toolTipHtml != undefined) { // Dont want to use infowindow
        infoWindow = new g.InfoWindow({ content: toolTipHtml });
        if (isOpen) infoWindow.open(map, marker);
    }
    if (arrayIndex != -1) { // Dont want to use
        if (arrayIndex == undefined) {
            registerOverlay(g.drawing.OverlayType.MARKER, uniqueNo, infoWindow, marker);
            if (infoWindow) {
                g.event.addListener(marker, "mouseover", function () {
                    var index = overlaysArray.marker.indexOf(marker);
                    infoWindow = overlaysArray.infoWindow[index];
                    infoWindow.open(map, marker);
                });

                g.event.addListener(marker, "mouseout", function () {
                    var index = overlaysArray.marker.indexOf(marker);
                    overlaysArray.infoWindow[index].close();
                });
            }
        } else
            registerOverlay(g.drawing.OverlayType.MARKER, uniqueNo, infoWindow, marker, null, null, null, null, arrayIndex);
    }
    return marker;
}

function drawRoute(index, distanceInMeters, refVar) {
    var overviewPath, overviewPathGeo = [];
    if (overlaysArray.polyline[index]) overviewPath = overlaysArray.polyline[index].getPath();
    else overviewPath = overlaysArray.polygon[index].getPath();

    for (var i = 0; i < overviewPath.length; i++)
        overviewPathGeo.push([overviewPath.getAt(i).lng(), overviewPath.getAt(i).lat()]);

    var distance = (distanceInMeters / 1000) / 111.12, //km
        geoInput = { type: "LineString", coordinates: overviewPathGeo };
    var geoReader = new jsts.io.GeoJSONReader(), geoWriter = new jsts.io.GeoJSONWriter();
    var geometry = geoReader.read(geoInput).buffer(distance);
    var polygon = geoWriter.write(geometry);

    var oLanLng = [], oCoordinates;
    oCoordinates = polygon.coordinates[0];
    for (i = 0; i < oCoordinates.length; i++)
        oLanLng.push(new g.LatLng(oCoordinates[i][1], oCoordinates[i][0]));

    if (refVar.routePolygon && refVar.routePolygon.setMap) refVar.routePolygon.setMap(null);
    refVar.routePolygon = new g.Polygon({
        paths: oLanLng,
        map: map
    });
    g.event.addListener(refVar.routePolygon, "rightclick", function () {
        this.setMap(null);
        //$("#btnSaveActivityRoute").hide();
    });
}

function registerOverlay(baseType, uniqueNo, infoWindow, marker, polygon, polyline, circle, rectangle, intervalId, arrayIndex) {
    if (arrayIndex == undefined) {
        overlaysArray.polygon.push(polygon);
        overlaysArray.circle.push(circle);
        overlaysArray.polyline.push(polyline);
        overlaysArray.rectangle.push(rectangle);
        overlaysArray.marker.push(marker);
        overlaysArray.infoWindow.push(infoWindow);
        overlaysArray.baseType.push(baseType);
        overlaysArray.uniqueNo.push(uniqueNo);
        overlaysArray.intervalId.push(intervalId);
    } else {
        unRegisterOverlay(arrayIndex);
        overlaysArray.polygon[arrayIndex] = polygon;
        overlaysArray.circle[arrayIndex] = circle;
        overlaysArray.polyline[arrayIndex] = polyline;
        overlaysArray.rectangle[arrayIndex] = rectangle;
        overlaysArray.marker[arrayIndex] = marker;
        overlaysArray.infoWindow[arrayIndex] = infoWindow;
        overlaysArray.baseType[arrayIndex] = baseType;
        overlaysArray.uniqueNo[arrayIndex] = uniqueNo;
        overlaysArray.intervalId[arrayIndex] = intervalId;
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
        overlaysArray.baseType.length = 0;
        overlaysArray.uniqueNo.length = 0;
        overlaysArray.intervalId.length = 0;
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
        deleteOverlay(overlaysArray.uniqueNo, 0);
        deleteOverlay(overlaysArray.intervalId, 0);
        overlaysArray.baseType[i] = null;
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
