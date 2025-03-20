function registerTrack(index) {
    $("#trackReplay").dialog({
        close: function () {
            clearTPath(index);
        }
    });
    var interval, path = overlaysArray.polyline[index].getPath(),
        polyline = overlaysArray.polyline[index],
        totalPathLength = g.geometry.spherical.computeLength(path), i = 0, offsetPercent = 0,
        trackPolyline = new g.Polyline({
            map: map,
            path: [path.getAt(0)],
            strokeColor: "#71cf90",
            strokeWeight: 6,
            geodesic: true,
            zIndex: 2
        }), svgVehicle = null,
        refVar = { routePolygon: null },
        // Declaring and initiating info window and index for animated asset will display on pause
        infoWindowDynamic = new g.InfoWindow(), indexForDynamicAsset, trackStepInMeters = 1;

    $("#trackReplay #trackTotalPathLength").html((totalPathLength / 1000).toFixed(2));
    $("#trackReplay #trackSlider").slider({
        range: "min",
        min: 0,
        max: g.geometry.spherical.computeLength(path),
        value: 0,
        slide: function (event, ui) {
            offsetPercent = (ui.value / totalPathLength) * 100;
            offsetPercent = animateShape(totalPathLength, offsetPercent);
        }
    });
    $("#trackReplay #trackSlider .ui-slider-handle").width("0.6em");

    $("#trackReplay #trackBackward").attr("disabled", "disabled");
    $("#trackReplay #trackStop").attr("disabled", "disabled");
    $("#trackReplay #trackForward").attr("disabled", "disabled");

    $("#trackReplay #trackPlay").off("click");
    $("#trackReplay #trackPlay").on("click", function (e) {
        if ($(this).is("[disabled]")) e.preventDefault(); //no action on disable
        var action = $(e.target).hasClass("ui-icon-play") ? "Play" : "Pause";
        if (action == "Play") {
            if (!interval) { //stopped not paused
                $("#trackReplay #trackBackward").removeAttr("disabled");
                $("#trackReplay #trackStop").removeAttr("disabled");
                $("#trackReplay #trackForward").removeAttr("disabled");
                overlaysArray.markerPolyline[index].setMap(null); //to hide markerpolyline

                //closing markers infowindow if its open while play the track vehicle
                if (overlaysArray.infoWindow[index].isOpen) overlaysArray.infoWindow[index].close();

                if (svgVehicle == null) svgVehicle = getVehicleTypeSVGOject(index);
                var icons = svgVehicle.getIconsWithChangeColor(null, colorEnum.Blue, false);
                icons.unshift(polyline.icons[0]);
                polyline.set("icons", icons);
            }
            interval = setInterval(function() {
                offsetPercent = animateShape(totalPathLength, offsetPercent);

                if (polylineData.length == i || offsetPercent >= 100) stopObjectWhileRunningOrPause();
            }, 500); //0.5 sec is fixed of interval
            infoWindowDynamic.close();
        } else {
            clearInterval(interval);

            infoWindowDynamic.open(map);
            var geocoder = new g.Geocoder();
            geocoder.geocode({ "latLng": new g.LatLng(polylineData[indexForDynamicAsset].Lati.toString(), polylineData[indexForDynamicAsset].Longi.toString()) },
                function(results, status) {
                    if (status == g.GeocoderStatus.OK)
                        if (results[0]) $("#googleMapPopLocId").html(results[0].formatted_address);
                });
        }
        $(e.target).toggleClass("ui-icon-play");
        $(e.target).toggleClass("ui-icon-pause");
    });

    $("#trackReplay #trackStop").off("click");
    $("#trackReplay #trackStop").click(function (e) {
        if ($(this).is("[disabled]")) e.preventDefault(); //no action on disable
        stopObjectWhileRunningOrPause();
    });

    function stopObjectWhileRunningOrPause() {
        $("#trackReplay #trackBackward").attr("disabled", "disabled");
        $("#trackReplay #trackStop").attr("disabled", "disabled");
        $("#trackReplay #trackForward").attr("disabled", "disabled");
        if ($("#trackPlay .ui-icon").hasClass("ui-icon-pause")) {
            $("#trackPlay .ui-icon").toggleClass("ui-icon-pause");
            $("#trackPlay .ui-icon").toggleClass("ui-icon-play");
        }
        i = 0;
        trackStepInMeters = 1;
        clearInterval(interval);
        interval = null;
        offsetPercent = animateShape(totalPathLength, -1);
        polyline.set("icons", [polyline.icons[0]]); // Remove all icons except direction arrows
        if (overlaysArray.markerPolyline[index] != null) overlaysArray.markerPolyline[index].setMap(map);

        infoWindowDynamic.close();
        $("#trackReplay #trackDate").html("");
        $("#trackReplay #imgTrackIgnition").removeAttr("src");
        $("#trackReplay #imgTrackSignal").removeAttr("src");
        $("#trackReplay #trackSpeed").html("");
        $("#trackReplay #trackLati").html("");
        $("#trackReplay #trackLongi").html("");
    }

    $("#trackReplay #trackBackward").off("click");
    $("#trackReplay #trackBackward").click(function (e) {
        if ($(this).is("[disabled]")) e.preventDefault(); //no action on disable
        if (trackStepInMeters - (polylineData.length * 0.05) >= 0)
            trackStepInMeters -= polylineData.length * 0.05;
    });
    $("#trackReplay #trackForward").off("click");
    $("#trackReplay #trackForward").click(function (e) {
        if ($(this).is("[disabled]")) e.preventDefault(); //no action on disable
        trackStepInMeters += polylineData.length * 0.05; //5 percentage of the full track
    });

    function animateShape(totalLength, percent) {
        if (percent == -1) {
            $("#trackReplay #trackSlider").slider("value", 0);
            $("#trackReplay #trackPathLength").html(0);
            getTotalLength(0, 0, true);
            percent = 0;
        } else {
            var icons = polyline.get("icons");
            if (icons.length < 2) return 0;
            percent += (trackStepInMeters / totalLength) * 100;
            for (var j = 1; j < icons.length; j++) {
                icons[j].offset = percent + "%";
            }
            polyline.set("icons", icons);
            $("#trackReplay #trackSlider").slider("value", (percent * totalLength) / 100);
            var lengthTillPoint = (((percent * totalLength) / 100) / 1000); //1000 m = 1 Kms
            $("#trackReplay #trackPathLength").html(lengthTillPoint.toFixed(2));
            getTotalLength(0, lengthTillPoint, true);
        }
        return percent;
    }

    function getTotalLength(fromIndex, toIndex, runOverPolyline) {
        var length = 0, j = 0, runOverPath = (toIndex > 0) ? [path.getAt(0)] : [];
        var lengthTillPoint = toIndex * 1000, twoPointsDistance;
        for (j = 0; j < path.getLength() - 1; j++) {
            length += twoPointsDistance = g.geometry.spherical.computeDistanceBetween(path.getAt(j), path.getAt(j + 1));
            if (length >= lengthTillPoint) {
                //length = Distance till 2nd point
                var distanceCoveredInTwoPoints = twoPointsDistance - (length - lengthTillPoint);
                var fraction = distanceCoveredInTwoPoints / twoPointsDistance;
                //console.log(length + "-" + twoPointsDistance + "-" + distanceCoveredInTwoPoints + "-" + fraction +"-" + j);
                var latLng = mercatorInterpolate(map, path.getAt(j), path.getAt(j + 1), fraction);
                runOverPath.push(latLng);
                break;
            }
            runOverPath.push(path.getAt(j + 1));
        }
        var speed = parseFloat(polylineData[j].Speed),
            ignition = parseInt(polylineData[j].DI1),
            icons;
        if (svgVehicle != null) {
            if (speed > 0)
                icons = svgVehicle.getIconsWithChangeColor(polyline.icons, colorEnum.Green, true);
            else if (speed == 0 && ignition == 1)
                icons = svgVehicle.getIconsWithChangeColor(polyline.icons, colorEnum.Blue, true);
            else if (speed == 0 && ignition == 0)
                icons = svgVehicle.getIconsWithChangeColor(polyline.icons, colorEnum.Grey, true);
            polyline.set("icons", icons);
        }

        //Setting up the grid under animation toolbar
        $("#trackReplay #trackDate").html(polylineData[j].RTCDTTM.toString());
        $("#trackReplay #imgTrackIgnition").attr({ src: imgIgintion(parseInt(polylineData[j].DI1)) });
        $("#trackReplay #imgTrackSignal").attr({ src: imgGSMSignals(polylineData[j].GSMSignals) });
        $("#trackReplay #trackSpeed").html(polylineData[j].Speed);
        $("#trackReplay #trackLati").html(parseFloat(polylineData[j].Lati).toFixed(5));
        $("#trackReplay #trackLongi").html(parseFloat(polylineData[j].Longi).toFixed(5));
        // Setting info window content and position for animated asset will display on pause
        infoWindowDynamic.setContent("<table border=0 class='msgpopuptable'><tbody>" +
                            "<tr><td><b>Date:</b></td><td>" + polylineData[j].RTCDTTM.toString() + "</td></tr>" +
                            "<tr><td><b>Ignition:</b></td><td>" + (parseInt(polylineData[j].DI1) ? "On" : "Off") + "</td></tr>" +
                            "<tr><td><b>GSM Signals:</b></td><td>" + polylineData[j].GSMSignals + "</td></tr>" +
                            "<tr><td><b>Speed(Kmph):</b></td><td>" + polylineData[j].Speed + "</td></tr>" +
                            "<tr><td><b>Latitude:</b></td><td>" + polylineData[j].Lati + "</td></tr>" +
                            "<tr><td><b>Longitude:</b></td><td>" + polylineData[j].Longi + "</td></tr>" +
                            "<tr><td><b>Location:</b></td><td><span id='googleMapPopLocId'>Loading.....</span></td></tr></tbody></table>");
        infoWindowDynamic.setPosition(path.getAt(j));
        indexForDynamicAsset = j;
        if (infoWindowDynamic.isOpen) {
            var geocoder = new g.Geocoder();
            geocoder.geocode({ "latLng": new g.LatLng(polylineData[j].Lati.toString(), polylineData[j].Longi.toString()) },
                function (results, status) {
                    if (status == g.GeocoderStatus.OK)
                        if (results[0]) $("#googleMapPopLocId").html(results[0].formatted_address);
                });
        }
        if (runOverPolyline) trackPolyline.setPath(runOverPath);
        return length;
    }

    $("#btnDrawActivityRoute").off("click");
    $("#btnDrawActivityRoute").click(function (e) {
        if (!validateActivityBuffer()) return;
        if (parseInt($("#txtActivityBuffer").val()) == 0 || $("#txtActivityBuffer").val() == "") {
            if (refVar.routePolygon != null) drawRoute(index, 0, refVar);
            saveRoute(index, parseInt($("#txtActivityBuffer").val()) || 0, polyline);
            $("#btnSaveActivityRoute").hide();
        } else {
            drawRoute(index, parseInt($("#txtActivityBuffer").val()) || 0, refVar);
            $("#btnSaveActivityRoute").show();
        }
    });
    
    $("#btnSaveActivityRoute").off("click");
    $("#btnSaveActivityRoute").click(function (e) {
        if (!validateActivityBuffer()) return;
        saveRoute(index, parseInt($("#txtActivityBuffer").val()) || 0, polyline);
        $(this).hide();
    });

    $("#btnShowTrackMapbyDate").off("click");
    $("#btnShowTrackMapbyDate").click(function () {
        if (refVar.routePolygon != null) refVar.routePolygon.setMap(null);
        $("#btnSaveActivityRoute").hide();
        drawTodaysTrack($("#lblUnitIdTrackMap").val(), $("#lblAssetNameTrackMap").val(), $("#txtTrackMapDate").val(), $("#txtTrackMapFromTime"), $("#txtTrackMapToTime"));
    });

    function clearTPath(idx) {
        clearTrackPath(idx);
        if (refVar.routePolygon != null) refVar.routePolygon.setMap(null);
    }
}

function clearTrackPath(idx) {
    $("#txtActivityBuffer").val("");
    $("#btnSaveActivityRoute").hide();
    $("#trackReplay #trackStop").click();
    deleteOverlay(overlaysArray.polyline, idx);
    deleteOverlay(overlaysArray.markerArray, idx);
    deleteOverlay(overlaysArray.infoWindowArray, idx);
    // to uncheck asset from treeview
    $("[index-overlay=" + idx + "]").prop("checked", false);
    toggleAsset($("[index-overlay=" + idx + "]")[0]);
}

function drawTodaysTrack(unitId, assetName, mapDate, fromTimeObj, toTimeObj) {
    if (!validateTime($(fromTimeObj).val())) {
        $(fromTimeObj).val("");
        alert("From time is not in correct format (HH:mm 24 hours).");
        $(fromTimeObj).focus();
        return;
    } else if (!validateTime($(toTimeObj).val())) {
        $(toTimeObj).val("");
        alert("To time is not in correct format (HH:mm 24 hours).");
        $(toTimeObj).focus();
        return;
    } else if (Date.parseExact($(fromTimeObj).val(), ["H:m"]) >= Date.parseExact($(toTimeObj).val(), ["H:m"])) {
        alert("From time should be less than To time.");
        $(toTimeObj).focus();
        return;
    }
    $.ajax({
        url: urlGetTodaysMap + "?unitId=" + unitId + "&ignition=1&fromDate=" + mapDate + "&toDate=" + mapDate + "&fromTime=" + $(fromTimeObj).val() + "&toTime=" + $(toTimeObj).val(),
        //url: urlGetTodaysTrack + "?unitId=" + unitId + "&mapDate=" + mapDate + "&fromTime=" + $(fromTimeObj).val() + "&toTime=" + $(toTimeObj).val(),
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            $(":checkbox[unitid=" + unitId + "]").parent().parent().parent().parent().parent().parent().showLoaderPanel();
            $(":checkbox[unitid=" + unitId + "]").attr("disabled", true);
        },
        complete: function () {
            $(":checkbox[unitid=" + unitId + "]").parent().parent().parent().parent().parent().parent().hideLoaderPanel();
            $(":checkbox[unitid=" + unitId + "]").removeAttr("disabled");
        },
        success: function (data, textStatus, xhr) {
            if (textStatus == "success") {
                if (data.length == 0) {
                    $("#trackReplay tr[name='trackControls'], #trackReplay table[name='trackControls']").hide();
                    if (!$("#trackReplay").dialog("isOpen")) {
                        if (!confirm("Vehicle movement not detected for today!\nDo you want to select another date/time?")) {
                            $("[unitid=" + unitId + "]").prop("checked", false);
                            toggleAsset($("[unitid=" + unitId + "]")[0]);
                            return;
                        }
                    } else 
                        alert("Vehicle movement not detected for the selected date/time!\nPlease select another date/time?");
                }
                if ($("#histroyTrackReplay").dialog("isOpen")) $("#histroyTrackReplay").dialog("close");
                if (!$("#trackReplay").dialog("isOpen")) $("#trackReplay").dialog("open");

                $("#lblUnitIdTrackMap").val(unitId);
                $("#lblAssetNameTrackMap").val(assetName);
                $("#trackReplay").dialog({ title: "Today's Activity of " + assetName });

                //delete all polylines
                $.each(overlaysArray.polyline, function (idx) {
                    if (overlaysArray.polyline[idx]) deleteOverlay(overlaysArray.polyline, idx);
                });
                //delete all infoWindowArray
                $.each(overlaysArray.infoWindowArray, function (idx) {
                    if (overlaysArray.infoWindowArray[idx]) deleteOverlay(overlaysArray.infoWindowArray, idx);
                });
                //delete all markerArray
                $.each(overlaysArray.markerArray, function (idx) {
                    if (overlaysArray.markerArray[idx]) deleteOverlay(overlaysArray.markerArray, idx);
                });

                if (data.length == 0) return;

                var path = [], bounds = new g.LatLngBounds(), length = 0, i = 0, distanceBetween;
                var markerArray = [], infoWindowArray = [], partUrl, pinStyle;
                var geocoder = new g.Geocoder();
                polylineData = [];
                $(data).each(function () {
                    if (parseFloat(this.Longi) != 0 && parseFloat(this.Lati) != 0) {
                        try {
                            path[i] = new g.LatLng(parseFloat(this.Lati), parseFloat(this.Longi));
                            if (i > 0) {
                                distanceBetween = g.geometry.spherical.computeDistanceBetween(path[i - 1], path[i]);
                                length += distanceBetween;
                                if (isNaN(length))
                                    alert("[" + i + "] length=" + length + " segment=" + distanceBetween);
                            }
                            bounds.extend(path[i]);
                        } catch (e) {
                            console.log(e.message.toString());
                        }
                        i++;
                    }
                });
                $("#trackReplay tr[name='trackControls'], #trackReplay table[name='trackControls']").show();
                var arrowSymbol = {
                    path: g.SymbolPath.FORWARD_CLOSED_ARROW, //FORWARD_OPEN_ARROW
                    strokeColor: "#000",
                    strokeOpacity: 1
                };
                var polyline = new g.Polyline({
                    map: map,
                    path: path,
                    strokeColor: "#ff0000",
                    strokeWeight: 2,
                    //strokeOpacity: 1,
                    geodesic: true,
                    clickable: true,
                    icons: [{ icon: arrowSymbol,
                        repeat: "1%",
                        fixedRotation: false
                    }],
                    zIndex: 3
                });
                // Initiating info window for animated asset will display on pause
                var infoWindowDynamic = new g.InfoWindow();

                var tid = setInterval(function () {//because histroyTrackReplay window is closing after the polyline is made
                    if (!$("#histroyTrackReplay").dialog("isOpen")) {
                        clearInterval(tid);
                        var index = $(":checkbox[unitid=" + unitId + "]").attr("index-overlay");
                        
                        overlaysArray.polyline[index] = polyline;
                        registerTrack(index);
                        map.fitBounds(bounds);

                        polylineData = data; ///polylineData = data;  for both together before

                        g.event.addListener(polyline, "mouseover", function (h) {
                            var latlng = h.latLng;
                            var needle = {
                                minDistance: 9999999999, //silly high
                                index: -1,
                                latlng: null
                            };
                            polyline.getPath().forEach(function (routePoint, index) {
                                var dist = g.geometry.spherical.computeDistanceBetween(latlng, routePoint);
                                if (dist < needle.minDistance) {
                                    needle.minDistance = dist;
                                    needle.index = index;
                                    needle.latlng = routePoint;
                                }
                            });
                            // The closest point in the polyline
                            //console.log("Closest index: " + needle.index);
                            // The clicked point on the polyline
                            //console.log(latlng);
                            var j = needle.index;

                            infoWindowDynamic.open(map);
                            infoWindowDynamic.setContent("<table border=0 class='msgpopuptable'><tbody>" +
                                "<tr><td><b>Date:</b></td><td>" + polylineData[j].RTCDTTM.toString() + "</td></tr>" +
                                "<tr><td><b>Ignition:</b></td><td>" + (parseInt(polylineData[j].DI1) ? "On" : "Off") + "</td></tr>" +
                                "<tr><td><b>GSM Signals:</b></td><td>" + polylineData[j].GSMSignals + "</td></tr>" +
                                "<tr><td><b>Speed(Kmph):</b></td><td>" + polylineData[j].Speed + "</td></tr>" +
                                "<tr><td><b>Latitude:</b></td><td>" + polylineData[j].Lati + "</td></tr>" +
                                "<tr><td><b>Longitude:</b></td><td>" + polylineData[j].Longi + "</td></tr>" +
                                "<tr><td><b>Location:</b></td><td><span id='googleMapPopLocIdTrack'>Loading.....</span></td></tr></tbody></table>");
                            infoWindowDynamic.setPosition(h.latLng);
                            geocoder.geocode({ "latLng": h.latLng }, function (results, status) {
                                if (status == g.GeocoderStatus.OK)
                                    if (results[0]) $("#googleMapPopLocIdTrack").html(results[0].formatted_address);
                                    else $("#googleMapPopLocIdTrack").html("Geocoder failed due to: " + status);
                            });
                        });
                        g.event.addListener(polyline, "mouseout", function () {
                            infoWindowDynamic.close();
                        });
                        $.ajax({
                            url: urlGetEventsExceptDrive + "?unitId=" + unitId + "&mapDate=" + mapDate + "&fromTime=" + $(fromTimeObj).val() + "&toTime=" + $(toTimeObj).val(),
                            type: "GET",
                            contentType: "application/json charset=utf-8",
                            dataType: "json",
                            beforeSend: function() {
                                $(":checkbox[unitid=" + unitId + "]").parent().parent().parent().parent().parent().parent().showLoaderPanel();
                                $(":checkbox[unitid=" + unitId + "]").attr("disabled", true);
                            },
                            complete: function() {
                                $(":checkbox[unitid=" + unitId + "]").parent().parent().parent().parent().parent().parent().hideLoaderPanel();
                                $(":checkbox[unitid=" + unitId + "]").removeAttr("disabled");
                            },
                            success: function(eventData, eventTextStatus, xhr1) {
                                if (eventTextStatus == "success") {
                                    $(eventData).each(function (i) {
                                        pinStyle = "pin";
                                        var zindex = 0;
                                        if (eventData[i].EventType == "R") {
                                            partUrl = "chst=d_map_xpin_icon&chld=" + pinStyle + "|petrol|FFFF00|0";
                                            zindex = 1000 * i;
                                        } else if (eventData[i].EventType == "N" || eventData[i].EventType == "K") {
                                            partUrl = "chst=d_map_xpin_icon&chld=" + pinStyle + "|parking|0000FF|0";
                                            zindex = 1 * i;
                                        } else if (eventData[i].EventType == "T") {
                                            //partUrl = "chst=d_map_xpin_letter&chld=" + pinStyle + "|T|FF0000|CCC|0";
                                            partUrl = "chst=d_map_xpin_icon&chld=" + pinStyle + "|caution|FF0000|0";
                                            //partUrl = "d_map_spin&chld=2.1|0|FF0000|13|b|T";
                                            zindex = 2000 * i;
                                        }

                                        infoWindowArray[i] = new g.InfoWindow({
                                            content: "<div id='iw-container'>" +
                                                "<div class='iw-title'><span id='googleMapPopLocIdArr" + infoWindowArray.length + "'>Loading.....</span></div>" +
                                                "<div class='iw-content'>" +
                                                "<table border=0 class='msgpopuptable'><tbody>" +
                                                "<tr><td><b>Event:</b></td><td class=eventtype-" + eventData[i].EventType.toLowerCase() + ">" + EventName(eventData[i].EventType) + "</td></tr>" +
                                                "<tr><td><b>Selected Date:</b></td><td>" + eventData[i].RTCDTTM.toString() + "</td></tr>" +
                                                "<tr><td><b>Start Date & Time:</b></td><td>" + eventData[i].OpenDt.toString() + "</td></tr>" +
                                                "<tr><td><b>Finish Date & Time:</b></td><td>" + eventData[i].FRTCDTTM.toString() + "</td></tr>" +
                                                "<tr><td><b>Duration (H:M:S):</b></td><td>" + eventData[i].TotDuration + "</td></tr>" +
                                                "<tr><td><b>Fuel Qty (Ltrs):</b></td><td>" + eventData[i].NetQty + "</td></tr>" +
                                                "<tr><td><b>Ignition:</b></td><td>" + (parseInt(eventData[i].DI1) ? "On" : "Off") + "</td></tr>" +
                                                "<tr><td><b>GSM Signals:</b></td><td>" + eventData[i].GSMSignals + "</td></tr>" +
                                                "<tr><td><b>Speed (Kmph):</b></td><td>" + eventData[i].Speed + "</td></tr>" +
                                                (eventData[i].EventType == "R" ? "<tr><td><b>Fuel Station:</b></td><td>" + eventData[i].PoiName + "</td></tr>" : "") +
                                                "<tr><td><b>Latitude:</b></td><td>" + eventData[i].Lati + "</td></tr>" +
                                                "<tr><td><b>Longitude:</b></td><td>" + eventData[i].Longi + "</td></tr>" +

                                                "</tbody></table>" +
                                                "</div>" +
                                                "<div class='iw-bottom-gradient'></div>" +
                                                "</div>"
                                            //, maxWidth: 300
                                        });
                                        markerArray[i] = new g.Marker({
                                            position: new g.LatLng(parseFloat(eventData[i].Lati), parseFloat(eventData[i].Longi)),
                                            map: map,
                                            title: "Latitude: " + eventData[i].Lati + " Longitude :" + eventData[i].Longi,
                                            draggable: false,
                                            optimized: false,
                                            icon: new g.MarkerImage("https://chart.googleapis.com/chart?" + partUrl, null, null, null, null),
                                            infoWindow: infoWindowArray[i],
                                            animation: g.Animation.DROP,
                                            zIndex: zindex
                                        });

                                        g.event.addListener(markerArray[i], "click", function() {
                                            infoWindowArray[i].open(map, markerArray[i]);
                                            geocoder.geocode({ "latLng": markerArray[i].getPosition() }, function(results, status) {
                                                if (status == g.GeocoderStatus.OK)
                                                    if (results[0]) $("#googleMapPopLocIdArr" + i).html(results[0].formatted_address);
                                                    else $("#googleMapPopLocIdArr" + i).html("Geocoder failed due to: " + status);
                                            });
                                        });
                                        // *
                                        // The g.event.addListener() event waits for
                                        // the creation of the infowindow HTML structure 'domready'
                                        // and before the opening of the infowindow, defined styles are applied.
                                        // *
                                        g.event.addListener(infoWindowArray[i], "domready", function() {
                                            // Reference to the DIV that wraps the bottom of infowindow
                                            var iwOuter = $(".gm-style-iw");

                                            // Reference to the div that contains the corners and tail elements.
                                            var iwCornersTail = iwOuter.prev();

                                            // Reference to the div that groups the close button elements.
                                            var iwBtnClose = iwOuter.next();

                                            //Removes left and top margins.
                                            // Sets a width 100% to fill the whole width of infowindow.
                                            iwOuter.css({ left: "0", top: "0", width: "100%" });

                                            // Moves the infowindow 115px to the right.
                                            //iw_outer.parent().parent().css({ left: "115px" });

                                            // Moves the shadow of the arrow 47px to the left margin.
                                            //iw_corners_tail.children(":nth-child(1)").css({ left: "47px" });

                                            // Moves the arrow 47px to the left margin.
                                            //iw_corners_tail.children(":nth-child(3)").css({ left: "47px" });

                                            // Applies a 10px radius to the bottom corners of the shadow.
                                            // Changes the desired shadow color.
                                            iwCornersTail.children(":nth-child(2)").css({ "border-bottom-left-radius": "10px", "border-bottom-right-radius": "10px", "background-color": "rgba(72, 181, 233, 0.4)" });

                                            // Applies 10px radius for the lower bottom corners 
                                            iwCornersTail.children(":nth-child(4)").css({ "border-bottom-left-radius": "10px", "border-bottom-right-radius": "10px" });

                                            // Changes the desired tail shadow color.
                                            iwCornersTail.children(":nth-child(3)").find("div").children().css({ "box-shadow": "rgba(72, 181, 233, 0.6) 0px 1px 6px" });

                                            // Apply the desired effect to the close button
                                            iwBtnClose.css({ opacity: "1", right: "-12px", top: "-12px", border: "7px solid #48b5e9", "border-radius": "13px", "box-shadow": "0 0 5px #3990B9" });

                                            // If the content of infowindow not exceed the set maximum height, then the gradient is removed.
                                            if ($(".iw-content").height() <= 200) {
                                                $(".iw-bottom-gradient").css({ display: "none" });
                                            }

                                            // The API automatically applies 0.7 opacity to the button after the mouseout event. This function reverses this event to the desired value.
                                            iwBtnClose.mouseout(function() {
                                                $(this).css({ opacity: "1" });
                                            });

                                            //Asif edit 
                                            var tableWidth = iwOuter.find("table").width();
                                            /*doesn't work :(
                                            iwCornersTail.children(":nth(1)").width(tableWidth + 9); //Resize Shadow according to table
                                            iwCornersTail.children(":nth(3)").width(tableWidth + 7); //Resize White background according to table
                                            */
                                            iwBtnClose.css({ left: tableWidth });

                                            // On Refuel extra row is added for Fuel Station(PoiName)
                                            if (eventData[i].EventType == "R") {
                                                iwOuter.find("table").width(0);
                                                $(".iw-content").css({ "max-height": 220 });
                                            }
                                        });
                                    });
                                    if (markerArray.length > 0) overlaysArray.markerArray[index] = markerArray;
                                    if (infoWindowArray.length > 0) overlaysArray.infoWindowArray[index] = infoWindowArray;
                                }
                            }
                        });
                    }
                }, 500);
            }
        }
    });
}

function EventName(eventType) {
    if (eventType == "N" || eventType == "K") 
        return "Parking";
    else if (eventType == "C")
        return "Driving";
    else if (eventType == "R")
        return "Refueling";
    else if (eventType == "T")
        return "Fuel Theft";
}
function initiateTrackMapByDate() {
    //Just initiating today's map dialog box and MapByDate button event
    $("#trackReplay").dialog({
        show: "blind",
        hide: "explode",
        title: "Animation Control",
        position: { my: "left+420 top+50", at: "left top" }, //[300, 100],
        autoOpen: false,
        height: 210, width: 400
    });

    $("#btnShowTrackMapbyDate").click(function () {
        drawTodaysTrack($("#lblUnitIdTrackMap").val(), $("#lblAssetNameTrackMap").val(), $("#txtTrackMapDate").val(), $("#txtTrackMapFromTime"), $("#txtTrackMapToTime"));
    });

    //Datepicker will hide in 2 secs need to do that because on window open it covers all the space even don't need to select date.
    $("#txtTrackMapDate").datepicker({
        dateFormat: "mm/dd/yy",
        showOn: "button",
        buttonImage: "/images/common/calendar.gif",
        buttonImageOnly: true,
        buttonText: "Select date"
    });
    $("#txtTrackMapDate").datepicker({
        beforeShow: datepicker_beforeShow,
        onClose: function () {
            $(window).unbind(".datepicker_beforeShow");
        }
    });
    $("#txtTrackMapFromTime").timepicker({
        timeFormat: "HH:mm",
        showOn: "button",
        buttonImage: "/images/common/clock.png",
        buttonImageOnly: true,
        buttonText: "Select from time"
    });
    $("#txtTrackMapToTime").timepicker({
        timeFormat: "HH:mm",
        showOn: "button",
        buttonImage: "/images/common/clock.png",
        buttonImageOnly: true,
        buttonText: "Select to time"
    });
}

function validateActivityBuffer() {
    if ($("#txtActivityBuffer").val() != "")
        if (isNaN($("#txtActivityBuffer").val())) {
            alert("Route buffer should be in integer.");
            $("#txtActivityBuffer").focus();
            return false;
        }
    if (parseInt($("#txtActivityBuffer").val()) < 0) {
        alert("Route buffer should be greater than or equals to zero.");
        $("#txtActivityBuffer").focus();
        return false;
    }
    return true;
}

function saveRoute(index, distanceInMeters, polylineToSave) {
    if (polylineToSave == null) {
        alert("Please draw geo fence first.");
        $("#btnShowTrackMapbyDate").focus();
        return false;
    }
    var originalContent;
    $("#geofenseMst").dialog({
        height: 300,
        width: 450,
        show: "blind",
        hide: "explode",
        title: "Route Geo Fence",
        modal: true,
        position: { my: "left+520 top+50", at: "left top" }, //[400, 100],
        autoOpen: false,
        close: function () {
            $("#geofenseMst").html(originalContent);
        }
    });
    originalContent = $("#geofenseMst").html();
    $("#geofenseMst").dialog("open");


    $("#selGeoFenseOrganizationByUser").hide();
    $("#selGeoFenseType").hide();
    $("#txtGeoMargin").hide();

    $("#selGeoFenseOrganizationByUser").closest("td").css({ height: "26px" });
    $("#selGeoFenseOrganizationByUser").closest("td").html("<p>" + $("[index-overlay=" + index + "]").closest("li[id*='-']").children("span").html() + "</p>");
    $("#selGeoFenseType").closest("td").css({ height: "26px" });
    $("#selGeoFenseType").closest("td").html("<p>Polyline</p>");
    $("#txtGeoMargin").closest("td").css({ height: "26px" });
    $("#txtGeoMargin").closest("td").html("<p>" + $("#txtActivityBuffer").val() + "</p>");

    var orgCode = $("[index-overlay=" + index + "]").closest("li[id*='-']").prop("id").split("-")[1];
    // on selection of organization bring company list in listbox
    $.ajax({
        url: urlGetCompaniesbyOrgCode + "?orgCode=" + orgCode,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function(data) {
            var bindData = { org: data };
            var output, template;
            if (data.length == 0) {
                template = "<option value=''>No Company Found</option>";
            } else {
                output = "<option value=''>Select Company</option>";
                template = output + "{{#org}}<option value={{Id}}>{{CompanyName}}</option>{{/org}}";
            }
            var html = Mustache.to_html(template, bindData);
            $("#selGeoFenseCompanyByUser").html(html);
            $("#selGeoFenseCompanyByUser").show();

            $("#btnGeoFenseSave").click(function() {
                if ($("#txtGeoTitle").val() == "") {
                    alert("Please select Title");
                    $("#txtGeoTitle").focus();
                    return false;
                } else if ($("#txtGeoName").val() == "") {
                    alert("Please select Name");
                    $("#txtGeoName").focus();
                    return false;
                }
                var postData = [];
                $(polylineToSave.getPath().getArray()).each(function (i, plnglat) {
                    postData.push({ Lati: plnglat.lat(), Longi: plnglat.lng(), Sequence: i + 1 });
                });
                $.ajax({
                    method: "POST",
                    url: urlAddPolyGeofencedtl + "?companyCode=" + $("#selGeoFenseCompanyByUser").val() + "&gfType=L" //Polyline
                            + "&gfTitle=" + $("#txtGeoTitle").val() + "&gfName=" + $("#txtGeoName").val() + "&gfMargin=" + $("#txtActivityBuffer").val()
                            + "&comment=" + $("#txtGeoComments").val() + "&orgCode=" + orgCode,
                    //contentType: "application/json charset=utf-8",
                    dataType: "json",
                    data: { "": postData },
                    beforeSend: function () {
                        $("#geoFenceLoader").css({ left: $("#geofenseMst").parent().css("left") });
                        $("#geoFenceLoader").css({ top: $("#geofenseMst").parent().css("top") });
                        $("#geofenseMst").dialog({
                            closeOnEscape: false,
                            beforeClose: function (event, ui) { return false; },
                            dialogClass: "noclose",
                            draggable: false
                        });
                        $("#geoFenceLoader").fadeIn({
                            complete: function () {
                                $("#geofenseMst table").fadeOut();
                            }
                        });
                    },
                    complete: function () {
                        $("#geoFenceLoader").fadeOut({
                            complete: function () {
                                $("#geofenseMst").dialog({
                                    closeOnEscape: true,
                                    beforeClose: function (event, ui) { return true; },
                                    dialogClass: "",
                                    draggable: true
                                });
                                $("#geofenseMst table").fadeIn();
                                $("#geofenseMst").dialog("close");
                            }
                        });
                    },
                    success: function (data, textStatus, xhr) {
                        if (textStatus == "success") {
                            if (data) 
                                alert("Route geo fence has been saved successfully.");
                            else 
                                alert("An error occur while saving route geo fence. Contact the administrator");
                        }
                    }
                });
            });
        }
    });
    $("#btnGeoFenceCancel").click(function () {
        $("#geofenseMst").dialog("close");
    });
}

function drawRoute(index, distanceInMeters, refVar) {
    var overviewPath = overlaysArray.polyline[index].getPath(), overviewPathGeo = [];
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
        $("#btnSaveActivityRoute").hide();
    });
}