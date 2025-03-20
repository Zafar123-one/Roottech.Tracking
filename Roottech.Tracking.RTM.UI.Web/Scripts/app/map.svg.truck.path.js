function registerSVGTruck() {
    var scale = 0.1,
        anchor = new g.Point(640, 220),
        fillColor = "#00ff00",
        rotation = 270;
    
    var truckSvg = {
        //fill color black
        frontBumper: "M724.761,159.972c0,0,10.646,0.365,15.644,15.535c4.999,15.171-1.11,103.063-1.11,103.063s1.128,15.376-16.923,15.616 z",
        backBumper: "M232.5,137.5 h9.5 v169 h-9.5z",
        leftLastWheel: "M339.5,140.25c0,4.694-2.487,8.5-5.554,8.5h-57.392c-3.067,0-5.554-3.806-5.554-8.5l0,0c0-4.694,2.487-8.5,5.554-8.5h57.392C337.013,131.75,339.5,135.556,339.5,140.25L339.5,140.25z",
        rightLastWheel: "M339.5,305c0,4.694-2.487,8.5-5.554,8.5h-57.392c-3.067,0-5.554-3.806-5.554-8.5l0,0c0-4.694,2.487-8.5,5.554-8.5h57.392C337.013,296.5,339.5,300.306,339.5,305L339.5,305z",
        leftMiddleWheel: "M408,140.25c0,4.694-2.487,8.5-5.554,8.5h-57.392c-3.067,0-5.554-3.806-5.554-8.5l0,0c0-4.694,2.487-8.5,5.554-8.5h57.392C405.513,131.75,408,135.556,408,140.25L408,140.25z",
        rightMiddleWheel: "M408,305c0,4.694-2.487,8.5-5.554,8.5h-57.392c-3.067,0-5.554-3.806-5.554-8.5l0,0c0-4.694,2.487-8.5,5.554-8.5h57.392C405.513,296.5,408,300.306,408,305L408,305z",
        leftFirstWheel: "M548.5,140.25c0,4.694-2.486,8.5-5.555,8.5h-57.391c-3.068,0-5.555-3.806-5.555-8.5l0,0c0-4.694,2.486-8.5,5.555-8.5h57.391C546.014,131.75,548.5,135.556,548.5,140.25L548.5,140.25z",
        rightFirstWheel: "M548.5,305c0,4.694-2.486,8.5-5.555,8.5h-57.391c-3.068,0-5.555-3.806-5.555-8.5l0,0c0-4.694,2.486-8.5,5.555-8.5h57.391C546.014,296.5,548.5,300.306,548.5,305L548.5,305z",

        // different color on basis on iginition
        cabin: "M597.75,143.75l47.5,2l21.5,2l13.25,3.5l30.75,2.75c0,0,20.5,3.25,25,19s-1,107-1,107s-5.25,16.5-21.5,16.75S651.5,303,651.5,303l-53.75,1.5V143.75z",
        trailer: "M240,135.5 h340.5 v175.5 h-340.5z",

        //color fill none
        leftMirrorSupport: "M666.986,147.833 667.421,122.5 676.045,150.205 z",
        rightMirrorSupport: "M666.986,301.184 667.421,326.517 676.045,298.812 z",
        leftDoor: "M608.758,143.75 609.645,148.883 645.145,148.883 651.979,146.305 z",
        rightDoor: "M608.758,304.944 609.645,299.812 645.145,299.812 651.979,302.389 z",
        leftHeadLightSupport: "M708.065,153.872c0,0,20.771,5.211,29.369,39.537 z",
        rightHeadLightSupport: "M735.857,258.993c-0.269,1.663-5.183,28.314-24.857,37.507 z",

        //yellow color #FFFBB8
        leftHeadLight: "M709.35,153.872c0,0,21.226,3.193,25.58,18.081s1.63,18.582,1.63,18.582l0,0C731.677,171.024,715.584,155.437,709.35,153.872z",
        rightHeadLight: "M710.242,297.146c-1.429,0.19,16.583,0.096,23.084-15.358c2.535-6.026,2.08-17.466,1.715-18.042l0,0C737.44,257.21,726.639,294.098,710.242,297.146z",

        //gray color
        hanger: "M574.5,161.5 h23.5 v116.5 h-23.5z",

        //red color
        leftBackLight: "M237.25,145.75 h4.75 v17.75 h-4.75z",
        rightBackLight: "M237.25,285.125 h4.75 v17.75 h-4.75z",

        //light blue color #D6FCFC, opacity 0.8, stroke #000
        leftFrontLight: "M667.158,134.418c0,1.885-1.528,3.413-3.413,3.413l0,0c-1.884,0-3.412-1.528-3.412-3.413v-12.505c0-1.885,1.528-3.413,3.412-3.413l0,0c1.885,0,3.413,1.528,3.413,3.413V134.418z",
        rightFrontLight: "M667.158,314.599c0-1.885-1.528-3.413-3.413-3.413l0,0c-1.884,0-3.412,1.528-3.412,3.413v12.505c0,1.885,1.528,3.413,3.412,3.413l0,0c1.885,0,3.413-1.528,3.413-3.413V314.599z",
        topAirWindow: "M652.439,253.079c0,4.675-4.137,8.464-9.238,8.464h-27.19c-5.103,0-9.238-3.79-9.238-8.464v-66.658c0-4.675,4.136-8.464,9.238-8.464h27.19c5.102,0,9.238,3.79,9.238,8.464V253.079z",
        frontWindShield: "M693.679,159.879c0,0,6.572,68.633,0,124.3l-33.999-8.359c6.573-55.667-1.67-108.749-1.67-108.749L693.679,159.879z",

        //gray line #C4C0C0
        trailerLine1: "M563.667,148.57c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,147.125,563.667,147.772,563.667,148.57L563.667,148.57z",
        trailerLine2: "M563.667,158.316c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,156.872,563.667,157.519,563.667,158.316L563.667,158.316z",
        trailerLine3: "M563.667,168.063c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,166.618,563.667,167.265,563.667,168.063L563.667,168.063z",
        trailerLine4: "M563.667,177.81c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,176.365,563.667,177.012,563.667,177.81L563.667,177.81z",
        trailerLine5: "M563.667,187.556c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,186.111,563.667,186.758,563.667,187.556L563.667,187.556z",
        trailerLine6: "M563.667,197.303c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,195.858,563.667,196.505,563.667,197.303L563.667,197.303z",
        trailerLine7: "M563.667,207.049c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,205.604,563.667,206.251,563.667,207.049L563.667,207.049z",
        trailerLine8: "M563.667,216.796c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,215.351,563.667,215.998,563.667,216.796L563.667,216.796z",
        trailerLine9: "M563.667,226.542c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,225.098,563.667,225.745,563.667,226.542L563.667,226.542z",
        trailerLine10: "M563.667,236.289c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,234.844,563.667,235.491,563.667,236.289L563.667,236.289z",
        trailerLine11: "M563.667,246.036c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,244.591,563.667,245.238,563.667,246.036L563.667,246.036z",
        trailerLine12: "M563.667,255.782c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,254.337,563.667,254.984,563.667,255.782L563.667,255.782z",
        trailerLine13: "M563.667,265.529c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,264.084,563.667,264.731,563.667,265.529L563.667,265.529z",
        trailerLine14: "M563.667,275.275c0,0.798-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.647-1.445-1.445l0,0c0-0.798,0.647-1.445,1.445-1.445h301.111C563.021,273.831,563.667,274.478,563.667,275.275L563.667,275.275z",
        trailerLine15: "M563.667,285.021c0,0.799-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.646-1.445-1.445l0,0c0-0.797,0.647-1.444,1.445-1.444h301.111C563.021,283.577,563.667,284.225,563.667,285.021L563.667,285.021z",
        trailerLine16: "M563.667,294.768c0,0.799-0.646,1.445-1.444,1.445H261.111c-0.798,0-1.445-0.646-1.445-1.445l0,0c0-0.797,0.647-1.443,1.445-1.443h301.111C563.021,293.324,563.667,293.971,563.667,294.768L563.667,294.768z"
    };
    var truckBlackParts = {
        anchor: anchor,
        path: "M0, 0 " + truckSvg["frontBumper"] + truckSvg["backBumper"] +
            truckSvg["leftFirstWheel"] + truckSvg["rightFirstWheel"] + truckSvg["leftMiddleWheel"] + truckSvg["rightMiddleWheel"] + truckSvg["leftLastWheel"] + truckSvg["rightLastWheel"],
        fillColor: "#000",
        fillOpacity: 1,
        scale: scale,
        strokeColor: "#000",
        strokeWeight: .3,
        strokeOpacity: 1,
        rotation: rotation
    };
    var truckColoredParts = {
        anchor: anchor,
        path: "M0, 0 " + truckSvg["cabin"] + truckSvg["trailer"],
        fillColor: fillColor,//"#F91616"
        fillOpacity: 1,
        scale: scale,
        strokeColor: "#000",
        strokeWeight: .3,
        strokeOpacity: 1,
        rotation: rotation
    };
    var truckLineParts = {
        anchor: anchor,
        path: "M0, 0 " + truckSvg["leftMirrorSupport"] + truckSvg["rightMirrorSupport"] + truckSvg["leftDoor"] + truckSvg["rightDoor"]
            + truckSvg["leftHeadLightSupport"] + truckSvg["rightHeadLightSupport"],
        fillOpacity: 0,//0.68553503,
        scale: scale,
        strokeColor: "#000",
        strokeWeight: 1,
        strokeOpacity: 1,
        rotation: rotation
    };
    var truckYellowParts = {
        anchor: anchor,
        path: "M0, 0 " + truckSvg["leftHeadLight"] + truckSvg["rightHeadLight"],
        fillColor: "#FFFBB8",//"#2e87cf",
        fillOpacity: 1,//0.566038,
        scale: scale,
        strokeWeight: 0,
        strokeOpacity: 0,
        rotation: rotation
    };
    var truckGrayParts = {
        anchor: anchor,
        path: "M0, 0 " + truckSvg["hanger"],
        fillColor: "#F7F0F0",
        fillOpacity: 1,
        scale: scale,
        strokeColor: "#000",
        strokeWeight: 1,
        strokeOpacity: 1,
        rotation: rotation
    };
    var truckRedParts = {
        anchor: anchor,
        path: "M0, 0 " + truckSvg["leftBackLight"] + truckSvg["rightBackLight"],
        fillColor: "#FC6124",
        fillOpacity: 1,
        scale: scale,
        strokeWeight: 0,
        strokeOpacity: 0,
        rotation: rotation

    };
    var truckBlueParts = {
        anchor: anchor,
        path: "M0, 0 " + truckSvg["leftFrontLight"] + truckSvg["rightFrontLight"] + truckSvg["topAirWindow"] + truckSvg["frontWindShield"],
        fillColor: "#D2FBFF",
        fillOpacity: 1,
        scale: scale,
        strokeColor: "#000",
        strokeWeight: .3,
        strokeOpacity: 1,
        rotation: rotation
    };
    var truckGrayLines = {
        anchor: anchor,
        path: "M0, 0 " + truckSvg["trailerLine1"] + truckSvg["trailerLine2"] + truckSvg["trailerLine3"] + truckSvg["trailerLine4"] + truckSvg["trailerLine5"]
            + truckSvg["trailerLine6"] + truckSvg["trailerLine7"] + truckSvg["trailerLine8"] + truckSvg["trailerLine9"] + truckSvg["trailerLine9"]
            + truckSvg["trailerLine10"] + truckSvg["trailerLine11"] + truckSvg["trailerLine12"] + truckSvg["trailerLine13"] + truckSvg["trailerLine14"]
            + truckSvg["trailerLine15"] + truckSvg["trailerLine16"],
        fillColor: "#000",//"#C4C0C0",
        fillOpacity: 1,
        scale: scale,
        rotation: rotation
    };
    this.getIconsWithChangeColor = function (icons, colorEnum, withArrow) {
        fillColor = colorEnum;
        if (icons == null)
            icons = [{ icon: truckBlackParts, offset: "0%" }, { icon: truckRedParts, offset: "0%" }, { icon: truckGrayParts, offset: "0%" },
            { icon: truckColoredParts, offset: "0%" }, { icon: truckLineParts, offset: "0%" }, { icon: truckYellowParts, offset: "0%" }, 
            { icon: truckBlueParts, offset: "0%" }, { icon: truckGrayLines, offset: "0%"}];

        if (icons.length > 1)
            icons[3 + parseInt(withArrow ? 1 : 0)].icon.fillColor = fillColor; //+1 is because of arrow icon
        return icons;
    };
    this.changeAngle = function (icons, angle) {
        $(icons).each(function () {
            $(this)[0].icon.rotation = angle + 270;
        });
    }
    this.changeColor = function (icons, fillThisColor) {
        $(icons).each(function () {
            $(this)[0].icon.fillColor = fillThisColor;
        });
    }
}