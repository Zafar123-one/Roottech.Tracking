function registerAnimationControlButtons(index, todayOnly) {
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
        // Declaring and initiating info window and index for animated asset will display on pause
        infoWindowDynamic = new g.InfoWindow(), indexForDynamicAsset, trackStepInMeters = 1;

    $("#trackBackward").attr("disabled", "disabled");
    $("#trackStop").attr("disabled", "disabled");
    $("#trackForward").attr("disabled", "disabled");

    if (todayOnly) {
        $("#histroyTrackReplay").dialog({
            close: function () {
                clearAnimatePath(index);
            }
        });

        $("#histroyTrackReplay #trackTotalPathLength").html((totalPathLength / 1000).toFixed(2));
        $("#histroyTrackReplay #trackSlider").slider({
            range: "min",
            min: 0,
            max: g.geometry.spherical.computeLength(path),
            value: 0,
            slide: function (event, ui) {
                offsetPercent = (ui.value / totalPathLength) * 100;
                offsetPercent = animateShape(totalPathLength, offsetPercent);
            }
        });
        $("#histroyTrackReplay #trackSlider .ui-slider-handle").width("0.6em");

        $("#histroyTrackReplay #trackAnimationObject").off('change');
        $("#histroyTrackReplay #trackAnimationObject").change(function () {
            $("#histroyTrackReplay #trackSlider").slider("option", "max",
                g.geometry.spherical.computeLength(path));
        });

        $('#histroyTrackReplay #trackPlay').off('click');
        $("#histroyTrackReplay #trackPlay").click(function (e) {
            if ($(this).is("[disabled]")) e.preventDefault(); //no action on disable
            var action = $(e.target).hasClass("ui-icon-play") ? "Play" : "Pause"; //$(this).val();
            if (action == "Play") {
                if (!interval) { //stopped not paused
                    $("#histroyTrackReplay #trackBackward").removeAttr("disabled");
                    $("#histroyTrackReplay #trackStop").removeAttr("disabled");
                    $("#histroyTrackReplay #trackForward").removeAttr("disabled");
                    $("#histroyTrackReplay #trackIgnition").attr("disabled", "disabled");
                    $("#histroyTrackReplay #trackAnimationObject").attr("disabled", "disabled");
                    overlaysArray.markerPolyline[index].setMap(null); //to hide markerpolyline

                    //closing markers infowindow if its open while play the track vehicle
                    if (overlaysArray.infoWindow[index].isOpen) overlaysArray.infoWindow[index].close();

                    if ($("#histroyTrackReplay #trackAnimationObject").val() == "S") {
                        var shapeSymbol = { path: g.SymbolPath.CIRCLE, strokeColor: "#393", scale: 6 };
                        var openArrowSymbol = { path: g.SymbolPath.FORWARD_OPEN_ARROW, strokeColor: "#004e8a", scale: 6 };
                        polyline.set('icons', [polyline.icons[0], { icon: openArrowSymbol, offset: '0%' }, { icon: shapeSymbol, offset: '0%' }]);
                    } else if ($("#histroyTrackReplay #trackAnimationObject").val() == "G") {
                        if (svgVehicle == null) svgVehicle = getVehicleTypeSVGOject(index);
                        var icons = svgVehicle.getIconsWithChangeColor(null, colorEnum.Blue, false);
                        icons.unshift(polyline.icons[0]);
                        polyline.set('icons', icons);
                    }
                }
                interval = setInterval(function () {
                    offsetPercent = animateShape(totalPathLength, offsetPercent);

                    if (polylineData.length == i || offsetPercent >= 100) stopObjectWhileRunningOrPause();
                }, 500); //0.5 sec is fixed of interval//, parseInt($("#trackTime").val() == "" ? 3 : $("#trackTime").val()) * 1000);
                infoWindowDynamic.close();
            } else {
                clearInterval(interval);

                infoWindowDynamic.open(map);
                var geocoder = new g.Geocoder();
                geocoder.geocode({ 'latLng': new g.LatLng(polylineData[indexForDynamicAsset].Lati.toString(), polylineData[indexForDynamicAsset].Longi.toString()) },
                    function (results, status) {
                        if (status == g.GeocoderStatus.OK)
                            if (results[0]) $("#googleMapPopLocId").html(results[0].formatted_address);
                    });
            }
            $(e.target).toggleClass("ui-icon-play");
            $(e.target).toggleClass("ui-icon-pause");
        });

        $('#histroyTrackReplay #trackStop').off('click');
        $("#histroyTrackReplay #trackStop").click(function (e) {
            if ($(this).is("[disabled]")) e.preventDefault(); //no action on disable
            $("#trackIgnition").removeAttr("disabled");
            $("#histroyTrackReplay #trackAnimationObject").removeAttr("disabled");
            stopObjectWhileRunningOrPause();
        });

        $('#histroyTrackReplay #trackBackward').off('click');
        $("#histroyTrackReplay #trackBackward").click(function (e) {
            if ($(this).is("[disabled]")) e.preventDefault(); //no action on disable
            if (trackStepInMeters - (polylineData.length * 0.05) >= 0)
                trackStepInMeters -= polylineData.length * 0.05;
        });
        $('#histroyTrackReplay #trackForward').off('click');
        $("#histroyTrackReplay #trackForward").click(function (e) {
            if ($(this).is("[disabled]")) e.preventDefault(); //no action on disable
            trackStepInMeters += polylineData.length * 0.05; //5 percentage of the full track
        });

        $("#trackIgnition").off("change");
        $("#trackIgnition").change(function () {
            drawTodaysPath($("[index-overlay=" + index + "]").attr("unitid"), $("[index-overlay=" + index + "]").parent().parent().text(), $(this).val(), $("#txt_MapDate").val(), $("#txt_MapDate").val(), $("#txt_MapDate").val(), $("#txtMapFromTime"), $("#txtMapToTime"));
        });
    }
    else {
        $("#periodTrackReplay").dialog({
            close: function () {
                clearAnimatePath(index);
            }
        });
        $("#periodTrackReplay #trackTotalPathLength").html((totalPathLength / 1000).toFixed(2));
        $("#periodTrackReplay #trackSlider").slider({
            range: "min",
            min: 0,
            max: g.geometry.spherical.computeLength(path),
            value: 0,
            slide: function (event, ui) {
                offsetPercent = (ui.value / totalPathLength) * 100;
                offsetPercent = animateShape(totalPathLength, offsetPercent);
            }
        });
        $("#periodTrackReplay #trackAnimationObject").off('change');
        $("#periodTrackReplay #trackAnimationObject").change(function () {
            $("#periodTrackReplay #trackSlider").slider("option", "max",
                g.geometry.spherical.computeLength(path));
        });

        $('#periodTrackReplay #trackPlay').off('click');
        $("#periodTrackReplay #trackPlay").click(function (e) {
            if ($(this).is("[disabled]")) e.preventDefault(); //no action on disable
            var action = $(e.target).hasClass("ui-icon-play") ? "Play" : "Pause"; //$(this).val();
            if (action == "Play") {
                if (!interval) { //stopped not paused
                    $("#periodTrackReplay #trackBackward").removeAttr("disabled");
                    $("#periodTrackReplay #trackStop").removeAttr("disabled");
                    $("#periodTrackReplay #trackForward").removeAttr("disabled");
                    $("#periodTrackReplay #trackIgnition").attr("disabled", "disabled");
                    $("#periodTrackReplay #trackAnimationObject").attr("disabled", "disabled");
                    overlaysArray.markerPolyline[index].setMap(null); //to hide markerpolyline

                    //closing markers infowindow if its open while play the track vehicle
                    if (overlaysArray.infoWindow[index].isOpen) overlaysArray.infoWindow[index].close();

                    if ($("#periodTrackReplay #trackAnimationObject").val() == "S") {
                        var shapeSymbol = { path: g.SymbolPath.CIRCLE, strokeColor: "#393", scale: 6 };
                        var openArrowSymbol = { path: g.SymbolPath.FORWARD_OPEN_ARROW, strokeColor: "#004e8a", scale: 6 };
                        polyline.set('icons', [polyline.icons[0], { icon: openArrowSymbol, offset: '0%' }, { icon: shapeSymbol, offset: '0%' }]);
                    } else if ($("#periodTrackReplay #trackAnimationObject").val() == "G") {
                        if (svgVehicle == null) svgVehicle = getVehicleTypeSVGOject(index);
                        var icons = svgVehicle.getIconsWithChangeColor(null, colorEnum.Blue, false);
                        icons.unshift(polyline.icons[0]);
                        polyline.set('icons', icons);
                    }
                }
                interval = setInterval(function () {
                    offsetPercent = animateShape(totalPathLength, offsetPercent);

                    if (polylineData.length == i || offsetPercent >= 100) stopObjectWhileRunningOrPause();
                }, 500); //0.5 sec is fixed of interval//, parseInt($("#trackTime").val() == "" ? 3 : $("#trackTime").val()) * 1000);
                infoWindowDynamic.close();
            } else {
                clearInterval(interval);

                infoWindowDynamic.open(map);
                var geocoder = new g.Geocoder();
                geocoder.geocode({ 'latLng': new g.LatLng(polylineData[indexForDynamicAsset].Lati.toString(), polylineData[indexForDynamicAsset].Longi.toString()) },
                    function (results, status) {
                        if (status == g.GeocoderStatus.OK)
                            if (results[0]) $("#googleMapPopLocId").html(results[0].formatted_address);
                    });
            }
            $(e.target).toggleClass("ui-icon-play");
            $(e.target).toggleClass("ui-icon-pause");
        });

        $('#periodTrackReplay #trackStop').off('click');
        $("#periodTrackReplay #trackStop").click(function (e) {
            if ($(this).is("[disabled]")) e.preventDefault(); //no action on disable
            $("#trackIgnition").removeAttr("disabled");
            $("#periodTrackReplay #trackAnimationObject").removeAttr("disabled");
            stopObjectWhileRunningOrPause();
        });

        $('#periodTrackReplay #trackBackward').off('click');
        $("#periodTrackReplay #trackBackward").click(function (e) {
            if ($(this).is("[disabled]")) e.preventDefault(); //no action on disable
            if (trackStepInMeters - (polylineData.length * 0.05) >= 0)
                trackStepInMeters -= polylineData.length * 0.05;
        });
        $('#periodTrackReplay #trackForward').off('click');
        $("#periodTrackReplay #trackForward").click(function (e) {
            if ($(this).is("[disabled]")) e.preventDefault(); //no action on disable
            trackStepInMeters += polylineData.length * 0.05; //5 percentage of the full track
        });

        $("#trackPeriodIgnition").off("change");
        $("#trackPeriodIgnition").change(function () {
            drawTodaysPath($("[index-overlay=" + index + "]").attr("unitid"), $("[index-overlay=" + index + "]").parent().parent().text(), $(this).val(), $("#txt_MapDate").val(), $("#txt_MapDate").val(), $("#txt_MapDate").val(), $("#txtMapFromTime"), $("#txtMapToTime"));
        });
    }
    function stopObjectWhileRunningOrPause() {
        i = 0;
        trackStepInMeters = 1;
        clearInterval(interval);
        interval = null;
        offsetPercent = animateShape(totalPathLength, -1);
        polyline.set('icons', [polyline.icons[0]]); // Remove all icons except direction arrows
        if (overlaysArray.markerPolyline[index] != null) overlaysArray.markerPolyline[index].setMap(map);

        infoWindowDynamic.close();

        if (todayOnly) {
            $("#histroyTrackReplay #trackBackward").attr("disabled", "disabled");
            $("#histroyTrackReplay #trackStop").attr("disabled", "disabled");
            $("#histroyTrackReplay #trackForward").attr("disabled", "disabled");
            if ($("#histroyTrackReplay #trackPlay .ui-icon").hasClass("ui-icon-pause")) {
                $("#histroyTrackReplay #trackPlay .ui-icon").toggleClass("ui-icon-pause");
                $("#histroyTrackReplay #trackPlay .ui-icon").toggleClass("ui-icon-play");
            }
            $("#histroyTrackReplay #trackDate").html("");
            $("#histroyTrackReplay #imgTrackIgnition").removeAttr("src");
            $("#histroyTrackReplay #imgTrackSignal").removeAttr("src");
            $("#histroyTrackReplay #trackSpeed").html("");
            $("#histroyTrackReplay #trackLati").html("");
            $("#histroyTrackReplay #trackLongi").html("");
        }
        else {
            $("#periodTrackReplay #trackBackward").attr("disabled", "disabled");
            $("#periodTrackReplay #trackStop").attr("disabled", "disabled");
            $("#periodTrackReplay #trackForward").attr("disabled", "disabled");
            if ($("#periodTrackReplay #trackPlay .ui-icon").hasClass("ui-icon-pause")) {
                $("#periodTrackReplay #trackPlay .ui-icon").toggleClass("ui-icon-pause");
                $("#periodTrackReplay #trackPlay .ui-icon").toggleClass("ui-icon-play");
            }
            $("#periodTrackReplay #trackDate").html("");
            $("#periodTrackReplay #imgTrackIgnition").removeAttr("src");
            $("#periodTrackReplay #imgTrackSignal").removeAttr("src");
            $("#periodTrackReplay #trackSpeed").html("");
            $("#periodTrackReplay #trackLati").html("");
            $("#periodTrackReplay #trackLongi").html("");
        }
    }
    function animateBySteps(animationObject, step, sign) {
        var nextStep, leftSide, rightSide, greaterAndLesserCondition;
        step = (step / 2);
        if (sign == "+") {
            nextStep = leftSide = offsetPercent + step;
            rightSide = 100;
            greaterAndLesserCondition = leftSide < rightSide;
        } else {
            nextStep = offsetPercent - step;
            leftSide = offsetPercent;
            rightSide = step;
            greaterAndLesserCondition = leftSide > rightSide;
        }
        if (leftSide == rightSide || greaterAndLesserCondition) {
            offsetPercent = nextStep;
            animateShape(totalPathLength, offsetPercent);
            //console.log(offsetPercent);
        }
    }
    function animateShape(totalLength, percent) {
        if (percent == -1) {
            if (todayOnly) {
                $("#histroyTrackReplay #trackSlider").slider("value", 0);
                $("#histroyTrackReplay #trackPathLength").html(0);
                getTotalLength($("#histroyTrackReplay #trackAnimationObject").val(), 0, 0, true);
            }
            else
            {
                $("#periodTrackReplay #trackSlider").slider("value", 0);
                $("#periodTrackReplay #trackPathLength").html(0);
                getTotalLength($("#periodTrackReplay #trackAnimationObject").val(), 0, 0, true);
            }
            percent = 0;
        } else {
            var icons = polyline.get('icons');
            if (icons.length < 2) return 0;
            percent += (trackStepInMeters / totalLength) * 100;
            for (var j = 1; j < icons.length; j++) {
                icons[j].offset = percent + "%";
            }
            polyline.set('icons', icons);
            var lengthTillPoint = (((percent * totalLength) / 100) / 1000); //1000 m = 1 Kms
            if (todayOnly) {
                $("#histroyTrackReplay #trackSlider").slider("value", (percent * totalLength) / 100);
                $("#histroyTrackReplay #trackPathLength").html(lengthTillPoint.toFixed(2));
                getTotalLength($("#histroyTrackReplay #trackAnimationObject").val(), 0, lengthTillPoint, true);
            }
            else
            {
                $("#periodTrackReplay #trackSlider").slider("value", (percent * totalLength) / 100);
                $("#periodTrackReplay #trackPathLength").html(lengthTillPoint.toFixed(2));
                getTotalLength($("#periodTrackReplay #trackAnimationObject").val(), 0, lengthTillPoint, true);
            }
        }
        return percent;
    }
    function getTotalLength(animationObject, fromIndex, toIndex, runOverPolyline) {
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
        if (animationObject == "G") {
            var speed = parseFloat(polylineData[j].Speed),
                ignition = parseInt(polylineData[j].DI1), icons;
            if (svgVehicle != null) {
                if (speed > 0)
                    icons = svgVehicle.getIconsWithChangeColor(polyline.icons, colorEnum.Green, true);
                else if (speed == 0 && ignition == 1)
                    icons = svgVehicle.getIconsWithChangeColor(polyline.icons, colorEnum.Blue, true);
                else if (speed == 0 && ignition == 0)
                    icons = svgVehicle.getIconsWithChangeColor(polyline.icons, colorEnum.Grey, true);
                //icons.unshift(polyline.icons[0]);
                polyline.set('icons', icons);
            }
        }
        //Setting up the grid under animation toolbar
        if (todayOnly) {
            $("#histroyTrackReplay #trackDate").html(polylineData[j].RTCDTTM.toString());
            $("#histroyTrackReplay #imgTrackIgnition").attr({ src: imgIgintion(parseInt(polylineData[j].DI1)) });
            $("#histroyTrackReplay #imgTrackSignal").attr({ src: imgGSMSignals(polylineData[j].GSMSignals) });
            $("#histroyTrackReplay #trackSpeed").html(polylineData[j].Speed);
            $("#histroyTrackReplay #trackLati").html(parseFloat(polylineData[j].Lati).toFixed(5));
            $("#histroyTrackReplay #trackLongi").html(parseFloat(polylineData[j].Longi).toFixed(5));
        }
        else {
            $("#periodTrackReplay #trackDate").html(polylineData[j].RTCDTTM.toString());
            $("#periodTrackReplay #imgTrackIgnition").attr({ src: imgIgintion(parseInt(polylineData[j].DI1)) });
            $("#periodTrackReplay #imgTrackSignal").attr({ src: imgGSMSignals(polylineData[j].GSMSignals) });
            $("#periodTrackReplay #trackSpeed").html(polylineData[j].Speed);
            $("#periodTrackReplay #trackLati").html(parseFloat(polylineData[j].Lati).toFixed(5));
            $("#periodTrackReplay #trackLongi").html(parseFloat(polylineData[j].Longi).toFixed(5));
        }
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
            geocoder.geocode({ 'latLng': new g.LatLng(polylineData[j].Lati.toString(), polylineData[j].Longi.toString()) },
                function (results, status) {
                    if (status == g.GeocoderStatus.OK)
                        if (results[0]) $("#googleMapPopLocId").html(results[0].formatted_address);
                });
        }
        if (runOverPolyline) trackPolyline.setPath(runOverPath);
        return length;
    }

    function clearAnimatePath(idx) {
        clearPath(idx, todayOnly);
        trackPolyline.setMap(null);
    }
}

function clearPath(idx, todayOnly) {
    if (todayOnly)
        $("#histroyTrackReplay #trackStop").click();
    else
        $("#periodTrackReplay #trackStop").click();
    deleteOverlay(overlaysArray.polyline, idx);
    // to uncheck asset from treeview
    $("[index-overlay=" + idx + "]").prop('checked', false);
    toggleAsset($("[index-overlay=" + idx + "]")[0]);
}

function getStateOfAssetImage(speed, ignition) {
    if (speed > 0)
        return colorEnum.Green;
    else if (speed == 0 && ignition == 1)
        return colorEnum.Blue;
    else if (speed == 0 && ignition == 0)
        return colorEnum.Grey;
    return colorEnum.Yellow;
}
function validateTime(input) {
    return Date.parseExact(input, [
            "H:m",
            "h:mt",
            "h:m t",
            "ht","h t"]) != null ||
        Date.parseExact(input, [
            "h:mtt",
            "h:m tt",
            "htt","h tt"]) != null;
};
var polylineData;
function drawTodaysPath(unitId, assetName, ignition, fromDate, toDate, fromTimeObj, toTimeObj, todayOnly = true) {
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
    if (!todayOnly)
        if (Date.parseExact(fromDate, ["dd/MM/yyyy"]) < Date.parseExact(fromDate, ["dd/MM/yyyy"])) {
            alert("From date should be less than To date.");
            $(toTimeObj).focus();
            return;
        }

    $.ajax({
        url: urlGetTodaysMap + "?unitId=" + unitId + "&ignition=" + ignition + "&fromDate=" + fromDate + "&toDate=" + toDate + "&fromTime=" + $(fromTimeObj).val() + "&toTime=" + $(toTimeObj).val(),
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
                    $("tr[name='trackControls'], table[name='trackControls']").hide();
                    if (!$("#histroyTrackReplay").dialog("isOpen") || !$("#periodTrackReplay").dialog("isOpen")) {
                        if (!confirm("Vehicle movement not detected for today!\nDo you want to select another date/time?")) {
                            $("[unitid=" + unitId + "]").prop('checked', false);
                            toggleAsset($("[unitid=" + unitId + "]")[0]);
                            return;
                        }
                    } else
                        alert("Vehicle movement not detected for the selected date/time!\nPlease select another date/time?");
                }

                if ($("#trackReplay").dialog("isOpen")) $("#trackReplay").dialog("close");
                if (todayOnly) {
                    if ($("#periodTrackReplay").dialog("isOpen")) $("#periodTrackReplay").dialog("close");
                    if (!$("#histroyTrackReplay").dialog("isOpen")) $("#histroyTrackReplay").dialog("open");
                    $("#lblUnitIdMap").val(unitId);
                    $("#lblAssetNameMap").val(assetName);
                    $("#histroyTrackReplay").dialog({ title: "Today's Drive for " + assetName });
                }
                else {
                    if (!$("#periodTrackReplay").dialog("isOpen")) $("#periodTrackReplay").dialog("open");
                    if ($("#histroyTrackReplay").dialog("isOpen")) $("#histroyTrackReplay").dialog("close");
                }

                //delete all polylines
                $.each(overlaysArray.polyline, function (idx) {
                    if (overlaysArray.polyline[idx]) deleteOverlay(overlaysArray.polyline, idx);
                });
                
                if (data.length == 0) return;

                var path = [], bounds = new g.LatLngBounds(), length = 0, i = 0, distanceBetween;
                var geocoder = new g.Geocoder();
                $(data).each(function () {
                    if (parseFloat(this.Longi) != 0 && parseFloat(this.Lati) != 0) {
                        try {
                            //path.push(new g.LatLng(parseFloat(this.Lati), parseFloat(this.Longi)));
                            path[i] = new g.LatLng(parseFloat(this.Lati), parseFloat(this.Longi));
                            if (i > 0) {
                                distanceBetween = g.geometry.spherical.computeDistanceBetween(path[i - 1], path[i]);
                                length += distanceBetween;
                                //path[i - 1].distanceFrom(path[i]);
                                if (isNaN(length))
                                    alert("[" + i + "] length=" + length + " segment=" + distanceBetween);
                            }
                            bounds.extend(path[i]);
                            //point = path[parseInt(i / 2)];
                        } catch (e) {
                            console.log(e.message.toString());
                        }
                        i++;
                    } //else path.push(path[path.length - 1]);
                });
                $("tr[name='trackControls'], table[name='trackControls']").show();
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

                var tid = setInterval(function() { //because histroyTrackReplay window is closing after the polyline is made
                    if (!$("#trackReplay").dialog("isOpen")) {
                        clearInterval(tid);

                        var index = $(":checkbox[unitid=" + unitId + "]").attr("index-overlay");
                        overlaysArray.polyline[index] = polyline;
                        registerAnimationControlButtons(index, todayOnly);

                        map.fitBounds(bounds);

                        polylineData = data;

                        g.event.addListener(polyline, 'mouseover', function(h) {
                            var latlng = h.latLng;
                            var needle = {
                                minDistance: 9999999999, //silly high
                                index: -1,
                                latlng: null
                            };
                            polyline.getPath().forEach(function(routePoint, index) {
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
                            geocoder.geocode({ 'latLng': h.latLng }, function (results, status) {
                                if (status == g.GeocoderStatus.OK)
                                    if (results[0]) $("#googleMapPopLocIdTrack").html(results[0].formatted_address);
                                    else $("#googleMapPopLocIdTrack").html("Geocoder failed due to: " + status);
                            });
                        });
                        g.event.addListener(polyline, 'mouseout', function() {
                            infoWindowDynamic.close();
                        });
                    }
                }, 500);
            }
        }
    });
}
// not used but need to use after calculating offset on icon on polyline from one to 2nd point
// animateShape for runover polyline
function mercatorInterpolate(map, latLngFrom, latLngTo, fraction) {
    // Get projected points
    var projection = map.getProjection();
    var pointFrom = projection.fromLatLngToPoint(latLngFrom);
    var pointTo = projection.fromLatLngToPoint(latLngTo);
    // Adjust for lines that cross the 180 meridian
    if (Math.abs(pointTo.x - pointFrom.x) > 128) {
        if (pointTo.x > pointFrom.x)
            pointTo.x -= 256;
        else
            pointTo.x += 256;
    }
    // Calculate point between
    var x = pointFrom.x + (pointTo.x - pointFrom.x) * fraction;
    var y = pointFrom.y + (pointTo.y - pointFrom.y) * fraction;
    var pointBetween = new google.maps.Point(x, y);
    // Project back to lat/lng
    var latLngBetween = projection.fromPointToLatLng(pointBetween);
    return latLngBetween;
}

function initiateMapByDate() {
    //Just initiating today's map dialog box and MapByDate button event
    $("#histroyTrackReplay").dialog({
        show: "blind",
        hide: "fold",//"explode",
        title: "Animation Control",
        position: { my: "left+420 top+50", at: "left top" }, //[300, 100],
        autoOpen: false,
        height: 210, width: 400
    });

    $("#btnShowMapbyDate").click(function () {
        drawTodaysPath($("#lblUnitIdMap").val(), $("#lblAssetNameMap").val(), $("#trackIgnition").val(), $("#txt_MapDate").val(), $("#txt_MapDate").val(), $("#txtMapFromTime"), $("#txtMapToTime"));
    });

    //Datepicker will hide in 2 secs need to do that because on window open it covers all the space even don't need to select date.
    $("#txt_MapDate").datepicker({
        dateFormat: "mm/dd/yy",
        showOn: "button",
        buttonImage: '/images/common/calendar.gif',
        buttonImageOnly: true,
        buttonText: "Select date"
    });
    $("#txt_MapDate").datepicker({
        beforeShow: datepicker_beforeShow,
        onClose: function() {
            $(window).unbind('.datepicker_beforeShow');
        }
    });
    $("#txtMapFromTime").timepicker({
        timeFormat: "HH:mm",
        showOn: "button",
        buttonImage: '/images/common/clock.png',
        buttonImageOnly: true,
        buttonText: "Select from time"
    });
    $("#txtMapToTime").timepicker({
        timeFormat: "HH:mm",
        showOn: "button",
        buttonImage: '/images/common/clock.png',
        buttonImageOnly: true,
        buttonText: "Select to time"
    });

    //Just initiating period's map dialog box and MapByPeriod button event
    $("#periodTrackReplay").dialog({
        show: "blind",
        hide: "fold",//"explode",
        title: "Animation Control",
        position: { my: "left+420 top+50", at: "left top" }, //[300, 100],
        autoOpen: false,
        height: 210, width: 430
    });

    $("#btnShowMapbyPeriod").click(function () {
        drawTodaysPath($("#lblUnitIdPeriodMap").val(), $("#lblAssetNamePeriodMap").val(), $("#trackPeriodIgnition").val(), $("#txt_FromDate").val(), $("#txt_ToDate").val(), $("#txtPeriodFromTime"), $("#txtPeriodToTime"), false);
    });

    //Datepicker will hide in 2 secs need to do that because on window open it covers all the space even don't need to select date.
    $("#txt_FromDate").datepicker({
        dateFormat: "mm/dd/yy",
        showOn: "button",
        buttonImage: '/images/common/calendar.gif',
        buttonImageOnly: true,
        buttonText: "Select date",
        beforeShow: datepicker_beforeShow,
        onClose: function () {
            $(window).unbind('.datepicker_beforeShow');
        }
    });
    $("#txt_ToDate").datepicker({
        dateFormat: "mm/dd/yy",
        showOn: "button",
        buttonImage: '/images/common/calendar.gif',
        buttonImageOnly: true,
        buttonText: "Select date",
        beforeShow: datepicker_beforeShow,
        onClose: function () {
            $(window).unbind('.datepicker_beforeShow');
        }
    });
    $("#txtPeriodFromTime").timepicker({
        timeFormat: "HH:mm",
        showOn: "button",
        buttonImage: '/images/common/clock.png',
        buttonImageOnly: true,
        buttonText: "Select from time"
    });
    $("#txtPeriodToTime").timepicker({
        timeFormat: "HH:mm",
        showOn: "button",
        buttonImage: '/images/common/clock.png',
        buttonImageOnly: true,
        buttonText: "Select to time"
    });
    $("#periodTrackReplay #trackSlider .ui-slider-handle").width("0.6em");
}

// set 1sec timeout to hide datepicker on mouseout occuring at least 300ms after mousein,
// and cancel timeout when mousein occurs again before datepicker is hidden
function datepicker_beforeShow(input, inst) {
    $(window).unbind('.datepicker_beforeShow').bind('mousemove.datepicker_beforeShow', function (event) {
        if ($(document.elementFromPoint(event.pageX, event.pageY)).closest('#ui-datepicker-div').size()) {
            if (!inst._mouseover) {
                try {
                    if (inst._hideTimeout)
                        clearTimeout(inst._hideTimeout);
                } catch (e) { };
                inst._hideTimeout = null;
                inst._mouseover = true;
                inst._mouseoverTime = new Date;
            }

        } else {
            if (inst._mouseover && (event.pageX || event.pageY)) {
                var now = new Date;
                if (now.getTime() - inst._mouseoverTime.getTime() > 300) {
                    inst._hideTimeout = setTimeout(function () {
                        $('#' + inst.id).datepicker('hide');
                        $('#search').focus();
                        $(window).unbind('.datepicker_beforeShow');
                        inst._hideTimeout = null;
                    }, 1000);
                    inst._mouseover = false;
                }
            }
        }
    });
}