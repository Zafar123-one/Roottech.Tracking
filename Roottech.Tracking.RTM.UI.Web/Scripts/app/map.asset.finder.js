var markerRestorePostion, markerRestoreAngle, markerRestoreColor, windowInfoRestoreContent;

function loadAssetFinder(assetObject) {
    var markerPolyline = overlaysArray.markerPolyline[$(assetObject).attr("index-overlay")];
    markerRestorePostion = markerPolyline.getPath().getAt(0);
    markerRestoreAngle = markerPolyline.icons[0].icon.rotation;
    markerRestoreColor = markerPolyline.icons[1].icon.fillColor;
    windowInfoRestoreContent = overlaysArray.infoWindow[$(assetObject).attr("index-overlay")].getContent();
    $("#txtAFMapDate").val(getNowDate());
    var minutes = (Date.now().getMinutes() < 10 ? '0' : '') + Date.now().getMinutes();
    $("#txtAFMapFromTime").val(Date.now().addHours(-1).getHours() + ":" + minutes);
    $("#txtAFMapToTime").val(Date.now().getHours() + ":" + minutes);

    $("#assetFinder").dialog("open");
    $("#btnShowAssetFinder").off('click').on('click', function () { findAssetPosition(assetObject); });
}

function findAssetPosition(assetObject) {
    if (!validateTime($("#txtAFMapFromTime").val())) {
        $("#txtAFMapFromTime").val("");
        alert("From time is not in correct format (HH:mm 24 hours).");
        $("#txtAFMapFromTime").focus();
        return;
    } else if (!validateTime($("#txtAFMapToTime").val())) {
        $("#txtAFMapToTime").val("");
        alert("To time is not in correct format (HH:mm 24 hours).");
        $("#txtAFMapToTime").focus();
        return;
    } else if (Date.parseExact($("#txtAFMapFromTime").val(), ["H:m"]) >= Date.parseExact($("#txtAFMapToTime").val(), ["H:m"])) {
        alert("From time should be less than To time.");
        $("#txtAFMapToTime").focus();
        return;
    }
    getAndSetAssetPosition(assetObject, $("#txtAFMapDate").val(), $("#txtAFMapFromTime"), $("#txtAFMapToTime"));
}

function getAndSetAssetPosition(assetObject, mapDate, fromTimeObj, toTimeObj) {
    $.ajax({
        url: urlGetFirstCDRByDateAndFromTimeAndToTime + "?assetNo=" + assetObject[0].name + "&mapDate=" + mapDate + "&fromTime=" + $(fromTimeObj).val() + "&toTime=" + $(toTimeObj).val(),
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        beforeSend: function() {
            $(assetObject).parent().parent().parent().parent().parent().parent().showLoaderPanel();
            $(assetObject).attr("disabled", true);
        },
        complete: function() {
            $(assetObject).parent().parent().parent().parent().parent().parent().hideLoaderPanel();
            $(assetObject).removeAttr("disabled");
        },
        success: function(data, textStatus, xhr) {
            if (data === null || data.length === 0) {
                alert("There is no data found for asset location");
                $(assetObject).prop("checked", false);
                return;
            }

            var index = $(assetObject).attr("index-overlay");
            $("#infoWindowTemp").html("<div id='iw-container'>" +
                            "<div class='iw-title'><span id='googleMapPopLocId'>Loading.....</span></div>" +
                            "<div class='iw-content'>" +
                            "<table border=0 class='msgpopuptable'><tbody>" +
                            "<tr><td><b>Date:</b></td><td>" + data.RTCDTTM.toString() + "</td></tr>" +
                            "<tr><td><b>GSM Signals #:</b></td><td>" + data.GSMSignals + "</td></tr>" +
                            "<tr><td><b>Ignition:</b></td><td>" + (parseInt(data.DI1) ? "On" : "Off") + "</td></tr>" +
                            "<tr><td><b>Speed(Kmph):</b></td><td>" + data.Speed + "</td></tr>" +
                            "<tr><td><b>Latitude:</b></td><td>" + data.Lati + "</td></tr>" +
                            "<tr><td><b>Longitude:</b></td><td>" + data.Longi + "</td></tr></tbody></table>" +
                            "</div><div class='iw-bottom-gradient'></div></div>");

            var latLng = new google.maps.LatLng(data.Lati.toString(), data.Longi.toString());
            var geocoder = new google.maps.Geocoder();

            geocoder.geocode({ 'latLng': latLng }, function(results, status) {
                if (status == google.maps.GeocoderStatus.OK)
                    if (results[0]) $("#infoWindowTemp #googleMapPopLocId").html(results[0].formatted_address);
                    else $("#infoWindowTemp #googleMapPopLocId").html("Geocoder failed due to: " + status);

                    setAssetPosition(index, latLng, getStateOfAssetImage(data.Speed, data.DI1), data.Angle, true);
                    $("#assetFinder").dialog({
                        close: function () { clearAssetFinder(index); }
                    });
            });
        }
    });
}

function clearAssetFinder(idx) {
    setAssetPosition(idx, markerRestorePostion, markerRestoreColor, markerRestoreAngle, true);
    overlaysArray.infoWindow[idx].setContent(windowInfoRestoreContent);
}

function intiateAssetFinder() {
    $("#assetFinder").dialog({
        height: 120,
        width: 380,
        show: "blind",
        hide: "explode",
        title: 'Asset Finder',
        position: { my: "left+420 top+50", at: "left top" }, //[300, 100],
        autoOpen: false
    });

    //Datepicker will hide in 2 secs need to do that because on window open it covers all the space even don't need to select date.
    $("#txtAFMapDate").datepicker({
        dateFormat: "mm/dd/yy",
        showOn: "button",
        buttonImage: '/images/common/calendar.gif',
        buttonImageOnly: true,
        buttonText: "Select date"
    });
    $("#txtAFMapDate").datepicker({
        beforeShow: datepicker_beforeShow,
        onClose: function () {
            $(window).unbind('.datepicker_beforeShow');
        }
    });
    $("#txtAFMapFromTime").timepicker({
        timeFormat: "HH:mm",
        showOn: "button",
        buttonImage: '/images/common/clock.png',
        buttonImageOnly: true,
        buttonText: "Select from time"
    });
    $("#txtAFMapToTime").timepicker({
        timeFormat: "HH:mm",
        showOn: "button",
        buttonImage: '/images/common/clock.png',
        buttonImageOnly: true,
        buttonText: "Select to time"
    });
}