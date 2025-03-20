function addMarkerForResource(latlng, toolTipHtml, isOpen, isDraggable, 
    arrayIndex, optimized, markerImage) {
    // create a marker
    var marker = new google.maps.Marker({
        position: latlng,
        map: map,
        title: 'Latitude: ' + latlng.lat() + '  Longitude :' + latlng.lng(),
        draggable: isDraggable
        //, optimized: false
    });

    if (optimized != undefined) marker.setOptimized = optimized;
    if (markerImage != undefined) marker.setIcon(markerImage);
    var infoWindow;
    if (toolTipHtml != undefined) { // Dont want to use infowindow
        infoWindow = new g.InfoWindow({
            content: toolTipHtml
            
        });
        
        if (isOpen) infoWindow.open(map, marker);
        //$('#speedometer').speedometer({ percentage: 90 });
    }
    if (arrayIndex != -1) { // Dont want to use
        if (arrayIndex == undefined) {
            registerOverlay(null, null, null, null, marker, infoWindow, null, google.maps.drawing.OverlayType.MARKER);
            if (infoWindow) {
                google.maps.event.addListener(marker, 'mouseover', function() {
                    var index = overlaysArray.marker.indexOf(marker);
                    infoWindow = overlaysArray.infoWindow[index];
                    infoWindow.open(map, marker);
                });

                google.maps.event.addListener(marker, 'mouseout', function() {
                    var index = overlaysArray.marker.indexOf(marker);
                    overlaysArray.infoWindow[index].close();
                });
            }
        } else
            registerOverlay(null, null, null, null, marker, infoWindow, null, google.maps.drawing.OverlayType.MARKER, null, arrayIndex);
    }
    return marker;
}

function toggleAsset(assetCheckBox, afterSuccessCallBack) {
    
    if (assetCheckBox.checked) {
        if (identifyDisabled) {
            var event = { data: { ModuleName: "Identify" } };
            permissionDeniedMsg(event);
            $(assetCheckBox).prop("checked", false);
        } else
            createOrMoveMarker(assetCheckBox, true, afterSuccessCallBack);
    }else {
        unRegisterOverlay($(assetCheckBox).attr("index-overlay"));
        //if today's map dialog box is open then close it. 
        if ($("#histroyTrackReplay").dialog("isOpen")) $("#histroyTrackReplay").dialog("close");
        if ($("#periodTrackReplay").dialog("isOpen")) $("#periodTrackReplay").dialog("close");
        if ($("#trackReplay").dialog("isOpen")) $("#trackReplay").dialog("close");
        if ($("#assetFinder").dialog("isOpen")) $("#assetFinder").dialog("close");
        
        //clear interval and remove from array
        clearIntervalForMarker(assetCheckBox);
        $("#imgIgnition" + assetCheckBox.name).prop('src',  imgIgintion(0));
        $("#imgSignal" + assetCheckBox.name).prop('src', imgGSMSignals(0));
    }
}

var intervals = [];

function createOrMoveMarker(assetObject, create, afterSuccessCallBack) {
    $.ajax({
        url: urlLastCdr + "?assetNo=" + assetObject.name,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            if (create) {
                $(assetObject).parent().parent().parent().parent().parent().parent().showLoaderPanel();
                $(assetObject).attr("disabled", true);
                clearContextMenu(); // getposition error was solved. when check and quickly todays map
            }
        },
        complete: function () {
            if (create) {
                $(assetObject).parent().parent().parent().parent().parent().parent().hideLoaderPanel();
                $(assetObject).removeAttr("disabled");
            }
        },
        success: function (data, textStatus, xhr) {
            if (data.length === 0) {
                alert("There is no data found for asset location");
                $(assetObject).prop("checked", false);
                callRightClickOnObject(); // getposition error was solved. when check and quickly todays map
                return;
            }
            //convert mph to kmph
            data[0].Speed = Math.round((data[0].Speed * 1.609344) * 100) / 100;
            
            $("#imgIgnition" + assetObject.name).prop('src',  imgIgintion(data[0].DI1));

            $("#imgSignal" + assetObject.name).prop('src', imgGSMSignals(data[0].GSMSignals));
            if (textStatus != "success")
                callRightClickOnObject(); // getposition error was solved. when check and quickly todays map
            else {
                timerCountdown();
                var index = $(assetObject).attr("index-overlay");
                $("#infoWindowTemp").html(
                            "<div id='iw-container'>" +
                            "<div class='iw-title'><span id='googleMapPopLocId'>Loading.....</span></div>" +
                            "<div class='iw-content'>" +
                            "<table border=0 class='msgpopuptable'><tbody>" +
                            "<tr><td><b>Date:</b></td><td>" + data[0].RTCDTTM.toString() + "</td></tr>" +
                            //"<td valign='top' rowspan='8'><div></div></td></tr>" +
                            "<tr><td><b>Vehicle #:</b></td><td>" + data[0].Plateid + "</td></tr>" +
                            "<tr><td><b>Device #:</b></td><td>" + data[0].UnitId + "</td></tr>" +
                            "<tr><td><b>Ignition:</b></td><td>" + data[0].Ignition.replace("Iginition ", "") + "</td></tr>" +
                            "<tr><td><b>Speed(Kmph):</b></td><td>" + data[0].Speed + "</td></tr>" +
                            "<tr><td><b>Current Fuel:</b></td><td>" + data[0].Qty + "</td></tr>" +
                            "<tr><td><b>Latitude:</b></td><td>" + data[0].Latitude + "</td></tr>" +
                            "<tr><td><b>Longitude:</b></td><td>" + data[0].Longitude + "</td></tr></tbody></table>" +
                            "</div><div class='iw-bottom-gradient'></div></div>");
                var latLng = new google.maps.LatLng(data[0].Latitude.toString(), data[0].Longitude.toString());
                var geocoder = new google.maps.Geocoder();

                geocoder.geocode({ 'latLng': latLng }, function(results, status) {
                    if (status == google.maps.GeocoderStatus.OK)
                        if (results[0]) $("#infoWindowTemp #googleMapPopLocId").html(results[0].formatted_address);
                        else $("#infoWindowTemp #googleMapPopLocId").html("Geocoder failed due to: " + status);

                    if (create) {
                        var assetPolyline = new g.Polyline({
                            map: map,
                            path: [latLng, latLng],
                            strokeColor: "#71cf90",
                            strokeWeight: 30,
                            geodesic: true,
                            zIndex: 9999
                        });

                        var infoWindow1 = new g.InfoWindow({ content: $("#infoWindowTemp").html() });
                        infoWindow1.setPosition(latLng);
                        infoWindow1.open(map);

                        g.event.addListener(infoWindow1, "domready", function () {
                            var iw_outer = $(".gm-style-iw");
                            var iw_corners_tail = iw_outer.prev();
                            var iw_btnClose = iw_outer.next();
                            iw_outer.css({ left: "0", top: "0", maxWidth: "300px" });
                            iw_corners_tail.children(":nth-child(2)").css({ "border-bottom-left-radius": "10px", "border-bottom-right-radius": "10px", "background-color": "rgba(72, 181, 233, 0.4)" });
                            iw_corners_tail.children(":nth-child(4)").css({ "border-bottom-left-radius": "10px", "border-bottom-right-radius": "10px" });
                            iw_corners_tail.children(":nth-child(3)").find("div").children().css({ "box-shadow": "rgba(72, 181, 233, 0.6) 0px 1px 6px" });
                            iw_btnClose.css({ opacity: "1", right: "-12px", top: "-12px", border: "7px solid #48b5e9", "border-radius": "13px", "box-shadow": "0 0 5px #3990B9" });
                            if ($(".iw-content").height() <= 200) $(".iw-bottom-gradient").css({ display: "none" });
                            iw_btnClose.mouseout(function () {
                                $(this).css({ opacity: "1" });
                            });
                        });

                        if (index == undefined)
                            registerOverlay(null, null, null, null, null, infoWindow1, null, google.maps.drawing.OverlayType.POLYLINE, assetPolyline);
                        else
                            registerOverlay(null, null, null, null, null, infoWindow1, null, google.maps.drawing.OverlayType.POLYLINE, assetPolyline, index);

                        if (index == undefined) {
                            index = overlaysArray.markerPolyline.length - 1;
                            $(assetObject).attr("index-overlay", index);
                        }
                        
                        changeColorAndAngleOfSvgVehicle(index, assetPolyline, getStateOfAssetImage(data[0].Speed, data[0].DI1), data[0].Angle);

                        g.event.addListener(assetPolyline, 'mouseover', function() {
                            overlaysArray.infoWindow[overlaysArray.markerPolyline.indexOf(assetPolyline)].open(map); //, assetPolyline);
                        });

                        g.event.addListener(assetPolyline, 'mouseout', function() {
                            overlaysArray.infoWindow[overlaysArray.markerPolyline.indexOf(assetPolyline)].close();
                        });
                        map.setCenter(latLng);
                        // set interval and push it in array to management it.
                        setIntervalForMarker(assetObject, $("#txtTime").val());
                        callRightClickOnObject(); // getposition error was solved. when check and quickly todays map
                    } else {
                        setAssetPosition(index, latLng, getStateOfAssetImage(data[0].Speed, data[0].DI1), data[0].Angle, false);
                        callRightClickOnObject(); // getposition error was solved. when check and quickly todays map
                    }
                    if (afterSuccessCallBack) afterSuccessCallBack();
                });
            }
        }
    });
}

function setAssetPosition(index, latLng, color, angle, mapCenter) {
    overlaysArray.markerPolyline[index].setPath([latLng, latLng]);
    changeColorAndAngleOfSvgVehicle(index, overlaysArray.markerPolyline[index], color, angle);
    overlaysArray.infoWindow[index].setOptions({
        position: latLng,
        content: $("#infoWindowTemp").html()
    });
    if (mapCenter) map.setCenter(latLng);
}

function changeColorAndAngleOfSvgVehicle(index, markerPolyline, color, angle) {
    var svgVehicle = getVehicleTypeSVGOject(index);
    var icons = svgVehicle.getIconsWithChangeColor(markerPolyline.icons, color, false);
    svgVehicle.changeAngle(icons, parseFloat(angle));
    markerPolyline.set("icons", icons);
}

function imgIgintion(data) {
    return $("#imagePath").prop("class") + "vehicleIcons/acc_" + (data == 0 ? "off" : "on") + ".png";
}

function imgGSMSignals(data) {
    var rpath = $("#imagePath").prop("class") + "vehicleIcons/gsm";
    if (data == 0) return rpath + "Off.png";
    else if (data > 0 && data < 11) return rpath + "Low.png";
    else if (data > 10 && data < 21) return rpath + "Normal.png";
    else if (data > 20 && data < 32) return rpath + "High.png";
    else return rpath + "Off.png";
}

function autoRefreshIntervalRegisterEvents() {
    $("#chkAutoRefresh").click(function() {
        setAutoRefreshTime($("#txtTime").val());
    });

    $("#txtTime").blur(function() {
        if ($(this).val() == "") $(this).val("60");
        setAutoRefreshTime($(this).val());
    });
}

function setAutoRefreshTime(timeVal) {
    if ($('#chkAutoRefresh').is(':checked')) {
        $('#assetTree :checkbox:checked').each(function() {
            setIntervalForMarker(this, timeVal);
             timerCountdown();
        });
        $('#assetTree :checkbox:not(:checked)').each(function() {
            clearIntervalForMarker(this);
            $("#defaultCountdown").removeAttr("class");
        });
        return;
    } else {
        $('#assetTree :checkbox:checked').each(function() {
            clearIntervalForMarker(this);
        });
    }
}

function setIntervalForMarker(obj, timeVal) {
    
    if ($('#chkAutoRefresh').is(':checked')) {
        var interval = setInterval(function() { createOrMoveMarker(obj, false);}, timeVal * 1000);
        if ($(obj).attr("index-interval") == undefined) {
            intervals.push(interval);
            $(obj).attr("index-interval", intervals.length - 1);
        } else
            intervals[$(obj).attr("index-interval")] = interval;
    }
}

function clearIntervalForMarker(obj) {
    clearInterval(intervals[$(obj).attr("index-interval")]);
    intervals[$(obj).attr("index-interval")] = null;
}

function clearContextMenu() {
    $('#assetTree li ul li ul li ul li').contextMenu('contextMenu', {});
}

function callRightClickOnObject() {
    if ($('#assetTree li ul li ul li ul li').length == 0) return;
    $('#assetTree li ul li ul li ul li').contextMenu('contextMenu', {
            'Identify': {
                click: function(element) {
                    selectAssetAndShowInfo(element);
                },
                klass: "custom-class1",
                image: "Common/magnifier.png",
                hide: identifyDisabled
            },
            "Ample View": {
                click: function(element) { // element is the jquery obj clicked on when context menu launched
                    showAmpleView(element);
                },
                klass: "custom-class1", // a custom css class for this menu item (usable for styling)
                image: "Common/icon8.png",
                hide: ampleViewDisabled
            },
            "Today's Drive": {
                click: function(element) {
                    selectAssetAndShowInfo(element,
                        function() {
                            $("#trackIgnition").val("1");
                            $("#txt_MapDate").val(getNowDate());
                            $("#txtMapFromTime").val("00:00");
                            $("#txtMapToTime").val("23:59");
                            drawTodaysPath($('li#' + element[0].id).find("[type=checkbox]").attr("unitid"), $('li#' + element[0].id).find("[type=checkbox]").parent().parent().text(), $("#trackIgnition").val(), $("#txt_MapDate").val(), $("#txt_MapDate").val(), $("#txtMapFromTime"), $("#txtMapToTime")); // Ignition on
                        });
                },
                klass: "custom-class1",
                image: "Common/map.png",
                hide: todayMapDisabled
            },
            "Period Drive": {
                click: function (element) {
                    selectAssetAndShowInfo(element,
                        function () {
                            $("#trackPeriodIgnition").val("1");
                            $("#txt_FromDate").val(getNowDate(-1));
                            $("#txt_ToDate").val(getNowDate());
                            $("#txtPeriodFromTime").val("00:00");
                            $("#txtPeriodToTime").val("23:59");
                            var unitId = $('li#' + element[0].id).find("[type=checkbox]").attr("unitid");
                            var assetName = $('li#' + element[0].id).find("[type=checkbox]").parent().parent().text();
                            $("#lblUnitIdPeriodMap").val(unitId);
                            $("#lblAssetNamePeriodMap").val(assetName);
                            $("#periodTrackReplay").dialog({ title: "Period's Drive for " + assetName });
                            $("#periodTrackReplay").dialog("open");
                            
                            //drawTodaysPath($('li#' + element[0].id).find("[type=checkbox]").attr("unitid"), $('li#' + element[0].id).find("[type=checkbox]").parent().parent().text(), $("#trackPeriodIgnition").val(), $("#txt_FromDate").val(), $("#txt_ToDate").val(), $("#txtPeriodFromTime"), $("#txtPeriodToTime")); // Ignition on
                        });
                },
                klass: "custom-class1",
                image: "Common/map.png",
                hide: todayMapDisabled
            },
            "Today's Activity": {
                click: function (element) {
                    selectAssetAndShowInfo(element,
                        function () {
                            $("#txtTrackMapDate").val(getNowDate());
                            $("#txtTrackMapFromTime").val("00:00");
                            $("#txtTrackMapToTime").val("23:59");
                            drawTodaysTrack($('li#' + element[0].id).find("[type=checkbox]").attr("unitid"), $('li#' + element[0].id).find("[type=checkbox]").parent().parent().text(), $("#txtTrackMapDate").val(), $("#txtTrackMapFromTime"), $("#txtTrackMapToTime"));
                        });
                },
                klass: "custom-class1",
                image: "Common/map.png",
                hide: todayMapDisabled
            },
            "Today's Report": {
                click: function(element) {
                    loadGrid(element[0].id, $('li#' + element[0].id).find("[type=checkbox]").parent().text());
                },
                klass: "custom-class1",
                image: "Common/report.png",
                hide: todayReportDisabled
            },
            "Asset finder": {
                click: function (element) {
                    selectAssetAndShowInfo(element,
                        function() {
                            loadAssetFinder($('li#' + element[0].id).find("[type=checkbox]"));
                        });
                },
                klass: "custom-class1",
                image: "Common/find.png",
                hide: assetFinderDisabled
            }
        },
        {
            //delegateEventTo: 'childrenSelector',
            //showMenu: function() { $(this).hide(); },//alert("Showing menu"); },
            //hideMenu: function () { console.log(this); },
            //leftClick: true , // trigger on left click instead of right click
            disable_native_context_menu: true
        }
    );
}

function getNowDate(addDays = 0) {
    //Thu May 19 2011 17:25:38 GMT+1000 {}
    var fullDate = new Date();
    //convert month to 2 digits
    var twoDigitMonth = ((fullDate.getMonth().length + 1) === 1) ? (fullDate.getMonth() + 1) : '0' + (fullDate.getMonth() + 1);
    var currentDate = twoDigitMonth + "/" + (fullDate.getDate() + addDays) + "/" + fullDate.getFullYear() ;
    return currentDate;
}

function selectAssetAndShowInfo(element, afterSuccessCallBack) {
    var checkbox = $('li#' + element[0].id).find("[type=checkbox]");
    if (checkbox.attr("id") == 'chkAsset' + element[0].id) {
        if (!checkbox.prop("checked")) {
            checkbox.prop({checked : true});
            toggleAsset(checkbox[0], afterSuccessCallBack);
        } else {
            if (afterSuccessCallBack) afterSuccessCallBack();
            map.setCenter(overlaysArray.markerPolyline[checkbox.attr("index-overlay")].getPath().getAt(0));
        }
    }
}

function showAllAsset() {
    $('#assetTree :checkbox').each(function() {
         $("#" + this.id).prop({ checked: true });
        toggleAsset(this);
        });
}

function getVehicleTypeSVGOject(index) {
    if ($("[index-overlay=" + index + "]").closest("li[class*=collapsable]").parent().parent().prop("id").split(".")[1] == "3")
    //if ($("[index-overlay=" + index + "]").closest("li[class*=lastCollapsable]").prop("id").split(".")[1] == "3")
        return new registerSVGTruck();
    else
        return new registerSVGVehicle();
}