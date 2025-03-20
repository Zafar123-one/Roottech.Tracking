
var map;
//var bounds = null;
var directionDisplay;
var directionsService;
var stepDisplay;
var markerArray = [];
var position;
var marker = null;
var polyline = null;
var poly2 = null;
var speed = 0.000005, wait = 1;
var infowindow = null;

var myPano;
var panoClient;
var nextPanoId;
var timerHandle = null;

function createMarker(latlng, label, html) {
    // alert("createMarker("+latlng+","+label+","+html+","+color+")");
    var contentString = '<b>' + label + '</b><br>' + html;
    var marker = new google.maps.Marker({
        position: latlng,
        map: map,
        title: label,
        zIndex: Math.round(latlng.lat() * -100000) << 5
    });
    marker.myname = label;
    // gmarkers.push(marker);

    google.maps.event.addListener(marker, 'click', function () {
        infowindow.setContent(contentString);
        infowindow.open(map, marker);
    });
    return marker;
}


function initializeTrack(lines, path, bounds, totalLength) {
    infowindow = new google.maps.InfoWindow({
        size: new google.maps.Size(150, 50)
    });

    // Create a map and center it on Manhattan.
    /*var myOptions = {
        center: new google.maps.LatLng(0, 0),
        zoom: 13,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
    */

    // Instantiate an info window to hold step text.
    stepDisplay = new google.maps.InfoWindow();

    // Read the data from the xml file
    //downloadUrl("GMdataAB_78465_2008-05-15.xml", function (doc) {
        var g = google.maps;
        //var xmlDoc = xmlParse(doc);
        //bounds = new google.maps.LatLngBounds();
        // ========= Now process the polylines ===========
        //var lines = xmlDoc.documentElement.getElementsByTagName("line");
        // read each line
        //for (var a = 0; a < 1; a++) { //lines.length
            // get any line attributes
            var label;// = lines[a].getAttribute("label");
    if (!label) label = "polyline #";//  + a;
            //var geodesic = lines[a].getAttribute("geodesic");
            //if (!geodesic) { geodesic = false; }
            geodesic = false;
            var opacity;// = lines[a].getAttribute("opacity");
            if (!opacity) { opacity = 0.45; }
            var colour = "#FF0000"; //lines[a].getAttribute("colour");
            var width = 4;// parseFloat(lines[a].getAttribute("width"));
            /*var html = lines[a].getAttribute("html");
            // read each point on that line
            var points = lines[a].getElementsByTagName("point");
            var pts = [];
            var length = 0;
            var point = null;
            for (var i = 0; i < lines.length; i++) {
                pts[i] = new g.LatLng(parseFloat(lines[i].Lati), parseFloat(lines[i].Longi));
                if (i > 0) {
                    length += pts[i - 1].distanceFrom(pts[i]);
                    if (isNaN(length)) {
                        alert("[" + i + "] length=" + length + " segment=" + pts[i - 1].distanceFrom(pts[i]));
                    };
                }
                bounds.extend(pts[i]);
                point = pts[parseInt(i / 2)];
            }*/
            // length *= 0.000621371192; // miles/meter 
            // alert("poly:"+label+" point="+point+" i="+i+" (i/2)%2="+parseInt(i/2)+" length="+length);
            var carSymbol =
    {
        path: google.maps.SymbolPath.CIRCLE, //getStateOfAssetImage(45, 1), //
        //rotation: 145,
        strokeColor: "#393",
        //strokeOpacity: 1,
        //strokeWeight: 5 
        scale: 8
    };

            polyline = new g.Polyline({
                map: map,
                path: path,
                strokeColor: colour,
                strokeOpacity: opacity,
                strokeWeight: width,
                geodesic: geodesic,
                clickable: true
                ,icons: [{ icon: carSymbol, offset: '100%'}]
            });
            //        createClickablePolyline(poly, html, label, point, length);
            // map.addOverlay(poly);
        //}
            map.fitBounds(bounds);
            animateCircle(totalLength);
        //startAnimation();
    //});


    poly2 = new google.maps.Polyline({
        path: [],
        strokeColor: '#FF0000',
        strokeWeight: 3
    });
}
var offsetId;

function animateCircle(totalLegth, meterStep) {
    var percentage = 0;
    offsetId = window.setInterval(function () {
        percentage += (meterStep / totalLegth) * 100;
        var icons = polyline.get('icons');
        icons[0].offset = (totalLegth * percentage) / 100 + "px";
        console.log((totalLegth * percentage) / 100);
        polyline.set('icons', icons);
    }, 1000);
}

var steps = [];


var step = 1; // 5; // metres
var tick = 100; // milliseconds
var eol;
var k = 0;
var stepnum = 0;
var speed = "";
var lastVertex = 1;


//=============== animation functions ======================
function updatePoly(d) {
    // Spawn a new polyline every 20 vertices, because updating a 100-vertex poly is too slow
    if (poly2.getPath().getLength() > 20) {
        poly2 = new google.maps.Polyline([polyline.getPath().getAt(lastVertex - 1)]);
        // map.addOverlay(poly2)
    }

    if (polyline.GetIndexAtDistance(d) < lastVertex + 2) {
        if (poly2.getPath().getLength() > 1) {
            poly2.getPath().removeAt(poly2.getPath().getLength() - 1);
        }
        poly2.getPath().insertAt(poly2.getPath().getLength(), polyline.GetPointAtDistance(d));
    } else {
        poly2.getPath().insertAt(poly2.getPath().getLength(), polyline.getPath().getAt(polyline.getPath().getLength() - 1));
    }
}


function animate(d) {
    // alert("animate("+d+")");
    if (d > eol) {
        var endlocation = polyline.getPath().getAt(polyline.getPath().getLength() - 1);
        map.panTo(endlocation);
        marker.setPosition(endlocation);
        return;
    }
    var p = polyline.GetPointAtDistance(d);
    map.panTo(p);
    marker.setPosition(p);
    updatePoly(d);
    timerHandle = setTimeout("animate(" + (d + step) + ")", tick);
}


function startAnimation() {
    if (timerHandle) clearInterval(timerHandle);
    eol = polyline.Distance();
    map.setCenter(polyline.getPath().getAt(0));
    // map.addOverlay(new google.maps.Marker(polyline.getAt(0),G_START_ICON));
    // map.addOverlay(new GMarker(polyline.getVertex(polyline.getVertexCount()-1),G_END_ICON));
    if (marker) {
        marker.setMap(null);
        delete marker;
        marker = null;
    }
    var car = getStateOfAssetImage(45, 1);
    if (!marker) marker = new google.maps.Marker({ location: polyline.getPath().getAt(0), map: map} /*,{icon:car}*/);
    var carSymbol =
    {
        size: new g.Size(19,41),
        path: car //google.maps.SymbolPath.CIRCLE, //
        //,rotation: 145,
        //strokeColor: "#393",
        //strokeOpacity: 1,
        //strokeWeight: 5 
        //scale: 8

    };

    marker.setIcon(carSymbol);
    // map.addOverlay(marker);
    poly2 = new google.maps.Polyline({ path: [polyline.getPath().getAt(0)], strokeColor: "#0000FF", strokeWeight: 10 });
    // map.addOverlay(poly2);
    setTimeout("animate(50)", 2000);  // Allow time for the initial map display
}


//=============== ~animation funcitons =====================

