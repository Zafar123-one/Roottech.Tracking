﻿using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Roottech.Tracking.PdfRpt.Core.Contracts;
using Roottech.Tracking.PdfRpt.Core.Helper;
using Roottech.Tracking.PdfRpt.Core.Helper.HtmlToPdf;

namespace Roottech.Tracking.PdfRpt.FooterTemplates
{
    /// <summary>
    /// Defines dynamic footer of the pages by using iTextSharp's limited HTML to PDF capabilities (HTMLWorker class).
    /// </summary>
    public class XHtmlFooterProvider : IPageFooter
    {
        // we will put the final number of pages in a template
        PdfTemplate _totalPageCountTemplate;
        Image _totalPageCountImage;

        /// <summary>
        /// Properties of page footers.
        /// </summary>
        public XFooterBasicProperties FooterProperties { set; get; }

        /// <summary>
        /// Returns dynamic HTML content of the page footer.
        /// </summary>
        public Func<FooterData, string> AddPageFooter { set; get; }

        /// <summary>
        /// Fires before closing the document
        /// </summary>
        /// <param name="writer">PdfWriter</param>
        /// <param name="document">PDF Document</param>
        /// <param name="columnCellsSummaryData">List of all rows summaries data</param>
        public void ClosingDocument(PdfWriter writer, Document document, IList<SummaryCellData> columnCellsSummaryData)
        {
            setFinalPageNumber(writer);
        }

        /// <summary>
        /// Fires when a page is finished, just before being written to the document.
        /// </summary>
        /// <param name="writer">PdfWriter</param>
        /// <param name="document">PDF Document</param>
        /// <param name="columnCellsSummaryData">List of all rows summaries data</param>
        public void PageFinished(PdfWriter writer, Document document, IList<SummaryCellData> columnCellsSummaryData)
        {
            var pageFooterHtml = AddPageFooter(new FooterData
            {
                PdfDoc = document,
                PdfWriter = writer,
                SummaryData = columnCellsSummaryData,
                CurrentPageNumber = writer.PageNumber,
                TotalPagesCountImage = _totalPageCountImage
            });
            var table = createTable(pageFooterHtml);

            var page = document.PageSize;
            table.SetTotalWidth(new[] { page.Width - document.LeftMargin - document.RightMargin });
            table.WriteSelectedRows(
                    rowStart: 0,
                    rowEnd: -1,
                    xPos: document.LeftMargin,
                    yPos: document.BottomMargin - FooterProperties.SpacingBeforeTable,
                    canvas: writer.DirectContent);
        }

        /// <summary>
        /// Fires when the document is opened.
        /// </summary>
        /// <param name="writer">PdfWriter</param>
        /// <param name="columnCellsSummaryData">List of all rows summaries data</param>
        public void DocumentOpened(PdfWriter writer, IList<SummaryCellData> columnCellsSummaryData)
        {
            initTemplate(writer);
        }

        private PdfGrid createTable(string html)
        {
            var table = new PdfGrid(1)
            {
                RunDirection = (int)FooterProperties.RunDirection,
                WidthPercentage = FooterProperties.TableWidthPercentage
            };
            var htmlCell = new XmlWorkerHelper
            {
                Html = html,
                RunDirection = FooterProperties.RunDirection,
                InlineCss = FooterProperties.InlineCss,
                ImagesPath = FooterProperties.ImagesPath,
                CssFilesPath = FooterProperties.CssFilesPath,
                PdfElement = _totalPageCountImage,
                DefaultFont = FooterProperties.PdfFont.Fonts[0]
            }.RenderHtml();            
            htmlCell.Border = 0;
            table.AddCell(htmlCell);

            if (FooterProperties.ShowBorder)
                return table.AddBorderToTable(FooterProperties.BorderColor, FooterProperties.SpacingBeforeTable);
            table.SpacingBefore = this.FooterProperties.SpacingBeforeTable;

            return table;
        }

        private void initTemplate(PdfWriter writer)
        {
            _totalPageCountTemplate = writer.DirectContent.CreateTemplate(FooterProperties.TotalPagesCountTemplateWidth, FooterProperties.TotalPagesCountTemplateHeight);
            _totalPageCountImage = Image.GetInstance(_totalPageCountTemplate);
        }

        private void setFinalPageNumber(PdfWriter writer)
        {
            if (FooterProperties.PdfFont == null)
                throw new NullReferenceException("PdfFont is null.");

            var font = FooterProperties.PdfFont.Fonts[0];
            var text = "" + (writer.PageNumber - 1);
            var textLen = font.BaseFont.GetWidthPoint(text, font.Size);
            var x = FooterProperties.RunDirection == PdfRunDirection.LeftToRight ?
                     0 : FooterProperties.TotalPagesCountTemplateWidth - textLen;

            _totalPageCountTemplate.BeginText();
            _totalPageCountTemplate.SetFontAndSize(font.BaseFont, font.Size);
            _totalPageCountTemplate.SetTextMatrix(x, 0);
            _totalPageCountTemplate.ShowText(text);
            _totalPageCountTemplate.EndText();
        }
    }
}