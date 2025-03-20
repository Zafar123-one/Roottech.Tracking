function registerOtherButtons() {
    $("div.topSlide").click(function () {
        if (parseInt($("div.topTrack").css("top").replace("px", "")) == 0)
            $("div.topTrack").animate({ top: -42 }, function () {
                $("div.topSlide").css({ backgroundImage: "url(/Images/Common/topSlidingBack.png)" });
            });
        else
            $("div.topTrack").animate({ top: 0 }, function () {
                $("div.topSlide").css({ backgroundImage: "url(/Images/Common/topSliding.png)" });
            });
    });

    $(".GeoFenceTreeArea .TreeOpen").click(function () {
        $("div.GeoFenceTreeArea").animate({
            left:
                (parseInt($("div.GeoFenceTreeArea").css("left").replace("px", "")) == 0) ? -80 : 0
        });
    });
}

function registerToolBoxButtons() {
    $("#featureList").dialog({
        //height: 400,
        show: "blind",
        hide: "explode",
        //,draggable: false
        //,resizable: false 
        //,modal: true
        title: 'Assets Tree',
        position: { my: "left+120 top+50", at: "left top"}//[100, 100]
    });
    $("#showFeatureList").click(function () { $("#featureList").dialog("open"); });
    // Report Buttons
    $("#vehicleReportCard").dialog({
        height: 670,
        width: 700,
        show: "blind",
        hide: "explode",
        title: 'Vehicle Report Card',
        position: { my: "left+520 top+50", at: "left top" }, //[400, 100],
        autoOpen: false
    });
    $("#showVehicleReport").click(function () {
        $("#vehicleReportCard").dialog("open");
        $("#dateForVehicleReportCard").datepicker({
            dateFormat: 'mm/dd/yy'
        });
        getVehicleList("#selectVehicleForReportCard");
    });
    $("#vehicleFuelActivityReport").dialog({
        height: 150,
        width: 700,
        show: "blind",
        hide: "explode",
        title: 'Vehicle Fuel Activity Reports',
        position: { my: "left+520 top+50", at: "left top" }, //[400, 100],
        autoOpen: false
    });
    $("#showFleetReport").click(function () {
        $("#vehicleFuelActivityReport").dialog("open");
        $("#fromDateForFuelReport").datepicker({
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
        $("#toDateForFuelReport").datepicker({
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
        getUnitGroupList("#selectUnitGroupForFuelReport");
    });
    $("#StartStopReport").dialog({
        height: 120,
        width: 700,
        show: "blind",
        hide: "explode",
        title: 'Start / Stop Report',
        position: { my: "left+520 top+50", at: "left top" }, //[400, 100],
        autoOpen: false
    });
    $("#showStartStopReport").click(function () {
        $("#StartStopReport").dialog("open");
        $("#fromDateForStartStopReport").datepicker({
            dateFormat: 'mm/dd/yy'
        });
        $("#ToDateForStartStopReport").datepicker({
            dateFormat: 'mm/dd/yy'
        });
        getVehicleList("#selectVehicleForStartStopReport");
    });
    $("#clearGraphics").click(function () {
        deleteOverlays();
        $("#lblTotlaDistance").text("");
        $("#cTr > .overlay").css('backgroundColor', 'transparent');
    });
}

function permissionDeniedMsg(event) {
    alert("You don't have permission to access " + event.data.ModuleName + " module. If this is incorrect please contact your administrator.");
    //return false;
}

function manageAccessToolBoxButtons() {
    $.ajax({
        url: urlGetModAccess,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function(data, textStatus, xhr) {
            if (textStatus == "success") {
                registerToolBoxButtons();
                $(data).each(function() {
                    $("." + this.ObjCode).prop('title', this.Description);

                    if (this.Allow == 'N') {
                        switch (this.ObjCode) {
                            case "TBX_A_BM":
                                $("#showBookMark").off("click").click({ ModuleName: this.Description }, permissionDeniedMsg);
                            //$("#").dialog("destroy");
                            break;
                        case "TBX_A_QR":
                            $("#showQueryByRectangle").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            //$("#").dialog("destroy");
                            break;
                        case "TBX_A_GF":
                            $("#showGeoFence").off("click").click({ ModuleName: this.Description }, permissionDeniedMsg);
                            $("#geofenceAdmin").dialog("destroy");
                            break;
                        case "TBX_A_AS":
                            $("#showAlertMark").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            //$("#").dialog("destroy");
                            break;
                        case "TBX_A_LL":
                            $("#showLatLongLocator").off("click").click({ ModuleName: this.Description }, permissionDeniedMsg);
                            $("#latLongLocator").dialog("destroy");
                            break;
                        case "TBX_A_ME":
                            $("#showMeasure").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            $("#measureTool").dialog("destroy");
                            break;
                        case "TBX_A_ML":
                            $("#showMapLegend").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            //$("#").dialog("destroy");
                            break;
                        case "TBX_A_GC":
                            $("#showGeoCoder").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            $("#geoCoder").dialog("destroy");
                            break;
                        case "TBX_B_VRPT":
                            $("#showVehicleReport").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            $("#vehicleReportCard").dialog("destroy");
                            break;
                        case "TBX_B_FRPT":
                            $("#showFleetReport").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            $("#vehicleFuelActivityReport").dialog("destroy");
                            break;
                        case "TBX_B_SRPT":
                            $("#showStartStopReport").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            $("#StartStopReport").dialog("destroy");
                            break;
                        case "TBX_B_DRPT":
                            $("#showDaliyDistanceReport").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            //$("#").dialog("destroy");
                            break;
                        case "TBX_B_MRPT":
                            $("#showMontlySummaryReport").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            //$("#").dialog("destroy");
                            break;
                        case "TBX_B_ARPT":
                            $("#showAlertReport").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            //$("#").dialog("destroy");
                            break;
                        case "TBX_B_GRPT":
                            $("#showGeofenceAlertReport").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            //$("#").dialog("destroy");
                            break;
                        case "TBX_B_SMSG":
                            $("#showSystemMessage").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            //$("#").dialog("destroy");
                            break;
                        case "TBX_B_DFLT":
                            $("#showDataFilter").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            //$("#").dialog("destroy");
                            break;
                        case "TBX_B_ATRE":
                            $("#showFeatureList").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            $("#featureList").dialog("destroy");
                            break;
                        case "TBX_B_POI":
                            $("#showPoiAdmin").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            $("#poiAdmin").dialog("destroy");
                            break;
                        case "TBX_B_CLG":
                            $("#clearGraphics").off("click").click({ModuleName: this.Description}, permissionDeniedMsg );
                            break;
                        case "ATR_B_ID":
                            identifyDisabled = true;
                            break;
                        case "ATR_B_AV":
                            ampleViewDisabled = true;
                            break;
                        case "ATR_B_TM":
                            todayMapDisabled = true;
                            break;
                        case "ATR_B_TR":
                            todayReportDisabled = true;
                            break;
                        case "ATR_B_AF":
                            assetFinderDisabled = true;
                            break;
                        }
                    }
                });
            }
        }
    });
}

function registerGeoFenceButtons() {
    $("#mapPan").click(function () { mapPan(); $("#cTr > .overlay").css('backgroundColor', 'transparent'); });
    $("#rectangleByZoom").click(function () { rectangleByZoom(); });
    $("#fitAllToScreen").click(function () { zoomBackToFitAllAssetsOnMap(); });
    $("#zoomIN").click(function () { setZoomControl("In"); });
    $("#zoomOut").click(function () { setZoomControl("Out"); });
    $("#setHome").click(function () { setHome(); });
    $("#roadmap").prop("checked",true);
    $("[name='selectMapType']").click(function () {
        var obj = this.id;
        $("#hybrid").attr("checked", false);
        map.setMapTypeId(obj);
    });
    
    $("[name = 'satelliteType']").click(function () {
        var object = this;
        if ($(object).is(':checked')) {
            if ($("#satellite").is(':checked')) {
                map.setMapTypeId(object.id);
            } else {
                $(object).attr("checked", false);
                alert('Applicable only for Satellite view');
            }
        } else {
            if ($("#satellite").is(':checked')) {
                map.setMapTypeId('satellite');
            }
        }

    });
}

function setZoomControl(obj) {
    var lvl = parseInt(map.getZoom());
    if (obj == "In") map.setZoom(lvl + 1); else map.setZoom(lvl - 1);
}

function topDrawing(obj) {
    $("#cTr > .overlay").css('backgroundColor', 'transparent');
    if (obj == "drawPolyLine") {
        $("#tddrawPolyLine").css('backgroundColor', '#A5CFF7');
        drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYLINE);
    }
    else if (obj == "drawCircle") {
        $("#tddrawCircle").css('backgroundColor', '#A5CFF7');
        drawingManager.setDrawingMode(google.maps.drawing.OverlayType.CIRCLE);
    }
    else if (obj == "drawRectangle") {
        $("#tddrawRectangle").css('backgroundColor', '#A5CFF7');
        drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
    }
    else if (obj == "drawPolygon") {
        $("#tddrawPolygon").css('backgroundColor', '#A5CFF7');
        drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYGON);
    }
}


function registerMeasureButtons() {
    $("#measureTool").dialog({
        autoOpen: false,
        show: "blind",
        hide: "explode",
        //draggable: false,
        resizable: false,
        //modal: false,
        title: "Measure Tool",
        position: { my: "left+520 top+50", at: "left top" },
        close: function () {
            mapPan();
            $("#cTr > .overlay").css('backgroundColor', 'transparent');
            if ($("#measureTool").dialog("option", "title") == "Add Geo Fence")
                $("#gbox_listGeoFenceAdmin .ui-jqgrid-titlebar-close.HeaderButton .ui-icon.ui-icon-circle-triangle-s").parent().click();
        }
    });
    $("#showMeasure").click(function () { mapDrawing(false); });

    $("#drawPolyLine").click(function () { topDrawing("drawPolyLine"); });
    $("#drawCircle").click(function () { topDrawing("drawCircle"); });
    $("#drawRectangle").click(function () { topDrawing("drawRectangle"); });
    $("#drawPolygon").click(function () { topDrawing("drawPolygon"); });
}

function mapPan() {
    drawingManager.setDrawingMode(null);
}
function rectangleByZoom() {
    var myKeyDragZoom = map.getDragZoomObject();
    myKeyDragZoom.buttonDiv_.onclick(document.createEvent('MouseEvent'));
}

function zoomBackToFitAllAssetsOnMap() {
    var bounds = new g.LatLngBounds();
    $.each(overlaysArray.baseType, function (index, value) {
        if (overlaysArray.markerPolyline[index] !== null)
            bounds.extend(overlaysArray.markerPolyline[index].getPath().getAt(0));
    });
    map.fitBounds(bounds);
}

function logout() {
    window.location.href = urlLoginPage;
    //jQuery('#map_canvas').empty();
}

function setHome() {
    map.setZoom(zoom);
    map.setCenter(karachi);
}

function getVehicleList(ddlId) {
    $.ajax({
        url: urlGetVehicleList,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data) {
            var bindData = { org: data };
            var template = "{{#org}}<option value={{Id}}>{{PlateID}}</option>{{/org}}";
            var html = Mustache.to_html(template, bindData);
            $(ddlId).html(html);
            $(ddlId).show();
        }
    });
}

function getUnitGroupList(ddlId) {
    $.ajax({
        url: urlGetUnitGroupList,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data) {
            var bindData = { org: data };
            var template = "{{#org}}<option value={{Id}}>{{Description}}</option>{{/org}}";
            var html = Mustache.to_html(template, bindData);
            html = "<option value=>Select Unit Group</option>" + html;
            $(ddlId).html(html);
            $(ddlId).show();
        }
    });
}