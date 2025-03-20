var g = google.maps, countCallBacks = 0, bounds = new g.LatLngBounds(), gfAssets = null, areaToGF = { 0: "I", 1: "O", 2 : "B"}
    overlaysArray = {
        marker: [],
        infoWindow: [],
        polygon: [],
        circle: [],
        polyline: [],
        rectangle: [],
        baseType: [],
        uniqueNo: [],
        intervalId: []
    };
var karachi = new g.LatLng(24.893599, 67.027903),
            colorEnum = {
                Grey: "#ccc",
                Red: "#ff0000",
                Green: "#00ff00",
                Blue: "#003366",
                Yellow: "#f7ce00"
            },
            zoom = 17, map, partURL;

function initialize(mapCanvas, latLng, zoom) {
    partURL = "http://localhost:82/";
    var mapOptions = {
        center: latLng,
        zoom: zoom,
        //minZoom: minZoomLevel,
        //maxZoom: 9,
        mapTypeId: g.MapTypeId.ROADMAP, //.TERRAIN,
        mapTypeControl: true,
        mapTypeControlOptions:
        {
            mapTypeIds: [
                g.MapTypeId.ROADMAP,
                g.MapTypeId.TERRAIN,
                g.MapTypeId.HYBRID,
                g.MapTypeId.SATELLITE
            ],
            style: g.MapTypeControlStyle.DROPDOWN_MENU
        },
        overviewMapControl: true,
        overviewMapControlOptions: { opened: true },
        scaleControl: true,
        scaleControlOptions: { poistion: g.ControlPosition.TOP_LEFT },
        zoomControl: true,
        zoomControlOptions: { style: g.NavigationControlStyle.LARGE }, //SMALL 
        panControl: true,
        panControlOptions: { poistion: g.ControlPosition.LEFT_BOTTOM },
        rotateControl: true,
        rotateControlOptions: { poistion: g.ControlPosition.LEFT_BOTTOM },
        streetViewControl: true,
        streetViewControlOptions: { poistion: g.ControlPosition.RIGHT_CENTER }
    };
    map = new g.Map(document.getElementById(mapCanvas), mapOptions);


    $.get(partURL + urlGetGeoFencesByOrgCode, function (geoFenceData) {
        drawGeoFenceByType(geoFenceData, bounds);
        if (!bounds.isEmpty()) map.fitBounds(bounds);
        getFirstEdrGeoByRtcdttmAndUpdatePullDt();
    });
    return;
    countCallBacks++;
    $.get(partURL + urlGetAllAssetsForMonitoring, function (data) {
        countCallBacks++;
        $.get(partURL + urlGetGeofenseForMonitoring, function(geoFenceData) {
            drawGeoFenceByType(geoFenceData, bounds);
            countCallBacks++;
            $.get(partURL + urlGetGfAssetsForMonitoring, function(GfAssetsData) {
                gfAssets = GfAssetsData;
                $.each(data, function(i) {
                    getLastCdrByAssetNo(this.Id, bounds);
                });
            }).always(function () { reduceCallBacks(); });
            
        }).always(function () { reduceCallBacks(); });
    })
    //.done(function() { console.log("second success"); })
    //.fail(function (t, e, g) { console.log("error");  })
    .always(function () { reduceCallBacks(); });
}

function reduceCallBacks() {
    countCallBacks--;
    if ((!bounds.isEmpty()) && countCallBacks == 0)
        map.fitBounds(bounds);
}

function getLastCdrByAssetNo(assetNo, bounds) {
    if (bounds) countCallBacks++;
    $.get(partURL + urlLastCdr + assetNo, function (cdrs) {
        var cdr = cdrs[0], cdrLatLng = new g.LatLng(cdr.Latitude, cdr.Longitude);
        var speed = parseInt(cdr.Speed), ignition = (cdr.Ignition == "Iginition On"), color = "#888", marker, infoWindow ;

        if (speed > 0)
            color = "#393"; //#008000
        else if (speed == 0 && ignition == 1)
            color = "#0000FF";
        else if (speed == 0 && ignition == 0)
            color = "#888"; //"#008080";

        var overlayIndex = overlaysArray.uniqueNo.indexOf("M" + assetNo);
        if (overlayIndex > -1) {
            marker = overlaysArray.marker[overlayIndex];
            marker.setPosition(cdrLatLng);
            var icon = marker.getIcon();
            icon.rotation = parseInt(cdr.Angle);
            icon.strokeColor = color;
            marker.setIcon(marker.icon);

            overlaysArray.infoWindow[overlayIndex].setContent(
                "<table border=0 class='msgpopuptable'><tbody>" +
                "<tr><th>Date:</th><td>" + cdr.RTCDTTM.toString() + "</td></tr>" +
                "<tr><th>Vehicle #:</th><td>" + cdr.Plateid + "</td></tr>" +
                "<tr><th>Device #:</th><td>" + cdr.UnitId + "</td></tr>" +
                "<tr><th>Ignition:</th><td>" + cdr.Ignition.replace("Iginition ", "") + "</td></tr>" +
                "<tr><th>Speed(Kmph):</th><td>" + cdr.Speed + "</td></tr>" +
                "<tr><th>Latitude:</th><td>" + cdr.Latitude + "</td></tr>" +
                "<tr><th>Longitude:</th><td>" + cdr.Longitude + "</td></tr></tbody></table>");
        } else {
            marker = new g.Marker({
                position: cdrLatLng,
                map: map,
                draggable: false,
                optimized: true,
                icon: {
                    path: g.SymbolPath.FORWARD_CLOSED_ARROW, //g.SymbolPath.CIRCLE,
                    strokeColor: color,
                    scale: 6,
                    rotation: parseInt(cdr.Angle)
                }
            });

            infoWindow = new g.InfoWindow({
                disableAutoPan: true,
                content: "<table border=0 class='msgpopuptable'><tbody>" +
                    "<tr><th>Date:</th><td>" + cdr.RTCDTTM.toString() + "</td></tr>" +
                    "<tr><th>Vehicle #:</th><td>" + cdr.Plateid + "</td></tr>" +
                    "<tr><th>Device #:</th><td>" + cdr.UnitId + "</td></tr>" +
                    "<tr><th>Ignition:</th><td>" + cdr.Ignition.replace("Iginition ", "") + "</td></tr>" +
                    "<tr><th>Speed(Kmph):</th><td>" + cdr.Speed + "</td></tr>" +
                    "<tr><th>Latitude:</th><td>" + cdr.Latitude + "</td></tr>" +
                    "<tr><th>Longitude:</th><td>" + cdr.Longitude + "</td></tr></tbody></table>"
            });
            infoWindow.open(map, marker);
        }
        $.each(gfAssets, function(i) {
            if (this.AssetNo == assetNo) {
                var geoFenceIndex = overlaysArray.uniqueNo.indexOf(this.GeoFenceNo);
                var drawingOverlay;
                if (overlaysArray.baseType[geoFenceIndex] == g.drawing.OverlayType.POLYGON) {
                    drawingOverlay = (this.GFMargin != 0) ? overlaysArray.polyline[geoFenceIndex] : overlaysArray.polygon[geoFenceIndex]; //buffer check
                    console.log(cdrLatLng);
                    console.log(drawingOverlay);
                    console.log(geoFenceIndex);
                    var containsLocation = g.geometry.poly.containsLocation(cdrLatLng, drawingOverlay);
                        
                    if (this.DML_Type == null) this.DML_Type = areaToGF[this.AreaToGF];

                    if (areaToGF[this.AreaToGF] != "O") {
                        if (containsLocation && this.DML_Type == "O")
                            sendMessage(geoFenceIndex, cdr.Plateid, this.GFMargin != 0, "N");
                    } else if (areaToGF[this.AreaToGF] != "I")
                        if (!containsLocation && this.DML_Type == "I")
                            sendMessage(geoFenceIndex, cdr.Plateid, this.GFMargin != 0, "X");
                    
                    this.DML_Type = containsLocation ? "I" : "O";
                } 
                else if (overlaysArray.baseType[geoFenceIndex] == g.drawing.OverlayType.POLYLINE) {
                    drawingOverlay = (this.GFMargin != 0) ? overlaysArray.polygon[geoFenceIndex] : overlaysArray.polyline[geoFenceIndex]; //buffer check
                    if (this.GFMargin != 0) {
                        console.log(g.geometry.poly.containsLocation(cdrLatLng, overlaysArray.polygon[geoFenceIndex]));
                    } else {
                        console.log(g.geometry.poly.isLocationOnEdge(cdrLatLng, overlaysArray.polyline[geoFenceIndex]));
                    }
                } else if (overlaysArray.baseType[geoFenceIndex] == g.drawing.OverlayType.RECTANGLE) {

                } else if (overlaysArray.baseType[geoFenceIndex] == g.drawing.OverlayType.CIRCLE) {

                }
                //g.geometry.poly.isLocationOnEdge for polyline
            }
        });
        if (overlayIndex == -1) {
            registerOverlay(g.drawing.OverlayType.MARKER, "M" + assetNo, infoWindow, marker);

            g.event.addListener(marker, "mouseover", function() {
                var index = overlaysArray.marker.indexOf(marker);
                infoWindow = overlaysArray.infoWindow[index];
                infoWindow.open(map, marker);
            });

            g.event.addListener(marker, "mouseout", function() {
                var index = overlaysArray.marker.indexOf(marker);
                overlaysArray.infoWindow[index].close();
            });
        }
        if (bounds) bounds.extend(cdrLatLng);
        if (overlayIndex == -1) overlaysArray.intervalId[overlaysArray.uniqueNo.indexOf("M" + assetNo)] = setInterval(getLastCdrByAssetNo, parseInt($("#txtTime").val()) * 1000, assetNo, undefined);
    }).always(function () { if (bounds) reduceCallBacks(); });
}

function sendMessage(index, plateId, isBuffer, action) {
    var elem = document.createElement('div');
    elem.innerHTML = overlaysArray.infoWindow[index].getContent();

    console.log($(elem).find("tr:nth(0) td").text()); //Title
    console.log(plateId);
    console.log(action);
    console.log(isBuffer);

}

function timeIntervelChanged() {
    $.each(overlaysArray.baseType, function (i) {
        if (overlaysArray.baseType[i] == g.drawing.OverlayType.MARKER) {
            clearInterval(overlaysArray.intervalId[i]);
            overlaysArray.intervalId[i] = setInterval(getLastCdrByAssetNo, parseInt($("#txtTime").val()) * 1000, overlaysArray.uniqueNo[i].substring(1), undefined);
        }
    });
}

function getFirstEdrGeoByRtcdttmAndUpdatePullDt() {
    $.get(partURL + urlGetFirstEdrGeoByRtcdttmAndUpdatePullDt, function (edr) {
        var cdrLatLng = new g.LatLng(edr.Latitude, edr.Longitude);
        var containsLocation;
        $.each(overlaysArray.baseType, function (i) {

            if (overlaysArray.baseType[i] == g.drawing.OverlayType.POLYGON) 
                containsLocation = g.geometry.poly.containsLocation(cdrLatLng, overlaysArray.polygon[i]);
             else if (overlaysArray.baseType[i] == g.drawing.OverlayType.CIRCLE) 
                containsLocation = overlaysArray.circle[i].getBounds().contains(cdrLatLng);
             else if (overlaysArray.baseType[i] == g.drawing.OverlayType.RECTANGLE) 
                containsLocation = overlaysArray.rectangle[i].getBounds().contains(cdrLatLng);
            
            if (containsLocation) {
                $.post(partURL + urlUpdateEdrGeo + "?id=" + edr.Id + "&geoFenceNo=" + overlaysArray.uniqueNo + "&iOType=I");
                //$.post(partURL + urlUpdateEdrGeo, { id: edr.Id, geoFenceNo: overlaysArray.uniqueNo, iOType: "I" });
            };
        });
        if (!containsLocation)
            $.post(partURL + urlUpdateEdrGeo + "?id=" + edr.Id + "&geoFenceNo=0&iOType=O");
        })
    .always(function () { getFirstEdrGeoByRtcdttmAndUpdatePullDt(); });
}