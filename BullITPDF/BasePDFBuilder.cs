using System;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;

namespace BullITPDF
{
    public class BasePDFBuilder
    {
        static bool IsFontResolverSet = false;
        protected readonly bool _swapPages;
        protected PdfDocument Document {get; set;}
        protected readonly XSize _pageSize;
        private int _currentPageIndex;
        private XPen _line;
        protected BasePDFBuilder(FontResolver fontResolver, double width, double height, bool swapPages = true)
        {
            if (!IsFontResolverSet)
            {
                IsFontResolverSet = true;
                GlobalFontSettings.FontResolver = fontResolver;
            }
            _swapPages = swapPages;
            _pageSize = new XSize()
            {
                Height = XUnit.FromCentimeter(height),
                Width = XUnit.FromCentimeter(width)
            };
            _currentPageIndex = 0;
        }
        protected void AddStringToPDF(string content, XGraphics gfx, double left, double top, int fontSize=10)
        {
            if (string.IsNullOrEmpty(content))
                return;
            XFont font = new XFont("Calibri", fontSize);
            XPoint point = new XPoint(XUnit.FromCentimeter(left), XUnit.FromCentimeter(top));
            gfx.DrawString(content, font, XBrushes.Black, point);
        }
        protected void AddStringToPDF(string content, XGraphics gfx, double left, double top,double width, double height,int fontSize=10)
        {
            if (string.IsNullOrEmpty(content))
                return;
            XFont font = new XFont("Calibri", fontSize);
            XRect rectangle = new XRect(XUnit.FromCentimeter(left),  XUnit.FromCentimeter(top), XUnit.FromCentimeter(width) , XUnit.FromCentimeter(height));
            gfx.DrawString(content, font, XBrushes.Black,rectangle,XStringFormats.TopCenter);
        }
        protected void CreateGrid(XGraphics gfx)
        {
            XFont font = new XFont("Calibri", 10);
            _line = new XPen(XColors.Red, XUnit.FromMillimeter(0.2));
            for(var i=0;i<XUnit.FromCentimeter(_pageSize.Width);i+=1)
            {
                gfx.DrawLine(_line, 0, XUnit.FromCentimeter(i+1), XUnit.FromCentimeter(_pageSize.Width), XUnit.FromCentimeter(i+1)); 
                gfx.DrawString(i.ToString(), font,XBrushes.Black,0, XUnit.FromCentimeter(i+1));
            }
            for(var i=0;i<XUnit.FromCentimeter(_pageSize.Height);i+=1)
            {
                gfx.DrawLine(_line, XUnit.FromCentimeter(i+1), 0, XUnit.FromCentimeter(i+1),XUnit.FromCentimeter(_pageSize.Height)); 
                gfx.DrawString(i.ToString(), font,XBrushes.Black,XUnit.FromCentimeter(i), XUnit.FromCentimeter(_pageSize.Height));
            }
        }
        protected XGraphics CreateNextPage(bool buildWithBackground)
        {
            PdfPage page;
            
            if (buildWithBackground)
            {
                page = Document.Pages[_currentPageIndex];
                _currentPageIndex++;
            }
            else
            {
                page = Document.AddPage();
                page.Orientation = PageOrientation.Landscape;
                page.Width = _pageSize.Width;
                page.Height = _pageSize.Height;
            }
            XGraphics gfx = XGraphics.FromPdfPage(page);
            return gfx;
        }
        protected static string AddOrdinalsToNumber(int? pedigreeGenerationDay)
        {
            if (pedigreeGenerationDay == null)
            {
                return string.Empty;
            }
            switch (pedigreeGenerationDay.Value)
            {
                case 1:
                    return pedigreeGenerationDay + "st";
                case 2:
                    return pedigreeGenerationDay + "nd";
                case 3:
                    return pedigreeGenerationDay + "rd";
                default:
                    return pedigreeGenerationDay + "th";
            }
        }
    }    
}

        