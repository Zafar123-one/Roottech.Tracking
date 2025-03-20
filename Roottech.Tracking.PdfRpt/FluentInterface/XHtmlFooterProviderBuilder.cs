using System;
using Roottech.Tracking.PdfRpt.Core.Contracts;
using Roottech.Tracking.PdfRpt.FooterTemplates;

namespace Roottech.Tracking.PdfRpt.FluentInterface
{
    /// <summary>
    /// Defines dynamic footer of the pages by using iTextSharp's HTML to PDF capabilities (XmlWorker class).
    /// </summary>
    public class XHtmlFooterProviderBuilder
    {
        private readonly XHtmlFooterProvider _builder = new XHtmlFooterProvider();

        internal XHtmlFooterProvider HtmlFooterProvider
        {
            get { return _builder; }
        }

        /// <summary>
        /// Properties of page footers.
        /// </summary>
        public void PageFooterProperties(XFooterBasicProperties properties)
        {
            _builder.FooterProperties = properties;
        }

        /// <summary>
        /// Returns dynamic HTML content of the page footer.
        /// </summary>
        public void AddPageFooter(Func<FooterData, string> footer)
        {
            _builder.AddPageFooter = footer;
        }
    }
}