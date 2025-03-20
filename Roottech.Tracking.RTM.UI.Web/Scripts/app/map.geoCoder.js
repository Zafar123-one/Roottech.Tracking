var geoCodeMarker, geoCodeEvent, geoCodeInfoWindow;
function geoCoderRegisterEvents() {

    $("#geoCoder").dialog({
        width: 380,
        height: 100,
        show: "blind",
        hide: "explode",
        title: 'Geo Locater',
        position: { my: "left+420 top+50", at: "left top" }, //[300, 100],
        autoOpen: false,
        close: function () { $("#clearGeoCodePushpin").click(); }
    });
    $("#showGeoCoder").click(function () {
        registerAutoComplete(); $("#geoCoder").dialog("open"); });
    
    var geocoder = new google.maps.Geocoder();
    $("#goToGeoCode").click(function () {
        if ($("#geoCodeAddress").val() == "") {
            $("#geoCodeAddress").focus();
            return;
        }
        geocoder.geocode({ 'address': $("#geoCodeAddress").val() }, function(results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                map.setCenter(results[0].geometry.location);
                geoCodeMarker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location
                });
            } else {
                alert("Geocode was not successful for the following reason: " + status);
            }
        });
    });

    $("#getGeoCode").click(function () {
        google.maps.event.removeListener(geoCodeEvent);
        geoCodeEvent = google.maps.event.addListener(map, 'click', function (event) {
            var markerImage = new google.maps.MarkerImage("http://www.google.com/intl/en_us/mapfiles/ms/micons/ylw-pushpin.png",
            //"/Content/IconsLandVistaMapMarkersIconsDemo/ICO/NotCentered/MapMarker_PushPin1_Right_Azure.ico",
            // This marker is 32 pixels wide by 32 pixels tall.
                new google.maps.Size(32, 32),
            // The origin for this image is 0,0.
                new google.maps.Point(0, 0),
            // The anchor for this image is the base of the flagpole at 16,32.
                new google.maps.Point(16, 32)
            );
            if (geoCodeMarker != undefined) geoCodeMarker.setMap(null);
            var latLng = event.latLng;
            geocoder.geocode({ 'latLng': latLng }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    if (results[1]) {
                        //map.setZoom(11);
                        //console.log(results.geometry.getLocation());
                        if (geoCodeMarker != undefined) geoCodeMarker.setMap(null);
                        geoCodeMarker = addMarkerForResource(latLng, "", false, false, -1, undefined, markerImage);
                        $("#geoCodeAddress").val(results[1].formatted_address);
                    }
                } else {
                    $("#geoCodeAddress").val("Geocoder failed due to: " + status);
                }
            });
        });
    });

    $("#clearGeoCodePushpin").click(function () {
        google.maps.event.removeListener(geoCodeEvent);
        if (geoCodeMarker != undefined) geoCodeMarker.setMap(null);
        if (geoCodeInfoWindow != undefined) geoCodeInfoWindow.setMap(null);
        $("#geoCodeAddress").val("");
    });
}

function registerAutoComplete() {
    autoComplete = new google.maps.places.Autocomplete($("#geoCodeAddress")[0]);

    autoComplete.bindTo('bounds', map);

    geoCodeInfoWindow = new google.maps.InfoWindow();
    geoCodeMarker = new google.maps.Marker({
        map: map
    });

    geoCodeEvent = google.maps.event.addListener(autoComplete, 'place_changed', function () {
        geoCodeInfoWindow.close();
        geoCodeMarker.setVisible(false);
        $("#geoCodeAddress")[0].className = '';
        var place = autoComplete.getPlace();
        if (!place.geometry) {
            // Inform the user that the place was not found and return.
            $("#geoCodeAddress")[0].className = 'notfound';
            return;
        }

        // If the place has a geometry, then present it on a map.
        if (place.geometry.viewport) {
            map.fitBounds(place.geometry.viewport);
        } else {
            map.setCenter(place.geometry.location);
            map.setZoom(17); // Why 17? Because it looks good.
        } 
        var image = {
            url: place.icon,
            size: new google.maps.Size(71, 71),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(35, 35)
        };
        geoCodeMarker.setIcon(image);
        geoCodeMarker.setPosition(place.geometry.location);
        geoCodeMarker.setVisible(true);
        
        var address = '';
        if (place.address_components) {
            address = [
                (place.address_components[0] && place.address_components[0].short_name || ''),
                (place.address_components[1] && place.address_components[1].short_name || ''),
                (place.address_components[2] && place.address_components[2].short_name || '')
            ].join(' ');
        }

        geoCodeInfoWindow.setContent('<div><strong>' + place.name + '</strong><br>' + address);
        geoCodeInfoWindow.open(map, geoCodeMarker);
    });
}

// Sets a listener on a radio button to change the filter type on Places
// Autocomplete.
function setupClickListener(id, types) {
    var radioButton = document.getElementById(id);
    google.maps.event.addDomListener(radioButton, 'click', function () {
        autoComplete.setTypes(types);
    });
}
/*
setupClickListener('changetype-all', []);
setupClickListener('changetype-establishment', ['establishment']);
setupClickListener('changetype-geocode', ['geocode']);*/