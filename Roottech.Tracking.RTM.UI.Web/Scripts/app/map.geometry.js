var poly;
var geodesic;
var map;
var clickcount = 0;
/*
function initialize() {
    var atlantic = new google.maps.LatLng(34, -40.605);
    var mapOptions = {
        zoom: 4,
        center: atlantic,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    map = new google.maps.Map(document.getElementById('map_canvas'), mapOptions);

    var polyOptions = {
        strokeColor: '#FF0000',
        strokeOpacity: 1.0,
        strokeWeight: 3
    }
    poly = new google.maps.Polyline(polyOptions);
    poly.setMap(map);
    var geodesicOptions = {
        strokeColor: '#CC0099',
        strokeOpacity: 1.0,
        strokeWeight: 3,
        geodesic: true
    }
    geodesic = new google.maps.Polyline(geodesicOptions);
    geodesic.setMap(map);

    // Add a listener for the click event
    google.maps.event.addListener(map, 'click', addLocation);
}
*/
function addLocation(event) {
    clickcount++;
    if (clickcount == 1) addOrigin(event);
    if (clickcount == 2) addDestination(event);
}

function addOrigin(event) {
    clearPaths();
    var path = poly.getPath();
    path.push(event.latLng);
    var gPath = geodesic.getPath();
    gPath.push(event.latLng);
}

function addDestination(event) {
    var path = poly.getPath();
    path.push(event.latLng);
    var gPath = geodesic.getPath();
    gPath.push(event.latLng);
    adjustHeading();
    clickcount = 0;
}

function clearPaths() {
    var path = poly.getPath();
    while (path.getLength()) {
        path.pop();
    }
    var gPath = geodesic.getPath();
    while (gPath.getLength()) {
        gPath.pop();
    }
}

function adjustHeading() {
    var path = poly.getPath();
    var pathSize = path.getLength();
    var heading = google.maps.geometry.spherical.computeHeading(path.getAt(0), path.getAt(pathSize - 1));
    document.getElementById('heading').value = heading;
    document.getElementById('origin').value = path.getAt(0).lat()
      + "," + path.getAt(0).lng();
    document.getElementById('destination').value = path.getAt(pathSize - 1).lat()
      + "," + path.getAt(pathSize - 1).lng();
}