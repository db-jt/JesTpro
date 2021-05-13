// This file is part of JesTpro project.
//
// JesTpro is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (if needed) any later version.
//
// JesTpro has NO WARRANTY!! It is distributed for test, study or 
// personal environments. Any commercial distribution
// has no warranty! 
// See the GNU General Public License in root project folder  
// for more details or  see <http://www.gnu.org/licenses/>

using DinkToPdf;
using DinkToPdf.Contracts;
using jt.jestpro.Helpers;
using jt.jestpro.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jt.jestpro.Services
{
    public class DinkToPDF : IHtmlToPDF
    {
        private readonly IConverter _converter;

        public DinkToPDF(IConverter converter)
        {
            _converter = converter;
        }

        public async Task<string> CreateReport(string html, string fileName, PdfSettings currentConf, string staticResourcePath = "")
        {
            var savePath = fileName.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase) ? System.IO.Path.Combine(currentConf.DefaultPath, fileName) : System.IO.Path.Combine(currentConf.DefaultPath, $"{fileName}.pdf");
            //MARGINS
            var mSettings = new MarginSettings() { Top = 10 };
            if (currentConf.Margin != null && currentConf.Margin.Length == 4)
            {
                mSettings.Top = currentConf.Margin[0];
                mSettings.Right = currentConf.Margin[1];
                mSettings.Bottom = currentConf.Margin[2];
                mSettings.Left = currentConf.Margin[3];
            }
            //ORIENTATION (default is portrait)
            var orientation = Orientation.Portrait;
            if (currentConf.Orientation != null && currentConf.Orientation.Equals("landscape", StringComparison.InvariantCultureIgnoreCase))
            {
                orientation = Orientation.Landscape;
            }
            //PAPER FORMAT
            var paperSize = PaperKind.A4;
            if (currentConf.PdfPaperSize != null)
            {
                if (Enum.TryParse(currentConf.PdfPaperSize, out PaperKind pSize))
                    paperSize = pSize;
                else
                    throw new Exception($"PdfFormat {currentConf.PdfPaperSize} is not valid! Please check tempalte configuration");
            }

            //var converter = new SynchronizedConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = orientation,
                    PaperSize = paperSize,
                    Margins = mSettings,
                    Out = $@"{savePath}",
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = $@"{html}",
                        WebSettings = {
                            DefaultEncoding = "utf-8",
                            UserStyleSheet = staticResourcePath,
                        },
                        HeaderSettings = {
                            FontSize = 1,
                            Right = "",
                            Line = false,
                            Spacing = 1
                        },
                    }
                }
            };
            _converter.Convert(doc);
            return savePath;
        }
    }
}
