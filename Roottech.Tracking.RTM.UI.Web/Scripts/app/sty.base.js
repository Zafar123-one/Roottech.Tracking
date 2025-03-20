var gMap = google.maps;//, countCallBacks = 0, bounds = new g.LatLngBounds(), gfAssets = null, areaToGF = { 0: "I", 1: "O", 2: "B" }
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
var karachi = new gMap.LatLng(24.893599, 67.027903),
            colorEnum = {
                Grey: "#ccc",
                Red: "#ff0000",
                Green: "#00ff00",
                Blue: "#003366",
                Yellow: "#f7ce00"
            },
            zoom = 17, map;
// This is the minimum zoom level that we'll allow
var minZoomLevel = 5;
var _unitIdForAmpleView;
var _siteNameForAmpleViewDetail;
var _fuelRateForAmpleView, isBank;
function initialize(mapCanvas, latLng, zoom) {

    var mapOptions = {
        center: latLng,
        zoom: zoom,
        minZoom: minZoomLevel,
        mapTypeId: gMap.MapTypeId.ROADMAP,
        mapTypeControl: false,
        mapTypeControlOptions: {
                mapTypeIds: [gMap.MapTypeId.ROADMAP,
                    gMap.MapTypeId.TERRAIN,
                    gMap.MapTypeId.HYBRID,
                    gMap.MapTypeId.SATELLITE]
            },
        overviewMapControl: true,
        overviewMapControlOptions: { opened: true },
        scaleControl: false,
        scaleControlOptions: { poistion: gMap.ControlPosition.TOP_LEFT },
        zoomControl: false,
        zoomControlOptions: { style: gMap.NavigationControlStyle.LARGE },
        panControl: false,
        panControlOptions: { poistion: gMap.ControlPosition.LEFT_BOTTOM }, 
        rotateControl: false,
        rotateControlOptions: { poistion: gMap.ControlPosition.LEFT_BOTTOM },
        streetViewControl: false,
        streetViewControlOptions: { poistion: gMap.ControlPosition.RIGHT_CENTER }
    };
    map = new gMap.Map(document.getElementById(mapCanvas), mapOptions);

    map.enableKeyDragZoom({
        key: "none",
        visualEnabled: true,
        visualSize: new gMap.Size(0, 0),
        boxStyle: {
            border: "1px dashed black", //"4px dashed black",
            backgroundColor: "transparent",
            opacity: 1.0
        },
        veilStyle: { opacity: 0.35, cursor: "crosshair" }
    });

    gMap.event.addListener(map, "zoom_changed", function() { $("#zoomLevel").html(map.getZoom()); });
    $("#zoomLevel").html(zoom);

    registerStationaryButtons();

    var monthNames = ["December", "November", "October", "September", "August",
        "July", "June", "May", "April", "March", "February", "January"];
    $.each(monthNames, function (key, value) {
        $('#monthsForSummary')
            .append($("<option></option>")
                       .attr("value", Math.abs(key - monthNames.length))
                       .text(value)); 
    });

    $.each(new Array(11),
        function (n) {
            $('#yearsForSummary')
            .append($("<option></option>")
                       .attr("value", (new Date()).getFullYear() - n)
                       .text((new Date()).getFullYear() - n));
        }
    );
    $("#statisticsDate").datepicker({ dateFormat: _dateFormat, showButtonPanel: true });
    $("#statisticsDate").datepicker("setDate", new Date());

    $.ajax({
        url: urlOrg,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data.length < 1) return;

            if (data.length === 1) {
                $("#lblWelcome").html($("#lblWelcome").html().split("-")[0] + " - " + data[0].Description);
                return;
            }

            var html;
            $.each(data, function (index, value) {
                html += "<option value='" + value.Id + "' " + (value.Id === userOrgCode? "selected" : "") + ">" + value.Description + "</option>";
            });
            var welcomeHtml = "<form action='" + urlMapIndex + "'  method='post'>" + $("#lblWelcome").html().split("-")[0];
            welcomeHtml += ' - <select name="selOrganizationByUser" id="selOrganizationByUser" onchange="this.form.submit();">';
            welcomeHtml += html;
            welcomeHtml += "</select></form>";
            $("#lblWelcome").addClass("right");
            $("#lblWelcome").html(welcomeHtml);
        }
    });
}

function registerStationaryButtons() {
    $("#mapPan").click(function () { mapPan(); $("#cTr > .overlay").css('backgroundColor', 'transparent'); });
    $("#rectangleByZoom").click(function () { rectangleByZoom(); });
    $("#fitAllToScreen").click(function () { zoomBackToFitAllAssetsOnMap(); });
    $("#zoomIN").click(function () { setZoomControl("In"); });
    $("#zoomOut").click(function () { setZoomControl("Out"); });
    $("#setHome").click(function () { setHome(); });
    $("#roadmap").prop("checked", true);
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

    $("#divAmpleView").dialog({
        autoOpen: false,
        title: 'Ample View',
        width: 950,
        height: 620,
        background: "#f4f4f4",
        position: { my: "left+140 top+50", at: "left top" },
        modal: true
        , close: function() {
            $(".ui-state-error.ui-corner-all").stop(true);
            $("#guageInfo").empty();
            $("#guageInfo").append('<div></div>');
            $("#guageInfo").children("div").jqxLinearGauge(this.ChartArgs);
            clearInterval(intervalForAV);
        }
    });
    //$("#divAmpleView").parent().children(".ui-dialog-titlebar").children(".ui-dialog-title").css({ width: '20%' });
    //$("#divAmpleView").parent().children(".ui-dialog-titlebar").children(".ui-dialog-titlebar-close").before(
    //    $("<div class='ui-jqgrid-titlebar-close HeaderButton' style=float:right;width:200px;font-weight:normal><input type='text' id='txtTimeForAV' value='60' style='width: 30px' onblur='setAutoRefreshTimeForAV();' />(sec)<label><input type='checkbox' name='autoRefresh' id='chkAutoRefreshForAV' onclick='setAutoRefreshTimeForAV();'>Auto Refresh</label></div>"));

    $("#divRefuelDetails").dialog({
        height: 200,
        width: 420,
        show: "blind",
        hide: "explode",
        title: "Refuel Detail (Monthly)",
        position: { my: "left+520 top+50", at: "left top" }, //[400, 100],
        autoOpen: false,
        modal: true
        , close: function () { $("#tableRefuelDetails tr").not(":first").remove() }
    });

    $(".GeoFenceTreeArea .TreeOpen").click(function () {
        //$("div.GeoFenceTreeArea").animate({
        //    left:
        //        (parseInt($("div.GeoFenceTreeArea").css("left").replace("px", "")) === 0) ? -80 : 0
        //});

        if (parseInt($("div.GeoFenceTreeArea").css("left").replace("px", "")) === 0)
            $("div.GeoFenceTreeArea").animate({ left: -80 }, function () {
                $("div.TreeOpen").css({ backgroundImage: "url(/Images/Common/openTreeBar.png)" });
            });
        else
            $("div.GeoFenceTreeArea").animate({ left: 0 }, function () {
                $("div.TreeOpen").css({ backgroundImage: "url(/Images/Common/openTreeBarMinus.png)" });
            });
    });

    $("#fuelActivityReportCard").dialog({
        height: 170,
        width: 750,
        show: "blind",
        hide: "explode",
        title: 'Fuel Activity Reports',
        position: { my: "left+520 top+50", at: "left top" }, //[400, 100],
        autoOpen: false
        , close: function () { resetFuelActivityReportCard(); }
    });

    $("#showFuelReport").click(function () {
        $("#fuelActivityReportCard").dialog("open");
        $("#fromDateForFuelReport").datepicker({
            dateFormat: _dateFormat,
            beforeShow: function(input, inst) {
                $('#ui-datepicker-div').removeClass("monthForMonthwiseStats");
            }
            , showButtonPanel: true
        });
        $("#fromDateForFuelReport").datepicker("setDate", new Date());
        $("#toDateForFuelReport").datepicker({ dateFormat: _dateFormat, showButtonPanel: true });
        $("#toDateForFuelReport").datepicker("setDate", new Date());
    });

    bindSelectsByName("#cityForFuelReport", urlGetCities);
    bindSelectsByName("#regionForFuelReport", urlGetRegionsByCity + "?city=");//, "#cityForFuelReport", "Please select city first");//urlGetRegions);//
    bindSelectsByName("#siteForFuelReport", urlGetSitesByRegion + "?region=");
    bindSelectsByName("#assetForFuelReport", urlGetAssetsBySite + "?site=", "#siteForFuelReport", "Please select site first");

    //$("#regionForFuelReport").change(function () {
    //    $("#siteForFuelReport").html("");
    //    $("#siteForFuelReport").val("");
    //    $("#assetForFuelReport").html("");
    //    $("#assetForFuelReport").val("");
    //});
    //$("#siteForFuelReport").change(function () {
    //    $("#assetForFuelReport").html("");
    //    $("#assetForFuelReport").val("");
    //});

    $("#assetActivityReportCard").dialog({
        height: 170,
        width: 730,
        show: "blind",
        hide: "explode",
        title: 'Asset/Grid Activity Reports',
        position: { my: "left+520 top+50", at: "left top" }, //[400, 100],
        autoOpen: false
        , close: function () {
            resetAssetActivityReportCard();
        }
    });

    $("#showAssetActivityReport").click(function () {
        $("#assetActivityReportCard").dialog("open");
        $("#fromDateForAssetReport").datepicker({
            dateFormat: _dateFormat,
            beforeShow: function (input, inst) {
                $('#ui-datepicker-div').removeClass("monthForMonthwiseStats");
            }, showButtonPanel: true
        });
        $("#fromDateForAssetReport").datepicker("setDate", new Date());
        $("#toDateForAssetReport").datepicker({ dateFormat: _dateFormat, showButtonPanel: true });
        $("#toDateForAssetReport").datepicker("setDate", new Date());
    });


    bindSelectsByName("#cityForAssetReport", urlGetCities);
    bindSelectsByName("#regionForAssetReport", urlGetRegionsByCity + "?city=");//, "#cityForAssetReport", "Please select city first");
    bindSelectsByName("#siteForAssetReport", urlGetSitesByRegion + "?region=");
    bindSelectsByName("#assetForAssetReport", urlGetAssetsBySite + "?site=", "#siteForAssetReport", "Please select site first");

    //$("#regionForAssetReport").change(function () {
    //    $("#siteForAssetReport").html("");
    //    $("#siteForAssetReport").val("");
    //    $("#assetForAssetReport").html("");
    //    $("#assetForAssetReport").val("");
    //});
    //$("#siteForAssetReport").change(function () {
    //    $("#assetForAssetReport").html("");
    //    $("#assetForAssetReport").val("");
    //});

    $("#monthwiseStatisticsReportCard").dialog({
        height: 180,
        width: 620,
        show: "blind",
        hide: "explode",
        title: 'Monthwise Statistics Reports',
        position: { my: "left+520 top+50", at: "left top" },
        autoOpen: false,
        close: function () {
            resetMonthwiseStatisticsReportCard();
        }
    });

    $('#monthForMonthwiseStats').datepicker({
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        dateFormat: 'M-yy',
        onClose: function(dateText, inst) {
            function isDonePressed() {
                return ($('#ui-datepicker-div').html().indexOf('ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all ui-state-hover') > -1);
            }

            if (isDonePressed()) {
                var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                $(this).datepicker('setDate', new Date(year, month, 1)).trigger('change');
                $('.date-picker').focusout(); //Added to remove focus from datepicker input box on selecting date
            }
        },
        beforeShow: function(input, inst) {

            /*inst.dpDiv.addClass('month_year_datepicker');

            if ((datestr = $(this).val()).length > 0) {
                year = datestr.substring(datestr.length - 4, datestr.length);
            }*/
            $('#ui-datepicker-div').addClass("monthForMonthwiseStats");
        }
    });
    

    $("#showMonthwiseStatisticsReport").click(function () {
        $("#monthwiseStatisticsReportCard").dialog("open");
        $("#monthForMonthwiseStats").datepicker({ dateFormat: "M-yy", showButtonPanel: true });
        $("#monthForMonthwiseStats").datepicker("setDate", new Date());
    });

    bindSelectsByName("#cityForMonthwiseStats", urlGetCities);
    bindSelectsByName("#regionForMonthwiseStats", urlGetRegionsByCity + "?city=", "#cityForMonthwiseStats", "Please select city first");
    bindSelectsByName("#siteForMonthwiseStats", urlGetSitesByRegion + "?region=");
    bindSelectsByName("#assetForMonthwiseStats", urlGetAssetsBySite + "?site=", "#siteForMonthwiseStats", "Please select site first");

    //$("#regionForMonthwiseStats").change(function () {
    //    $("#siteForMonthwiseStats").html("");
    //    $("#siteForMonthwiseStats").val("");
    //    $("#assetForMonthwiseStats").html("");
    //    $("#assetForMonthwiseStats").val("");
    //});
    //$("#siteForMonthwiseStats").change(function () {
    //    $("#assetForMonthwiseStats").html("");
    //    $("#assetForMonthwiseStats").val("");
    //});

    $("#assetInfoReportCard").dialog({
        height: 120,
        width: 600,
        show: "blind",
        hide: "explode",
        title: 'Asset Information Report',
        position: { my: "left+520 top+50", at: "left top" },
        autoOpen: false,
        close: function () {
            resetAssetInfoReportCard();
        }
    });

    $("#showDaliyDistanceReport").click(function () {
        $("[name='assetInfoReport'][value=-1]").prop("checked", true);
        $("#assetInfoReportCard").dialog("open");
    });

    //bindSelectsByName("#regionForAssetInfoReport", urlGetRegions);
    bindSelectsByName("#regionForAssetInfoReport", urlGetRegionsByCity + "?city=");//, "#cityForAssetInfoReport", "Please select city first");
    bindSelectsByName("#siteForAssetInfoReport", urlGetSitesByRegion + "?region=");
    bindSelectsByName("#assetForAssetInfoReport", urlGetAssetsBySite + "?site=", "#siteForAssetInfoReport", "Please select site first");

    $("#regionForAssetInfoReport").change(function () {
        $("#siteForAssetInfoReport").html("");
        $("#siteForAssetInfoReport").val("");
        $("#assetForAssetInfoReport").html("");
        $("#assetForAssetInfoReport").val("");
    });
    $("#siteForAssetInfoReport").change(function () {
        $("#assetForAssetInfoReport").html("");
        $("#assetForAssetInfoReport").val("");
    });

    
    $("#contactInfo").dialog({
        modal: true,
        //height: 120, 
        width: 450,
        show: "blind",
        hide: "explode",
        title: 'Contact Directory',
        position: { my: "left+520 top+250", at: "left top" },
        autoOpen: false,
        close: function () {
            $("#txtContactInfoSalutation").val("");
            $("#txtContactInfoName").val("");
            $("#txtContactInfoDesignation").val("");
            $("#txtContactInfoCell").val("");
            $("#txtContactInfoPhone").val("");
            $("#txtContactInfoExtension").val("");
            $("#txtContactInfoEmail").val("");
            //$("#assetInfoReportCard .error").hide();
            //$("#assetInfoLoader").hide();
        }
    });
        
    $("#contractManagement").dialog({
        modal: true,
        //height: 120, 
        width: 900,
        show: "blind",
        hide: "explode",
        title: 'Fleet Contract',
        position: { my: "left+520 top+250", at: "left top" },
        autoOpen: false,
        close: function () {
            cancelContractManagement();
            $("#txtContractManagementContractType").html("");
            $("#txtContractManagementMasterContract").html("");
            $("#txtContractManagementVendorCode").html("");
            $("#txtContractManagementToVendorCode").html("");
            $("#txtContractManagementContrStatus").html("");
            $("#txtContractManagementCompCode").html("");
        }
    });
    $("#txtContractManagementProject").prop('disabled', true);
    $("#txtContractManagementVendorCode").prop('disabled', true);
    $("#txtContractManagementRemainingDays").prop('disabled', true);
    $("#txtContractManagementContractDate").datepicker({ dateFormat: _dateFormat, showButtonPanel: true });
    $("#txtContractManagementDateFrom").datepicker({ dateFormat: _dateFormat, showButtonPanel: true });
    $("#txtContractManagementDateTo").datepicker({ dateFormat: _dateFormat, showButtonPanel: true });
    $("#txtContractManagementExpiryAlertDate").datepicker({ dateFormat: _dateFormat, showButtonPanel: true });

    bindSelectsByName("#txtContractManagementContractType", urlLovGetContrTypes, undefined, "", true);
    bindSelectsByName("#txtContractManagementVendorCode", urlLovGetVendors, undefined, "", true);
    bindSelectsByName("#txtContractManagementToVendorCode", urlLovGetVendors, undefined, "", true);
    bindSelectsByName("#txtContractManagementContrStatus", urlLovGetContrStatuses, undefined, "", true);
    bindSelectsByName("#txtContractManagementCompCode", urlLovGetCompanies, undefined, "", true);
    //bindSelectsByName("#txtContractManagementMasterContract", urlLovGetContrTypes, undefined, "", true);

    $("#chartFuelTransaction").dialog({
        title: "Trend Data: Fuel Usage",
        show: "blind",
        hide: "explode",
        position: { my: "left+520 top+50", at: "left top" }, //[400, 100],
        modal: true,
        height: $(window).height() - ($(window).height() * .20),
        width: $(window).width() - ($(window).width() * .20),
        autoOpen: false,
        open: function () {
            $("#chartFuelTransaction").append('<div id="trend_chart" style="margin:auto;"></div><div id="trend_pie_chart" style="margin:auto;width:400px"></div>');
            $("#trend_chart").height($("#chartFuelTransaction").height() - 40);
            $("#trend_chart").width($("#chartFuelTransaction").width() - 50);
            showFuelTransactionChart(_unitIdForAmpleView, $('#monthsForSummary').val(), $('#yearsForSummary').val(), $("#noofMonthstoGet").val());
        }, //END OPEN
        close: function () {
            $("#chartFuelTransaction").empty();
            //$("#tableRefuelDetails tr").not(":first").remove();
        }
    }); //END DIALOG

    for (var i=1;i<13;i++){
        $("#noofMonthstoGet").append($("<option />").val(i).text(i));
    }
    $("#noofMonthstoGet").val(6);
}
function resetFuelActivityReportCard()
{
    $("#cityForFuelReport").html("");
    $("#regionForFuelReport").html("");
    $("#siteForFuelReport").html("");
    $("#assetForFuelReport").html("");
    $("#fromDateForFuelReport").datepicker("setDate", new Date());
    $("#toDateForFuelReport").datepicker("setDate", new Date());
    $("#fuelActivityLoader").hide();
    if ($("#regionForFuelReport_combobox").length) $("#regionForFuelReport").combobox("destroy");
    if ($("#siteForFuelReport_combobox").length) $("#siteForFuelReport").combobox("destroy");
    if ($("#assetForFuelReport_combobox").length) $("#assetForFuelReport").combobox("destroy");
    if ($("#cityForFuelReport_combobox").length) $("#cityForFuelReport").combobox("destroy");
}

function resetAssetActivityReportCard() {
    $("#cityForAssetReport").html("");
    $("#regionForAssetReport").html("");
    $("#siteForAssetReport").html("");
    $("#assetForAssetReport").html("");
    $("#fromDateForAssetReport").datepicker("setDate", new Date());
    $("#toDateForAssetReport").datepicker("setDate", new Date());
    $("#assetActivityLoader").hide();
    $("#assetActivityReportCard .error").hide();
    $("input[name=assetReport][value=1]").prop("checked", true);

    if ($("#regionForAssetReport_combobox").length) $("#regionForAssetReport").combobox("destroy");
    if ($("#siteForAssetReport_combobox").length) $("#siteForAssetReport").combobox("destroy");
    if ($("#assetForAssetReport_combobox").length) $("#assetForAssetReport").combobox("destroy");
    if ($("#cityForAssetReport_combobox").length) $("#cityForAssetReport").combobox("destroy");
}

function resetMonthwiseStatisticsReportCard() {
    $("#cityForMonthwiseStats").html("");
    $("#regionForMonthwiseStats").html("");
    $("#siteForMonthwiseStats").html("");
    $("#assetForMonthwiseStats").html("");
    $("#engineForMonthwiseStats").val("");
    $("#ratingForMonthwiseStats").val("");
    $("#monthStatsLoader").hide();
    $("#monthwiseStatisticsReportCard .error").hide();
    if ($("#regionForMonthwiseStats_combobox").length) $("#regionForMonthwiseStats").combobox("destroy");
    if ($("#siteForMonthwiseStats_combobox").length) $("#siteForMonthwiseStats").combobox("destroy");
    if ($("#assetForMonthwiseStats_combobox").length) $("#assetForMonthwiseStats").combobox("destroy");
    if ($("#cityForMonthwiseStats_combobox").length) $("#cityForMonthwiseStats").combobox("destroy");
}

function resetAssetInfoReportCard() {
    $("#regionForAssetInfoReport").html("");
    $("#siteForAssetInfoReport").html("");
    $("#assetForAssetInfoReport").html("");
    $("#assetInfoReportCard .error").hide();

    $("#assetInfoLoader").hide();
    $("input[name=assetInfoReport][value=-1]").prop("checked", true);
    if ($("#regionForAssetInfoReport_combobox").length) $("#regionForAssetInfoReport").combobox("destroy");
    if ($("#siteForAssetInfoReport_combobox").length) $("#siteForAssetInfoReport").combobox("destroy");
    if ($("#assetForAssetInfoReport_combobox").length) $("#assetForAssetInfoReport").combobox("destroy");

}

function bindSelectsByName(selectName, url, reqField, msg, addEmptyOption) {
    $(selectName).data('open', false);
    $(selectName).bind("click focus", function () {
        if ($(selectName).data('open') === false && $(selectName).html() === "") {
            $(selectName).data('open', true);

            if ($(reqField).val() == null && reqField !== undefined) {
                alert(msg);
                return;
            }
            var id = "";
            if (selectName.indexOf("Fuel") > -1) {
                id = (url.indexOf("Regions") > -1 && $("#cityForFuelReport").val() !== null ? $("#cityForFuelReport").val() : "");
                if (url.indexOf("Sites") > -1 && $("#regionForFuelReport").val() !== null) id = $("#regionForFuelReport").val();
                if (url.indexOf("Assets") > -1 && $("#siteForFuelReport").val() !== null) id = $("#siteForFuelReport").val();
            } else if (selectName.indexOf("AssetReport") > -1) {
                id = (url.indexOf("Regions") > -1 && $("#cityForAssetReport").val() !== null ? $("#cityForAssetReport").val() : "");
                if (url.indexOf("Sites") > -1 && $("#regionForAssetReport").val() !== null) id = $("#regionForAssetReport").val();
                if (url.indexOf("Assets") > -1 && $("#siteForAssetReport").val() !== null) id = $("#siteForAssetReport").val();
            } else if (selectName.indexOf("Monthwise") > -1) {
                id = (url.indexOf("Regions") > -1 && $("#cityForMonthwiseStats").val() !== null ? $("#cityForMonthwiseStats").val() : "");
                if (url.indexOf("Sites") > -1 && $("#regionForMonthwiseStats").val() !== null) id = $("#regionForMonthwiseStats").val();
                if (url.indexOf("Assets") > -1 && $("#siteForMonthwiseStats").val() !== null) id = $("#siteForMonthwiseStats").val();
            } else if (selectName.indexOf("AssetInfo") > -1) {
                id = (url.indexOf("Sites") >= -1 && $("#regionForAssetInfoReport").val() !== null ? $("#regionForAssetInfoReport").val() : "");
                if (url.indexOf("Assets") > -1 && $("#siteForAssetInfoReport").val() !== null) id = $("#siteForAssetInfoReport").val();
            }
            $.ajax({
                url: url + id,
                type: "GET",
                contentType: "application/json charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var options;
                    if (addEmptyOption) options = "<option value=>Choose</option>";
                    $.each(data, function (index, value) {
                        if (selectName.indexOf("#siteFor") > -1)
                            options += "<option value={0}>{0} - {1}</option>".format(value.Id, value.Name);
                        else
                            options += "<option value={0}>{1}</option>".format(value.Id, value.Name);
                    });
                    $(selectName).html(options);
                    $(selectName).combobox();
                }
            });
        } else $(selectName).data('open', false);
    });
}

function setZoomControl(obj) {
    var lvl = parseInt(map.getZoom());
    if (obj == "In") map.setZoom(lvl + 1); else map.setZoom(lvl - 1);
}

function mapPan() {
    drawingManager.setDrawingMode(null);
}
function rectangleByZoom() {
    var myKeyDragZoom = map.getDragZoomObject();
    myKeyDragZoom.buttonDiv_.onclick(document.createEvent('MouseEvent'));
}

function logout() {
    window.location.href = urlLogout;
}

function setHome() {
    map.setZoom(zoom);
    map.setCenter(karachi);
}

function ampleViewButtonFormatter(cellvalue, options, rowObject) {
    return "<a href='#' id='showAmpleView8' title='Ample View' onclick=showAmpleView(this," + options.rowId + ")><img src='/Images/common/binoculars-32X16.jpg'></a>";
}

function loadDashboardStationaryGrid() {
    $.ajax({
        url: urlSelectedOrganizationIsBank,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function(data) {
            isBank = data;

            var stationaries = null;
            var colModel = null;
            var colNames = null;
            var orderFields = null;
            if (isBank) {
                colModel = [
                    { name: "Id", index: "Id", key: true, hidden: true, sorttype: "int" },
                    { name: "RegionName", index: "RegionName", width: 80, align: "center", sorttype: "string" },
                    { name: "CityId", index: "city.CityName", width: 80, align: "center", sorttype: "string" },
                    { name: "Title", index: "Title", width: 80, align: "center", sorttype: "string" },
                    { name: "SiteName", index: "SiteName", align: "left", sorttype: "string" },
                    { name: "AssetName", index: "AssetName", align: "left", sorttype: "string" },
                    //{ name: "Itemtype", index: "Itemtype", width: 80, align: "center", sorttype: "string" },
                    { name: "Capacity", index: "Capacity", width: 60, template: integerTemplate, align: "center" },
                    //{ name: "Basevolume", index: "Basevolume", width: 90, template: integerTemplate },
                    { name: "MinLevel", index: "MinLevel", width: 90, template: integerTemplate, align: "center" },
                    { name: "Currents", index: "Currents", width: 80, template: numberTemplate, align: "center", searchoptions: { sopt: ["le", "eq", "ne", "lt", "gt", "ge", "nu", "nn"]} },
                    { name: "PerLevel", index: "PerLevel", width: 80, formatter: "number", align: "center", sorttype: "number", searchtype: "number", 
                        searchrules: { required: false, integer: true }, searchoptions: { sopt: ["le"]} },
                    { name: "DgStatus", index: "DgStatus", width: 100, align: "center", sorttype: "string", stype: "select",
                        searchoptions: { sopt: ["eq", "ne"], value: ":Any;Genset-ON:Genset-ON;Genset Off:Genset Off;Fuel Theft:Fuel Theft" } },
                    { name: "Totduration", index: "Totduration", width: 80, align: "center", sorttype: "string", search:false },
                    { name: "GridStatus", index: "GridStatus", width: 80, align: "center", sorttype: "string", stype: "select",
                        searchoptions: { sopt: ["eq", "ne"], value: ":Any;1:ON;0:Off" } },
                    { name: "GridDuration", index: "GridDuration", width: 80, align: "center", sorttype: "string", search:false },
                    { name: 'Frtcdttm', index: 'Frtcdttm', width: 150, align: 'center', template: dateTimeTemplate },
                    { name: "Status", index: "Status", width: 40, align: "left", sorttype: "string", search:false, formatter:statusFmatter },
                    { name: "AssetNo", index: "AssetNo", hidden: true, sorttype: "int" },
                    {
                        name: 'AmpleView',
                        index: 'AmpleView',
                        width: 40,
                        align: "center",
                        resizable: false,
                        search: false,
                        sortable: false,
                        formatter: ampleViewButtonFormatter
                    },
                    { name: "MinimumFuel", index: "MinimumFuel", hidden: true },
                    { name: "SiteCode", index: "SiteCode", hidden: true }
                ];
                colNames = [
                    "Unit Id", "Region", "City", "Branch Code", "Branch Name", "Asset Name", "Capacity (Ltrs)", "Min Fuel (Ltrs)",
                    "Current Fuel (Ltrs)", "Fuel Level (%)", "DG Status", "DG Duration", "Grid Status", "Grid Duration", 'Last Message Date', 'Status', 'Asset', 
                    'Ample View', 'Fuel Critically Low', 'Site Code'
                ];
                orderFields = "RegionName,Title, SiteName, AssetName";
            } else {
                colModel = [
                    { name: "Id", index: "Id", key: true, hidden: true, sorttype: "int" },
                    { name: "RegionId", index: "region.Regionname", width: 80, align: "left", sorttype: "string" },
                    { name: "Cityid", index: "city.CityName", width: 80, align: "center", sorttype: "string" },
                    { name: "SiteName", index: "SiteName", align: "left", sorttype: "string" },
                    { name: "AssetName", index: "AssetName", align: "left", sorttype: "string" },
                    { name: "Itemtype", index: "Itemtype", width: 80, align: "center", sorttype: "string" },
                    { name: "Capacity", index: "Capacity", width: 60, template: integerTemplate },
                    { name: "Basevolume", index: "Basevolume", width: 90, template: integerTemplate },
                    { name: "MinLevel", index: "MinLevel", width: 90, template: integerTemplate },
                    { name: "Currents", index: "Currents", width: 80, template: numberTemplate },
                    { name: "Leveltype", index: "Leveltype", width: 80, align: "center", sorttype: "string" },
                    {
                        name: "Gridduration",
                        index: "edrFuelRun.EventType",
                        width: 100,
                        align: "left",
                        sorttype: "string",
                        stype: "select",
                        searchoptions: { sopt: ["eq", "ne"], value: ":Any;C:Genset-ON;N:Genset Off" }

                    },
                    { name: "Totduration", index: "Totduration", width: 80, align: "center", sorttype: "string", search:false },
                    { name: 'Frtcdttm', index: 'Frtcdttm', width: 150, align: 'center', template: dateTimeTemplate },
                    { name: "Status", index: "Status", width: 40, align: "left", sorttype: "string", search:false, formatter:statusFmatter },
                    {
                        name: 'AmpleView',
                        index: 'AmpleView',
                        width: 40,
                        align: "center",
                        resizable: false,
                        search: false,
                        sortable: false,
                        formatter: ampleViewButtonFormatter
                    }
                ];
                colNames = [
                    "Unit Id", "Region", "City", "Site Name", "Asset Name", "Asset Type",
                    "Capacity (Ltrs)", "Base Volume (Ltrs)", "Min Fuel Level (Ltrs)",
                    "Current Fuel (Ltrs)", "Fuel Level", "Current Status", "Duration", 'Last Message Date', 'Status', 'Ample View'
                ];
                orderFields = "RegionId,SiteName,AssetName";
            }
            loadjqGrid(
                [], "listDashboardStationary", urlGetDashboardStationariesForMap, {
                    isBank: isBank
                },
                colNames, colModel,
                "pagerDashboardStationary", orderFields, "asc", "", 10, _rowList, 1, 1, "Dashboard Stationary",
                {
                    root: function(obj) {
                        stationaries = obj.rows;
                        return obj.rows;
                    },
                    page: "page",
                    total: "total",
                    records: "records",
                    repeatitems: false,
                    userdata: "userdata"
                },
                undefined, undefined, false, false, function() {
                    $grid.setGridWidth($(window).width());
                    $grid.setGridHeight(230); //$(window).height() - ($(window).height()/1.5));
                    //$(".ui-jqgrid-active").css({ bottom: 0, position: fixed, left: 0 });
                    $("#" + $grid.attr("aria-labelledby")).show();

                    $grid.jqGrid("gridResize", { disabled: true });
                    $("#gbox_listDashboardStationary").draggable({ disabled: true });
                    //$grid[0].toggleToolbar();
                    deleteOverlays();//delete both stationaries and boundries first
                    drawStationariesOnMap(stationaries);
                    drawCityBoundriesOnMap(stationaries);
                    //isBank = $grid.getGridParam("userData")["isBank"];
                    if ($("[type=checkbox][name=minFuel]")[0] == undefined && isBank)
                        $('#gbox_' + $grid[0].id + ' .ui-jqgrid-title').before(
                            $("<div class='ui-jqgrid-titlebar-close HeaderButton' style=right:90px;width:200px><label><input type='checkbox' name='minFuel' onclick='setMinimumFuel(this.checked);'>Fuel Critically Low</label></div>"));


                    ////$("#divAmpleView").parent().children(".ui-dialog-titlebar").children(".ui-dialog-title").css({ width: '20%' });
                    //$('#gbox_' + $grid[0].id + ' .ui-jqgrid-title').before(
                    //    $("<div class='ui-jqgrid-titlebar-close HeaderButton' style=float:right;width:200px;font-weight:normal><input type='text' id='txtTimeForAV' value='60' style='width: 30px' onblur='setAutoRefreshTimeForAV();' />(sec)<label><input type='checkbox' name='autoRefresh' id='chkAutoRefreshForAV' onclick='setAutoRefreshTimeForAV();'>Auto Refresh</label></div>"));


                }, undefined, undefined, undefined, undefined, undefined, true, undefined, undefined, undefined, undefined,
                undefined, undefined, undefined, undefined, undefined, undefined, undefined, true,
                function(rd) {
                    if (isBank) {
                        if (rd.MinLevel > rd.Currents) 
                            return { "style": "color:red;font-weight: bold;" };
                        else if (rd.DgStatus === "Genset-ON") // verify that the testing is correct in your case
                            return { "style": "color:green;font-weight: bold;" };
                    }
                    else
                    if (rd.Gridduration === "Genset-ON") // verify that the testing is correct in your case
                        return { "style": "color:green;font-weight: bold;" };
                    return {};
                }, true);
        }
    });
}

function statusFmatter (cellvalue, options, rowObject)
{
    var today = new Date($("#listDashboardStationary").getGridParam('userData').Now);
    var openDate = new Date(rowObject.OpenDt);
    var diff = new Date(today - openDate);
    var hours = diff/1000/60/60/24;
    return (hours > 24 ? "Down" : "Live");
}

function setMinimumFuel(isMin) {

    $("#listDashboardStationary").setGridParam({ postData: { filters: '{"groupOp":"AND","rules":[{"field":"MinimumFuel","op":"eq","data":"' +isMin+ '"}]}'} });
    $("#listDashboardStationary").trigger("reloadGrid");
}

function loadPage() {
    loadDashboardStationaryGrid();
}

function showAmpleView(obj, unitId, month, year) {
    month = month || "";
    year = year || "";
    _unitIdForAmpleView = unitId;

    $.ajax({
        url: urlGetMultipleDatasetsForAmpleViewByUnitId + "?unitId=" + unitId + "&month=" + month + "&year" + year,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        beforeSend: function() { $(obj).parent().showLoaderPanel("horz-big"); },
        complete: function() { $(obj).parent().hideLoaderPanel(); },
        success: function(data, textStatus, xhr) {
            if (textStatus === "success") {
                var siteCaption = isBank ? "Branch" : "Site";
                var statusCaption = isBank ? "Branch" : "Bank";
                if (isBank) $("#lblInformationType").html(" Branch Information ");
                if (data.site != null) {
                    $("#siteInfo").html(
                        "<table width='100%' class='ampleviewtable ampleviewtableborder'><tr>"
                        + "{0}{1}{2}{3}".format("<td class='ampleviewcelllabel'>", siteCaption + (isBank ? " Code" : " Id"), "</td><td class='ampleviewcelltext'>", data.site.Title, "</td>")
                        + "{0}{1}{2}{3}".format("<td class='ampleviewcelllabel'>", siteCaption, " Name</td><td class='ampleviewcelltext'>", data.site.SiteName, "</td>")
                        + "{0}{1}{2}{3}".format("<td class='ampleviewcelllabel'>", isBank ? "Branch Type" : "Site Category", "</td><td class='ampleviewcelltext'>", data.site.SiteType, "</td>")
                        + "</tr><tr>"
                        + (isBank ? "" : "{0}{1}{2}{3}".format("<td class='ampleviewcelllabel'>", siteCaption, " Type</td><td class='ampleviewcelltext'>", data.site.SiteOiduType, "</td>"))
                        + "{0}{1}{2}{3}".format("<td class='ampleviewcelllabel'>", siteCaption, " Priority</td><td class='ampleviewcelltext'>", data.site.Priority, "</td>")
                        + "{0}{1}{2}{3}".format("<td class='ampleviewcelllabel'>", isBank ? "Branch Category" : "Site Status", "</td><td class='ampleviewcelltext'>", data.site.SiteCat, "</td>")
                        + (isBank ? "<td></td><td></td>" : "")
                        + "</tr><tr>"
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'><div>Address<a style='border:none; display:inline-block' href='#' onclick='openContactInfoWindow(true)'> <img src='/Images/common/Male-user-add-icon.png'/></a></div></td><td class='ampleviewcelltext'>", data.site.Address, "</td>")
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Location</td><td class='ampleviewcelltext'>", data.site.BlockCode, "</td>")
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Area</td><td class='ampleviewcelltext'>", data.site.AreaId, "</td>")
                        + "</tr><tr>"
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>City</td><td class='ampleviewcelltext'>", data.site.CityId, "</td>")
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Region</td><td class='ampleviewcelltext'>", data.site.RegionId, "</td>")
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Cluster/Territory</td><td class='ampleviewcelltext'>", (data.site.TerritoryName == null ? '' : data.site.TerritoryName), "</td>")
                        + "</tr></table>"
                    );
                    _siteNameForAmpleViewDetail = data.site.SiteName;
                }
                var compRunHr = 0;
                if (data.monthWiseSummary != null)
                    if (data.monthWiseSummary.CompRunHr !== null) compRunHr = data.monthWiseSummary.CompRunHr;

                if (data.asset != null)
                    $("#assetInfo").html(
                        "<table width='100%' class='ampleviewtable ampleviewtableborder'><tr>"
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Asset Name</td><td colspan=5 class='ampleviewcelltext'>", data.asset.AssetName, "</td>")
                        + "</tr><tr>"
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Assset Group</td><td class='ampleviewcelltext'>", data.asset.AssetGroupName, "</td>")
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Serial #</td><td class='ampleviewcelltext'>", data.asset.AssetSNo, "</td>")
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Asset # / Inventory #</td><td class='ampleviewcelltext'>", data.asset.inventoryNo, "</td>")
                        + "</tr><tr>"
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Engine Make</td><td class='ampleviewcelltext'>", (data.asset.EngineUMake == null ? '' : data.asset.EngineUMake), "</td>")
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Alternator Make</td><td class='ampleviewcelltext'>", data.asset.AlternatorMake, "</td>")
                        + "{0}{1} <b>(KVA)</b>{2}".format("<td class='ampleviewcelllabel'>Rating</td><td class='ampleviewcelltext'>", data.asset.Dgcap, "</td>")
                        + "</tr><tr>"
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>No Of Fuel Tank(s)</td><td class='ampleviewcelltext'>", data.asset.NoOfTanks, "</td>")
                        + "{0}{1} <b>(Ltrs)</b>{2}".format("<td class='ampleviewcelllabel'>Tank Cap [ Stated ]</td><td class='ampleviewcelltext'>", data.asset.FuelCapacity, "</td>")
                        + "{0}{1} <b>(Ltrs)</b>{2}".format("<td class='ampleviewcelllabel'>Tank Cap [ Calibrated ]</td><td class='ampleviewcelltext'>", data.asset.ColCapacity, "</td>")
                        + "</tr><tr>"
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Installation Date</td><td class='ampleviewcelltext'>", (data.asset.InstalYear == null ? '' : new Date(data.asset.InstalYear).toLocaleDateString("en-gb")), "</td>")
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Condition</td><td class='ampleviewcelltext'>", data.asset.ATConditionName, "</td>")
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>FuelGEX Active Date</td><td class='ampleviewcelltext'>", (data.asset.StartDate == null ? '' : new Date(data.asset.StartDate).toLocaleDateString("en-gb")), "</td>")
                        + "</tr><tr>"
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Status</td><td class='ampleviewcelltext'>", (data.asset.AssetStatusName == null ? '' : data.asset.AssetStatusName), "</td>")
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Battery Volts</td><td class='ampleviewcelltext'>", data.currentLevel.AI1, "</td>")
                        + "{0}{1}{2}".format("<td class='ampleviewcelllabel'>Total Run Hrs</td><td class='ampleviewcelltext'>", compRunHr, "</td>")
                        + "</tr></table>"
                    );

                var contractInfoHtml = "<table width='100%' class='ampleviewtable ampleviewtableborder'><tr><th>Project</th><th>Title</th><th>Contract Type</th><th>Contractor</th><th>Start Date</th><th>Expiry Date</th></tr>";
                if (data.contractMsts.length > 0) {
                    
                    $.each(data.contractMsts, function(index, value) {
                        contractInfoHtml += "<tr><td>" + value.MasterContract + "</td><td>" + value.Title + "</td><td>" + value.ContrType + "</td><td>" + value.ToVendorCode + "</td><td>" + (value.DateFrom == null ? '' : new Date(value.DateFrom).toLocaleDateString("en-gb")) + "</td><td>" + (value.DateTo == null ? '' : new Date(value.DateTo).toLocaleDateString("en-gb")) + "</td></tr>";
                    });
                } else 
                    contractInfoHtml += "<tr>{0}{0}{0}{0}{0}{0}</tr><tr>{0}{0}{0}{0}{0}{0}</tr>".format("<td>N/A</td>");

                contractInfoHtml += "</table>";
                $("#contractInfo").html(contractInfoHtml);

                var fuelInfoHtml = "<table width='100%' class='ampleviewtable ampleviewtableborder'><tr><th>Event #</th><th style='width:150px;'>Status</th><th>Event Start</th><th>Event Stop</th><th>Duration</th><th>Fuel Vol</th></tr>";
                if (data.currentLevel != null)
                    fuelInfoHtml += "<tr style='font-weight: bold;color: {1}'><td>{2}{0}{3}{0}{4}{0}{5}{0}{6}</td><td style='text-align:right; padding-right: 4px;'>{7}".format("</td><td>",
                    (data.currentLevel.EventTypeName == "Currently DG-ON" ? "green;" : "red"),
                    data.currentLevel.Id, 
                    data.currentLevel.EventTypeName, 
                    (data.currentLevel.OpenDt == null ? '' : formatDateTime(new Date(data.currentLevel.OpenDt))),
                    (data.currentLevel.FRTCDTTM == null ? '' : formatDateTime(new Date(data.currentLevel.FRTCDTTM))),
                     data.currentLevel.TotDuration,
                     parseFloat(data.currentLevel.BalanceQty).toFixed(2), "</td></tr>");
                    //fuelInfoHtml += "<tr style='font-weight: bold;color: " + (data.currentLevel.EventTypeName == "Currently DG-ON" ? "green;" : "red") + "'><td>" + data.currentLevel.Id + "</td><td>" + data.currentLevel.EventTypeName + "</td><td>" + (data.currentLevel.OpenDt == null ? '' : formatDateTime(new Date(data.currentLevel.OpenDt))) + "</td><td>" + (data.currentLevel.FRTCDTTM == null ? '' : formatDateTime(new Date(data.currentLevel.FRTCDTTM))) + "</td><td>" + data.currentLevel.TotDuration + "</td><td style='text-align:right; padding-right: 4px;'>" + parseFloat(data.currentLevel.BalanceQty).toFixed(2) + "</td></tr>";
                else
                    fuelInfoHtml += "<tr style='font-weight: bold;'><td></td><td>Currently DG</td><td>N/A</td><td>N/A</td><td>N/A</td><td style='text-align:right; padding-right: 4px;'>0.0</td></tr>";
                if (data.currentGrid != null)
                    fuelInfoHtml += "<tr style='font-weight: bold;color: {7}'><td>{1}{0}{2}{0}{3}{0}{4}{0}{5}</td><td style='text-align:right; padding-right: 4px;'>{6}</td></tr>".format("</td><td>",
                    data.currentGrid.Id, data.currentGrid.EventTypeName,
                    (data.currentGrid.OpenDt == null ? '' : formatDateTime(new Date(data.currentGrid.OpenDt))),
                    (data.currentGrid.FRTCDTTM == null ? '' : formatDateTime(new Date(data.currentGrid.FRTCDTTM))),
                    data.currentGrid.TotDuration, parseFloat(data.currentGrid.BalanceQty).toFixed(2),
                    (data.currentGrid.EventTypeName === "Currently GRID-ON" ? "green;" : "red"));
                else
                    fuelInfoHtml += "<tr><td>N/A</td><td>Currently Grid</td><td>N/A</td><td>N/A</td><td>N/A</td><td style='text-align:right; padding-right: 4px;'>0.0</td></tr>";

                if (data.lastConsume != null)
                    fuelInfoHtml += "<tr><td>{1}{0}{2}{0}{3}{0}{4}{0}{5}</td><td style='text-align:right; padding-right: 4px;'>{6}</td></tr>".format("</td><td>", 
                    data.lastConsume.Id, data.lastConsume.EventTypeName, 
                    (data.lastConsume.OpenDt == null ? '' : formatDateTime(new Date(data.lastConsume.OpenDt))),
                    (data.lastConsume.FRTCDTTM == null ? '' : formatDateTime(new Date(data.lastConsume.FRTCDTTM))),
                    data.lastConsume.TotDuration, parseFloat(data.lastConsume.BalanceQty).toFixed(2));
                else
                    fuelInfoHtml += "<tr><td>N/A</td><td>Last DG</td><td>N/A</td><td>N/A</td><td>N/A</td><td style='text-align:right; padding-right: 4px;'>0.0</td></tr>";

                if (data.lastGrid != null)
                    fuelInfoHtml += "<tr><td>{1}{0}{2}{0}{3}{0}{4}{0}{5}</td><td style='text-align:right; padding-right: 4px;'>{6}</td></tr>".format("</td><td>",
                    data.lastGrid.Id, data.lastGrid.EventTypeName,
                    (data.lastGrid.OpenDt == null ? '' : formatDateTime(new Date(data.lastGrid.OpenDt))),
                    (data.lastGrid.FRTCDTTM == null ? '' : formatDateTime(new Date(data.lastGrid.FRTCDTTM))),
                    data.lastGrid.TotDuration, parseFloat(data.lastGrid.BalanceQty).toFixed(2));
                else
                    fuelInfoHtml += "<tr><td>N/A</td><td>Last Grid</td><td>N/A</td><td>N/A</td><td>N/A</td><td style='text-align:right; padding-right: 4px;'>0.0</td></tr>";

                if (data.lastRefuel != null)
                    fuelInfoHtml += "<tr><td>{1}{0}{2}{0}{3}{0}{4}{0}{5}</td><td style='text-align:right; padding-right: 4px;'>{6}</td></tr>".format("</td><td>", 
                    data.lastRefuel.Id, data.lastRefuel.EventTypeName,
                    (data.lastRefuel.OpenDt == null ? '' : formatDateTime(new Date(data.lastRefuel.OpenDt))),
                    (data.lastRefuel.FRTCDTTM == null ? '' : formatDateTime(new Date(data.lastRefuel.FRTCDTTM))),
                    data.lastRefuel.TotDuration, parseFloat(data.lastRefuel.BalanceQty).toFixed(2));
                else
                    fuelInfoHtml += "<tr><td>N/A</td><td>Last Refuel</td><td>N/A</td><td>N/A</td><td>N/A</td><td style='text-align:right; padding-right: 4px;'>0.0</td></tr>";

                if (data.lastTheft != null)
                    fuelInfoHtml += "<tr><td>{1}</td><td class='ui-state-error ui-corner-all'>{2}<span class='ui-icon ui-icon-alert' style='float: right; margin-right: .3em;'></span>{0}{3}{0}{4}{0}{5}</td><td style='text-align:right; padding-right: 4px;'>{6}</td></tr>".format("</td><td>",
                    data.lastTheft.Id, data.lastTheft.EventTypeName,
                    (data.lastTheft.OpenDt == null ? '' : formatDateTime(new Date(data.lastTheft.OpenDt))),
                    (data.lastTheft.FRTCDTTM == null ? '' : formatDateTime(new Date(data.lastTheft.FRTCDTTM))),
                    data.lastTheft.TotDuration, parseFloat(data.lastTheft.BalanceQty).toFixed(2));
                else
                    fuelInfoHtml += "<tr><td>N/A</td><td>Last Theft</td><td>N/A</td><td>N/A</td><td>N/A</td><td style='text-align:right; padding-right: 4px;'>0.0</td></tr>";
                //(data.Theft !== 0 ? "" : ""), (data.Theft !== 0 ? '' : ""),
                fuelInfoHtml += "</table>";
                $("#fuelInfo").html(fuelInfoHtml);

                getMonthWiseHtml(data.monthWiseSummary, month, year);
                _fuelRateForAmpleView = data.fuelPrice;
                setStatisticsFieldValue(data.todayStatistics);
                setSensorBoardHtml(data.sensorInfo, data.sensorInfoVoltage);

                var unitData = $('#listDashboardStationary').getRowData(unitId);
                var capacity = parseFloat(unitData.Capacity);
                var minLevel = parseFloat(unitData.MinLevel);
                var currents = parseFloat(unitData.Currents);
                capacity = (Math.floor(capacity * 0.01) * 100) + 100;
                $("#guageInfo").jqxLinearGauge({
                    orientation: 'vertical',
                    min: 0,
                    max: capacity,
                    //showRanges: false,
                    ranges: [
                        { startValue: 0, endValue: minLevel, style: { fill: '#FF0000', stroke: '#FF0000' }, endWidth: 5, startWidth: 1 }
                        , { startValue: minLevel, endValue: parseFloat(unitData.Capacity), style: { fill: '#61b314', stroke: '#61b314' }, endWidth: 10, startWidth: 5 }
                    ],
                    ticksMajor: { size: '15%', interval: (capacity/4) },
                    ticksMinor: { interval: (capacity/8), size: '10%', style: { 'stroke-width': 1, stroke: '#aaaaaa'} },
                    labels: { interval: Math.round(capacity/4), position: 'near', offset: 0 },
                    background: { visible: false, backgroundType: 'rectangle' },
                    pointer: { size: '5%', pointerType: 'default', offset: 2 }, //pointerType: 'arrow'
                    //scaleLength: '93%',
                    rangeSize: [3,0],//ticksOffset: [0, 0], //thinner red-green line
                    //rangesOffset: -3, //to move red-green line little bit left side
                    width: '95%',
                    height: 200,
                    colorScheme: 'scheme01',// + ((minLevel > currents) + 1) * 2,
                    animationDuration: 1000,
                    ticksPosition: 'both'
                });
                if (data.currentGrid.FRTCDTTM != null)
                    $("#lblLastMsgDate").html(formatDateTime(new Date(data.currentGrid.FRTCDTTM)));
                $("#lblCurrentFuel").html(currents);
                $("#lblMinLevel").html(minLevel);
                $("#lblFullCapacity").html(parseFloat(unitData.Capacity));
                $("#guageInfo").jqxLinearGauge('value', currents);

                $('#monthsForSummary').val((new Date()).getMonth() + 1);
                $("#divAmpleView").parent().css({ opacity: 1 });
                $("#divAmpleView").dialog("open");
            }
        }
    });
}

function openContactInfoWindow(toOpen) {
    $.ajax({
        url: urlGetSiteDirectoriesBySiteCode + "?SiteCode=" + $("#listDashboardStationary").getCell( $("#listDashboardStationary").getGridParam( 'selrow'), 'SiteCode'),
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function(data, textStatus, xhr) {
            if (textStatus === "success") {
                var tableHtml = "<table class='ampleviewtable ampleviewtableborder'><tr><th style='display:none'>Id</th><th>Name</th><th>Designation</th><th>Cell</th><th style='display:none' /></tr>";
                $(data).each(function(i) {
                    tableHtml += "<tr onclick='showContactInfoDetials(this)'><td style='display:none'>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td style='display:none'>{4}</td><td style='display:none'>{5}</td><td style='display:none'>{6}</td><td style='display:none'>{7}</td><td style='display:none'>{8}</td></tr>".format(data[i].Id, data[i].Contname, data[i].Desig, data[i].Cell, data[i].Directphone, data[i].Extention, data[i].Email, data[i].Salutation, data[i].Status);
                });
                
                tableHtml += "</table>";
                if (data.length > 0) $("#listContactInfo").show(); else $("#listContactInfo").hide();

                $("#listContactInfo").html(tableHtml);
                if (toOpen) {
                    $("#contactInfo").dialog("open");
                    $("#contactInfo .error").hide();
                }
            }
        }
    });
}

function showContactInfoDetials(tableCol) {
    $("#txtContactInfoId").val($($(tableCol).children()[0]).html());
    $("#txtContactInfoName").val($($(tableCol).children()[1]).html());
    $("#txtContactInfoDesignation").val($($(tableCol).children()[2]).html());
    $("#txtContactInfoCell").val($($(tableCol).children()[3]).html());
    $("#txtContactInfoPhone").val($($(tableCol).children()[4]).html());
    $("#txtContactInfoExtension").val($($(tableCol).children()[5]).html());
    $("#txtContactInfoEmail").val($($(tableCol).children()[6]).html());
    $("#txtContactInfoSalutation").val($($(tableCol).children()[7]).html());
    $("#txtContactInfoStatus").val($($(tableCol).children()[8]).html());
}

function saveContactInfo() {
    $("#contactInfo .error").css("color", "red");
    if ($("#txtContactInfoId").val()) {
        $.ajax({
            url: urlPutSiteDirectory,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify({
                Id : $("#txtContactInfoId").val(),
                SiteCode: $("#listDashboardStationary").getCell($("#listDashboardStationary").getGridParam('selrow'), 'SiteCode'),
                Contname: $("#txtContactInfoName").val(),
                Desig: $("#txtContactInfoDesignation").val(),
                Cell: $("#txtContactInfoCell").val(),
                Directphone: $("#txtContactInfoPhone").val(),
                Extention: $("#txtContactInfoExtension").val(),
                Email: $("#txtContactInfoEmail").val(),
                Salutation: $("#txtContactInfoSalutation").val(),
                Status: $("#txtContactInfoStatus").val()
            }),
            dataType: 'json',
            complete: function(jqXHR) {
                if (jqXHR.readyState === 4) {
                    $("#contactInfo .error span").html("The contact have been modified successfully.");
                    $("#contactInfo .error").css("color", "green");
                    cancelContactInfo();
                    openContactInfoWindow(false);
                } else {
                    $("#contactInfo .error span").html("While saving contact, Error occured.");
                }
                $("#contactInfo .error").show();
            }
        });
    } else {
        $.post(urlPostSiteDirectory, {
            SiteCode: $("#listDashboardStationary").getCell($("#listDashboardStationary").getGridParam('selrow'), 'SiteCode'),
            Contname: $("#txtContactInfoName").val(),
            Desig: $("#txtContactInfoDesignation").val(),
            Cell: $("#txtContactInfoCell").val(),
            Directphone: $("#txtContactInfoPhone").val(),
            Extention: $("#txtContactInfoExtension").val(),
            Email: $("#txtContactInfoEmail").val(),
            Salutation: $("#txtContactInfoSalutation").val(),
            Status: $("#txtContactInfoStatus").val()
        }, function(data) {
            $("#contactInfo .error span").html("The contact have been saved successfully.");
            $("#contactInfo .error").css("color", "green");
            $("#contactInfo .error").show();
            cancelContactInfo();
            openContactInfoWindow(false);
        });
    }
}

function cancelContactInfo() {
    $("#txtContactInfoId").val("");
    $("#txtContactInfoName").val("");
    $("#txtContactInfoDesignation").val("");
    $("#txtContactInfoCell").val("");
    $("#txtContactInfoPhone").val("");
    $("#txtContactInfoExtension").val("");
    $("#txtContactInfoEmail").val("");
    $("#txtContactInfoSalutation").val("");
    $("#txtContactInfoStatus").val("");
}

var today;
function openContractMstWindow(toOpen) {
    $.ajax({
        url: urlGetContractMsts + "?SiteCode=" + $("#listDashboardStationary").getCell( $("#listDashboardStationary").getGridParam( 'selrow'), 'SiteCode'),
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function(data, textStatus, xhr) {
            if (textStatus === "success") {
                var contractMsts = data.contractMsts;
                today = new Date(data.Today);
                var tableHtml = "<table class='ampleviewtable ampleviewtableborder'><tr><th style='display:none'>Id</th><th>Code</th><th>Cont Date</th><th>Title</th><th style='display:none' /></tr>";
                $(contractMsts).each(function(i) {
                    tableHtml += "<tr onclick='showContractMst(this)'><td>{0}</td><td>{1}</td><td style='display:none'>{2}</td><td style='display:none'>{3}</td><td style='display:none'>{4}</td><td>{5}</td><td style='display:none'>{6}</td><td style='display:none'>{7}</td><td style='display:none'>{8}</td><td style='display:none'>{9}</td></td><td style='display:none'>{10}</td></td><td style='display:none'>{11}</td></td><td style='display:none'>{12}</td></td><td style='display:none'>{13}</td></tr>"
                        .format(contractMsts[i].Id, new Date(contractMsts[i].ContractDate).toLocaleDateString("en-gb"), contractMsts[i].ContrType, contractMsts[i].MasterContractNo, contractMsts[i].ContDscr, contractMsts[i].Title, new Date(contractMsts[i].DateFrom).toLocaleDateString("en-gb"), new Date(contractMsts[i].DateTo).toLocaleDateString("en-gb"), contractMsts[i].VendorCode, contractMsts[i].ToVendorCode, contractMsts[i].CompanyCode, contractMsts[i].ContrStatusNo, contractMsts[i].ExpiryAlertB4, new Date(contractMsts[i].ExpiryAlertDt).toLocaleDateString("en-gb"));
                });
                
                tableHtml += "</table>";
                if (contractMsts.length > 0) $("#listContractManagement").show(); else $("#listContractManagement").hide();

                $("#listContractManagement").html(tableHtml);
                if (toOpen) {
                    $("#contractManagement").dialog("open");
                    $("#contractManagement .error").hide();
                }
                if (contractMsts.length > 0) {
                    var options = "<option value=>Choose</option>";
                    $.each(contractMsts, function(i, v) {
                        options += "<option value={0}>{1}</option>".format(v.Id, v.Title);
                    });
                    $("#txtContractManagementMasterContract").html(options);

                    $("#txtContractManagementContractType").click();
                    $("#txtContractManagementVendorCode").click();
                    $("#txtContractManagementToVendorCode").click();
                    $("#txtContractManagementContrStatus").click();
                    $("#txtContractManagementCompCode").click();
                }
            }
        }
    });
}

function showContractMst(tableCol) {
    $("#txtContractManagementProject").val($($(tableCol).children()[0]).html());
    $("#txtContractManagementContractDate").val($($(tableCol).children()[1]).html());
    $("#txtContractManagementContractType").val($($(tableCol).children()[2]).html());
    $("#txtContractManagementMasterContract").val($($(tableCol).children()[3]).html());
    $("#txtContractManagementDescription").val($($(tableCol).children()[4]).html());
    $("#txtContractManagementTitle").val($($(tableCol).children()[5]).html());
    $("#txtContractManagementDateFrom").val($($(tableCol).children()[6]).html());
    $("#txtContractManagementDateTo").val($($(tableCol).children()[7]).html());
    $("#txtContractManagementVendorCode").val($($(tableCol).children()[8]).html());
    $("#txtContractManagementToVendorCode").val($($(tableCol).children()[9]).html());
    $("#txtContractManagementCompCode").val($($(tableCol).children()[10]).html());
    $("#txtContractManagementContrStatus").val($($(tableCol).children()[11]).html());
    $("#txtContractManagementExpiryAlertB4").val($($(tableCol).children()[12]).html());
    $("#txtContractManagementExpiryAlertDate").val($($(tableCol).children()[13]).html());

    var alertDate = $('#txtContractManagementExpiryAlertDate').datepicker('getDate');
    $("#txtContractManagementRemainingDays").val( -(today - alertDate)/1000/60/60/24);

    $("#txtContractManagementMasterContract option").show();
    $("#txtContractManagementMasterContract option[value=" + $($(tableCol).children()[0]).html() + "]").hide();
}

function saveContractManagement() {
    var from = $("#txtContractManagementDateFrom").datepicker("getDate");
    var to = $("#txtContractManagementDateTo").datepicker("getDate");
    var dayBeforAlert = $("#txtContractManagementExpiryAlertB4").val();
    $("#contractManagement .error").css("color", "red");
    
    if (Date.parse($("#txtContractManagementContractDate").val()) === null) {
        $("#contractManagement .error span").html("Please input valid Contract Date.");
        $("#contractManagement .error").show();
        $("#txtContractManagementContractDate").focus();
        return; 
    }
    else if ($("#txtContractManagementContractType").val() === "") {
        $("#contractManagement .error span").html("Please select Contract Type.");
        $("#contractManagement .error").show();
        $("#txtContractManagementContractType").focus();
        return; 
    }
    else if ($("#txtContractManagementToVendorCode").val() === "") {
        $("#contractManagement .error span").html("Please select Outsourcing Partner.");
        $("#contractManagement .error").show();
        $("#txtContractManagementToVendorCode").focus();
        return; 
    }
    else if ($("#txtContractManagementCompCode").val() === "") {
        $("#contractManagement .error span").html("Please select Client.");
        $("#contractManagement .error").show();
        $("#txtContractManagementCompCode").focus();
        return; 
    }
    else if (dayBeforAlert === "" || isNaN(dayBeforAlert)) {
        $("#contractManagement .error span").html("Please Enter The Day Befor Alert.");
        $("#contractManagement .error").show();
        $("#txtContractManagementExpiryAlertB4").focus();
        return;
    }
    else if (from === null) {
        $("#contractManagement .error span").html("From Date must be required.");
        $("#contractManagement .error").show();
        $("#txtContractManagementDateFrom").focus();
        return;
    }
    else if (to === null) {
        $("#contractManagement .error span").html("To Date must be required.");
        $("#contractManagement .error").show();
        $("#txtContractManagementDateTo").focus();
        return;
    }
    else if (to - from <= 0) {
        $("#contractManagement .error span").html("To Date must be grater then to from Date.");
        $("#contractManagement .error").show();
        $("#txtContractManagementDateFrom").focus();
        return;
    }
    else if (to - from <= dayBeforAlert * 86400 * 1000) {
        $("#contractManagement .error span").html("To Date must be atleast "+ dayBeforAlert +" days grater then to from Date.");
        $("#contractManagement .error").show();
        $("#txtContractManagementExpiryAlertB4").focus();
        return;
    }
    else if (Date.parse($("#txtContractManagementExpiryAlertDate").val()) === null) {
        $("#contractManagement .error span").html("Please input valid Alert Date.");
        $("#contractManagement .error").show();
        $("#txtContractManagementExpiryAlertDate").focus();
        return; 
    }
    else if ($("#txtContractManagementTitle").val() === "") {
        $("#contractManagement .error span").html("Please input Title.");
        $("#contractManagement .error").show();
        $("#txtContractManagementTitle").focus();
        return; 
    }
    if ($("#txtContractManagementProject").val()) {
        $.ajax({
            url: urlPutContractMst,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify({
                Id : $("#txtContractManagementProject").val(),
                ContractDate: $("#txtContractManagementContractDate").datepicker("getDate"),
                ContrType: $("#txtContractManagementContractType").val(),
                MasterContractNo: $("#txtContractManagementMasterContract").val(),
                Title: $("#txtContractManagementTitle").val(),
                ContrStatusNo: $("#txtContractManagementContrStatus").val(),
                ContDscr: $("#txtContractManagementDescription").val(),
                DateFrom: $("#txtContractManagementDateFrom").datepicker("getDate"),
                DateTo: $("#txtContractManagementDateTo").datepicker("getDate"),
                VendorCode: $("#txtContractManagementVendorCode").val(),
                ToVendorCode: $("#txtContractManagementToVendorCode").val(),
                CompanyCode: $("#txtContractManagementCompCode").val(),
                ExpiryAlertB4: $("#txtContractManagementExpiryAlertB4").val(),
                ExpiryAlertDt: $("#txtContractManagementExpiryAlertDate").datepicker("getDate")
            }),
            dataType: 'json',
            complete: function(jqXHR) {
                if (jqXHR.readyState === 4) {
                    $("#contractManagement .error span").html("The contact have been modified successfully.");
                    $("#contractManagement .error").css("color", "green");
                    cancelContractManagement();
                    openContractMstWindow(false);
                } else
                    $("#contractManagement .error span").html("While saving contact, Error occured.");
                $("#contractManagement .error").show();
            }
        });
    } else {
        $.post(urlPostContractMst, {
            Id : $("#txtContractManagementProject").val(),
            ContractDate: $("#txtContractManagementContractDate").datepicker("getDate"),
            ContrType: $("#txtContractManagementContractType").val(),
            MasterContractNo: $("#txtContractManagementMasterContract").val(),
            Title: $("#txtContractManagementTitle").val(),
            ContrStatusNo: $("#txtContractManagementContrStatus").val(),
            ContDscr: $("#txtContractManagementDescription").val(),
            DateFrom: $("#txtContractManagementDateFrom").datepicker("getDate"),
            DateTo: $("#txtContractManagementDateTo").datepicker("getDate"),
            VendorCode: $("#txtContractManagementVendorCode").val(),
            ToVendorCode: $("#txtContractManagementToVendorCode").val(),
            CompanyCode: $("#txtContractManagementCompCode").val(),
            ExpiryAlertB4: $("#txtContractManagementExpiryAlertB4").val(),
            ExpiryAlertDt: $("#txtContractManagementExpiryAlertDate").datepicker("getDate")
        }, function(data) {
            $("#contractManagement .error span").html("The contract have been saved successfully.");
            $("#contractManagement .error").css("color", "green");
            $("#contractManagement .error").show();
            cancelContractManagement();
            openContractMstWindow(false);
        });
    }
}

function cancelContractManagement() {
    //$("#txtContractManagementProject").prop('disabled', false);
    $("#txtContractManagementProject").val("");
    $("#txtContractManagementContractDate").datepicker("setDate", "");
    $("#txtContractManagementContractType").val("");
    $("#txtContractManagementMasterContract").val("");
    $("#txtContractManagementVendorCode").val("");
    $("#txtContractManagementToVendorCode").val("");
    $("#txtContractManagementContrStatus").val("");
    $("#txtContractManagementCompCode").val("");
    $("#txtContractManagementTitle").val("");
    $("#txtContractManagementDateFrom").datepicker("setDate", "");
    $("#txtContractManagementDateTo").datepicker("setDate", "");
    $("#txtContractManagementExpiryAlertB4").val("");
    $("#txtContractManagementExpiryAlertDate").datepicker("setDate", "");
    $("#txtContractManagementDescription").val("");
    $("#txtContractManagementRemainingDays").val("");
    $("#txtContractManagementMasterContract option").show();
}

function getContractAlertDay() {
    var from = $("#txtContractManagementDateFrom").datepicker("getDate");
    var to = $("#txtContractManagementDateTo").datepicker("getDate");
    var dayBeforAlert = $("#txtContractManagementExpiryAlertB4").val();
    $("#contractManagement .error").css("color", "red");
    if (dayBeforAlert === "" || isNaN(dayBeforAlert)) {
        $("#contractManagement .error span").html("Please Enter The Day Befor Alert.");
        $("#contractManagement .error").show();
        $("#txtContractManagementExpiryAlertB4").focus();
        return;
    }
    else if (from === null) {
        $("#contractManagement .error span").html("From Date must be required.");
        $("#contractManagement .error").show();
        $("#txtContractManagementDateFrom").focus();
        return;
    }
    else if (to === null) {
        $("#contractManagement .error span").html("To Date must be required.");
        $("#contractManagement .error").show();
        $("#txtContractManagementDateTo").focus();
        return;
    }
    else if (to - from <= 0) {
        $("#contractManagement .error span").html("To Date must be grater then to from Date.");
        $("#contractManagement .error").show();
        $("#txtContractManagementDateFrom").focus();
        return;
    }
    else if (to - from <= dayBeforAlert * 86400 * 1000) {
        $("#contractManagement .error span").html("To Date must be atleast "+ dayBeforAlert +" days grater then to from Date.");
        $("#contractManagement .error").show();
        $("#txtContractManagementExpiryAlertB4").focus();
        return;
    }
    var date2 = $('#txtContractManagementDateTo').datepicker('getDate'); 
    date2.setDate(date2.getDate()-dayBeforAlert); 
    $('#txtContractManagementExpiryAlertDate').datepicker('setDate', date2);
    $("#contractManagement .error").hide();

    var alertDate = $('#txtContractManagementExpiryAlertDate').datepicker('getDate');
    $("#txtContractManagementRemainingDays").val( -(today - alertDate)/1000/60/60/24);
}

function setVendorCode() {
    $("#txtContractManagementVendorCode").val($("#txtContractManagementToVendorCode").val());
}

function formatDateTime(date) {
    var dateTimeForCurrentTimeZone = new Date(date.valueOf() + date.getTimezoneOffset());// * 60000);
    return dateTimeForCurrentTimeZone.toLocaleDateString("en-gb") + " " + dateTimeForCurrentTimeZone.toLocaleTimeString("en-US", { hour: '2-digit', minute: '2-digit', hour12: true });
}

function roundSeconds(date) {

    date.setMinutes(date.getMinutes() + Math.round(date.getSeconds() / 60));
    date.getSeconds(0);

    return date;
}

function drawStationariesOnMap(data) {
    var bounds = new gMap.LatLngBounds();

    $(data).each(function(i) {
        var latLng;

        if (isBank)
            latLng = new gMap.LatLng(data[i].Lati, data[i].Longi);
        else latLng = new gMap.LatLng(data[i].Title, data[i].Gridact); 

        if (latLng.lat() !== 0 && latLng.lng() !== 0) {
            bounds.extend(latLng);
            var marker = new gMap.Marker({
                position: latLng,
                map: map,
                title: "({0})-({1})-({2})".format(data[i].Regionid, data[i].SiteName, data[i].Assetname), //data[i].Description,
                draggable: false,
                optimized: false,
                icon: "../Images/GeneratorsIcon/" + (data[i].Gridduration === "Genset Off" ? "DG-Grey-40x40.png" : "DG-Green-40x40.png")
                //, optimized: false
            });
            registerOverlay(null, null, null, null, marker, null, gMap.drawing.OverlayType.MARKER);
            var index = overlaysArray.marker.indexOf(marker);
            overlaysArray.uniqueNo[index] = data[i].Id;

            gMap.event.addListener(overlaysArray.marker[index], 'click', function () {
                if (overlaysArray.infoWindow[index] !== null)
                    overlaysArray.infoWindow[index].close();
                overlaysArray.infoWindow[index] = new gMap.InfoWindow({
                    content: "<div id='iw-container'>" +
                        "<div class='iw-title'><span>" + overlaysArray.marker[index].getTitle() + "</span></div>" + //"({0})-({1})-({2})".format(data.RegionName, data.SiteName, data.AssetName)
                        "<div id='assetTreeLoader' class='ajax-loader' style='height: 100px;position: relative;margin-left: 110px;'></div>" +
                        "<div class='iw-content'>" +
                        "<table border=0 class='msgpopuptable'><tbody>" +
                        "</tbody></table></div><div class='iw-bottom-gradient'></div></div>"
                });

                gMap.event.addListener(overlaysArray.infoWindow[index], "domready", function() {
                    var iwOuter = $(".gm-style-iw");
                    var iwCornersTail = iwOuter.prev();
                    var iwBtnClose = iwOuter.next();
                    iwOuter.css({ left: "0", top: "0" });
                    iwCornersTail.children(":nth-child(2)").css({ "border-bottom-left-radius": "10px", "border-bottom-right-radius": "10px", "background-color": "rgba(72, 181, 233, 0.4)" });
                    iwCornersTail.children(":nth-child(4)").css({ "border-bottom-left-radius": "10px", "border-bottom-right-radius": "10px" });
                    iwCornersTail.children(":nth-child(3)").find("div").children().css({ "box-shadow": "rgba(72, 181, 233, 0.6) 0px 1px 6px" });
                    iwBtnClose.css({ opacity: "1", right: "-12px", top: "-12px", border: "7px solid #48b5e9", "border-radius": "13px", "box-shadow": "0 0 5px #3990B9" });
                    if ($(".iw-content").height() <= 200) $(".iw-bottom-gradient").css({ display: "none" });
                    iwBtnClose.mouseout(function() { $(this).css({ opacity: "1" }); });
                    //Asif edit 
                    var tableWidth = iwOuter.find("#iw-container").width();
                    iwBtnClose.css({ left: tableWidth - 15 });

                    iwCornersTail.children(":nth-child(2)").width(tableWidth); //Resize Shadow according to table
                    iwCornersTail.children(":nth-child(4)").width(tableWidth - 2);
                });

                overlaysArray.infoWindow[index].open(map, marker);
                //overlaysArray.infoWindow[index] = infoWindow;

                $.ajax({
                    url: urlGetDashboardStationaryDetailsByUnitId + "?unitId=" + overlaysArray.uniqueNo[index],
                    type: "GET",
                    contentType: "application/json charset=utf-8",
                    dataType: "json",
                    success: function(data, textStatus, xhr) {
                        if (textStatus === "success") {

                            var capacity = (Math.floor(data.Capacity * 0.01) * 100) + 100;

                            gMap.event.addListener(overlaysArray.infoWindow[index], "content_changed", function() {
                                $('#gauge' + index).jqxLinearGauge({
                                    min: 0, //data.MinLevel,
                                    max: capacity,
                                    //showRanges: false,
                                    ranges: [
                                        { startValue: 0, endValue: data.MinLevel, style: { fill: '#FF0000', stroke: '#FF0000' }, endWidth: 5, startWidth: 1 },
                                        { startValue: data.MinLevel, endValue: parseFloat(data.Capacity), style: { fill: '#61b314', stroke: '#61b314' }, endWidth: 10, startWidth: 5 }
                                    ],
                                    //ticksMajor: { visible: false },
                                    //ticksMinor: { visible: false },
                                    //labels: { visible: false },
                                    //background: { backgroundType: 'rectangle' },
                                    //pointer: { size: '100%', offset: '0%' },
                                    //scaleLength: '100%',
                                    //ticksOffset: [0, 0],
                                    width: 60,
                                    height: 150,
                                    colorScheme: 'scheme01', //+ ((data.MinLevel > data.Currents) + 1) * 2,

                                    ticksMajor: { size: '15%', interval: (capacity / 4) },
                                    ticksMinor: { interval: (capacity / 8), size: '10%', style: { 'stroke-width': 1, stroke: '#aaaaaa' } },
                                    labels: { interval: Math.round(capacity / 4), position: 'near', offset: 7 },
                                    background: { visible: false },
                                    pointer: { size: '10%', pointerType: 'default', offset: 2 }, //pointerType: 'arrow'
                                    //scaleLength: '93%',
                                    //rangeSize: [3,0],//ticksOffset: [0, 0], //thinner red-green line
                                    //rangesOffset: 15, //to move red-green line little bit left side
                                    animationDuration: 1000,
                                    ticksPosition: 'both'
                                });
                                $('#gauge' + index).jqxLinearGauge('value', data.Currents);
                                $('#gauge' + index + ' td').css({ border: 0 });
                            });

                            var activationDt = '';
                            if (data.ActivationDt != null)
                                activationDt = Date.parse(data.ActivationDt).toString("dd-MM-yyyy HH:mm:ss");

                            overlaysArray.infoWindow[index].setContent(
                                "<div id='iw-container'>" +
                                "<div class='iw-title'><span>" + overlaysArray.marker[index].getTitle() + "</span></div>" + //"({0})-({1})-({2})".format(data.RegionName, data.SiteName, data.AssetName)
                                "<div class='iw-content'>" +
                                "<table border=0 class='msgpopuptable'><tbody>" +
                                "<tr><td><b>Current Status:</b></td><td " + (data.EventName.toLowerCase().indexOf("on") > -1 ? "class='eventname-gnGreen'" : "") + ">" + data.EventName.toLowerCase().replace("genset", "").replace("-", "") + "</td>" +
                                "<td rowspan='8'><div id='gauge" + index + "'></div></td></tr>" +
                                "<tr><td><b>Capacity (Ltrs):</b></td><td>" + data.Capacity + "</td></tr>" +
                                "<tr><td><b>Base Volume (Ltrs):</b></td><td>" + data.BaseVolume + "</td></tr>" +
                                "<tr><td><b>Min Fuel Level (Ltrs):</b></td><td>" + data.MinLevel + "</td></tr>" +
                                "<tr><td><b>Current Fuel (Ltrs):</b></td><td>" + data.Currents + "</td></tr>" +
                                "<tr><td><b>Fuel Level (Ltrs):</b></td><td>" + data.LevelType + "</td></tr>" +
                                "<tr><td><b>Duration (H:M:S):</b></td><td>" + data.TotDuration + "</td></tr>" +
                                "<tr><td><a href='#' id='showAmpleView" + index + "' title='Ample View' onclick=showAmpleView(this," + overlaysArray.uniqueNo[index] + ")><img src='/Images/common/binoculars-32X16.jpg'></a>" +
                                "<b>Rating:</b></td><td>" + data.Dgcap + " KVA</td></tr>" +
                                "<tr><td><b>Last Message Date:</b></td><td colspan=2>" + activationDt + "</td></tr>" +

                                //Date.parse(data[0].Fromdt.toString()).toString("dd-MM-yyyy HH:mm:ss")
                                "</tbody></table></div><div class='iw-bottom-gradient'></div></div>");
                        }
                    }
                });
            });
        }
    });
    if (data.length > 0)
        map.fitBounds(bounds);
    var breakloop = false;
    $.each(overlaysArray.marker, function(parentIndex, parentValue) {
        for (var childIndex = parentIndex; childIndex < overlaysArray.marker.length; childIndex++) {
            if (overlaysArray.uniqueNo[parentIndex] !== overlaysArray.uniqueNo[childIndex]
                && overlaysArray.circle[childIndex] === null && overlaysArray.circle[parentIndex] === null) {
                breakloop = false;
                $.each(overlaysArray.circle, function(circleIndex, circleValue) {
                    if (circleValue != null && overlaysArray.uniqueNo[parentIndex] !== overlaysArray.uniqueNo[circleIndex] &&
                    overlaysArray.uniqueNo[childIndex] !== overlaysArray.uniqueNo[circleIndex]) {
                        if (circleValue.getBounds().contains(overlaysArray.marker[childIndex].getPosition())) {
                            var unitsCount = parseInt(overlaysArray.rectangle[circleIndex].getTitle().split(" ")[0]) + 1;
                            overlaysArray.rectangle[circleIndex].setIcon(
                                new gMap.MarkerImage("https://chart.googleapis.com/chart?chst=d_map_pin_letter&chld=" + unitsCount + "|C6EF8C|000", null, null, null, null));
                            overlaysArray.rectangle[circleIndex].setTitle("{0} units in the circle".format(unitsCount));
                            breakloop = true;
                            return;
                        }
                    }
                });
                if (breakloop) return true;
                if (gMap.geometry.spherical.computeDistanceBetween(parentValue.getPosition(), overlaysArray.marker[childIndex].getPosition()) <= 100) {
                    overlaysArray.circle[parentIndex] = new gMap.Circle({
                        strokeColor: '#FF0000',
                        strokeOpacity: 0.8,
                        strokeWeight: 2,
                        fillColor: '#FF0000',
                        fillOpacity: 0.35,
                        map: map,
                        center: parentValue.getPosition(),
                        radius: 100
                    });
                    
                    overlaysArray.rectangle[parentIndex] = new gMap.Marker({
                        position: gMap.geometry.spherical.computeOffset(parentValue.getPosition(), 100, 180),
                        map: map,
                        title: "{0} units in the circle".format(2),
                        draggable: false,
                        optimized: false,
                        icon: new gMap.MarkerImage("https://chart.googleapis.com/chart?chst=d_map_pin_letter&chld=" + 2 + "|C6EF8C|000", null, null, null, null)
                    });
                }
            }
        }
    });
}

function drawCityBoundriesOnMap(data) {
    var cityNames = [];
    $.each(data, function(i, v) {
        cityNames.push(v.CityId);
    });
    cityNames = $.unique(cityNames);

    $.each(cityNames, function(i, v) {
        if (v !== undefined) {
            $.getJSON("/Content/json/pakistan/" + v + ".json", function(data) {
                var latLngs = [];
                $.each(data, function(i, v) {
                    latLngs.push(new gMap.LatLng(v.Lati, v.Longi));
                });
                var polygon = new gMap.Polygon({
                    paths: latLngs,
                    map: map,
                    fillColor: '#dc4b3f',
                    fillOpacity: 0.2,
                    strokeColor: '#dc4b3f',
                    strokeWeight: 1.2,
                    clickable: false
                });

                registerOverlay(polygon, null, null, null, null, null, gMap.drawing.OverlayType.POLYGON);
            });
        }
    });
}

function zoomBackToFitAllAssetsOnMap()
{
    var bounds = new gMap.LatLngBounds();
    $.each(overlaysArray.baseType, function(index, value) {
        if (value === "marker") 
            bounds.extend(overlaysArray.marker[index].getPosition());
    });
    map.fitBounds(bounds);
}

function changeYearOrMonthSum(changeType) {
    var month = $('#monthsForSummary').val() - changeType;
    var year = $('#yearsForSummary').val();

    if (month == 0) {
        month = 12;
        year--;
    }
    else if (month > 12)
    {
        month = 1;
        year++;
    }

    if ($('#yearsForSummary option[value=' + year + ']').length == 0)
        return;

    $('#monthsForSummary').val(month);
    $('#yearsForSummary').val(year);

    $.ajax({
        url: urlGetMonthWiseSummaryByUnitIdAndMonthAndYear + "?unitId=" + _unitIdForAmpleView + "&month=" + month + "&year=" + year,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function(data, textStatus, xhr) {
            if (textStatus === "success") {
                getMonthWiseHtml(data, month, year);

            }
        }
    });
}

function setSensorBoardHtml(data, voltageData) {
    var sensorBoardHtml = "<table class='ampleviewtable ampleviewtableborder' style='width:100%'><tr><th style='padding:5px'>S#</th><th style='width: 150px'>Digital Input</th><th>Status</th></tr>";
    

    if (data !== null)
        $.each(data, function (index, value) {
            sensorBoardHtml += "<tr><td>{0}</td><td>{1}</td><td style='background-color: white;'><img style='width: 30px' src='/Images/common/led_circle_{2}.gif'/></td></tr>";
            sensorBoardHtml = sensorBoardHtml.format(value.Id, value.Description + (value.DiStatus === "G" ? " ON" : " OFF"), (value.DiStatus === "G" ? "green" : "red"));
        });
    if (data !== null) {
        sensorBoardHtml += "<tr><th style='padding:5px'>S#</th><th style='width: 150px'>Analog Input</th><th>Volts</th></tr>";
        $.each(voltageData, function (index, value) {
            sensorBoardHtml += "<tr><td>{0}</td><td>{1}</td><td style='background-color: white;'>{2}</td></tr>";
            sensorBoardHtml = sensorBoardHtml.format(1, value.Description, value.Voltage);
        });
    }

    sensorBoardHtml += "</table>";
    $("#sensorBoardInfo").html(sensorBoardHtml);
}

function getMonthWiseHtml(data, month, year) {
    var monthWiseHtml = "<table width='100%' class='ampleviewtable ampleviewtableborder'>" +
    "<tr><th>{0}</th><td {22}>{1}</td><th>Ltrs</th><th {25}>{2}{26}</th><td {22}>{3}</td><th>Ltrs</th><th {23}>{4}{24}</th><td {22}>{5}</td><th>Ltrs</th></tr>" +
    "<tr><th>{6}</th><td {22}>{7}</td><th>Ltrs</th><th>{8}</th><td {22}>{9}</td><th>Ltrs</th><th>{10}</th><td {22}>{11}</td><th>Ltrs</th></tr>" +
    "<tr><th>{12}</th><td {22}>{13}</td><th>Ltrs</th><th>{14}</th><td {22}>{15}</td><th>Ltrs</th><th>{16}</th><td {22}>{17}</td><th></th></tr>" +
    "<tr><th>{18}</th><td {22}>{19}</td><th>Ltrs</th><th>{20}</th><td {22}>{21}</td><th>Ltrs</th><th colspan='3'></th></tr>";

    if (data !== null)
        monthWiseHtml = monthWiseHtml.format(
            "Opening", data.Id, "Refuel", (data.Refuel !== 0 ? "<a href='#' onclick='getRefuelDetailsForStationary(\"{0}\",\"{1}\",\"{2}\")'>".format("R", month,year) + data.Refuel + "</a>" : data.Refuel), "Theft", (data.Theft !== 0 ? "<a href='#' onclick='getRefuelDetailsForStationary(\"{0}\",\"{1}\",\"{2}\")'>".format("T", month, year) + data.Theft + "</a>" : data.Theft),
            "Consume", data.Consume,  "Unburned", data.Unconsume, "Net Consume", data.ConsumeWithUnCon,
            "Closing", data.Closing, "DG Run Hrs", data.TotalRuningHour, "Grid ON Hrs", data.GridRunHr, //"Return/Drain", data.FuelReturnDrain,
            "Stab Margin", data.StabMargin, "Avg Consumption", (data.AverageConsumption === null) ? 0 : data.AverageConsumption,
            "style = 'text-align:right;border-right-style: hidden;'", 
            (data.Theft !== 0 ? "class='ui-state-error ui-corner-all'" : ""), (data.Theft !== 0 ? '<span class="ui-icon ui-icon-alert" style="float: right; margin-right: .3em;"></span>' : ""),
            (data.Refuel !== 0 ? "class='ui-state-highlight ui-corner-all'" : ""), (data.Refuel !== 0 ? '<span class="ui-icon ui-icon-info" style="float: right; margin-right: .3em;"></span>' : "")
        );
    else
        monthWiseHtml = monthWiseHtml.format(
            "Opening", 0, "Refuel", 0, "Net Consumpe", 0, "Theft", 0, "Closing", 0, "Return/Drain", 0,
            "Stab Margin", 0, "Total Run Hrs", 0, "Avg Consumption", 0,
            "style = 'text-align:right;border-right-style: hidden;'", "style='text-align:right;border-right-style: hidden;'",
             "",  "", "", "");

    $("#monthWise").html(monthWiseHtml);

    $(".ui-state-error.ui-corner-all").stop(true);
    if (data !== null)
        if (data.Theft !== 0) 
            for (var i = 0; i < 100; i++) 
                $(".ui-state-error.ui-corner-all").fadeTo('slow', 0.0).fadeTo('slow', 1.0);
}

function getRefuelDetailsForStationary(eventType, month,year) {
    $.ajax({
        url: urlGetRefuelDetailsForStationaryByUnitIdAndEventTypeAndMonthAndYear + "?unitId=" + _unitIdForAmpleView + "&eventType=" + eventType + "&month=" + month + "&year=" + year,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function(data, textStatus, xhr) {
            if (textStatus === "success") {

                $.each(data, function(index, value) {
                    var time = new Date(value.CloseDt);
                    var date = new Date(value.OpenDt);
                    var cdate = new Date(date.getFullYear(), date.getMonth(), date.getDate(),
                        time.getHours(), time.getMinutes(), time.getSeconds());

                    // format and round seconds
                    cdate = formatDateTime(roundSeconds(cdate));

                    var refuelDetailHtml = "<tr><td>{0}</td><td>Ignition{1}</td><td>{2}</td><td style='text-align:right; padding-right: 4px;'>{3}</td></tr>";
                    refuelDetailHtml = refuelDetailHtml.format(value.Id, (value.LevelType == "N" ? "(On)" : "(Off)"), cdate, value.BalanceQty.toFixed(1));
                    $("#tableRefuelDetails").append(refuelDetailHtml);
                });
            }
        }
    });
    $("#divRefuelDetails").dialog({ title: "{0} {1} Detail (Monthly)".format(_siteNameForAmpleViewDetail, (eventType == "T"? "Fuel Theft": "Refuel")) });
    $("#divRefuelDetails").dialog("open");
}

function setStatisticsFieldValue(data) {
    if (_fuelRateForAmpleView === null) _fuelRateForAmpleView = 0;
    $("#tdStdFuelRate").text(_fuelRateForAmpleView);

    if (data.Id === null) data.Id = 0;
    $("#tdFuelOpening").text(data.Id.toFixed(2));

    if (data.TripConsume === null) data.TripConsume = 0;
    $("#tdTotalCons").text(data.TripConsume.toFixed(2));

    if (data.TripRefuel === null) data.TripRefuel = 0;
    $("#tdRefuelVolume").text(data.TripRefuel.toFixed(2));

    if (data.TripTheft === null) data.TripTheft = 0;
    $("#tdDrainVolume").text(data.TripTheft.toFixed(2));

    if (data.FuelLvlEnd === null) data.FuelLvlEnd = 0;
    $("#tdFuelClosing").text(data.FuelLvlEnd.toFixed(2));

    $("#tdTotalRun").text(data.TotRunHrs);

    $("#tdGridOff").text(data.GridOffHrs);

    if (data.AvgConsume === null) data.AvgConsume = 0;
    $("#tdAvgCons").text(data.AvgConsume.toFixed(2));

    if (data.FuelRateHr === null) data.FuelRateHr = 0;
    $("#tdFuelRate").text(data.FuelRateHr.toFixed(2));
}

function getStatisticsByDate(changeType) {
    var statsDate = $("#statisticsDate").datepicker("getDate");
    statsDate.setDate(statsDate.getDate() + changeType);

    $.ajax({
        url: urlGetStatisticsByUnitIdAndOpenDateAndFuelRate + "?unitId=" + _unitIdForAmpleView + "&openDate=" + statsDate.toJSON() + "&fuelRate=" + _fuelRateForAmpleView,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function(data, textStatus, xhr) {
            if (textStatus === "success") {
                setStatisticsFieldValue(data);
                $("#statisticsDate").datepicker("setDate", statsDate);
            }
        }
    });
}

function registerOverlay(polygon, circle, polyline, rectangle, marker, infoWindow, baseType, arrayIndex) {
    if (arrayIndex == undefined) {
        overlaysArray.polygon.push(polygon);
        overlaysArray.circle.push(circle);
        overlaysArray.polyline.push(polyline);
        overlaysArray.rectangle.push(rectangle);
        overlaysArray.marker.push(marker);
        overlaysArray.infoWindow.push(infoWindow);
        overlaysArray.baseType.push(baseType);
    } else {
        unRegisterOverlay(arrayIndex);
        overlaysArray.polygon[arrayIndex] = polygon;
        overlaysArray.circle[arrayIndex] = circle;
        overlaysArray.polyline[arrayIndex] = polyline;
        overlaysArray.rectangle[arrayIndex] = rectangle;
        overlaysArray.marker[arrayIndex] = marker;
        overlaysArray.infoWindow[arrayIndex] = infoWindow;
        overlaysArray.baseType[arrayIndex] = baseType;
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
// First, checks if it isn't implemented yet.
if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined' ? args[number] : match;
        });
    };
}

function gridErrorLoaderFunction(gridId, errorLoaderId) {
    $("#" + gridId).jqGrid('setGridParam', {
        loadError: function (xhr, st, err) {
            $("#" + errorLoaderId).hide();
        }
    });
}

function GetTimeInFullByHourAndMinuteAndSecond(hour, minute, second) {
    var addMinutes = Math.floor(second / 60);
    second = parseInt(second - (60 * Math.floor(second / 60)));
    minute += parseInt(addMinutes);

    var addHours = Math.floor(minute / 60);
    minute = parseInt(minute - (60 * Math.floor(minute / 60)));
    hour += parseInt(addHours);

    return ("{0}:{1}:{2}").format(hour, minute, second);
}

function showFuelTransactionChart(unitId, month, year, noOfMonths) {
    //var unitId = "11051810", month = 5, year = 2017, noOfMonths = 6;

    $.ajax({
        url: urlGetNoOfMonthsDashboard + "?unitId=" + unitId + "&month=" + month + "&year=" + year + "&noOfMonths=" + noOfMonths,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",

        beforeSend: function() {
            $("#chartFuelTransaction").showLoaderPanel("big");
            $("#chartFuelTransaction .ddt-loader").css({ top: parseInt($("#chartFuelTransaction").css("height").replace("px", "")) / 4, left : parseInt($("#chartFuelTransaction").css("width").replace("px", "")) / 2 });
            //$("#chartFuelTransaction .ddt-loader").css({ width: $("#chartFuelTransaction").css("width").replace("px", "") / 2 });
        },
        complete: function () {
            $("#chartFuelTransaction").hideLoaderPanel();
        },
        success: function(data, textStatus, xhr) {
            if (textStatus === "success") {
                if (data.length === 0) {
                    $("#chartFuelTransaction").dialog("close");
                    return;
                }
                $("#chartFuelTransaction").parent().css({ opacity: 1 });
                var lineRefuel = [], lineConsume = [], lineUnConsume = [], lineTheft = [], lineOpenningBal = [], lineClosing = [], lineRunHours = [], completeData = [],
                    totalRefuel = 0, totalConsume= 0, totalTheft = 0, totalOpenningBal = 0, totalClosing = 0, totalUnConsum = 0;

                var ind = 0;
                var indexMonth;
                for (var i = 0; i < noOfMonths; i++) {
                    indexMonth = new Date(year, month, 1).addMonths(-noOfMonths + i).getMonth();
                    
                    var openDate = new Date(null);
                    if (data[ind] != undefined) openDate = new Date(data[ind].OpenDate);
                    
                    if (openDate.getMonth() === indexMonth) {
                        completeData.push(data[ind]);
                        ind++;
                        continue;
                    } else {
                        var newDate = new Date(year, month, 1).addMonths(-noOfMonths + i); // openDate.addMonths(-1);
                        completeData.push({
                            Closing: 0,
                            CompRunHr: unitId,
                            GridRunHr: newDate.getMonthName().substr(0, 3) + "-" + newDate.getFullYear(),
                            Id: 0,
                            OpenDate: newDate, //.addMonths(-1),
                            OpenningMile: 0,
                            Refuel: 0,
                            Theft: 0,
                            TotalMileage: 0,
                            Unconsume: 0,
                            Consume: 0,
                            TotalRuningHour: "00:00:00"
                        });
                    }
                }
                $.each(data, function(i, v) {
                    if (i===0) totalOpenningBal = v.OpenningMile;
                    if (i===data.length-1) totalClosing = v.Closing;
                });
                $.each(completeData, function(i, v) {

                    var hours = Math.floor(v.TotalMileage / 3600);
                    var leftSecondAfterHours = v.TotalMileage % 3600;
                    var minutes = Math.floor(leftSecondAfterHours / 60);
                    var seconds = leftSecondAfterHours % 60;
                    v.TotalRuningHour = ("00" + hours).slice(-2) + ":" + ("00" + minutes).slice(-2) + ":" + ("00" + seconds).slice(-2);

                    /*v.TotalMileage = v.TotalMileage / 3600;
                    var tm = '0' + Math.floor(v.TotalMileage),
                        tm1 = '0' + Math.floor((((v.TotalMileage * 3600) % 3600) / 60)).toString();
                    v.Unconsume = tm.substr(tm.length - 3) + ':' + tm1.substr(tm1.length - 2);*/

                    //v.Consume = v.OpenningMile + v.Refuel - v.Closing;
                    v.Unconsume = -((((v.OpenningMile + v.Refuel) - (v.Consume  + v.Theft))) - v.Closing);
                    var openDate = new Date(v.OpenDate);
                    v.GridRunHr = openDate.getMonthName().substr(0, 3) + "-" + openDate.getFullYear();

                    lineRefuel.push([v.GridRunHr, v.Refuel]);
                    lineConsume.push([v.GridRunHr, v.Consume]);
                    lineUnConsume.push([v.GridRunHr, v.Unconsume]);
                    lineTheft.push([v.GridRunHr, v.Theft]);
                    lineOpenningBal.push([v.GridRunHr, v.OpenningMile]);
                    lineClosing.push([v.GridRunHr, v.Closing]);
                    lineRunHours.push([v.GridRunHr, Math.round(v.TotalMileage / 3600 * 100) / 100]);

                    totalRefuel += v.Refuel;
                    totalConsume += v.Consume;
                    totalTheft += v.Theft;
                    totalUnConsum += v.Unconsume;
                });
                $('#trend_chart').jqplot([lineRunHours, lineRefuel, lineConsume, lineTheft, lineOpenningBal, lineClosing, lineUnConsume], {
                    //title: seriesTitle,
                    animate: true,
                    seriesDefaults: {
                        showMarker: true,
                        pointLabels: { show: false },//Asif true
                        rendererOptions: { smooth: true }
                    },
                    // Size of the grid containing the plot.
                    gridDimensions: {
                        height: $("#trend_chart").height() * .95,
                        width: $("#trend_chart").width() * .95
                    },
                    series: [
                        {
                            pointLabels: { show: false },
                            label: "Run Hours",
                            showHighlight: false,
                            yaxis: 'y2axis',
                            renderer: $.jqplot.BarRenderer,
                            rendererOptions: {
                                // Speed up the animation a little bit.
                                // This is a number of milliseconds.  
                                // Default for bar series is 3000.  
                                animation: {
                                    speed: 2500
                                },
                                barWidth: 60,
                                barPadding: -60,
                                barMargin: 0,
                                highlightMouseOver: false
                            }
                        }, { label: "Refuel" }, { label: 'Consumption' }, { label: 'Theft' } , { label: 'Openning' } , { label: 'Closing' } , { label: 'Unburned/Drainage' }
                    ], //,lineWidth: 4, markerOptions: { style: 'filledCircle' } //END MARKER OPTIONS
                    legend: {
                        placement: 'outsideGrid',
                        show: true,
                        location: 'sw'
                    },
/*                    grid: {
                        drawGridLines: true, // whether to draw lines across the grid or not.
                        gridLineColor: '#CCCCCC', // Color of the grid lines.
                        background: '#FFFFFF', // CSS color spec for background color of grid.
                        borderColor: '#DDDDDD', // CSS color spec for border around grid.
                        borderWidth: 2.0, // pixel width of border around grid.
                        shadow: false, // draw a shadow for grid.
                        shadowAngle: 45, // angle of the shadow.  Clockwise from x axis.
                        shadowOffset: 1.5, // offset from the line of the shadow.
                        shadowWidth: 3, // width of the stroke for the shadow.
                        shadowDepth: 3, // Number of strokes to make when drawing shadow.  
                        // Each stroke offset by shadowOffset from the last.
                        shadowAlpha: 0.07, // Opacity of the shadow
                        renderer: $.jqplot.CanvasGridRenderer, // renderer to use to draw the grid.
                        rendererOptions: {} // options to pass to the renderer.  Note, the default
                    },*/
                    //////
                    // Use the fillBetween option to control fill between two
                    // lines on a plot.
                    //////
                    fillBetween: {
                        // series1: Required, if missing won't fill.
                        series1: 4,
                        // series2: Required, if  missing won't fill.
                        series2: 5,
                        // color: Optional, defaults to fillColor of series1.
                        color: "rgba(227, 167, 111, 0.7)",
                        // baseSeries:  Optional.  Put fill on a layer below this series
                        // index.  Defaults to 0 (first series).  If an index higher than 0 is
                        // used, fill will hide series below it.
                        baseSeries: 4,
                        // fill:  Optional, defaults to true.  False to turn off fill.  
                        fill: true
                    },
                    seriesColors: ['#d5e587', '#85802b', '#00749F', '#F00', '#C7754C', '#17BDB8', '#F4AD42'],
                    axes: {
                        xaxis: {
                            renderer: $.jqplot.CategoryAxisRenderer,
                            label: 'Month'
                        },
                        yaxis: {
                            label: 'Qty in Ltr'
                        },
                        y2axis: {
                            label: "Hours",
                            tickOptions: { formatString: "%d" },
                            rendererOptions: {
                                // align the ticks on the y2 axis with the y axis.
                                alignTicks: true,
                                forceTickAt0: true
                            }
                            //renderer:$.jqplot.DateAxisRenderer,
                            //tickOptions:{formatString:'%H:%M:%S'}
                            //tickInterval:'15 second',
                            ,tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                            //tickOptions: { formatString: '%d.%m.%y %H:%M:%S', angle: -30 }
                        }
                    },
                    cursor: {
                        show: true
                    },
                    highlighter: {
                        show: true,
                        tooltipContentEditor: function (str, seriesIndex, pointIndex, plot) {

                            var month = plot.data[seriesIndex][pointIndex][0];
                            var refuel = Math.round(plot.data[1][pointIndex][1] * 100) / 100;
                            var consump = Math.round(plot.data[2][pointIndex][1] * 100) / 100;
                            var theft = Math.round(plot.data[3][pointIndex][1] * 100) / 100;
                            var opening = Math.round(plot.data[4][pointIndex][1] * 100) / 100;
                            var closing = Math.round(plot.data[5][pointIndex][1] * 100) / 100;
                            var unConsume = Math.round(plot.data[6][pointIndex][1] * 100) / 100;
                            var runHours = plot.data[0][pointIndex][1];
                            
                            var html = "<table class='jqplot-highlighter'>" +
                                "<tr><th colspan=2>{6}</th></tr>" +
                                "<tr><td><b>Month:</b></td><td> {0}</td></tr>" +
                                "<tr><td><b>Opening:</b></td><td> {1}</td></tr>" +
                                "<tr><td><b>Refuel:</b></td><td> {2}</td></tr>" +
                                "<tr><td><b>Consumption:</b></td><td> {3}</td></tr>" +
                                "<tr><td><b>Theft:</b></td><td> {4}</td></tr>" +
                                "<tr><td><b>Closing:</b></td><td> {5}</td></tr>" +
                                "<tr><td><b>Unburned/Drainage:</b></td><td> {8}</td></tr>" +
                                "<tr><td><b>Run Hours:</b></td><td> {7}</td></tr></table>";
                            html = html.format(month, opening, refuel, consump, theft, closing, plot.series[seriesIndex].label, runHours, unConsume);
                            return html;
                        }
                    }
                });

                var s1 = [['Opening', totalOpenningBal], ['Refuel', totalRefuel]];
                var s2 = [['Consumption', totalConsume], ['Theft', totalTheft], ['Closing', totalClosing], ['Unburned/Drainage', totalUnConsum]];

                $('#trend_pie_chart').jqplot([s1, s2], {
                    seriesDefaults: {
                        //seriesColors: [['#C7754C', '#d5e587'],[ '#85802b', '#00749F', '#F00', , '#17BDB8', '#F4AD42']],
                        // make this a donut chart.
                        renderer:$.jqplot.DonutRenderer,
                        rendererOptions:{
                            // Donut's can be cut into slices like pies.
                            sliceMargin: 3,
                            // Pies and donuts can start at any arbitrary angle.
                            startAngle: -90,
                            showDataLabels: true,
                            // By default, data labels show the percentage of the donut/pie.
                            // You can show the data 'value' or data 'label' instead.
                            dataLabels: 'value',
                            // "totalLabel=true" uses the centre of the donut for the total amount
                            totalLabel: true,
                            varyBarColor : true
                        }
                    },
                    series: [
                        {seriesColors: ['#C7754C', '#85802b']}, //#d5e587
                        {seriesColors: [ '#00749F', '#F00', '#17BDB8', '#F4AD42']}
                    ],
                    highlighter: { show: false }
                });
            }
        }
    });
}

var intervalForAV;
function setAutoRefreshTimeForAV() {
    if ($("#txtTimeForAV").val() === '') $("#txtTimeForAV").val('60');
    clearInterval(intervalForAV);
    if ($('#chkAutoRefreshForAV').is(':checked'))
        intervalForAV = setInterval(function () { showAmpleView($("#divAmpleView"), _unitIdForAmpleView); }, $("#txtTimeForAV").val() * 1000);
}

$.widget("custom.combobox", {
    _create: function () {
        this.wrapper = $("<span>")
            .addClass("custom-combobox")
            .insertAfter(this.element)
            .attr('id', this.element[0].id + '_combobox');
        this.element.hide();
        this._createAutocomplete();
        this._createShowAllButton();
    },
    _change: function (event) {
        //this._trigger("change", event);
        if (event.target.id.indexOf("cityForFuelReport") === 0) {
            $("#regionForFuelReport").html("");
            $("#regionForFuelReport").val("");
            $("#siteForFuelReport").html("");
            $("#siteForFuelReport").val("");
            $("#assetForFuelReport").html("");
            $("#assetForFuelReport").val("");
            if ($("#regionForFuelReport_combobox").length) $("#regionForFuelReport").combobox("destroy");
            if ($("#siteForFuelReport_combobox").length) $("#siteForFuelReport").combobox("destroy");
            if ($("#assetForFuelReport_combobox").length) $("#assetForFuelReport").combobox("destroy");
        }
        else if (event.target.id.indexOf("regionForFuelReport") === 0) {
            $("#siteForFuelReport").html("");
            $("#siteForFuelReport").val("");
            $("#assetForFuelReport").html("");
            $("#assetForFuelReport").val("");
            if ($("#siteForFuelReport_combobox").length) $("#siteForFuelReport").combobox("destroy");
            if ($("#assetForFuelReport_combobox").length) $("#assetForFuelReport").combobox("destroy");
        }
        else if (event.target.id.indexOf("siteForFuelReport") === 0) {
            $("#assetForFuelReport").html("");
            $("#assetForFuelReport").val("");
            if ($("#assetForFuelReport_combobox").length) $("#assetForFuelReport").combobox("destroy");
        }
        else if (event.target.id.indexOf("cityForAssetReport") === 0) {
            $("#regionForAssetReport").html("");
            $("#regionForAssetReport").val("");
            $("#siteForAssetReport").html("");
            $("#siteForAssetReport").val("");
            $("#assetForAssetReport").html("");
            $("#assetForAssetReport").val("");
            if ($("#regionForAssetReport_combobox").length) $("#regionForAssetReport").combobox("destroy");
            if ($("#siteForAssetReport_combobox").length) $("#siteForAssetReport").combobox("destroy");
            if ($("#assetForAssetReport_combobox").length) $("#assetForAssetReport").combobox("destroy");
        }
        else if (event.target.id.indexOf("regionForAssetReport") === 0) {
            $("#siteForAssetReport").html("");
            $("#siteForAssetReport").val("");
            $("#assetForAssetReport").html("");
            $("#assetForAssetReport").val("");
            if ($("#siteForAssetReport_combobox").length) $("#siteForAssetReport").combobox("destroy");
            if ($("#assetForAssetReport_combobox").length) $("#assetForAssetReport").combobox("destroy");
        }
        else if (event.target.id.indexOf("siteForAssetReport") === 0) {
            $("#assetForAssetReport").html("");
            $("#assetForAssetReport").val("");
            if ($("#assetForAssetReport_combobox").length) $("#assetForAssetReport").combobox("destroy");
        }
        else if (event.target.id.indexOf("cityForMonthwiseStats") === 0) {
            $("#regionForMonthwiseStats").html("");
            $("#regionForMonthwiseStats").val("");
            $("#siteForMonthwiseStats").html("");
            $("#siteForMonthwiseStats").val("");
            $("#assetForMonthwiseStats").html("");
            $("#assetForMonthwiseStats").val("");
            if ($("#regionForMonthwiseStats_combobox").length) $("#regionForMonthwiseStats").combobox("destroy");
            if ($("#siteForMonthwiseStats_combobox").length) $("#siteForMonthwiseStats").combobox("destroy");
            if ($("#assetForMonthwiseStats_combobox").length) $("#assetForMonthwiseStats").combobox("destroy");
        }
        else if (event.target.id.indexOf("regionForMonthwiseStats") === 0) {
            $("#siteForMonthwiseStats").html("");
            $("#siteForMonthwiseStats").val("");
            $("#assetForMonthwiseStats").html("");
            $("#assetForMonthwiseStats").val("");
            if ($("#siteForMonthwiseStats_combobox").length) $("#siteForMonthwiseStats").combobox("destroy");
            if ($("#assetForMonthwiseStats_combobox").length) $("#assetForMonthwiseStats").combobox("destroy");
        }
        else if (event.target.id.indexOf("siteForMonthwiseStats") === 0) {
            $("#assetForMonthwiseStats").html("");
            $("#assetForMonthwiseStats").val("");
            if ($("#assetForMonthwiseStats_combobox").length) $("#assetForMonthwiseStats").combobox("destroy");
        }
    },
    _open: function (event) {
        //console.log(event.target.id);
    },

    _createAutocomplete: function () {
        var selected = this.element.children(":selected"),
            value = selected.val() ? selected.text() : "";
        
        this.input = $("<input>")
            .appendTo(this.wrapper)
            .val(value)
            .attr("title", "")
            .attr('id', this.element[0].id + '_autocomplete')
            .addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
            .autocomplete({
                delay: 0,
                minLength: 0,
                source: $.proxy(this, "_source"),
                appendTo: $("body"),
                change: $.proxy(this, "_change"),
                open: $.proxy(this, "_open")
            })
            .tooltip({
                classes: {
                    "ui-tooltip": "ui-state-highlight"
                }
            });

        this._on(this.input, {
            autocompleteselect: function (event, ui) {
                ui.item.option.selected = true;
                this._trigger("select", event, {
                    item: ui.item.option
                });
            },

            autocompletechange: "_removeIfInvalid"
        });
    },

    _createShowAllButton: function () {
        var input = this.input,
            wasOpen = false;

        $("<a>")
            .attr("tabIndex", -1)
            .attr("title", "Show All Items")
            .attr('id', this.element[0].id + '_anchor')
            .tooltip()
            .appendTo(this.wrapper)
            .button({
                icons: {
                    primary: "ui-icon-triangle-1-s"
                },
                text: false
            })
            .removeClass("ui-corner-all")
            .addClass("custom-combobox-toggle ui-corner-right")
            .on("mousedown", function () {
                wasOpen = input.autocomplete("widget").is(":visible");
            })
            .on("click", function () {
                input.trigger("focus");

                // Close if already visible
                if (wasOpen) {
                    return;
                }

                // Pass empty string as value to search for, displaying all results
                input.autocomplete("search", "");
            });
    },

    _source: function (request, response) {
        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
        response(this.element.children("option").map(function () {
            var text = $(this).text();
            if (this.value && (!request.term || matcher.test(text)))
                return {
                    label: text,
                    value: text,
                    option: this
                };
        }));
    },

    _removeIfInvalid: function (event, ui) {

        // Selected an item, nothing to do
        if (ui.item) {
            return;
        }

        // Search for a match (case-insensitive)
        var value = this.input.val(),
            valueLowerCase = value.toLowerCase(),
            valid = false;
        this.element.children("option").each(function () {
            if ($(this).text().toLowerCase() === valueLowerCase) {
                this.selected = valid = true;
                return false;
            }
        });

        // Found a match, nothing to do
        if (valid) {
            return;
        }

        // Remove invalid value
        this.input
            .val("")
            .attr("title", value + " didn't match any item")
            .tooltip("open");
        this.element.val("");
        this._delay(function () {
            this.input.tooltip("close").attr("title", "");
        }, 2500);
        this.input.autocomplete("instance").term = "";
    },

    _destroy: function () {
        this.wrapper.remove();
        this.element.show();
    }
});
