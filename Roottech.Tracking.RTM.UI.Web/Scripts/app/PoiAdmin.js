function getPoiByPoiNo(poiNo) { // Not yet used suppose to be remove
    var latlng = '';
    $.ajax({
        url: urlGetPoiByPoiNo + "?PoiNo=" + poiNo,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data, textStatus, xhr) {
            if (textStatus === "success") {
                if (!data.length) {
                    alert("There is no poi by poi # " + poiNo);
                    return;
                }
                drawPoiOnMap(data[0].Lati, data[0].Longi, data[0].Id, data[0].PoiType.GImage.ImgPath);
                //drawPolylineForRoute(latlng);
            }
        }
    });
    return latlng;
}

function drawPoiOnMap(lat, lng, poi, markerImagePath) {
    var latLng = new g.LatLng(lat, lng);
    var infoWindow = 'Latitude: ' + latLng.lat() + '  Longitude :' + latLng.lng() + 'POI # :' + poi;
    var markerImage = poiMarkerImage(markerImagePath);
    addMarkerForResource(latLng, infoWindow, false, false, undefined, undefined, markerImage);
}

function clearPoiAdmin() {
    $("#selCompanyByOrg").hide();
    $("#selPOIType option:selected").prop("selected", false);
    $("#showPoiManagement").hide();
}

function loadOrgByUser() {
    $("#listPoiAdmin").GridUnload();
    $("#selOrganizationByUser").off("change");
    $("#selOrganizationByUser").change(function() {
        if ($(this).val() != "")
            loadCompaniesByOrg($(this).val());
    });

    $.get(urlGetOrganizationsByUserCode, function (Organizations) {
        if (Organizations.length == 0) {
            alert("There is no organization assigned to your user. Please contact adminstrator.");
            return;
        }
        var output = "<option value=''>Select Organization</option>";
        $(Organizations).each(function() {
            output += "<option value='" + this.Id + "'>" + this.Description + "</option>";
        });
        $("#selOrganizationByUser").html(output);
        $("#poiAdmin").dialog("open");
        if (Organizations.length == 1) {
            /// One thing left if there are multiple companies in only organization then what
            $("#selOrganizationByUser").val($("#selOrganizationByUser option:eq(1)").val());
            loadCompaniesByOrg($("#selOrganizationByUser").val());
        }
    });
}

function loadCompaniesByOrg(orgCode) {
    $.get(urlGetCompaniesbyOrgCode, { orgCode: orgCode }, function(companies) {
        if (companies.length > 0) {
            var output = "<option value=''>Select Company</option>";
            $(companies).each(function() {
                output += "<option value='" + this.Id + "'>" + this.CompanyName + "</option>";
                //output += Mustache.render('<option value="{{Id}}">{{CompanyName}}</option>', this);
            });
            $("#selCompanyByOrg").html(output);
            $("#selCompanyByOrg").show();

            $("#showPoiManagement").show();
            $("#showPoiManagement").off('click');
            $("#showPoiManagement").click(function() {
                if ($("#selOrganizationByUser").val() == "") {
                    alert("Please select organization");
                    $("#selOrganizationByUser").focus();
                    return false;
                }
                loadPoiAdminGrid($("#selOrganizationByUser").val(), $("#selCompanyByOrg").val());
            });
        } else {
            alert("There is no company in selected Organization.");
            $("#selCompanyByOrg").hide();
            $("#showPoiManagement").hide();
        }
    });
}

function loadPOIType() {
    $.get(urlGetPoiTypes, function(PoiType) {
        if (PoiType.length > 0) {
            var output = "<option value=''>Select POI Type</option>";
            $(PoiType).each(function() {
                output += "<option value='" + this.Id + "'>" + this.TypeName + "</option>";
                //output += Mustache.render('<option value="{{Id}}">{{CompanyName}}</option>', this);
                poiTypes[this.Id] = this.TypeName;
            });
            $("#selPOIType").html(output);
        } else {
            alert("There is no POI Type .");
            $("#showPoiManagement").hide();
        }
    });
}

function customElement(value, options) {
    var el = document.createElement("div");

    var input = document.createElement("input");
    input.type = "text";
    input.value = value;
    input.className = "FormElement ui-widget-content ui-corner-all";
    input.role = "textbox";
    el.appendChild(input);

    input = document.createElement("input");
    input.type = "button";
    input.value = "Get Lat/Long from Map";
    input.onclick = function(e) {
        //console.log($(e.target).parent().children(":nth(0)"));
        $("#editmodlistPoiAdmin").hide();
        $("#gbox_listPoiAdmin .ui-jqgrid-titlebar-close.HeaderButton .ui-icon.ui-icon-circle-triangle-n").parent().click();
        $(".ui-widget-overlay:not(#lui_listPoiAdmin)").hide();

        var mapClickEvent = g.event.addListener(map, 'click', function(event) {
            $("#editmodlistPoiAdmin #Lati").val(event.latLng.lat());
            $("#editmodlistPoiAdmin #Longi").children(":nth(0)").val(event.latLng.lng());
            $(".ui-widget-overlay:not(#lui_listPoiAdmin)").show();
            $("#gbox_listPoiAdmin .ui-jqgrid-titlebar-close.HeaderButton .ui-icon.ui-icon-circle-triangle-s").parent().click();
            $("#editmodlistPoiAdmin").show();
            g.event.removeListener(mapClickEvent);
        });
    };
    el.appendChild(input);

    return el;
}

function customValue(elem, operation, value) {
    if (operation === 'get') {
        return $("input[type=text]", elem).val();
    } else if (operation === 'set') {
        $("input[type=text]", elem).val(value);
    }
}

function getColumnIndexByName (grid, columnName) {
    var cm = grid.jqGrid('getGridParam', 'colModel');
    var l = cm.length;
    for (var i = 0; i < l; i++)
        if (cm[i].name === columnName)
            return i; // return the index
    return -1;
}

function loadPoiAdminGrid(orgCode, companyCode) {
    var delOptions = {
            url: urlDeletePoi,
            modal: true,
            resize: false,
            drag: false,
            closeOnEscape: true,
            caption: 'Delete Existing POI',
            msg: '<br />&nbsp;Are you sure to delete the<br />&nbsp;selected POI?',
            bSubmit: 'Yes',
            bCancel: 'No',
            onclickSubmit: function(options, formid) {
                options.url += "/" + formid;
                options.mtype = "DELETE";
            },
            beforeSubmit: function(postdata, formid) {
                unRegisterOverlay(parseInt($grid.jqGrid('getCell', postdata, 'MarkerIndex')));
                return [true, ""];
            }
        },
        editOptions = {
//Edit //http://techbrij.com/add-edit-delete-jqgrid-asp-net-web-api
            url: urlEditPoi,
            closeAfterEdit: true,
            closeOnEscape: true,
            bSubmit: "Update",
            modal: true,
            width: 560,
            beforeShowForm: function(formid) {
                $(formid).find("#Block_Code").tokenInput("clear");
                var blockCode = $grid.getRowData(formid.find("#id_g").val()).Block_Code;
                $.ajax({
                    type: 'GET',
                    url: urlGetCompleteLocationByOrgCodeAndBlockCode,
                    data: { orgCode: orgCode, blockCode: blockCode },
                    success: function(location) {
                        $(formid).find("#Block_Code").tokenInput("add", { id: blockCode, name: location });
                    },
                    async: false
                });
            },
            onclickSubmit: function(options, postData) {
                if (options.url.indexOf("/" + postData.Id) == -1)
                    options.url += "/" + postData.Id; //$grid.getRowData($grid.getGridParam("selrow")).Id;
                options.mtype = "PUT";
            },
            beforeSubmit: function(postdata, formid) {
                //more validations
                if (postdata.GF_YN == "Y" && postdata.GF_Val < 1)
                    return [false, 'Buffer Distance (Meters) should be greater than or equals to 1']; //error

                //console.log(postdata);
                return [true, ""]; // no error
            }
        },
        colModel = [
            {
                name: 'act',
                index: 'act',
                width: 95,
                align: "center",
                sortable: false,
                editable: false,
                formatter: 'actions',
                formatoptions: //http://www.trirand.com/jqgridwiki/doku.php?id=wiki:predefined_formatter
                {
                    keys: true,
                    editformbutton: true,
                    editbutton: false,
                    delOptions: delOptions,
                    editOptions: editOptions
                }
            },
            {
                name: "Id",
                index: "Id",
                key: true,
                editable: true,
                hidden: true,
                sorttype: "int",
                editrules: { edithidden: false },
                editoptions: { dataInit: function(element) { $(element).attr("readonly", "readonly"); } }
            },
            //{ name: "select", index: "", width: 40, align: "center", edittype: "checkbox", formatter: "checkbox", editable: false, formatoptions: { disabled: false } }, //editoptions: { value: "True:False" }, 
            { name: "PoiName", index: "PoiName", width: 300, align: "left", sorttype: "string", editable: true, editoptions: { size: 50, maxlength: 50 }, editrules: { required: true } },
            { name: "PoiTypeNo", index: "PoiTypeNo", editable: true, hidden: true, edittype: "select", editrules: { edithidden: true } },
            { name: "TypeName", index: "TypeName", editable: false },
            { name: "PostalAddr", index: "PostalAddr", editable: true, hidden: true, editrules: { edithidden: true, required: true }, editoptions: { size: 50, maxlength: 100 } },
            {
                name: "Block_Code",
                index: "Block_Code",
                editable: true,
                hidden: true,
                editrules: { edithidden: true, required: true },
                editoptions: {
                    dataInit: function(elem) {
                        //$(elem).val();
                        $(elem).tokenInput(urlGetCompleteLocationsByOrgCode + orgCode,
                        {
                            //tokenValue: "Id", propertyToSearch: "Name",
                            minChars: 4,
                            tokenLimit: 1 /*,
                                        resultsFormatter: function(item) {
                                            return "<li id='" + item.id + "'>" + item.name + "_" + item.id + "</li>";
                                        },
                                        prePopulate: [
                                            { id: value, name : "" },
                                        ]*/
                        });
                    }
                }
            },
            { name: "Lati", index: "Lati", width: 110, template: floatTemplate, editable: true, editrules: { required: true } },
            {
                name: "Longi",
                index: "Longi",
                width: 110,
                template: floatTemplate,
                editable: true,
                editrules: { required: true },
                edittype: "custom",
                editoptions: { custom_element: customElement, custom_value: customValue }
            },
            {
                name: "GF_YN",
                index: "GF_YN",
                width: 70,
                template: checkboxTemplate,
                searchoptions: { value: ":Select;Y:Yes;N:No", sopt: ["eq", "ne", "nu", "nn"] },
                editoptions: {
                    value: "Y:N",
                    dataEvents: [
                        {
                            type: 'change',
                            fn: function(e) {
                                $grid.jqGrid("setColProp", "GF_Val", {
                                    editrules: { required: ($(e.target)[0].checked) }
                                });
                            }
                        }
                    ]
                },
                editable: true,
                edittype: "checkbox"
            },
            {
                name: "GF_Val",
                index: "GF_Val",
                width: 70,
                template: integerTemplate,
                editable: true,
                hidden: true,
                editrules: { edithidden: true, number: true }

            },
            { name: "CustomerRefNo", index: "CustomerRefNo", width: 100, align: "left", sorttype: "string", editable: true, hidden: true, editrules: { edithidden: true }, editoptions: { size: 20, maxlength: 20 } },
            { name: "ContactPerson", index: "ContactPerson", width: 100, align: "left", sorttype: "string", editable: true, hidden: true, editrules: { edithidden: true }, editoptions: { size: 40, maxlength: 40 } },
            { name: "Phone", index: "Phone", width: 100, align: "left", sorttype: "string", editable: true, hidden: true, editrules: { edithidden: true }, editoptions: { size: 20, maxlength: 20 } },
            { name: "Cell", index: "Cell", width: 100, align: "left", sorttype: "string", editable: true, hidden: true, editrules: { edithidden: true }, editoptions: { size: 20, maxlength: 20 } },
            { name: "Email", index: "Email", width: 200, align: "left", sorttype: "string", editable: true, hidden: true, editrules: { edithidden: true, email: true, required: false }, editoptions: { size: 50, maxlength: 100} },
            { name: "MarkerIndex", index: "MarkerIndex", width: 1, sortable: false, editable: false, hidden: true, search: false, view: false }
        ];

    loadjqGrid(
        [], "listPoiAdmin", urlGetPois, { orgCode: orgCode, companyCode: companyCode },
        [
            "Actions", "Code", "POI Name", "POI Type Code", "POI Type", //"OrgCode", "Company Code", 
            "Address", "Location Code", //"AreaId", "CityId", "StateId", "CountryId", 
            "Latitude", "Longitude", "Buffer", "Buffer Distance (Meters)",
            "Customer Name", "Contact Person", "Phone", "Cell", "Email", "MarkerIndex"
        ], colModel,
        "pagerPoiAdmin", "Id", "asc", "", 50, _rowList, 1, 1, "POI Managment",
        {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            userdata: "userdata"
        }, undefined, undefined, false, false, function() {
            //-/http://stackoverflow.com/questions/11743983/adding-a-custom-button-in-row-in-jqgrid
            $grid.find(">tbody>tr.jqgrow>td:nth-child(" + (getColumnIndexByName($grid, 'act') + 1) + ")")
                .each(function () {
                    drawButtonPointToMap($(this), $grid);
                    drawButtonForPoiImageDetails($(this), $grid);
                });

            $grid.setGridWidth(750);
            $grid.setGridHeight($(window).height() - 200);
            $(".ui-jqgrid-active").css("left", 350).css("bottom", 100);
            $("#pagerPoiAdmin_left").removeAttr("style");
            $("#" + $grid.attr("aria-labelledby")).show();
            $grid.setColProp("PoiTypeNo", { editoptions: { value: poiTypes } });
            //$grid.setColProp("Block_Code", { editoptions: { value: locations} });

            if ($grid.getGridParam("userData").length > 0)
                $($grid.getGridParam("userData")).each(function () {
                    users[this.Id] = this.Name;
                });
            else
                alert("There are no Users in this organization.");

            //-----Close Selection of Org
            $("#poiAdmin").dialog("close");
        }, undefined, //Edit
        {
            url: urlAddPoi,
            closeAfterAdd: true,
            closeOnEscape: true,
            bSubmit: "Add",
            modal: true,
            width: 500,
            beforeShowForm: function(formid) {
                $(formid).find("#Block_Code").tokenInput("clear");
                $('#tr_Id', formid).hide();
                //var longiObject = $(formid).find("#Longi").parent().parent();
                //longiObject.html(longiObject.html().replace("&nbsp;", ""));
            },
            onclickSubmit: function(params) {
                //params.url += "/" + $grid.getRowData($grid.getGridParam("selrow")).Id;
                params.mtype = "POST";
            },
            beforeSubmit: function(postdata, formid) {
                postdata.OrgCode = $("#selOrganizationByUser").val();
                postdata.Company_Code = $("#selCompanyByOrg").val();

                //more validations
                if (postdata.GF_YN === "Y" && postdata.GF_Val < 1)
                    return [false, 'Buffer Distance (Meters) should be greater than or equals to 1']; //error
                return [true, ""]; // no error
            }
        }, //Add
        undefined, //Delete 
        true, { groupField: ["TypeName"], groupColumnShow: [false] }, undefined, undefined, undefined, undefined, undefined, undefined, undefined,
        false, undefined, // use the onSelectRow that is triggered on row click to show a details grid, 
        function () { clearMapFromSelectedMarkersInGrid(); }, // Before Request event of grid
        function () { clearMapFromSelectedMarkersInGrid(); }, ";Edit POI;;Add POI;;Delete POI;;Find POI;;Refresh;Notice;Please select POI;;"); // Before closing grid event of grid
}

function drawButtonPointToMap(col, grid) {
    $("<div>", {
                title: "Display on Map",
                mouseover: function() {
                    col.addClass('ui-state-hover');
                },
                mouseout: function() {
                    col.removeClass('ui-state-hover');
                },
                click: function(e) {
                    //-/Show marker on map
                    var id = $(e.target).closest("tr.jqgrow").attr("id");
                    $(e.target).toggleClass("ui-icon-image").toggleClass("ui-icon-closethick");
                    if ($(e.target).hasClass("ui-icon-image")) {
                        unRegisterOverlay(parseInt(grid.jqGrid('getCell', id, 'MarkerIndex')));
                        return;
                    }
                    var latLng = new g.LatLng(grid.jqGrid('getCell', id, 'Lati'), grid.jqGrid('getCell', id, 'Longi'));
                    $.get(urlGetGImagePathByPoiTypeNo, { poiTypeNo: grid.jqGrid('getCell', id, 'PoiTypeNo') }, function(imagePath) {
                        var markerImage = poiMarkerImage(imagePath);
                        var marker = addMarkerForResource(latLng, undefined, false, false, grid.jqGrid('getCell', id, 'MarkerIndex') == "" ? undefined : grid.jqGrid('getCell', id, 'MarkerIndex'), undefined, markerImage);

                        //drawing buffer circle around point of interest
                        if (grid.jqGrid('getCell', id, 'GF_YN') === "Y" && parseInt(grid.jqGrid('getCell', id, 'GF_Val')) > 0) {
                            var j = overlaysArray.marker.indexOf(marker);
                            var circle = new g.Circle({
                                strokeColor: '#FF0000',
                                strokeOpacity: 0.8,
                                strokeWeight: 2,
                                fillColor: '#FF0000',
                                fillOpacity: 0.35,
                                map: map,
                                center: latLng,
                                radius: parseInt(grid.jqGrid('getCell', id, 'GF_Val'))
                            });
                            overlaysArray.circle[j] = circle;

                            g.event.addListener(circle, "rightclick", function(mouseEvent) {
                                unRegisterOverlay(overlaysArray.circle.indexOf(circle));
                            });
                        }
                        grid.jqGrid('setCell', id, 'MarkerIndex', overlaysArray.marker.indexOf(marker));
                        map.setCenter(latLng);
                        $("#gbox_listPoiAdmin .ui-jqgrid-titlebar-close.HeaderButton .ui-icon.ui-icon-circle-triangle-n").parent().click();

                        //create empty LatLngBounds object
                        var bounds = new g.LatLngBounds();
                        //extend the bounds to include each marker's position
                        var rows = grid.getDataIDs(), row;
                        if (rows)
                            for (var i = 0; i < rows.length; i++) {
                                row = grid.getRowData(rows[i]);
                                if (row.MarkerIndex && overlaysArray.marker[parseInt(row.MarkerIndex)] != null)
                                    bounds.extend(overlaysArray.marker[parseInt(row.MarkerIndex)].position);
                            }
                        //now fit the map to the newly inclusive bounds
                        map.fitBounds(bounds);
                        //(optional) restore the zoom level after the map is done scaling
                        var listener = g.event.addListener(map, "idle", function() {
                            if (map.getZoom() > zoom) map.setZoom(zoom);
                            g.event.removeListener(listener);
                        });
                    });
                    //-/Show marker on map
                }
            }
        ).css({ "margin-right": "5px", "float": "left", "cursor": "pointer" })
        .addClass("ui-pg-div ui-inline-custom")
        .append('<span class="ui-icon ui-icon-image"></span>')
        .prependTo(col.children("div"));
}

function drawButtonForPoiImageDetails(col, grid) {
    $("<div>", {
        title: 'Poi Image::' + col.parent().prop('id'),
        mouseover: function () {
            col.addClass('ui-state-hover');
        },
        mouseout: function () {
            col.removeClass('ui-state-hover');
        },
        click: function (e) {
            var id = $(e.target).closest("tr.jqgrow").attr("id");
            $(e.target).toggleClass("ui-icon-video").toggleClass("ui-icon-closethick");
            if ($(e.target).hasClass("ui-icon-video")) {
                $("#listPoiImage").GridUnload();
                $("div.poiImage").dialog("close");
                return;
            }
            if ($("#gbox_listPoiImage")[0] === undefined) {
                var delOptions = {
                        url: urlDeletePoiImage,
                        modal: true,
                        resize: false,
                        drag: false,
                        closeOnEscape: true,
                        caption: 'Delete Existing POI Image',
                        msg: '<br />&nbsp;Are you sure to delete the<br />&nbsp;selected POI Image?',
                        bSubmit: 'Yes',
                        bCancel: 'No',
                        onclickSubmit: function(options, formid) {
                            options.url += "/" + formid;
                            options.mtype = "DELETE";
                        },
                        beforeSubmit: function(postdata, formid) {
                            //unRegisterOverlay(parseInt($grid.jqGrid('getCell', postdata, 'MarkerIndex')));
                            return [true, ""];
                        }
                    },
                    editOptions = {
                        //Edit //http://techbrij.com/add-edit-delete-jqgrid-asp-net-web-api
                        url: urlEditPoiImage,
                        closeAfterEdit: true,
                        closeOnEscape: true,
                        bSubmit: "Update",
                        modal: true,
                        width: 560,
                        beforeShowForm: function(formid) {

/*                            $(formid).find("#Block_Code").tokenInput("clear");
                            var blockCode = $grid.getRowData(formid.find("#id_g").val()).Block_Code;
                            $.ajax({
                                type: 'GET',
                                url: urlGetCompleteLocationByOrgCodeAndBlockCode,
                                data: { orgCode: orgCode, blockCode: blockCode },
                                success: function(location) {
                                    $(formid).find("#Block_Code").tokenInput("add", { id: blockCode, name: location });
                                },
                                async: false
                            });*/
                        },
                        onclickSubmit: function(options, postData) {
                            if (options.url.indexOf("/" + postData.Id) === -1)
                                options.url += "/" + postData.Id; //$grid.getRowData($grid.getGridParam("selrow")).Id;
                            options.mtype = "PUT";
                        },
                        beforeSubmit: function (postdata, formid) {
                            console.log(postdata);
                            //more validations
                            if (!$("#ImagePath").val())
                                return [false, 'Please choose an image file.'];
                            postdata.PoiNo = grid.jqGrid('getCell', id, 'Id');
                            postdata.ImagePath = $("#ImagePath").val().substring($("#ImagePath").val().lastIndexOf("\\") + 1);

                            return [true, ""]; // no error
                        },
                        afterSubmit: UploadImage
                    };
                loadjqGrid([], "listPoiImage", urlGetPoiImagesByPoiNo, { poiNo: grid.jqGrid('getCell', id, 'Id') },
                    ["Actions", "Id", "Title", "Name", "Image Path", "For All Users", "Added By"], [//"AddedbyUserCode", 
                        {
                            name: 'act',
                            index: 'act',
                            width: 75,
                            align: "center",
                            sortable: false,
                            editable: false,
                            formatter: 'actions',
                            formatoptions: //http://www.trirand.com/jqgridwiki/doku.php?id=wiki:predefined_formatter
                            {
                                keys: true,
                                editformbutton: true,
                                editbutton: false,
                                delOptions: delOptions,
                                editOptions: editOptions
                            }
                        },
                        {
                            name: 'Id',
                            index: 'Id',
                            key: true,
                            editable: true,
                            hidden: true,
                            sorttype: "int",
                            editrules: { edithidden: false },
                            editoptions: { dataInit: function(element) { $(element).attr("readonly", "readonly"); } }
                        },
                        { name: 'Title', index: 'Title', width: 100, align: "left", sorttype: "string", editable: true, editoptions: { size: 30, maxlength: 50 }, editrules: { required: true } },
                        { name: 'Name', index: 'Name', width: 100, align: "left", sorttype: "string", editable: true, editoptions: { size: 50, maxlength: 100 }, editrules: { required: true } },
                        {
                            name: 'ImagePath', index: 'ImagePath', width: 100, align: "left", sorttype: "string", editable: true, editrules: { required: true },
                            editoptions: { size: 20, maxlength: 300, enctype: "multipart/form-data" }, edittype: 'file'//, formoptions: { elmsuffix: '(*)' }
                            , formatter:linkFormat//, unformat:linkUnFormat
                            //formatter: 'showlink', formatoptions: { baseLinkUrl: 'javascript:', showAction: "GetAndShowUserData($('div.poiImage'),'", addParam: "');"}
                        },
                        {
                            name: 'ForAll',
                            index: 'ForAll',
                            template: checkboxTemplate,
                            editable: true,
                            editoptions: {
                                value: "true:false",
                                dataEvents: [
                                    {
                                        type: 'change',
                                        fn: function(e) {
                                            $grid.jqGrid("setColProp", "ForAll", {
                                                editrules: { required: ($(e.target)[0].checked) }
                                            });
                                        }
                                    }
                                ]
                            },
                            edittype: "checkbox"
                        },
                        //{ name: 'AddedByUserCode', index: 'AddedByUserCode', hidden: true, edittype: "select", editable: false, editrules: { edithidden: false, required: false} },
                        { name: 'UserCode', index: 'UserCode', width: 180, align: 'left', sorttype: "string" }
                    ], "pagerPoiImage", "Id", "asc", "", 50, _rowList, 1, 1, "Poi Image::" + grid.jqGrid('getCell', id, 'Id'),
                    {
                        root: "rows",
                        page: "page",
                        total: "total",
                        records: "records",
                        repeatitems: false,
                        userdata: "userdata"
                    },
                    undefined, undefined, true, true, function () {
                        $grid.find(">tbody>tr.jqgrow>td:nth-child(" + (getColumnIndexByName($grid, 'act') + 1) + ")")
                            .each(function() {
                                drawButtonForPoiImageUserAccessDetails($(this), $grid);
                            });

                        $grid.setGridWidth(750);
                        $grid.setGridHeight($(window).height() - 200);
                        $(".ui-jqgrid-active").css("left", 350).css("bottom", 100);
                        $("#pagerPoiImage_left").removeAttr("style");
                        $("#" + $grid.attr("aria-labelledby")).show();
                        //$grid.setColProp("AddedByUserCode", { editoptions: { value: users } });
                    }, undefined, //Edit
                    {
                        url: urlAddPoiImage,
                        closeAfterAdd: true,
                        closeOnEscape: true,
                        bSubmit: "Add",
                        modal: true,
                        width: 500,
                        editData : { orgCode: $("#selOrganizationByUser").val(), companyCode: ($("#selCompanyByOrg").val() == null ? "" : $("#selCompanyByOrg").val()) },
                        beforeShowForm: function(formid) {
                            //$(formid).find("#Block_Code").tokenInput("clear");
                            $('#tr_Id', formid).hide();
                            $("#tr_ImagePath #ImagePath").attr({ accept: ".bmp, .gif, .png, .jpg, .jpeg" }); //"image/*"
                        },
                        onclickSubmit: function(params) {
                            params.mtype = "POST";
                        },
                        beforeSubmit: function (postdata, formid) {
                            //more validations
                            var validFilesTypes = ["bmp", "gif", "png", "jpg", "jpeg"];//, "doc", "xls"];

                            if (!$("#ImagePath").val())
                                return [false, 'Please choose an image file.'];

                            var path = $("#ImagePath")[0].files[0].name;
                            var ext = path.substring(path.lastIndexOf(".") + 1, path.length).toLowerCase();
                            var isValidFile = false;
                            for (var i = 0; i < validFilesTypes.length; i++) {
                                if (ext === validFilesTypes[i]) {
                                    isValidFile = true;
                                    break;
                                }
                            }
                            if (!isValidFile) 
                                return [false, "Invalid File. Please upload a File with" + " extension:\n\n" + validFilesTypes.join(", ")];

                            //else if ($("#ImagePath")[0].files[0].type.indexOf("image/") < 0)
                            //    return [false, 'Please choose an image file.'];

                            postdata.PoiNo = grid.jqGrid('getCell', id, 'Id');
                            postdata.ImagePath = $("#ImagePath").val().substring($("#ImagePath").val().lastIndexOf("\\") + 1);

                            return [true, ""]; // no error
                        },
                        afterSubmit: UploadImage
                    }, //Add
                    undefined //Delete
                    , undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined,
                    function () {
                        $(e.target).toggleClass("ui-icon-video").toggleClass("ui-icon-closethick");
                        $("div.poiImage").dialog("close");
                    }
                );
            } else {
                $("#listPoiImage").jqGrid('setGridParam', { url: urlGetPoiImagesByPoiNo, postData: { poiNo: grid.jqGrid('getCell', id, 'Id') }, datatype: 'json' });
                $("#listPoiImage").jqGrid('setCaption', 'Poi Image::' + grid.jqGrid('getCell', id, 'Id'));
                $("#listPoiImage").trigger("reloadGrid");
            }
        }
    }
    ).css({ "margin-right": "5px", "float": "left", "cursor": "pointer" })
    .addClass("ui-pg-div ui-inline-custom")
    .append('<span class="ui-icon ui-icon-video"></span>')
    .prependTo(col.children("div"));
}

function ValidateFile() {
    var file = document.getElementById("<%=FileUpload1.ClientID%>");
    var label = document.getElementById("<%=Label1.ClientID%>");
    var path = file.value;
    var ext = path.substring(path.lastIndexOf(".") + 1, path.length).toLowerCase();
    var isValidFile = false;
    for (var i = 0; i < validFilesTypes.length; i++) {
        if (ext === validFilesTypes[i]) {
            isValidFile = true;
            break;
        }
    }
    if (!isValidFile) {
        label.style.color = "red";
        label.innerHTML = "Invalid File. Please upload a File with" + " extension:\n\n" + validFilesTypes.join(", ");
    }
    return isValidFile;
}

function linkFormat(cellvalue, options, rowObject) {
    return "<a title='" + cellvalue + "' onclick='showImageInDiv(\"" + cellvalue + "\"); return false;' href='#'>" + cellvalue.substring(cellvalue.lastIndexOf("/") + 1) + "</a>";
}

function linkUnFormat(cellvalue, options, cell) {
    return $('a', cell).attr('title');
}

function showImageInDiv(cellvalue) {
    var img = $("div.poiImage img");
    img.attr({ src: cellvalue });
    img[0].onload = function () {
        if (img[0].height > img[0].width) {
            img.height('100%');
            img.width('auto');
        } else {
            img.height('auto');
            //img.width('100%');
        }
        if (img[0].height > $(window).height() -65)
            img.height($(window).height() - 65);
        $("div.poiImage").dialog({ title: cellvalue.substring(cellvalue.lastIndexOf("/") + 1) });
        $("div.poiImage").dialog("open");
    }
}

function UploadImage(response, postdata) {
    var data = $.parseJSON(response.responseText);
    if (response.status === 200)  //(data.success)
        if ($("#ImagePath").val() !== "")
            ajaxFileUpload(data.ImagePath.substring(data.ImagePath.lastIndexOf("\\") + 1));
    console.log($(this));
    $("#listPoiImage").jqGrid("setGridParam", { datatype: 'json' });
    return [true];//, data.message, data.id];
}

function ajaxFileUpload(fileName) {
    showLoadingBar($("#poiAdminLoader"), $("#editmodlistPoiImage"));
    $.ajaxFileUpload
    ({
        url: urlUploadFile,
        secureuri: false,
        fileElementId: 'ImagePath',
        dataType: 'json',
        data: { filePath: fileName },  //{ id: id },
        success: function (data, status) {
            $("#poiAdminLoader").fadeOut();
            if (typeof (data.success) !== 'undefined')
                if (data.success)
                    return true;
                else return alert(data.message);
            else
                return alert('Failed to upload file!');
        },
        error: function(data, status, e) {
            $("#poiAdminLoader").fadeOut();
            return alert('Failed to upload file!');
        }
    });
}

function drawButtonForPoiImageUserAccessDetails(col, grid) {
    $("<div>", {
        title: 'Poi Image User Access::' + col.parent().prop('id'),
        mouseover: function () {
            col.addClass('ui-state-hover');
        },
        mouseout: function () {
            col.removeClass('ui-state-hover');
        },
        click: function (e) {
            var id = $(e.target).closest("tr.jqgrow").attr("id");
            $(e.target).toggleClass("ui-icon-note").toggleClass("ui-icon-closethick");
            if ($(e.target).hasClass("ui-icon-note")) {
                $("#listPoiImageUserAccess").GridUnload();
                return;
            }
            if ($("#gbox_listPoiImageUserAccess")[0] === undefined) {
                var delOptions = {
                    url: urlDeletePoiImageAccess,
                    modal: true,
                    resize: false,
                    drag: false,
                    closeOnEscape: true,
                    caption: 'Delete Existing POI Image User Access',
                    msg: '<br />&nbsp;Are you sure to delete the<br />&nbsp;selected POI Image User Access?',
                    bSubmit: 'Yes',
                    bCancel: 'No',
                    onclickSubmit: function(options, formid) {
                        options.url += "/" + formid;
                        options.mtype = "DELETE";
                    },
                    beforeSubmit: function(postdata, formid) {
                        //unRegisterOverlay(parseInt($grid.jqGrid('getCell', postdata, 'MarkerIndex')));
                        return [true, ""];
                    }
                };
                loadjqGrid([], "listPoiImageUserAccess", urlGetPoiImageAccessesByPoiImageNo, { poiImageNo: grid.jqGrid('getCell', id, 'Id') },
                    ["Actions", "Id", "Access To User", "Access To User"], [
                        {
                            name: 'act',
                            index: 'act',
                            width: 75,
                            align: "center",
                            sortable: false,
                            editable: false,
                            formatter: 'actions',
                            formatoptions: //http://www.trirand.com/jqgridwiki/doku.php?id=wiki:predefined_formatter
                            {
                                keys: true,
                                editformbutton: false,
                                editbutton: false,
                                delOptions: delOptions
                            }
                        },
                        {
                            name: 'Id',
                            index: 'Id',
                            key: true,
                            editable: true,
                            hidden: true,
                            sorttype: "int",
                            editrules: { edithidden: false },
                            editoptions: { dataInit: function (element) { $(element).attr("readonly", "readonly"); } }
                        },
                        { name: 'UserCode', index: 'UserCode', hidden: true, edittype: "select", editable: true, editrules: { edithidden: true, required: true } },
                        { name: 'UserCodeName', index: 'UserCodeName', width: 180, align: 'left', sorttype: "string" }
                    ], "pagerPoiImageUserAccess", "Id", "asc", "", 50, _rowList, 1, 1, "Poi Image User Access::" + grid.jqGrid('getCell', id, 'Id'),
                    {
                        root: "rows",
                        page: "page",
                        total: "total",
                        records: "records",
                        repeatitems: false,
                        userdata: "userdata"
                    },
                    undefined, undefined, true, true, function () {
                        $grid.setGridWidth(350);
                        $grid.setGridHeight($(window).height() - 200);
                        $(".ui-jqgrid-active").css("left", 350).css("bottom", 100);
                        $("#pagerPoiImageUserAccess_left").removeAttr("style");
                        $("#" + $grid.attr("aria-labelledby")).show();
                        var usersExceptAddedByUser = {};
                        $.each(users, function (index, value) {
                            if ($("#listPoiImage").jqGrid('getCell', 66, 'UserCode').substring(0, $("#listPoiImage").jqGrid('getCell', 66, 'UserCode').indexOf(" -")) !== index)
                                usersExceptAddedByUser[parseInt(index)] = value;
                        });
                        $grid.setColProp("UserCode", { editoptions: { value: usersExceptAddedByUser } });
                    }, undefined, //Edit
                    {
                        url: urlAddPoiImageAccess,
                        closeAfterAdd: true,
                        closeOnEscape: true,
                        bSubmit: "Add",
                        modal: true,
                        width: 500,
                        //editData: { orgCode: $("#selOrganizationByUser").val(), companyCode: ($("#selCompanyByOrg").val() == null ? "" : $("#selCompanyByOrg").val()) },
                        beforeShowForm: function (formid) { $('#tr_Id', formid).hide(); },
                        onclickSubmit: function (params) { params.mtype = "POST"; },
                        beforeSubmit: function (postdata, formid) {
                            //more validations
                            var ids = $("#listPoiImageUserAccess").getDataIDs();
                            for (var i = 0; i < ids.length; i++) {
                                var row = $("#listPoiImageUserAccess").getRowData(ids[i]);
                                if (row.UserCode === postdata.UserCode)
                                    return [false, "User does exists already. Duplication is not allowed"]; // no error
                                //grid.editRow(ids[i], true);
                            };
                            if ($("#listPoiImage").jqGrid('getCell', 66, 'UserCode').substring(0, $("#listPoiImage").jqGrid('getCell', 66, 'UserCode').indexOf(" -")) === postdata.UserCode)
                                return [false, "This image is added by this user."]; // no error
                            postdata.PoiImageNo = grid.jqGrid('getCell', id, 'Id');
                            return [true, ""]; // no error
                        },
                        afterSubmit: function (response, postdata) {
                            $("#listPoiImageUserAccess").jqGrid("setGridParam", { datatype: 'json' });
                            return [true];
                        }
                    }, //Add
                    undefined //Delete
                    , undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined,
                    function () {
                        $(e.target).toggleClass("ui-icon-note").toggleClass("ui-icon-closethick");
                        $("div.poiImage").dialog("close");
                    }
                );
            } else {
                $("#listPoiImageUserAccess").jqGrid('setGridParam', { url: urlGetPoiImageAccessesByPoiImageNo, postData: { poiImageNo: grid.jqGrid('getCell', id, 'Id') }, datatype: 'json' });
                $("#listPoiImageUserAccess").jqGrid('setCaption', 'Poi Image User Access::' + grid.jqGrid('getCell', id, 'Id'));
                $("#listPoiImageUserAccess").trigger("reloadGrid");
            }
        }
    }
    ).css({ "margin-right": "5px", "float": "left", "cursor": "pointer" })
    .addClass("ui-pg-div ui-inline-custom")
    .append('<span class="ui-icon ui-icon-note"></span>')
    .prependTo(col.children("div"));
}

function showLoadingBar(loader, loadTo) {
    loader.css({ left: loadTo.css("left"), top: loadTo.css("top"), width: loadTo.css("width"), height: loadTo.css("height") });
    loader.fadeIn();//loader.ajaxStart(function () { $(this).show(); }).ajaxComplete(function () { $(this).hide(); });
}

function clearMapFromSelectedMarkersInGrid() {
    var rows = $grid.getDataIDs(), row;
    if (rows)
        for (var i = 0; i < rows.length; i++) {
            row = $grid.getRowData(rows[i]);
            if (row.MarkerIndex) {
                if (row.MarkerIndex.indexOf("-") == -1)
                    unRegisterOverlay(parseInt(row.MarkerIndex));
                else
                    for (var k = row.MarkerIndex.split("-")[0]; k < row.MarkerIndex.split("-")[1]; k++)
                        unRegisterOverlay(k);

            }

        }
}

function poiMarkerImage(imageFileName) {
    return new g.MarkerImage("/Images/poiImages/" + imageFileName, new g.Size(32, 32), new g.Point(0, 0), new g.Point(imageFileName == "landmark.png" ? 5 : 16, 32));
}

function drawPoibyType() {
    if ($("#selOrganizationByUser").val() == "") {
        alert("Please select organization");
        $("#selOrganizationByUser").focus();
        return false;
    } else if ($("#selPOIType").val() == "") {
        alert("Please select POI Type");
        $("#selPOIType").focus();
        return false;
    }
    var poiType = $("#selPOIType").val();
    $.ajax({
        url: urlGetPoisbyType + "?orgCode=" + $("#selOrganizationByUser").val() + "&companyCode=" + ($("#selCompanyByOrg").val() == null ? "" : $("#selCompanyByOrg").val()) + "&poiType=" + poiType,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function(data, textStatus, xhr) {
            if (textStatus == "success") {
                if (!data.length) {
                    alert("There are no pois by poi type " + poiType);
                    return;
                }
                var bounds = new g.LatLngBounds();
                $(data).each(function() {
                    drawPoiOnMap(this.Lati, this.Longi, this.Id, this.PoiType.GImage.ImgPath);
                    bounds.extend(new g.LatLng(this.Lati, this.Longi));
                });
                map.fitBounds(bounds);
            }
        }
    });
}

function findPOIsByLatLngAndPoiType() {
    if ($("#txtLat").val() === "") {
        alert("Please input Latitude");
        $("#txtLat").focus();
        return false;
    } else if ($("#txtLng").val() === "") {
        alert("Please input Longitude");
        $("#txtLng").focus();
        return false;
    } else if ($("#selPOIType").val() === "") {
        alert("Please select POI Type");
        $("#selPOIType").focus();
        return false;
    }
    var poiType = $("#selPOIType").val();
    showLoadingBar($("#poiAdminLoader"), $("#poiAdmin").parent());//$("#editmodlistPoiImage"));
    $.ajax({
        url: urlGetPoisbyType + "?orgCode=" + $("#selOrganizationByUser").val() + "&companyCode=" + ($("#selCompanyByOrg").val() == null ? "" : $("#selCompanyByOrg").val()) + "&poiType=" + poiType,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            //$("#poiAdminLoader").css({ height: $("#poiAdmin").parent().css("height") });
            //$("#poiAdminLoader").css({ width: $("#poiAdmin").parent().css("width") });
            //$("#poiAdminLoader").css({ left: $("#poiAdmin").parent().css("left") });
            //$("#poiAdminLoader").css({ top: $("#poiAdmin").parent().css("top") });
            $("#poiAdmin").dialog({
                closeOnEscape: false,
                beforeClose: function (event, ui) { return false; },
                dialogClass: "noclose",
                draggable: false
            });
            //$("#poiAdminLoader").fadeIn();
        },
        success: function(data, textStatus, xhr) {
            if (textStatus == "success") {
                if (!data.length) {
                    alert("There are no pois by poi type " + poiType);
                    return;
                }
                var latLng = new g.LatLng($("#txtLat").val(), $("#txtLng").val()), //new g.LatLng(31.3347, 73.4214), //
                circle, bounds = new g.LatLngBounds();

                $.get(urlGetGImagePathByPoiTypeNo, { poiTypeNo: poiType }, function(imagePath) {
                    var markerImage = poiMarkerImage(imagePath);
                    var marker = addMarkerForResource(latLng, undefined, true, false, undefined, undefined,
                        new g.MarkerImage("http://www.google.com/intl/en_us/mapfiles/ms/micons/ylw-pushpin.png", new g.Size(32, 32), new g.Point(0, 0), new g.Point(11, 32)));

                    $(data).each(function() {
                        circle = new g.Circle({
                            map: map,
                            center: new g.LatLng(this.Lati, this.Longi),
                            radius: parseInt($("#txtBuffer").val())
                        });

                        if (!circle.getBounds().contains(latLng)) circle.setMap(null);
                        else {
                            marker = addMarkerForResource(new g.LatLng(this.Lati, this.Longi), undefined, true, false, undefined, undefined, markerImage);
                            overlaysArray.circle[overlaysArray.marker.indexOf(marker)] = circle;
                            g.event.addListener(circle, "rightclick", function(mouseEvent) {
                                unRegisterOverlay(overlaysArray.circle.indexOf(circle));
                            });
                            //extend the bounds
                            bounds.extend(circle.getBounds().getNorthEast());
                            bounds.extend(circle.getBounds().getSouthWest());
                        }
                    });

                    if (!bounds.isEmpty()) {
                        //now fit the map to the newly inclusive bounds
                        map.fitBounds(bounds);
                        //(optional) restore the zoom level after the map is done scaling
                        var listener = g.event.addListener(map, "idle", function() {
                            if (map.getZoom() > zoom) map.setZoom(zoom);
                            g.event.removeListener(listener);
                        });
                    } else 
                        alert("There is no POI found which contains the given lat lng.");
                    
                    map.setCenter(latLng);

                    $("#poiAdmin").dialog({
                        closeOnEscape: true,
                        beforeClose: function(event, ui) { return true; },
                        dialogClass: "",
                        draggable: true
                    });
                    $("#poiAdminLoader").fadeOut();
                });
            }
        }
    });
}