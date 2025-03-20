//codeAddress('Bissell House, Saint George Street, Toronto, ON, Canada')
var map, mapOptions, drawingManager, autoComplete,
    overlaysArray = {
        polygon: [],
        circle: [],
        polyline: [],
        rectangle: [],
        marker: [],
        infoWindow: [],
        customOverlayLabel: [],
        markerPolyline: [],
        markerArray: [],
        infoWindowArray: [],
        baseType: []
    }, g = google.maps;//--temp--
    var karachi = new google.maps.LatLng(24.893599, 67.027903); //--temp--
    var colorEnum = {
        Grey: "#ccc",
        Red: "#ff0000",
        Green: "#00ff00",
        Blue: "#003366",
        Yellow: "#f7ce00"
    }
    var zoom = 17;
    // This is the minimum zoom level that we'll allow
    var minZoomLevel = 5;
$(document).ready(function () {
    $.ajaxSetup({
        error: function (x, t, e) { //jqXHR, textStatus, errorThrown
            if (x.responseText.indexOf("The wait operation timed out") >= 0)
                alert("The server is busy right now, Please try again later or contact system administrator.");
            else if (x.status == 550)
                alert("550 Error Message");
            else if (x.status == "403")
                alert("403. Not Authorized");
            else if (x.status == "500")
                alert("500. Internal Server Error");
            else
                alert("Error..." + x.readyState + " " + x.status + " " + e.msg + " " + e + " \n please try again later.");
        }
    });

    //--temp--/-*
    initialize("map_canvas", karachi, zoom);
    $("#zoomLevel").html(zoom);
    /*{
        var theta, radius, dx, dy, deltaLongitude, deltaLatitude, finalLongitude, finalLatitude;
        radius = 300;
        var circle = new g.Circle({
            map: map,
            center: karachi,
            radius: radius
        });
        for (var i = 1; i <= 360; i++) {
            theta = i;
            dx = radius * Math.cos(theta);
            dy = radius * Math.sin(theta);
            deltaLongitude = dx / (111320 * Math.cos(karachi.lat()));
            deltaLatitude = dy / 110540;
            finalLongitude = karachi.lng() + deltaLongitude;
            finalLatitude = karachi.lat() + deltaLatitude;

            addMarkerForResource(new g.LatLng(finalLatitude, finalLongitude), undefined, true, false, undefined, undefined, undefined);
        }
        addMarkerForResource(karachi, undefined, true, false, undefined, undefined, undefined);
    }
    return;*/
    //google.maps.event.addDomListener(window, "load", initialize("map_canvas", karachi, zoom));
    //advanceDocking("#dock");

    registerOtherButtons();
    manageAccessToolBoxButtons(); //registerToolBoxButtons();
    registerGeoFenceButtons();
    latLngLocatorRegisterEvents();
    geoCoderRegisterEvents();
    autoRefreshIntervalRegisterEvents();
    latLngLocatorRegisterEvents();
    registerMeasureButtons();
    // Create the DIV to hold the control and call the HomeControl()
    // constructor passing in this DIV.
    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(new HomeControl(map, karachi, 1));
    //--temp--*/
    getVehicleTree();
    timerCountdown();
    $("#chkSelectAll").click(function () {
        showAllAsset();
    });

    intiateAssetFinder();
    initiateMapByDate(); //--temp--
    initiateTrackMapByDate(); //--temp--
    console.log("test error.");
    return;
});

function getVehicleTree(expandAll) {
    var treeviewClickCounts = 1;
    $("#assetTree").empty();
    $("#assetTree").fadeOut({
        complete: function() {
            $("#assetTreeLoader").fadeIn(
            {
                complete: function() {
                    $("#assetTree").treeview({
                        url: urlTreeView, //+ "?splateid=%" + $("#txtSearch").val() +"%",
                        ajax: {
                            type: "GET",
                            data: { sPlateId: $("#txtSearch").val(), sOrgName: $("#txtOrgSearch").val(), sResourceType: $("#txtTypeSearch").val(), sGroup: $("#txtGroupSearch").val() },
                            complete: function (response, status) {
                                treeviewClickCounts--;
                                if (expandAll) {
                                    if (status == "success") {
                                        var treeNodes = JSON.parse(response.responseText);
                                        $(treeNodes).each(function () {
                                            if (this.hasChildren) {
                                                $("li[id='" + this.id + "'] > span").click();
                                                treeviewClickCounts++;
                                            }
                                        });
                                    }
                                }
                                callRightClickOnObject();
                                if (0 == treeviewClickCounts) {
                                    // this was the last Ajax connection, do the thing
                                    closeTreeviewNodes();
                                    $("#assetTreeLoader").fadeOut({
                                        complete: function () {
                                            $("#assetTree").fadeIn();
                                        }
                                    });
                                }
                            },
                            error: function (xhr, errDesc, exception) {
                                treeviewClickCounts--;
                                if (0 == treeviewClickCounts) {
                                    // this was the last Ajax connection, do the thing
                                    $("#assetTreeLoader").fadeOut({
                                        complete: function () {
                                            $("#assetTree").fadeIn();
                                        }
                                    });
                                }
                            }
                        },
                        animated: "fast",
                        collapsed: true,
                        control: "#assetTreeControl"
                    });

                }
            });       
        }
    });
}

function closeTreeviewNodes() {
    $("#assetTree").find("li[id*='.']").each(function () {
        if ($(this)[0].id.split(".").length == 3 && $(this).find(":checkbox").length == 0 && $(this).hasClass("collapsable"))
            $(this).find("> span").click();
    });
    $("#assetTree").find("li[id*='.']").each(function () {
        if ($(this)[0].id.split(".").length == 2 && $(this).find(":checkbox").length == 0 && $(this).hasClass("collapsable"))
            $(this).find("> span").click();
    });
    $("#assetTree").find("li[id*='-']").each(function () {
        if ($(this)[0].id.split("-").length == 2 && $(this).find(":checkbox").length == 0 && $(this).hasClass("collapsable"))
            $(this).find("> span").click();
    });
}

function filterTreeView() {
    if ($("#assetTree").length > 0) {
        if (!$("#txtSearch").val() && !$("#txtOrgSearch").val() && !$("#txtTypeSearch").val() && !$("#txtGroupSearch").val()) {
            alert("Please Enter Search Criteria");
            $("#txtSearch").focus();
            return;
        }
        getVehicleTree(true);
    }
}

function clearFilterTreeView() {
    $("#txtOrgSearch").val("");
    $("#txtTypeSearch").val("");
    $("#txtGroupSearch").val("");
    $("#txtSearch").val("");
    getVehicleTree(false);
}

function advanceSearchTreeView() {
    $("#txtOrgSearch").removeAttr("visibility");
    $('#txtTypeSearch').attr("visibility", "visible");
    $('#txtGroupSearch').attr("visibility", "visible");
}

function timerCountdown() {
    var newYear = $("#txtTime").val();
    if ($('#chkAutoRefresh').is(':checked')) {
        $('#defaultCountdown').countdown({ until: newYear, format: "s", onTick: $("#defaultCountdown").removeAttr("class")}); //,onTick: createOrMoveMarker(obj,false)
    }
}

g.InfoWindow.prototype.isOpen = function() {
    var thisMap = this.getMap();
    return (thisMap !== null && typeof thisMap !== "undefined");
};