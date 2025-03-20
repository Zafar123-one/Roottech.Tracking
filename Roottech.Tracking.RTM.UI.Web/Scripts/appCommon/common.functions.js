function formatRating(cellValue, options, rowObject) {
    //return "<span style='color:" + (parseInt(cellValue) >= 0 ? "" : "red") + "' originalValue='" +
     //               cellValue + "'>" + formatCurrency(cellValue) + "</span>";

    var value = parseFloat(cellValue), retult,
        op = $.extend({}, $.jgrid.formatter.number); // or $.jgrid.formatter.integer

    if (!$.fmatter.isUndefined(options.colModel.formatoptions)) {
        op = $.extend({}, op, options.colModel.formatoptions);
    }
    retult = $.fmatter.util.NumberFormat(Math.abs(value), op);
    return (value >= 0 ? retult : '(' + retult + ')');

}

function negativeNumberCellAttribute(rowid, cellvalue) {
    if (cellvalue == "") return;
    return parseFloat(cellvalue) >= 0 ? '' : ' style="color:red;font-weight:bold;"';
}

function unformatRating(cellValue, options, cellObject) {
    return $(cellObject.html()).attr("originalValue");
}

function formatCurrency(num) {
    //if (!num) return null;
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num))
        num = "0";
    var sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    var cents = num % 100;
    num = Math.floor(num / 100).toString();
    if (cents < 10)
        cents = "0" + cents;
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
        num = num.substring(0, num.length - (4 * i + 3)) + ',' +
                        num.substring(num.length - (4 * i + 3));
    return (((sign) ? '' : '(') + num + '.' + cents + ((sign) ? '' : ')'));
}

function showImageByImageNo(imageNo) {
    var db = $("#dbimages").data("url");
    var imageFolderName = $("#imageFolderName").data("url");
    var path = db + imageFolderName + "-" + pad(imageNo, 5) + ".jpg"; //ABNE-00001.jpg
    showImage(path);
}

var myWindow, i = 0;
function showImage(valueAccessor) {
    if (myWindow)
        if (!myWindow.closed) {
            myWindow.document.write("<script type='text/javascript'>openImage('" + valueAccessor + "');</script>");
            myWindow.focus();
            return;
        }
    i++;
    myWindow = window.open("", "imageNM", "status=0,toolbar=0,location=0,menubar=0,scrollbars=1,fullscreen=1,resizable=1");
    var t = "<link rel='stylesheet' href='" + $("#content").data("url") + "style/jquery.iviewer.css' />";
    t += "<script src='" + $("#script").data("url") + "jquery-2.1.3.min.js' type='text/javascript'></script>";
    t += "<script src='" + $("#script").data("url") + "jquery-ui-1.11.2.min.js' type='text/javascript'></script>";
    t += "<script src='" + $("#script").data("url") + "jquery.mousewheel-3.0.6.pack.js' type='text/javascript'></script>";
    t += "<script src='" + $("#script").data("url") + "jquery.iviewer.js' type='text/javascript'></script>";
    t += "<a class='go' href='" + valueAccessor + "'>Show image!</a><div id='iviewer'><div class='loader'></div><div class='viewer'></div><ul class='controls'><li class='close'></li><li class='zoomin'></li><li class='zoomout'></li></ul><p class='info'>Use your scrollwheel or the zoom buttons to zoom in/out. Click and drag to view other parts of the image when zoomed.</p></div>";
    myWindow.document.write(t);
    myWindow.document.write("<script src='" + $("#script").data("url") + "appCommon/iviewer.main.js??version=" + i.toString() + ".0'></script>");
    myWindow.document.write("<script type='text/javascript'>openImage('" + valueAccessor + "');</script>");
    myWindow.focus();
};

function pad(str, max) {
    return str.length < max ? pad("0" + str, max) : str;
}