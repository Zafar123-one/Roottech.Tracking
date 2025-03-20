var _rowNum = $("#noOfRows").val(),
    _rowList = [10, 20, 30, 40, 50, 70, 100, 500, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000, 7500, 8000, 8500, 9000, 9500, 10000],
    _height = "100%",
    _width = "550",
    _dateFormat = "dd/mm/yy", _timeFormat = " HH:mm:ss";
//["eq", "ne", "lt", "le", "gt", "ge", "bw", "bn", "in", "ni", "ew", "en", "cn", "nc"]
var bindDatePicker = function (elem) {
    $(elem).datepicker({ dateFormat: _dateFormat, changeYear: true, changeMonth: true, showButtonPanel: true });
},
    bindDateTimePicker = function (elem) {
        $(elem).datetimepicker({
            showSecond: true,
            stepHour: 2,
            stepMinute: 10,
            stepSecond: 10,
            dateFormat: _dateFormat,
            timeFormat: _timeFormat,
            addSliderAccess: true,
            sliderAccessArgs: { touchonly: false }, changeYear: true, changeMonth: true, showButtonPanel: true
	    });
    },
    dateValidation = function (value, colname) {
        try {
            $.datepicker.parseDate(_dateFormat, value);
        } catch (e) {
            return [false, e];
        }
        return [true, ""];
    },
    checkboxTemplate = {
        width: 75,
        formatter: "checkbox",
        align: "center",
        stype: "select",
        sorttype: "number",
        searchoptions: { value: ":Select;true:Yes;false:No", sopt: ["eq", "ne", "nu", "nn"] }
    },
    integerTemplate = {
        formatter: "string",
        align: "right",
        sorttype: "integer",
        searchoptions: { sopt: ["eq", "ne", "lt", "le", "gt", "ge", "nu", "nn"] },
        searchtype: "integer",
        searchrules: { required: false, integer: true }
    },
    numberTemplate = {
        formatter: "number",
        align: "right",
        sorttype: "number",
        searchoptions: { sopt: ["eq", "ne", "lt", "le", "gt", "ge", "nu", "nn"]},
        searchtype: "number",
        searchrules: { required: false, integer: true }
    },
    floatTemplate = {
        //formatter: "number",
        align: "right",
        sorttype: "number",
        searchoptions: { sopt: ["eq", "ne", "lt", "le", "gt", "ge", "nu", "nn"] },
        searchtype: "number",
        searchrules: { required: true, number: true },
        formatoptions: { decimalSeparator: ".", decimalPlaces: 20, defaultValue: '0.00' }
    },
    moneyTemplate = {
        align: "right",
        sorttype: "number",
        formatter: "number",
        cellattr: negativeNumberCellAttribute,
        formatoptions: { thousandsSeparator: "," },
        searchoptions: { sopt: ["eq", "ne", "lt", "le", "gt", "ge", "nu", "nn"] },
        searchtype: "number",
        searchrules: { required: true, number: true }
    },
    dateTemplate = {
        width: 75,
        align: "center",
        sorttype: "date",
        formatter: "date",
        formatoptions: { srcformat: "Y-m-d ", newformat: "d/m/Y" },
        searchtype: "date",
        searchrules: { required: false, custom: true, custom_func: dateValidation },
        datefmt: "d/m/Y",
        editrules: { date: true },
        searchoptions: { sopt: ["eq", "ne", "lt", "le", "gt", "ge", "nu", "nn"], dataInit: bindDatePicker }
    },
    dateTimeTemplate = {
        width: 75,
        align: "center",
        sorttype: "datetime",
        formatter: dateTimeFormatter,
        //formatter: "datetime",
        //formatoptions: { srcformat: "Y-m-dTh:i:s", newformat: "d/m/Y h:i:s" },
        searchtype: "datetime",
        searchrules: { required: false, custom: true, custom_func: dateValidation },
        datefmt: "d/m/Y H:i:s",
        editrules: { date: true },
        searchoptions: { sopt: ["eq", "ne", "lt", "le", "gt", "ge", "nu", "nn"], dataInit: bindDateTimePicker }
    },
    dateRequiredTemplate = JSON.parse(JSON.stringify(dateTemplate)),
    defaultSearch = "cn",
    filterTemplateLabel = 'Show columns by Template:&nbsp;',
    getColumnIndex = function (columnIndex) {
        var cm = this.jqGrid("getGridParam", "colModel"), i, l = cm.length;
        for (i = 0; i < l; i++) {
            if ((cm[i].index || cm[i].name) === columnIndex) {
                return i; // return the colModel index
            }
        }
        return -1;
    },
    refreshSerchingToolbar = function (myDefaultSearch) {
        var filters, i, l, rules, rule, iCol, cmi, control, tagName,
            $this = $(this),
            postData = $this.jqGrid('getGridParam', 'postData'),
            cm = $this.jqGrid('getGridParam', 'colModel');

        for (i = 0, l = cm.length; i < l; i++) {
            control = $("#gs_" + $.jgrid.jqID(cm[i].name));
            if (control.length > 0) {
                tagName = control[0].tagName.toUpperCase();
                if (tagName === "SELECT") { // && cmi.stype === "select"
                    control.find("option[value='']")
                        .attr('selected', 'selected');
                } else if (tagName === "INPUT") {
                    control.val('');
                }
            }
        }

        if (typeof (postData.filters) === "string" &&
            typeof (this.ftoolbar) === "boolean" && this.ftoolbar) {

            filters = $.parseJSON(postData.filters);
            if (filters && filters.groupOp === "AND" && typeof (filters.groups) === "undefined") {
                // only in case of advance searching without grouping we import filters in the
                // searching toolbar
                rules = filters.rules;
                for (i = 0, l = rules.length; i < l; i++) {
                    rule = rules[i];
                    iCol = getColumnIndex.call($this, rule.field);
                    if (iCol >= 0) {
                        cmi = cm[iCol];
                        control = $("#gs_" + $.jgrid.jqID(cmi.name));
                        if (control.length > 0 &&
                            (((typeof (cmi.searchoptions) === "undefined" ||
                                typeof (cmi.searchoptions.sopt) === "undefined")
                                    && rule.op === myDefaultSearch) ||
                                        (typeof (cmi.searchoptions) === "object" &&
                                            $.isArray(cmi.searchoptions.sopt) &&
                                                cmi.searchoptions.sopt.length > 0 &&
                                                    cmi.searchoptions.sopt[0] === rule.op))) {
                            tagName = control[0].tagName.toUpperCase();
                            if (tagName === "SELECT") { // && cmi.stype === "select"
                                control.find("option[value='" + $.jgrid.jqID(rule.data) + "']")
                                    .attr('selected', 'selected');
                            } else if (tagName === "INPUT") {
                                control.val(rule.data);
                            }
                        }
                    }
                }
            }
        }
    },
    iTemplate,
    templateOptions = '';

    //$.jgrid.defaults.cmTemplate.
    $(document).ready(function () {
        //CSS Files
        dateRequiredTemplate.searchrules.required = true;
        dateRequiredTemplate.searchrules.custom_func = dateValidation;
        dateRequiredTemplate.searchoptions.dataInit = bindDatePicker;
        var path = $("#contentPath").prop("class");
        //$("<link/>", { rel: "stylesheet", type: "text/css", media: "screen", href: path + "themes/base/minified/jquery-ui.min.css" }).appendTo("head"); //jquery.ui.all.css
        $("<link/>", { rel: "stylesheet", type: "text/css", media: "screen", href: path + "styles/ui.multiselect.css" }).appendTo("head");
        $("<link/>", { rel: "stylesheet", type: "text/css", media: "screen", href: path + "jquery.jqGrid/ui.jqgrid.css" }).appendTo("head");

        path = $("#scriptPath").prop("class");
        //Must load language tag BEFORE script tag
        
        //$.getScript(path + "jquery-ui-1.10.0.min.js", function () {
            $.getScript(path + "jquery-ui.multiselect.js", function () {
                $.getScript(path + "i18n/grid.locale-en.js", function () {
                    $.getScript(path + "jquery.jqGrid.min.js")
                    .done(function (script, textStatus) {
                        loadPage();
                        /*global $ */
                        /*jslint browser: true, plusplus: true */
                        $.jgrid.formatter.integer.thousandsSeparator = ",";
                        $.jgrid.formatter.number.thousandsSeparator = ",";
                        $.jgrid.formatter.currency.thousandsSeparator = ",";
                    })
                    .fail(function (jqxhr, settings, exception) {
                        console.log(jqxhr);
                        console.log(settings);
                        console.log(exception);
                    });
                    /*, function (data, textStatus, jqxhr) {
                    console.log('sssrrr');
                    console.log(data);
                    });*/
                });
            });
        //});
    });

/**
* This function formats the date column for the  grid.
*
* The grid could be populated with dates that are in m/d/Y format or in Y-m-dTH:i:s format; need
* to account for this; want the dates to end up being in m/d/Y format always.
*
* @param cellvalue     is the value to be formatted
* @param options       an object containing the following element
*                      options : { rowId: rid, colModel: cm} where rowId - is the id of the row colModel is the object of the properties for this column getted from colModel array of jqGrid
* @param rowObject     is a row data represented in the format determined from datatype option;
*                      the rowObject is array, provided according to the rules from jsonReader
* @return              the new formatted cell value html
*/
function dateTimeFormatter(cellvalue, options, rowObject) {

    // parseExact just returns 'null' if the date you are trying to
    // format is not in the exact format specified
    var parsedDate = Date.parseExact(cellvalue, "yyyy-MM-ddTHH:mm:ss");
    if (parsedDate == null)
        parsedDate = new Date(cellvalue);

    // if parsed date is null, just used the passed cell value; otherwise,
    // transform the date to desired format
    var formattedDate = parsedDate ? parsedDate.toLocaleString('en-GB', { hour12: true }) : cellvalue;//.toString("dd-MM-yyyy HH:mm:ss tt")

    return formattedDate;
}

function decodeErrorMessage(jqXHR, textStatus, errorThrown) {
    var html, errorInfo, i, errorText = textStatus + "\n" + errorThrown;
    if (jqXHR.responseText.charAt(0) === "[") {
        try {
            errorInfo = $.parseJSON(jqXHR.responseText);
            errorText = "";
            for (i = 0; i < errorInfo.length; i++) {
                if (errorText.length !== 0) {
                    errorText += "<hr/>";
                }
                errorText += errorInfo[i].Source + ": " + errorInfo[i].Message;
            }
        } catch (e) {
        }
    } else {
        html = /<body.*?>([\s\S]*)<\/body>/.exec(jqXHR.responseText);
        if (html !== null && html.length > 1) {
            errorText = html[1];
        }
    }
    return errorText;
}

function loadjqGrid(arrGridUnload, listtoLoad, url, postData, colNames, colModel, pager,
    sortname, sortorder, imgpath, rowNum, rowList, height, width, caption, jsonReader, templates,
    datastr, loadonce, footerrow, loadComplete, editOptions, addOptions, delOptions, grouping,
    groupingView, userDataOnFooter, subGrid, subGridUrl, subGridModel, serializeSubGridData,
    subGridRowExpanded, ajaxSubgridOptions, multiselect, onSelectRow, beforeRequest, beforeClosingGrid,
    navLabels, isNotDraggable, rowattr, searchOperators, dataType) {

    templates = (templates == undefined) ? [] : templates;
    var tmplNames = [];
    $(templates).each(function (index, value) {
        tmplNames.push("Template " + index);
    });
    
    //console.log(userDataOnFooter);
    $.each(arrGridUnload, function() {
        $("#" + this).GridUnload(); // destroy the grid saving the table and pager
    });
    $("#" + listtoLoad).GridUnload();

    $grid = $("#" + listtoLoad);
    //navLabels to be fixed if not defined or uncorrect
    if (navLabels == undefined) navLabels = ";;;;;;;;;;;;;";
    if (navLabels.split(";").length < 14) {
        var j = 14 - navLabels.split(";").length;
        for (var i = 0; i < j; i++)
            navLabels += ";";
    }

    // here code to load new jqGrid configuration – like colmodel, colnames and etc
    $grid.jqGrid({
            url: (typeof datastr == "undefined") ? url : null,
            datastr: (typeof datastr == "undefined") ? null : datastr,
            datatype: (typeof datastr == "undefined") ? ((typeof dataType == "undefined") ? "json" : dataType) : "jsonstring",
            mtype: "GET",
            postData: postData,
            colNames: colNames,
            colModel: colModel,
            pager: "#" + pager,
            sortname: sortname,
            sortorder: sortorder,
            multiselect: (typeof multiselect == "undefined") ? false : multiselect,
            viewrecords: (rowNum !== 0),//to show/hide view number of records label on the right bottom of grid
            altRows: true,
            altclass: "altRowClass",
            loadonce: (typeof loadonce == "undefined") ? false : loadonce,
            imgpath: imgpath,

            pgbuttons : (rowNum !== 0),//set paging required
            //pgtext : "",
            pginput: (rowNum !== 0), //set paging required

            rownumbers: (rowNum !== 0), //true, //set paging required
            gridview: true, // to improve jqGrid performance
            rowNum: rowNum,
            rowList: rowList,
            height: height,
            width: width,
            autoWidth: false,
            shrinkToFit: false,
            ignoreCase: true,
            sortable: true,
            caption: caption,
            //id:"Id",
            footerrow: (typeof footerrow == "undefined") ? false : footerrow,
            grouping: (typeof grouping == "undefined") ? false : grouping,
            groupingView: (typeof groupingView == "undefined") ? null : groupingView,
            userDataOnFooter: (typeof userDataOnFooter == "undefined") ? false : userDataOnFooter,
            toolbar: [!$.isEmptyObject(templates), "top"],
            subGrid: (typeof subGrid == "undefined") ? false : subGrid,
            subGridOptions: (typeof subGrid == "undefined") ? null :
            {
                "plusicon": "ui-icon-triangle-1-e",
                "minusicon": "ui-icon-triangle-1-s",
                "openicon": "ui-icon-arrowreturn-1-e",
                "reloadOnExpand": false,
                "selectOnExpand": true
            },
            subGridUrl: (typeof subGridUrl == "undefined") ? null : subGridUrl,
            subGridModel: (typeof subGridModel == "undefined") ? null : subGridModel,
            serializeSubGridData: (typeof serializeSubGridData == "undefined") ? null : serializeSubGridData,
            subGridRowExpanded: (typeof subGridRowExpanded == "undefined") ? null : subGridRowExpanded,
            ajaxSubgridOptions: (typeof ajaxSubgridOptions == "undefined") ? null : ajaxSubgridOptions,
            loadError: function(jqXHR, textStatus, errorThrown) {
                $("#" + this.id + "_err").remove();
                // insert div with the error description before the grid
                $grid.closest("div.ui-jqgrid").before(
                    "<div id='" + this.id + "_err' style='max-width:" + this.style.width +
                    ";'><div class='ui-state-error ui-corner-all' style='padding:0.7em;float:left;'><span class='ui-icon ui-icon-alert' style='float:left; margin-right: .3em;'></span><span style='clear:left'>" +
                    decodeErrorMessage(jqXHR, textStatus, errorThrown) + "</span></div><div style='clear:left'/></div>");
            },
            beforeRequest: function() {
                if (typeof beforeRequest != "undefined") beforeRequest();
                //$("#" + $grid.attr("aria-labelledby")).hide();
            },
            loadComplete: function() {
                $("#" + this.id + "_err").remove();
                var $this = $(this);

                if (typeof (this.ftoolbar) !== "boolean" && (rowNum != 0)) // create toolbar if needed
                    $this.jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, searchOperators: (typeof searchOperators == "undefined") ? false : searchOperators, defaultSearch: defaultSearch });
                if (typeof loadComplete != "undefined") loadComplete();
                $("#" + pager + "_left").css("width", "170");
                //refreshSerchingToolbar.call(this, defaultSearch);
            },
            onSelectRow: function(id, status) {
                if (typeof onSelectRow != "undefined") onSelectRow(id, status);
            },
            onInitGrid: function() {
                if (isNotDraggable) return;
                $("#" + $grid.attr("aria-labelledby")).hide();
                //if ($(this).attr('initialized') == undefined) {             $(this).attr('initialized', true);
                //Deactivate ALL opened jqGrid windows
                $(".ui-jqgrid").removeClass('ui-jqgrid-active');
                // Make the window draggable.
                $('#gbox_' + this.id).draggable({
                        handle: "div.ui-jqgrid-titlebar",
                        start: function() {
                            $(".ui-jqgrid").removeClass('ui-jqgrid-active');
                            $(this).addClass('ui-jqgrid-active');
                        }
                    }).click(function() {
                        $(".ui-jqgrid").removeClass('ui-jqgrid-active');
                        $(this).addClass('ui-jqgrid-active');
                    }).css({'position': 'absolute', 'height': ''})
                    .addClass('ui-jqgrid-active');
                //Close button on the title bar
                var temp = $("<a></a>")
                    .addClass('ui-jqgrid-titlebar-close HeaderButton')
                    .css("right", 20)
                    .attr("href", "javascript:void(0)")
                    .attr("role", "link")
                    .click(function() {
                        if (typeof beforeClosingGrid != "undefined") beforeClosingGrid();
                        $(this).parents('.ui-jqgrid').hide();
                    })
                    .hover(
                        function() { $(this).addClass('ui-state-hover'); },
                        function() { $(this).removeClass('ui-state-hover'); }
                    ).append($("<span></span>").addClass("ui-icon ui-icon-circle-close"));
                $('#gbox_' + this.id + ' .ui-jqgrid-title').before(temp);
                //if (typeof onInitGrid != "undefined") onInitGrid();
                //}

            },
            rowattr: rowattr, /*
        gridComplete: function () { console.log("gridComplete"); },
        beforeProcessing: function (data, status, xhr) {
            console.log("beforeProcessing");
            console.log(data);
            console.log(status); console.log(xhr);
        },
        beforeSelectRow: function (rowid, e) {
            console.log("beforeSelectRow");
            console.log(rowid); console.log(e);
            return true;
        },
        loadBeforeSend: function (xhr,settings) {
            console.log("loadBeforeSend");
            console.log(xhr); console.log(settings);
        },*/
            //hide or show grid
            //onHeaderClick: function(gridstate) {console.log("onHeaderClick");console.log(gridstate);},
            jsonReader: (typeof jsonReader == "undefined") ? null : jsonReader
            /*, jsonReader: {
        root: "rows",
        page: "page",
        total: "total",
        records: "records",
        repeatitems: false,
        userdata: "userdata"
        }*/
        })
        .jqGrid("navGrid", "#" + pager,
        {
            edit: (editOptions == undefined) ? false : true,
            add: (addOptions == undefined) ? false : true,
            del: (delOptions == undefined) ? false : true,
            search: (rowNum !== 0),
            refresh: (rowNum !== 0),
            //,searchtext: "" //Find
            edittext: navLabels.split(";")[0],
            edittitle: navLabels.split(";")[1] !== "" ? navLabels.split(";")[1] : "Edit selected row",
            addtext: navLabels.split(";")[2],
            addtitle: navLabels.split(";")[3] !== "" ? navLabels.split(";")[3] : "Add new row",
            deltext: navLabels.split(";")[4],
            deltitle: navLabels.split(";")[5] !== "" ? navLabels.split(";")[5] : "Delete selected row",
            searchtext: navLabels.split(";")[6],
            searchtitle: navLabels.split(";")[7] !== "" ? navLabels.split(";")[7] : "Find records",
            refreshtext: navLabels.split(";")[8],
            refreshtitle: navLabels.split(";")[9] !== "" ? navLabels.split(";")[9] : "Reload Grid",
            alertcap: navLabels.split(";")[10] !== "" ? navLabels.split(";")[10] : "Warning",
            alerttext: navLabels.split(";")[11] !== "" ? navLabels.split(";")[11] : "Please, select row",
            viewtext: navLabels.split(";")[12],
            viewtitle: navLabels.split(";")[13] !== "" ? navLabels.split(";")[13] : "View selected row"
        },
        (editOptions == undefined) ? {} : editOptions,
        (addOptions == undefined) ? {} : addOptions,
        (delOptions == undefined) ? {} : delOptions,
        /*{ url: editUrl, width: 500 }, // edit options
        {url: addUrl }, // add options
        {url: delUrl }, //del options*/
        {
        multipleSearch: true,
        multipleGroup: true,
        recreateFilter: true,
        showQuery: true,
        closeOnEscape: true,
        closeAfterSearch: true/*,
        //set the label, names and contents of the template
        width: 500,
        tmplLabel: $.isEmptyObject(templates) ? "" : filterTemplateLabel,
        tmplNames: tmplNames, //["Template One", "Template Two"], 
        tmplFilters: templates//[template1, template2]*/
    }) // search options
    //.jqGrid("filterToolbar", { stringResult: true, searchOnEnter: true, defaultSearch: defaultSearch })
    .jqGrid("navButtonAdd", "#" + pager, {
        caption: "",
        buttonicon: "ui-icon-calculator", //"ui-icon-wrench",
        position: "last",
        title: "Choose Columns",
        onClickButton: function () {
            $grid.jqGrid("columnChooser", {
                modal: true,
                //Function which will be called when column chooser is closed
                done: function (perm) {
                    //We check if user has accepted
                    if (perm) {
                        //First we are resizing grid
                        var gridWidth = this.jqGrid("getGridParam", "width");
                        this.jqGrid("setGridWidth", gridWidth);
                        console.log(gridWidth);
                        //then we remap columns
                        this.jqGrid("remapColumns", perm, true);
                    }
                }
            });
        }
    })
    .jqGrid("navButtonAdd", "#" + pager, {
        id: pager + "_excel",
        caption: "",
        buttonicon: "ui-icon-document", //newwin
        title: "Export To Excel (Selected)",
        onClickButton: function (e) {
            try {
                $grid.jqGrid("excelExport", { tag: "excel", url: url + "Ee" + "?all=false" });
            } catch (e) {
                window.location = url + "Ee" + "?all=false&oper=excel";
            }
        }
    })
    .jqGrid("navButtonAdd", "#" + pager, {
        id: pager + "_excel_all",
        caption: "",
        buttonicon: "ui-icon-document-b", //print
        title: "Export To Excel (All)",
        onClickButton: function (e) {
            try {
                $grid.jqGrid("excelExport", { tag: "excel", url: url + "Ee" + "?all=true" });
            } catch (e) {
                window.location = url + "Ee" + "?all=true&oper=excel";
            }
        }
    })
    .jqGrid("gridResize", {
        minWidth: 350,
        minHeight: 150,
        stop:
		function (grid, ev, ui) {
		    //
		    // There seems to be an issue with resizing the grid so I added
		    // this code to remove the "height" style.
		    //
		    $(grid.srcElement).parent().css("height", null);
		}
    });            //Resizable grid
    //.jqGrid("sortableRows"); //Sortable and dragable and dropable rows
    if (rowNum !== 0)
        $grid.jqGrid("navButtonAdd", "#" + pager, {
            id: "filter_Toolbar",
            caption: "",
            title: "Toggle Search Bar",
            buttonicon: "ui-icon-pin-s",
            onClickButton: function() { $grid[0].toggleToolbar(); }
        });
    if (!$.isEmptyObject(templates)) {
        var reloadWithNewFilterTemplate = function () {
            var iTemplate = parseInt($('#filterTemplates').val(), 10),
            cm = $grid.getGridParam("colModel");
            if (isNaN(iTemplate)) {
                for (var i = 0; i < cm.length; i++) $grid.jqGrid("showCol", cm[i].name);
            } else if (iTemplate >= 0) {
                if (!$.isEmptyObject(templates[iTemplate])) for (var i = 0; i < cm.length; i++) $grid.jqGrid("hideCol", cm[i].name);
                for (var i = 0; i < templates[iTemplate].length; i++)
                    $grid.jqGrid("showCol", $grid.getGridParam("colModel")[templates[iTemplate][i]].name);
            }
        };
        // Filter Templates
        for (iTemplate = 0; iTemplate < templates.length; iTemplate++) {
            templateOptions += '<option value="' + iTemplate + '">' +
                tmplNames[iTemplate] + "</option>";
        }
        $("#t_" + $.jgrid.jqID($grid[0].id)).append("<label for='filterTemplates'>" + filterTemplateLabel + "</label>" +
            "<select id='filterTemplates'><option value=''>Show All</option>" + templateOptions + "</select>");
        $("#filterTemplates").change(reloadWithNewFilterTemplate).keyup(function (e) {
            var keyCode = e.keyCode || e.which;
            if (keyCode === $.ui.keyCode.PAGE_UP || keyCode === $.ui.keyCode.PAGE_DOWN ||
                keyCode === $.ui.keyCode.END || keyCode === $.ui.keyCode.HOME ||
                    keyCode === $.ui.keyCode.UP || keyCode === $.ui.keyCode.DOWN ||
                        keyCode === $.ui.keyCode.LEFT || keyCode === $.ui.keyCode.RIGHT) {
                reloadWithNewFilterTemplate(templates);
            }
        });
    }
    $grid.closest(".ui-jqgrid-bdiv").width(function (i, width) { return width + 1; });
}