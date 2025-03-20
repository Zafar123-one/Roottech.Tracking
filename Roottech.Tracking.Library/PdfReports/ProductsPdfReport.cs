using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Roottech.Tracking.Domain.RSSP.Entities;
using Roottech.Tracking.Library.Utils;
using Roottech.Tracking.PdfRpt.Core.Contracts;
using Roottech.Tracking.PdfRpt.FluentInterface;

namespace Roottech.Tracking.Library.PdfReports
{
    public class ProductsPdfReport
    {
        public IPdfReportData CreatePdfReport(IList<RptGenRefuel> dataSourceList)
        {
            return new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.LeftToRight);
                doc.Orientation(PageOrientation.Landscape);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata { Author = "Bnf Tech", Application = "PdfRpt", Keywords = "Fuel Intake Report", Subject = "Fuel Intake Report", Title = "Fuel Intake" });
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
                    defaultHeader.Message("Fuel Intake Report");
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
                dataSource.StronglyTypedList(dataSourceList);
            })
            .MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings("Summary");
                summarySettings.PreviousPageSummarySettings("Previous Page Summary");
                summarySettings.PageSummarySettings("Page Summary");
            })
            .MainTableColumns(columns =>
            {
                //columns.AddColumn(column =>
                //{
                //    column.PropertyName("rowNo");
                //    column.IsRowNumber(true);
                //    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                //    column.IsVisible(true);
                //    column.Order(0);
                //    column.Width(1);
                //    column.HeaderCell("#");
                //});

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenRefuel>(x => x.Id);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1);
                    column.HeaderCell("Batch No.");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenRefuel>(x => x.SiteName);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(2);
                    column.HeaderCell("Site Name", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenRefuel>(x => x.AssetName);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("Asset Name", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenRefuel>(x => x.RefuelType);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Event Desc", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenRefuel>(x => x.OpenDt);
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
                    column.PropertyName<RptGenRefuel>(x => x.EndDt);
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
                    column.PropertyName<RptGenRefuel>(x => x.TotDuration);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width(1);
                    column.HeaderCell("Total Duration", horizontalAlignment: HorizontalAlignment.Center);
                    column.AggregateFunction(aggregateFunction => aggregateFunction.CustomAggregateFunction(new CustomSum()));
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RptGenRefuel>(x => x.Increase);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(8);
                    column.Width(1);
                    column.HeaderCell("Refueled(Ltrs)", horizontalAlignment: HorizontalAlignment.Center);
                    column.AggregateFunction(aggregateFunction => aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum));
                });


/*                columns.AddColumn(column =>
                {
                    column.PropertyName<Product>(x => x.Price);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(2);
                    column.HeaderCell("Price");
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                    });
                    column.AggregateFunction(aggregateFunction =>
                    {
                        aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
                        aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                    });
                });*/

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
                //var fileName = string.Format("{0}-products-rpt.pdf", Guid.NewGuid().ToString("N"));
                var fileName = "Fuel Intake Report.pdf";
                fileName = HttpUtility.UrlEncode(fileName, Encoding.UTF8);
                data.FlushInBrowser(fileName, FlushType.Attachment);
            }); // creating an in-memory PDF file
        }
    }
}