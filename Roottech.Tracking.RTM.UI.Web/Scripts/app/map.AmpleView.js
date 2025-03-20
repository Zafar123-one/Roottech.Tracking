var fuelCapacity = "";
function showAmpleView(element) {
    var checkbox = $('li#' + element[0].id).find("[type=checkbox]");
//    getAmpleViewFleetInfo(checkbox[0]);
//    getAmpleViewInfo(checkbox[0]);
//    getAmpleViewCurrentStatus(checkbox[0]);
//    getAmpleViewLastEvent(checkbox[0]);
//    getSensorBoard(checkbox[0]);

    CurrentStatus(checkbox[0]);
    getAmpleViewLastEvent(checkbox[0]);
    getAmpleViewMonthly(checkbox[0]);

    $("#divMain").dialog({
        title: 'Ample View',
        width: 1102,
        height: 546,
        background: "#f4f4f4",
        position: { my: "left+140 top+50", at: "left top"}
    });

    $("#divMain").parent().css({ opacity: 1 });
    $("#divMain").dialog("open");
    
    $('#lnkTripBoard').click(function (e) {
        e.preventDefault();
        $('#divTripBoard').slideToggle('fast', function () {
            $('#divSensorBoard').slideToggle('fast');
        });
    });

    $('#lnkSensorBoard').click(function (e) {
        e.preventDefault();
        $('#divSensorBoard').slideToggle('fast', function () {
            $('#divTripBoard').slideToggle('fast');
        });
    });

    $('#lnkImageGallery').click(function(e) {
        e.preventDefault();
        $("#galleria").dialog({
            title: 'Image Gallery',
            width: 755,
            height: 450
        });
        $("#galleria").dialog("open");

    });

}

function getAmpleViewInfo(assetObject) {
    $.ajax({
        url: urlAmpleViewInfo + "?assetNo=" + assetObject.name,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data, textStatus, xhr) {
            var bindData = { view: data };
            //            $("#templates").load("template.html #template1",function(){
            //	          var template = document.getElementById('template1').innerHTML;
            //	          var output = Mustache.render(template, view);
            //	          $("#person").html(output);
            //                
            $("#txtPlateId").html(data[0].Id.toString());
            $("#txtName").html(data[0].Trip_Status.toString());
            $("#txtTripNo").html(data[0].TripNo.toString());
            $("#txtTripStatus").html(data[0].Trip_Status.toString());
            $("#txtProject").html(data[0].ProjDesc.toString());

            $("#txtEDD").html('-');
            $("#txtADD").html('-');
            $("#txtEDA").html('-');
            $("#lblTotalCapacity").html(data[0].Capacity.toString());
            fuelCapacity = data[0].Capacity.toString();
        }
    });
}

function getAmpleViewCurrentStatus(assetObject) {
    $.ajax({
        url: urlAmpleViewCurrentStatus + "?assetNo=" + assetObject.name,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data, textStatus, xhr) {
            $("#txtEvent").html(data[0].Id.toString());
            $("#txtActivity").html(data[0].Event.toString());
            $("#txtSite").html(data[0].Site.toString());
            $("#txtFromTime").html(Date.parse(data[0].Fromdt.toString()).toString("dd-MM-yyyy HH:mm:ss"));
            $("#txtToTime").html(Date.parse(data[0].Todate.toString()).toString("dd-MM-yyyy HH:mm:ss"));
            //$("#txtTotDuration").html(data[0].TotalDuration.toString());
            $("#txtFromMileage").html(data[0].FromMileage.toString());
            $("#txtCurrentMileage").html(data[0].CurrentMileage.toString());
            $("#txtNetMileage").html(data[0].NetMileage.toString());
            $("#txtNetKilo").html(data[0].NetMileage_KM.toString());
            $("#txtCOpenFuel").html(data[0].FromFuelBal.toString());
            $("#txtCCurrentFuel").html(data[0].CurrentFuelBal.toString());
            $("#lblCurrentFuel").html(data[0].CurrentFuelBal.toString());
            $("#txtCNetFuel").html(data[0].NetFuelBal.toString());
            $("#txtMaxSpeed").html(data[0].MaxSpeed.toString());
            $("#txtIdleHours").html(data[0].IdlHr.toString());
            $("#txtBattery").html(data[0].Batteryval.toString());


            guageMeter(fuelCapacity, data[0].CurrentFuelBal.toString());
        }
    });
}

function getAmpleViewLastEvent(assetObject) {
    $.ajax({
        url: urlAmpleViewLastEvent + "?assetNo=" + assetObject.name + "&unitid=" + assetObject.attributes["unitid"].value,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data, textStatus, xhr) {

            var table = document.getElementById("listLastEventStatus");
            while (table.hasChildNodes()) {
                table.removeChild(table.firstChild);
            }
            var row = table.insertRow(0);
            var cell1 = row.insertCell(0);
            var cell2 = row.insertCell(1);
            var cell3 = row.insertCell(2);
            var cell4 = row.insertCell(3);
            var cell5 = row.insertCell(4);
            var cell6 = row.insertCell(5);
            var cell7 = row.insertCell(6);
            var cell8 = row.insertCell(7);
            var cell9 = row.insertCell(8);


            cell1.innerHTML = "<strong>Event #</strong>";
            cell2.innerHTML = "<strong>Event</strong> ";
            cell3.innerHTML = "<strong>Event Start</strong>";
            cell4.innerHTML = "<strong>Event Stop</strong>";
            cell5.innerHTML = "<strong>Duration</strong>";
            cell6.innerHTML = "<strong>Idle Hours</strong>";
            cell7.innerHTML = "<strong>Total Mileage</strong>";
            cell8.innerHTML = "<strong>Avg Consumption (L/Km)</strong>";
            cell9.innerHTML = "<strong>Fuel (L)</strong>";


            // cell1.border = "10px";

            $(data).each(function () {
                var row = table.insertRow(table.rows.length);
                var cell1 = row.insertCell(0);
                var cell2 = row.insertCell(1);
                var cell3 = row.insertCell(2);
                var cell4 = row.insertCell(3);
                var cell5 = row.insertCell(4);
                var cell6 = row.insertCell(5);
                var cell7 = row.insertCell(6);
                var cell8 = row.insertCell(7);
                var cell9 = row.insertCell(8);


                cell1.innerHTML = this.Id;
                cell2.innerHTML = this.Activity;
                cell3.innerHTML = Date.parse(this.Initiate).toString("dd-MM-yyyy HH:mm:ss");
                cell4.innerHTML = Date.parse(this.Stopdt).toString("dd-MM-yyyy HH:mm:ss");
                cell5.innerHTML = this.totDuration;
                cell6.innerHTML = this.IDLHr;
                cell7.innerHTML = this.TotalMileage;
                cell8.innerHTML = this.AtcComKM;
                cell9.innerHTML = this.NetQty;

                cell1.align = "center";
                cell2.align = "left";
                cell3.align = "center";
                cell4.align = "center";
                cell5.align = "center";
                cell6.align = "center";
                cell7.align = "right";
                cell8.align = "right";
                cell9.align = "right";
            });
        }
    });
}

function getAmpleViewMonthly(assetObject) {
    $.ajax({
        url: urlAmpleViewMonthly + "?assetNo=" + assetObject.name,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data, textStatus, xhr) {
            $("#txtMOpenFuel").html(data[0].Opening_Fuel.toString() + " Ltrs");
            $("#txtMOpenMileage").html(data[0].OpenMile.toString());
            $("#txtTotalRefuel").html(data[0].Refuel.toString() + " Ltrs");
            $("#txtTotalTheft").html(data[0].Theft.toString() + " Ltrs");
            $("#txtTotalConsumed").html(data[0].Consume.toString() + " Ltrs");
            $("#txtTotalRunHours").html(data[0].totRuninghr);
            var currentMileage = $("#tdCurrentMileage").html();
            var openMileage = $("#txtMOpenMileage").html();
            if (currentMileage - openMileage == "0") {
                $("#txtMTotalMileage").html("0");
            } else {
                $("#txtMTotalMileage").html((currentMileage - openMileage).toFixed(1));
            }
            $("#txtMTotalKM").html(data[0].TotalKM.toString());
            
            //var totalMileage = $("#txtMTotalMileage").html();
            //if (totalMileage == 0) {
            //    $("#txtAvConM").html("0");
            //    $("#txtAvConK").html("0");
            //} else {
            //    var AvCon = data[0].Consume / //consumedFuel / totalMileage;
            //        $("#txtAvConM").html((AvCon).toFixed(2) + " Mi/Ltrs");

            //    var avConM = AvCon.toFixed(2) / 1.609344; // $("#txtAvConM").html();
            //    avConM = avConM.toFixed(2);
            //    $("#txtAvConK").html(avConM + " KM/Ltrs");
            //}
            $("#txtAvgkmltr").html(data[0].Avgkmltr.toString() + " KM/Ltrs");
            
            $("#txtTotalIdleHours").html(data[0].IdleHours);
        }
    });
}

function getAmpleViewFleetInfo(assetObject) {
    $.ajax({
        url: urlAmpleViewFleetInfo + "?assetNo=" + assetObject.name,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data, textStatus, xhr) {
//            var bindData = { view: data };
////            var template = document.getElementById('basicInfo').innerHTML;
////            var output = Mustache.render(template, data);
////            document.getElementById('basicInfo').innerHTML = output;
//            $("#basicInfo").load("template.html #template", function() {
//                var template = document.getElementById('basicInfo').innerHTML;
//                var output = Mustache.render(template, data);
//                $("#basicInfo").html(output);
//            });
            
            $("#txtFleetId").html(data[0].Id.toString());
            $("#txtDepot").html(data[0].SiteName.toString());
            $("#txtUsage").html(data[0].UnitUsageDesc.toString());
            $("#txtType").html(data[0].UnitTypeDesc.toString());
            $("#txtRegDate").html(data[0].RegDt.toString());
            $("#txtDriverName").html(data[0].DriverName.toString());
            $("#txtCellNo").html(data[0].DriverCellNo.toString());
        }
    });
}


function getSensorBoard(assetObject) {
    $.ajax({
        url: urlSensorBoard + "?assetNo=" + assetObject.name,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data, textStatus, xhr) {
            var table = document.getElementById("listSensorBoardStatus");
            while (table.hasChildNodes()) {
                table.removeChild(table.firstChild);
            }
            var row = table.insertRow(0);
            var cell1 = row.insertCell(0);
            var cell2 = row.insertCell(1);
            var cell3 = row.insertCell(2);


            cell1.innerHTML = "ID";
            cell2.innerHTML = "Digital Input";
            cell3.innerHTML = "Status";



            $(data).each(function () {
                var row = table.insertRow(table.rows.length);
                var cell1 = row.insertCell(0);
                var cell2 = row.insertCell(1);
                var cell3 = row.insertCell(2);



                cell1.innerHTML = this.Id;
                cell2.innerHTML = this.InputValues;
                cell3.innerHTML = this.ActualValue;

            });
        }
    });
}

function getImageSlider(assetObject) {
    $.ajax({
        url: urlAssetsImage + "?assetNo=" + assetObject.name,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data) {
            console.log(data);
            //Galleria.run('#galleria');
            //var myAnchor = { img: data };
            //var output1 = Mustache.render("<a href={{UnitImage}}> <img id={{Id}} src={{UnitImage}}/> </a>", data);
            //var output1 = Mustache.render("<a href={{UnitImage}}> {{UnitImage}}</a>", data);
            

        }
    });
}


function guageMeter(maxVal, currentVal) {
    var opts = {
        lines: 12, // The number of lines to draw
        angle: 0, // The length of each line
        lineWidth: 0.10, // The line thickness
        pointer: {
            length: 0.81, // The radius of the inner circle
            strokeWidth: 0.051, // The rotation offset
            color: '#000000' // Fill color
        },
        limitMax: 'false',   // If true, the pointer will not go past the end of the gauge

        colorStart: '#FF0000', //'#6FADCF',   // Colors
        colorStop: '#61b314',    // just experiment with them
        strokeColor: '#FF0000', //'#E0E0E0',   // to see which ones work best for you
        generateGradient: true
    };
    var target = document.getElementById('fuelometerGuage1'); //$("#fuelometerGuage"); // your canvas element
    var gauge = new Gauge(target).setOptions(opts); // create sexy gauge!
    gauge.maxValue = parseInt(maxVal); // set max gauge value
    gauge.animationSpeed = 32; // set animation speed (32 is default value)
    gauge.set(parseInt(currentVal)); // set actual value
}

function CurrentStatus(assetObject) {
    $.ajax({
        url: urlCurrentStatus + "?assetNo=" + assetObject.name,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data, textStatus, xhr) {
            $("#tdBatteryLabel").html("Battery");
            $("#tdBatteryVal").html(data[0].Battery.toString() + " V");
            $("#tdDateLabel").html("Data Log");
            $("#tdDateVal").html(data[0].Rtcdttm.toString());
            $("#tdDeviceLabel").html("Device #");
            $("#tdDeviceNo").html(data[0].UnitId.toString());
            $("#tdFuelQtyLabel").html("Fuel Available");
            $("#tdFuelQty").html(data[0].FuelQuantity.toString() + " L");
            $("#tdIgnitionLabel").html("Ignition");
            if (data[0].Ignition.toString() == "0") {
                $("#tdIgnitionStatus").html("Off");
            } else {
                $("#tdIgnitionStatus").html("On");
            }

            $("#tdCompressorLabel").html("Refrig. Compressor");
            if (data[0].Compressor.toString() == "0") {
                $("#tdCompressorStatus").html("Off");
            } else {
                $("#tdCompressorStatus").html("On");
            }

            $("#tdLatiLabel").html("Latitude");
            var myLati = data[0].Lati.toFixed(4);
            $("#tdLatiVal").html(myLati.toString());
            //$("#tdLocationLabel").html("Location");
            //$("#tdLocationVal").html(data[0].DriverCellNo.toString());
            $("#tdLongiLabel").html("Longitude");
            var myLongi = data[0].Longi.toFixed(4);
            $("#tdLongiVal").html(myLongi.toString());
            $("#tdSpeedLabel").html("Speed");
            $("#tdSpeedVal").html(data[0].Speed.toString() + " Mi/Hr");
            $("#tdTempLabel").html("Refrig. Temperature");
            $("#tdTempVal").html(data[0].Temperature.toString() + " C");

            $("#tdAssetNoLabel").html("Asset #");
            $("#tdAssetNo").html(data[0].Id.toString());
            $("#tdAssetNameLabel").html("Name");
            $("#tdAssetNameVal").html(data[0].AssetName.toString());
            $("#tdDepotLabel").html("Depot");
            if (data[0].Depot != null) $("#tdDepotName").html(data[0].Depot.toString());
            $("#tdUsageDescLabel").html("Usage");
            $("#tdUsageDescVal").html(data[0].UnitUsageDesc.toString());
            $("#tdUsageTypeLabel").html("Type");
            $("#tdUsageTypeVal").html(data[0].UnitTypeDesc.toString());
            $("#tdRegDateLabel").html("Registration Date");
            $("#tdRegDateVal").html(data[0].RegDt.toString());
            $("#tdDriverNameLabel").html("Driver Name");
            $("#tdDriverNameVal").html("N/A");
            $("#tdCellLabel").html("Cell #");
            $("#tdCellVal").html("N/A");

            $("#tdEventNo").html(data[0].EventNo.toString());
            $("#tdActivity").html(data[0].Activity.toString());
            if (data[0].SiteName == null) {
                $("#tdSite").html("N/A");
            } else {
                $("#tdSite").html(data[0].SiteName.toString());
            }
            $("#tdEventStart").html(Date.parse(data[0].EventStart.toString()).toString("dd-MM-yyyy HH:mm:ss"));
            $("#tdEventEnd").html(Date.parse(data[0].EventEnd.toString()).toString("dd-MM-yyyy HH:mm:ss"));
            if (data[0].TotalDuration == null) {
                $("#tdTotalDuration").html("00:00:00");
            } else {
                $("#tdTotalDuration").html(data[0].TotalDuration.toString());
            }

            $("#tdFromMilege").html(data[0].FromMileage.toString());
            $("#tdCurrentMileage").html(data[0].CurrentMileage.toString());
            $("#tdNetMileage").html(data[0].NetMileage.toString());
            $("#tdNetMileInKM").html(data[0].NetMileageInKm.toString());
            $("#tdNetMileInKM").html(data[0].NetMileageInKm.toString());
            $("#tdOpenFuel").html(data[0].FuelOpenBal.toString() + " L");
            $("#tdCurrentFuel").html(data[0].CurrentFuelBal.toString() + " L");
            $("#tdNetFuel").html(data[0].NetFuelBal.toString() + " L");
            $("#preview-textfield").html("Available: " + data[0].FuelQuantity.toString() + " / " + data[0].TotalCapacity.toString() + " L");
            guageMeter(data[0].TotalCapacity.toString(), data[0].FuelQuantity.toString());


            //$("#tdEventNo").html("N/A");
        }
    });
}

function pCurrentStatus(assetObject) {
    $.ajax({
        url: urlCurrentStatus + "?assetNo=" + assetObject.name,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function(data, textStatus, xhr) {
            $("#tdpBatteryLabel").html("Battery");
            $("#tdpBatteryVal").html(data[0].Battery.toString() + " V");
            $("#tdpDateLabel").html("Data Log");
            $("#tdpDateVal").html(data[0].Rtcdttm.toString());
            $("#tdpDeviceLabel").html("Device #");
            $("#tdpDeviceNo").html(data[0].UnitId.toString());
            $("#tdpFuelQtyLabel").html("Fuel Available");
            $("#tdpFuelQty").html(data[0].FuelQuantity.toString() + " L");
            $("#tdpIgnitionLabel").html("Ignition");
            if (data[0].Ignition.toString() == "0") {
                $("#tdpIgnitionStatus").html("Off");
            } else {
                $("#tdpIgnitionStatus").html("On");
            }

            $("#tdpCompressorLabel").html("Refrig. Compressor");
            if (data[0].Compressor.toString() == "0") {
                $("#tdpCompressorStatus").html("Off");
            } else {
                $("#tdpCompressorStatus").html("On");
            }

            $("#tdpLatiLabel").html("Latitude");
            var myLati = data[0].Lati.toFixed(4);
            $("#tdpLatiVal").html(myLati.toString());
            //$("#tdLocationLabel").html("Location");
            //$("#tdLocationVal").html(data[0].DriverCellNo.toString());
            $("#tdpLongiLabel").html("Longitude");
            var myLongi = data[0].Longi.toFixed(4);
            $("#tdpLongiVal").html(myLongi.toString());
            $("#tdpSpeedLabel").html("Speed");
            $("#tdpSpeedVal").html(data[0].Speed.toString() + " Mi/Hr");
            $("#tdpTempLabel").html("Refrig. Temperature");
            $("#tdpTempVal").html(data[0].Temperature.toString() + " C");
        }
    });
}