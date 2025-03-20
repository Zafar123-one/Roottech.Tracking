using System.Web.Optimization;

namespace Roottech.Tracking.RTM.UI.Web.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery/min").Include("~/Scripts/jquery-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include("~/Scripts/jquery-ui-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui/min").Include("~/Scripts/jquery-ui-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/map/scripts").Include(
                "~/Scripts/jquery-ui-timepicker-addon.min.js",
                "~/Scripts/jquery-ui-sliderAccess.js",
                "~/Scripts/jquery.dialog.minimize.js",
                "~/Scripts/jquery.treeview.js",
                "~/Scripts/jquery.treeview.edit.js",
                "~/Scripts/jquery.treeview.async.js",
                "~/Scripts/jquery.countdown.js",
                "~/Scripts/jquery.fancybox.js",
                "~/Scripts/jQueryRotateCompressed.2.2.js",
                "~/Scripts/jquery.contextMenu.js",
                "~/Scripts/app/map.buttons.js",
                "~/Scripts/app/map.AmpleView.js",
                "~/Scripts/mustache.js",
                "~/Scripts/app/ajax.loader.js",
                "~/Scripts/appCommon/common.functions.js",
                "~/Scripts/appCommon/jqGrid.functions.js",
                "~/Scripts/date.js",
                "~/Scripts/galleria-1.2.9.min.js",
                "~/Scripts/galleria.classic.min.js",
                "~/Scripts/app/gauge.min.js",
                "~/Scripts/speedoMeter/jquery.speedometer.js",
	            "~/Scripts/speedoMeter/jquery.jqcanvas-modified.js",
                "~/Scripts/speedoMeter/excanvas-modified.js",
                "~/Scripts/jquery.tokeninput.js",
                "~/Scripts/javascript.util.js",
                "~/Scripts/jsts.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/jqPlot/scripts").Include(
                "~/Scripts/jqPlot/jquery.jqplot.min.js",
                "~/Scripts/jqPlot/plugins/jqplot.barRenderer.min.js",
                "~/Scripts/jqPlot/plugins/jqplot.canvasTextRenderer.min.js",
                "~/Scripts/jqPlot/plugins/jqplot.canvasAxisLabelRenderer.min.js",
                "~/Scripts/jqPlot/plugins/jqplot.categoryAxisRenderer.min.js",
                "~/Scripts/jqPlot/plugins/jqplot.highlighter.min.js",
                "~/Scripts/jqPlot/plugins/jqplot.cursor.min.js",
                "~/Scripts/jqPlot/plugins/jqplot.donutRenderer.min.js",
                "~/Scripts/jqPlot/plugins/jqplot.pointLabels.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/googleMap/scripts").Include(
                "~/Scripts/app/map.js",
                "~/Scripts/app/map.options.js",
                "~/Scripts/app/map.controls.js",
                "~/Scripts/app/map.drawing.js",
                "~/Scripts/app/map.geometry.js",
                "~/Scripts/app/map.latLng.locator.js",
                "~/Scripts/app/map.marker.js",
                "~/Scripts/app/map.geoCoder.js",
                "~/Scripts/app/map.arrowHandler.js",
                "~/Scripts/app/map.animation.control.js",
                "~/Scripts/keydragzoom_packed.js",
                "~/Scripts/app/PoiAdmin.js",
                "~/Scripts/app/map.geoFence.Admin.js",
                "~/Scripts/app/map.svg.vehicle.path.js",
                "~/Scripts/app/map.svg.truck.path.js",
                "~/Scripts/app/map.asset.finder.js",
                "~/Scripts/app/map.track.js"
                ));
            bundles.Add(new StyleBundle("~/bundles/map/css").Include(
                "~/Content/styles/common.css",
                "~/Content/jquery-ui-timepicker-addon.min.css",
                "~/Content/styles/map.css",
                "~/Content/styles/galleria.classic.css",
                "~/Content/Styles/token-input.css",
                "~/Scripts/jqPlot/jquery.jqplot.min.css"
                ));

            bundles.Add(new StyleBundle("~/Content/treeview/css").Include("~/Content/jquery.treeview.css","~/Content/jquery.fancybox.css"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/themes/base/minified/css").Include("~/Content/themes/base/minified/jquery-ui.min.css"));
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include("~/Content/themes/base/all.css"));

            // for stationay
            bundles.Add(new StyleBundle("~/bundles/sty/css").Include(
                "~/Content/styles/common.css",
                "~/Content/styles/map.css",
                "~/Content/jqwidgets/jqx.base.css",
                "~/Content/jquery-ui-timepicker-addon.min.css",
                "~/Scripts/jqPlot/jquery.jqplot.min.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/sty/scripts").Include(
                "~/Scripts/keydragzoom_packed.js",
                "~/Scripts/jqwidgets/jqxcore1.js",
                "~/Scripts/jqwidgets/jqxdraw.js",
                "~/Scripts/jqwidgets/jqxgauge.js",
                "~/Scripts/appCommon/common.functions.js",
                "~/Scripts/appCommon/jqGrid.functions.js",
                "~/Scripts/app/sty.base.js",
                "~/Scripts/app/jqGridExportToExcel.js",
                "~/Scripts/date.js",
                "~/Scripts/app/ajax.loader.js",
                "~/Scripts/jquery-ui-timepicker-addon.min.js",
                "~/Scripts/app/PoiAdmin.js",
                "~/Scripts/jquery-ui-sliderAccess.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/login/scripts").Include(
                "~/Scripts/jquery.fancybox.js",
                "~/Scripts/app/login.js"
                ));

            bundles.Add(new StyleBundle("~/bundles/login/css").Include(
                "~/Content/login-box.css",
                "~/Content/styles/common.css",
                "~/Content/jquery.fancybox.css"
                ));
        
            BundleTable.EnableOptimizations = true;
        }
    }
}