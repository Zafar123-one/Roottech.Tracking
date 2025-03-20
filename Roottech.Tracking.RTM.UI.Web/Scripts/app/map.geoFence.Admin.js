function drawPoiByRoute(route, isFromGrid){
    var arrlatlng = [];
    $.ajax({
        url: urlGetPoisByRoute + "?route=" + route,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        beforeSend: function () { beforeSendForGeoFence(isFromGrid ? "listRouteAdmin" : "geofenceAdmin"); },
        complete: function () { completeForGeoFence(isFromGrid ? "listRouteAdmin" : "geofenceAdmin"); },
        success: function (data, textStatus, xhr) {
            if (textStatus == "success") {
                if (!data.length) {
                    alert("There is no route available for selected organization and company. Try to select other options.");
                    clearGeoFenceAdmin(false);
                    return;
                }
                //save the length of the overlays to remove in the end
                if ($("#geofenceObjects").val() != "")
                    $("#geofenceObjects").val($("#geofenceObjects").val().split("-")[0]);
                else
                    $("#geofenceObjects").val(overlaysArray.marker.length);
                var markerIndex = overlaysArray.marker.length;
                $(data).each(function () {
                    drawPoiOnMap(this.Poi.Lati, this.Poi.Longi, this.Poi.Id, this.Poi.PoiType.GImage.ImgPath);
                    arrlatlng.push(new g.LatLng(this.Poi.Lati, this.Poi.Longi));
                });
                drawPolylineForRoute(arrlatlng);
                //save the length of the overlays to remove in the end
                if (overlaysArray.marker.length > 0 && overlaysArray.marker.length != parseInt($("#geofenceObjects").val()))
                    $("#geofenceObjects").val(parseInt($("#geofenceObjects").val()) + "-" + overlaysArray.marker.length);

                if (isFromGrid) {
                    $grid.jqGrid("setCell", route, "MarkerIndex", markerIndex.toString() + "-" + (overlaysArray.polyline.length).toString());
                    $("#gbox_listRouteAdmin .ui-jqgrid-titlebar-close.HeaderButton .ui-icon.ui-icon-circle-triangle-n").parent().click();
                }
            }
        }
    });
}

function drawPolylineForRoute(arrLatLng) {
    var options = {
        fillColor: "#e2ffd1",
        fillOpacity: 0.5,
        strokeWeight: 2,
        strokeColor: "#9d9d9d",
        strokeOpacity: 1.0,
        clickable: true,
        zIndex: 1,
        editable: true,
        geodesic: true,
        strokePosition: g.StrokePosition.INSIDE
    };
    var polylineOptions = options;

    var lineSymbol = {
        path: g.SymbolPath.FORWARD_CLOSED_ARROW, //FORWARD_OPEN_ARROW
        strokeColor: "#000000",
        strokeOpacity: 1
    };
    var arrowArray = [];
    for (var i = 0; i < 10; i++) {
        arrowArray.push({
            icon: lineSymbol,
            offset: ((i + 1) * 10) + "%",
            repeat: ((i + 1) * 10) + "%",
            fixedRotation: false
        });
    }
    polylineOptions.icons = arrowArray;
    var polyLine = new g.Polyline(polylineOptions);
    polyLine.setMap(map);
    polyLine.setPath(arrLatLng);

    registerOverlay(null, null, polyLine, null, null, null, null, g.drawing.OverlayType.POLYLINE);

    var bounds = new g.LatLngBounds();
    $(arrLatLng).each(function () {
        bounds.extend(this);
    });
    map.fitBounds(bounds);
}

function drawGeoFenceByType(orgCode, companyCode, type, geofenceNo) {
    var urlGeofence = urlGetGeofenceDtl + "?orgCode=" + orgCode + "&companyCode=" + companyCode + "&type=" + type;
    if (geofenceNo != 0)
        urlGeofence = urlGetGeofenceDtl + "?orgCode=" + orgCode + "&companyCode=" + companyCode + "&geofenceNo=" + geofenceNo;

    $.ajax({
        url: urlGeofence,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        beforeSend: function () { beforeSendForGeoFence(geofenceNo != 0 ? "listGeoFenceAdmin" : "geofenceAdmin"); },
        complete: function () { completeForGeoFence(geofenceNo != 0 ? "listGeoFenceAdmin" : "geofenceAdmin"); },
        success: function (data, textStatus, xhr) {
            if (textStatus == "success" && data.length > 0 ) {
                var latLng, infoWindow = new g.InfoWindow(), i, options = {
                    strokeColor: "#FF0000",
                    strokeOpacity: 0.8,
                    strokeWeight: 2,
                    fillColor: "#FF0000",
                    fillOpacity: 0.35,
                    map: map,
                    zIndex: 100
                }, bounds = new g.LatLngBounds(), path = [];
                //save the length of the overlays to remove in the end
                if ($("#geofenceObjects").val() != "")
                    $("#geofenceObjects").val($("#geofenceObjects").val().split("-")[0]);
                else
                    $("#geofenceObjects").val(overlaysArray.marker.length);
                
                $(data).each(function (index) {
                    if (this.GFType == "C") {
                        latLng = new g.LatLng(this.CLati, this.CLongi);
                        options.center = latLng;
                        options.radius = this.CRadius;
                        infoWindow = "<table border=0 ><tbody><tr><td><b>Circle Title:</b> " + this.GFTitle + "</td></tr>" +
                            "<tr><td><b>Name:</b> " + this.GFName + "</td></tr>" +
                             "<tr><td><b>Comment:</b> " + this.Comment + "</td></tr>" +
                            "<tr><td><b>Buffer:</b> " + this.GFMargin + "</td></tr>" +
                            "<tr><td><b>Latitude:</b> " + this.CLati + "</td></tr>" +
                            "<tr><td><b>Longitude:</b> " + this.CLongi + "</td></tr>" +
                            "<tr><td><b>Radius:</b> " + this.CRadius + "</td></tr></tbody></table>";
                        i = overlaysArray.marker.indexOf(addMarkerForResource(latLng, infoWindow, false, false, undefined, undefined)); // Add the circle for this city to the map.

                        // Add the circle for this city to the map.
                        var circle = new g.Circle(options);
                        overlaysArray.baseType[i] = g.drawing.OverlayType.CIRCLE;
                        overlaysArray.circle[i] = circle;

                        g.event.addListener(circle, "rightclick", function (mouseEvent) {
                            unRegisterOverlay(overlaysArray.circle.indexOf(circle));
                        });
                        bounds.extend(latLng);

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
                        infoWindow = "<table border=0 ><tbody><tr><td><b>Rectangle Title:</b> " + this.GFTitle + "</td></tr>" +
                            "<tr><td><b>Name:</b> " + this.GFName + "</td></tr>" +
                            "<tr><td><b>Comment:</b> " + this.Comment + "</td></tr>" +
                            "<tr><td><b>Buffer:</b> " + this.GFMargin + "</td></tr>" +
                            "<tr><td><b>Left Top Latitude:</b> " + this.SLeftTopLati + "</td></tr>" +
                            "<tr><td><b>Left Top Longitude:</b> " + this.SLeftTopLongi + "</td></tr>" +
                            "<tr><td><b>Right Bottom Latitude:</b> " + this.SRightBotLati + "</td></tr>" +
                            "<tr><td><b>Right Bottom Longitude:</b> " + this.SRightBotLongi + "</td></tr></tbody></table>";
                        i = overlaysArray.marker.indexOf(addMarkerForResource(new g.LatLng(this.SLeftTopLati, this.SLeftTopLongi), infoWindow, false, false, undefined, undefined)); 
                        
                        // Add the rectangle for this city to the map.
                        var rectangle = new g.Rectangle(options);
                        overlaysArray.baseType[i] = g.drawing.OverlayType.RECTANGLE;
                        overlaysArray.rectangle[i] = rectangle;

                        g.event.addListener(rectangle, "rightclick", function (mouseEvent) {
                            unRegisterOverlay(overlaysArray.rectangle.indexOf(rectangle));
                        });
                        bounds.extend(new g.LatLng(this.SLeftTopLati, this.SLeftTopLongi));
                        bounds.extend(new g.LatLng(this.SRightBotLati, this.SRightBotLongi));

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
                        /*if (i > 0) {
                            var distanceBetween = g.geometry.spherical.computeDistanceBetween(path[i - 1], path[i]);
                            var length = distanceBetween;
                            if (isNaN(length))
                                alert("[" + i + "] length=" + length + " segment=" + distanceBetween);
                        }*/
                        bounds.extend(path[i]);
                        if ((index == data.length - 1 || this.GeofenceNo != data[index + 1].GeofenceNo)) {
                            infoWindow = "<table border=0 ><tbody><tr><td><b>" + (this.GFType == "P" ? "Polygon" : "Polyline") + " Title:</b> " + this.GFTitle + "</td></tr>" +
                            "<tr><td><b>Name:</b> " + this.GFName + "</td></tr>" +
                            "<tr><td><b>Comment:</b> " + this.Comment + "</td></tr>" +
                            "<tr><td><b>Buffer:</b> " + this.GFMargin + "</td></tr>" +
                            "<tr><td><b>Latitude:</b> " + this.PLati + "</td></tr>" +
                            "<tr><td><b>Longitude:</b> " + this.PLongi + "</td></tr>" +
                            "<tr><td><b>Last Sequence:</b> " + this.PSeq + "</td></tr></tbody></table>";
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
                                icons: [
                                    {
                                        icon: arrowSymbol,
                                        repeat: "1%",
                                        fixedRotation: false
                                    }
                                ],
                                zIndex: 3
                            });
                            i = overlaysArray.marker.indexOf(addMarkerForResource(path[0], infoWindow, false, false, undefined, undefined));

                            overlaysArray.polyline[i] = polyline;

                            // Draw buffer polygon
                            if (this.GFMargin > 0) {
                                var refVar = { routePolygon: null };
                                drawRoute(i, this.GFMargin, refVar);
                                overlaysArray.polygon[i] = refVar.routePolygon;
                            }
                            // Initiating info window for animated asset will display on pause
                            var infoWindowDynamic = new g.InfoWindow();
                            g.event.addListener(polyline, "mouseover", function(h) {
                                var needle = {
                                    minDistance: 9999999999, //silly high
                                    index: -1,
                                    latlng: null
                                };
                                polyline.getPath().forEach(function(routePoint, index) {
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
                                    function(results, status) {
                                        if (status == g.GeocoderStatus.OK)
                                            if (results[0]) $("#googleMapPopLocId").html(results[0].formatted_address);
                                    });
                            });
                            g.event.addListener(polyline, "mouseout", function() {
                                infoWindowDynamic.close();
                            });
                        }
                    }
                });
                if (overlaysArray.marker.length > 0 && overlaysArray.marker.length != parseInt($("#geofenceObjects").val()))
                    $("#geofenceObjects").val(parseInt($("#geofenceObjects").val()) + "-" + overlaysArray.marker.length);

                if (!bounds.isEmpty())
                    map.fitBounds(bounds);

                if (geofenceNo != 0) {
                    $grid.jqGrid("setCell", geofenceNo, "MarkerIndex", i);
                    $("#gbox_listGeoFenceAdmin .ui-jqgrid-titlebar-close.HeaderButton .ui-icon.ui-icon-circle-triangle-n").parent().click();
                }
            } else alert("No data found");
        }
    });
}

function saveGeofence(geoFenceType, linePathArray, cRadius, cLati, cLongi, sLeftToplat, sLeftToplng, sRightBotLat, sRightBotLng, overlayIndex) {
    var originalContent;
    $("#geofenseMst").dialog({
        height: 300,
        width: 450,
        show: "blind",
        hide: "explode",
        title: "Geo Fence",
        modal: true,
        position: { my: "left+520 top+50", at: "left top" }, //[400, 100],
        autoOpen: false,
        close: function () {
            clearGeofenceAdminIns(originalContent, overlayIndex);
        }
    });
    originalContent = $("#geofenseMst").html();

    var orgCode = $("#selOrganizationByUserForGeoFence").val();

    $("#selGeoFenseOrganizationByUser").hide();
    $("#selGeoFenseType").hide();

    $("#selGeoFenseOrganizationByUser").closest("td").css({ height: "26px" });
    $("#selGeoFenseOrganizationByUser").closest("td").html("<p>" + $("#selOrganizationByUserForGeoFence option:selected").text() + "</p>");

    $("#selGeoFenseType").closest("td").css({ height: "26px" });
    $("#selGeoFenseType").closest("td").html("<p>Polyline</p>");
    
    $("#btnGeoFenseSave").click(function () {
        if ($("#selGeoFenseOrganizationByUser").val() == "") {
            alert("Please select organization");
            $("#selGeoFenseOrganizationByUser").focus();
            return false;
        } else if ($("#txtGeoTitle").val() == "") {
            alert("Please select Title");
            $("#txtGeoTitle").focus();
            return false;
        } else if ($("#txtGeoName").val() == "") {
            alert("Please select Name");
            $("#txtGeoName").focus();
            return false;
        }

        var postData = [];
        if (geoFenceType == "L" || geoFenceType == "P")
            $(linePathArray).each(function(index, element) {
                postData.push({ Lati: element.lat(), Longi: element.lng(), Sequence: index + 1 });
            });
        else if (geoFenceType == "C" ) 
            postData.push({ Lati: cLati, Longi: cLongi, Sequence: 1 });
        else if (geoFenceType == "R") {
            postData.push({ Lati: sLeftToplat, Longi: sLeftToplng, Sequence: 1 });
            postData.push({ Lati: sRightBotLat, Longi: sRightBotLng, Sequence: 2 });
        }
        $.ajax({
            method: "POST",
            url: urlAddPolyGeofencedtl + "?companyCode=" + $("#selGeoFenseCompanyByUser").val() + "&gfType=" + geoFenceType
            + "&gfTitle=" + $("#txtGeoTitle").val() + "&gfName=" + $("#txtGeoName").val() + "&gfMargin=" + $("#txtGeoMargin").val()
            + "&comment=" + $("#txtGeoComments").val() + "&orgCode=" + orgCode + "&cRadius=" + cRadius,
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
                        unRegisterOverlay(overlayIndex);
                        $("#measureTool").dialog("close");
                        $("#geofenseMst").dialog("close");
                        $("#listGeoFenceAdmin").trigger("reloadGrid");
                        $("#gbox_listGeoFenceAdmin .ui-jqgrid-titlebar-close.HeaderButton .ui-icon.ui-icon-circle-triangle-s").parent().click();
                    }
                });
            },
            success: function (data, textStatus, xhr) {
                if (data)
                    alert("Geo fence has been saved successfully.");
                else
                    alert("An error occur while saving route geo fence. Contact the administrator");
            }
        });
    });

    $("#btnGeoFenceCancel").click(function () {
        $("#geofenseMst").dialog("close");
        clearGeofenceAdminIns(originalContent, overlayIndex);
    });
    $("#geofenseMst").dialog("open");

    bindCompanyByOrg(orgCode, $("#selCompanyByOrgForGeoFence").val());
}

function clearGeofenceAdminIns(originalContent, overlayIndex) {
    $("#geofenseMst").html(originalContent);
    unRegisterOverlay(overlayIndex);
}

function addGeofenceDtl(gfMst, cRadius, cLati, cLongi, pLati, pLongi, sLeftTopLati, sLeftTopLongi, sRightTopLati, sRightTopLongi, pSeq) {
    $.ajax({
        type: "POST",
        url: urlSetGeofenceDtl + "?gfMst=" + gfMst + "&cRadius=" + cRadius + "&cLati=" + cLati + "&cLongi=" + cLongi + "&pLati=" + pLati + "&pLongi=" + pLongi + "&pSeq=" + pSeq + "&sLeftTopLati=" + sLeftTopLati + "&sLeftTopLongi=" + sLeftTopLongi + "&sRightTopLati=" + sRightTopLati + "&sRightTopLongi=" + sRightTopLongi,
        contentType: "application/json charset=utf-8",
        dataType: "json",
        //data:stringPost,// JSON.stringify(stringPost),
        //data: JSON.stringify(GFMst),
        success: function (data, textStatus, xhr) {
        }
    });
}

function bindOrganizationByUser(isRoute, selectGeoFenceType) {
    if (isRoute) 
        $("#selRouteOrganizationByUser").change(function () {
            bindCompanyByOrg($(this).val(), "");
        });    
    else
        $("#selGeoFenseOrganizationByUser").change(function () {
            bindCompanyByOrg($(this).val(), "");
        });
    
    $.ajax({
        url: urlGetOrganizationsByUserCode,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function(data) {
            var bindData = { org: data };
            var output = "<option value=''>Select Organization</option>";
            var template = output + "{{#org}}<option value={{Id}}>{{Description}}</option>{{/org}}";
            var html = Mustache.to_html(template, bindData);

            if (isRoute) {
                $("#selRouteOrganizationByUser").html(html);
                $("#selRouteOrganizationByUser").show();
            } else {
                $("#selGeoFenseOrganizationByUser").html(html);
                $("#selGeoFenseOrganizationByUser").show();

                addOptionsToSelGeoFenceType("#selGeoFenseType", selectGeoFenceType);
                $("#selGeoFenseType").show();
            }

            output = "<option value=''>Select Company</option>";
            html = Mustache.to_html(output, output);
            if (isRoute) {
                $("#selRouteCompanyByUser").html(html);
                $("#selRouteCompanyByUser").show();
                $("#routeAdmin").dialog("option", "height", $(window).height());
                $("#routeAdmin").dialog("open");
            } else {
                $("#selGeoFenseCompanyByUser").html(html);
                $("#selGeoFenseCompanyByUser").show();
                $("#geofenseMst").dialog("open");
            }
        }
    });
}

function bindCompanyByOrg(orgCode, selectedCompany) {
    $.ajax({
        url: urlGetCompaniesbyOrgCode + "?orgCode=" + orgCode,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        beforeSend: function () { $("#selGeoFenseCompanyByUser").html("<option value=''>Loading....</option>"); },
        success: function (data) {
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

            $("#selRouteCompanyByUser").html(html);
            $("#selRouteCompanyByUser").show();

            if (selectedCompany)
                $("#selCompanyByOrgForGeoFence").val(selectedCompany);
        }
    });
}

function addOptionsToSelGeoFenceType(selectControlId, selectGeoFenceType) {
    var output = "<option value=''>Please select Geofense Type</option>";
    output += "<option value='C'>Circle</option>";
    output += "<option value='P'>Polygon</option>";
    output += "<option value='L'>Polyline</option>";
    output += "<option value='R'>Rectangle</option>";

    if (selectGeoFenceType != "")
        output = output.replace(
                    output.substr(
                        output.indexOf("'" + selectGeoFenceType + "'"), 4), "'" + selectGeoFenceType + "' selected>");

    var html = Mustache.to_html(output, output);
    $(selectControlId).html(html);
}

function clearGeoFenceAdmin(clearGeofenceFromMap) {
    $("#selCompanyByOrgForGeoFence").hide();
    $("#markersRadio").hide();
    $("[name='grp1']:checked").prop("checked", false);
    $("#selRouteByOrgCom").hide();
    $("#selGfType").val("");
    $("#selGfType").hide();
    $("#showGrid").hide();
    $("#drawOnMap").hide();

    if (clearGeofenceFromMap) {
        if ($("#geofenceObjects").
            val().split("-").length > 1)
            for (var i = $("#geofenceObjects").val().split("-")[0]; i < $("#geofenceObjects").val().split("-")[1]; i++) {
                unRegisterOverlay(i);
            }
        $("#geofenceObjects").val("");
    }
}

function loadOrgByUserForGeofence() {
    $("#listGeoFenceAdmin").GridUnload();
    $("#selOrganizationByUserForGeoFence").off("change");
    $("#selOrganizationByUserForGeoFence").change(function () {
        if ($(this).val() != "")
            loadCompaniesByOrgForGeofence($(this).val());
    });

    $.get(urlGetOrganizationsByUserCode, function (Organizations) {
        if (Organizations.length == 0) {
            alert("There is no organization assigned to your user. Please contact adminstrator.");
            return;
        }
        var output = "<option value=''>Select Organization</option>";
        $(Organizations).each(function () {
            output += "<option value='" + this.Id + "'>" + this.Description + "</option>";
        });
        $("#selOrganizationByUserForGeoFence").html(output);
        $("#geofenceAdmin").dialog("open");
        if (Organizations.length == 1) {
            /// One thing left if there are multiple companies in only organization then what
            $("#selOrganizationByUserForGeoFence").val($("#selOrganizationByUserForGeoFence option:eq(1)").val());
            loadCompaniesByOrgForGeofence($("#selOrganizationByUserForGeoFence").val());
        }
        addOptionsToSelGeoFenceType("#selGfType", "");
    });
}

function loadCompaniesByOrgForGeofence(orgCode) {
    $.get(urlGetCompaniesbyOrgCode, { orgCode: orgCode }, function (companies) {
        var output = "<option value=''>Select Company</option>";
        $(companies).each(function () {
            output += "<option value='" + this.Id + "'>" + this.CompanyName + "</option>";
        });
        $("#selCompanyByOrgForGeoFence").html(output);
        $("#selCompanyByOrgForGeoFence").show();

        $("#markersRadio").show();

        $("#drawOnMap").show();
        $("#drawOnMap").off("click");
        $("#drawOnMap").click(function () {
            if ($("#selOrganizationByUserForGeoFence").val() == "") {
                alert("Please select organization");
                $("#selOrganizationByUserForGeoFence").focus();
                return false;
            } 
            else if ($("[name='grp1']:checked").attr("id") == undefined) {
                alert("Please choose one of the radio buttons first to draw either route or geo fence on map.");
                $("#rdshowRouteMarker").focus();
                return false;
            }

            if ($("#rdshowRouteMarker").is(":checked")) {
                if ($("#selRouteByOrgCom").val() == "") {
                    alert("Please select Route");
                    $("#selRouteByOrgCom").focus();
                    return false;
                }
                drawPoiByRoute($("#selRouteByOrgCom").val(),false);
            } else if ($("#rdshowGeofenceMarker").is(":checked")) {
                if ($("#selGfType").val() == "") {
                    alert("Please select Geo Fense Type");
                    $("#selRouteByOrgCom").focus();
                    return false;
                }
                drawGeoFenceByType($("#selOrganizationByUserForGeoFence").val(), $("#selCompanyByOrgForGeoFence").val(), $("#selGfType").val(), 0);
            }
        });

        $("#showGrid").show();
        $("#showGrid").off("click");
        $("#showGrid").click(function () {
            if ($("#selOrganizationByUserForGeoFence").val() == "") {
                alert("Please select organization");
                $("#selOrganizationByUserForGeoFence").focus();
                return false;
            }
            else if ($("[name='grp1']:checked").attr("id") == undefined) {
                alert("Please choose one of the radio buttons first to list either route or geo fence on map.");
                $("#rdshowRouteMarker").focus();
                return false;
            }
            if ($("#rdshowRouteMarker").is(":checked")) 
                loadRouteAdminGrid($("#selOrganizationByUserForGeoFence").val(), $("#selCompanyByOrgForGeoFence").val());
            else if ($("#rdshowGeofenceMarker").is(":checked")) 
                loadGeoFenceAdminGrid($("#selOrganizationByUserForGeoFence").val(), $("#selCompanyByOrgForGeoFence").val());
        });
    });
}

function markerTypeClickEvent() {
    $("#rdshowRouteMarker, #rdshowGeofenceMarker").change(function() {
        if ($("#rdshowRouteMarker").is(":checked")) {
            if (this.checked) {
                if ($("#selOrganizationByUserForGeoFence").val() == "") {
                    alert("Please select organization");
                    $("#selOrganizationByUserForGeoFence").focus();
                    return false;
                } 
                loadRouteByOrgCom($("#selOrganizationByUserForGeoFence").val(), $("#selCompanyByOrgForGeoFence").val());
                $("#selGfType").hide();
            }
        } else if ($("#rdshowGeofenceMarker").is(":checked")) {
            if (this.checked) {
                if ($("#selOrganizationByUserForGeoFence").val() == "") {
                    alert("Please select organization");
                    $("#selOrganizationByUserForGeoFence").focus();
                    return false;
                } 
                $("#selGfType").show();
                $("#selRouteByOrgCom").hide();
            }
        }
    });
}

function loadRouteByOrgCom(orgCode, Company_code) {
    $.get(urlGetRoute, { orgCode: orgCode, Company_code: Company_code }, function (routes) {
        if (routes.length > 0) {
            var output = "<option value=''>Select Route</option>";
            $(routes).each(function () {
                output += "<option value='" + this.Id + "'>" + this.RouteName + "</option>";
            });
            $("#selRouteByOrgCom").html(output);
            $("#selRouteByOrgCom").show();

            $("#showGrid").show();
            $("#showGrid").off("click");
            $("#showGrid").click(function () {
                if ($("#selOrganizationByUserForGeoFence").val() == "") {
                    alert("Please select organization");
                    $("#selOrganizationByUserForGeoFence").focus();
                    return false;
                }
                loadRouteAdminGrid($("#selOrganizationByUserForGeoFence").val(), $("#selCompanyByOrgForGeoFence").val());
            });
            $("#drawOnMap").show();
        } else {
            alert("There is no route in selected Organization and company.");
            //$("[name='grp1']:checked").prop("checked", false);// to list empty if user wants to add route
            $("#drawOnMap").hide();
        }
    });
}

function beforeSendForGeoFence(windowNameToLoadOn) {
    if (windowNameToLoadOn.startsWith("list")) {
        $("#" + windowNameToLoadOn).fadeOut();
        $("#load_" + windowNameToLoadOn).fadeIn();
        return;
    }
    $("#geoFenceLoader").css({
        left: $("#" + windowNameToLoadOn).parent().css("left"),
        top: $("#" + windowNameToLoadOn).parent().css("top"),
        height: "180px"
    });
    $("#geoFenceLoader p").css({ "padding-top": "150px" });

    $("#" + windowNameToLoadOn).dialog({
        closeOnEscape: false,
        beforeClose: function (event, ui) { return false; },
        dialogClass: "noclose",
        draggable: false
    });

    $("#geoFenceLoader").fadeIn({
        complete: function () {
            $("#" + windowNameToLoadOn + " table").fadeOut();
        }
    });
}

function completeForGeoFence(windowNameToLoadOn) {
    if (windowNameToLoadOn.startsWith("list")) {
        $("#load_" + windowNameToLoadOn).fadeOut();
        $("#" + windowNameToLoadOn).fadeIn();
        return;
    }
    $("#geoFenceLoader").fadeOut({
        complete: function () {
            $("#" + windowNameToLoadOn).dialog({
                closeOnEscape: true,
                beforeClose: function (event, ui) { return true; },
                dialogClass: "",
                draggable: true
            });
            $("#" + windowNameToLoadOn + " table").fadeIn();
        }
    });
}

function loadGeoFenceAdminGrid(orgCode, companyCode) {
    var grid = $("#listGeoFenceAdmin"),
        getColumnIndexByName = function (grid, columnName) {
            var cm = $("#listGeoFenceAdmin").jqGrid("getGridParam", "colModel"), i, l = cm.length;
            for (i = 0; i < l; i++) {
                if (cm[i].name === columnName) {
                    return i; // return the index
                }
            }
            return -1;
        },
        delOptions = {
            url: urlDeleteGeofence,
            modal: true,
            resize: false,
            drag: false,
            closeOnEscape: true,
            caption: "Delete Existing Geo Fence",
            msg: "<br />&nbsp;Are you sure to delete the<br />&nbsp;selected Geo Fence?",
            bSubmit: "Yes",
            bCancel: "No",
            onclickSubmit: function (options, formid) {
                options.url += "/" + formid;
                options.mtype = "DELETE";
            },
            beforeSubmit: function (postdata, formid) {
                unRegisterOverlay(parseInt($grid.jqGrid("getCell", postdata, "MarkerIndex")));
                return [true, ""];
            }
        },
        editOptions = {
            //Edit //http://techbrij.com/add-edit-delete-jqgrid-asp-net-web-api
            url: urlEditGeofence,
            closeAfterEdit: true,
            closeOnEscape: true,
            bSubmit: "Update",
            modal: true,
            width: 560,
            onclickSubmit: function (options, postData) {
                if (options.url.indexOf("/" + postData.Id) == -1)
                    options.url += "/" + postData.Id;
                options.mtype = "PUT";
            },
            beforeSubmit: function (postdata, formid) {
                //more validations
                if (postdata.GFMargin < 0)
                    return [false, "Buffer Distance (Meters) should be greater than or equals to 0"]; //error
                return [true, ""]; // no error
            }
        },
        colModel = [
            {
                name: "act",
                index: "act",
                width: 75,
                align: "center",
                sortable: false,
                editable: false,
                formatter: "actions",
                formatoptions: //http://www.trirand.com/jqgridwiki/doku.php?id=wiki:predefined_formatter
                {
                    keys: true,
                    editformbutton: true,
                    editbutton: false,
                    delOptions: delOptions,
                    editOptions: editOptions
                }
            },
            {
                name: "Id",
                index: "Id",
                key: true,
                editable: true,
                hidden: true,
                sorttype: "int",
                editrules: { edithidden: false },
                editoptions: { dataInit: function (element) { $(element).attr("readonly", "readonly"); } }
            },
    //{ name: "select", index: "", width: 40, align: "center", edittype: "checkbox", formatter: "checkbox", editable: false, formatoptions: { disabled: false } }, //editoptions: { value: "True:False" }, 
            {name: "GFName", index: "GFName", width: 300, align: "left", sorttype: "string", editable: true, editoptions: { maxlength: 100 }, editrules: { required: true} },
            { name: "GFTitle", index: "GFTitle", width: 100, align: "left", sorttype: "string", editable: true, hidden: true, editrules: { edithidden: true, required: true }, editoptions: { size: 15, maxlength: 15} },
            {
                name: "GFType",
                index: "GFType",
                width: 70,
                editable: false,
                //hidden: true,
                formatter:"select",
                edittype: "select",
                stype: "select",
                editrules: { edithidden: true },
                editoptions: { value: "C:Circle;P:Polygon;L:Polyline;R:Rectangle", defaultValue: "C" },
                searchoptions: { sopt: ["eq", "ne"], value: ":Any;C:Circle;P:Polygon;L:Polyline;R:Rectangle" }
            },
            {
                name: "GFMargin",
                index: "GFMargin",
                width: 130,
                template: integerTemplate,
                editable: true,
                //hidden: true,
                editrules: { edithidden: true, number: true }
            },
            { name: "Comment", index: "Comment", editable: true, hidden: true, editrules: { edithidden: true }, editoptions: { maxlength: 100} },
            { name: "MarkerIndex", index: "MarkerIndex", width: 1, sortable: false, editable: false, hidden: true, search: false, view: false }
        ];

    loadjqGrid(
        [], "listGeoFenceAdmin", urlGetGeofenceMst, { orgCode: orgCode, companyCode: companyCode },
        [
            "Actions", "Code", "Geo Fence Name", "Geo Fence Title", "GF Type",
            "Buffer Distance (Meters)", "Comments", "MarkerIndex"
        ], colModel,
        "pagerGeoFenceAdmin", "Id", "asc", "", 50, _rowList, 1, 1, "Geo Fence Managment",
        {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            userdata: "userdata"
        }, undefined, undefined, false, false, function () {
            //-/http://stackoverflow.com/questions/11743983/adding-a-custom-button-in-row-in-jqgrid
            var iCol = getColumnIndexByName(grid, "act");
            $grid.find(">tbody>tr.jqgrow>td:nth-child(" + (iCol + 1) + ")")
                .each(function() {
                    $("<div>", {
                                title: "Display on Map",
                                mouseover: function() { $(this).addClass("ui-state-hover"); },
                                mouseout: function() { $(this).removeClass("ui-state-hover"); },
                                click: function(e) {
                                    //-/Show polygon,polyline,circle or rectangle on map
                                    var id = $(e.target).closest("tr.jqgrow").attr("id");
                                    $(e.target).toggleClass("ui-icon-image").toggleClass("ui-icon-closethick");
                                    if ($(e.target).hasClass("ui-icon-image")) {
                                        unRegisterOverlay(parseInt($grid.jqGrid("getCell", id, "MarkerIndex")));
                                        return;
                                    }
                                    drawGeoFenceByType($("#selOrganizationByUserForGeoFence").val(), $("#selCompanyByOrgForGeoFence").val(), "", id);
                                }
                            }
                        ).css({ "margin-right": "5px", float: "left", cursor: "pointer" })
                        .addClass("ui-pg-div ui-inline-custom")
                        .append("<span class='ui-icon ui-icon-image'></span>")
                        .prependTo($(this).children("div"));
                });
            $grid.setGridWidth(750);
            $grid.setGridHeight($(window).height() - 200);
            $(".ui-jqgrid-active").css("left", 350).css("bottom", 100);
            $("#pagerGeoFenceAdmin_left").removeAttr("style");
            $("#" + $grid.attr("aria-labelledby")).show();

            if ($("#pagerGeoFenceAdmin_left").find(".ui-icon-plus").length == 0)
                $grid.jqGrid("navButtonAdd", "#pagerGeoFenceAdmin", {
                    caption: "",
                    buttonicon: "ui-icon-plus",
                    position: "first",
                    title: "Add Geo Fence",
                    onClickButton: function() {
                        mapDrawing(true);
                        $("#gbox_listGeoFenceAdmin .ui-jqgrid-titlebar-close.HeaderButton .ui-icon.ui-icon-circle-triangle-n").parent().click();
                    }
                });

            //-----Close Selection of Org
            $("#geofenceAdmin").dialog("close");
        }, undefined, undefined, undefined, //Edit,Add,Delete 
        false, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined,
        false, undefined, function () { clearMapFromSelectedMarkersInGrid(); }, // Before Request event of grid
        function () { clearMapFromSelectedMarkersInGrid(); }, ";Edit Geo Fence;;;;Delete Geo Fence;;Find Geo Fence;;Refresh;Notice;Please select Geo Fence;;"); // Before closing grid event of grid
    }

function loadRouteAdminGrid(orgCode, companyCode) {
    var grid = $("#listRouteAdmin"),
        getColumnIndexByName = function(grid, columnName) {
            var cm = $("#listRouteAdmin").jqGrid("getGridParam", "colModel"), i, l = cm.length;
            for (i = 0; i < l; i++) {
                if (cm[i].name === columnName) {
                    return i; // return the index
                }
            }
            return -1;
        },
        delOptions = {
            url: urlDeleteRoute,
            modal: true,
            resize: false,
            drag: false,
            closeOnEscape: true,
            caption: "Delete Existing Route",
            msg: "<br />&nbsp;Are you sure to delete the<br />&nbsp;selected Route?",
            bSubmit: "Yes",
            bCancel: "No",
            onclickSubmit: function(options, formid) {
                options.url += "/" + formid;
                options.mtype = "DELETE";
            },
            beforeSubmit: function (postdata, formid) {
                for (var i = $("#listRouteAdmin").jqGrid("getCell", postdata, "MarkerIndex").split("-")[0]; i < $("#listRouteAdmin").jqGrid("getCell", postdata, "MarkerIndex").split("-")[1]; i++) {
                    unRegisterOverlay(i);
                }
                return [true, ""];
            }
        },
        editOptions = {
            //Edit //http://techbrij.com/add-edit-delete-jqgrid-asp-net-web-api
            url: urlEditRoute,
            closeAfterEdit: true,
            closeOnEscape: true,
            bSubmit: "Update",
            modal: true,
            width: 560,
            onclickSubmit: function(options, postData) {
                if (options.url.indexOf("/" + postData.Id) == -1)
                    options.url += "/" + postData.Id;
                options.mtype = "PUT";
            }
        },
        colModel = [
            {
                name: "act",
                index: "act",
                width: 75,
                align: "center",
                sortable: false,
                editable: false,
                formatter: "actions",
                formatoptions: //http://www.trirand.com/jqgridwiki/doku.php?id=wiki:predefined_formatter
                {
                    keys: true,
                    editformbutton: true,
                    editbutton: false,
                    delOptions: delOptions,
                    editOptions: editOptions
                }
            },
            {
                name: "Id",
                index: "Id",
                key: true,
                editable: true,
                hidden: true,
                sorttype: "int",
                editrules: { edithidden: false },
                editoptions: { dataInit: function(element) { $(element).attr("readonly", "readonly"); } }
            },
            {name: "RouteName", index: "RouteName", width: 300, align: "left", sorttype: "string", editable: true, editoptions: { maxlength: 100 }, editrules: { required: true} },
            { name: "Title", index: "Title", width: 100, align: "left", sorttype: "string", editable: true, hidden: true, editrules: { edithidden: true, required: true }, editoptions: { size: 15, maxlength: 15 } },
            {
                name: "LineColor",
                index: "LineColor",
                width: 70,
                editable: true,
                //hidden: true,
                formatter: "select",
                edittype: "select",
                stype: "select",
                //editrules: { edithidden: true },
                editoptions: { value: "Red:Red;Blue:Blue;White:White;Green:Green", defaultValue: "R" },
                searchoptions: { sopt: ["eq", "ne"], value: ":Any;Red:Red;Blue:Blue;White:White;Green:Green" }
            },
            { name: "Comments", index: "Comments", editable: true, hidden: true, editrules: { edithidden: true }, editoptions: { maxlength: 100 } },
            { name: "MarkerIndex", index: "MarkerIndex", width: 1, sortable: false, editable: false, hidden: true, search: false, view: false }
        ];
    loadjqGrid(
        [], "listRouteAdmin", urlGetRouteMst, { orgCode: orgCode, companyCode: companyCode },
        ["Actions", "Code", "Route Name", "Title", "Line Color", "Comments", "MarkerIndex"], colModel,
        "pagerRouteAdmin", "Id", "asc", "", 50, _rowList, 1, 1, "Route Managment",
        {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            userdata: "userdata"
        }, undefined, undefined, false, false, function() {
            //-/http://stackoverflow.com/questions/11743983/adding-a-custom-button-in-row-in-jqgrid
            var iCol = getColumnIndexByName(grid, "act");
            $("#listRouteAdmin").find(">tbody>tr.jqgrow>td:nth-child(" + (iCol + 1) + ")")
                .each(function() {
                    $("<div>", {
                                title: "Display on Map",
                                mouseover: function() { $(this).addClass("ui-state-hover"); },
                                mouseout: function() { $(this).removeClass("ui-state-hover"); },
                                click: function(e) {
                                    //-/Show polygon,polyline,circle or rectangle on map
                                    var id = $(e.target).closest("tr.jqgrow").attr("id");
                                    $(e.target).toggleClass("ui-icon-image").toggleClass("ui-icon-closethick");
                                    if ($(e.target).hasClass("ui-icon-image")) {
                                        for (var i = $("#listRouteAdmin").jqGrid("getCell", id, "MarkerIndex").split("-")[0]; i < $("#listRouteAdmin").jqGrid("getCell", id, "MarkerIndex").split("-")[1]; i++) {
                                            unRegisterOverlay(i);
                                        }
                                        return;
                                    }
                                    drawPoiByRoute(id, true);
                                }
                            }
                        ).css({ "margin-right": "5px", float: "left", cursor: "pointer" })
                        .addClass("ui-pg-div ui-inline-custom")
                        .append("<span class='ui-icon ui-icon-image'></span>")
                        .prependTo($(this).children("div"));
                });
            $("#listRouteAdmin").setGridWidth(530);
            $("#listRouteAdmin").setGridHeight($(window).height() - 200);
            $(".ui-jqgrid-active").css("left", 350).css("bottom", 100);
            $("#pagerRouteAdmin_left").removeAttr("style");
            $("#" + $("#listRouteAdmin").attr("aria-labelledby")).show();
            
            //-----Close Selection of Org
            $("#geofenceAdmin").dialog("close");

            if ($("#pagerRouteAdmin_left").find(".ui-icon-plus").length == 0) {
                $("#routeAdmin").dialog({
                    height: $(window).height(),
                    width: 620,
                    show: "blind",
                    hide: "explode",
                    draggable: true,
                    resizable: false,
                    modal: true,
                    title: "Add Route by selecting POI's",
                    position: { my: "left+520 top+50", at: "left top" },
                    autoOpen: false,
                    closeOnEscape: false,
                    //beforeClose: function (event, ui) { return false; },
                    dialogClass: "noclose",
                    //close: function () { clearRouteAdminIns(false); }
                });

                $("#listRouteAdmin").jqGrid("navButtonAdd", "#pagerRouteAdmin", {
                    caption: "",
                    buttonicon: "ui-icon-plus",
                    position: "first",
                    title: "Add Route",
                    onClickButton: function() {
                        $("#selRouteOrganizationByUser").closest("tr").hide();
                        $("#selRouteCompanyByUser").closest("tr").hide();
                        $("#routeAdmin").dialog("option", "height", $(window).height() - 50);
                        $("#routeAdmin").dialog("open");
                        $("#gbox_listRouteAdmin .ui-jqgrid-titlebar-close.HeaderButton .ui-icon.ui-icon-circle-triangle-n").parent().click();

                        $("#routeAdminLoader").css({ height: $("#routeAdmin").parent().css("height") });
                        $("#routeAdminLoader").css({ width: $("#routeAdmin").parent().css("width") });
                        $("#routeAdminLoader").css({ left: 520 });
                        $("#routeAdminLoader").css({ top: 50 });

                        $("#routeAdminLoader").fadeIn({
                            complete: function() {
                                loadPoiSelection(orgCode, companyCode);
                            }
                        });
                    }
                });
            }
        }, undefined, undefined, undefined, //Edit,Add,Delete 
        false, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined,
        false, undefined, function() { clearMapFromSelectedMarkersInGrid(); }, // Before Request event of grid
        function () { clearMapFromSelectedMarkersInGrid(); }, ";Edit Route;;Add Route;;Delete Route;;Find Route;;Refresh;Notice;Please select Rote;;"); // Before closing grid event of grid
}

//
function addRouteMst(orgCode, companyCode, title, routeName, lineColor, comment) {
    Resequence();

    var maxSeq = 0;
    $($("#listPoiSelection").jqGrid("getGridParam", "selarrrow")).each(function (index, value) {
        var currentSeq = parseInt($("#listPoiSelection").jqGrid("getCell", value, "Seq")) || 0;
        if (currentSeq > parseInt(maxSeq))
            maxSeq = currentSeq;
    });

    var poiNos = [];
    for (var i = 0; i < maxSeq; i++) {
        poiNos.push(0);
    }

    $($("#listPoiSelection").jqGrid("getGridParam", "selarrrow")).each(function(index, value) {
        var currentSeq = parseInt($("#listPoiSelection").jqGrid("getCell", value, "Seq")) || 0;
        if (currentSeq > 0)
            poiNos[currentSeq - 1] = $("#listPoiSelection").jqGrid("getCell", value, "Id");
    });

    $.ajax({
        method: "POST",
        url: urlSetRouteMst + "?orgCode=" + orgCode + "&companyCode=" + companyCode + "&title=" + title + "&routeName=" + routeName + "&lineColor=" + lineColor + "&comment=" + comment,
        dataType: "json",
        data: { "": poiNos },
        beforeSend: function () {
            $("#routeAdminLoader").css({ left: $("#routeAdmin").parent().css("left") });
            $("#routeAdminLoader").css({ top: $("#routeAdmin").parent().css("top") });
            $("#routeAdmin").dialog({
                //closeOnEscape: false,
                beforeClose: function (event, ui) { return false; },
                //dialogClass: "noclose",
                draggable: false
            });
            $("#routeAdminLoader").fadeIn();
        },
        complete: function () {
            $("#routeAdmin").dialog({
                //closeOnEscape: true,
                beforeClose: function (event, ui) { return true; },
                //dialogClass: "",
                draggable: true
            });

            //unRegisterOverlay(overlayIndex);
            clearRouteAdminIns();
            $("#listRouteAdmin").trigger("reloadGrid");
            $("#routeAdminLoader").fadeOut();
        },
        success: function (data, textStatus, xhr) {
            if (data)
                alert("Route has been saved successfully.");
            else
                alert("An error occur while saving route. Contact the administrator");
        }
    });
}

function loadPoiSelection(orgCode, companyCode) {
    var grid = $("#listPoiSelection"),
        getColumnIndexByName = function (grid, columnName) {
            var cm = $("#listPoiSelection").jqGrid("getGridParam", "colModel"), i, l = cm.length;
            for (i = 0; i < l; i++) {
                if (cm[i].name === columnName) {
                    return i; // return the index
                }
            }
            return -1;
        },
        colModel = [
            {
                name: "act",
                index: "act",
                width: 50,
                align: "center",
                sortable: false,
                editable: false,
                formatter: "actions",
                formatoptions: //http://www.trirand.com/jqgridwiki/doku.php?id=wiki:predefined_formatter
                {
                    keys: true,
                    editformbutton: false,
                    editbutton: false,
                    delbutton: false
                }
            },
            {
                name: "Id",
                index: "Id",
                key: true,
                hidden: true,
                sorttype: "int" 
            },
            { name: "PoiName", index: "PoiName", width: 300, align: "left", sorttype: "string" },
            { name: "PoiTypeNo", index: "PoiTypeNo", hidden: true },
            { name: "TypeName", index: "TypeName", width: 100 },
            { name: "Lati", index: "Lati", width: 110, template: floatTemplate, hidden: true },
            { name: "Longi", index: "Longi", width: 110, template: floatTemplate, hidden: true },
            { name: "Seq", index: "Seq", width: 30, sortable: false, search: false, view: false },
            { name: "MarkerIndex", index: "MarkerIndex", width: 1, sortable: false, hidden: true, search: false, view: false }
        ];
    loadjqGrid(
        [], "listPoiSelection", urlGetPois, { orgCode: orgCode, companyCode: companyCode },
        ["Display", "Code", "POI Name", "POI Type Code", "POI Type", "Latitude", "Longitude", "Seq", "MarkerIndex"], colModel,
        "pagerPoiSelection", "Id", "asc", "", 0, undefined, 1, 1, "POI Selection",
        {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            userdata: "userdata"
        }, undefined, undefined, true, false, function () {
            //-/http://stackoverflow.com/questions/11743983/adding-a-custom-button-in-row-in-jqgrid
            var iCol = getColumnIndexByName(grid, "act");
            $grid.find(">tbody>tr.jqgrow>td:nth-child(" + (iCol + 1) + ")")
                .each(function() {
                    $("<div>", {
                                title: "Display POI on Map",
                                mouseover: function() { $(this).addClass("ui-state-hover"); },
                                mouseout: function() { $(this).removeClass("ui-state-hover"); },
                                click: function(e) {
                                    //-/Show marker on map
                                    var id = $(e.target).closest("tr.jqgrow").attr("id");
                                    $(e.target).toggleClass("ui-icon-image").toggleClass("ui-icon-closethick");
                                    if ($(e.target).hasClass("ui-icon-image")) {
                                        unRegisterOverlay(parseInt($grid.jqGrid("getCell", id, "MarkerIndex")));
                                        return;
                                    }
                                    var latLng = new g.LatLng($grid.jqGrid("getCell", id, "Lati"), $grid.jqGrid("getCell", id, "Longi"));
                                    $.get(urlGetGImagePathByPoiTypeNo, { poiTypeNo: $grid.jqGrid("getCell", id, "PoiTypeNo") }, function (imagePath) {
                                        var markerImage = poiMarkerImage(imagePath);
                                        var marker = addMarkerForResource(latLng, undefined, false, false, $grid.jqGrid("getCell", id, "MarkerIndex") == "" ? undefined : $grid.jqGrid("getCell", id, "MarkerIndex"), undefined, markerImage);

                                        $grid.jqGrid("setCell", id, "MarkerIndex", overlaysArray.marker.indexOf(marker));
                                        map.setCenter(latLng);
                                        $("[aria-describedby='routeAdmin']").find(".dialog-minimize.ui-dialog-titlebar-min").click();
                                        //$("[aria-describedby='routeAdmin']").find(".dialog-restore.ui-dialog-titlebar-rest").click()

                                        //create empty LatLngBounds object
                                        var bounds = new g.LatLngBounds();
                                        //extend the bounds to include each marker's position
                                        var rows = $grid.getDataIDs(), row;
                                        if (rows)
                                            for (var i = 0; i < rows.length; i++) {
                                                row = $grid.getRowData(rows[i]);
                                                if (row.MarkerIndex && overlaysArray.marker[parseInt(row.MarkerIndex)] != null)
                                                    bounds.extend(overlaysArray.marker[parseInt(row.MarkerIndex)].position);
                                            }
                                        //now fit the map to the newly inclusive bounds
                                        map.fitBounds(bounds);
                                        //(optional) restore the zoom level after the map is done scaling
                                        var listener = g.event.addListener(map, "idle", function () {
                                            if (map.getZoom() > zoom) map.setZoom(zoom);
                                            g.event.removeListener(listener);
                                        });
                                    });
                                    //-/Show marker on map
                                }
                            }
                        ).css({ "margin-right": "5px", float: "left", cursor: "pointer" })
                        .addClass("ui-pg-div ui-inline-custom")
                        .append("<span class='ui-icon ui-icon-image'></span>")
                        .prependTo($(this).children("div"));
                });
            $grid.setGridWidth(590);
            $grid.setGridHeight($(window).height() - 370);
            $grid.closest(".ui-jqgrid-active").css("left", 350).css("bottom", 100);
            $("#pagerPoiSelection_left").removeAttr("style");
            $("#" + $grid.attr("aria-labelledby")).show();

            if ($("#pagerPoiSelection_left").find(".ui-icon-arrowthickstop-1-s").length == 0)
                $grid.jqGrid("navButtonAdd", "#pagerPoiSelection", {
                    caption: "",
                    buttonicon: "ui-icon-arrowthickstop-1-s",
                    position: "first",
                    title: "Resequence",
                    onClickButton: function () { Resequence(); }
                });

            $("#routeAdminLoader").fadeOut();
        }, undefined, undefined, undefined, //Edit,Add,Delete 
        false, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined,
        true, function (id, status) {
            if (status) {
                var maxSeq = 0;
                $($("#listPoiSelection").jqGrid("getGridParam", "selarrrow")).each(function(index,value) {
                    var currentSeq = parseInt($("#listPoiSelection").jqGrid("getCell", value, "Seq")) || 0;
                    if (currentSeq > parseInt(maxSeq))
                        maxSeq = currentSeq;
                    //console.log(value + " " + currentSeq + " " + maxSeq);
                });
                $("#listPoiSelection").jqGrid("setCell", id, "Seq", parseInt(maxSeq) + 1);
            }
            else
                $("#listPoiSelection").jqGrid("setCell", id, "Seq", 0);
            //$("#listPoiSelection").getRowData(rowKey);
        }, function () { clearMapFromSelectedMarkersInGrid(); }, // Before Request event of grid
        function () { clearMapFromSelectedMarkersInGrid(); }, ";;;;;;;Find POI;;Refresh;Notice;Please select POI;;",true); // Before closing grid event of grid
}

function Resequence() {
    $($("#listPoiSelection").jqGrid("getGridParam", "selarrrow")).each(function(index, value) {
        $("#listPoiSelection").jqGrid("setCell", value, "Seq", index + 1);
    });
}