﻿using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Roottech.Tracking.PdfRpt.Core.Helper.HtmlToPdf
{
    /// <summary>
    /// XmlWorker's Unicode Font Provider class.
    /// </summary>
    public class UnicodeFontProvider : FontFactoryImp
    {
        private readonly Font _defaultFont;
        /// <summary>
        /// XmlWorker's Unicode Font Provider class.
        /// </summary>
        /// <param name="defaultFont">a missing fonts provider</param>
        public UnicodeFontProvider(Font defaultFont)
        {
            _defaultFont = defaultFont;
        }

        /// <summary>
        /// Provides a font with BaseFont.IDENTITY_H encoding.
        /// </summary>
        public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color, bool cached)
        {
            return FontFactory.GetFont(
                string.IsNullOrEmpty(fontname) ? _defaultFont.Familyname : fontname,
                BaseFont.IDENTITY_H, BaseFont.EMBEDDED, size, style, color);
        }
    }
}