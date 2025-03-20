function registerSVGVehicle() {
    var scale = 0.05, anchor = new g.Point(180, 100), fillColor = "#00ff00",
        iconsIndexToChangeColor = [1,2,3];
    
    var carSvg = {
        frontTrunkCover: 'M 37.1984,44.521 c -11.667,18.667 -10.816,196.219 7.851,210.219 18.03,-14.851 122.476,-28.646 142.342,-27.364 20.288,-1.492 99.694,8.055 124.089,22.697 6.577,2.13 19.727,-205.553 1.059,-219.553 C 254.2044,14.187 60.5324,14.187 37.1984,44.521 z',
        frontMirror1: 'M 41.763616,257.38721 c 0,0 65.530014,-26.89747 138.643194,-27.58506 65.8331,-0.64088 95.56474,9.623 135.61035,25.01937 -4.85339,48.75588 -9.70781,94.9428 -21.84334,110.33917 C 182.53043,336.93366 175.25034,336.93474 63.606954,365.16178 61.180259,349.76541 39.33692,259.95291 41.763616,257.38721 z',
        frontMirror2: 'M 45.5104,260.195 c 0,0 63.783,-24.762 134.947,-25.395 64.078,-0.59 93.017,8.859 131.995,23.033 -4.724,44.885 -9.449,87.405 -21.261,101.579 -108.667,-25.986 -115.753,-25.985 -224.42,0.001 -2.362,-14.174 -23.623,-96.856 -21.261,-99.218 z',
        backTrunkCover1: 'M 75.697199,531.4265 c -12.368936,59.36816 -22.263456,173.15986 -22.263456,173.15986 19.790088,17.31567 121.211797,24.73683 123.685167,24.73683 7.42115,0 108.84391,-7.42116 126.15959,-32.15798 0,-14.8423 -9.89557,-121.2118 -19.79009,-168.21208 -86.57941,12.36789 -202.843427,7.42115 -207.791211,2.47337 z',
        backTrunkCover2: 'M 80.9454,536.588 c -11.812,56.695 -21.261,165.363 -21.261,165.363 18.899,16.536 115.754,23.623 118.116,23.623 7.087,0 103.943,-7.087 120.479,-30.71 0,-14.174 -9.45,-115.754 -18.899,-160.638 -82.681,11.811 -193.71,7.087 -198.435,2.362 z',
        rightSideMirror: 'M 321.9024,279.094 c 0,0 28.348,2.362 28.348,2.362 0,0 14.174,14.174 4.725,21.261 -9.45,7.087 -33.073,-2.362 -33.073,-2.362 0,0 0,-16.536 0,-21.261 z',
        leftSideMirror: 'M 36.946,283.8187 c 0,0 -28.348,2.362 -28.348,2.362 0,0 -14.174,14.174 -4.725,21.261 9.45,7.087 33.073,-2.362 33.073,-2.362 0,0 0,-16.536 0,-21.261 z',
        leftHeadLight: 'M 52.581979,17.02534 C 43.558875,18.598469 33.262373,36.022549 37.998468,38.927091 49.278826,33.238469 71.325454,25.42117 92.982086,23.242565 97.267617,20.942707 103.11609,13.887009 103.34211,12.31388 88.454284,11.466689 62.424963,15.089243 52.581979,17.02534 z',
        rightHeadLight: 'M 298.69502,12.11524 c 9.0231,1.573129 19.31961,14.632868 14.58351,17.53741 -11.28036,-5.688623 -33.32698,-7.505051 -54.98362,-9.683655 -4.28553,-2.299859 -10.134,-10.9920864 -10.36002,-12.5652148 14.88783,-0.8471911 40.91715,2.7753628 50.76013,4.7114598 z',
        frontBorder1: 'M 112.2854,10.38 c 37.098,-6.547 99.837,-6.002 126.569,-2.729',
        frontBorder2: 'M 112.2854,13.1078 c 37.098,-6.547 99.837,-6.002 126.569,-2.729',
        frontBorder3: 'M 112.2854,13.1078 c 37.098,-6.547 99.837,-6.002 126.569,-2.729',
        frontBorder4: 'M 112.2854,16.3811 c 37.098,-6.547 99.837,-6.002 126.569,-2.729',
        backBorder: 'M 36.35,310.5283 c -2.333,25.667 -14.991005,443.4545 4.666,443.345 69.85641,26.46665 221.62204,23.58419 278.5244,-7.0003 17.53027,-0.0711 6.999,-417.678 4.666,-443.345',

        rightBottomTire: 'M 311.28699,621.03802 v65 c0,8,27,8,27,0 v-65 c-0,-10,-27,-10,-27,-0 z',
        leftBottomTire: 'M 16.286961,631.03802 v65 c0,8,27,8,27,0 v-65 c-0,-10,-27,-10,-27,-0 z',
        rightTopTire: 'M 318.78699,105.12427 v65 c0,8,27,8,27,0 v-65 c-0,-10,-27,-10,-27,-0 z',
        leftTopTire: 'M 8.6333046,109.12427 v65 c0,8,27,8,27,0 v-65 c-0,-10,-27,-10,-27,-0 z',

        leftWindShieldBorder: 'M 41.536846,281.87867 c 0,0 21.53351,82.44164 21.53351,82.44164 0,0 0,154.57631 -3.075844,157.15332 -3.077146,2.57593 -27.686499,46.37213 -27.686499,46.37213 0,0 6.151687,-280.81414 9.228833,-285.96709 z',
        rightWindShieldBorder: 'M 318.84115,276.87867 c 0,0 -21.53351,82.44164 -21.53351,82.44164 0,0 0,154.57631 3.07585,157.15332 3.07714,2.57593 27.6865,46.37213 27.6865,46.37213 0,0 -6.15169,-280.81414 -9.22884,-285.96709 z',
        leftWindShield: 'M 43.1484,295.63 c 0,0 16.536,75.595 16.536,75.595 0,0 0,141.739 -2.362,144.102 -2.363,2.362 -21.261,42.521 -21.261,42.521 0,0 4.724,-257.493 7.087,-262.218 z',
        rightWindShield: 'M 317.178,290.90542 c 0,0 -16.536,75.595 -16.536,75.595 0,0 0,141.739 2.362,144.102 2.363,2.362 21.261,42.521 21.261,42.521 0,0 -4.724,-257.493 -7.087,-262.218 z',

        fullCover: "m 178.7264,782.982 c -113.068,2.362 -130.402,-17.92 -147.106,-21.261 -16.705,-38.776 -19.877,-365.731 -9.855,-392.458 7.493,-60.54 -4.936,-70.565 -8.687,-143.526 -7.14,-85.213 9.815,-37.829 -4.439,-124.481 21.658,-90.216 -19.136,-92.053 168.522,-100.631 172.209,2.401 147.956,10.415 169.614,100.631 -14.254,86.652 2.701,39.268 -4.439,124.481 -3.751,72.961 -16.18,82.986 -8.687,143.526 10.022,26.727 6.85,353.682 -9.855,392.458 -26.153,15.153 -95.459,21.261 -145.068,21.261 z"
    };
    var carIcon = {
        anchor: anchor,
        path: 'M0, 0 ' + carSvg['frontTrunkCover'] + carSvg['frontMirror1'] + carSvg['backTrunkCover1'],
        fillColor: fillColor,
        fillOpacity: 1,
        scale: scale,
        strokeColor: "#60585a",
        strokeWeight: .3,
        strokeOpacity: 1
    };
    var carSideMirrorIcon = {
        anchor: anchor,
        path: "M0, 0 " + carSvg['rightSideMirror'] + carSvg['leftSideMirror'],
        fillColor: fillColor,
        fillOpacity: 1,
        scale: scale,
        strokeColor: "#60585a",
        strokeWeight: 1,
        strokeOpacity: 1
    };
    var carTireIcon = {
        anchor: anchor,
        path: "M0, 0 " + carSvg['rightBottomTire'] + carSvg['leftBottomTire'] + carSvg['rightTopTire'] + carSvg['leftTopTire'],
        fillColor: "#000000",
        fillOpacity: 0.68553503,
        scale: scale,
        strokeColor: "#000000",
        strokeWeight: .3,
        strokeOpacity: 1
    };
    var carHeadLightIcon = {
        anchor: anchor,
        path: "M0, 0 " + carSvg['leftHeadLight'] + carSvg['rightHeadLight'],
        fillColor: "#2e87cf",
        fillOpacity: 0.566038,
        scale: scale,
        strokeColor: "#000000",
        strokeWeight: 1.0820076,
        strokeOpacity: 0.25157201
    };
    var carbackTrunkCover = {
        anchor: anchor,
        path: "M0, 0 " + carSvg['backTrunkCover2'] + carSvg['frontMirror2'],
        fillColor: "#000000",
        fillOpacity: 0.78616399,
        scale: scale,
        strokeColor: "#000000",
        strokeWeight: 1,
        strokeOpacity: 1
    };
    var carWindShieldIcon = {
        anchor: anchor,
        path: "M0, 0 " + carSvg['leftWindShieldBorder'] + carSvg['rightWindShieldBorder'] + carSvg['leftWindShield'] + carSvg['rightWindShield'],
        fillColor: "#000000",
        fillOpacity: 0.68553503,
        scale: scale,
        strokeColor: "#000000",
        strokeWeight: .3,
        strokeOpacity: 1
    };
    var carBackBorderIcon = {
        anchor: anchor,
        path: "M0, 0 " + carSvg['backBorder'] + carSvg['frontBorder1'] + carSvg['frontBorder2'] + carSvg['frontBorder3'] + carSvg['frontBorder4'],
        fillColor: "#000000",
        fillOpacity: 0,
        scale: scale,
        strokeColor: "#000000",
        strokeWeight: .3,
        strokeOpacity: 1
    };
    var carFullCoverIcon = {
        anchor: anchor,
        path: "M0, 0 " + carSvg["fullCover"],
        fillColor: fillColor,
        fillOpacity: 1,
        scale: scale,
        strokeColor: "#60585a",
        strokeWeight: 1,
        strokeOpacity: 1
    };
    this.getIconsWithChangeColor = function (icons, colorEnum, withArrow) {
        fillColor = colorEnum;
        /*switch (color) {
        case "Grey":
            fillColor = "#ccc";
            break;
        case "Red":
            fillColor = "#ff0000";
            break;
        case "Blue":
            fillColor = "#003366";
            break;
        case "Yellow":
            fillColor = "#f7ce00";
            break;
        case "Green":
            fillColor = "#00ff00";
            break;
        }*/
        if (icons == null)
            icons = [{ icon: carTireIcon, offset: '0%' }, { icon: carFullCoverIcon, offset: '0.01%' }, { icon: carSideMirrorIcon, offset: '0.02%' }, { icon: carIcon, offset: '0.03%' }, { icon: carbackTrunkCover, offset: '0.04%' }, { icon: carHeadLightIcon, offset: '0.05%' }, { icon: carWindShieldIcon, offset: '0.06%' }, { icon: carBackBorderIcon, offset: '0.07%' }];

        if (icons.length > 1) {
            for (var i = 0; i < iconsIndexToChangeColor.length; i++) {
                var indx = parseInt(iconsIndexToChangeColor[i]) + parseInt(withArrow ? 1 : 0);
                icons[indx].icon.fillColor = fillColor; //+1 is because of arrow icon
            }
        }
        return icons;
    };
    this.changeAngle = function (icons, angle) {
        $(icons).each(function () {
            $(this)[0].icon.rotation = angle;
            //console.log($(this)[0].icon); ///adjust postion of the mouseover
            //console.log($(this)[0].icon.anchor);
            //$(this)[0].icon.anchor = new google.maps.Point(0, 32);
        });
        //icons.rotation = angle;
    }
    this.changeColor = function (icons, fillColor) {
        $(icons).each(function () {
            $(this)[0].icon.fillColor = fillColor;
        });
    }
    /*var polyline = new google.maps.Polyline({
    path: [new google.maps.LatLng(24.897059, 67.026705),
    new google.maps.LatLng(24.89504, 67.026297),
    new google.maps.LatLng(24.895546, 67.025996)],
    map: map,
    icons: [{ icon: carTireIcon }, { icon: carFullCoverIcon }, { icon: carIcon }, { icon: carbackTrunkCover }, { icon: carHeadLightIcon }, { icon: carWindShieldIcon }, { icon: carBackBorderIcon}]
    })*/
}