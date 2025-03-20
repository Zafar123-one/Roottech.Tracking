using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using Castle.Core.Internal;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NHibernate.Transform;
using NHibernate.Type;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.RSPT.Entities;
using Roottech.Tracking.Domain.RSSP.Entities;
using Roottech.Tracking.Library.Models.Grid;
using Roottech.Tracking.Library.PdfReports;
using Roottech.Tracking.Library.Utils;
using Roottech.Tracking.PdfRpt.Core.Contracts;
using Roottech.Tracking.PdfRpt.FluentInterface;
using Roottech.Tracking.RTM.UI.Web.Infrastructure;

namespace Roottech.Tracking.WebApi.Controllers
{
    [Authorize]
    public class StationaryReportApiController : ApiController
    {
        private readonly IKeyedRepository<double, EDRFuelRun> _repositoryEdrFuelRun;
        private readonly ISession _session;

        public StationaryReportApiController(ISessionFactory sessionFactory,
            IKeyedRepository<double, EDRFuelRun> repositoryEdrFuelRun)
        {
            _repositoryEdrFuelRun = repositoryEdrFuelRun;
            _session = sessionFactory.GetCurrentSession();
        }

        public dynamic GetFuelIntakeReport(GridSettings grid, string sidx, string sord, int page, int rows, DateTime fromDate, DateTime toDate, string region, string city, string site, string resource)
        {
            var rptGenRefuels = GetRptGenRefuels(fromDate, toDate, region, city, site, resource);

            var totalRecords = rptGenRefuels.Count();
            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = rptGenRefuels
            };
        }

        public void GetFuelIntakeReportEe(GridSettings grid, DateTime fromDate, DateTime toDate, string region, string city, string site, string resource)
        {
            var rptGenRefuels = GetRptGenRefuels(fromDate, toDate, region, city, site, resource);
            string[,] colNamesToReplace = new string[8,2];

            colNamesToReplace[0, 0] = "Id";
            colNamesToReplace[0, 1] = "Batch No.";

            colNamesToReplace[1, 0] = "SiteName";
            colNamesToReplace[1, 1] = "Site Name";

            colNamesToReplace[2, 0] = "AssetName";
            colNamesToReplace[2, 1] = "Asset Name";

            colNamesToReplace[3, 0] = "RefuelType";
            colNamesToReplace[3, 1] = "Event Description";

            colNamesToReplace[4, 0] = "OpenDt";
            colNamesToReplace[4, 1] = "Event Start";

            colNamesToReplace[5, 0] = "EndDt";
            colNamesToReplace[5, 1] = "Event Stop";

            colNamesToReplace[6, 0] = "TotDuration";
            colNamesToReplace[6, 1] = "Total Duration";

            colNamesToReplace[7, 0] = "Increase";
            colNamesToReplace[7, 1] = "Refueled(Ltrs)";

            ExportFilesByHttpResponse.ExportToExcel(rptGenRefuels.AsQueryable(),
                row => new
                {
                    row.Id,
                    row.SiteName,
                    row.AssetName,
                    row.RefuelType,
                    row.OpenDt,
                    row.EndDt,
                    row.TotDuration,
                    row.Increase
                }, null, "Fuel Intake Report", colNamesToReplace);
        }

        public void GetFuelIntakeReportEp(GridSettings grid, DateTime fromDate, DateTime toDate, string region, string city, string site, string resource)
        {
            var rptGenRefuels = GetRptGenRefuels(fromDate, toDate, region, city, site, resource);

            new ProductsPdfReport().CreatePdfReport(rptGenRefuels);
        }

        private IList<RptGenRefuel> GetRptGenRefuels(DateTime fromDate, DateTime toDate, string region, string city, string site,
            string resource)
        {
            if (string.IsNullOrEmpty(city)) city = "%";
            if (string.IsNullOrEmpty(site)) site = "%";
            if (string.IsNullOrEmpty(resource)) resource = "%";
            if (string.IsNullOrEmpty(region)) region = "%";

            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;
            var userCode = principal.FindFirst("User_Code").Value;

            var siteCode = GetSiteCodeFromFleetUnitsViewByTitle(site, orgCode);

            var rptGenRefuels = _session.GetNamedQuery("GetRptGenRefuel")
                .SetDateTime("FromDt", fromDate).SetDateTime("ToDt", toDate)
                .SetString("Region", region)
                .SetString("City", city)
                .SetString("Territory", "%")
                .SetString("Site", siteCode == 0 ? "%" : siteCode.ToString())
                .SetString("Orgcode", orgCode)
                .SetString("Resource", resource)
                .SetString("UserCode", userCode)
                .SetTimeout(0)
                .List<RptGenRefuel>();
            return rptGenRefuels;
        }

        private IList<RptGenConsumption> GetRptGenConsumptions(DateTime fromDate, DateTime toDate, string site, string region, string city, string resource)
        {

            if (string.IsNullOrEmpty(city)) city = "%";
            if (string.IsNullOrEmpty(site)) return null;// site = "%";
            if (string.IsNullOrEmpty(resource)) resource = "%";
            if (string.IsNullOrEmpty(region)) region = "%";
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;
            var userCode = principal.FindFirst("UserOrgCode").Value;

            var siteCode = GetSiteCodeFromFleetUnitsViewByTitle(site, orgCode);

            return _session.GetNamedQuery("GetRptGenConsumption")
                .SetDateTime("FromDt", fromDate).SetDateTime("ToDt", toDate)
                .SetString("Region", region)
                .SetString("City", city)
                .SetString("Territory", "%")
                .SetString("Site", siteCode == 0 ? "%" : siteCode.ToString())
                .SetString("Orgcode", orgCode)
                .SetString("Resource", resource)
                .SetString("UserCode", userCode)
                .SetTimeout(0)
                .List<RptGenConsumption>();//.SearchGrid(grid, out totalRecords);
        }

        public dynamic GetFuelConsumptionReport(GridSettings grid, string sidx, string sord, int page, int rows, DateTime fromDate, DateTime toDate, string site, string region, string city, string resource)
        {
            var genConsumptions = GetRptGenConsumptions(fromDate, toDate, site, region, city, resource);

            int totalRecords = genConsumptions.Count();
            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = genConsumptions
            };
        }

        public void GetFuelConsumptionReportEe(GridSettings grid, DateTime fromDate, DateTime toDate, string site, string region, string city, string resource)
        {
            var genConsumptions = GetRptGenConsumptions(fromDate, toDate, site, region, city, resource);

            string[,] colNamesToReplace = new string[8, 2];

            colNamesToReplace[0, 0] = "Id";
            colNamesToReplace[0, 1] = "Batch No.";

            colNamesToReplace[1, 0] = "SiteName";
            colNamesToReplace[1, 1] = "Site Name";

            colNamesToReplace[2, 0] = "AssetName";
            colNamesToReplace[2, 1] = "Asset Name";

            colNamesToReplace[3, 0] = "SiteCode";
            colNamesToReplace[3, 1] = "Site Code";

            colNamesToReplace[4, 0] = "OpenDt";
            colNamesToReplace[4, 1] = "Event Start";

            colNamesToReplace[5, 0] = "CloseDt";
            colNamesToReplace[5, 1] = "Event Stop";

            colNamesToReplace[6, 0] = "Duration";
            colNamesToReplace[6, 1] = "Total Duration";

            colNamesToReplace[7, 0] = "ConsumeQty";
            colNamesToReplace[7, 1] = "Fuel Consumed(Ltrs)";

            ExportFilesByHttpResponse.ExportToExcel(genConsumptions.AsQueryable(),
                row => new
                {
                    row.Id,
                    row.SiteCode,
                    row.SiteName,
                    row.AssetName,
                    row.OpenDt,
                    row.CloseDt,
                    row.Duration,
                    row.ConsumeQty
                }, null, "Fuel Consumptions Report", colNamesToReplace);
        }

        public void GetFuelConsumptionReportEp(GridSettings grid, DateTime fromDate, DateTime toDate, string site,
            string region, string city, string resource)
        {
            var genConsumptions = GetRptGenConsumptions(fromDate, toDate, site, region, city, resource);
            new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.LeftToRight);
                doc.Orientation(PageOrientation.Landscape);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata { Author = "Bnf Tech", Application = "PdfRpt", Keywords = "Fuel Consumption Report", Subject = "Fuel Consumption Report", Title = "Fuel Consumption" });
                doc.Compression(new CompressionSettings
                {
                    EnableCompression = true,
                    EnableFullCompression = true
                });
            })
            .DefaultFonts(fonts =>
            {
                fonts.Path(Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\arial.ttf"),
                           Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\verdana.ttf"));
                fonts.Size(9);
                //fonts.Color(System.Drawing.Color.Black);
            })
            .PagesFooter(footer => footer.DefaultFooter(DateTime.Now.ToString("dd/MM/yyyy")))
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    //Asif defaultHeader.ImagePath(System.IO.Path.Combine(AppPath.ApplicationPath, "Content\\Images\\01.png"));
                    defaultHeader.Message("Fuel Consumption Report");
                });
            })
            .MainTableTemplate(template => template.BasicTemplate(BasicTemplate.ClassicTemplate))
            .MainTablePreferences(table => table.ColumnsWidthsType(TableColumnWidthType.Relative))
            .MainTableDataSource(dataSource => dataSource.StronglyTypedList(genConsumptions))
            .MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings("Summary");
                summarySettings.PreviousPageSummarySettings("Previous Page Summary");
                summarySettings.PageSummarySettings("Page Summary");
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenConsumption>(x => x.Id);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1);
                    column.HeaderCell("Batch No.");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenConsumption>(x => x.SiteCode);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("Site Code", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenConsumption>(x => x.SiteName);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("Site Name", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenConsumption>(x => x.AssetName);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(2);
                    column.HeaderCell("Asset Name", horizontalAlignment: HorizontalAlignment.Center);
                });
                
                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenConsumption>(x => x.OpenDt);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(1);
                    column.HeaderCell("Event Start");
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : ((DateTime)obj).ToString("yyyy-MM-dd"));
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenConsumption>(x => x.CloseDt);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(1);
                    column.HeaderCell("Event Stop");
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : ((DateTime)obj).ToString("yyyy-MM-dd"));
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenConsumption>(x => x.Duration);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width(1);
                    column.HeaderCell("Total Duration", horizontalAlignment: HorizontalAlignment.Center);
                    column.AggregateFunction(aggregateFunction => aggregateFunction.CustomAggregateFunction(new CustomSum()));
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenConsumption>(x => x.ConsumeQty);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(8);
                    column.Width(1);
                    column.HeaderCell("Fuel Consumed(Ltrs)", horizontalAlignment: HorizontalAlignment.Center);
                    column.AggregateFunction(aggregateFunction => aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum) );
                });
            })
            .MainTableEvents(events => events.DataSourceIsEmpty(message: "There is no data available to display."))
            .Export(export =>
            {
                export.ToExcel();
                export.ToCsv();
                export.ToXml();
            })
            .Generate(data =>
            {
                var fileName = "Fuel Consumption Report.pdf";
                fileName = HttpUtility.UrlEncode(fileName, Encoding.UTF8);
                data.FlushInBrowser(fileName, FlushType.Attachment);
            }); // creating an in-memory PDF file

        }

        private IList<RptFuelTheftDetail> GetRptFuelTheftDetails(DateTime fromDate, DateTime toDate, string region, string city, string site, string resource)
        {
            if (string.IsNullOrEmpty(city)) city = "%";
            if (string.IsNullOrEmpty(site)) site = "%";
            if (string.IsNullOrEmpty(resource)) resource = "%";
            if (string.IsNullOrEmpty(region)) region = "%";

            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;
            var userCode = principal.FindFirst("User_Code").Value;

            var siteCode = GetSiteCodeFromFleetUnitsViewByTitle(site, orgCode);

            return _session.GetNamedQuery("GetRptFuelTheftDetail")
                .SetDateTime("FromDt", fromDate).SetDateTime("ToDt", toDate)
                .SetString("Region", region)
                .SetString("City", city)
                .SetString("Territory", "%")
                .SetString("Site", siteCode == 0 ? "%" : siteCode.ToString())
                .SetString("Orgcode", orgCode)
                .SetString("Resource", resource)
                .SetString("UserCode", userCode)
                .SetTimeout(0)
                .List<RptFuelTheftDetail>();//.AsQueryable().SearchGrid(grid, out totalRecords);
        }

        public dynamic GetFuelTheftReport(GridSettings grid, string sidx, string sord, int page, int rows, DateTime fromDate, DateTime toDate, string region, string city, string site, string resource)
        {

            var fuelTheftDetails = GetRptFuelTheftDetails(fromDate, toDate, region, city, site, resource);
            int totalRecords = fuelTheftDetails.Count();

            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = fuelTheftDetails
            };
        }

        public void GetFuelTheftReportEe(GridSettings grid, DateTime fromDate, DateTime toDate, string region, string city, string site, string resource)
        {
            var fuelTheftDetails = GetRptFuelTheftDetails(fromDate, toDate, region, city, site, resource);

            string[,] colNamesToReplace = new string[8, 2];

            colNamesToReplace[0, 0] = "Id";
            colNamesToReplace[0, 1] = "Batch No.";

            colNamesToReplace[1, 0] = "SiteName";
            colNamesToReplace[1, 1] = "Site Name";

            colNamesToReplace[2, 0] = "AssetName";
            colNamesToReplace[2, 1] = "Asset Name";

            colNamesToReplace[3, 0] = "RefuelType";
            colNamesToReplace[3, 1] = "Event Description";

            colNamesToReplace[4, 0] = "OpenDt";
            colNamesToReplace[4, 1] = "Event Start";

            colNamesToReplace[5, 0] = "EndDt";
            colNamesToReplace[5, 1] = "Event Stop";

            colNamesToReplace[6, 0] = "TotDuration";
            colNamesToReplace[6, 1] = "Total Duration";

            colNamesToReplace[7, 0] = "Decrease";
            colNamesToReplace[7, 1] = "Fuel Theft(Ltrs)";

            ExportFilesByHttpResponse.ExportToExcel(fuelTheftDetails.AsQueryable(),
                row => new
                {
                    row.Id,
                    row.SiteName,
                    row.AssetName,
                    row.RefuelType,
                    row.OpenDt,
                    row.EndDt,
                    row.TotDuration,
                    row.Decrease
                }, null, "Fuel Theft Report", colNamesToReplace);
        }

        public void GetFuelTheftReportEp(GridSettings grid, DateTime fromDate, DateTime toDate, string region,
            string city, string site, string resource)
        {
            var fuelTheftDetails = GetRptFuelTheftDetails(fromDate, toDate, region, city, site, resource);
            new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.LeftToRight);
                doc.Orientation(PageOrientation.Landscape);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata { Author = "Bnf Tech", Application = "PdfRpt", Keywords = "Fuel Theft Report", Subject = "Fuel Theft Report", Title = "Fuel Theft" });
                doc.Compression(new CompressionSettings
                {
                    EnableCompression = true,
                    EnableFullCompression = true
                });
            })
            .DefaultFonts(fonts =>
            {
                fonts.Path(Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\arial.ttf"),
                           Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\verdana.ttf"));
                fonts.Size(9);
                //fonts.Color(System.Drawing.Color.Black);
            })
            .PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd/MM/yyyy"));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    //Asif defaultHeader.ImagePath(System.IO.Path.Combine(AppPath.ApplicationPath, "Content\\Images\\01.png"));
                    defaultHeader.Message("Fuel Theft Report");
                });
            })
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.ClassicTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
            })
            .MainTableDataSource(dataSource =>
            {
                dataSource.StronglyTypedList(fuelTheftDetails);
            })
            .MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings("Summary");
                summarySettings.PreviousPageSummarySettings("Previous Page Summary");
                summarySettings.PageSummarySettings("Page Summary");
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.PropertyName<RptFuelTheftDetail>(x => x.Id);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1);
                    column.HeaderCell("Batch No.");
                });
                
                columns.AddColumn(column =>
                {
                    column.PropertyName<RptFuelTheftDetail>(x => x.SiteName);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(2);
                    column.HeaderCell("Site Name", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptFuelTheftDetail>(x => x.AssetName);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("Asset Name", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptFuelTheftDetail>(x => x.RefuelType);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Event Desc", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptFuelTheftDetail>(x => x.OpenDt);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(1);
                    column.HeaderCell("Event Start");
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : ((DateTime)obj).ToString("yyyy-MM-dd"));
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptFuelTheftDetail>(x => x.EndDt);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(1);
                    column.HeaderCell("Event Stop");
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : ((DateTime)obj).ToString("yyyy-MM-dd"));
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptFuelTheftDetail>(x => x.TotDuration);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width(1);
                    column.HeaderCell("Total Duration", horizontalAlignment: HorizontalAlignment.Center);
                    column.AggregateFunction(aggregateFunction => aggregateFunction.CustomAggregateFunction(new CustomSum()));
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptFuelTheftDetail>(x => x.Decrease);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(8);
                    column.Width(1);
                    column.HeaderCell("Fuel Theft(Ltrs)", horizontalAlignment: HorizontalAlignment.Center);
                    column.AggregateFunction(aggregateFunction => aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum));
                });
            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "There is no data available to display.");
            })
            .Export(export =>
            {
                export.ToExcel();
                export.ToCsv();
                export.ToXml();
            })
            .Generate(data =>
            {
                var fileName = "Fuel Theft Report.pdf";
                fileName = HttpUtility.UrlEncode(fileName, Encoding.UTF8);
                data.FlushInBrowser(fileName, FlushType.Attachment);
            }); // creating an in-memory PDF file
        }

        public dynamic GetAssetOperationReport(GridSettings grid, string sidx, string sord, int page, int rows, DateTime fromDate, DateTime toDate, string region, string city, string site, string resource, bool dgStatus)
        {
            //exec SP_Rpt_AssetOperation @fromdate = '2016-03-01',@todate = '2016-03-03',@Region = '%',@City = '%',@Territory = '%',@SiteCode = '1814',@Orgcode = 33,@Resource = '131',@EventType = 'N',@levelTYpe = 'N'
            if (string.IsNullOrEmpty(city)) city = "%";
            if (string.IsNullOrEmpty(site)) site = "%";
            if (string.IsNullOrEmpty(resource)) resource = "%";
            if (string.IsNullOrEmpty(region)) region = "%";

            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;

            var siteCode = GetSiteCodeFromFleetUnitsViewByTitle(site, orgCode);
                

            var assetOperations = _session.GetNamedQuery("GetRptAssetOperation")
                .SetDateTime("FromDt", fromDate).SetDateTime("ToDt", toDate)
                .SetString("Region", region)
                .SetString("City", city)
                .SetString("Territory", "%")
                .SetString("Site", siteCode == 0 ? "%" : siteCode.ToString())
                .SetString("Orgcode", orgCode)
                .SetString("Resource", resource)
                .SetString("EventType", (dgStatus ? "C": "N"))
                .SetString("LevelType", (dgStatus ? "D" : "N"))
                .SetTimeout(0)
                .List<RptAssetOperation>();

            int totalRecords = assetOperations.Count();

            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = assetOperations
            };
        }

        public void GetAssetOperationReportEe(GridSettings grid, DateTime fromDate, DateTime toDate, string region,
            string city, string site, string resource, bool dgStatus)
        {
            if (string.IsNullOrEmpty(city)) city = "%";
            if (string.IsNullOrEmpty(site)) site = "%";
            if (string.IsNullOrEmpty(resource)) resource = "%";
            if (string.IsNullOrEmpty(region)) region = "%";

            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;

            var siteCode = GetSiteCodeFromFleetUnitsViewByTitle(site, orgCode);
                
            var assetOperations = _session.GetNamedQuery("GetRptAssetOperation")
                .SetDateTime("FromDt", fromDate).SetDateTime("ToDt", toDate)
                .SetString("Region", region)
                .SetString("City", city)
                .SetString("Territory", "%")
                .SetString("Site", siteCode == 0 ? "%" : siteCode.ToString())
                .SetString("Orgcode", orgCode)
                .SetString("Resource", resource)
                .SetString("EventType", (dgStatus ? "C" : "N"))
                .SetString("LevelType", (dgStatus ? "D" : "N"))
                .List<RptAssetOperation>();

            string[,] colNamesToReplace = new string[9, 2];

            colNamesToReplace[0, 0] = "Id";
            colNamesToReplace[0, 1] = "Batch No.";

            colNamesToReplace[1, 0] = "RegionName";
            colNamesToReplace[1, 1] = "Region Name";

            colNamesToReplace[2, 0] = "CityName";
            colNamesToReplace[2, 1] = "City Name";

            colNamesToReplace[3, 0] = "SiteName";
            colNamesToReplace[3, 1] = "Site Name";

            colNamesToReplace[4, 0] = "AssetName";
            colNamesToReplace[4, 1] = "Asset Name";

            colNamesToReplace[5, 0] = "OpenDt";
            colNamesToReplace[5, 1] = "Event Start";

            colNamesToReplace[6, 0] = "CloseDt";
            colNamesToReplace[6, 1] = "Event Stop";

            colNamesToReplace[7, 0] = "AssetTotalDuration";
            colNamesToReplace[7, 1] = "Total Duration";

            colNamesToReplace[8, 0] = "Consumption";
            colNamesToReplace[8, 1] = "Consumption (Ltrs)";

            ExportFilesByHttpResponse.ExportToExcel(assetOperations.AsQueryable(),
                row => new
                {
                    row.Id,
                    row.RegionName,
                    row.CityName,
                    row.SiteName,
                    row.AssetName,
                    row.OpenDt,
                    row.CloseDt,
                    row.AssetTotalDuration,
                    row.Consumption
                }, null, "Asset Activity Report", colNamesToReplace);
        }

        public dynamic GetGridPowerOperationReport(GridSettings grid, string sidx, string sord, int page, int rows, DateTime fromDate, DateTime toDate, string region, string city, string site, string resource, bool status)
        {
            //exec SP_Rpt_GridPower_OPeration @fromdate = '2016-12-01',@todate = '2016-12-01',@Region = '%',@City = '%',@Territory = '%',@SiteCode = '1855',@Orgcode = 33,@Resource = '133',@Status = '1'
            if (string.IsNullOrEmpty(city)) city = "%";
            if (string.IsNullOrEmpty(site)) site = "%";
            if (string.IsNullOrEmpty(resource)) resource = "%";
            if (string.IsNullOrEmpty(region)) region = "%";

            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;

            var siteCode = GetSiteCodeFromFleetUnitsViewByTitle(site, orgCode);


            var assetOperations = _session.GetNamedQuery("GetRptGridPowerOperation")
                .SetDateTime("FromDt", fromDate).SetDateTime("ToDt", toDate)
                .SetString("Region", region)
                .SetString("City", city)
                .SetString("Territory", "%")
                .SetString("Site", siteCode == 0 ? "%" : siteCode.ToString())
                .SetString("Orgcode", orgCode)
                .SetString("Resource", resource)
                .SetString("Status", (Convert.ToByte(status).ToString()))
                .List<RptGridPowerOperation>();

            int totalRecords = assetOperations.Count();

            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = assetOperations
            };
        }

        public void GetGridPowerOperationReportEe(GridSettings grid, DateTime fromDate, DateTime toDate, string region,
            string city, string site, string resource, bool status)
        {
            if (string.IsNullOrEmpty(city)) city = "%";
            if (string.IsNullOrEmpty(site)) site = "%";
            if (string.IsNullOrEmpty(resource)) resource = "%";
            if (string.IsNullOrEmpty(region)) region = "%";

            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;

            var siteCode = GetSiteCodeFromFleetUnitsViewByTitle(site, orgCode);
                
            var assetOperations = _session.GetNamedQuery("GetRptGridPowerOperation")
                .SetDateTime("FromDt", fromDate).SetDateTime("ToDt", toDate)
                .SetString("Region", region)
                .SetString("City", city)
                .SetString("Territory", "%")
                .SetString("Site", siteCode == 0 ? "%" : siteCode.ToString())
                .SetString("Orgcode", orgCode)
                .SetString("Resource", resource)
                .SetString("Status", (Convert.ToByte(status).ToString()))
                .List<RptGridPowerOperation>();

            string[,] colNamesToReplace = new string[9, 2];

            colNamesToReplace[0, 0] = "GridBatchNo";
            colNamesToReplace[0, 1] = "Batch No.";

            colNamesToReplace[1, 0] = "RegionName";
            colNamesToReplace[1, 1] = "Region Name";

            colNamesToReplace[2, 0] = "CityName";
            colNamesToReplace[2, 1] = "City Name";

            colNamesToReplace[3, 0] = "SiteName";
            colNamesToReplace[3, 1] = "Site Name";

            colNamesToReplace[4, 0] = "AssetName";
            colNamesToReplace[4, 1] = "Asset Name";

            colNamesToReplace[5, 0] = "OpenDt";
            colNamesToReplace[5, 1] = "Event Start";

            colNamesToReplace[6, 0] = "CloseDt";
            colNamesToReplace[6, 1] = "Event Stop";

            colNamesToReplace[7, 0] = "TotalDuration";
            colNamesToReplace[7, 1] = "Total Duration";

            ExportFilesByHttpResponse.ExportToExcel(assetOperations.AsQueryable(),
                row => new
                {
                    row.Id.GridBatchNo,
                    row.RegionName,
                    row.CityName,
                    row.SiteName,
                    row.AssetName,
                    row.OpenDt,
                    row.CloseDt,
                    row.Id.TotalDuration,
                }, null, "Grid Activity Report", colNamesToReplace);
        }

        private double GetSiteCodeFromFleetUnitsViewByTitle(string site, string orgCode)
        {
            var siteCode = _session.QueryOver<FleetUnitsView>()
                .Select(Projections.Distinct(Projections.Property<FleetUnitsView>(x => x.SiteCode)))
                .Where(x => x.OrgCode == orgCode && x.Title == site).SingleOrDefault<double>();
            return siteCode;
        }

        public dynamic GetGridPowerAssetOperationReport(GridSettings grid, string sidx, string sord, int page, int rows, DateTime fromDate, DateTime toDate, string region, string city, string site, string resource, bool status)
        {
            if (string.IsNullOrEmpty(city)) city = "%";
            if (string.IsNullOrEmpty(site)) site = "%";
            if (string.IsNullOrEmpty(resource)) resource = "%";
            if (string.IsNullOrEmpty(region)) region = "%";

            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;

            var siteCode = GetSiteCodeFromFleetUnitsViewByTitle(site, orgCode);
                
            var assetOperations = _session.GetNamedQuery("GetRptGridPowerAssetOperation")
                .SetDateTime("FromDt", fromDate).SetDateTime("ToDt", toDate)
                .SetString("Region", region)
                .SetString("City", city)
                .SetString("Territory", "%")
                .SetString("Site", siteCode == 0 ? "%" : siteCode.ToString())
                .SetString("Orgcode", orgCode)
                .SetString("Resource", resource)
                .SetString("Status", (Convert.ToByte(status).ToString()))
                .SetString("LevelType", status ? "C" : "N")
                .SetTimeout(0)
                .List<RptGridPowerAssetOperation>();

            int totalRecords = assetOperations.Count();

            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = assetOperations
            };
        }

        public dynamic GetMonthwiseStatisticsReport(GridSettings grid, string sidx, string sord, int page, int rows, string month, string region, string city, string site, string resource, string engineMake, string rating)
        {
            if (string.IsNullOrEmpty(city)) city = "%";
            if (string.IsNullOrEmpty(site)) site = "%";
            if (string.IsNullOrEmpty(resource)) resource = "%";
            if (string.IsNullOrEmpty(region)) region = "%";
            if (string.IsNullOrEmpty(rating)) rating = "%";

            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;

            var siteCode = GetSiteCodeFromFleetUnitsViewByTitle(site, orgCode);
                
            var monthwiseStatistics = _session.GetNamedQuery("GetRptMonthwiseStatistics")
                .SetString("Month", month)
                .SetString("Region", region)
                .SetString("City", city)
                .SetString("Site", siteCode == 0 ? "%" : siteCode.ToString())
                .SetString("Orgcode", orgCode)
                .SetString("Resource", resource)
                .SetString("EngineUMake",  (! engineMake.IsNullOrEmpty() ? "%" + engineMake + "%" : "%"))
                .SetString("DgCap", rating)
                .SetTimeout(0)
                .List<RptMonthwiseStatistics>();

            int totalRecords = monthwiseStatistics.Count();

            var pageSize = rows;
            var totalPages = (int) Math.Ceiling((float) totalRecords/pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = monthwiseStatistics
            };
        }

        public IList<MonthWiseSummary> GetNoOfMonthsDashboard(string unitId, short month, short year, short noOfMonths)
        {
            MonthWiseSummary monthWiseSummary = null;
            var toDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            var fromDate = toDate.AddMonths(-noOfMonths).AddDays(1);

            return _repositoryEdrFuelRun.QueryOver()
                .Where(x => x.UnitID == unitId)
                .And(Expression.Sql("DATEADD(m, DATEDIFF(m, 0, opendt), 0) >= ?", fromDate, NHibernateUtil.DateTime))
                .And(Expression.Sql("DATEADD(m, DATEDIFF(m, 0, opendt), 0) < ?", toDate, NHibernateUtil.DateTime))

                .SelectList(list => list
                    .SelectGroup(x => x.UnitID).WithAlias(() => monthWiseSummary.CompRunHr)
                    .Select(Projections.SqlGroupProjection("DATEADD(m, DATEDIFF(m, 0, opendt), 0) as opendt",
                        "DATEADD(m, DATEDIFF(m, 0, opendt), 0) ", new[] {"opendt"},
                        new IType[] {NHibernateUtil.Date})).WithAlias(() => monthWiseSummary.OpenDate)
                    .Select(Projections.Sum(
                        Projections.Conditional(
                            Restrictions.Eq(Projections.Property<EDRFuelRun>(x => x.EventType), "R"),
                            Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"),
                                NHibernateUtil.Double,
                                Projections.Property<EDRFuelRun>(x => x.NQty),
                                Projections.Property<EDRFuelRun>(x => x.BalanceQty)), Projections.Constant(0.0))))
                    .WithAlias(() => monthWiseSummary.Refuel)
                    .Select(Projections.Sum(
                        Projections.Conditional(
                            Restrictions.Eq(Projections.Property<EDRFuelRun>(x => x.EventType), "C"),
                            Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"),
                                NHibernateUtil.Double,
                                Projections.Property<EDRFuelRun>(x => x.BalanceQty),
                                Projections.Property<EDRFuelRun>(x => x.NQty)), Projections.Constant(0.0))))
                    .WithAlias(() => monthWiseSummary.Consume)
                    .Select(Projections.Sum(
                        Projections.Conditional(
                            Restrictions.Eq(Projections.Property<EDRFuelRun>(x => x.EventType), "T"),
                            Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"),
                                NHibernateUtil.Double,
                                Projections.Property<EDRFuelRun>(x => x.BalanceQty),
                                Projections.Property<EDRFuelRun>(x => x.NQty)), Projections.Constant(0.0))))
                    .WithAlias(() => monthWiseSummary.Theft)
                    .Select(Projections.Sum(
                        Projections.Conditional(
                            Restrictions.Eq(Projections.Property<EDRFuelRun>(x => x.EventType), "C"),
                            Projections.SqlFunction(new VarArgsSQLFunction("(", "+", ")"), NHibernateUtil.Double,
                                Projections.Property<EDRFuelRun>(x => x.DurationSs),
                                Projections.SqlFunction(new VarArgsSQLFunction("((", "*3600)+(", "*60))"),
                                    NHibernateUtil.Double,
                                    Projections.Property<EDRFuelRun>(x => x.DurationHh),
                                    Projections.Property<EDRFuelRun>(x => x.DurationMi))),
                            Projections.Constant(0, NHibernateUtil.Double))))
                    .WithAlias(() => monthWiseSummary.TotalMileage)
                    .Select(Projections.SqlFunction("FN_GetOpenFuelBalance", NHibernateUtil.Double,
                        Projections.Property<EDRFuelRun>(x => x.UnitID),
                        Projections.SqlFunction(new SQLFunctionTemplate(NHibernateUtil.DateTime, "DateAdd(m," +
                                                                                                 new SQLFunctionTemplate
                                                                                                     (
                                                                                                     NHibernateUtil
                                                                                                         .DateTime,
                                                                                                     "DateDiff(m, 0, opendt)") +
                                                                                                 ",0)"),
                            NHibernateUtil.DateTime))).WithAlias(() => monthWiseSummary.OpenningMile)
                    .Select(Projections.SqlFunction("FN_GetCloseFuelBalance", NHibernateUtil.Double,
                        Projections.Property<EDRFuelRun>(x => x.UnitID),
                        Projections.SqlFunction(new SQLFunctionTemplate(NHibernateUtil.DateTime, "DateAdd(m," +
                                                                                                 new SQLFunctionTemplate
                                                                                                     (
                                                                                                     NHibernateUtil
                                                                                                         .DateTime,
                                                                                                     "DateDiff(m, 0, opendt)") +
                                                                                                 ",0)"),
                            NHibernateUtil.DateTime))).WithAlias(() => monthWiseSummary.Closing)
                ).TransformUsing(Transformers.AliasToBean<MonthWiseSummary>())
                .UnderlyingCriteria.SetTimeout(0)
                .List<MonthWiseSummary>();
        }
    }
}