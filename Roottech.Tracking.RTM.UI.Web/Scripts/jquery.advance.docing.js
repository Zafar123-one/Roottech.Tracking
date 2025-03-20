//$(document).ready(function(){
function advanceDocking(objString) {
    var docked = 0;
    var height = "420px"; //$(window).height();
    $(objString + " li ul").height(height);

    $(objString + " .dock").click(function () {
        $(this).parent().parent().addClass("docked").removeClass("free");

        docked += 1;
        var dockH = (height) / docked;
        var dockT = 0;

        $(objString + " li ul.docked").each(function () {
            $(this).height(dockH).css("top", dockT + "px");
            dockT += dockH;
        });
        $(this).parent().find(".undock").show();
        $(this).hide();

        if (docked > 0)
            $("#content").css("margin-left", "250px");
        else
            $("#content").css("margin-left", "60px");
    });

    $(objString + " .undock").click(function() {
        $(this).parent().parent().addClass("free").removeClass("docked")
            .animate({ left: "-80px" }, 200).height(height).css("top", "0px");

        docked = docked - 1;
        var dockH = (height) / docked
        var dockT = 0;

        $(objString + " li ul.docked").each(function() {
            $(this).height(dockH).css("top", dockT + "px");
            dockT += dockH;
        });
        $(this).parent().find(".dock").show();
        $(this).hide();

        if (docked > 0)
            $("#content").css("margin-left", "250px");
        else
            $("#content").css("margin-left", "60px");
    });

    $(objString + " li").hover(function() {
        $(this).find("ul").animate({ left: "15px" }, 200);
    }, function() {
        $(this).find("ul.free").animate({ left: "-80px" }, 200);
    });
} //}); 