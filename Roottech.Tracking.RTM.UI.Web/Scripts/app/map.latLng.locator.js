function latLngLocatorRegisterEvents() {
    var markerImage = new google.maps.MarkerImage("http://www.google.com/intl/en_us/mapfiles/ms/micons/ylw-pushpin.png",
    //"/Content/IconsLandVistaMapMarkersIconsDemo/ICO/NotCentered/MapMarker_PushPin1_Right_Azure.ico",
    // This marker is 32 pixels wide by 32 pixels tall.
                new google.maps.Size(32, 32),
    // The origin for this image is 0,0.
                new google.maps.Point(0, 0),
    // The anchor for this image is the base of the flagpole at 11,32.
                new google.maps.Point(11, 32)
            );
    $("#latLongLocator").dialog({
        width: 410,
        height: 140,
        show: "blind",
        hide: "explode",
        title: 'Lat Lng Locator',
        autoOpen: false,
        close: function () { $("#clearPushpin").click(); },
        position: { my: "left+420 top+50", at: "left top" } //[300, 100],
    });
    $("#showLatLongLocator").click(function () { $("#latLongLocator").dialog("open"); });
    var pushPinMarkers, pushPinEvent;
    $("#goToPoint").click(function () {
        if ($("#latitude").val() == "") {
            $("#latitude").focus();
            return;
        } else if ($("#longitude").val() == "") {
            $("#longitude").focus();
            return;
        }
        var latLng = new google.maps.LatLng($("#latitude").val(), $("#longitude").val());
        if (pushPinMarkers != undefined) pushPinMarkers.setMap(null);
        pushPinMarkers = addMarkerForResource(latLng, "", false, false, -1, undefined, markerImage);
        map.setCenter(latLng);
    });

    $("#getPointFromMap").click(function() {
        google.maps.event.removeListener(pushPinEvent);
        pushPinEvent = google.maps.event.addListener(map, 'click', function(event) {
            if (pushPinMarkers != undefined) pushPinMarkers.setMap(null);
            pushPinMarkers = addMarkerForResource(event.latLng, "", false, false, -1, undefined, markerImage);
            $("#latitude").val(pushPinMarkers.getPosition().lat());
            $("#longitude").val(pushPinMarkers.getPosition().lng());
        });
    });

    $("#clearPushpin").click(function() {
        google.maps.event.removeListener(pushPinEvent);
        if (pushPinMarkers != undefined) pushPinMarkers.setMap(null);
        $("#latitude").val("");
        $("#longitude").val("");
    });
}